using SharedViewModal.ViewModels;

public class StatViewModel
{
    public int TotalBooks { get; set; }
    public int TotalCategories { get; set; }
    public int TotalCustomers { get; set; } 
    public decimal TotalRevenues { get; set; }  
    
    public List<BookViewModel>? TopBooks { get; set; } = [];
    public List<CategoryViewModel>? TopCategories { get; set; } = [];
}

