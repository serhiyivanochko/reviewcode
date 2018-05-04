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
using com.refractored.fab;
using System.Threading;

namespace DayMax
{
    #region AssetsActivity
    [Activity(Label = "AssetsActivity")]
    public class AssetsActivity : Activity
    {
        ProgressDialog progress;
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Assets);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AssetsMain);
            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;

            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_Asset);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Add_Asset);
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Add_Asset))
                        StartActivity(typeof(AssetAddNewActivity));

                    
                };
                popUpmenu.Show();


            };

            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    List<Asset> buff = PurseRecord.assets();
                    var mainList = FindViewById<LinearLayout>(Resource.Id.assetsmainlayout);
                    int cnt = 0;
                    //mainList.AddView(TableHeaders.AssetsLoansHeader(this));
                    foreach (var a in buff)
                    {
                        
                            mainList.AddView(TableContents.AssetsContent(this, a, cnt));
                        
                        cnt++;
                    }
                    progress.Dismiss();
                });
            })).Start();

            #region Добавити запис

            //   var imageadd = FindViewById<FloatingActionButton>(Resource.Id.fab);

            //var add_new = FindViewById<ImageView>(Resource.Id.addnewitem);
            //add_new.Click += delegate
            //{
            //GlobalVariables.CurrentViewAsset = tmp.Id;
            //    StartActivity(typeof(AssetAddNewActivity));
            //};
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
    }
    #endregion

    #region AssetItemActivity
    [Activity(Label = "AssetItemActivity")]
    public class AssetItemActivity : Activity, ViewTreeObserver.IOnScrollChangedListener
    {
        ProgressDialog progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AssetsItemLayout);
            Display display = WindowManager.DefaultDisplay;
            int width_screen = display.Width;
            #region Меню
            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_AssetItem);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Delete))
                    {
                        List<PurseRecord> tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), GlobalVariables.CurrentViewAsset);
                        TableContents.DeleteData("Purse", GlobalVariables.CurrentViewAsset);
                        foreach (var a in tmp)
                        {
                            TableContents.DeleteData("PurseRecord", a.Id);
                        }
                        GlobalVariables.StateActivity = true;
                        this.Finish();
                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Assets);
                        Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        StartActivity(typeof(AssetsActivity));
                    }
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Sell_Asset))
                    {

                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Sell_Asset);
                        GlobalVariables.CurrentInsertTransfer = 0;

                        StartActivity(typeof(InsertLoansActivity));
                    }
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Reg_Payment))
                    {
                        GlobalVariables.CurrentInsertTransfer = 1;
                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Reg_Payment);
                        StartActivity(typeof(InsertLoansActivity));
                    }
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Change_Cost))
                    {
                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Change_Cost);
                        StartActivity(typeof(AssetAddChangeCostActivity));
                    }
                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Passive_Profit))
                    {
                        GlobalVariables.PreviousTitleActivity = GlobalVariables.CurrentTitleName;
                        GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Passive_Profit);
                        GlobalVariables.CurrentTabTypeWrite = 2;
                        
                        StartActivity(typeof(InsertCashFlowActivity));
                    }

                    
                };
                popUpmenu.Show();


            };
            #endregion

            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    var Date = FindViewById<TextView>(Resource.Id.tDate);
                    var Name = FindViewById<TextView>(Resource.Id.tName);
                    var category = FindViewById<TextView>(Resource.Id.tCategory);
                    var Gain_on_sale = FindViewById<TextView>(Resource.Id.tGain_on_sale);
                    var Pag = FindViewById<TextView>(Resource.Id.tPag);
                    var Price = FindViewById<TextView>(Resource.Id.tPrice);
                    var Sell_date = FindViewById<TextView>(Resource.Id.tSell_date);
                    var Current_price = FindViewById<TextView>(Resource.Id.tCurrent_price);
                    int A = GlobalVariables.CurrentViewAsset;
                    // var Cash = FindViewById<TextView>(Resource.Id.tCash);
                    Asset buf1 = new Asset();
                    List<Asset> buff = PurseRecord.assets(GlobalVariables.CurrentViewAsset);
                    buf1 = buff[0];


                    FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = buf1.Name;

                    Date.Text = buf1.DateStart.ToString("dd-MM-yyyy");
                    Name.Text = buf1.Name;
                    category.Text = buf1.Category;
                    Gain_on_sale.Text = (Convert.ToDouble(buf1.Current_price) - Convert.ToDouble(buf1.Summ_buy)).ToString() + buf1.ShortName;
                    Pag.Text = "";
                    Price.Text = buf1.Summ_buy + buf1.ShortName;
                    Sell_date.Text = buf1.DateEnd.ToString("dd-MM-yyyy");
                    Current_price.Text = buf1.Current_price.ToString() + buf1.ShortName;
                    GlobalVariables.CurrentPurseToTransfer = buf1.IdPurse;
                    #region Рух коштів
                    #region Шапка таблиці
                    
                    var mainList = FindViewById<LinearLayout>(Resource.Id.assetshistorylayout);

                    #endregion

                    #region Контент таблиці
                    ScrollView sc = FindViewById<ScrollView>(Resource.Id.assetshistoryscroll);
                    sc.ViewTreeObserver.AddOnScrollChangedListener(this);
                    List<PurseRecord> tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0," AND pr.IdPurse="+buf1.IdPurse);

                    if (mainList.ChildCount == 0)
                    {

                        mainList.AddView(TableHeaders.CashFlowHeader(this));
                        foreach (var a in tmp)
                        {
                            mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Asset",1));
                        }
                    }

                    #endregion
                    #endregion
                    progress.Dismiss();
                });
            })).Start();
            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate {
                GlobalVariables.StateActivity = true;
                Finish();
                //StartActivity(typeof(AssetsActivity));
            };

        }
        public void OnScrollChanged()
        {
            List<PurseRecord> tmp = new List<PurseRecord>();
            LinearLayout mainList = FindViewById<LinearLayout>(Resource.Id.assetshistorylayout);
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.assetshistoryscroll);
            

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
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND pr.IdPurse=" + GlobalVariables.CurrentViewAsset);
                }
                catch (Exception ex) { }
                foreach (var a in tmp)
                {
                    mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Asset", 1));
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
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Assets);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
    }
    #endregion

    #region AssetAddNewActivity
    [Activity(Label = "AssetAddNewActivity")]
    public class AssetAddNewActivity : Activity
    {
        

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AssetsAddItemLayout);
            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            FindViewById<LinearLayout>(Resource.Id.FlyOutContent).RequestFocus();
            
            var Name = FindViewById<EditText>(Resource.Id.tAssetName);
            var Price = FindViewById<EditText>(Resource.Id.tPrice);
            
            var Description = FindViewById<EditText>(Resource.Id.tDescription);
            var Date = FindViewById<EditText>(Resource.Id.tDate);
            var Currency = FindViewById<Spinner>(Resource.Id.tCurrency);
            var Checked = FindViewById<CheckBox>(Resource.Id.tChecked);
            var OK = FindViewById<Button>(Resource.Id.btnOK);
            var Cancel = FindViewById<Button>(Resource.Id.btnCancel);
            var sPurse = FindViewById<LinearLayout>(Resource.Id.ShowPurse);
            sPurse.Enabled = false;

            List<PurseCat> pc = PurseCat.GetCatList(3);
            List<string> catts = new List<string>();
            foreach (var a in pc)
            {
                catts.Add(a.Name);
            }
            var Category = FindViewById<Spinner>(Resource.Id.sCategory);
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, catts);
                    Category.Adapter = adapter;
                });
            })).Start();
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                    Currency.Adapter = adapter;
                });
            })).Start();
            EditText ePurseSpinner = new EditText(this);
            Checked.CheckedChange += delegate {
                if (Checked.Checked)
                {
                    sPurse.Enabled = true;
                    #region Рахунки
                    new Thread(new ThreadStart(() =>
                    {
                        this.RunOnUiThread(() =>
                        {
                            ePurseSpinner = TableContents.PurseTree(this, FindViewById<EditText>(Resource.Id.sPurse));
                        });
                    })).Start();

                    #endregion
                }
                else
                {
                    sPurse.Enabled = false;
                }
            };
            
            
            DateTime dateinsert = DateTime.Now;
            Date.Click += delegate
            {
                Date.Text = "";
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    Date.Text = time.ToLongDateString();
                    dateinsert = time;
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };
            var Summ = FindViewById<EditText>(Resource.Id.tPrice);
            OK.Click += delegate
            {
                if (Name.Text != "" && Price.Text != "" && Date.Text != "")
                {
                    try
                    {
                        string columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                        int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                        int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(Currency.SelectedItemId)];
                        string query = "'" + Name.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Description.Text + "'," + GlobalVariables.current_user + "," + 0 + "," + GlobalVariables.current_dir;
                        bool rez = TableContents.InsertData("Purse", columns, query);
                        if (rez)
                        {
                            if (Summ.Text != "")
                            {
                                columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                query = Purse.purseMaxValueId().ToString() + "," + 9 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Summ.Text + ",'" + Description.Text + "'," + Summ.Text + "," + "1,1";
                                rez = TableContents.InsertData("PurseRecord", columns, query);


                                columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                query = "'summ_buy'," + Summ.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                TableContents.InsertData("PurseProp", columns, query);

                            }
                            if (Checked.Checked)
                            {
                                try
                                {
                                    columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                    query = ePurseSpinner.Tag + "," + 3 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Summ.Text + ",'" + Description.Text + "'," + Summ.Text + "," + "1,1";
                                    rez = TableContents.InsertData("PurseRecord", columns, query);
                                }
                                catch { }
                            }
                        }
                        GlobalVariables.StateActivity = true;
                        GlobalVariables.insertass = PurseRecord.assets(Purse.purseMaxValueId());
                        Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        Finish();


                    }
                    catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                }
            };
            Cancel.Click += delegate
            {
                StartActivity(typeof(AssetsActivity));
            };

            FindViewById<View>(Resource.Id.backbutton).Click += delegate {
                Finish();
                //StartActivity(typeof(AssetsActivity));
            };
        }
    }
    #endregion

    #region AssetAddChangeCostActivity
    [Activity(Label = "AssetAddChangeCostActivity")]
    public class AssetAddChangeCostActivity : Activity
    {


        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AssetAddChangeCostLayout);
            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            FindViewById<LinearLayout>(Resource.Id.FlyOutContent).RequestFocus();
            FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;

            var Date = FindViewById<EditText>(Resource.Id.tDate);
            string date = "";
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
                       FindViewById<View>(Resource.Id.backbutton).Click += delegate {
                Finish();
                //StartActivity(typeof(AssetsActivity));
            };
            FindViewById<Button>(Resource.Id.btnOK).Click += delegate
            {
                if (FindViewById<EditText>(Resource.Id.tPrice).Text != "" && date != "")
                {
                    try
                    {
                        TableContents.UpdateData("PurseProp", " MetaValue = '" + FindViewById<EditText>(Resource.Id.tPrice).Text, "' WHERE MetaKey='summ_buy' AND IdPurse =" + GlobalVariables.CurrentViewAsset);
                        TableContents.UpdateData("Purse", " DateStart = '" + date, "' WHERE Id =" + GlobalVariables.CurrentViewAsset);
                        Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                        GlobalVariables.StateActivity = true;
                        Finish();
                    }
                    catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); Finish(); }
                }
            };
        }
    }
    #endregion
}