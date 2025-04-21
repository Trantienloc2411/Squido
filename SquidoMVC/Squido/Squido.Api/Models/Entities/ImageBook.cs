using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entities;

public class ImageBook
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
        
    public string UrlImage { get; set; }
        
    public string BookId { get; set; }
    
    public bool IsDeleted { get; set; }
        
    [ForeignKey("BookId")]
    public Book Book { get; set; }
}