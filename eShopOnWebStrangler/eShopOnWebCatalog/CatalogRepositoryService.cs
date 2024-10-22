using eShopOnWebCatalog.Entities;
using eShopOnWebCatalog.Interfaces;

namespace eShopOnWebCatalog;

public class CatalogRepositoryService
{
    private readonly IRepository<CatalogItem> _catalogItemRepository;

    public CatalogRepositoryService(IRepository<CatalogItem> catalogItemRepository)
    {

    }
}
