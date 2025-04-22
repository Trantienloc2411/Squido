using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class UserService(IUnitOfWork uoW, IMapper mapper, ILogger<UserService> logger)
    : IUserService
{
    public async Task<UserViewModel> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            var result = new UserViewModel();
            var user = await uoW.UserRepository.GetSingleWithIncludeAsync
                (c => email == c.Email, c => c.Role);
            if(BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
            {
                result = mapper.Map<UserViewModel>(user);
                return result;
            }
            
            return result = new UserViewModel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseMessage<RegisterRequestVm>> CreateUserAsync(RegisterRequestVm registerRequestVm)
    {
        try
        {
            
            if (IsUserExists(registerRequestVm.Email))
            {
                return ResponseFactory.Error<RegisterRequestVm>("User with the same email already exists", "USER_EXISTS");
            }
            else
            {
                var newUser = mapper.Map<User>(registerRequestVm);
                newUser.Password = Hashing(registerRequestVm.Password);
                uoW.UserRepository.Insert(newUser);
                uoW.Save();
                return ResponseFactory.Success(registerRequestVm, "User Created");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return ResponseFactory.Exception<RegisterRequestVm>(ex);
        }
    }

    private bool IsUserExists(string? email)
    {
        return uoW.UserRepository.GetAll().Any(c => string.Equals(c.Email, email, StringComparison.CurrentCultureIgnoreCase));
    }

    private static string Hashing(string password)
    {
        try
        {
            string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            return hashPassword;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}