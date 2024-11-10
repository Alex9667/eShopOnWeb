using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

[Table("Inventory")]
public class InventoryModel
{
    [Key]
    public int ItemId { get; set; }
    public int Units { get; set; }

    public InventoryModel(int units)
    {
        Units = units;
    }

    public InventoryModel(int itemId, int units)
    {
        ItemId = itemId;
        Units = units;
    }
}
