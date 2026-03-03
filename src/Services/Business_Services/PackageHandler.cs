using Microsoft.EntityFrameworkCore;
using CHKS.Data;
using CHKS.Entity;
using CHKS.Models;

namespace CHKS.Services;

/// <summary>
/// Service for managing product packages and applying them to carts.
/// </summary>
public class PackageService(
    IDbContextFactory<Rardi_Context> contextFactory,
    CartControlService cartControl,
    InventoryControlService inventoryControl,
    ILogger<PackageService> logger)
{
    private readonly Rardi_Context _context = contextFactory.CreateDbContext();

    // ── Package CRUD ──────────────────────────────────────────────────────────

    public async Task<List<Package>> GetPackages()
    {
        return await _context.Packages
            .AsNoTracking()
            .Include(p => p.PackageItems)
                .ThenInclude(pi => pi.Product)
            .Select(p => new Package(
                p.Id,
                p.Name,
                p.Description,
                p.PackageItems.Select(pi => new PackageItem(
                    pi.Id,
                    pi.PackageId,
                    pi.ProductId,
                    pi.Product.Name,
                    pi.Product.Export,
                    pi.Quantity
                ))
            ))
            .ToListAsync();
    }

    public async Task<Package?> GetPackage(Guid id)
    {
        return await _context.Packages
            .AsNoTracking()
            .Include(p => p.PackageItems)
                .ThenInclude(pi => pi.Product)
            .Where(p => p.Id == id)
            .Select(p => new Package(
                p.Id,
                p.Name,
                p.Description,
                p.PackageItems.Select(pi => new PackageItem(
                    pi.Id,
                    pi.PackageId,
                    pi.ProductId,
                    pi.Product.Name,
                    pi.Product.Export,
                    pi.Quantity
                ))
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<Package> CreatePackage(CreatePackageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Package name cannot be empty.", nameof(request));

        var model = new Package_Model
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? ""
        };

        try
        {
            await _context.Packages.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create package {Name}", request.Name);
            throw new InvalidOperationException("Failed to create package.", ex);
        }

        return new Package(model.Id, model.Name, model.Description, []);
    }

    public async Task UpdatePackage(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Package name cannot be empty.", nameof(name));

        await _context.Packages
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, name.Trim())
                .SetProperty(p => p.Description, description?.Trim() ?? ""));
    }

    public async Task DeletePackage(Guid id)
    {
        await _context.Packages.Where(p => p.Id == id).ExecuteDeleteAsync();
    }

    // ── Package Items ─────────────────────────────────────────────────────────

    public async Task<PackageItem> AddItemToPackage(AddPackageItemRequest request)
    {
        if (request.Quantity < 1)
            throw new ArgumentException("Quantity must be at least 1.", nameof(request));

        // If the same product already exists in the package, update its quantity
        var existing = await _context.PackageItems
            .FirstOrDefaultAsync(pi => pi.PackageId == request.PackageId && pi.ProductId == request.ProductId);

        if (existing is not null)
        {
            await _context.PackageItems
                .Where(pi => pi.Id == existing.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Quantity, request.Quantity));

            var product = await _context.Inventory.AsNoTracking()
                .FirstAsync(p => p.Id == request.ProductId);
            return new PackageItem(existing.Id, existing.PackageId, existing.ProductId, product.Name, product.Export, request.Quantity);
        }

        var model = new PackageItem_Model
        {
            PackageId = request.PackageId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        try
        {
            await _context.PackageItems.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add product {ProductId} to package {PackageId}", request.ProductId, request.PackageId);
            throw new InvalidOperationException("Failed to add item to package.", ex);
        }

        var prod = await _context.Inventory.AsNoTracking().FirstAsync(p => p.Id == request.ProductId);
        return new PackageItem(model.Id, model.PackageId, model.ProductId, prod.Name, prod.Export, model.Quantity);
    }

    public async Task RemoveItemFromPackage(Guid packageItemId)
    {
        await _context.PackageItems.Where(pi => pi.Id == packageItemId).ExecuteDeleteAsync();
    }

    public async Task UpdateItemQuantity(Guid packageItemId, int quantity)
    {
        if (quantity < 1) throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));
        await _context.PackageItems
            .Where(pi => pi.Id == packageItemId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Quantity, quantity));
    }

    // ── Apply package to cart ─────────────────────────────────────────────────

    /// <summary>
    /// Applies a package to a cart. Returns the list of items that were skipped due to insufficient stock.
    /// Caller should check the returned list and decide whether to continue or cancel.
    /// </summary>
    public async Task<List<PackageItem>> GetOutOfStockItems(Guid packageId)
    {
        var package = await GetPackage(packageId)
            ?? throw new ArgumentException("Package not found.", nameof(packageId));

        var outOfStock = new List<PackageItem>();
        foreach (var item in package.Items)
        {
            if (!await inventoryControl.IsStockAvailable(item.ProductId, item.Quantity))
                outOfStock.Add(item);
        }
        return outOfStock;
    }

    /// <summary>
    /// Adds all in-stock package items to the cart, skipping out-of-stock ones.
    /// </summary>
    public async Task ApplyPackageToCart(int cartId, Guid packageId, IEnumerable<Guid> skipProductIds)
    {
        var package = await GetPackage(packageId)
            ?? throw new ArgumentException("Package not found.", nameof(packageId));

        var skipSet = new HashSet<Guid>(skipProductIds);

        foreach (var item in package.Items)
        {
            if (skipSet.Contains(item.ProductId)) continue;

            try
            {
                await cartControl.AddProductToCart(cartId, item.ProductId, item.Quantity);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Skipping product {ProductId} while applying package to cart {CartId}", item.ProductId, cartId);
            }
        }
    }
}
