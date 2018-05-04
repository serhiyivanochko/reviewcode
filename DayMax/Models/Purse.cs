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

namespace DayMax.Models
{
    public class Purse
    {
        public const int StatusOFF = 2;
        public string Name { get; set; }
        public string getCutName(int length = 23)
        {
            if (this.Name.Length > length)
                return this.Name.Substring(0, length) + "...";
            return this.Name;
        }
        public int Id { get; set; }
        public int IdPurseCat { get; set; }
        public int IdPurseType = -1;
        public int IdCurrency { get; set; }
        public DateTime DateStart { get; set; }
        public int IdDirection { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string ImgFull { get; set; }
        public string ImgCrop { get; set; }
        public string CropCoords { get; set; }
        public static Dictionary<int, Dictionary<string, string>> allPurse;
        public static Dictionary<int, string> listType;
        public static Dictionary<int, string> listDir;

        public Purse()
        {
            this.IdPurseCat = 0;
        }




        #region ValuesAcive
        //a["Key"]
        //a["Name"]
        //a["Type"]
        //a["DateStart"]
        //a["Summ"]
        //a["Description"]
        //a["ShortName"]
        //a["Status"]
        //a["status"]
        //a["Summ1"]
        //a["Summ2"]
        //a["SummCurrency"]
        //a["idPurse"]
        //a["date_return"]
        //a["summ_return"]
        //a["percent_return"]
        //a["creditor"]
        //a["purse_credit_type"]
        //a["summ_return_left"]
        #endregion
        #region ValuesPurseList
        /*  value[0].key = "Id";
            value[1].key = "Name";
            value[2].key = "Type";
            value[3].key = "DateStart";
            value[4].key = "Summ";
            value[5].key = "Description";
            value[6].key = "ShortName";
            value[7].key = "Status";
            value[8].key = "Summ1";
            value[9].key = "Summ2";
            value[10].key = "SummCurrency";*/
        #endregion
        #region purseMaxValueId
        public static int purseMaxValueId()
        {
            var conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = @"SELECT Id FROM Purse";
            SqliteCommand comand = new SqliteCommand(query, conn2);
            SqliteDataReader r = comand.ExecuteReader();
            int rez = 0;
            while (r.Read())
            {
                if (Convert.ToInt32(r["Id"]) > rez) rez = Convert.ToInt32(r["Id"]);
            }
            conn2.Close();
            return rez;
        }
        #endregion
        #region purseList
        public static Dictionary<int, Dictionary<string, string>> purseList(string select = "", string query_join = "", string condition = "")
        {



            var conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            //string query1 = @"SELECT * FROM Purse";
            //SqliteCommand comand1 = new SqliteCommand(query1, conn2);
            //SqliteDataReader rd = comand1.ExecuteReader();
            //while (rd.Read())
            //{
            //    string Name = rd["Name"].ToString();
            //    int id = Convert.ToInt32(rd["Id"]);
            //}

            string query = @"
                SELECT p.Id,p.Name,pt.Name as Type,p.DateStart,p.Id as Summ,p.Description,c.ShortName,p.Status ";
            if (select != "")
                query += "," + select;
            query += @" FROM Purse p
                INNER JOIN Currency c ON c.Id = p.IdCurrency
                LEFT JOIN PurseCat pt ON pt.Id=p.IdPurseCat";
            if (query_join != "")
                query += " " + query_join;
            if (condition != "")
                query += " WHERE " + condition;
            SqliteCommand comand = new SqliteCommand(query, conn2);

            Dictionary<int, double[]> summArr = Purse.summArr2(conn2, condition);
            Dictionary<int, Dictionary<string, string>> list = new Dictionary<int, Dictionary<string, string>>();
            //Dictionary<int,<Dictionary<string, string>> list = new 

            string tmpQuery = comand.CommandText;
            SqliteDataReader r = comand.ExecuteReader();
            int idPurse = 0;
            double[] tmp;
            bool isset = false;
            while (r.Read())
            {

                Dictionary<string, string> itemParams = new Dictionary<string, string>();
                itemParams["Id"] = r["Id"].ToString();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    itemParams[r.GetName(i)] = r.GetValue(i).ToString();
                }
                idPurse = Convert.ToInt32(itemParams["Id"]);
                isset = summArr.TryGetValue(idPurse, out tmp);
                if (isset)
                {
                    itemParams["Summ"] = (tmp[1] - tmp[0]).ToString();
                    itemParams["Summ1"] = tmp[1].ToString();
                    itemParams["Summ2"] = tmp[0].ToString();
                }
                else
                {
                    itemParams["Summ"] = "0";
                    itemParams["Summ1"] = "0";
                    itemParams["Summ2"] = "0";
                }
                itemParams["SummCurrency"] = itemParams["Summ"] + " " + itemParams["ShortName"];
                list.Add(idPurse, itemParams);
            }
            conn2.Close();
            return list;
        }
        #endregion
        #region propList
        public static Dictionary<int, Dictionary<string, string>> propList(string select = "", string query_join = "", string condition = "")
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = @"SELECT pp.MetaKey,pp.MetaValue,pp.IdPurse ";
            if (select != "")
                query += select;
            query += @" FROM PurseProp pp
                  INNER JOIN Purse p ON p.Id=pp.IdPurse ";
            if (query_join != "")
                query += query_join;
            query += @" WHERE p.IdUser=" + GlobalVariables.current_user.ToString();
            if (condition != "")
                query += " AND " + condition;
            SqliteCommand comand3 = new SqliteCommand(query, conn2);
            Dictionary<int, Dictionary<string, string>> p_fields = new Dictionary<int, Dictionary<string, string>>();
            SqliteDataReader r = comand3.ExecuteReader();
            int idPurse = 0;
            Dictionary<string, string> tmp1;
            while (r.Read())
            {
                idPurse = Convert.ToInt32(r["IdPurse"]);
                if (!p_fields.ContainsKey(idPurse))
                    p_fields[idPurse] = new Dictionary<string, string>();
                string fieldName = "";
                for (int i = 0; i < r.FieldCount; i++)
                {
                    fieldName = r.GetName(i);
                    if (fieldName != "MetaKey" && fieldName != "MetaValue")
                        p_fields[idPurse][r.GetName(i)] = r.GetValue(i).ToString();
                }
                p_fields[idPurse][r["MetaKey"].ToString()] = r["MetaValue"].ToString();
            }
            conn2.Close();
            return p_fields;
        }

