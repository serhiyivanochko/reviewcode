using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using Android.Content.Res;
using DayMax.Models;
using System.Threading;
using Android.Views.InputMethods;
using static Android.Widget.AdapterView;

namespace DayMax
{

    #region CashFlowActivity
    [Activity(Label = "CashFlowActivity")]
    public class CashFlowActivity : Activity, ViewTreeObserver.IOnScrollChangedListener
    {
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.CashFlow);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        ProgressDialog progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            base.OnCreate(savedInstanceState);





            Display display = WindowManager.DefaultDisplay;
            int width_screen = display.Width;
            SetContentView(Resource.Layout.CashFlowMain);

            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;


            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_menu);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    GlobalVariables.PreviousTitleActivity = GlobalVariables.CurrentTitleName;
                    GlobalVariables.CurrentTitleName = arg1.Item.TitleFormatted.ToString();
                    
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Insert_Earnings)) { GlobalVariables.CurrentTabTypeWrite = 1; StartActivityForResult(typeof(InsertCashFlowActivity),0); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Insert_Expense)) { GlobalVariables.CurrentTabTypeWrite = 0; StartActivityForResult(typeof(InsertCashFlowActivity), 0); }
                    if (GlobalVariables.CurrentTitleName == Resources.GetString(Resource.String.Insert_Transfer)) { GlobalVariables.CurrentInsertTransfer = 0; GlobalVariables.CurrentTabTypeWrite = 2; StartActivity(typeof(InsertLoansActivity));}
                    
                    
                };
                popUpmenu.Show();


            };
            ScrollView sc = new ScrollView(this);
            #region Вкладки
            
                    TabHost tabHost = FindViewById<TabHost>(Resource.Id.tabHost1);
                    tabHost.Setup();

                    TabHost.TabSpec tabSpec;

                    tabSpec = tabHost.NewTabSpec("tag1");
                    tabSpec.SetIndicator(Resources.GetString(Resource.String.History));

                    tabSpec.SetContent(Resource.Id.cashflowscroll);
                    tabHost.AddTab(tabSpec);


                    tabSpec = tabHost.NewTabSpec("tag2");
                    tabSpec.SetIndicator(Resources.GetString(Resource.String.Earnings));
                    tabSpec.SetContent(Resource.Id.earningscroll);
                    tabHost.AddTab(tabSpec);

                    tabSpec = tabHost.NewTabSpec("tag3");
                    tabSpec.SetIndicator(Resources.GetString(Resource.String.Expenses));
                    tabSpec.SetContent(Resource.Id.expencescroll);
                    tabHost.AddTab(tabSpec);

                    tabSpec = tabHost.NewTabSpec("tag4");
                    tabSpec.SetIndicator(Resources.GetString(Resource.String.Transfers));
                    tabSpec.SetContent(Resource.Id.transferscroll);
                    tabHost.AddTab(tabSpec);
                    tabHost.CurrentTab = 0;
                    GlobalVariables.CurrentTabView = 0;
                    tabHost.TabWidget.GetChildAt(0).LayoutParameters.Width = LinearLayout.LayoutParams.WrapContent;
                    tabHost.TabWidget.GetChildAt(1).LayoutParameters.Width = LinearLayout.LayoutParams.WrapContent;
                    tabHost.TabWidget.GetChildAt(2).LayoutParameters.Width = LinearLayout.LayoutParams.WrapContent;
                    tabHost.TabWidget.GetChildAt(3).LayoutParameters.Width = LinearLayout.LayoutParams.WrapContent;
                    List<PurseRecord> tmp = new List<PurseRecord>();
                    LinearLayout mainList = new LinearLayout(this);
                    
                    tabHost.TabChanged += delegate
                    {
                        switch (tabHost.CurrentTab)
                        {
                            case 0: GlobalVariables.CurrentTabTypeWrite = 2;
                                
                                sc = FindViewById<ScrollView>(Resource.Id.cashflowscroll);
                                sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                                tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath));
                                mainList = FindViewById<LinearLayout>(Resource.Id.cashflowmainlayout);
                                if (mainList.ChildCount == 0)
                                {
                                    
                                    mainList.AddView(TableHeaders.CashFlowHeader(this));
                                    foreach (var a in tmp)
                                    {
                                        mainList.AddView(TableContents.CashFlowContent(this, a, 0, "CashFlow", 0));
                                    }
                                }
                                GlobalVariables.CurrentTabView = 0; break;
                            case 1: GlobalVariables.CurrentTabTypeWrite = 1;
                                sc = FindViewById<ScrollView>(Resource.Id.earningscroll);
                                sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                                tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, " AND ct.TypeWrite = 1 ");
                                mainList = FindViewById<LinearLayout>(Resource.Id.earningmainlayout);
                                if (mainList.ChildCount == 0)
                                {
                                    
                                    mainList.AddView(TableHeaders.CashFlowHeader(this));
                                    foreach (var a in tmp)
                                    {
                                        mainList.AddView(TableContents.CashFlowContent(this, a, 0, "CashFlow", 0));
                                    }
                                }
                                GlobalVariables.CurrentTabView = 1; break;
                            case 2: GlobalVariables.CurrentTabTypeWrite = 0;
                                sc = FindViewById<ScrollView>(Resource.Id.expencescroll);
                                sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                                tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, " AND ct.TypeWrite = 0 ");
                                mainList = FindViewById<LinearLayout>(Resource.Id.expensesmainlayout);
                                if (mainList.ChildCount == 0)
                                {
                                    
                                    mainList.AddView(TableHeaders.CashFlowHeader(this));
                                    foreach (var a in tmp)
                                    {
                                        mainList.AddView(TableContents.CashFlowContent(this, a, 0, "CashFlow", 0));
                                    }
                                }
                                GlobalVariables.CurrentTabView = 2; break;
                            case 3: GlobalVariables.CurrentTabTypeWrite = 2;
                                sc = FindViewById<ScrollView>(Resource.Id.transferscroll);
                                sc.ViewTreeObserver.AddOnScrollChangedListener(this);

                                mainList = FindViewById<LinearLayout>(Resource.Id.transfersmainlayout);
                                if (mainList.ChildCount == 0)
                                {
                                    List<Category> catts = PurseRecord.listAllCatts("WHERE c.Id=2 OR c.Id=54");
                                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, "", " AND (pr.IdCategory=" + catts[0].Id+ " OR pr.IdCategory=" + catts[1].Id+")");
                                    mainList.AddView(TableHeaders.CashFlowHeader(this));
                                    foreach (var a in tmp)
                                    {
                                        mainList.AddView(TableContents.CashFlowContent(this, a, 0, "CashFlow", 0));
                                    }
                                }

                                GlobalVariables.CurrentTabView = 3; break;
                        }
                    };
               
            #endregion

            #region Рух коштів
            
            
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    sc = FindViewById<ScrollView>(Resource.Id.cashflowscroll);
                    sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath));
                    mainList = FindViewById<LinearLayout>(Resource.Id.cashflowmainlayout);
                    if (mainList.ChildCount == 0)
                    {
                        mainList.AddView(TableHeaders.CashFlowHeader(this));
                        foreach (var a in tmp)
                        {
                            mainList.AddView(TableContents.CashFlowContent(this, a, 0, "CashFlow", 0));
                        }
                    }

                    progress.Dismiss();
                });
            })).Start();
            #endregion

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

        public void OnScrollChanged()
        {
            List<PurseRecord> tmp = new List<PurseRecord>() ;
            LinearLayout mainList = new LinearLayout(this);
            ScrollView sc = new ScrollView(this);
            
            switch (GlobalVariables.CurrentTabView) {
                case 0: sc = FindViewById<ScrollView>(Resource.Id.cashflowscroll);break;
                case 1: sc = FindViewById<ScrollView>(Resource.Id.earningscroll); break;
                case 2: sc = FindViewById<ScrollView>(Resource.Id.expencescroll); break;
                case 3: sc = FindViewById<ScrollView>(Resource.Id.transferscroll); break;
            }
            
            double scrollingSpace =
            sc.GetChildAt(0).Height - sc.Height;

            if (scrollingSpace <= sc.ScrollY) // Touched bottom
            {
                switch (GlobalVariables.CurrentTabView)
                {
                    case 0: mainList = FindViewById<LinearLayout>(Resource.Id.cashflowmainlayout);
                        try {
                            PurseRecord.bufflistcashflow.Clear();
                            PurseRecord lastID = new PurseRecord();
                            lastID.Id = Convert.ToInt32(mainList.GetChildAt(mainList.ChildCount-1).Tag);
                            PurseRecord.bufflistcashflow.Add(lastID);

                            tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, "");
                        }
                        catch (Exception ex) { }
                        break;
                    case 1: mainList = FindViewById<LinearLayout>(Resource.Id.earningmainlayout);
                        try
                        {
                            PurseRecord.bufflistcashflow.Clear();
                            PurseRecord lastID = new PurseRecord();
                            lastID.Id = Convert.ToInt32(mainList.GetChildAt(mainList.ChildCount - 1).Tag);
                            PurseRecord.bufflistcashflow.Add(lastID);

                            tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND ct.TypeWrite = 1 ");
                        }
                        catch (Exception ex) { }
                        break;
                    case 2: mainList = FindViewById<LinearLayout>(Resource.Id.expensesmainlayout);
                        try {
                            PurseRecord.bufflistcashflow.Clear();
                            PurseRecord lastID = new PurseRecord();
                            lastID.Id = Convert.ToInt32(mainList.GetChildAt(mainList.ChildCount - 1).Tag);
                            PurseRecord.bufflistcashflow.Add(lastID);

                            tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND ct.TypeWrite = 0 ");
                        }
                        catch (Exception ex) { }
                        break;
                    case 3: mainList = FindViewById<LinearLayout>(Resource.Id.transfersmainlayout);
                        try {
                            PurseRecord.bufflistcashflow.Clear();
                            PurseRecord lastID = new PurseRecord();
                            lastID.Id = Convert.ToInt32(mainList.GetChildAt(mainList.ChildCount - 1).Tag);
                            PurseRecord.bufflistcashflow.Add(lastID);

                            List<Category> catts = PurseRecord.listAllCatts("WHERE c.Id=2 OR c.Id=54");
                            tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, "", " AND (pr.IdCategory=" + catts[0].Id + " OR pr.IdCategory=" + catts[1].Id + ")");
                            
                        }
                        catch (Exception ex) { }
                        break;
                }
                        foreach (var a in tmp)
                        {
                            mainList.AddView(TableContents.CashFlowContent(this, a, 0 , "CashFlow", 0));
                        }


                int ing = sc.ChildCount; 
            }
            else
            {
                //Do the load more like things
            }
        }
    }
    #endregion

    #region CashFlowItemActivity
    [Activity(Label = "CashFlowItemActivity")]
    public class CashFlowItemActivity : Activity
    {
        ProgressDialog progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CashFlowItemLayout);
            //FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;


            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_WithDelete);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    TableContents.DeleteData("PurseRecord",GlobalVariables.CurrentIdCachFlow);
                    GlobalVariables.StateActivity = true;
                    this.Finish();
                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                    switch (GlobalVariables.PreviousActivity) {
                        case 0:StartActivity(typeof(CashFlowActivity));break;
                        case 1: StartActivity(typeof(AssetsActivity)); break;
                        case 2: StartActivity(typeof(LoanItemActivity)); break;
                        case 3: StartActivity(typeof(PurseItemActivity)); break;
                    }

                };
                popUpmenu.Show();


            };


            switch (GlobalVariables.CurrentViewCashFlowAboutItem) {
                #region CashFlow
                case "CashFlow":
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Group = FindViewById<TextView>(Resource.Id.tGroup);
                            var Price = FindViewById<TextView>(Resource.Id.tPrice);
                            var TypeCat = FindViewById<TextView>(Resource.Id.tTypeCat);
                            var Date = FindViewById<TextView>(Resource.Id.tDate);
                            var Cantragent = FindViewById<TextView>(Resource.Id.tContragent);
                            var Comment = FindViewById<TextView>(Resource.Id.tComment);
                            // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                            PurseRecord buf = PurseRecord.get_purse_record_with_current_id(GlobalVariables.CurrentIdCachFlow);
                            Price.Text = buf.Price.ToString();
                            TypeCat.Text = buf.Category;
                            Date.Text = buf.DateRecord.ToString("dd-MM-yyyy");
                            
                            Cantragent.Text = buf.Contragent;
                            Comment.Text = buf.Description;
                            Group.Text = buf.Purse;
                            var Item = FindViewById<TextView>(Resource.Id.titleNameWithoutMenu);
                            Item.Text = buf.Category;
                            progress.Dismiss();
                        });
                    })).Start();
                break;
                #endregion
                #region Asset
                case "Asset":
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Group = FindViewById<TextView>(Resource.Id.tGroup);
                            var Price = FindViewById<TextView>(Resource.Id.tPrice);
                            var TypeCat = FindViewById<TextView>(Resource.Id.tTypeCat);
                            var Date = FindViewById<TextView>(Resource.Id.tDate);
                            var Cantragent = FindViewById<TextView>(Resource.Id.tContragent);
                            var Comment = FindViewById<TextView>(Resource.Id.tComment);
                            int a = GlobalVariables.CurrentViewcashFlow;
                            // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                            //List<Dictionary<string, string>> buff = PurseRecord.assets(0);
                            PurseRecord buf = PurseRecord.get_purse_record_with_current_id(GlobalVariables.CurrentIdCachFlow);
                            Price.Text = buf.Price.ToString();
                            TypeCat.Text = buf.Category;
                            Date.Text = buf.DateRecord.ToString("dd-MM-yyyy");
                            Cantragent.Text = buf.Contragent;
                            Comment.Text = buf.Description;
                            Group.Text = buf.Purse;

                            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf.Category;
                            progress.Dismiss();
                        });
                    })).Start();
                    break;
                #endregion
                #region Loan
                case "Loan":
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Group = FindViewById<TextView>(Resource.Id.tGroup);
                            var Price = FindViewById<TextView>(Resource.Id.tPrice);
                            var TypeCat = FindViewById<TextView>(Resource.Id.tTypeCat);
                            var Date = FindViewById<TextView>(Resource.Id.tDate);
                            var Cantragent = FindViewById<TextView>(Resource.Id.tContragent);
                            var Comment = FindViewById<TextView>(Resource.Id.tComment);
                            // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                            List<Dictionary<string, string>> buff = PurseRecord.listLoans(0);
                            PurseRecord buf = PurseRecord.get_purse_record_with_current_id(GlobalVariables.CurrentIdCachFlow);
                            Price.Text = buf.Price.ToString();
                            TypeCat.Text = buf.Category;
                            Date.Text = buf.DateRecord.ToString("dd-MM-yyyy");
                            Cantragent.Text = buf.Contragent;
                            Comment.Text = buf.Description;
                            Group.Text = buf.Purse;

                            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf.Category;
                            progress.Dismiss();
                        });
                    })).Start();
                    break;
                #endregion
                #region Bill
                case "Bill":
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Group = FindViewById<TextView>(Resource.Id.tGroup);
                            var Price = FindViewById<TextView>(Resource.Id.tPrice);
                            var TypeCat = FindViewById<TextView>(Resource.Id.tTypeCat);
                            var Date = FindViewById<TextView>(Resource.Id.tDate);
                            var Cantragent = FindViewById<TextView>(Resource.Id.tContragent);
                            var Comment = FindViewById<TextView>(Resource.Id.tComment);
                            // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                            PurseRecord buf = PurseRecord.get_purse_record_with_current_id(GlobalVariables.CurrentIdCachFlow);
                            Price.Text = buf.Price.ToString();
                            TypeCat.Text = buf.Category;
                            Date.Text = buf.DateRecord.ToString("dd-MM-yyyy");
                            Cantragent.Text = buf.Contragent;
                            Comment.Text = buf.Description;
                            Group.Text = buf.Purse;

                            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf.Category;
                            
                            progress.Dismiss();
                        });
                    })).Start();
                    break;
                #endregion
                #region Bill
                case "Investment":
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            var Group = FindViewById<TextView>(Resource.Id.tGroup);
                            var Price = FindViewById<TextView>(Resource.Id.tPrice);
                            var TypeCat = FindViewById<TextView>(Resource.Id.tTypeCat);
                            var Date = FindViewById<TextView>(Resource.Id.tDate);
                            var Cantragent = FindViewById<TextView>(Resource.Id.tContragent);
                            var Comment = FindViewById<TextView>(Resource.Id.tComment);
                            // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                            PurseRecord buf = PurseRecord.get_purse_record_with_current_id(GlobalVariables.CurrentIdCachFlow);
                            Price.Text = buf.Price.ToString();
                            TypeCat.Text = buf.Category;
                            Date.Text = buf.DateRecord.ToString("dd-MM-yyyy");
                            Cantragent.Text = buf.Contragent;
                            Comment.Text = buf.Description;
                            Group.Text = buf.Purse;

                            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf.Category;

                            progress.Dismiss();
                        });
                    })).Start();
                    break;
                    #endregion
            }




            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate {
                Finish();
                //StartActivity(typeof(CashFlowActivity));
            };

        }
    }
    #endregion

    #region InsertCashFlowActivity
    [Activity(Label = "InsertCashFlowActivity")]
    public class InsertCashFlowActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.InsertCashFlowLayout);
            
            FindViewById<LinearLayout>(Resource.Id.FlyOutContent).RequestFocus();
            //FindViewById<LinearLayout>(Resource.Id.cashflowinsertframe).RequestFocus();
            GlobalVariables.SelectionSpinner = -1;
            string date;
            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            

            FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;
            
            #region Категорії

            //List<Cattype> cttypes = PurseRecord.listTypeCatts(new SqliteConnection(GlobalVariables.dbPath));

            
            

            var CategorySpinner = TableContents.CattsTree(this, FindViewById<EditText>(Resource.Id.sCategory));
            
            //var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Catts);
            //CategorySpinner.Adapter = adapter;
            #endregion

            #region Рахунки

            if (FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text == Resources.GetString(Resource.String.Passive_Profit)|| FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text == Resources.GetString(Resource.String.GiveEarning) || FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text == Resources.GetString(Resource.String.GiveExpense))
            {
                Dictionary<int, Dictionary<string, string>> CurrentPurse = Purse.purseList("", "", "p.Id = " + GlobalVariables.CurrentPurseToTransfer.ToString());
                FindViewById<EditText>(Resource.Id.sPurse).Text = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Name"];
                FindViewById<EditText>(Resource.Id.sPurse).Tag = CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Id"];
                GlobalVariables.idPurseIns = Convert.ToInt32(CurrentPurse[GlobalVariables.CurrentPurseToTransfer]["Id"]);
                FindViewById<EditText>(Resource.Id.sPurse).Enabled = false;
            }
            else
            {
                var ePurseSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sPurse));
            }
            #endregion

            #region Дата
            DateTime dateinsert = DateTime.Now;
            var Date = FindViewById<EditText>(Resource.Id.tDate);
            Date.Click += delegate
            {
                Date.Text = "";
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    
                    Date.Text = time.ToLongDateString();
                    date = time.ToString();
                    dateinsert = time;
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };
            #endregion

            FindViewById<Button>(Resource.Id.btnOK).Click += delegate {
                
                    List<int> idcats = PurseRecord.listIdCatts(new SqliteConnection(GlobalVariables.dbPath), 0, GlobalVariables.CurrentTabTypeWrite);
                    int IdCatt = Convert.ToInt32(CategorySpinner.Tag);
                    string DateIns = dateinsert.ToString("yyyy-MM-dd HH:mm:ss");
                    string SummIns = FindViewById<EditText>(Resource.Id.tPrice).Text;
                    string DescIns = FindViewById<EditText>(Resource.Id.tDescription).Text;

                
                int purse = GlobalVariables.idPurseIns;

                string columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                string query = purse + ","+ IdCatt + ",'" + DateIns + "'," + SummIns+ ",'"+ DescIns+"',"+SummIns+","+"1,1";
                
                if (DateIns != "" && SummIns != "" && GlobalVariables.idPurseIns!=null)
                {
                    bool rez = TableContents.InsertData("PurseRecord", columns, query);
                    GlobalVariables.StateActivity = true;
                    if (rez)
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        this.Finish();
                        
                    }
                    else { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                }
                else {
                    Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();
                }
            };
            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate {
                Finish();
                //StartActivity(typeof(CashFlowActivity));
            };

        }
    }
    #endregion

}