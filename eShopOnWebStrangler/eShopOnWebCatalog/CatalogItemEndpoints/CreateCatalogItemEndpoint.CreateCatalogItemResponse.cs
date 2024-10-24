using System;
using eShopOnWebCatalog;
using eShopOnWebCatalog.CatalogItemEndpoints;

namespace eShopOnWebCatalog.CatalogItemEndpoints;

public class CreateCatalogItemResponse : BaseResponse
{
    public CreateCatalogItemResponse(Guid correlationId) : base(correlationId)
    {
    }

    public CreateCatalogItemResponse()
    {
    }

    public CatalogItemDto CatalogItem { get; set; }
}
