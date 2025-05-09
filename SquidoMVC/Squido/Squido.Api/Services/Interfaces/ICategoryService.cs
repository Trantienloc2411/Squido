using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces;

public interface ICategoryService
{
   Task<IEnumerable<CategoryViewModel>> GetCategories();
   Task<ResponseMessage<CreateCategoryModel>> CreateCategory(CreateCategoryModel model);
   Task<ResponseMessage<CategoryViewModel>> UpdateCategory(int id, CreateCategoryModel model);
   Task<ResponseMessage<CategoryViewModel>> GetCategoryById(int id);
   Task<ResponseMessage<CategoryViewModel>> DeleteCategory(int id);
   
}