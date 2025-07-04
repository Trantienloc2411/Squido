using System;
using System.Text;
using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Helper;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
{

    public async Task<ResponseMessage<OrderResultViewModel>> CreateOrderAsync(OrderViewModel orderViewModel, List<OrderItemViewModel> orderItemViewModels)
    {
        try
        {
            string orderId = GenId();
            foreach (var orderItem in orderItemViewModels)
            {
                orderItem.OrderId = orderId;
            }

            var order = new Order();
            order = mapper.Map<Order>(orderViewModel);
            order.Id = orderId;

            await unitOfWork.OrderRepository.AddAsync(order);
            unitOfWork.Save();
            var bookList = await unitOfWork.BookRepository.GetAllWithIncludeAsync(
                t => t.IsDeleted == false && 
                     t.Category.IsDeleted == false && 
                     t.Author.IsDeleted == false, 
                t=> t.Category, 
                t => t.Author);
            var orderItemEntities = orderItemViewModels
                .Join(bookList,
                    orderItem => orderItem.BookId,
                    book => book.Id,
                    (orderItem, book) => new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderItem.OrderId,
                        BookId = orderItem.BookId,
                        Title = book.Title,
                        AuthorName = book.Author?.FullName ?? string.Empty,
                        CategoryName = book.Category?.Name ?? string.Empty,
                        Quantity = orderItem.Quantity,
                        UnitPrice = book.Price,
                        Order = order,
                        Book = book
                    })
                .ToList();

            foreach (var orderItemEntity in orderItemEntities)
            {
                await unitOfWork.OrderItemRepository.AddAsync(orderItemEntity);
            }

            unitOfWork.Save();

            var user = await unitOfWork.UserRepository.GetSingleWithIncludeAsync(u => u.Id == order.CustomerId, u => u.Orders);
            if (user == null)
            {
                return new ResponseMessage<OrderResultViewModel>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                };
            }

            var result = new OrderResultViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = (OrderStatusEnum)order.Status,
                PaymentMethod = (PaymentMethod)order.PaymentMethod,
                UserViewModel = mapper.Map<UserViewModel>(user),
                OrderNote = order.OrderNote,
                OrderItemViewModels = mapper.Map<List<OrderItemViewModel>>(order.OrderItems)
            };

            return new ResponseMessage<OrderResultViewModel>
            {
                IsSuccess = true,
                Message = "Order created successfully",
                Data = result
            };
        }
        catch (System.Exception ex)
        {
            return new ResponseMessage<OrderResultViewModel>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = null
            };
        }
        finally
        {
            unitOfWork.Dispose();
        }
    }

    public async Task<ResponseMessage<OrderResultViewModel>> GetOrderAsync(string orderId)
    {
        try
        {
            var order = await unitOfWork.OrderRepository.GetSingleWithIncludeAsync(t => t.Id == orderId, t=> t.OrderItems);
            if (order == null)
            {
                return new ResponseMessage<OrderResultViewModel>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Order not found",
                };
            }
            return new ResponseMessage<OrderResultViewModel>()
            {
                Data = mapper.Map<OrderResultViewModel>(order),
                IsSuccess = true,
                Message = "Order found",
            };
        }
        catch (Exception e)
        {
            return new ResponseMessage<OrderResultViewModel>()
            {
                Data = null,
                IsSuccess = false,
                Message = e.Message,
            };
        }
    }

    public async Task<ResponseMessage<List<OrderResultViewModel>>> GetOrderByUserIdAsync(Guid userId)
    {
        try
        {
            var order = await unitOfWork.OrderRepository.GetAllWithIncludeAsync(
                c => c.CustomerId == userId, 
                t => t.OrderItems,
                t => t.Customer);
            if(order == null) 
            {
                return new ResponseMessage<List<OrderResultViewModel>>
                {
                    IsSuccess = false,
                    Message = "Order not found",
                    Data = null
                };
            }
            return new ResponseMessage<List<OrderResultViewModel>>
            {
                IsSuccess = true,
                Message = "Order found",
                Data = mapper.Map<List<OrderResultViewModel>>(order)
            };
        }
        catch (Exception e)
        {
            return new ResponseMessage<List<OrderResultViewModel>>
            {
                IsSuccess = false,
                Message = e.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseMessage<List<OrderViewModel>>> GetAllOrdersAsync(string? keyword)
    {
        try
        {
            var order = await unitOfWork.OrderRepository.GetAllWithIncludeAsync(t=> true);

            if (keyword is null)
            {
                return new ResponseMessage<List<OrderViewModel>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = mapper.Map<List<OrderViewModel>>(order)
                };
            }
            else
            {
                var result = order.Where(c => c.Id.ToLower().Contains(keyword.ToLower()));
                return new ResponseMessage<List<OrderViewModel>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = mapper.Map<List<OrderViewModel>>(result)
                };
            }
        }
        catch (Exception e)
        {
            return new ResponseMessage<List<OrderViewModel>>
            {
                IsSuccess = false,
                Message = e.Message,
                Data = null
            };
        }
    }


    private static string GenId()
    {
        string unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        string currentDate = DateTime.UtcNow.ToString("yyyyMMdd");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(unixTime).Append(currentDate);
        return stringBuilder.ToString();
    }
}