        #endregion
        #region summArr
        public static Dictionary<int, double> summArr(string condition = "")
        {
            var conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = @"SELECT pr.IdPurse,SUM(pr.Summ) as Summ,ct.TypeWrite
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id
                INNER JOIN Category c ON c.Id=pr.IdCategory AND c.Id!=9
                INNER JOIN Cattype ct ON ct.Id=c.CatType";
            if (condition != "")
                query += " WHERE " + condition;
            query += " GROUP BY pr.IdPurse,ct.TypeWrite";
            SqliteCommand comand2 = new SqliteCommand(query, conn2);
            Dictionary<int, double> summArr = new Dictionary<int, double>();
            SqliteDataReader r = comand2.ExecuteReader();
            int idPurse = 0;
            double tmp;
            while (r.Read())
            {
                idPurse = Convert.ToInt32(r["IdPurse"]);
                if (!summArr.TryGetValue(idPurse, out tmp))
                    summArr[idPurse] = 0;
                if (Convert.ToInt32(r["TypeWrite"]) == 0)
                    summArr[idPurse] -= Convert.ToDouble(r["Summ"]);
                else
                    summArr[idPurse] += Convert.ToDouble(r["Summ"]);
            }
            conn2.Close();
            return summArr;
        }
        #endregion
        #region summArr2
        public static Dictionary<int, double[]> summArr2(SqliteConnection conn2, string condition = "")
        {

            string query = @"SELECT pr.IdPurse,SUM(pr.Summ) as Summ,ct.TypeWrite
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id
                INNER JOIN Category c ON c.Id=pr.IdCategory AND c.Id!=9
                INNER JOIN PurseCat pt ON p.IdPurseCat=pt.Id
                INNER JOIN Cattype ct ON ct.Id=c.CatType";
            if (condition != "")
                query += " WHERE " + condition;
            query += " GROUP BY pr.IdPurse,ct.TypeWrite";
            SqliteCommand comand2 = new SqliteCommand(query, conn2);
            Dictionary<int, double[]> summArr = new Dictionary<int, double[]>();
            SqliteDataReader r = comand2.ExecuteReader();
            int idPurse = 0;
            double[] tmp;
            while (r.Read())
            {
                idPurse = Convert.ToInt32(r["IdPurse"]);
                if (!summArr.TryGetValue(idPurse, out tmp))
                {
                    summArr[idPurse] = new double[2] { 0, 0 };

                }
                if (Convert.ToInt32(r["TypeWrite"]) == 0)
                    summArr[idPurse][0] += Convert.ToDouble(r["Summ"]);
                else
                    summArr[idPurse][1] += Convert.ToDouble(r["Summ"]);
            }
            return summArr;
        }
        #endregion
        #region delete
        /* public static bool Delete(SQLiteConnection conn,int Id)
         {
             conn.Execute("DELETE FROM PurseRecord WHERE IdPurse = ?", new object[1] { Id });
             conn.Execute("DELETE FROM PurseProp WHERE IdPurse = ?", new object[1] { Id });
             conn.Delete<Purse>(Id);
             return true;
         }*/
        #endregion
        #region Converter
        public static Dictionary<int, double> Converter()
        {
            Dictionary<int, string> listCurr = new Dictionary<int, string>();
            Dictionary<int, double> listVal = new Dictionary<int, double>();
            listCurr[1] = "EUR";
            listCurr[2] = "USD";
            listCurr[3] = "UAH";
            listVal[1] = 0;
            listVal[2] = 0;
            listVal[3] = 0;

            String query = @"SELECT cr.IdCurrencyFrom,cr.IdCurrencyTo,cr.Nominal, cr.Rate
                FROM CurrencyRate cr
                INNER JOIN Purse p ON cr.IdCurrencyFrom = p.IdCurrency
                INNER JOIN PurseRecord pr ON p.Id=pr.IdPurse
                WHERE cr.IdCurrencyTo IN (" + string.Format("{0}", string.Join(",", listCurr.Keys)) + @")
                AND cr.IdUser=p.IdUser
                AND p.IdUser=" + GlobalVariables.user.ToString();


            using (var conn = new SqliteConnection(GlobalVariables.dbPath))
            {
                conn.Open();
                SqliteCommand comand = new SqliteCommand(query, conn);
                Dictionary<int, Dictionary<int, CurrencyRate>> rates = new Dictionary<int, Dictionary<int, CurrencyRate>>();
                SqliteDataReader r = comand.ExecuteReader();
                while (r.Read())
                {
                    int from = Convert.ToInt32(r["IdCurrencyFrom"]);
                    int to = Convert.ToInt32(r["IdCurrencyTo"]);
                    int nominal = Convert.ToInt32(r["Nominal"]);
                    double rate = Convert.ToDouble(r["rate"]);
                    rates[from] = new Dictionary<int, CurrencyRate>();
                    rates[from][to] = new CurrencyRate() { IdCurrencyFrom = from, IdCurrencyTo = to, Rate = rate, Nominal = nominal };
                }

                query = @"SELECT SUM(pr.Summ) as Summ,ct.TypeWrite,p.IdCurrency
                FROM PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id
                INNER JOIN Category c ON c.Id=pr.IdCategory
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                WHERE p.IdUser=" + GlobalVariables.user.ToString() + " GROUP BY p.IdCurrency, ct.TypeWrite";
                SqliteCommand comand2 = new SqliteCommand(query, conn);
                r = comand2.ExecuteReader();
                double tmp = 0;
                double summ = 0;
                int idCurr = 0;
                CurrencyRate tmpCur = new CurrencyRate();
                Dictionary<int, CurrencyRate> tmpDict;
                while (r.Read())
                {
                    foreach (var cur in listCurr)
                    {
                        tmp = listVal[cur.Key];
                        summ = Convert.ToDouble(r["Summ"]);
                        if (Convert.ToInt32(r["IdCurrency"]) != cur.Key)
                        {
                            idCurr = Convert.ToInt32(r["IdCurrency"]);
                            if (rates.TryGetValue(idCurr, out tmpDict) && tmpDict.TryGetValue(cur.Key, out tmpCur) != false)
                                summ = summ * (rates[Convert.ToInt32(r["IdCurrency"])][cur.Key].Rate * rates[Convert.ToInt32(r["IdCurrency"])][cur.Key].Nominal);
                            else
                                summ = 0;
                        }
                        if (r["TypeWrite"] == "0")
                            tmp -= summ;
                        else
                            tmp += summ;
                        listVal[cur.Key] = tmp;
                    }
                    //idPurse = Convert.ToInt32(r["IdPurse"]);
                }
                conn.Close();
            }

            return listVal;
        }
        #endregion
        #region getPursesTreeSTR
        public static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, string>>>> getPursesTreeSTR(SqliteConnection conn2, int current_user, int current_dir)
        {
            allPurse = new Dictionary<int, Dictionary<string, string>>();
            allPurse = Purse.purseList("p.IdCurrency,p.IdPurseCat, c.ShortName as currName, d.Name as DirName,d.Id as DirId", " INNER JOIN Directions d ON p.IdDirection=d.Id ", "p.IdUser=" + current_user.ToString());
            Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, string>>>> pList = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, string>>>>();
            string str = "";
            listType = new Dictionary<int, string>();
            listDir = new Dictionary<int, string>();
            string tmpstr = "";
            int IdPurseCat = 0, idDir = 0;
            Dictionary<int, Dictionary<int, Dictionary<string, string>>> tmp = new Dictionary<int, Dictionary<int, Dictionary<string, string>>>();
            Dictionary<int, Dictionary<string, string>> tmp2 = new Dictionary<int, Dictionary<string, string>>();
            foreach (int idPurse in allPurse.Keys)
            {
                IdPurseCat = Convert.ToInt32(allPurse[idPurse]["IdPurseCat"]);
                idDir = Convert.ToInt32(allPurse[idPurse]["DirId"]);
                if (!pList.TryGetValue(idDir, out tmp))
                {
                    pList[idDir] = new Dictionary<int, Dictionary<int, Dictionary<string, string>>>();
                    listDir.Add(idDir, allPurse[idPurse]["DirName"]);
                }
                if (!pList[idDir].TryGetValue(IdPurseCat, out tmp2))
                {
                    pList[idDir][IdPurseCat] = new Dictionary<int, Dictionary<string, string>>();
                    if (!listType.TryGetValue(IdPurseCat, out tmpstr))
                        listType.Add(IdPurseCat, allPurse[idPurse]["Type"]);
                }
                pList[idDir][IdPurseCat].Add(idPurse, allPurse[idPurse]);
                //allPurseSTR += "<option value='" + purse["Id"] + "' curr='" + purse["IdCurrency"].ToString() + "'>" + purse["Name"] + "(" + purse["Summ"] + " " + purse["ShortName"] + ")" + "</option>";
            }
            //str = "<ul>";
            //foreach(int keyDir in pList.Keys)
            //{
            //    if (keyDir != 1)
            //        str += "<li><span><b>"+listDir[keyDir]+"</b></span><ul>";
            //    foreach (int keyType in pList[keyDir].Keys)
            //    {
            //        str += "<li type='" + keyType + "'><span>" + listType[keyType] + "</span><ul>";
            //        foreach (int idPurse in pList[keyDir][keyType].Keys)
            //        {
            //            str += "<li><input type='checkbox' value='" + idPurse + "' curr_t='" + allPurse[idPurse]["currName"] + "' curr='" + allPurse[idPurse]["IdCurrency"].ToString() + "' title='" + allPurse[idPurse]["Name"] + "'><span>" + allPurse[idPurse]["Name"] + "(" + allPurse[idPurse]["Summ"] + " " + allPurse[idPurse]["ShortName"] + ")</span></li>";
            //        }
            //    str += "</ul></li>";
            //    }
            //    if (keyDir != 1)
            //        str += "</ul></li>";
            //}
            //str += "</ul>";
            return pList;
        }
        #endregion
        #region getFieldTitle
        public string getFieldTitle(string field)
        {

            Dictionary<string, string> titles = new Dictionary<string, string>();
            titles["Name"] = "#*Name*#";
            titles["IdPurseCat"] = "#*Type bill*#";
            titles["DateStart"] = "#*Start date*#";
            titles["Summ"] = "#*Total*#";
            titles["IdCurrency"] = "#*Main currency*#";
            titles["Description"] = "#*Comment*#";

            string tmp = "";
            if (titles.TryGetValue(field, out tmp))
                return tmp;

            return "";
        }
        #endregion
        #region trush
        public static string getUploadFileForm()
        {
            return @"<form method='post' enctype='multipart/form-data'><input type='file' name='purse_photo'/><input type='submit' name='upload_photo'></form>";
        }
        public static string getCropFileForm()
        {
            return @"<form method='post' enctype='multipart/form-data'><input type='file' name='purse_photo'/><input type='submit' name='upload_photo'></form>";
        }
        #endregion

        internal PurseCat PurseCat
        {
            get => default(PurseCat);
            set
            {
            }
        }

        internal PurseType PurseType
        {
            get => default(PurseType);
            set
            {
            }
        }

        internal Currency Currency
        {
            get => default(Currency);
            set
            {
            }
        }
    }
}
