using System;
using System.Linq;
using Ardalis.Specification;
using eShopOnWebCatalog.Entities;


namespace eShopOnWebCatalog.Specifications;

public class CatalogItemsSpecification : Specification<CatalogItem>
{
    public CatalogItemsSpecification(params int[] ids)
    {
        Query.Where(c => ids.Contains(c.Id));
    }
}
