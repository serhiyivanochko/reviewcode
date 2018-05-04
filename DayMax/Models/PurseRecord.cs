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
using System.Collections.Specialized;
using static Android.Widget.AdapterView;
using System.Globalization;
using Android.Content.Res;

namespace DayMax.Models
{
    public class PurseRecord
    {

        //public int IdPurse { get; set; }
        //public int IdCategory { get; set; }
        //public int IdContragent { get; set; }


        public string Purse { get; set; }
        public string Category { get; set; }
        public string Contragent { get; set; }
        public string type_write { get; set; }
        public int Id { get; set; }
        public DateTime DateRecord { get; set; }
        public string Description { get; set; }
        public double Summ { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string Tag { get; set; }
        public int ShortName { get; set; }

        public Category Category1
        {
            get => default(Category);
            set
            {
            }
        }

        public static List<PurseRecord> listcashflow;
        public static List<PurseRecord> bufflistcashflow;
        public static List<PurseRecord> listExpenses;
        public static List<PurseRecord> listEarnings;
        public static List<PurseRecord> listPurseItem;
        public static List<Dictionary<string, string>> list2;
        public static List<Asset> listAssets;
        public static List<Category> CategoryNames;
        public static List<int> CategoryIds;
        public static List<Category> Categories;
        public static List<Cattype> catTypes;


        public PurseRecord()
        {
            Amount = 1;
        }
        #region Інфа по інвестиції
        public static Investment investmentsP(SqliteConnection conn2, Purse purse, Context context)
        {
            conn2.Open();
            List<Purse> list = new List<Models.Purse>();

            String query = @"SELECT pr.Summ,pr.Amount,pr.DateRecord,c.CatType as idtype,ct.TypeWrite,p.Name,p.DateStart,p.Id as purseId,cr.ShortName as CurName,p.IdDirection
                FROM PurseRecord pr
                INNER JOIN Category c ON c.Id=pr.IdCategory
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                INNER JOIN Purse p ON pr.IdPurse=p.Id 
                INNER JOIN Currency cr ON p.IdCurrency=cr.Id
                    AND p.Id=" + purse.Id.ToString() +
                  " AND p.IdUser=" + GlobalVariables.current_user.ToString() +
                  ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "");
            SqliteCommand comm = new SqliteCommand(query, conn2);
            SqliteDataReader reader = comm.ExecuteReader();




            //SqliteCommand purses = new SqliteCommand("SELECT p.* FROM Purse p INNER JOIN PurseCat pc ON pc.Id=p.IdPurseCat WHERE pc.IdPurseType=1 AND p.IdUser=" + GlobalVariables.current_user.ToString() + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : ""),conn2);


            double[][] monthArr = new double[7][];
            int i = 0;
            for (; i < monthArr.Length; i++)
            {
                monthArr[i] = new double[12];
            }
            double summ = 0, finn = 0, sum_spys = 0, doh = 0, vyv_kost = 0, balance = 0, prybutok = 0;
            int idtype = 0;
            DateTime dr = new DateTime();

            string currency = "";
            while (reader.Read())
            {
                if (purse == null)
                {
                    currency = reader["CurName"].ToString();
                }
                summ = Convert.ToDouble(reader["Summ"]) * Convert.ToInt32(reader["Amount"]);
                if (reader["TypeWrite"].ToString() == "1")
                    balance += summ;
                else
                    balance -= summ;
                dr = Convert.ToDateTime(reader["DateRecord"]);
                idtype = Convert.ToInt32(reader["idtype"]);
                if (idtype == 1 || idtype == 7)
                    idtype = 0;
                else if (idtype == 5)
                    idtype = 1;
                else if (idtype > 1 && idtype < 5)
                {
                    idtype = 2;
                    monthArr[idtype + 2][dr.Month - 1] += summ;
                }
                else if (idtype == 6)
                    idtype = 3;
                monthArr[idtype][dr.Month - 1] += summ;
                //monthArr[0][j] + monthArr[2][j] - monthArr[1][j]).ToString();
                switch (idtype)
                {
                    case 0:
                        finn += summ;
                        prybutok += summ;
                        break;
                    case 1:
                        sum_spys += summ;
                        prybutok -= summ;
                        break;
                    case 2:
                        doh += summ;
                        prybutok += summ;
                        break;
                    case 3:
                        vyv_kost += summ;
                        break;
                }

            }


            DateTime now = DateTime.Now;
            int summDays = Convert.ToInt32((now - purse.DateStart).TotalDays);
            Dictionary<int, String> di = new Dictionary<int, string>();
            di.Add(1, context.Resources.GetString(Resource.String.January));
            di.Add(2, context.Resources.GetString(Resource.String.February));
            di.Add(3, context.Resources.GetString(Resource.String.March));
            di.Add(4, context.Resources.GetString(Resource.String.April));
            di.Add(5, context.Resources.GetString(Resource.String.May));
            di.Add(6, context.Resources.GetString(Resource.String.June));
            di.Add(7, context.Resources.GetString(Resource.String.July));
            di.Add(8, context.Resources.GetString(Resource.String.August));
            di.Add(9, context.Resources.GetString(Resource.String.September));
            di.Add(10, context.Resources.GetString(Resource.String.October));
            di.Add(11, context.Resources.GetString(Resource.String.November));
            di.Add(12, context.Resources.GetString(Resource.String.December));
            //System.Globalization.DateTimeFormatInfo di = new System.Globalization.DateTimeFormatInfo();
            double totalHours = (DateTime.Today - purse.DateStart).TotalHours;
            int totalDays = Convert.ToInt32((DateTime.Today - purse.DateStart).TotalDays);
            int totalMonth = 0;
            try
            {
                totalMonth = totalDays / 30;
            }
            catch (Exception e) { }
            int daysOk = 0;
            try
            {
                daysOk = Convert.ToInt32(sum_spys / (prybutok / totalDays));
            }
            catch (Exception e) { }
            int currMonth = DateTime.Now.Month;
            Investment buf = new Investment();

            buf.purse = purse;
            buf.summDays = summDays;
            buf.balance = balance;
            buf.currency = currency;
            buf.finn = finn;
            buf.sum_spys = sum_spys;
            buf.vyv_kost = vyv_kost;
            buf.doh = doh;
            buf.prybutok = prybutok;
            buf.prybutokSTR = prybutok.ToString();
            buf.di = di;
            buf.monthArr = monthArr;
            buf.totalHours = totalHours;
            buf.totalDays = totalDays;
            buf.totalMonth = totalMonth;
            buf.currMonth = currMonth;
            buf.daysOk = daysOk;
            //purses = purses


            double tmpBalance = 0;
            for (int j = 0; j < monthArr[0].Length && j < currMonth; j++)
            {
                tmpBalance += (monthArr[0][j] + monthArr[2][j] - monthArr[1][j] - monthArr[3][j]);

            }

            conn2.Close();
            return buf;
        }
        #endregion

