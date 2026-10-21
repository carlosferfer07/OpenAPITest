using BusinessService.Models.Charts;

namespace BusinessService.Interfaces
{
    public interface IChartDataService
    {
        Task<IReadOnlyList<MonthValue>> GetOvertimeByMonthAsync(int year, int? employeeId = null);
        Task<IReadOnlyList<NameValue>> GetTotalHoursByEmployeeAsync(DateTime from, DateTime to, int? employeeId = null);
        Task<IReadOnlyList<WeekValue>> GetAbsencesByWeekAsync(int year, int? employeeId = null);
        Task<DateTime?> GetLatestRecordedDateAsync();
    }
}
