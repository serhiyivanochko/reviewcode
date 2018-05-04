﻿using System;
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

namespace DayMax.Models
{
    class Currency
    {
        
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public Currency(){}
        
    }
}
