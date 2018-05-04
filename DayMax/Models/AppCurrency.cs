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
    public class AppCurrency
    {
        public int IdCurrency { get; set; }
        public short IsMain { get; set; }
        public AppCurrency() { }
        public static Dictionary<int, string> getAppCurr(SqliteConnection conn2)
        {
            bool con = false;
            if(conn2.State == ConnectionState.Closed) { conn2.Open(); con = true; }
            
            Dictionary<int, string> list = new Dictionary<int, string>();
            string query = @"SELECT c.Id,c.ShortName
                FROM Currency c";
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                list.Add(Convert.ToInt32(r["Id"]), r["ShortName"].ToString());
            }
            if(con==true)
            conn2.Close();
            return list;
        }
        public static List<int> getAppCurrId(SqliteConnection conn2) {
            conn2.Open();
            List<int> list = new List<int>();
            string query = @"SELECT c.Id,c.ShortName
                FROM Currency c";
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                list.Add(Convert.ToInt32(r["Id"]));
            }
            conn2.Close();
            return list;
        }
        public static List<string> getAppCurrSTR(SqliteConnection conn2)
        {
            conn2.Open();
            List<string> list = new List<string>();
            string query = @"SELECT c.Id,c.ShortName
                FROM Currency c";
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                list.Add(r["ShortName"].ToString().ToUpper());
            }
            conn2.Close();
            return list;
        }
    }
}
