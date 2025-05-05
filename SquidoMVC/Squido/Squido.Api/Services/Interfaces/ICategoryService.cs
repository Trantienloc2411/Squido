using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces;

public interface ICategoryService
{
   Task<IEnumerable<CategoryViewModel>> GetCategories();
   Task<CategoryViewModel> GetCategoryById(int id);
   Task CreateCategory(CategoryViewModel model);
}