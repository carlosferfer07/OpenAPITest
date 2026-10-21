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

        public async Task<IReadOnlyList<MonthValue>> GetOvertimeByMonthAsync(int year, int? employeeId = null)
        {
            var query = _db.Overtimes.Where(x => x.Date.Year == year);
            if (employeeId is int id)
                query = query.Where(x => x.EmployeeId == id);

            var rows = await query
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

        public async Task<IReadOnlyList<NameValue>> GetTotalHoursByEmployeeAsync(DateTime from, DateTime to, int? employeeId = null)
        {
            // "to" is meant as a whole day, so anything logged on that day still counts
            var start = from.Date;
            var endExclusive = to.Date.AddDays(1);

            var query = _db.WorkHours.Where(x => x.Date >= start && x.Date < endExclusive);
            if (employeeId is int id)
                query = query.Where(x => x.EmployeeId == id);

            var rows = await query
                .Join(_db.Employees,
                    wh => wh.EmployeeId,
                    e => e.Id,
                    (wh, e) => new { e.Id, e.FirstName, e.LastName, wh.Hours })
                // Group by id so two people with the same name don't get merged together
                .GroupBy(x => new { x.Id, x.FirstName, x.LastName })
                .Select(g => new {
                    g.Key.FirstName,
                    g.Key.LastName,
                    Value = g.Sum(x => x.Hours)
                })
                .OrderByDescending(x => x.Value)
                .ToListAsync();

            return rows.Select(x => new NameValue($"{x.FirstName} {x.LastName}".Trim(), x.Value)).ToList();
        }

        public async Task<IReadOnlyList<WeekValue>> GetAbsencesByWeekAsync(int year, int? employeeId = null)
        {
            var query = _db.Absences.Where(x => x.Year == year);
            if (employeeId is int id)
                query = query.Where(x => x.EmployeeId == id);

            var rows = await query
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

        public async Task<DateTime?> GetLatestRecordedDateAsync()
        {
            return await _db.WorkHours.MaxAsync(x => (DateTime?)x.Date);
        }
    }
}
