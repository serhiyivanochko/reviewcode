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

namespace DayMax.Models
{
    public static class InsertStrings
    {
        public static string InsertCashFlow = @"
                INSERT p.Name as PurseName,c.Name as CategoryName,ct.Name as CattypeName,pr.DateRecord,pr.Summ,pr.Price,pr.Amount,pr.Tag,pr.Description,d.Name as `Group`,ca.Name,'cat_color_'||ct.TypeWrite as rowClass
                INTO PurseRecord pr
                INNER JOIN Purse p ON pr.IdPurse=p.Id AND p.IdUser=" + GlobalVariables.current_user + ((GlobalVariables.current_dir != 1) ? " AND p.IdDirection=" + GlobalVariables.current_dir : "") + @"
                LEFT JOIN Category c ON c.Id=pr.IdCategory
                LEFT JOIN Directions d ON d.Id=p.IdDirection
                INNER JOIN Cattype ct ON ct.Id=c.CatType
                LEFT JOIN Contragent ca ON pr.IdContragent = ca.Id VALUES(";
    }
}