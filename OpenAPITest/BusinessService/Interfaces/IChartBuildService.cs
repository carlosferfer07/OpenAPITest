using BusinessService.Models.Charts;

namespace BusinessService.Interfaces
{
    public interface IChartBuildService
    {
        Task<object> BuildOvertimeByMonth(ChartSpec plan);
        Task<object> BuildHoursByEmployee(ChartSpec plan);
        Task<object> BuildAbsencesByWeek(ChartSpec plan);
    }
}
