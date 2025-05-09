using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.enums;
using GenderEnum = SharedViewModal.EnumModel.GenderEnum;

namespace WebApplication1.Models.Entities;

public class User
{
        [Key]
        public Guid Id { get; set; }
        
        public string? Username { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; } 
        
        public enums.GenderEnum Gender { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string? AvatarImg { get; set; }
        
        public string? HomeAddress { get; set; }
        public string? WardName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        [MaxLength(10)]
        public string? Phone { get; set; }
        
        public int RoleId { get; set; }
        
        public bool IsDeleted { get; set; }
        
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        
        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
}