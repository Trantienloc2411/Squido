using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squido.Models.Entities;

public class User
{
        [Key]
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; } 
        
        public int Gender { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string AvatarImg { get; set; }
        
        public string Address { get; set; }
        
        public int RoleId { get; set; }
        
        public bool IsDeleted { get; set; }
        
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        
        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
}