        #region Список активів
        public static List<Asset> assets(int id = 0)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);


            conn2.Open();
            var listCurr = AppCurrency.getAppCurr(conn2);
            string condition;
            if (id == 0)
                condition = " pc.IdPurseType=3  AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "");
            else
                condition = " pc.IdPurseType=3 AND pp.IdPurse=" + id + " AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "");
            Dictionary<int, Dictionary<string, string>> p_fields = Models.Purse.propList("", " INNER JOIN PurseCat pc ON pc.Id=p.IdPurseCat", condition);
            Dictionary<string, string> tmpD = null;
            Dictionary<int, Dictionary<string, string>> list;
            if (id != 0)
                list = Models.Purse.purseList("p.Status", "", " p.Id = " + id + " AND pt.IdPurseType=3 AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : ""));
            else
                list = Models.Purse.purseList("p.Status", "", "pt.IdPurseType=3 AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : ""));
            string tmpSTR = "";
            string summ_buy = "";
            double current_price = 0;
            listAssets = new List<Asset>();
            foreach (var idPurse in list.Keys)
            {
                Asset buf = new Asset();

                SqliteCommand comm = new SqliteCommand(@"select pr.Summ
                FROM PurseRecord pr
                where pr.IdCategory=9 AND pr.IdPurse=" + idPurse + " ORDER BY pr.DateRecord DESC LIMIT 1", conn2);
                SqliteDataReader r = comm.ExecuteReader();
                while (r.Read())
                {
                    current_price = Convert.ToDouble(r["Summ"]);
                }
                if (!p_fields.TryGetValue(idPurse, out tmpD))
                    tmpD = null;
                buf.IdPurse = idPurse;
                buf.Status = Convert.ToInt32(list[idPurse]["Status"]);
                buf.Id = Convert.ToInt32(list[idPurse]["Id"]);
                buf.DateStart = (Convert.ToDateTime(list[idPurse]["DateStart"]));
                buf.Type = list[idPurse]["Type"];
                buf.Name = list[idPurse]["Name"];
                buf.Category = list[idPurse]["Type"];
                //buf.IdPurse = Convert.ToInt32(list[idPurse]["IdPurse"]);
                summ_buy = "0";
                if (tmpD != null)
                {
                    if (!tmpD.TryGetValue("summ_buy", out summ_buy))
                        summ_buy = "0";
                }
                buf.Summ_buy = Convert.ToDouble(summ_buy);
                buf.Current_price = Convert.ToDouble(current_price);
                tmpSTR = "";
                if (tmpD != null)
                {
                    if (tmpD.TryGetValue("DateEnd", out tmpSTR))
                    {
                        tmpSTR = tmpSTR.Substring(0, 10);
                        tmpSTR = tmpSTR.Replace('.', '-');
                        buf.DateEnd = DateTime.ParseExact(tmpSTR, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }
                }
                buf.ShortName = list[idPurse]["ShortName"];
                //loansSTR += "<td>" + (Convert.ToDouble(current_price) - Convert.ToDouble(summ_buy)).ToString() + " " + list[idPurse]["ShortName"] + "</td>";

                //loansSTR += "<td>" + list[idPurse]["Summ"] + " " + list[idPurse]["ShortName"] + "/" + (Convert.ToDouble(summ_buy) - Convert.ToDouble(list[idPurse]["Summ"])).ToString() + "</td>";

                listAssets.Add(buf);
            }
            return listAssets;

        }
        #endregion

        #region Список позик
        public static List<Dictionary<string, string>> listLoans(int id = 0)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            string query_join = "";
            //if (q_post_params != null && q_post_params["purse_credit_type"] != null)
            //    query_join = "INNER JOIN PurseProp pp ON pp.IdPurse=p.Id AND pp.MetaKey='purse_credit_type' AND pp.MetaValue='" + q_post_params["purse_credit_type"] + "'";
            string condition = " p.IdPurseCat=5" + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "");
            if (id != 0)
                condition += " AND p.Id=" + id.ToString();

            var list = Models.Purse.purseList("p.Id as status", query_join, condition);
            Dictionary<int, Dictionary<string, string>> p_fields = Models.Purse.propList("", "", condition);
            list2 = new List<Dictionary<string, string>>();

            Dictionary<string, string> tmpItemParmas = new Dictionary<string, string>();
            foreach (var idPurse in list.Keys)
            {
                tmpItemParmas = list[idPurse];
                if (p_fields.ContainsKey(idPurse))
                {
                    foreach (var item in p_fields[idPurse])
                    {
                        tmpItemParmas.Add(item.Key, item.Value);
                    }
                    tmpItemParmas.Add("summ_return_left", "0");
                }



                double summ = Convert.ToDouble(tmpItemParmas["Summ1"]);
                string tmpSumm, tmpSumm2;//percent_return summ_return
                var resTry = tmpItemParmas.TryGetValue("percent_return", out tmpSumm);
                var resTry2 = tmpItemParmas.TryGetValue("summ_return", out tmpSumm2);
                if (!resTry)
                    tmpItemParmas.Add("percent_return", "0");
                if (!resTry2)
                    tmpItemParmas.Add("summ_return", "0");
                if (resTry2 && tmpSumm2.Trim() != "" && (!resTry || tmpSumm.Trim() == ""))
                {
                    double diff = Convert.ToDouble(tmpSumm2) - summ;
                    tmpSumm = tmpItemParmas["percent_return"] = Convert.ToInt32(diff / (summ / 100)).ToString();
                }
                else if (/*(!resTry2 || (resTry2 && tmpSumm2.Trim() == "")) && */resTry && tmpSumm.Trim() != "")
                {
                    tmpItemParmas["summ_return"] = (summ + ((summ / 100) * Convert.ToDouble(tmpSumm))).ToString();
                }
                tmpItemParmas["summ_return_left"] = (summ + ((summ / 100) * Convert.ToDouble(tmpSumm)) - Convert.ToDouble(tmpItemParmas["Summ2"])).ToString();
                tmpItemParmas["Summ"] = tmpItemParmas["Summ"] + " " + tmpItemParmas["ShortName"].ToUpper();
                tmpItemParmas["DateStart"] = Convert.ToDateTime(tmpItemParmas["DateStart"]).ToString("dd-MM-yyyy");
                if (tmpItemParmas.TryGetValue("date_return", out condition))
                {
                    tmpItemParmas["date_return"] = tmpItemParmas["date_return"].Replace('.', '-');
                    tmpItemParmas["date_return"] = tmpItemParmas["date_return"].Substring(0, 10);
                    //tmpItemParmas["date_return"] = Convert.ToDateTime("14-10-2016").ToString("dd-MM-yyyy");
                }
                list2.Add(tmpItemParmas);
            }

            return list2;
        }
        #endregion

        #region Список Типів категорій
        public static List<Cattype> listTypeCatts(SqliteConnection conn2)
        {
            conn2.Open();
            string query = @"SELECT * FROM Cattype";
            catTypes = new List<Cattype>();
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                Cattype buf = new Cattype();
                buf.Name = r["Name"].ToString();
                buf.Real = Convert.ToInt32(r["Real"]);
                buf.Id = Convert.ToInt32(r["Id"]);
                buf.TypeWrite = Convert.ToInt32(r["TypeWrite"]);


                catTypes.Add(buf);
            }
            return catTypes;
        }
        #endregion

