using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_Static
{
    public class Root // Root class is a Main class. API return rates in a rate it return all currency name with value.
    {
        public Rate rates { get; set; } // get all record in rates and set in rate class as currency name wise.
        public long timestamp;
        public string license;
    }
}
