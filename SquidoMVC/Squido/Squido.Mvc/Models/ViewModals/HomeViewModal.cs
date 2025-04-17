using SharedViewModal.ViewModels;
using Squido.Models.Entities;

namespace Squido.Models.ViewModals;

public class HomeViewModal
{
    public ICollection<BookViewModel>? Books { get; set; }
    public List<CategoryViewModel>? Categories { get; set; }
}