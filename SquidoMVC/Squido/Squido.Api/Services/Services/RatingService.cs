using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class RatingService(IUnitOfWork unitOfWork, IMapper mapper) : IRatingService
{
    public async Task<ResponseMessage<CreateRatingViewModel>> CreateRating(CreateRatingViewModel model)
    {
        try
        {
            var rating = mapper.Map<Rating>(model);
            rating.CreatedDate = DateTime.Now;
            rating.Id = Guid.NewGuid().ToString();
            await unitOfWork.RatingRepository.AddAsync(rating);
            unitOfWork.Save();
            return ResponseFactory.Success(mapper.Map<CreateRatingViewModel>(rating), "Added Rating");
            
        }
        catch (Exception e)
        {
            return ResponseFactory.Exception<CreateRatingViewModel>(e);
        }
    }

    public async Task<ResponseMessage<List<RatingViewModel>>> GetRatingsByBookId(string id)
    {
        try
        {
            var listRating = await unitOfWork.RatingRepository.GetAllWithIncludeAsync(c => c.BookId.ToString() == id, c => c.Customer);
            
            var result = mapper.Map<List<RatingViewModel>>(listRating);
            
            return ResponseFactory.Success(result,"Get List success");
        }
        catch (Exception e)
        {
            return ResponseFactory.Exception<List<RatingViewModel>>(e);
        }
    }


}