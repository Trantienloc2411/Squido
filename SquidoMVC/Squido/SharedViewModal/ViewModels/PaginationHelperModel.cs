namespace SharedViewModal.ViewModels;
public class PaginationHelperModel
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public string? Keyword { get; set; }
    public List<int>? Categories { get; set; }
    public string Action { get; set; } = "Store";
    public string Controller { get; set; } = "HomeMvc";
}
