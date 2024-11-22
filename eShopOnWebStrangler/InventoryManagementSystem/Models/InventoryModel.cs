using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

[Table("Inventory")]
public class InventoryModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ItemId { get; set; }
    public int CatalogItemId {  get; set; }
    public int Units { get; set; }
    public int ReservedUnits { get; set; }

    public InventoryModel(int catalogItemId, int units, int reservedUnits)
    {
        CatalogItemId = catalogItemId;
        Units = units;
        ReservedUnits = reservedUnits;
    }
}
