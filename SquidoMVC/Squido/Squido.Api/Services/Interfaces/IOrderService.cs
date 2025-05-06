using System;
using SharedViewModal.ViewModels;

namespace WebApplication1.Services.Interfaces;

public interface IOrderService
{
    Task<ResponseMessage<OrderResultViewModel>> CreateOrderAsync(OrderViewModel orderViewModel, List<OrderItemViewModel> orderItemViewModels);
    Task<ResponseMessage<OrderResultViewModel>> GetOrderAsync(string orderId);
    Task<ResponseMessage<List<OrderResultViewModel>>> GetOrderByUserIdAsync(Guid userId);
    Task<ResponseMessage<List<OrderViewModel>>> GetAllOrdersAsync(string? keyword);
}
