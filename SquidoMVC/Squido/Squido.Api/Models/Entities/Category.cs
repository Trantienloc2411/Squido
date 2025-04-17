using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squido.Models.Entities;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
        
    public string Name { get; set; }
        
    public string Description { get; set; }
        
    public DateTime CreatedDate { get; set; } // Note: DateTime as integer seems unusual
        
    public DateTime? UpdatedDate { get; set; } // Note: DateTime as integer seems unusual
        
    // Navigation property
    public virtual ICollection<Book> Books { get; set; }
}