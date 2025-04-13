using Squido.Models.Entities;

namespace Squido.Services.Interfaces;

public interface ICategoryService
{
   IEnumerable<Category> GetCategories();
}