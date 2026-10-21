using BusinessService.Interfaces;
using BusinessService.Models.Charts;

namespace BusinessService.Services
{
    public class ChartBuilderService: IChartBuildService
    {
        private readonly IChartDataService _chartData;
        public ChartBuilderService(IChartDataService chartDataService) => _chartData = chartDataService;

        public async Task<ChartResponse> BuildOvertimeByMonth(ChartSpec plan)
        {
            var year = plan.Year ?? await GetLatestYearAsync();
            var data = await _chartData.GetOvertimeByMonthAsync(year, plan.EmployeeId);

            return BuildResponse(plan, data.Select(x => x.Month), "Overtime hours", data.Select(x => x.Value));
        }

        public async Task<ChartResponse> BuildHoursByEmployee(ChartSpec plan)
        {
            var (from, to) = await ResolveDateRangeAsync(plan);
            var data = await _chartData.GetTotalHoursByEmployeeAsync(from, to, plan.EmployeeId);

            return BuildResponse(plan, data.Select(x => x.Name), "Total hours", data.Select(x => x.Value));
        }

        public async Task<ChartResponse> BuildAbsencesByWeek(ChartSpec plan)
        {
            var year = plan.Year ?? await GetLatestYearAsync();
            var data = await _chartData.GetAbsencesByWeekAsync(year, plan.EmployeeId);

            return BuildResponse(plan, data.Select(x => $"W{x.WeekNumber}"), "Absent hours", data.Select(x => x.Value));
        }

        private static ChartResponse BuildResponse(ChartSpec plan, IEnumerable<string> labels, string datasetLabel, IEnumerable<decimal> values)
        {
            return new ChartResponse
            {
                Type = plan.ChartType,
                Title = plan.Title,
                Labels = labels.ToList(),
                Datasets = [new ChartDataset { Label = datasetLabel, Data = values.ToList() }]
            };
        }

        /// <summary>
        /// When the user doesn't mention a year, show the most recent year we actually have data for
        /// instead of failing (or showing an empty chart for the current year).
        /// </summary>
        private async Task<int> GetLatestYearAsync()
        {
            var latest = await _chartData.GetLatestRecordedDateAsync();
            return latest?.Year ?? DateTime.UtcNow.Year;
        }

        /// <summary>
        /// Works out which dates to use when the prompt was vague. If only a year was given we take the
        /// whole year; otherwise we fall back to the last month that actually has data in it.
        /// </summary>
        private async Task<(DateTime from, DateTime to)> ResolveDateRangeAsync(ChartSpec plan)
        {
            if (plan.From is null && plan.To is null && plan.Year is int year)
                return (new DateTime(year, 1, 1), new DateTime(year, 12, 31));

            var to = plan.To
                ?? plan.From?.AddMonths(1)
                ?? await _chartData.GetLatestRecordedDateAsync()
                ?? DateTime.UtcNow;
            var from = plan.From ?? to.AddMonths(-1);

            return (from, to);
        }
    }
}
