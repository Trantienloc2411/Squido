using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squido.Models.Entities;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
        
    public string OrderItemId { get; set; }
        
    public string CustomerId { get; set; }
        
    public int RatingValue { get; set; }
        
    public DateTime CreatedDate { get; set; }
        
    [ForeignKey("OrderItemId")]
    public OrderItem OrderItem { get; set; }
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
}