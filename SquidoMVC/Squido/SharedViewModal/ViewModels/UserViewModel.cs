using SharedViewModal.EnumModel;

namespace SharedViewModal.ViewModels;

public class UserViewModel
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string Address { get; set; }
    public GenderEnum Gender { get; set; }
    public bool IsDeleted { get; set; }
    public RoleViewModel? Role { get; set; }
    public string? Token { get; set; }
}

