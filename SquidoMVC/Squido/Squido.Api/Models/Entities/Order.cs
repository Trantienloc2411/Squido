using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Entities;
namespace WebApplication1.Models.Entities;

public class Order
{
    [Key]
    public string Id { get; set; }
        
    public Guid CustomerId { get; set; }
        
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? CompleteTime { get; set; }
        
    public decimal TotalAmount { get; set; }
        
    public int Status { get; set; }
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
        
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}