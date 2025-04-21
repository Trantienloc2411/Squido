using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entities;

public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoleId { get; set; }
        
    public string? RoleName { get; set; } // Note: RoleName as integer seems unusual
        
    // Navigation property
    public virtual ICollection<User> Users { get; set; }
}