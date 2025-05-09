using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Models.enums;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;
public class StatService(IUnitOfWork unitOfWork) : IStatsService
{

    public async Task<StatViewModel> GetStatsAsync()
    {
        var totalBooks = await unitOfWork.BookRepository.CountAsync(b => b.IsDeleted == false && b.Category.IsDeleted == false && b.Author.IsDeleted == false);
        var totalCategories = await unitOfWork.CategoryRepository.CountAsync(t => t.IsDeleted == false);
        var totalCustomers = await unitOfWork.UserRepository.CountAsync(c => c.IsDeleted == false && c.RoleId == 1);

        var getAllOrders = await unitOfWork.OrderRepository.GetAllWithIncludeAsync(o => o.Status == Models.enums.OrderStatusEnum.Completed, o => o.OrderItems);

        var totalRevenues = getAllOrders.Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity));

        var allBook = await unitOfWork.BookRepository.GetAllWithIncludeAsync(b => b.IsDeleted == false, b => b.Category);

        var topBooks = allBook
            .OrderByDescending(b => b.UpdatedDate)
            .Take(5)
            .Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                CategoryName = b.Category.Name,
                ImageUrl = b.ImageUrl,
                Quantity = b.Quantity,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate
            });
        var topCategories = allBook
            .GroupBy(b => b.CategoryId)
            .Select(g => new CategoryViewModel
            {
                Id = g.Key,
                Name = g.FirstOrDefault()!.Category.Name,
                BookCount = g.Count()
            })
            .OrderByDescending(c => c.BookCount)
            .Take(5);
        
        


        return new StatViewModel
        {
            TotalBooks = totalBooks,
            TotalCategories = totalCategories,
            TotalCustomers = totalCustomers,
            TotalRevenues = totalRevenues,
            TopBooks = [.. topBooks],
            TopCategories = [.. topCategories]
        };
    }
}
