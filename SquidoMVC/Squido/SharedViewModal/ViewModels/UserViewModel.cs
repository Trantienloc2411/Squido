using SharedViewModal.EnumModel;

namespace SharedViewModal.ViewModels;

public class UserViewModel
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? HomeAddress { get; set; }
    public string? WardName { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Phone {get;set;}
    public int RoleId { get; set; }
    public GenderEnum? Gender { get; set; }
    public bool? IsDeleted { get; set; }
    public RoleViewModel? Role { get; set; }
    public string? Token { get; set; }
}

