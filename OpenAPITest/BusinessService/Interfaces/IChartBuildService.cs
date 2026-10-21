using BusinessService.Models.Charts;

namespace BusinessService.Interfaces
{
    public interface IChartBuildService
    {
        Task<ChartResponse> BuildOvertimeByMonth(ChartSpec plan);
        Task<ChartResponse> BuildHoursByEmployee(ChartSpec plan);
        Task<ChartResponse> BuildAbsencesByWeek(ChartSpec plan);
    }
}
