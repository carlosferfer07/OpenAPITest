using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService.Models.Charts
{
    public class MonthValue
    {
        public string Month { get; set; }
        public decimal Value { get; set; }

        public MonthValue(string month, decimal value)
        {
            Month = month;
            Value = value;
        }
    }
}
