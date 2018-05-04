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
    public class PurseFields
    {
        public string DbName { get; set; }
        public string DbType { get; set; }
        public string Title { get; set; }
        public string Values { get; set; }
        public string Value;
        public string HtmlType { get; set; }
        public PurseFields() {
            this.Value = "";
        }
        

        //public string BuildHtml()
        //{
        //    string inp = "";
        //    string value = ((this.Value.Trim() != "") ? "value='" + this.Value + "'" : "");
        //    if (HtmlType == "text")
        //    {
        //        switch (DbType)
        //        {
        //            case "System.DateTime":
        //                inp = "<input type='text' name='" + DbName + "' " + value + " class='datepicker_field'>";
        //                break;
        //            default:
        //                inp = "<input type='text' name='" + DbName + "' "+value+" placeholder='" + ((this.Title.Trim() != "") ? this.Title : "Ввести") + "'>";
        //                break;
        //        }
        //    }
        //    else if (HtmlType == "hidden")
        //        inp = "<input type='hidden' name='" + DbName + "' " + value + ">";
        //    else if (HtmlType == "select")
        //    {
        //        var values = this.getValues();
        //        inp = "<select name='" + this.DbName + "'>";
        //        foreach (var val in values)
        //        {
        //            inp += "<option value='" + val["Key"] + "'>" + val["Value"] + "</option>";
        //        }
        //        inp += "</select>";
        //    }
        //    else if (HtmlType == "radio")
        //    {
        //        var values = this.getValues();
        //        inp += "<div class='radio_group'>";
        //        foreach (var val in values)
        //        {
        //            inp += "<label><input type='radio' name='" + this.DbName + "' value='" + val["Key"] + "'>" + val["Value"] + "</label>";
        //        }
        //        inp += "</div>";
        //    }

        //    return inp;
        //}

        public List<Dictionary<string, string>> getValues()
        {
            List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
            if (this.Values.Trim() != "")
            {
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //values = js.Deserialize<List<Dictionary<string, string>>>(this.Values);
            }
            return values;
        }
    }
}
