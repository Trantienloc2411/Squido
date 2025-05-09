using System.Security.Claims;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string username, int RoleId);
    RefreshToken? GenerateRefreshToken(Guid userId);
    Task<bool> ValidateRefreshTokenAsync(string token, string userId);
    Task<RefreshToken?> GetValidRefreshTokenAsync(string token, Guid userId);
    Task RevokeRefreshTokenAsync(string token);
    ClaimsPrincipal ValidateAccessToken(string token);
} 