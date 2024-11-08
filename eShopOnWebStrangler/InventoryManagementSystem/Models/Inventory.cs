using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

[Table("Inventory")]
public class Inventory
{
    [Key]
    public int ItemId { get; set; }
    public int Units { get; set; }
}
