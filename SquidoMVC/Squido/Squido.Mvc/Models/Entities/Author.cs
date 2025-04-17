namespace Squido.Models.Entities;

public class Author
{
    public Guid AuthorId { get; set; }
    public string FullName { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<Book> Books { get; set; }
}