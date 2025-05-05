namespace SharedViewModal.ViewModels;
public class CreateBookViewModel
{
    public string? Title { get; set; }
    public int? CategoryId { get; set; }
    public Guid? AuthorId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}