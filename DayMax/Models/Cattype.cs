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
    public class Cattype
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeWrite { get; set; }// РОЗХІД/ДОХІД
        public int Real { get; set; }// РЕАЛЬНІ/ВІРТУАЛЬНІ
        public string Description { get; set; }
        public Cattype() { }
        public static List<Cattype> listCattype;

        public static List<Cattype> getCattype(int id)
        {
            listCattype = new List<Cattype>();

            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = "SELECT * FROM Cattype;";
            
            SqliteCommand cmd = new SqliteCommand(query, conn2);

            SqliteDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Cattype ct = new Cattype();
                ct.Id = Convert.ToInt32(r["Id"]);
                ct.Name = r["Name"].ToString();
                //ct.Description = r["Description"].ToString();
                //ct.TypeWrite = Convert.ToInt32(r["type_write"]);
                ct.Real = Convert.ToInt32(r["Real"]);
                listCattype.Add(ct);
            }
            conn2.Close();
            return listCattype;
        }
        public static Cattype getCurrentCattype(int id)
        {
            Cattype ct = new Cattype();

            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = "SELECT * FROM Cattype WHERE Id="+id+";";

            SqliteCommand cmd = new SqliteCommand(query, conn2);

            SqliteDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                
                ct.Id = Convert.ToInt32(r["Id"]);
                ct.Name = r["Name"].ToString();
                //ct.Description = r["Description"].ToString();
                //ct.TypeWrite = Convert.ToInt32(r["type_write"]);
                ct.Real = Convert.ToInt32(r["Real"]);
                
            }
            conn2.Close();
            return ct;
        }
    }
}
