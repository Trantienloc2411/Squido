using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;

namespace WebApplication1.Services.Interfaces;

public interface IUserService
{
    Task<UserViewModel> GetUserByEmailAndPasswordAsync(string email, string password);
    Task<ResponseMessage<RegisterRequestVm>> CreateUserAsync(RegisterRequestVm registerRequest);
    Task<ResponseMessage<UserViewModel>> GetUserByIdAsync(Guid userId);

    Task<ResponseMessage<UserViewModel>> UpdateUserAsync(UserViewModel userViewModel, Guid userId); // <T>

}