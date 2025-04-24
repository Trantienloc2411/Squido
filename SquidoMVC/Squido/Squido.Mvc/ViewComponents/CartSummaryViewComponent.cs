using System;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using Squido.Helper;

namespace Squido.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItem = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? [];

            int itemCount = ItemCountInSession(cartItem);

            return View(new CartSummaryViewModel
            {
                ItemCount = itemCount
            });

        }

        private int ItemCountInSession (List<CartItem> cartItem) {
            int itemCount = 0;
            itemCount = cartItem.Count;
            return itemCount;
        }

    }
}
