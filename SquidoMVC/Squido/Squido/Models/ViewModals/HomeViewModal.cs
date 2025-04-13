using Squido.Models.Entities;

namespace Squido.Models.ViewModals;

public class HomeViewModal
{
    public ICollection<Book> Books { get; set; }
    public List<Category> Categories { get; set; }
}