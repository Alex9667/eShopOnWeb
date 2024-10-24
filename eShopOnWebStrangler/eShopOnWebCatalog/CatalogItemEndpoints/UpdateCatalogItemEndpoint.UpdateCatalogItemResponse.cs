using System;
using eShopOnWebCatalog;
using eShopOnWebCatalog.CatalogItemEndpoints;

namespace eShopOnWebCatalog.CatalogItemEndpoints;

public class UpdateCatalogItemResponse : BaseResponse
{
    public UpdateCatalogItemResponse(Guid correlationId) : base(correlationId)
    {
    }

    public UpdateCatalogItemResponse()
    {
    }

    public CatalogItemDto CatalogItem { get; set; }
}