        #region Список категорій
        public static List<Category> listCatts(SqliteConnection conn2, int parent, int type_write, string where = "")
        {
            conn2.Open();
            string query = @"SELECT c.* FROM Category c 
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                WHERE c.Parent=" + parent.ToString() + " AND c.CatType<6 AND c.IdUser=" + GlobalVariables.current_user.ToString() + " AND (c.IdDirection=" + GlobalVariables.current_dir + " OR c.`Default`=1) AND c.NotShow=0";
            Categories = new List<Category>();
            CategoryNames = new List<Category>();
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                Category buf = new Category();
                buf.Name = r["Name"].ToString();
                buf.CatType = Convert.ToInt32(r["CatType"]);
                buf.Id = Convert.ToInt32(r["Id"]);
                buf.Default = Convert.ToInt32(r["Default"]);
                buf.Description = r["Description"].ToString();
                buf.IdDirection = Convert.ToInt32(r["IdDirection"]);
                buf.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                buf.NotShow = Convert.ToInt32(r["NotShow"]);
                buf.Parent = Convert.ToInt32(r["Parent"]);
                Categories.Add(buf);
            }
            List<Cattype> cttypes = listTypeCatts(new SqliteConnection(GlobalVariables.dbPath));
            if (Categories.Count > 0)
            {
                if (type_write == 2)
                {
                    foreach (var item in Categories)
                    {

                        conn2.Close();
                        listChildrenCatts(conn2, item.Id, item);
                        CategoryNames.Add(item);
                    }
                }
                else
                {
                    foreach (var item in Categories)
                    {
                        foreach (var buf in cttypes)
                        {
                            if (buf.Id == item.CatType && buf.TypeWrite == type_write)
                            {
                                conn2.Close();
                                listChildrenCatts(conn2, item.Id, item);
                                CategoryNames.Add(item);
                            }
                        }
                    }
                }
            }

