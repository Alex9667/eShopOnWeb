namespace eShopOnWebCatalog.Models;

public class CatalogItem
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public int CatalogBrandId { get; set; }

    public CatalogType CatalogType { get; set; }
    public CatalogBrand CatalogBrand { get; set; }
}
