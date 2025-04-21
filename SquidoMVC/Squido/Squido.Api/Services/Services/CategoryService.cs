using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryViewModel>> GetCategories()
    {
        var categories = await unitOfWork.CategoryRepository.GetAllWithIncludeAsync(c => true);
        return mapper.Map<IEnumerable<CategoryViewModel>>(categories);
    }
}