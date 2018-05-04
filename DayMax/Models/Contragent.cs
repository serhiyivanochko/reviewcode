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
    class Contragent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Contragent() { }
        public static List<string> getContrnames(SqliteConnection conn2) {
            List<string> list = new List<string>();
            conn2.Open();

            string query = @"
                SELECT c.Id,c.Name, c.Description
                FROM Contragent c
                WHERE c.IdUser=" + GlobalVariables.current_user.ToString();
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                list.Add(r["Name"].ToString());
            }

                conn2.Close();
            return list;
        }
        public static List<int> getContrIds()
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            List<int> list = new List<int>();
            conn2.Open();

            string query = @"
                SELECT c.Id,c.Name, c.Description
                FROM Contragent c
                WHERE c.IdUser=" + GlobalVariables.current_user.ToString();
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                list.Add(Convert.ToInt32(r["Id"]));
            }

            conn2.Close();
            return list;
        }
    }
}
