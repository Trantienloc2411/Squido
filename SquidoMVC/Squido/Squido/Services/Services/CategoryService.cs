using Squido.DAOs.Interfaces;
using Squido.Models.Entities;
using Squido.Services.Interfaces;

namespace Squido.Services.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoryService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public IEnumerable<Category> GetCategories()
    {
        return _unitOfWork.CategoryRepository.GetAll();
    }
}