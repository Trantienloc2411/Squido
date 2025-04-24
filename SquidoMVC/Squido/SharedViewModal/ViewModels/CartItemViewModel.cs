namespace SharedViewModal.ViewModels;

public class CartItemViewModel
{
    public string? BookId { get; set; }
    public string? Title { get; set; }
    public decimal? Price { get; set; }
    public string? AuthorName {get;set;}
    public string? ImageUrl { get; set; }
    public int QuantityCart { get; set; }
    public int QuantityOnStore { get; set; }
}