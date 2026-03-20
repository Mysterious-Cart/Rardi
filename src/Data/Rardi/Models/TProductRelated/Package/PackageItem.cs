using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models;

[Table("PackageItem")]
[PrimaryKey("Id")]
public class PackageItem_Model
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PackageId { get; set; }
    public Package_Model Package { get; set; }

    [Required]
    public Guid ProductId { get; set; }
    public ProductModel Product { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;
}