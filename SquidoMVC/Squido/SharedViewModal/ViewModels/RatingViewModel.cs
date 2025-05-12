namespace SharedViewModal.ViewModels;

public class RatingViewModel
{
    public required string Id { get; set; }
    public required Guid CustomerId { get; set; }
    public required string BookId { get; set; }
    public UserViewModel? User { get; set; }
    public required int RatingValue { get; set; }
    public string? Comment { get; set; }
    public required DateTime CreatedDate { get; set; }
}