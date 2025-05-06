using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryViewModel>> GetCategories()
    {
        var categories = await unitOfWork.CategoryRepository.GetAllWithIncludeAsync(c => c.IsDeleted == false);
        return mapper.Map<IEnumerable<CategoryViewModel>>(categories);
    }

    public async Task<ResponseMessage<CreateCategoryModel>> CreateCategory(CreateCategoryModel model)
    {
        try
        {
            var category = mapper.Map<Category>(model);
            category.IsDeleted = false;
            category.CreatedDate = DateTime.Now;

            await unitOfWork.CategoryRepository.AddAsync(category);
            unitOfWork.Save();
            return new ResponseMessage<CreateCategoryModel>()
            {
                IsSuccess = true,
                Data = mapper.Map<CreateCategoryModel>(category)
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseMessage<CategoryViewModel>> UpdateCategory(int id, CreateCategoryModel model)
    {
        try
        {
            var oldCat = await 
                unitOfWork
                    .CategoryRepository
                    .GetSingleWithIncludeAsync(t => t.Id == id);

            if (oldCat != null)
            {
                var newCat = mapper.Map<Category>(model);
                newCat.IsDeleted = false;
                newCat.CreatedDate = oldCat.CreatedDate;
                newCat.UpdatedDate = DateTime.Now;
                newCat.Id = oldCat.Id;
                await unitOfWork.CategoryRepository.UpdateAsync(newCat);
                unitOfWork.Save();
                return new ResponseMessage<CategoryViewModel>()
                {
                    IsSuccess = true,
                    Data = mapper.Map<CategoryViewModel>(newCat)
                };
            }
            else
            {
                return new ResponseMessage<CategoryViewModel>()
                {
                    IsSuccess = false,
                };
            }
        }
        catch (Exception e)
        {
            return new ResponseMessage<CategoryViewModel>()
            {
                ExceptionMessage = e.Message,
            };
        }
    }

    public async Task<ResponseMessage<CategoryViewModel>> GetCategoryById(int id)
    {
        try
        {
            var cat = await unitOfWork.CategoryRepository.GetSingleWithIncludeAsync(t => t.Id == id);
            if (cat == null)
                return new ResponseMessage<CategoryViewModel>
                {
                    IsSuccess = false,

                };
            return new ResponseMessage<CategoryViewModel>()
            {
                IsSuccess = true,
                Data = mapper.Map<CategoryViewModel>(cat)
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseMessage<CategoryViewModel>> DeleteCategory(int id)
    {
        try
        {
            var category = await unitOfWork.CategoryRepository.GetSingleWithIncludeAsync(t => t.Id == id); 
            if(category != null)
            {
                category.IsDeleted = true;
                await unitOfWork.CategoryRepository.UpdateAsync(category);
                unitOfWork.Save();
                return new ResponseMessage<CategoryViewModel>
                {
                    IsSuccess = true,

                };
            }
            else
            {
                return new ResponseMessage<CategoryViewModel>()
                {
                    IsSuccess = false
                };
            }
            
        }
        catch (Exception e)
        {
            return new ResponseMessage<CategoryViewModel>()
            {
                ExceptionMessage = e.Message,
                IsSuccess = false,
            };
        }
    }
}