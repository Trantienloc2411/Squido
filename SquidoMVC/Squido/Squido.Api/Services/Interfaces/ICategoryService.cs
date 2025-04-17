using SharedViewModal.ViewModels;
using Squido.Models.Entities;

namespace WebApplication1.Services.Interfaces;

public interface ICategoryService
{
   Task<IEnumerable<CategoryViewModel>> GetCategories();
}