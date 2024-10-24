using System;
using System.Collections.Generic;
using eShopOnWebCatalog;
using eShopOnWebCatalog.CatalogItemEndpoints;

namespace eShopOnWebCatalog.CatalogItemEndpoints;

public class ListPagedCatalogItemResponse : BaseResponse
{
    public ListPagedCatalogItemResponse(Guid correlationId) : base(correlationId)
    {
    }

    public ListPagedCatalogItemResponse()
    {
    }

    public List<CatalogItemDto> CatalogItems { get; set; } = new List<CatalogItemDto>();
    public int PageCount { get; set; }
}
