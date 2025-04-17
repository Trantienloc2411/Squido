namespace SharedViewModal.ViewModels;

public class ViewBookDetailViewModel
{
    
    public BookViewModel Book {get;set;} = new BookViewModel();
    public CategoryViewModel Category {get;set;} = new CategoryViewModel();
    
    public List<BookViewModel> BookRelated {get;set;} = new();
    
    public string BookDescription {get;set;} = "";
    //Author
    public string? Bio { get; set; } = "";
    public string? ImageUrl { get; set; } = "";

    //Rating
    public double RatingValueAverage { get; set; } = 0;
    


}