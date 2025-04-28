using System;

namespace SharedViewModal.ViewModels;

public class OrderViewModel
{
    public string? Id { get; set; }

    public Guid? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? ConfirmDate { get; set; }

    public DateTime? CompleteDate { get; set; }
    public decimal? ShippingFee { get; set; } = 0;

    public PaymentMethodEnum? PaymentMethod { get; set; }

    public OrderStatusEnum? Status { get; set; }

    public string? OrderNote { get; set; }
}

public enum PaymentMethodEnum
{
    CreditCard,
    Paypal,
    ApplePay,
}

public enum PaymentMethod
{
    Credit,
    Paypal,
    ApplePay
}
