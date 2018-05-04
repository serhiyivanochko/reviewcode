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
using Mono.Data.Sqlite;

namespace DayMax.Models
{
    class CurrencyRate
    {
        public int IdCurrencyFrom { get; set; }        
        public int IdCurrencyTo { get; set; }
        public int Nominal { get; set; }
        public double Rate { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public int Id { get; set; }
        public int IdUser { get; set; }
        public CurrencyRate() { }
        
        #region Converter
        public static Dictionary<string, double> Converter()
        {
            Dictionary<int, string> listCurr = AppCurrency.getAppCurr(new SqliteConnection(GlobalVariables.dbPath));
            Dictionary<string, double> listVal = new Dictionary<string, double>();
            
            foreach (var a in listCurr) {
                listVal.Add(a.Value,0);
            }
            
            
            String query = @"SELECT cr.IdCurrencyFrom,cr.IdCurrencyTo,cr.Nominal, cr.Rate
                FROM CurrencyRate cr
                INNER JOIN Purse p ON cr.IdCurrencyFrom = p.IdCurrency
                INNER JOIN PurseRecord pr ON p.Id=pr.IdPurse
                WHERE cr.IdCurrencyTo IN (" + string.Format("{0}", string.Join(",", listCurr.Keys)) + @")
                AND cr.IdUser=p.IdUser
                AND p.IdUser=" + GlobalVariables.user.ToString() + " AND p.IdDirection = " + GlobalVariables.current_dir;


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
                WHERE p.IdUser=" + GlobalVariables.user.ToString() + " AND p.IdDirection = " + GlobalVariables.current_dir+ " GROUP BY p.IdCurrency, ct.TypeWrite";
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
                        tmp = listVal[cur.Value];
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
                        listVal[cur.Value] = tmp;
                    }
                    //idPurse = Convert.ToInt32(r["IdPurse"]);
                }
                conn.Close();
            }


         


            return listVal;
        }


        #endregion
    }
}