namespace CHKS.Domain.Entities;

public record Package(Guid Id, string Name, string Description, IEnumerable<PackageItem> Items);

public record PackageItem(Guid Id, Guid PackageId, Guid ProductId, string ProductName, decimal ProductPrice, int Quantity);

public record CreatePackageRequest(string Name, string Description);
public record AddPackageItemRequest(Guid PackageId, Guid ProductId, int Quantity);