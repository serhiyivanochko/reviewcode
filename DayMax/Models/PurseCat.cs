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
    class PurseCat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdListFields { get; set; }
        public int IdPurseType { get; set; }
        public int Default { get; set; }
        public int IdDirection { get; set; }

        internal PurseType PurseType
        {
            get => default(PurseType);
            set
            {
            }
        }

        public static List<PurseFields> listPurseFileds;
        public static List<PurseCat> CattsName;
        public PurseCat() { }


        public static List<PurseFields> getFields(int IdPurseCat)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();

            string query = @"SELECT pf.* FROM PurseFields pf INNER JOIN ListFields lf ON lf.IdField=pf.Id INNER JOIN PurseCat pc ON pc.IdListFields=lf.IdList WHERE pc.Id=" + IdPurseCat;
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            listPurseFileds = new List<PurseFields>();
            while (r.Read())
            {
                PurseFields buf = new PurseFields();
                buf.DbName = r["DbName"].ToString();
                buf.DbType = r["DbType"].ToString();
                buf.HtmlType = r["HtmlType"].ToString();
                buf.Title = r["Title"].ToString();
                buf.Values = r["Values"].ToString();
            }
            conn2.Close();
            return listPurseFileds;
        }
        public static List<string> GetNameStrings(int idPurseType)
        {
            List<string> buf = new List<string>();

            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            //string query = @"SELECT * FROM PurseCat";
            string query = @"SELECT * FROM ListFields WHERE IdPurseType=" + idPurseType;
            SqliteCommand cmd = new SqliteCommand(query, conn2);
            SqliteDataReader sr = cmd.ExecuteReader();

            while (sr.Read())
            {
                try
                {
                    buf.Add(sr["FieldName"].ToString());
                }
                catch (Exception ex) { }

            }

            return buf;
        }
        public static List<PurseCat> GetCatList(int idPurseType)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            //string query = @"SELECT * FROM PurseCat";
            string query = @"SELECT pc.* FROM PurseCat pc WHERE pc.IdUser=" + GlobalVariables.current_user + " AND pc.IdPurseType= " + idPurseType + " AND (pc.IdDirection=" + GlobalVariables.current_dir + " OR pc.`Default`=1)";
            SqliteCommand cmd = new SqliteCommand(query, conn2);
            SqliteDataReader sr = cmd.ExecuteReader();
            CattsName = new List<PurseCat>();
            while (sr.Read())
            {
                try
                {
                    PurseCat pc = new PurseCat();
                    pc.Id = Convert.ToInt32(sr["Id"]);
                    pc.IdPurseType = Convert.ToInt32(sr["IdPurseType"]);
                    pc.Name = sr["Name"].ToString();
                    CattsName.Add(pc);
                }
                catch (Exception ex) { }

            }

            return CattsName;
        }
    }
}
