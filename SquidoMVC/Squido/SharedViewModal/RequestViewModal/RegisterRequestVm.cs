using System.ComponentModel.DataAnnotations;
using SharedViewModal.EnumModel;
using SharedViewModal.ViewModels;

namespace SharedViewModal.RequestViewModal;

public class RegisterRequestVm
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; } 
    public GenderEnum Gender { get; set; }
    [MaxLength(32)]
    public string? Password { get; set; }
    public int RoleId { get; set; }
}