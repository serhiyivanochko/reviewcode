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
using System.Globalization;
using System.Threading;
using static Android.Widget.AdapterView;
using Android.Views.InputMethods;
using Android.Graphics;

namespace DayMax
{

    #region LoansActivity
    [Activity(Label = "LoansActivity")]
    public class LoansActivity : Activity
    {
        ProgressDialog progress;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {

            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);


            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoansMain);

            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;

            #region Вкладки
            TabHost tabHost = FindViewById<TabHost>(Resource.Id.tabHost1Loans);
            tabHost.Setup();

            TabHost.TabSpec tabSpec;

            tabSpec = tabHost.NewTabSpec("tag1");
            tabSpec.SetIndicator(Resources.GetString(Resource.String.All));
            tabSpec.SetContent(Resource.Id.LoansAll);
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("tag2");
            tabSpec.SetIndicator(Resources.GetString(Resource.String.Debitors));
            tabSpec.SetContent(Resource.Id.LoansDebitors);
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("tag3");
            tabSpec.SetIndicator(Resources.GetString(Resource.String.Creditors));
            tabSpec.SetContent(Resource.Id.LoansCreditors);
            tabHost.AddTab(tabSpec);

            tabHost.CurrentTab = GlobalVariables.CurrentViewTabLoan;
            
