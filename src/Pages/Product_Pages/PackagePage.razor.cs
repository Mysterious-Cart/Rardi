using Microsoft.AspNetCore.Components;
using MudBlazor;
using CHKS.Services;
using CHKS.Entity;
using CHKS.Pages.Component;

namespace CHKS.Pages;

public partial class PackagePage
{
    [Inject] protected PackageService PackageService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected IDialogService DialogService { get; set; }

    private List<Package> _all = [];
    private IEnumerable<Package> filtered = [];
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadPackages();
    }

    private async Task LoadPackages()
    {
        isLoading = true;
        _all = await PackageService.GetPackages();
        filtered = _all;
        isLoading = false;
    }

    private void Search(string text)
    {
        filtered = string.IsNullOrWhiteSpace(text)
            ? _all
            : _all.Where(p =>
                p.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(text, StringComparison.OrdinalIgnoreCase));
    }

    private async Task OpenCreateDialog()
    {
        var dialog = await DialogService.ShowAsync<Component.CreatePackageDialog>("New Package",
            new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true,
                CloseButton = true,
                CloseOnEscapeKey = true,
                BackdropClick = false
            });

        var result = await dialog.Result;
        if (!result.Canceled)
            await LoadPackages();
    }
}
