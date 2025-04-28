using System;

namespace SharedViewModal.ViewModels;

public class OrderResultViewModel
{
    public string? Id { get; set; }
    public DateTime? OrderDate { get; set; }
    public OrderStatusEnum Status {get;set;}
    public PaymentMethod PaymentMethod { get; set; }
    public UserViewModel? UserViewModel  {get;set;}
    public string? OrderNote {get;set;}
    public List<OrderItemViewModel>? OrderItemViewModels {get;set;}  =[];
}

public enum OrderStatusEnum
{
    Pending,
    Confirmed,
    Delivered,
    Completed,
    Canceled 
}
