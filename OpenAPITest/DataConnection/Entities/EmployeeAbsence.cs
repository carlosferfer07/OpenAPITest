using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService.Models.Employee
{
    public class EmployeeAbsence
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int WeekNumber { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal HoursAbsent { get; set; }

        public Employee? Employee { get; set; }
    }

}
