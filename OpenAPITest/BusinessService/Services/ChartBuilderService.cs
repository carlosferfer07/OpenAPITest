using BusinessService.Interfaces;
using BusinessService.Models.Charts;

namespace BusinessService.Services
{
    public class ChartBuilderService: IChartBuildService
    {
        private readonly IChartDataService _chartData;
        public ChartBuilderService(IChartDataService chartDataService) => _chartData = chartDataService;

        public async Task<object> BuildOvertimeByMonth(ChartSpec plan)
        {
            if (plan.Year is null)
                throw new InvalidOperationException("Year is required.");

            var data = await _chartData.GetOvertimeByMonthAsync(plan.Year.Value);

            return new
            {
                type = plan.ChartType,
                title = plan.Title,
                labels = data.Select(x => x.Month),
                datasets = new[] { new {
                    label = "Overtime hours",
                    data = data.Select(x => x.Value)
                    }
                }
            };
        }

        public async Task<object> BuildHoursByEmployee(ChartSpec plan)
        {
            var from = plan.From ?? DateTime.UtcNow.AddMonths(-1);
            var to = plan.To ?? DateTime.UtcNow;

            var data = await _chartData.GetTotalHoursByEmployeeAsync(from, to);

            return new
            {
                type = plan.ChartType,
                title = plan.Title,
                labels = data.Select(x => x.Name),
                datasets = new[] { new {
                    label = "Total hours",
                    data = data.Select(x => x.Value)
                    }
                }
            };
        }

        public async Task<object> BuildAbsencesByWeek(ChartSpec plan)
        {
            if (plan.Year is null)
                throw new InvalidOperationException("Year is required.");

            var data = await _chartData.GetAbsencesByWeekAsync(plan.Year.Value);

            return new
            {
                type = plan.ChartType,
                title = plan.Title,
                labels = data.Select(x => $"W{x.WeekNumber}"),
                datasets = new[] { new {
                    label = "Absent hours",
                    data = data.Select(x => x.Value)
                    }
                }
            };
        }

    }
}
