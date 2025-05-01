namespace WebApplication1.Services.Interfaces;
public interface IStatsService
{
    Task<StatViewModel> GetStatsAsync();
}
