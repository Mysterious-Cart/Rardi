using Microsoft.AspNetCore.Components;
using MudBlazor;
using CHKS.Services;
using CHKS.Entity;

namespace CHKS.Pages;

public partial class PackageModifer
{
    [Parameter] public Guid PackageId { get; set; }
    [Inject] protected PackageService PackageService { get; set; }
    [Inject] protected InventoryControlService InventoryControlService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected IDialogService DialogService { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    private Package? _package;
    private List<Product> _allProducts = [];
    private IEnumerable<Product> _filteredProducts = [];

    private bool _editingName;
    private string _editName = "";
    private string _editDescription = "";
    private MudTextField<string>? _nameField;

    protected override async Task OnInitializedAsync()
    {
        await LoadPackage();
        _allProducts = await InventoryControlService.GetProductList();
        _filteredProducts = _allProducts;
    }

    private async Task LoadPackage()
    {
        _package = await PackageService.GetPackage(PackageId);
        if (_package is null)
            NavigationManager.NavigateTo("/packages");
    }

    private void SearchProducts(string text)
    {
        _filteredProducts = string.IsNullOrWhiteSpace(text)
            ? _allProducts
            : _allProducts.Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase));
    }

    private async Task AddProduct(Product product)
    {
        try
        {
            await PackageService.AddItemToPackage(new AddPackageItemRequest(PackageId, product.Id, 1));
            await LoadPackage();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task RemoveItem(PackageItem item)
    {
        await PackageService.RemoveItemFromPackage(item.Id);
        await LoadPackage();
    }

    private async Task ChangeItemQty(PackageItem item, int qty)
    {
        if (qty < 1) return;
        await PackageService.UpdateItemQuantity(item.Id, qty);
        await LoadPackage();
    }

    private void StartEditName()
    {
        _editName = _package!.Name;
        _editDescription = _package.Description;
        _editingName = true;
    }

    private async Task SaveName()
    {
        if (string.IsNullOrWhiteSpace(_editName)) return;
        await PackageService.UpdatePackage(PackageId, _editName, _editDescription);
        _editingName = false;
        await LoadPackage();
    }

    private void CancelEditName() => _editingName = false;

    private async Task DeletePackage()
    {
        var result = await DialogService.ShowMessageBox(
            "Delete Package",
            $"Are you sure you want to delete \"{_package?.Name}\"? This cannot be undone.",
            yesText: "Delete", cancelText: "Cancel");

        if (result == true)
        {
            await PackageService.DeletePackage(PackageId);
            NavigationManager.NavigateTo("/packages");
        }
    }
}
