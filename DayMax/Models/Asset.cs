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
    public class Asset
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int IdPurse { get; set; }
        public double Summ_buy { get; set; }
        public double Current_price { get; set; }

        public Purse Purse
        {
            get => default(Purse);
            set
            {
            }
        }
    }
}