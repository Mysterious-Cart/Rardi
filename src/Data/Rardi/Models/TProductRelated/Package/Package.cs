using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace CHKS.Models;

[Table("Package")]
[PrimaryKey("Id")]
public class Package_Model
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public ICollection<PackageItem_Model> PackageItems { get; set; } = [];
}