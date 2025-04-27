using AutoMapper;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;

namespace Squido.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BookViewModel, CartItemViewModel>()
            .ReverseMap();

        CreateMap<UpdateAddressRequestVm, UserViewModel>().ReverseMap();
    }
}