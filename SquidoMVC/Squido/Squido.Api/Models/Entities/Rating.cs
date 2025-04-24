using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace   WebApplication1.Models.Entities;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
        
    public Guid OrderItemId { get; set; }
        
    public Guid CustomerId { get; set; }
        
    public int RatingValue { get; set; }
        
    public DateTime CreatedDate { get; set; }
        
    [ForeignKey("OrderItemId")]
    public OrderItem OrderItem { get; set; }
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
}