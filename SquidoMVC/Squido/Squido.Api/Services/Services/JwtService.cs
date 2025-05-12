using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApplication1.Services.Services;

public class JwtService : IJwtService
{
    private readonly IUnitOfWork uoW;
    private readonly IConfiguration config;

    public JwtService(IUnitOfWork uoW, IConfiguration config) 
    {
        this.uoW = uoW;
        this.config = config;
    }

    public string GenerateToken(string userId, string username, int RoleId)
    {
        string? secret = config["Jwt:Key"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Role", RoleId.ToString())
        };


        var token = new JwtSecurityToken
        (
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(config["Jwt:ExpiryInMiunte"])),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken? GenerateRefreshToken(Guid userId)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var refreshToken = new RefreshToken
        {
            Token = token,
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now,
            UserId = userId
        };
        uoW.RefreshTokenRepository.Insert(refreshToken);
        uoW.Save();
        return refreshToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, Guid userId)
    {
        var storedToken = await uoW.RefreshTokenRepository
            .GetSingleWithIncludeAsync(c => c.Token == token && c.UserId == userId);
        if (storedToken == null || storedToken.IsExpired) return false;
        return true;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token, Guid userId)
    {
        var storedToken = await uoW.RefreshTokenRepository
            .GetSingleWithIncludeAsync(rt => rt.Token == token && rt.UserId == userId);

        if (storedToken == null || storedToken.IsExpired)
            return null;

        return storedToken;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var storedToken = await uoW.RefreshTokenRepository
            .GetSingleWithIncludeAsync(rt => rt.Token == token);

        if (storedToken != null)
        {
            await uoW.RefreshTokenRepository.DeleteAsync(storedToken);
            uoW.Save();
        }
    }

    public ClaimsPrincipal ValidateAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "The parameter 'token' cannot be a 'null' or an empty object.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenMalformedException("Invalid token format");
            }

            return principal;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenMalformedException("Invalid token format", ex);
        }
    }
}