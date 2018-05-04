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
using Android.Content.Res;
using DayMax.Models;
using System.Threading;

namespace DayMax
{

    [Activity(Label = "DayMax")]
    public class MainActivity : Activity
    {

       

        
        ProgressDialog progress;
        protected override void OnCreate(Bundle bundle)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            base.OnCreate(bundle);

            
            SetContentView(Resource.Layout.Main);

            
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {


                    Dictionary<string, double> Converted = CurrencyRate.Converter();
                    foreach (var a in Converted){
                        LinearLayout buf = new LinearLayout(this);
                        buf.Orientation = Android.Widget.Orientation.Horizontal;
                        TextView t = new TextView(this);
                        t.Text = a.Key.ToUpper();
                        t.SetTextColor(Resources.GetColor(Resource.Color.ConvertColor));
                        t.TextSize = 18;
                        t.SetWidth(120);

                        TextView t1 = new TextView(this);
                        t1.Text = a.Value.ToString();
                        t1.SetTextColor(Resources.GetColor(Resource.Color.GreenColor));
                        t1.TextSize = 20;

                        buf.AddView(t);
                        buf.AddView(t1);

                        FindViewById<LinearLayout>(Resource.Id.linearLayout4).AddView(buf);
                    }

                    FindViewById<ImageView>(Resource.Id.iGraph1).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph1));
                    FindViewById<ImageView>(Resource.Id.iGraph2).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph2));
                    FindViewById<ImageView>(Resource.Id.iGraph3).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph2));
                    FindViewById<ImageView>(Resource.Id.iGraph4).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph4));
                    FindViewById<ImageView>(Resource.Id.iGraph5).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph3));
                    FindViewById<ImageView>(Resource.Id.iGraph6).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph3));
                    FindViewById<ImageView>(Resource.Id.iGraph7).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph5));
                    FindViewById<ImageView>(Resource.Id.iGraph8).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph2));
                    FindViewById<ImageView>(Resource.Id.iGraph9).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph2));
                    FindViewById<ImageView>(Resource.Id.iGraph10).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.tmp_graph4));

                    progress.Dismiss();
                });
            })).Start();

            #region Меню і навігація
            var menu = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);
            var menuButton = FindViewById(Resource.Id.MenuButton);
            menuButton.Click += (sender, e) =>
            {
                menu.AnimatedOpened = !menu.AnimatedOpened;

                var DayMax = FindViewById<LinearLayout>(Resource.Id.LinearDayMax);
                DayMax.Click += delegate { GlobalVariables.CurrentTitleName = DayMax.Tag.ToString(); StartActivity(typeof(DayMaxActivity)); };

                var Home = FindViewById<LinearLayout>(Resource.Id.LinearHome);
                Home.Click += delegate { StartActivity(typeof(MainActivity)); };
                var Assets = FindViewById<LinearLayout>(Resource.Id.LinearAssets);
                Assets.Click += delegate { GlobalVariables.CurrentTitleName = Assets.Tag.ToString(); StartActivity(typeof(AssetsActivity)); };
                var Bills = FindViewById<LinearLayout>(Resource.Id.LinearBills);
                Bills.Click += delegate { GlobalVariables.CurrentTitleName = Bills.Tag.ToString(); StartActivity(typeof(BillsActivity)); };
                var Business = FindViewById<LinearLayout>(Resource.Id.LinearBusiness);
                Business.Click += delegate { GlobalVariables.CurrentTitleName = Business.Tag.ToString(); StartActivity(typeof(BusinessActivity)); };
                var CashFlow = FindViewById<LinearLayout>(Resource.Id.LinearCashFlow);
                CashFlow.Click += delegate { GlobalVariables.CurrentTitleName = CashFlow.Tag.ToString(); StartActivity(typeof(CashFlowActivity)); };

                var Investment = FindViewById<LinearLayout>(Resource.Id.LinearInvestment);
                Investment.Click += delegate { GlobalVariables.CurrentTitleName = Investment.Tag.ToString(); StartActivity(typeof(InvestmentActivity)); };
                var Loans = FindViewById<LinearLayout>(Resource.Id.LinearLoans);
                Loans.Click += delegate { GlobalVariables.CurrentTitleName = Loans.Tag.ToString(); StartActivity(typeof(LoansActivity)); };

            };


            
            #endregion
        }
    }
}

