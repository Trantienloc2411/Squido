using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Entities;
using WebApplication1.Models.enums;

namespace WebApplication1.Models.Entities;

public class Order
{
    [Key]
    public string Id { get; set; }
        
    public Guid CustomerId { get; set; }
        
    public DateTime OrderDate { get; set; }
    public DateTime? ConfirmDate { get; set; }
    
    public DateTime? CompleteDate { get; set; }
    
        
    public decimal TotalAmount { get; set; }
        
    public OrderStatusEnum Status { get; set; }
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
        
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}