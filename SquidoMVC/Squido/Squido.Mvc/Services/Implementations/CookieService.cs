using System;
using Squido.Services.Interfaces;

namespace Squido.Services.Implementations;

public class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public void SetCookie(string key, string value, int? expireTimeMinutes = null)
    {
        CookieOptions option = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expireTimeMinutes.HasValue
                ? DateTimeOffset.Now.AddMinutes(expireTimeMinutes.Value)
                : DateTimeOffset.Now.AddHours(1)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, option);
    }

    public string? GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[key];
    }

    public void DeleteCookie(string key)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
    }
}
