using SharedViewModal.ViewModels;

namespace WebApplication1.Services.Interfaces;

public interface IRatingService
{
    Task<ResponseMessage<CreateRatingViewModel>> CreateRating(CreateRatingViewModel model);
    Task<ResponseMessage<List<RatingViewModel>>> GetRatingsByBookId(string id);
    
}