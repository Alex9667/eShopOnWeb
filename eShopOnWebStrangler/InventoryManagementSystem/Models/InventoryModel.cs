using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

[Table("Inventory")]
public class InventoryModel
{
    [Key]
    public int ItemId { get; set; }
    public int Units { get; set; }
    public int ReservedUnits { get; set; }

    public InventoryModel(int units, int reservedUnits)
    {
        Units = units;
        ReservedUnits = reservedUnits;
    }

    public InventoryModel(int itemId, int units, int reservedUnits)
    {
        ItemId = itemId;
        Units = units;
        ReservedUnits = reservedUnits;
    }
}
