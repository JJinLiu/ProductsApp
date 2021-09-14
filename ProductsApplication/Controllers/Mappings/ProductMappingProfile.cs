using AutoMapper;
using ProductsApplication.Controllers.Dto;
using ProductsApplication.Models;

namespace ProductsApplication.Controllers.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Name.Trim()))
                .ReverseMap();
        }
    }
}
