using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService.Models.Charts
{
    public class ChartSpec
    {
        public string Dataset { get; set; } = string.Empty;   // enum-like
        public string ChartType { get; set; } = string.Empty; // "bar" | "line" | "pie"
        public string Title { get; set; } = string.Empty;

        public int? Year { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? EmployeeId { get; set; }
    }
}
