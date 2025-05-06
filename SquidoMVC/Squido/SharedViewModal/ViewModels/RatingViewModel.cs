namespace SharedViewModal.ViewModels;

public class RatingViewModel
{
    public Guid? Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? BookId { get; set; }
    public UserViewModel? User { get; set; }
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
}