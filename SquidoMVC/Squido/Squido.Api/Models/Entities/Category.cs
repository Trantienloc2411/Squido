using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entities;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
        
    public string Name { get; set; }
        
    public string Description { get; set; }
        
    public DateTime CreatedDate { get; set; } 
    
        
    public DateTime? UpdatedDate { get; set; } 
    
    public bool IsDeleted { get; set; } = false;    
    // Navigation property
    public virtual ICollection<Book> Books { get; set; }
}