            conn2.Close();
            return CategoryNames;
        }

        public static void listChildrenCatts(SqliteConnection conn2, int parent, Category item)
        {
            conn2.Open();
            string query = @"SELECT c.* FROM Category c 
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                WHERE c.Parent=" + parent.ToString() + " AND c.IdUser=" + GlobalVariables.current_user.ToString() + " AND (c.IdDirection=" + GlobalVariables.current_dir + " OR c.`Default`=1) AND c.NotShow=0";


            SqliteCommand comand = new SqliteCommand(query, conn2);

            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                Category buf = new Category();
                buf.Name = r["Name"].ToString();
                buf.CatType = Convert.ToInt32(r["CatType"]);
                buf.Id = Convert.ToInt32(r["Id"]);
                buf.Default = Convert.ToInt32(r["Default"]);
                buf.Description = r["Description"].ToString();
                buf.IdDirection = Convert.ToInt32(r["IdDirection"]);
                buf.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                buf.NotShow = Convert.ToInt32(r["NotShow"]);
                buf.Parent = Convert.ToInt32(r["Parent"]);
                //CategoryNames.Add(buf.Name);
                item.Children.Add(buf);

            }
        }

        public static List<Category> listAllCatts(string where = "")
        {

            string query = @"SELECT c.* FROM Category c " + where;

            List<Category> Categories1 = new List<Category>();
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                Category buf = new Category();
                buf.Name = r["Name"].ToString();
                buf.CatType = Convert.ToInt32(r["CatType"]);
                buf.Id = Convert.ToInt32(r["Id"]);
                buf.Default = Convert.ToInt32(r["Default"]);
                buf.Description = r["Description"].ToString();
                buf.IdDirection = Convert.ToInt32(r["IdDirection"]);
                buf.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                buf.NotShow = Convert.ToInt32(r["NotShow"]);
                buf.Parent = Convert.ToInt32(r["Parent"]);
                Categories1.Add(buf);
            }
            return Categories1;
        }
        #endregion

        #region Список ID категорій
        public static List<int> listIdCatts(SqliteConnection conn2, int parent, int type_write)
        {
            CategoryIds = new List<int>();
            List<Cattype> cttypes = listTypeCatts(new SqliteConnection(GlobalVariables.dbPath));
            if (Categories.Count > 0)
            {
                if (type_write == 2)
                {
                    foreach (var item in Categories)
                    {

                        conn2.Close();
                        listChildrenIdCatts(conn2, item.Id, item);
                        CategoryIds.Add(item.Id);
                    }
                }
                else
                {
                    foreach (var item in Categories)
                    {
                        foreach (var buf in cttypes)
                        {
                            if (buf.Id == item.CatType && buf.TypeWrite == type_write)
                            {
                                conn2.Close();
                                listChildrenIdCatts(conn2, item.Id, item);
                                CategoryIds.Add(item.Id);
                            }
                        }
                    }
                }
            }


            return CategoryIds;
        }

        public static void listChildrenIdCatts(SqliteConnection conn2, int parent, Category item)
        {


            conn2.Open();
            string query = @"SELECT c.* FROM Category c 
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                WHERE c.Parent=" + parent.ToString() + " AND c.IdUser=" + GlobalVariables.current_user.ToString() + " AND (c.IdDirection=" + GlobalVariables.current_dir + " OR c.`Default`=1) AND c.NotShow=0";


            SqliteCommand comand = new SqliteCommand(query, conn2);

            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                Category buf = new Category();
                buf.Name = r["Name"].ToString();
                buf.CatType = Convert.ToInt32(r["CatType"]);
                buf.Id = Convert.ToInt32(r["Id"]);
                buf.Default = Convert.ToInt32(r["Default"]);
                buf.Description = r["Description"].ToString();
                buf.IdDirection = Convert.ToInt32(r["IdDirection"]);
                buf.IdPurseType = Convert.ToInt32(r["IdPurseType"]);
                buf.NotShow = Convert.ToInt32(r["NotShow"]);
                buf.Parent = Convert.ToInt32(r["Parent"]);

                item.Children.Add(buf);

            }
        }

        #endregion

        #region Список витрат і доходів по рахунку
        public static List<PurseRecord> get_purse_record_list(SqliteConnection conn2, int PurseID)
        {

            conn2.Open();
            string query = @"
                SELECT pr.Id,p.Name as PurseName,c.Name as CategoryName,ct.Name as CattypeName,pr.DateRecord,pr.Summ,pr.Price,p.IdCurrency,pr.Amount,pr.Tag,pr.Description,d.Name as `Group`,ca.Name,'cat_color_'||ct.TypeWrite as rowClass
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "") + @"
                LEFT JOIN Category c ON c.Id=pr.IdCategory
                LEFT JOIN Directions d ON d.Id=p.IdDirection
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                LEFT JOIN Contragent ca ON pr.IdContragent = ca.Id";
            string where = "";
            if (PurseID != -1)
            {
                if (where != "")
                    where += " AND ";
                where += "p.Id=" + PurseID;
            }
            //if (q_post_params != null && q_post_params["cattype"] != null)
            //{
            //    if (where != "")
            //        where += " AND ";
            //    where += "ct.TypeWrite=" + q_post_params["cattype"];
            //}
            if (where != "")
                query += " WHERE " + where;
            //if (q_post_params != null && q_post_params["order"] != null)
            //    query += " ORDER BY " + q_post_params["order"];
            SqliteCommand comand = new SqliteCommand(query, conn2);
            listPurseItem = new List<PurseRecord>();
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                try
                {
                    PurseRecord buf = new PurseRecord();
                    //
                    buf.ShortName = Convert.ToInt32(r["IdCurrency"]);
                    buf.Id = Convert.ToInt32(r["Id"]);
                    buf.Amount = Convert.ToInt32(r["Amount"]);
                    buf.DateRecord = Convert.ToDateTime(r["DateRecord"]);
                    buf.Description = r["Description"].ToString();
                    //buf.IdCategory = Convert.ToInt32(r["IdCategory"]);
                    //buf.IdContragent = Convert.ToInt32(r["IdContragent"]);
                    //buf.IdPurse = Convert.ToInt32(r["IdPurse"]);
                    buf.Category = r["CategoryName"].ToString();
                    buf.Contragent = r["CattypeName"].ToString();
                    buf.Purse = r["PurseName"].ToString();
                    buf.type_write = r["rowClass"].ToString();
                    buf.Price = Convert.ToInt32(r["Price"]);
                    buf.Summ = Convert.ToInt32(r["Summ"]);
                    buf.Tag = r["Tag"].ToString();

                    //
                    listPurseItem.Add(buf);
                }
                catch (Exception ex) { }
            }

            conn2.Close();
            return listPurseItem;
        }
        #endregion

        #region Список видатків і доходів
        public static List<PurseRecord> get_purse_record_list(SqliteConnection conn, int id1 = 0, string typeWrite = "", string transfer = "")
        {
            string query = @"
                SELECT pr.Id,p.Name as PurseName,c.Name as CategoryName,ct.Name as CattypeName,p.IdCurrency,pr.DateRecord,pr.Summ,pr.Price,pr.Amount,pr.Tag,pr.Description,d.Name as `Group`,ca.Name,'cat_color_'||ct.TypeWrite as rowClass
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "") + @"
                LEFT JOIN Category c ON c.Id=pr.IdCategory
                LEFT JOIN Directions d ON d.Id=p.IdDirection
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                LEFT JOIN Contragent ca ON pr.IdContragent = ca.Id WHERE pr.Id>" + id1 + typeWrite + transfer + " ORDER BY pr.Id LIMIT 20;";

            conn.Open();
            SqliteCommand comand = new SqliteCommand(query, conn);
            bufflistcashflow = new List<PurseRecord>();
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                try
                {
                    PurseRecord buf = new PurseRecord();
                    //
                    buf.ShortName = Convert.ToInt32(r["IdCurrency"]);
                    buf.Id = Convert.ToInt32(r["Id"]);
                    buf.Amount = Convert.ToInt32(r["Amount"]);
                    buf.DateRecord = Convert.ToDateTime(r["DateRecord"]);
                    buf.Description = r["Description"].ToString();
                    //buf.IdCategory = Convert.ToInt32(r["IdCategory"]);
                    //buf.IdContragent = Convert.ToInt32(r["IdContragent"]);
                    //buf.IdPurse = Convert.ToInt32(r["IdPurse"]);
                    buf.Category = r["CategoryName"].ToString();
                    buf.Contragent = r["CattypeName"].ToString();
                    buf.Purse = r["PurseName"].ToString();
                    buf.type_write = r["rowClass"].ToString();
                    buf.Price = Convert.ToInt32(r["Price"]);
                    buf.Summ = Convert.ToInt32(r["Summ"]);
                    buf.Tag = r["Tag"].ToString();
                    bufflistcashflow.Add(buf);
                    listcashflow.Add(buf);
                }
                catch (Exception ex) { }
                //

            }

            conn.Close();
            return bufflistcashflow;
        }
        public static PurseRecord get_purse_record_with_current_id(int id)
        {
            SqliteConnection conn = new SqliteConnection(GlobalVariables.dbPath);
            string query = @"
                SELECT pr.Id,p.Name as PurseName,c.Name as CategoryName,ct.Name as CattypeName,p.IdCurrency,pr.DateRecord,pr.Summ,pr.Price,pr.Amount,pr.Tag,pr.Description,d.Name as `Group`,ca.Name,'cat_color_'||ct.TypeWrite as rowClass
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "") + @"
                LEFT JOIN Category c ON c.Id=pr.IdCategory
                LEFT JOIN Directions d ON d.Id=p.IdDirection
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                LEFT JOIN Contragent ca ON pr.IdContragent = ca.Id
                WHERE pr.Id = " + id;

            conn.Open();
            SqliteCommand comand = new SqliteCommand(query, conn);

            PurseRecord buf = new PurseRecord();
            SqliteDataReader r = comand.ExecuteReader();
            while (r.Read())
            {
                try
                {

                    //
                    buf.ShortName = Convert.ToInt32(r["IdCurrency"]);
                    buf.Id = Convert.ToInt32(r["Id"]);
                    buf.Amount = Convert.ToInt32(r["Amount"]);
                    buf.DateRecord = Convert.ToDateTime(r["DateRecord"]);
                    buf.Description = r["Description"].ToString();
                    //buf.IdCategory = Convert.ToInt32(r["IdCategory"]);
                    //buf.IdContragent = Convert.ToInt32(r["IdContragent"]);
                    //buf.IdPurse = Convert.ToInt32(r["IdPurse"]);
                    buf.Category = r["CategoryName"].ToString();
                    buf.Contragent = r["CattypeName"].ToString();
                    buf.Purse = r["PurseName"].ToString();
                    buf.type_write = r["rowClass"].ToString();
                    buf.Price = Convert.ToInt32(r["Price"]);
                    buf.Summ = Convert.ToInt32(r["Summ"]);
                    buf.Tag = r["Tag"].ToString();

                }
                catch (Exception ex) { }
                //

            }

            conn.Close();
            return buf;
        }



        #endregion





    }
}
