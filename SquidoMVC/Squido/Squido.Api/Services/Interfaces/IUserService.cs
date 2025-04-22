using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;

namespace WebApplication1.Services.Interfaces;

public interface IUserService
{
    Task<UserViewModel> GetUserByEmailAndPasswordAsync(string email, string password);
    Task<ResponseMessage<RegisterRequestVm>> CreateUserAsync(RegisterRequestVm registerRequest);
}