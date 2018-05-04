using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DayMax.Models
{
    public class Investment
    {
        public Purse purse { get; set; }
        public int summDays { get; set; }
        public double balance { get; set; }
        public string currency { get; set; }
        public double finn { get; set; }
        public double sum_spys { get; set; }
        public double vyv_kost { get; set; }
        public double doh { get; set; }
        public double prybutok { get; set; }
        public string prybutokSTR { get; set; }
        public Dictionary<int, String> di { get; set; }
        public double[][] monthArr { get; set; }
        public double totalHours { get; set; }
        public int totalDays { get; set; }
        public int totalMonth { get; set; }
        public int currMonth { get; set; }
        public int daysOk { get; set; }

        public Purse Purse
        {
            get => default(Purse);
            set
            {
            }
        }
    }
}