using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Services.Interfaces;
using WebApplication1.Models.enums;

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
                (c => email == c.Email && c.IsDeleted == false, c => c.Role);
            if (BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
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
        return uoW.UserRepository.GetAll().Where(c => c.IsDeleted == false).Any(c => string.Equals(c.Email, email, StringComparison.CurrentCultureIgnoreCase));
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

    public async Task<ResponseMessage<UserViewModel>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await uoW.UserRepository.GetSingleWithIncludeAsync(c => c.Id == userId, c => c.Role);
            if (user is null)
            {
                return ResponseFactory.Error<UserViewModel>("User not found", "USER_NOT_FOUND");
            }
            var result = mapper.Map<UserViewModel>(user);
            return ResponseFactory.Success(result, "User Found");
        }
        catch (System.Exception ex)
        {
            // 
            return ResponseFactory.Exception<UserViewModel>(ex);
        }
    }

    public async Task<ResponseMessage<UserViewModel>> UpdateUserAsync(UserViewModel userViewModel, Guid userId)
    {
        try
        {
            if (IsUserExists(userId))
            {
                var userOld = await uoW.UserRepository.GetSingleWithIncludeAsync(c => c.Id == userId && c.IsDeleted == false, c => c.Role);
                int role = userOld.RoleId;
                GenderEnum gender = userOld.Gender;
                // Map updated fields into the existing entity
                mapper.Map(userViewModel, userOld, opt => opt.Items["IgnoreRole"] = false);

                userOld.RoleId = role;
                userOld.Gender = gender;

                await uoW.UserRepository.UpdateAsync(userOld);
                uoW.Save();

                return ResponseFactory.Success(userViewModel, "User Updated");
            }
            else
            {
                return ResponseFactory.Error<UserViewModel>("User not found", "USER_NOT_FOUND");
            }
        }
        catch (System.Exception ex)
        {
            return ResponseFactory.Exception<UserViewModel>(ex);
        }
    }


    private bool IsUserExists(Guid userId)
    {
        try
        {
            return uoW.UserRepository.GetAll().Where(c => c.IsDeleted == false).Any(c => c.Id == userId);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex);
            throw;
        }
    }
}