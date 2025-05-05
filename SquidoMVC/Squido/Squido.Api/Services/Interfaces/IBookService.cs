using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces
{
    public interface IBookService
    {
        Task<ICollection<BookViewModel>> GetBooks(string? keyword = null);
        Task<ICollection<BookViewModel>> GetBooksPaginated(int currentPage, int pageSize = 10, string? keyword = null);
        Task<Book> GetBookById(string id);
        Task<ICollection<BookViewModel>> GetBookByAuthorId(string authorId, string? currentBook);
        Task<ICollection<BookViewModel>> GetBooksByCategoryId(List<int> categoryIds);

        
        Task<ResponseMessage<BookViewModel>> CreateBook(CreateBookViewModel bookViewModel);

        Task<ResponseMessage<BookViewModel>> UpdateBook(string id, CreateBookViewModel bookViewModel);

        Task<ResponseMessage<BookViewModel>> DeleteBook(string id);
        
        
    }
}