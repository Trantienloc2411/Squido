using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace   WebApplication1.Models.Entities;

public class Rating
{
    [Key]
    public string Id { get; set; }
        
    public Guid CustomerId { get; set; }
    
    [ForeignKey("Book")]
    public string BookId { get; set; }
        
    public int RatingValue { get; set; }
    [MaxLength(150)]
    public string Comment { get; set; }
        
    public DateTime CreatedDate { get; set; }
    
    public virtual Book? Book { get; set; }
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
}