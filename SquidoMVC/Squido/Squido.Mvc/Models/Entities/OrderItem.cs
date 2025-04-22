using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squido.Models.Entities;

public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
        
    public string OrderId { get; set; }
        
    public string BookId { get; set; }
        
    public int Quantity { get; set; }
        
    public decimal UnitPrice { get; set; }
        
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
        
    [ForeignKey("BookId")]
    public Book Book { get; set; }
        
    // Navigation property
    public virtual ICollection<Rating> Ratings { get; set; }
}