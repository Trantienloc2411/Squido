using WebApplication1.Models.Entities;

namespace WebApplication1.DAOs.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Book> BookRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    IGenericRepository<Category> CategoryRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }
    IGenericRepository<OrderItem> OrderItemRepository { get; }
    IGenericRepository<Rating> RatingRepository { get; }
    IGenericRepository<Role> RoleRepository { get; }
    IGenericRepository<Author> AuthorRepository { get; }
    IGenericRepository<RefreshToken> RefreshTokenRepository {get;}
    void Save();
    Task SaveAsync();
}