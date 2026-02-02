using BusinessService.Models.Charts;

namespace BusinessService.Interfaces
{
    public interface IChartAiService
    {
        Task<ChartSpec> CreatePlanAsync(string prompt, CancellationToken ct = default);
    }
}
