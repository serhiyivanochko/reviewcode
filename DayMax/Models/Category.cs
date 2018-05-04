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
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Parent { get; set; }
        public int IdPurseType { get; set; }
        public int NotShow { get; set; }
        public int CatType { get; set; }
        public int Default { get; set; }
        public int IdDirection { get; set; }
        public List<Category> Children { get; set; }
        public static List<Category> list { get; set; }

        internal PurseType PurseType
        {
            get => default(PurseType);
            set
            {
            }
        }

        public Cattype Cattype
        {
            get => default(Cattype);
            set
            {
            }
        }

        public Category()
        {

            Children = new List<Category>();
        }

        public static List<Category> getCatTree(int parent, int current_user = 1)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = "SELECT * FROM Category c WHERE c.Parent=" + parent.ToString() + " AND c.IdUser=" + current_user.ToString();
            SqliteCommand cmd = new SqliteCommand(query, conn2);
            list = new List<Category>();
            SqliteDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Category cat = new Category();
                cat.Id = Convert.ToInt32(r["Id"]);
                cat.Name = r["Name"].ToString();
                cat.Description = r["Description"].ToString();
                cat.Parent = Convert.ToInt32(r["Parent"]);
                cat.NotShow = Convert.ToInt32(r["NotShow"]);
                cat.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                cat.Default = Convert.ToInt32(r["Default"]);
                cat.CatType = Convert.ToInt32(r["CatType"]);
                list.Add(cat);
            }
            conn2.Close();
            foreach (var item in list)
            {
                item.Children = GetChildrenCatts(item.Id, current_user);
            }

            return list;
        }
        public static List<Category> GetChildrenCatts(int parent, int current_user = 1)
        {
            List<Category> Childrens = new List<Category>();
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = "SELECT * FROM Category c WHERE c.Parent=" + parent.ToString() + " AND c.IdUser=" + current_user.ToString();
            SqliteCommand cmd = new SqliteCommand(query, conn2);

            SqliteDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Category cat = new Category();
                cat.Id = Convert.ToInt32(r["Id"]);
                cat.Name = r["Name"].ToString();
                cat.Description = r["Description"].ToString();
                cat.Parent = Convert.ToInt32(r["Parent"]);
                cat.NotShow = Convert.ToInt32(r["NotShow"]);
                cat.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                cat.Default = Convert.ToInt32(r["Default"]);
                cat.CatType = Convert.ToInt32(r["CatType"]);
                Childrens.Add(cat);
            }
            return Childrens;
        }
        /*
        public static string getCatTreeSTR(ref string str, SqliteConnection conn, int parent, int current_user, int current_dir, int type_write = 2)
        {
            string query = @"SELECT c.* FROM Category c 
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                WHERE c.Parent=" + parent.ToString() + " AND c.IdUser=" + current_user.ToString() + " AND (c.IdDirection=" + current_dir + " OR c.`Default`=1) AND c.NotShow=0";
            if (type_write != 2)
                query += " AND ct.TypeWrite=" + type_write;
            List<Category> list = conn.Query<Category>(query);
            if (list.Count > 0)
            {
                str += "<ul>";
                foreach (var item in list)
                {
                    str += "<li><input type='checkbox' value='" + item.Id + "'><span>" + item.Name+"</span>";
                    getCatTreeSTR(ref str, conn, item.Id, current_user,current_dir);
                    str += "</li>";
                }
                str += "</ul>";
            }
            return str;
        }
        */
    }
}
