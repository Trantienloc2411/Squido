using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entities;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }
        
    public string OrderId { get; set; }
        
    public string BookId { get; set; }

    public string Title {get;set;}
    public string AuthorName { get; set; }
    public string CategoryName { get; set; }
        
    public int Quantity { get; set; }
        
    public decimal UnitPrice { get; set; }
        
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
        
    [ForeignKey("BookId")]
    public Book Book { get; set; }
        
    // Navigation property
    public virtual ICollection<Rating> Ratings { get; set; }
}