using Squido.DAOs.Interfaces;
using Squido.Models.Entities;
using Squido.Services.Interfaces;

namespace Squido.Services.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    public BookService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<ICollection<Book>> GetBooks()
    {
        return await _unitOfWork.BookRepository.GetAllWithIncludeAsync(p => true, p => p.Category);
    }
    
}