            tabHost.TabChanged += delegate
            {
                switch (tabHost.CurrentTab)
                {
                    case 0: GlobalVariables.CurrentViewTabLoan = 0;  break;
                    case 1: GlobalVariables.CurrentViewTabLoan = 1;  break;
                    case 2: GlobalVariables.CurrentViewTabLoan = 2;  break;

                }
            };
            #endregion

            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));

                    popUpmenu.Inflate(Resource.Menu.PopUp_Creditors);


                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    GlobalVariables.CurrentTitleName = arg1.Item.TitleFormatted.ToString();

                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Give_Loan)) { GlobalVariables.GiveTakeLoanIndex = 0; StartActivity(typeof(GiveTakeLoanActivity)); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Take_Loan)) { GlobalVariables.GiveTakeLoanIndex = 1; StartActivity(typeof(GiveTakeLoanActivity)); }
                    


                };
                popUpmenu.Show();


            };

            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    List<Dictionary<string, string>> buff = PurseRecord.listLoans(0);
                    var mainList = FindViewById<LinearLayout>(Resource.Id.loansmainlayout);
                    var debitorsList = FindViewById<LinearLayout>(Resource.Id.loansdebitorlayout);
                    var creditorsList = FindViewById<LinearLayout>(Resource.Id.loanscreditorslayout);

                    int cnt = 0;
                    
                    foreach (var a in buff)
                    {

                        try
                        {
                            if (a["purse_credit_type"] == "credit_out")
                                debitorsList.AddView(TableContents.LoansContent(this, a, cnt));
                            if (a["purse_credit_type"] == "credit_in")
                                creditorsList.AddView(TableContents.LoansContent(this, a, cnt));

                            mainList.AddView(TableContents.LoansContent(this, a, cnt));
                            cnt++;
                        }
                        catch (Exception ex) { }
                    }
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
        protected override void OnResume()
        {
            
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Loans); 
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
    }
    #endregion

    #region LoanItemActivity
    [Activity(Label = "LoanItemActivity")]
    public class LoanItemActivity : Activity, ViewTreeObserver.IOnScrollChangedListener
    {
        ProgressDialog progress;

        public void OnScrollChanged()
        {
            List<PurseRecord> tmp = new List<PurseRecord>();
            LinearLayout mainList = FindViewById<LinearLayout>(Resource.Id.loanshistorylayout);
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.loanshistoryscroll);


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
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND pr.IdPurse=" + GlobalVariables.CurrentViewLoanHistory);
                }
                catch (Exception ex) { }
                foreach (var a in tmp)
                {
                    mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Loan", 2));
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
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Loans);
            if (GlobalVariables.StateActivity == true) 
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoansItemLayout);

            Display display = WindowManager.DefaultDisplay;
            int width_screen = display.Width;
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    var Date = FindViewById<TextView>(Resource.Id.tDate);
                    var Creditor = FindViewById<TextView>(Resource.Id.tCreditor);
                    var Date_return = FindViewById<TextView>(Resource.Id.tDate_return);
                    var Percent = FindViewById<TextView>(Resource.Id.tPercent);
                    var Remaining = FindViewById<TextView>(Resource.Id.tRemaining);
                    var Remaining_summ = FindViewById<TextView>(Resource.Id.tRemaining_summ);
                    var Summ = FindViewById<TextView>(Resource.Id.tSumm);
                    var Summ_return = FindViewById<TextView>(Resource.Id.tSumm_return);
                    // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                    Dictionary<string, string> buf1 = new Dictionary<string, string>();
                    List<Dictionary<string, string>> buff = PurseRecord.listLoans(0);
                    foreach (var a in buff)
                    {
                        if (Convert.ToInt32(a["Id"]) == GlobalVariables.CurrentViewLoan)
                        {
                            buf1 = a;
                        }
                    }

                    FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf1["Name"];


                    Date.Text = buf1["DateStart"];
                    Creditor.Text = buf1["Name"];
                    Date_return.Text = buf1["date_return"];
                    Percent.Text = buf1["percent_return"] + "%";
                    int ddd = ((DateTime.ParseExact(buf1["date_return"], "dd-MM-yyyy", CultureInfo.InvariantCulture) - DateTime.Now).Days);
                    if (ddd > 0) Remaining.Text = ddd.ToString() + "days";
                    else Remaining.Text = "0days";
                    Remaining_summ.Text = buf1["summ_return_left"] + " " + buf1["ShortName"];
                    Summ.Text = buf1["Summ1"] + " " + buf1["ShortName"];
                    Summ_return.Text = buf1["summ_return"] + " " + buf1["ShortName"];

                    #region Рух коштів

                    List<PurseRecord> tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), Convert.ToInt32(buf1["IdPurse"]));
                    GlobalVariables.CurrentPurseToTransfer = Convert.ToInt32(buf1["IdPurse"]);


                    var mainList = FindViewById<LinearLayout>(Resource.Id.loanshistorylayout);
                    
                    #region Контент таблиці
                    ScrollView sc = FindViewById<ScrollView>(Resource.Id.loanshistoryscroll);
                    sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, " AND pr.IdPurse=" + Convert.ToInt32(buf1["IdPurse"]));

                    if (mainList.ChildCount == 0)
                    {

                        mainList.AddView(TableHeaders.CashFlowHeader(this));
                        foreach (var a in tmp)
                        {
                            mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Loan", 2));
                        }
                    }

                    #endregion
                    #endregion
                    var Back = FindViewById<View>(Resource.Id.backbutton);
                    Back.Click += delegate
                    {
                        GlobalVariables.StateActivity = true;
                        
                        Finish();
                        //StartActivity(typeof(LoansActivity));
                    };

                    FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
                    {

                        PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                        popUpmenu.Inflate(Resource.Menu.Pop_upLoans);

                        popUpmenu.MenuItemClick += (s1, arg1) =>
                        {
                            GlobalVariables.CurrentTitleName = arg1.Item.TitleFormatted.ToString();
                            if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Borrow_More)) { GlobalVariables.CurrentInsertTransfer = 1; GlobalVariables.CurrentTabTypeWrite = 1; }
                            if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Return_Money)) { GlobalVariables.CurrentInsertTransfer = 2; GlobalVariables.CurrentTabTypeWrite = 0; }
                            StartActivity(typeof(InsertLoansActivity));
                        };
                        popUpmenu.Show();
                    };

                    progress.Dismiss();
                });
            })).Start();
        }
        
    }
    #endregion

    #region InsertLoansActivity
    [Activity(Label = "InsertLoansActivity")]
    public class InsertLoansActivity : Activity
    {



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.InsertLoansLayout);
            
            FindViewById<LinearLayout>(Resource.Id.FlyOutContent).RequestFocus();
            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;
            #region Конрагент
            var SpinnerConragent = FindViewById<Spinner>(Resource.Id.sContagent);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Contragent.getContrnames(new SqliteConnection(GlobalVariables.dbPath)));
            SpinnerConragent.Adapter = adapter;
            #endregion
            #region Рахунки
            //if (Purse.allPurse == null) { Purse.getPursesTreeSTR(new SqliteConnection(GlobalVariables.dbPath), GlobalVariables.current_user,GlobalVariables.current_dir); }
            Dictionary<int, Dictionary<string, string>> CurrentPurse = Purse.purseList("", "", "p.Id = " + GlobalVariables.CurrentPurseToTransfer.ToString());
            string WritesToEdittextWithCurrentPurse;
            EditText ePurseSpinner = new EditText(this);
            
            EditText ePurseToTransferSpinner = new EditText(this);
            int currt = GlobalVariables.CurrentInsertTransfer; 
            switch (GlobalVariables.CurrentInsertTransfer)
            {
                case 0:
                    ePurseSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sFromPurse));
                    ePurseToTransferSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sToPurse));
                    break; //Transfer
                case 1:
                    ePurseSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sFromPurse));
                    WritesToEdittextWithCurrentPurse = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Name"] + "(" + CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Summ"] + CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["ShortName"] + ")";
                    ePurseToTransferSpinner = FindViewById<EditText>(Resource.Id.sToPurse);
                    ePurseToTransferSpinner.Text = WritesToEdittextWithCurrentPurse;
                    ePurseToTransferSpinner.Tag = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Id"];
                    ePurseToTransferSpinner.Enabled = false;
                    break;//Give Money
                case 2:
                    ePurseToTransferSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sToPurse));
                    WritesToEdittextWithCurrentPurse = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Name"] + "(" + CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Summ"] + CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["ShortName"] + ")";
                    ePurseSpinner = FindViewById<EditText>(Resource.Id.sFromPurse);
                    ePurseSpinner.Tag = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Id"];
                    ePurseSpinner.Text = WritesToEdittextWithCurrentPurse;
                    ePurseSpinner.Enabled = false;
                    break;//take Money
            }


            #endregion

           
            string date = "";
            var Date = FindViewById<EditText>(Resource.Id.tDate);
            Date.Click += delegate
            {
                Date.Text = "";
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    Date.Text = time.ToLongDateString();
                    date = time.ToString("yyyy-MM-dd HH:mm:ss");
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };
            int ID_From;
            int ID_To;
            var RadioNoCommission = FindViewById<RadioButton>(Resource.Id.rWithout_Comission);
            
            var RadioWithCommission = FindViewById<RadioButton>(Resource.Id.rAdd_Comission);
            var RadioSummSendResend = FindViewById<RadioButton>(Resource.Id.rSumm_Sent_Resent);
            LinearLayout.LayoutParams linWithRadios = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams linWithoutRadios = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 80);
            FindViewById<LinearLayout>(Resource.Id.lSumm_Send_Resend).LayoutParameters = linWithoutRadios;
            FindViewById<LinearLayout>(Resource.Id.lWith_Commission).LayoutParameters = linWithoutRadios;
            
            var PriceLayout = FindViewById<LinearLayout>(Resource.Id.tPriceLayout);
            PriceLayout.SetGravity(GravityFlags.Center);
            EditText eFrom = new EditText(this);
            EditText eTo = new EditText(this);
            EditText eConvert = new EditText(this);
            eFrom.SetHint(Resource.String.Discontinued);
            eTo.SetHint(Resource.String.Accepted);
            eFrom.InputType = Android.Text.InputTypes.ClassNumber;
            eTo.InputType = Android.Text.InputTypes.ClassNumber;
            eConvert.InputType = Android.Text.InputTypes.ClassNumber;
            string Firtts = "";
            string Seconds = "";
            string FirstPurse = "";
            string SecondPursev = "";
            LinearLayout.LayoutParams lFrom = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams lTo = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams lConvert = new LinearLayout.LayoutParams(200, LinearLayout.LayoutParams.WrapContent);
            eFrom.Background = Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            eTo.Background = Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            eConvert.Background = Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            lConvert.SetMargins(20, 0, 20, 0);
            int idPurseFrom = -1, idPurseTo= -1 ;
            eFrom.LayoutParameters = lFrom;
            eTo.LayoutParameters = lTo;
            eConvert.LayoutParameters = lConvert;
            eConvert.Text = "1";

            #region Ввід рахунків
            ePurseSpinner.AfterTextChanged += delegate {
                PriceLayout.RemoveAllViewsInLayout();
                
                if (ePurseSpinner.Text !="" && ePurseToTransferSpinner.Text != "")
                {
                    idPurseFrom = Convert.ToInt32(ePurseSpinner.Tag.ToString());
                    idPurseTo = Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString());
                    Dictionary<int, Dictionary<string, string>> bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseSpinner.Tag.ToString()));
                    Dictionary<string, string> buf1 = bufff[idPurseFrom];
                    bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString()));
                    Dictionary<string, string> buf2 = bufff[idPurseTo];
                    FirstPurse = Resources.GetString(Resource.String.CommissionToAccount)+ " " + buf1["Name"];
                    SecondPursev = Resources.GetString(Resource.String.CommissionToAccount) + " " + buf2["Name"];
                    Firtts = buf1["Name"];
                    Seconds = buf2["Name"];
                    if (buf1["ShortName"] != buf2["ShortName"])
                    {
                        
                        RadioSummSendResend.Enabled = false;
                        PriceLayout.AddView(eFrom);
                        PriceLayout.AddView(eTo);
                        PriceLayout.AddView(eConvert);
                        if (eFrom.Text != "" && PriceLayout.ChildCount > 2) eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                        eFrom.TextChanged += delegate {
                            if (eFrom.Text != "" && eConvert.Text != "")
                            {
                                eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                eTo.Enabled = false;
                            }
                            if (eFrom.Text == "") eTo.Enabled = true;
                        };
                        eTo.TextChanged += delegate {
                            if (eFrom.Text != "" && eConvert.Text != "")
                            {
                                eFrom.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                eFrom.Enabled = false;
                            }
                            if (eTo.Text == "") eFrom.Enabled = true;
                        };
                        eConvert.TextChanged += delegate {
                            if (eFrom.Text != "" && eConvert.Text!="")
                            {
                                eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                eTo.Enabled = false;
                            }
                        };
                    }
                    else {
                        PriceLayout.AddView(eFrom);
                        RadioSummSendResend.Enabled = true;
                        if (RadioSummSendResend.Checked) { PriceLayout.AddView(eTo); eTo.Enabled = true; }
                    }
                    if (RadioSummSendResend.Checked) {
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount1).Text = FirstPurse;
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount2).Text = SecondPursev;
                    }
                    if (RadioWithCommission.Checked) {
                        FindViewById<TextView>(Resource.Id.textView312141).Text = FirstPurse;
                        FindViewById<TextView>(Resource.Id.textView331141).Text = SecondPursev;
                    }
                    
                    ID_From = Convert.ToInt32(buf1["Id"]);
                }
            };
            ePurseToTransferSpinner.AfterTextChanged += delegate {
                PriceLayout.RemoveAllViewsInLayout();
                if (ePurseSpinner.Text != "" && ePurseToTransferSpinner.Text != "")
                {
                    idPurseFrom = Convert.ToInt32(ePurseSpinner.Tag.ToString());
                    idPurseTo = Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString());
                    Dictionary<int, Dictionary<string, string>> bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseSpinner.Tag.ToString()));
                    Dictionary<string, string> buf1 = bufff[idPurseFrom];
                    bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString()));
                    Dictionary<string, string> buf2 = bufff[idPurseTo];
                    FirstPurse = Resources.GetString(Resource.String.CommissionToAccount) + " " + buf1["Name"];
                    SecondPursev = Resources.GetString(Resource.String.CommissionToAccount) + " " + buf2["Name"];
                    Firtts = buf1["Name"];
                    Seconds = buf2["Name"];
                    if (buf1["ShortName"] != buf2["ShortName"])
                    {
                        RadioSummSendResend.Enabled = false;
                        
                        
                        PriceLayout.AddView(eFrom);
                        PriceLayout.AddView(eConvert);
                        PriceLayout.AddView(eTo);
                        if (eFrom.Text != "" && PriceLayout.ChildCount > 2) eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                        eFrom.TextChanged += delegate {
                          
                                if (eFrom.Text != "" && eConvert.Text != "")
                                {
                                    eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                    eTo.Enabled = false;
                                }
                                if (eFrom.Text == "") eTo.Enabled = true;
                           
                        };
                        eTo.TextChanged += delegate {
                           
                            if (eTo.Text != "" && eConvert.Text != "")
                            {
                                eFrom.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                eFrom.Enabled = false;
                            }
                            if (eTo.Text == "") eFrom.Enabled = true;
                          
                        };
                        eConvert.TextChanged += delegate {
                           
                            if (eFrom.Text != "" && eConvert.Text != "")
                            {
                                eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                                eTo.Enabled = false;
                            }
                           
                        };
                    }
                    else
                    {
                        PriceLayout.AddView(eFrom);
                        RadioSummSendResend.Enabled = true;
                        if (RadioSummSendResend.Checked) { PriceLayout.AddView(eTo); eTo.Enabled = true; }
                    }
                    if (RadioSummSendResend.Checked)
                    {
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount1).Text = FirstPurse;
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount2).Text = SecondPursev;
                    }
                    if (RadioWithCommission.Checked)
                    {
                        FindViewById<TextView>(Resource.Id.textView312141).Text = FirstPurse;
                        FindViewById<TextView>(Resource.Id.textView331141).Text = SecondPursev;
                    }
                    if (RadioSummSendResend.Checked) { eTo.Enabled = true; }
                    ID_To = Convert.ToInt32(buf2["Id"]);
                }
            };
            #endregion



            RadioNoCommission.CheckedChange += delegate {
                if (RadioNoCommission.Checked)
                {
                    PriceLayout.RemoveAllViewsInLayout();
                    if (ePurseSpinner.Text != "" && ePurseToTransferSpinner.Text != "")
                    {
                        Dictionary<int, Dictionary<string, string>> bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseSpinner.Tag.ToString()));
                        Dictionary<string, string> buf1 = bufff[idPurseFrom];
                        bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString()));
                        Dictionary<string, string> buf2 = bufff[idPurseTo];

                        if (buf1["ShortName"] != buf2["ShortName"])
                        {
                            PriceLayout.AddView(eFrom);
                            PriceLayout.AddView(eConvert);
                            PriceLayout.AddView(eTo);

                        }
                        else
                        {
                            PriceLayout.AddView(eFrom);
                        }
                    }

                    if (eFrom.Text != "" && PriceLayout.ChildCount>2) eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString();
                    RadioSummSendResend.Checked = false;
                    RadioWithCommission.Checked = false;
                    FindViewById<LinearLayout>(Resource.Id.lSumm_Send_Resend).LayoutParameters = linWithoutRadios;
                    FindViewById<LinearLayout>(Resource.Id.lWith_Commission).LayoutParameters = linWithoutRadios;

                }
               
            };
            RadioSummSendResend.CheckedChange += delegate {
                if (RadioSummSendResend.Checked)
                {
                    if (ePurseSpinner.Text != "" && ePurseToTransferSpinner.Text != "")
                    {
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount1).Text = FirstPurse;
                        FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount2).Text = SecondPursev;
                    }
                    eTo.Enabled = true;
                    PriceLayout.RemoveAllViewsInLayout();
                    PriceLayout.AddView(eFrom);
                    PriceLayout.AddView(eTo);
                    RadioNoCommission.Checked = false;
                    RadioWithCommission.Checked = false;
                    FindViewById<LinearLayout>(Resource.Id.lSumm_Send_Resend).LayoutParameters = linWithRadios;
                    FindViewById<LinearLayout>(Resource.Id.lWith_Commission).LayoutParameters = linWithoutRadios;
                    
                }
                
            };
            RadioWithCommission.CheckedChange += delegate {
                if (RadioWithCommission.Checked)
                {
                    PriceLayout.RemoveAllViewsInLayout();
                    if (ePurseSpinner.Text != "" && ePurseToTransferSpinner.Text != "")
                    {
                        FindViewById<TextView>(Resource.Id.textView312141).Text = FirstPurse;
                        FindViewById<TextView>(Resource.Id.textView331141).Text = SecondPursev;
                        Dictionary<int, Dictionary<string, string>> bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseSpinner.Tag.ToString()));
                        Dictionary<string, string> buf1 = bufff[idPurseFrom];
                        bufff = Purse.purseList("", "", "p.Id=" + Convert.ToInt32(ePurseToTransferSpinner.Tag.ToString()));
                        Dictionary<string, string> buf2 = bufff[idPurseTo];
                        if (buf1["ShortName"] != buf2["ShortName"])
                        {
                            PriceLayout.AddView(eFrom);
                            PriceLayout.AddView(eConvert);
                            PriceLayout.AddView(eTo);

                        }
                        else
                        {
                            PriceLayout.AddView(eFrom);
                        }
                    }
                    if (eFrom.Text != "" && PriceLayout.ChildCount > 2) eTo.Text = (Convert.ToInt32(eFrom.Text) * Convert.ToInt32(eConvert.Text)).ToString(); 
                    RadioNoCommission.Checked = false;
                    RadioSummSendResend.Checked = false;
                    FindViewById<LinearLayout>(Resource.Id.lWith_Commission).LayoutParameters = linWithRadios;
                    FindViewById<LinearLayout>(Resource.Id.lSumm_Send_Resend).LayoutParameters = linWithoutRadios;
                }
            };
            
            FindViewById<Button>(Resource.Id.btnOK).Click += delegate {
                
                int SummFrom;
                int SummTo;
                List<int> ContrIds = Contragent.getContrIds();
                #region Без комісії
                if (RadioNoCommission.Checked) {
                    if (eFrom.Text != "") {
                        SummFrom = Convert.ToInt32(eFrom.Text);
                        SummTo = Convert.ToInt32(eFrom.Text);
                        if (eTo.Text != "")
                        {
                            SummTo = Convert.ToInt32(eTo.Text);
                        }
                        int ContrId = ContrIds[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        string columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                        string descIns = FindViewById<EditText>(Resource.Id.tDescription).Text;
                        List<int> Contr = Contragent.getContrIds();
                        
                        string query = idPurseFrom + "," + 2 + ",'" + date + "'," + SummFrom + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_To) + " " + Seconds + ")") + "'," + SummFrom + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        string query2 = idPurseTo + "," + 54 + ",'" + date + "'," + SummTo + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_From) + " " + Firtts + ")") + "'," + SummTo + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        try
                        {
                            

                            TableContents.InsertData("PurseRecord", columns, query);
                            TableContents.InsertData("PurseRecord", columns, query2);
                            if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Sell_Asset))
                            {
                                columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                query = "'DateEnd','" + DateTime.Now.ToString("dd.MM.yyyy") + "'," + GlobalVariables.CurrentViewAsset + "," + GlobalVariables.current_user;
                                TableContents.InsertData("PurseProp", columns, query);
                            }

                            GlobalVariables.StateActivity = true;
                            Finish();
                            Finish();
                            Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        }
                        catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                        this.Finish();
                    }  
                }
                #endregion
                #region Сума відправки відрізняється
                if (RadioSummSendResend.Checked) {
                    if (eFrom.Text != ""&& eTo.Text != "")
                    {
                        SummFrom = Convert.ToInt32(eFrom.Text);
                        SummTo = Convert.ToInt32(eTo.Text);
                        if (SummFrom > SummTo)
                        {
                            int ContrId = ContrIds[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                            string columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                            string descIns = FindViewById<EditText>(Resource.Id.tDescription).Text;
                            int idPurseCommission = 0;

                            if (FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount1).Checked) idPurseCommission = idPurseFrom;
                            if (FindViewById<RadioButton>(Resource.Id.Radio_CommisionToAccount2).Checked) idPurseCommission = idPurseTo;

                            List<int> Contr = Contragent.getContrIds();
                            string query = idPurseFrom + "," + 2 + ",'" + date + "'," + SummFrom + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_To) + " " + Seconds + ")") + "'," + SummFrom + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                            string query2 = idPurseTo + "," + 54 + ",'" + date + "'," + SummFrom + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_From) + " " + Firtts + ")") + "'," + SummTo + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                            string query3 = idPurseCommission + "," + 3 + ",'" + date + "'," + (SummFrom - SummTo) + ",'" + Resources.GetString(Resource.String.Commission) + "'," + SummTo + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                            try
                            {
                                TableContents.InsertData("PurseRecord", columns, query);
                                TableContents.InsertData("PurseRecord", columns, query2);
                                TableContents.InsertData("PurseRecord", columns, query3);
                                GlobalVariables.StateActivity = true;
                                Finish();
                                Finish();
                                Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                            }
                            catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                        }
                        else { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                        this.Finish();
                    }

                }
                #endregion
                #region Комісія
                if (RadioWithCommission.Checked) {
                    if (eFrom.Text != "")
                    {
                        SummFrom = Convert.ToInt32(eFrom.Text);
                        SummTo = Convert.ToInt32(eFrom.Text);
                        if (eTo.Text != "")
                        {
                            SummTo = Convert.ToInt32(eTo.Text);
                        }
                        int ContrId = ContrIds[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        string columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                        string descIns = FindViewById<EditText>(Resource.Id.tDescription).Text;
                        List<int> Contr = Contragent.getContrIds();
                        int cntPercents = 0;
                        int cntPercents2 = 0;
                        
                        string query = idPurseFrom + "," + 2 + ",'" + date + "'," + SummFrom + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_To)+" "+Seconds+")") + "'," + SummFrom + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        string query2 = idPurseTo + "," + 54 + ",'" + date + "'," + SummTo + ",'" + (descIns + " (" + Resources.GetString(Resource.String.Transfer_From) + " " + Firtts + ")") + "'," + SummTo + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                        try
                        {
                            TableContents.InsertData("PurseRecord", columns, query);
                            TableContents.InsertData("PurseRecord", columns, query2);

                            if (FindViewById<CheckBox>(Resource.Id.tChecked1).Checked)
                            {
                                if (FindViewById<EditText>(Resource.Id.Percent1).Text != "")
                                {
                                    cntPercents = Convert.ToInt32(FindViewById<EditText>(Resource.Id.Percent1).Text);
                                    if (FindViewById<RadioButton>(Resource.Id.CheckBox1ComissionPercents).Checked)
                                    {
                                        cntPercents = SummFrom * cntPercents / 100;
                                        
                                    }
                                }
                            }
                            if (FindViewById<CheckBox>(Resource.Id.tChecked2).Checked)
                            {
                                if (FindViewById<EditText>(Resource.Id.Percent2).Text != "")
                                {
                                    cntPercents2 = Convert.ToInt32(FindViewById<EditText>(Resource.Id.Percent2).Text);
                                    if (FindViewById<RadioButton>(Resource.Id.CheckBox2ComissionPercents).Checked)
                                    {
                                        cntPercents2 = SummTo * cntPercents2 / 100;
                                    }
                                   
                                }
                            }



                            if (cntPercents > 0)
                            {
                                string query3 = idPurseFrom + "," + 3 + ",'" + date + "'," + cntPercents + ",'" + Resources.GetString(Resource.String.Commission) + "'," + cntPercents + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                                TableContents.InsertData("PurseRecord", columns, query3);
                            }
                            if (cntPercents2 > 0)
                            {
                                string query3 = idPurseTo + "," + 3 + ",'" + date + "'," + cntPercents2 + ",'" + Resources.GetString(Resource.String.Commission) + "'," + cntPercents2 + "," + "1," + Contr[Convert.ToInt32(FindViewById<Spinner>(Resource.Id.sContagent).SelectedItemId)];
                                TableContents.InsertData("PurseRecord", columns, query3);
                            }
                            GlobalVariables.StateActivity = true;
                            Finish();
                            Finish();
                            Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        }
                        catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                        this.Finish();
                    }
                }
                #endregion

            };
            FindViewById<Button>(Resource.Id.btnCancel).Click += delegate {
                Finish();
            };
            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate
            {
                Finish();
                //StartActivity(typeof(LoansActivity));
            };
        }
    }
    #endregion

    #region GiveTakeLoanActivity
    [Activity(Label = "GiveTakeLoan")]
    public class GiveTakeLoanActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.GiveTakeLoanLayout);

            FindViewById<LinearLayout>(Resource.Id.FlyOutContent).RequestFocus();
            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;

            #region Рахунки


            EditText ePurseSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sPurse));



            #endregion


            string date_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime date_return = DateTime.Now;
            var Date_start = FindViewById<EditText>(Resource.Id.tDate_start);
            var Date_return = FindViewById<EditText>(Resource.Id.tDate_return);
            Date_start.Click += delegate
            {
                Date_start.Text = "";
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    Date_start.Text = time.ToLongDateString();
                    date_start = time.ToString("yyyy-MM-dd HH:mm:ss");
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };
            Date_return.Click += delegate
            {
                Date_return.Text = "";
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    Date_return.Text = time.ToLongDateString();
                    date_return = time;
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            FindViewById<Button>(Resource.Id.btnOK).Click+=delegate {
                if (Date_return.Text != "" && Date_start.Text != "" && ePurseSpinner.Text != "" && FindViewById<EditText>(Resource.Id.tCreditor).Text != "" && FindViewById<EditText>(Resource.Id.tpercents).Text != "" && FindViewById<EditText>(Resource.Id.tPrice).Text != "")
                {
                    try
                    {
                        string creditrtype = "";
                        if (GlobalVariables.GiveTakeLoanIndex == 0) creditrtype = "credit_out";
                        else creditrtype = "credit_in";
                        int idcurr = 0;
                        string Currency = Purse.purseList("", "", "p.Id=" + ePurseSpinner.Tag)[Convert.ToInt32(ePurseSpinner.Tag)]["ShortName"];
                        Dictionary<int, string> cuu = AppCurrency.getAppCurr(new SqliteConnection(GlobalVariables.dbPath));
                        foreach (var a in cuu)
                        {
                            if (a.Value == Currency)
                            {
                                idcurr = a.Key;
                            }
                        }
                        string columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                        string Name = FindViewById<EditText>(Resource.Id.tCreditor).Text;
                        string Description = FindViewById<EditText>(Resource.Id.tDescription).Text;
                        string query = "'" + Name + "'," + idcurr + "," + 5 + ",'" + date_start + "','" + Description + "'," + GlobalVariables.current_user + "," + 0 + "," + GlobalVariables.current_dir;
                        bool rez = TableContents.InsertData("Purse", columns, query);
                        GlobalVariables.StateActivity = true;
                        if (rez)
                        {

                            columns = "MetaKey,MetaValue,IdPurse,IdUser";
                            query = "'date_return','" + date_return.ToString("dd.MM.yyyy") + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                            rez = TableContents.InsertData("PurseProp", columns, query);

                            query = "'percent_return'," + FindViewById<EditText>(Resource.Id.tpercents).Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                            rez = TableContents.InsertData("PurseProp", columns, query);
                            query = "'purse_credit_type','" + creditrtype + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                            rez = TableContents.InsertData("PurseProp", columns, query);

                            query = "'summ_return'," + (Convert.ToDouble(FindViewById<EditText>(Resource.Id.tPrice).Text) - Convert.ToDouble(FindViewById<EditText>(Resource.Id.tPrice).Text) * Convert.ToDouble(FindViewById<EditText>(Resource.Id.tpercents).Text) / 100) + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                            rez = TableContents.InsertData("PurseProp", columns, query);

                            if (creditrtype == "credit_in")
                            {
                                columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                query = ePurseSpinner.Tag + "," + 1 + ",'" + date_start + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + ",'" + FindViewById<EditText>(Resource.Id.tDescription).Text + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + "," + "1,1";
                                rez = TableContents.InsertData("PurseRecord", columns, query);
                            }
                            else
                            {
                                columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                query = ePurseSpinner.Tag + "," + 3 + ",'" + date_start + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + ",'" + FindViewById<EditText>(Resource.Id.tDescription).Text + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + "," + "1,1";
                                rez = TableContents.InsertData("PurseRecord", columns, query);
                            }



                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + date_start + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + ",'" + FindViewById<EditText>(Resource.Id.tDescription).Text + "'," + FindViewById<EditText>(Resource.Id.tPrice).Text + "," + "1,1";
                            rez = TableContents.InsertData("PurseRecord", columns, query);
                            Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();

                            Finish();
                            GlobalVariables.insert = PurseRecord.listLoans(Purse.purseMaxValueId());


                        }
                        else
                        {
                            Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();
                            Finish();
                        }

                    }
                    catch (Exception ex) { }
                }
            };

            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate
            {
                Finish();
                //StartActivity(typeof(LoansActivity));
            };
        }
    }
    #endregion
}