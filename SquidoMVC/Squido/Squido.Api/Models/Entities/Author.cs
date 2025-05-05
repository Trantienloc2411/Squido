using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities;

public class Author
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string FullName { get; set; }
    [MaxLength(200)]
    public string? Bio { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImageUrl { get; set; }
    public ICollection<Book> Books { get; set; }
}