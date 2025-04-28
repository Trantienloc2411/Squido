using System;
using System.Text;
using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
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

            foreach (var orderItem in orderItemViewModels)
            {
                var orderItemEntity = mapper.Map<OrderItem>(orderItem);
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
                Data = mapper.Map<OrderResultViewModel>(order)
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


    private static string GenId()
    {
        string unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        string currentDate = DateTime.UtcNow.ToString("yyyyMMdd");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(unixTime).Append(currentDate);
        return stringBuilder.ToString();
    }
}
