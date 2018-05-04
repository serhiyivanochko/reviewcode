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
using System.Data;
using Mono.Data.Sqlite;
using DayMax.Models;

namespace DayMax.Models
{
    class PurseProp
    {
        
        public int IdPurse { get; set; }
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
        public PurseProp() { }
        public PurseProp(bool tmp)
        {}
    }
}
