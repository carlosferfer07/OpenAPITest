using BusinessService.Models.Charts;

namespace BusinessService.Interfaces
{
    public interface IChartDataService
    {
        Task<IReadOnlyList<MonthValue>> GetOvertimeByMonthAsync(int year);
        Task<IReadOnlyList<NameValue>> GetTotalHoursByEmployeeAsync(DateTime from, DateTime to);
        Task<IReadOnlyList<WeekValue>> GetAbsencesByWeekAsync(int year);
    }
}
