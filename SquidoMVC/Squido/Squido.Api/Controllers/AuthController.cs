using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Services;

namespace WebApplication1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtService jwtService;
    private readonly IUserService userService;

    public AuthController(IJwtService jwtService, IUserService userService)
    {
        this.jwtService = jwtService;
        this.userService = userService;
    }

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
                else if (user.Role!.RoleId != 1 && user.Role.RoleId != 2)
                {
                    return Unauthorized("You are not authorized to login.");
                }

                var roles = user.Role.RoleId;

                // Generate tokens
                string accessToken = jwtService.GenerateToken(user.Id!.ToString(), user!.Username!, roles);
                var refreshToken = jwtService.GenerateRefreshToken(Guid.Parse(user.Id!.ToString()));

                // Return tokens along with user info if needed
                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken!.Token,
                    User = user
                });
            }
            else
            {
                return BadRequest("Email or password is incorrect");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred during login");
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