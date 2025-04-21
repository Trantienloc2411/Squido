using System;

namespace Squido.Services.Interfaces;

public interface ICookieService
{
    void SetCookie(string key, string value, int? expireTimeMinutes = null);
    string? GetCookie(string key);
    void DeleteCookie(string key);
}

