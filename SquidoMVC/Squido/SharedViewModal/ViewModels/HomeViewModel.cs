

namespace SharedViewModal.ViewModels;

public class HomeViewModel
{
    public ICollection<BookViewModel>? Books { get; set; }
    public List<CategoryViewModel>? Categories { get; set; }
}