using CHKS.Domain.Enums;
using CHKS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Data;

public static class TestingModeSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        await SeedRardiDataAsync(serviceProvider);
        await SeedIdentityDataAsync(serviceProvider);
    }

    private static async Task SeedRardiDataAsync(IServiceProvider serviceProvider)
    {
        using var db = serviceProvider.GetRequiredService<IDbContextFactory<Rardi_Context>>().CreateDbContext();

        await db.Database.EnsureCreatedAsync();

        if (await db.Inventory.AnyAsync())
        {
            return;
        }

        var oilFilterId = Guid.NewGuid();
        var coolantId = Guid.NewGuid();
        var brakePadId = Guid.NewGuid();

        var vehicles = new List<Vehicle_Model>
        {
            new() { Key = 1, Make = "Toyota", Model = "Hilux", Year = 2022 },
            new() { Key = 2, Make = "Honda", Model = "Civic", Year = 2021 }
        };

        var customers = new List<CustomerModel>
        {
            new()
            {
                PlateNumber = "KH-001-AA",
                Name = "Test Customer A",
                Phone = "0100000001",
                Vehicle_Id = 1,
                Description = "Testing mode seeded customer"
            },
            new()
            {
                PlateNumber = "KH-002-BB",
                Name = "Test Customer B",
                Phone = "0100000002",
                Vehicle_Id = 2,
                Description = "Testing mode seeded customer"
            }
        };

        var products = new List<ProductModel>
        {
            new()
            {
                Id = oilFilterId,
                Name = "Oil Filter",
                Normalized_Name = "OIL FILTER",
                Barcode = "TEST-0001",
                Description = "Pseudo inventory item",
                Status = ProductStatus.Active,
                Stock = 30,
                Import = 4.50m,
                Export = 7.00m,
                AllowTracking = true,
                AllowWarning = true,
                Optimal_Stock = 10
            },
            new()
            {
                Id = coolantId,
                Name = "Coolant",
                Normalized_Name = "COOLANT",
                Barcode = "TEST-0002",
                Description = "Pseudo inventory item",
                Status = ProductStatus.Active,
                Stock = 20,
                Import = 6.00m,
                Export = 9.50m,
                AllowTracking = true,
                AllowWarning = true,
                Optimal_Stock = 8
            },
            new()
            {
                Id = brakePadId,
                Name = "Brake Pad Set",
                Normalized_Name = "BRAKE PAD SET",
                Barcode = "TEST-0003",
                Description = "Pseudo inventory item",
                Status = ProductStatus.Active,
                Stock = 15,
                Import = 12.00m,
                Export = 18.00m,
                AllowTracking = true,
                AllowWarning = true,
                Optimal_Stock = 5
            }
        };

        var group = new GroupModel
        {
            Id = 1,
            Name = "Test Group"
        };

        var employee = new EmployeeModel
        {
            Id = 1,
            Name = "Test Employee"
        };

        var cart = new CartModel
        {
            CartId = 1,
            CustomerId = customers[0].PlateNumber,
            Total = 0,
            Status = CartStatus.Progress
        };

        var cartItem = new CartItemModel
        {
            Id = Guid.NewGuid(),
            CartId = 1,
            ProductId = oilFilterId,
            Qty = 2,
            Note = "Seeded cart item"
        };

        var stockLog = new StockLogsModel
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            ProductId = oilFilterId,
            Date = DateTime.UtcNow,
            Amount = 10,
            Type = TransactionLogType.In,
            Seen = false
        };

        db.Vehicles.AddRange(vehicles);
        db.Customers.AddRange(customers);
        db.Inventory.AddRange(products);
        db.Groups.Add(group);
        db.Employees.Add(employee);
        db.Carts.Add(cart);
        db.CartContents.Add(cartItem);
        db.StockLogs.Add(stockLog);

        await db.SaveChangesAsync();
    }

    private static async Task SeedIdentityDataAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }

        if (!await roleManager.RoleExistsAsync("Cashier"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Cashier" });
        }

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser != null)
        {
            return;
        }

        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@testing.local",
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (!createResult.Succeeded)
        {
            return;
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}