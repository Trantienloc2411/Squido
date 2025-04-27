using System;

namespace SharedViewModal.ViewModels;

public class OrderItemViewModel
{
    public Guid? Id { get; set; }
    public string? OrderId { get; set; }
    public string? BookId { get; set; }
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

}
