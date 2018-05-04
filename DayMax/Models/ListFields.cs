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

namespace DayMax.Models
{
    class ListFields
    {
        public int IdField { get; set; }
        public string FieldName { get; set; }
        public int PageForm { get; set; }
        public int IdPurseType { get; set; }
        public ListFields() { }
        
    }
}
