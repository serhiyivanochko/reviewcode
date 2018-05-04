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

namespace DayMax
{
    #region InvestmentActivity
    [Activity(Label = "InvestmentActivity")]
    public class InvestmentActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Investment);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.InvestmentMain);
            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;
            foreach (var a in TableContents.Investment(this))
            {
                FindViewById<LinearLayout>(Resource.Id.investmentmainlayout).AddView(a);
            }
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
    #endregion

    #region InvestmentItemActivity
    [Activity(Label = "InvestmentItemActivity")]
    public class InvestmentItemActivity : Activity, ViewTreeObserver.IOnScrollChangedListener
    {
        public void OnScrollChanged()
        {
            List<PurseRecord> tmp = new List<PurseRecord>();
            LinearLayout mainList = FindViewById<LinearLayout>(Resource.Id.linearLayoutCash);
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.mainscroll);


            double scrollingSpace =
            sc.GetChildAt(0).Height - sc.Height;

            if (scrollingSpace <= sc.ScrollY) // Touched bottom
            {
                try
                {
                    PurseRecord.bufflistcashflow.Clear();
                    PurseRecord lastID = new PurseRecord();
                    lastID.Id = Convert.ToInt32(mainList.GetChildAt(mainList.ChildCount - 1).Tag);
                    PurseRecord.bufflistcashflow.Add(lastID);
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND pr.IdPurse=" + GlobalVariables.CurrentViewAboutPurse);
                }
                catch (Exception ex) { }
                foreach (var a in tmp)
                {
                    mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Investment", 3));
                }
            }
            else
            {
                //Do the load more like things
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = GlobalVariables.PreviousTitleActivity;
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.InvastmentItemLayout);

            Dictionary<int, Dictionary<string, string>> purse1 = Purse.purseList("", "", "p.Id = " + GlobalVariables.CurrentViewAboutPurse);
            Dictionary<string, string> buf = purse1[GlobalVariables.CurrentViewAboutPurse];

            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_InvestItem);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    GlobalVariables.PreviousTitleActivity = FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text;
                    GlobalVariables.CurrentTitleName = arg1.Item.TitleFormatted.ToString();
                    GlobalVariables.CurrentPurseToTransfer = Convert.ToInt32(buf["Id"]);
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.GiveEarning)) { StartActivityForResult(typeof(InsertCashFlowActivity), 0); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.GiveExpense)) { StartActivityForResult(typeof(InsertCashFlowActivity), 0); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.EditBalance)) { StartActivityForResult(typeof(InsertCashFlowActivity), 0); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Transfer)) { GlobalVariables.CurrentInsertTransfer = 2; GlobalVariables.CurrentPurseToTransfer = GlobalVariables.CurrentViewAboutPurse; StartActivityForResult(typeof(InsertLoansActivity), 0); }
                    //if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Insert_Expense)) { GlobalVariables.CurrentTabTypeWrite = 0; StartActivityForResult(typeof(InsertCashFlowActivity), 0); }
                    //if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Insert_Transfer)) { GlobalVariables.CurrentInsertTransfer = 0; GlobalVariables.CurrentTabTypeWrite = 2; StartActivity(typeof(InsertLoansActivity)); }


                };
                popUpmenu.Show();


                };


            #region Content
                
            Purse purse = new Purse();
            purse.DateStart = Convert.ToDateTime(buf["DateStart"]);
            purse.Description = buf["Description"].ToString();
            purse.Id = Convert.ToInt32(buf["Id"]);
            purse.Name = buf["Name"];
            purse.Status = Convert.ToInt32(buf["Status"]);

            Investment Item = PurseRecord.investmentsP(new SqliteConnection(GlobalVariables.dbPath), purse, this);
            //Display display = WindowManager.DefaultDisplay;
            //int width_screen = display.Width-100;

            var Time = FindViewById<TextView>(Resource.Id.tTime);
            var Start = FindViewById<TextView>(Resource.Id.tStart);
            var Balance = FindViewById<TextView>(Resource.Id.sBalance);
            var Vnes = FindViewById<TextView>(Resource.Id.sVnes);
            var SummSpys = FindViewById<TextView>(Resource.Id.sSpus);
            var Earning = FindViewById<TextView>(Resource.Id.sEarning);
            var Expence = FindViewById<TextView>(Resource.Id.sExpense);
            var Prybutok = FindViewById<TextView>(Resource.Id.sPrybutok);
            
            Time.Text = Item.totalDays.ToString() + " " + Resources.GetString(Resource.String.Days);
            Start.Text = Item.purse.DateStart.ToString("dd-MM-yyy");
            Balance.Text = Item.balance.ToString() + " "+ GlobalVariables.CurrentShortName.ToUpper();
            Vnes.Text = Item.finn.ToString() + " " + GlobalVariables.CurrentShortName.ToUpper();
            SummSpys.Text = Item.sum_spys.ToString() + " " + GlobalVariables.CurrentShortName.ToUpper();
            Earning.Text = Item.doh.ToString() + " " + GlobalVariables.CurrentShortName.ToUpper();
            Expence.Text = Item.vyv_kost.ToString() + " " + GlobalVariables.CurrentShortName.ToUpper();
            Prybutok.Text = Item.prybutok.ToString() + " " + GlobalVariables.CurrentShortName.ToUpper();

            #region Таблиця Місяців
            LinearLayout lt = new LinearLayout(this);
            lt.SetHorizontalGravity(GravityFlags.CenterHorizontal);

            TableLayout table = new TableLayout(this);
            TableRow tr = new TableRow(this);


            TextView tw = new TextView(this);
            tw.Text = "";
            tr.AddView(tw);
            tw = new TextView(this);
            tw.Text = " | ";
            tr.AddView(tw);
            foreach (var a in Item.di) {
                tw = new TextView(this);
                tw.Text = a.Value;
                tr.AddView(tw);
                tw = new TextView(this);
                tw.Text = " | ";
                tr.AddView(tw);
            }
            TableRow trVnes = new TableRow(this);
            TableRow trtab3 = new TableRow(this);
            TableRow trEarning = new TableRow(this);
            TableRow trPryb = new TableRow(this);
            TableRow trVuv = new TableRow(this);

            tw = new TextView(this);
            tw.Text = Resources.GetString(Resource.String.Vnes);
            trVnes.AddView(tw);

            tw = new TextView(this);
            tw.Text = "|";
            trVnes.AddView(tw);

            tw = new TextView(this);
            tw.Text = Resources.GetString(Resource.String.tab3);
            trtab3.AddView(tw);

            tw = new TextView(this);
            tw.Text = "|";
            trtab3.AddView(tw);

            tw = new TextView(this);
            tw.Text = Resources.GetString(Resource.String.Earning);
            trEarning.AddView(tw);

            tw = new TextView(this);
            tw.Text = "|";
            trEarning.AddView(tw);

            tw = new TextView(this);
            tw.Text = Resources.GetString(Resource.String.Pryb);
            trPryb.AddView(tw);

            tw = new TextView(this);
            tw.Text = "|";
            trPryb.AddView(tw);

            tw = new TextView(this);
            tw.Text = Resources.GetString(Resource.String.Vuv);
            trVuv.AddView(tw);

            tw = new TextView(this);
            tw.Text = "|";
            trVuv.AddView(tw);

            foreach (var a in Item.di)
            {
                tw = new TextView(this);
                tw.Text = "("+Item.monthArr[0][a.Key-1].ToString()+")";
                trVnes.AddView(tw);

                tw = new TextView(this);
                tw.Text = "|";
                trVnes.AddView(tw);

                tw = new TextView(this);
                tw.Text = Item.monthArr[1][a.Key - 1].ToString();
                trtab3.AddView(tw);

                tw = new TextView(this);
                tw.Text = "|";
                trtab3.AddView(tw);

                tw = new TextView(this);
                tw.Text = Item.monthArr[2][a.Key - 1].ToString();
                trEarning.AddView(tw);

                tw = new TextView(this);
                tw.Text = "|";
                trEarning.AddView(tw);

                tw = new TextView(this);
                tw.Text = Item.monthArr[6][a.Key - 1].ToString();
                trPryb.AddView(tw);

                tw = new TextView(this);
                tw.Text = "|";
                trPryb.AddView(tw);

                tw = new TextView(this);
                tw.Text = "";
                trVuv.AddView(tw);

                tw = new TextView(this);
                tw.Text = "|";
                trVuv.AddView(tw);
            }

            table.AddView(tr);
            table.AddView(trVnes);
            table.AddView(trtab3);
            table.AddView(trEarning);
            table.AddView(trPryb);
            table.AddView(trVuv);
            
            table.SetGravity(GravityFlags.CenterHorizontal);
            lt.AddView(table);
            
            FindViewById<LinearLayout>(Resource.Id.LinearTable).AddView(lt);
            #endregion


            FindViewById<TextView>(Resource.Id.tHourEar).Text = Convert.ToInt32(Item.prybutok / Item.totalHours).ToString();
            FindViewById<TextView>(Resource.Id.tDayEar).Text = Convert.ToInt32(Item.prybutok / Item.totalDays).ToString();
            FindViewById<TextView>(Resource.Id.tMonthEar).Text = Convert.ToInt32(Item.prybutok / Item.totalMonth).ToString();
            FindViewById<TextView>(Resource.Id.tDateOkup).Text = Item.purse.DateStart.ToString("dd-MM-yyy");
            FindViewById<TextView>(Resource.Id.tTimeOkup).Text = Item.doh + " " + Resources.GetString(Resource.String.Days);
            #endregion

            #region Рух коштів
            #region Контент таблиці


            #endregion
            #endregion



            #region CashFlow
            var MainList = FindViewById<LinearLayout>(Resource.Id.linearLayoutCash);

            FindViewById<ScrollView>(Resource.Id.mainscroll).ViewTreeObserver.AddOnScrollChangedListener(this);
            List<PurseRecord> list = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, " AND pr.IdPurse=" + GlobalVariables.CurrentViewAboutPurse);

            if (MainList.ChildCount == 0)
            {

                MainList.AddView(TableHeaders.CashFlowHeader(this));
                foreach (var a in list)
                {
                    MainList.AddView(TableContents.CashFlowContent(this, a, 0, "Investment", 3));
                }
            }
            
            

            #endregion
            FindViewById<View>(Resource.Id.backbutton).Click += delegate {
                Finish();
                //StartActivity(typeof(AssetsActivity));
            };
        }

    }
    #endregion
}
