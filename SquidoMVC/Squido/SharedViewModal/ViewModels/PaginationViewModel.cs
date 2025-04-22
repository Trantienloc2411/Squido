namespace SharedViewModal.ViewModels;

public class PaginationViewModel<T>
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public List<CategoryViewModel>? Categories { get; set; }
}