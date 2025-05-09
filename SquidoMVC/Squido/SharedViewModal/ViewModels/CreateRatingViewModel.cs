namespace SharedViewModal.ViewModels;

public class CreateRatingViewModel
{
    public required Guid CustomerId { get; set; }
    public required string BookId { get; set; }
    public required int RatingValue { get; set; }
    public string? Comment { get; set; }
}