using System;
using SharedViewModal.ViewModels;

namespace WebApplication1.Services.Interfaces;

public interface IOrderService
{
    Task<ResponseMessage<OrderResultViewModel>> CreateOrderAsync(OrderViewModel orderViewModel);
}
