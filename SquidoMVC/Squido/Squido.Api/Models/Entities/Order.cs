using System.ComponentModel;
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
    [DefaultValue(0.00)]
    public decimal ShippingFee { get; set; } = 0;

    public PaymentMethodEnum PaymentMethod {get;set;}
        
    public OrderStatusEnum Status { get; set; }

    public string? OrderNote {get;set;}
        
    [ForeignKey("CustomerId")]
    public User Customer { get; set; }
        
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}