using System;

namespace SharedViewModal.ViewModels;

public class CheckoutViewModel
{
    public UserViewModel? UserViewModel {get;set;}
    public List<CartItemViewModel>? CartItemViewModels {get;set;}

    public OrderViewModel? OrderViewModel {get;set;}
}
