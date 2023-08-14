﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_Static
{
    // Make sure API return value names and where you want to store thst name are the same. like in API get response.
    public class Rate 
    {
        public double INR { get; set; }        
        public double JPY { get; set; }
        public double USD { get; set; }
        public double NZD { get; set; }
        public double EUR { get; set; }
        public double CAD { get; set; }
        public double ISK { get; set; }
        public double PHP { get; set; }
        public double DKK { get; set; }
        public double CZK { get; set; }
        public double NGN { get; set; }
    }
}
