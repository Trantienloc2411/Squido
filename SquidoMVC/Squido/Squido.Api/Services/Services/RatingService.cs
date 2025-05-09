using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class RatingService : IRatingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseMessage<CreateRatingViewModel>> CreateRating(CreateRatingViewModel model)
    {
        try
        {
            if (model == null)
            {
                return new ResponseMessage<CreateRatingViewModel>
                {
                    IsSuccess = false,
                    Message = "Invalid rating data",
                    Data = null
                };
            }

            if (model.RatingValue < 1 || model.RatingValue > 5)
            {
                return new ResponseMessage<CreateRatingViewModel>
                {
                    IsSuccess = false,
                    Message = "Invalid rating value",
                    Data = null
                };
            }

            var rating = _mapper.Map<Rating>(model);
            if (rating == null)
            {
                return new ResponseMessage<CreateRatingViewModel>
                {
                    IsSuccess = false,
                    Message = "Failed to map rating data",
                    Data = null
                };
            }

            rating.CreatedDate = DateTime.Now;
            rating.Id = Guid.NewGuid().ToString();

            await _unitOfWork.RatingRepository.AddAsync(rating);
            await _unitOfWork.SaveAsync();

            return new ResponseMessage<CreateRatingViewModel>
            {
                IsSuccess = true,
                Message = "Rating created successfully",
                Data = model
            };
        }
        catch (Exception ex)
        {
            return new ResponseMessage<CreateRatingViewModel>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseMessage<List<RatingViewModel>>> GetRatingsByBookId(string bookId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bookId))
            {
                return new ResponseMessage<List<RatingViewModel>>
                {
                    IsSuccess = false,
                    Message = "Invalid book ID",
                    Data = null
                };
            }

            var ratings = await _unitOfWork.RatingRepository.GetAllWithIncludeAsync(
                r => r.BookId == bookId,
                r => r.Customer
            );

            var ratingViewModels = _mapper.Map<List<RatingViewModel>>(ratings);
            return new ResponseMessage<List<RatingViewModel>>
            {
                IsSuccess = true,
                Message = "Ratings retrieved successfully",
                Data = ratingViewModels
            };
        }
        catch (Exception ex)
        {
            return new ResponseMessage<List<RatingViewModel>>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseMessage<List<RatingViewModel>>> GetRatingsByUserId(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return new ResponseMessage<List<RatingViewModel>>
                {
                    IsSuccess = false,
                    Message = "Invalid user ID",
                    Data = null
                };
            }

            var ratings = await _unitOfWork.RatingRepository.GetAllWithIncludeAsync(
                r => r.CustomerId == userId,
                r => r.Customer
            );

            var ratingViewModels = _mapper.Map<List<RatingViewModel>>(ratings);
            return new ResponseMessage<List<RatingViewModel>>
            {
                IsSuccess = true,
                Message = "Ratings retrieved successfully",
                Data = ratingViewModels
            };
        }
        catch (Exception ex)
        {
            return new ResponseMessage<List<RatingViewModel>>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = null
            };
        }
    }
}