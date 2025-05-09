using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.Entities
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public int CategoryId { get; set; }
        
        public string Description { get; set; }
        
        public Guid AuthorId { get; set; }
    
        [DefaultValue(0)]
        public int Quantity { get; set; }
        
        public decimal Price { get; set; }
    
        [DefaultValue(0)]
        public int BuyCount { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }

        public string? ImageUrl { get; set; }
    
        public bool IsDeleted { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        
        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual Author Author { get; set; }
    }
}