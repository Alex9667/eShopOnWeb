using AutoMapper;
using eShopOnWebCatalog.CatalogBrandEndpoints;
using eShopOnWebCatalog.CatalogItemEndpoints;
using eShopOnWebCatalog.CatalogTypeEndpoints;
using eShopOnWebCatalog.Entities;

namespace Microsoft.eShopWeb.PublicApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CatalogItem, CatalogItemDto>();
        CreateMap<CatalogType, CatalogTypeDto>()
            .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Type));
        CreateMap<CatalogBrand, CatalogBrandDto>()
            .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Brand));
    }
}
