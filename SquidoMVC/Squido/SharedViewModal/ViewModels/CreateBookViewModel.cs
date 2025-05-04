namespace SharedViewModal.ViewModels;
public class CreateBookViewModel
{
    public string? BookId { get; set; }
    public string? Title { get; set; }
    public int? CategoryId { get; set; }
    public Guid? AuthorId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public List<string>? ImageUrls { get; set; }
}