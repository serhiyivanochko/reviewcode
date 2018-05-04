using System;
using Android.App;
using Android.Content;

using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mono.Data.Sqlite;
using Android.Database;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using Android.Database.Sqlite;
using System.IO;

using System.Collections.Specialized;
using DayMax.Models;
using FortySevenDeg.SwipeListView;
using Android.Animation;
using Android.Content.Res;

namespace DayMax.Models
{
    public static class TableHeaders
    {
        public static LinearLayout CashFlowHeader(Context context) {
            LinearLayout Mainheader = new LinearLayout(context);
            Mainheader.Orientation = Android.Widget.Orientation.Vertical;
            LinearLayout header = new LinearLayout(context);
            header.Orientation = Android.Widget.Orientation.Horizontal;

            TextView typeCat = new TextView(context);
            typeCat.Text = context.Resources.GetString(Resource.String.Price);
            typeCat.TextSize = 14;
            typeCat.SetWidth(300);
            typeCat.SetTextColor(context.Resources.GetColor(Resource.Color.TableLineColor));
            TextView dateview = new TextView(context);
            dateview.Text = context.Resources.GetString(Resource.String.Date);
            dateview.TextSize = 14;
            dateview.SetTextColor(context.Resources.GetColor(Resource.Color.TableLineColor));
            LinearLayout br = new LinearLayout(context);
            LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(30, LinearLayout.LayoutParams.MatchParent);
            br.LayoutParameters = parms;
            header.AddView(br);
            header.AddView(typeCat);
            header.AddView(dateview);
            LinearLayout earningsHeader = header;
            LinearLayout expencesHeader = header;
            View v = new View(context);
            v.SetBackgroundColor(context.Resources.GetColor(Resource.Color.TableLineColor));
            parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
            v.LayoutParameters = parms;
            Mainheader.AddView(header);
            Mainheader.AddView(v);
            return Mainheader;
        }
        
        
    }
}