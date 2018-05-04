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
using System.Collections.Specialized;
using DayMax.Models;

namespace DayMax
{
    #region BusinessActivity
    [Activity(Label = "BusinessActivity")]
    public class BusinessActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Business);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BusinessMain);


            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;

            foreach (var a in TableContents.Business(this)) {
                FindViewById<LinearLayout>(Resource.Id.businessmainlayout).AddView(a);
            }
            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_Business);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Add_Business))
                    {
                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Add_Business);
                        this.Finish();
                        StartActivity(typeof(AddBusinessActivity));
                    }
                };
                popUpmenu.Show();


            };
            
            #region Μενώ ³ νΰβ³γΰφ³ÿ
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
    #endregion
    #region AddBusinessActivity
    [Activity(Label = "AddBusinessActivity")]
    public class AddBusinessActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddBusinessLayout);


            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;

            FindViewById<Button>(Resource.Id.btnOK).Click+=delegate
            {
                if (FindViewById<EditText>(Resource.Id.tBusinessName).Text != "")
                {
                    try
                    {
                        TableContents.InsertData("Directions", "'Default','Name','IdUser'", "0,'" + FindViewById<EditText>(Resource.Id.tBusinessName).Text + "'," + GlobalVariables.current_user);
                        GlobalVariables.StateActivity = true;
                        this.Finish();

                    }
                    catch (Exception ex) { }
                }
            };
            FindViewById<Button>(Resource.Id.btnCancel).Click += delegate {
                this.Finish();
            };
            FindViewById<View>(Resource.Id.backbutton).Click += delegate {
                this.Finish();
            };
        }
    }
    #endregion
}