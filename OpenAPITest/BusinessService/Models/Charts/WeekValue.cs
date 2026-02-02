using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessService.Models.Charts
{
    public class WeekValue
    {
        public int Year { get; set; }
        public int WeekNumber { get; set; }
        public decimal Value { get; set; }

        public WeekValue(int year, int weekNumber, decimal value)
        {
            Year = year;
            WeekNumber = weekNumber;
            Value = value;
        }
    }

}
