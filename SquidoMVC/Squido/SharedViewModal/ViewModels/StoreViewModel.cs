

namespace SharedViewModal.ViewModels;

public class StoreViewModel
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Count { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Count / PageSize);
}