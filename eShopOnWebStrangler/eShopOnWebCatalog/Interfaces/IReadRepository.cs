using Ardalis.Specification;

namespace eShopOnWebCatalog.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
