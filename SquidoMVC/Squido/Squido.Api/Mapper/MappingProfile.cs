using AutoMapper;
using SharedViewModal.ViewModels;
using Squido.Models.Entities;

namespace WebApplication1.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookViewModel>()
            .ForMember(dest => dest.CategoryName
                , opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.FullName));
        
        CreateMap<Category, CategoryViewModel>()
            .ForMember(dest => dest.CategoryId, 
                opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
    }
}