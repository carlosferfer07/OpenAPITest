using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService.Models.Charts
{
    public class NameValue
    {
        /// <summary>
        /// String representing the name associated with the value. e.g., employee name. Ana Silva
        /// </summary>
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public NameValue(string name, decimal value)
        {
            Name = name;
            Value = value;
        }
    }
}
