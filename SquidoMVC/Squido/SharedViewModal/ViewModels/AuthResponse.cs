using System;

namespace SharedViewModal.ViewModels;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserViewModel User { get; set; }
}
