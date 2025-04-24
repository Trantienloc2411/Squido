using AutoMapper;
using SharedViewModal.ViewModels;

namespace Squido.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BookViewModel, CartItemViewModel>()
            .ReverseMap();
    }
}