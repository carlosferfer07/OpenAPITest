using BusinessService.Interfaces;
using BusinessService.Models.Charts;
using DataConnection.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessService.Services
{
    public class ChartDataService : IChartDataService
    {
        private readonly AppDbContext _db;

        public ChartDataService(AppDbContext db) => _db = db;

        public async Task<IReadOnlyList<MonthValue>> GetOvertimeByMonthAsync(int year)
        {
            var rows = await _db.Overtimes
                .Where(x => x.Date.Year == year)
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new
                {
                    g.Key.Month,
                    Value = g.Sum(x => x.Hours)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();
            return rows
                .Select(x => new MonthValue(
                    month: CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month),
                    value: x.Value))
                .ToList();
        }

        public async Task<IReadOnlyList<NameValue>> GetTotalHoursByEmployeeAsync(DateTime from, DateTime to)
        {
            var rows = await _db.WorkHours
                .Where(x => x.Date >= from && x.Date <= to)
                .Join(_db.Employees,
                    wh => wh.EmployeeId,
                    e => e.Id,
                    (wh, e) => new { e.FirstName, e.LastName, wh.Hours })
                .GroupBy(x => new { x.FirstName, x.LastName })
                .Select(g => new {
                    Name = $"{g.Key.FirstName} {g.Key.LastName}",
                    Value = g.Sum(x => x.Hours)
                })
                .OrderByDescending(x => x.Value)
                .ToListAsync();

            return rows.Select(x => new NameValue(x.Name, x.Value)).ToList();
        }

        public async Task<IReadOnlyList<WeekValue>> GetAbsencesByWeekAsync(int year)
        {
            var rows = await _db.Absences
                .Where(x => x.Year == year)
                .GroupBy(x => new { x.Year, x.WeekNumber })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    WeekNumber = g.Key.WeekNumber,
                    Value = g.Sum(x => x.HoursAbsent)
                })
                .OrderBy(x => x.WeekNumber)
                .ToListAsync();

            return rows.Select(x => new WeekValue(x.Year, x.WeekNumber, x.Value)).ToList();
        }
    }
}
