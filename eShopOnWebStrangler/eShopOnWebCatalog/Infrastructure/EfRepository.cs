using Ardalis.Specification.EntityFrameworkCore;
using eShopOnWebCatalog.Data;
using eShopOnWebCatalog.Interfaces;

namespace eShopOnWebCatalog.Infrastructure;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(eShopOnWebCatalog.Data.CatalogContext dbContext) : base(dbContext)
    {
    }
}
