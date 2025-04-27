using AutoMapper;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
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

        CreateMap<UserViewModel, User>()
            .ForMember(dest => dest.Role, opt =>
                opt.Condition((src, dest, srcMember, destMember, context) =>
                context.Items.ContainsKey("IgnoreRole") == false)) // Ignore Role when updating

            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? new RoleViewModel
            {
                RoleId = src.Role.RoleId,
                RoleName = src.Role.RoleName
            } : null)); // Map Role to RoleViewModel



        CreateMap<User, RegisterRequestVm>().ReverseMap();
        CreateMap<Role, RoleViewModel>().ReverseMap();

        CreateMap<OrderItemViewModel, OrderItem>().ReverseMap();
        CreateMap<Order, OrderViewModel>().ReverseMap();

    }
}