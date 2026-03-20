using System.Collections.ObjectModel;

namespace CHKS.Domain.Entities;

public record Cart(int CartId, string Plate_Numbers, decimal Total, List<CartItem> CartContents);

public record CartItem(Guid ProductId, string Name, int Amount, decimal Price);