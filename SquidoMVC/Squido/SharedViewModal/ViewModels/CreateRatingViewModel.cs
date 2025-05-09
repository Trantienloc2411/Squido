namespace SharedViewModal.ViewModels;

public class CreateRatingViewModel
{
    public Guid CustomerId { get; set; }
    public string? BookId { get; set; }
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
}