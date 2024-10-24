using System;
using System.Collections.Generic;
using eShopOnWebCatalog;

namespace eShopOnWebCatalog.CatalogBrandEndpoints;

public class ListCatalogBrandsResponse : BaseResponse
{
    public ListCatalogBrandsResponse(Guid correlationId) : base(correlationId)
    {
    }

    public ListCatalogBrandsResponse()
    {
    }

    public List<CatalogBrandDto> CatalogBrands { get; set; } = new List<CatalogBrandDto>();
}
