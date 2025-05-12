using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Models.RequestViewModal;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Services;

namespace WebApplication1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IJwtService jwtService, IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            if (loginRequest is { Email: not null, Password: not null })
            {
                var user = await userService.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password);

                // Check if user exists and is valid
                if (user.Email == null)
                {
                    return BadRequest("Email or password is incorrect");
                }

                if ((bool)user!.IsDeleted!)
                {
                    return Unauthorized("This account was deleted or not authorized.");
                }
                else if (user.Role!.Id != 1 && user.Role.Id != 2)
                {
                    return Unauthorized("You are not authorized to login.");
                }

                var roles = user.Role.Id;

                // Generate tokens
                string accessToken = jwtService.GenerateToken(user.Id!.ToString(), user!.Username!, roles);
                var refreshToken = jwtService.GenerateRefreshToken(Guid.Parse(user.Id!.ToString()));

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshToken.Token,
                    user
                });
            }

            return BadRequest("Invalid request");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken) || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("Invalid request");
            }

            var userId = Guid.Parse(request.UserId);
            var isValid = await jwtService.ValidateRefreshTokenAsync(request.RefreshToken, userId);

            if (!isValid)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = await userService.GetUserByIdAsync(userId);
            if (user.Data == null)
            {
                return Unauthorized("User not found");
            }

            var accessToken = jwtService.GenerateToken(user.Data.Id!.ToString(), user.Data.Username!, user.Data.Role!.Id);
            var newRefreshToken = jwtService.GenerateRefreshToken(userId);

            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken.Token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid request");
            }

            await jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
            return Ok("Token revoked successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestVm registerRequest)
    {
        try
        {
            var result = await userService.CreateUserAsync(registerRequest);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}