using System;

namespace SharedViewModal.ViewModels;

public class LoginResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    
    public string? Email { get; set; }
}
