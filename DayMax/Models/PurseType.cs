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
    class PurseType
    {
        
        public string Name { get; set; }
        public int IdListFields { get; set; }
        
        public PurseType() { }
        
        public static PurseType getFields(SqliteConnection conn, int IdPurseCat)
        {
                conn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT pf.* FROM PurseFields pf WHERE pf.Id=" + IdPurseCat, conn);
            SqliteDataReader r = cmd.ExecuteReader();
            PurseType pt = new PurseType();
            while (r.Read()) {
                
                pt.Name = r["Name"].ToString();
            }
            conn.Close();
            return pt;
        }
    }
}
