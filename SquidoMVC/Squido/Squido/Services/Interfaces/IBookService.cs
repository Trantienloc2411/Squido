using Squido.Models.Entities;

namespace Squido.Services.Interfaces;

public interface IBookService
{ 
    Task<ICollection<Book>> GetBooks();
}