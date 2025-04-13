using Squido.Models.Entities;

namespace Squido.DAOs.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Book> BookRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    IGenericRepository<Category> CategoryRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }
    IGenericRepository<OrderItem> OrderItemRepository { get; }
    IGenericRepository<Rating> RatingRepository { get; }
    IGenericRepository<Role> RoleRepository { get; }
    IGenericRepository<ImageBook> ImageBookRepository { get; }
    void Save();
}