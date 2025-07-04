﻿

namespace SharedViewModal.ViewModels;

public class BookViewModel
{
    public string? Id {get;set;}
    public string? Title {get;set;}
    public string? CategoryName {get;set;}
    public Guid AuthorId {get;set;} 
    public string? AuthorName {get;set;}
    public int Quantity {get;set;}
    public decimal Price {get;set;}
    public int BuyCount { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    
}

