namespace SharedViewModal.ViewModels;

public class ViewBookDetailViewModel
{
    
    public BookViewModel? Book {get;set;} 
    public CategoryViewModel? Category {get;set;}
    
    public List<BookViewModel>? BookRelated {get;set;} 
    
    public string? BookDescription {get;set;} 
    //Author
    public string? Bio { get; set; } 
    public string? ImageUrl { get; set; }

    //Rating
    public double RatingValueAverage { get; set; } = 0;
    


}