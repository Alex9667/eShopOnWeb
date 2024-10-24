using System;
using eShopOnWebCatalog;
using eShopOnWebCatalog.CatalogItemEndpoints;

namespace eShopOnWebCatalog.CatalogItemEndpoints;

public class GetByIdCatalogItemResponse : BaseResponse
{
    public GetByIdCatalogItemResponse(Guid correlationId) : base(correlationId)
    {
    }

    public GetByIdCatalogItemResponse()
    {
    }

    public CatalogItemDto CatalogItem { get; set; }
}
