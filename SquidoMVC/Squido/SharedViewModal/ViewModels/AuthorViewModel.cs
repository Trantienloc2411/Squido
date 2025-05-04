namespace SharedViewModal.ViewModels;

public class AuthorViewModel
{
    public string AuthorId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<BookViewModel>? Books { get; set; } = [];
    
}