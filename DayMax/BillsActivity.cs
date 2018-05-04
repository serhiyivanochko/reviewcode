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
using System.Threading;

namespace DayMax
{
    #region BillsActivity
    [Activity(Label = "BillsActivity")]
    public class BillsActivity : Activity
    {
        ProgressDialog progress;
        protected override void OnResume()
        {
            base.OnResume();
            GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Bills);
            if (GlobalVariables.StateActivity == true)
                base.Recreate();
            GlobalVariables.StateActivity = false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            progress = ProgressDialog.Show(this, "", Resources.GetString(Resource.String.Loading));
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BillsMain);
            FindViewById<TextView>(Resource.Id.textNameTitle).Text = GlobalVariables.CurrentTitleName;
            
            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_Purse);

                popUpmenu.MenuItemClick += (s1, arg1) =>
                {



                    if (arg1.Item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Create_Purse))
                        StartActivity(typeof(InsertPurseActivity));

                    GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Create_Purse);
                };
                popUpmenu.Show();


            };

            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                    var mainList = FindViewById<LinearLayout>(Resource.Id.billsmainlayout);
                    Dictionary<int, Dictionary<string, string>> buff = Purse.purseList("", "", "p.IdDirection="+GlobalVariables.current_dir);
                    //LinearLayout tmp = new LinearLayout(this);
                    //tmp.SetBackgroundColor(Resources.GetColor(Resource.Color.TopRowColor));
                    LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                    #region Контент
                    foreach (var a in buff)
                    {
                        mainList.AddView(TableContents.BillsContent(this,a));
                    }

                    #endregion
                    progress.Dismiss();
                });
            })).Start();
            #region Меню і навігація
            var menu = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);
            var menuButton = FindViewById(Resource.Id.MenuButton);
            menuButton.Click += (sender, e) =>
            {
                menu.AnimatedOpened = !menu.AnimatedOpened;



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
                var DayMax = FindViewById<LinearLayout>(Resource.Id.LinearDayMax);
                DayMax.Click += delegate { GlobalVariables.CurrentTitleName = DayMax.Tag.ToString(); StartActivity(typeof(DayMaxActivity)); };
            };
                #endregion
        }
    }
    #endregion

    #region PurseItemActivity
    [Activity(Label = "PurseItemActivity")]
    public class PurseItemActivity : Activity, ViewTreeObserver.IOnScrollChangedListener
    {
        public void OnScrollChanged()
        {
            List<PurseRecord> tmp = new List<PurseRecord>();
            LinearLayout mainList = FindViewById<LinearLayout>(Resource.Id.purseitemmainlayout);
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.purseitemscroll);


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
                    tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), PurseRecord.bufflistcashflow[PurseRecord.bufflistcashflow.Count - 1].Id, " AND pr.IdPurse=" + GlobalVariables.CurrentViewPurse);
                }
                catch (Exception ex) { }
                foreach (var a in tmp)
                {
                   mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Bill",3));
                }
            }
            else
            {
                //Do the load more like things
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PurseItemLayout);

            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            List<PurseRecord> tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), 0, " AND pr.IdPurse=" + GlobalVariables.CurrentViewPurse);
            var mainList = FindViewById<LinearLayout>(Resource.Id.purseitemmainlayout);

            var Name = FindViewById<TextView>(Resource.Id.tNamePurse);
            var Category = FindViewById<TextView>(Resource.Id.tNameCategoryPurse);
            var Description = FindViewById<TextView>(Resource.Id.tDescription);
            var Balance = FindViewById<TextView>(Resource.Id.tBalance);
            var Earnings = FindViewById<TextView>(Resource.Id.tEarnings);
            var Expences = FindViewById<TextView>(Resource.Id.tExpence);
            var Image = FindViewById<ImageView>(Resource.Id.imagePurse);
            
            Dictionary<int, Dictionary<string, string>> buf = Purse.purseList("", "", "p.Id=" + GlobalVariables.CurrentViewPurse);
            Name.Text = buf[GlobalVariables.CurrentViewPurse]["Name"];
            Category.Text = buf[GlobalVariables.CurrentViewPurse]["Type"];
            Description.Text = buf[GlobalVariables.CurrentViewPurse]["Description"];
            Balance.Text = buf[GlobalVariables.CurrentViewPurse]["SummCurrency"];
            Earnings.Text = buf[GlobalVariables.CurrentViewPurse]["Summ1"] + " " + buf[GlobalVariables.CurrentViewPurse]["ShortName"].ToUpper();
            Expences.Text = buf[GlobalVariables.CurrentViewPurse]["Summ2"] + " " + buf[GlobalVariables.CurrentViewPurse]["ShortName"].ToUpper();
            Image.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.Default_Purse_Icon));
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
            ll.Gravity = GravityFlags.Center;
            Image.LayoutParameters = ll;
            #region Меню
            FindViewById<View>(Resource.Id.MoreButton).Click += (sender, arg) =>
            {
                PopupMenu popUpmenu = new PopupMenu(this, FindViewById<View>(Resource.Id.MoreButton));
                popUpmenu.Inflate(Resource.Menu.PopUp_WithDelete);
                popUpmenu.MenuItemClick += (s1, arg1) =>
                {
                    TableContents.DeleteData("Purse", GlobalVariables.CurrentViewPurse);
                    foreach (var a in tmp) {
                        TableContents.DeleteData("PurseRecord",a.Id);
                    }
                    GlobalVariables.StateActivity = true;
                    this.Finish();
                    GlobalVariables.CurrentTitleName = Resources.GetString(Resource.String.Bills);
                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                    StartActivity(typeof(BillsActivity));
                };
                popUpmenu.Show();


            };
            #endregion

            #region Рух коштів
            #region Контент таблиці
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.purseitemscroll);
            sc.ViewTreeObserver.AddOnScrollChangedListener(this);
            

            if (mainList.ChildCount == 0)
            {

                mainList.AddView(TableHeaders.CashFlowHeader(this));
                foreach (var a in tmp)
                {
                    mainList.AddView(TableContents.CashFlowContent(this, a, 0, "Bill", 3));
                }
            }

            #endregion
            #endregion
            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate {
                GlobalVariables.StateActivity = true;
                Finish();
                
            };
        }
    }
    #endregion

    #region InsertPurseActivity
    [Activity(Label = "InsertPurseActivity")]
    public class InsertPurseActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.InsertPurseLayout);

            FindViewById<TextView>(Resource.Id.titleNameWithoutMenu).Text = GlobalVariables.CurrentTitleName;
            List<PurseRecord> tmp = PurseRecord.get_purse_record_list(new SqliteConnection(GlobalVariables.dbPath), GlobalVariables.CurrentViewPurse);
            FindViewById<View>(Resource.Id.MoreButton).Visibility = ViewStates.Invisible;
            var Currency = FindViewById<Spinner>(Resource.Id.tCurrency);
            #region LinearLayouts
            LinearLayout Buttons = new LinearLayout(this),
                Calculate = new LinearLayout(this),
                CalculateEvery = new LinearLayout(this),
                CreditLimit = new LinearLayout(this),
                Direction = new LinearLayout(this),
                CardNumber = new LinearLayout(this),
                Institution = new LinearLayout(this),
                PercentType = new LinearLayout(this),
                PercentInterval = new LinearLayout(this),
                TrasferMoney = new LinearLayout(this),
                NameHouse = new LinearLayout(this),
                NameBank = new LinearLayout(this),
                date = new LinearLayout(this),
                Name = new LinearLayout(this),
                Summ = new LinearLayout(this),
                sCurrency = new LinearLayout(this),
                YearPercent = new LinearLayout(this),
                CountMonth = new LinearLayout(this),
                PaymentType = new LinearLayout(this),
                PayCredit = new LinearLayout(this),
                MonthCommission = new LinearLayout(this),
                Description = new LinearLayout(this);
            #endregion
            EditText date1;
            List<PurseCat> pc = PurseCat.GetCatList(2);
            List<string> catts = new List<string>();
            foreach (var a in pc) {
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


            
            DateTime dateinsert = DateTime.Now;
            Category.ItemSelected += delegate {
                string query = "";
                bool res = false;
                string columns = "";
                LinearLayout rez = FindViewById<LinearLayout>(Resource.Id.insertpursemainframe);
                
                List<string> q;
                rez.RemoveAllViewsInLayout();
                switch (pc[Convert.ToInt32(Category.SelectedItemId)].Id)
                {
                    #region Кредит
                    case 3:
                        NameHouse =TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.House_Name));
                        rez.AddView(NameHouse);

                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);

                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);

                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
        
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion

                        YearPercent = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Year_Percent), "numeric");
                        rez.AddView(YearPercent);

                        CountMonth = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Count_Month), "numeric");
                        rez.AddView(CountMonth);

                        q = new List<string>(); q.Add(Resources.GetString(Resource.String.Anuitentic));q.Add(Resources.GetString(Resource.String.Difference));
                        PaymentType = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Payment_Type), q);
                        rez.AddView(PaymentType);

                        PayCredit = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Pay_Credit_Mounth), "numeric");
                        rez.AddView(PayCredit);

                        MonthCommission = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Month_Comission), "numeric");
                        rez.AddView(MonthCommission);

                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);

                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;

                                EditText eBankName = NameBank.GetChildAt(1) as EditText;
                                EditText eYearPercent = YearPercent.GetChildAt(1) as EditText;
                                EditText eCountMonth = CountMonth.GetChildAt(1) as EditText;
                                Spinner ePaymentType = PaymentType.GetChildAt(1) as Spinner;
                                EditText ePaymentDay = PayCredit.GetChildAt(1) as EditText;
                                EditText eMnthlyCommision = MonthCommission.GetChildAt(1) as EditText;

                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eBankName.Text != "" && eSumm.Text != "" && eMnthlyCommision.Text != "" && ePaymentDay.Text != "" && eCountMonth.Text != "" && eYearPercent.Text != "" && eName.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'bank_name','" + eBankName.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'year_procent'," + eYearPercent.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'count_month'," + eCountMonth.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'payment_type','" + ePaymentType.SelectedItem + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'pay_to_day','" + ePaymentDay.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'monthly_commision'," + eMnthlyCommision.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);
                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };

                        break;
                    #endregion
                    #region Гаманець
                    case 4:
                        
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);

                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion

                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);

                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);

                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);

                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eSumm.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex) { Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show(); }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };


                        break;
                    #endregion
                    #region Депозит
                    case 8:
                        NameBank = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name_Bank));
                        rez.AddView(NameBank);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        TrasferMoney = TableContents.CreateCheckBox(this, Resources.GetString(Resource.String.Transfer_Money));
                        rez.AddView(TrasferMoney);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ),"numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        CountMonth = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Count_Month), "numeric");
                        rez.AddView(CountMonth);
                        YearPercent = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Year_Percent), "numeric");
                        rez.AddView(YearPercent);
                        q = new List<string>();
                        q.Add(Resources.GetString(Resource.String.Simple));
                        q.Add(Resources.GetString(Resource.String.Complex));
                        PercentType = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Percent_Type), q);
                        rez.AddView(PercentType);
                        q = new List<string>();
                        q.Add(Resources.GetString(Resource.String.Every_Day));
                        q.Add(Resources.GetString(Resource.String.Every_Month));
                        q.Add(Resources.GetString(Resource.String.Per_Qartal));
                        q.Add(Resources.GetString(Resource.String.Twice_Year));
                        q.Add(Resources.GetString(Resource.String.Every_Year));
                        q.Add(Resources.GetString(Resource.String.At_the_End));

                        PercentInterval = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Percent_Type), q);
                        rez.AddView(PercentInterval);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);
                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eBankName = NameBank.GetChildAt(1) as EditText;
                                EditText eYearPercent = YearPercent.GetChildAt(1) as EditText;
                                EditText eCountMonth = CountMonth.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                Spinner ePercentType = PercentType.GetChildAt(1) as Spinner;
                                Spinner ePercentInterval = PercentInterval.GetChildAt(1) as Spinner;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eBankName.Text != "" && eYearPercent.Text != "" && eCountMonth.Text != "" && eSumm.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'bank_name','" + eBankName.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'year_procent'," + eYearPercent.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'count_month'," + eCountMonth.Text + "," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'procent_type','" + ePercentType.SelectedItem + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'percent_interval','" + ePercentInterval.SelectedItem + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);
                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };

                        break;
                    #endregion
                    #region Кредитна картка
                    case 9:
                        NameBank = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name_Bank));
                        rez.AddView(NameBank);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        CardNumber = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Card_Number), "numeric");
                        rez.AddView(CardNumber);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        #region Бізнес
                        q = new List<string>();
                        List<int> q1 = new List<int>();
                        query = @"SELECT * FROM Directions";
                        SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
                        conn2.Open();
                        SqliteCommand cmd = new SqliteCommand(query, conn2);

                        SqliteDataReader r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            q.Add(r["Name"].ToString());
                            q1.Add(Convert.ToInt32(r["Id"]));
                        }
                        conn2.Close();
                        Direction = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Direction), q);
                        rez.AddView(Direction);
                        #endregion
                        CreditLimit = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Credit_Limit), "numeric");
                        rez.AddView(CreditLimit);
                        #region Radiobuttons
                        Calculate = TableContents.CreateRadio(this, Resources.GetString(Resource.String.Percent_Calculate));
                        rez.AddView(Calculate);
                        CalculateEvery = TableContents.CreateRadio(this, Resources.GetString(Resource.String.Percent_Calculate_Every));
                        Spinner sp = new Spinner(this);
                        q = new List<string>();
                        q.Add(Resources.GetString(Resource.String.This_Month));
                        q.Add(Resources.GetString(Resource.String.Next_Month));
                        sp.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, q);
                        sp.Background = Resources.GetDrawable(Resource.Drawable.rounded_edittext);
                        LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                        ll1.SetMargins(10,10,10,10);
                        sp.LayoutParameters = ll1;
                        sp.SetPadding(20, 20, 20, 20);
                        Calculate.AddView(sp);
                        LinearLayout Calc1 = Calculate.GetChildAt(0) as LinearLayout;
                        LinearLayout CalcEvery1 = CalculateEvery.GetChildAt(0) as LinearLayout;

                        RadioButton Calc = Calc1.GetChildAt(0) as RadioButton;
                        RadioButton CalcEvery = CalcEvery1.GetChildAt(0) as RadioButton;
                        Calc.CheckedChange += delegate
                        {
                            if (Calc.Checked)
                            {
                                Calculate.GetChildAt(1).Enabled = true;
                                CalcEvery.Checked = false;
                                Calc1.GetChildAt(1).Enabled = true;
                                CalcEvery1.GetChildAt(1).Enabled = false;
                                
                            }

                        };
                        CalcEvery.CheckedChange += delegate
                        {
                            if (CalcEvery.Checked)
                            {
                                Calculate.GetChildAt(1).Enabled = false;
                                Calc.Checked = false;
                                Calc1.GetChildAt(1).Enabled = false;
                                CalcEvery1.GetChildAt(1).Enabled = true;
                            }
                        };
                        rez.AddView(CalculateEvery);
                        #endregion
                        YearPercent = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Year_Percent));
                        rez.AddView(YearPercent);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);

                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eBankName = NameBank.GetChildAt(1) as EditText;
                                EditText eCardNum = CardNumber.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                Spinner eDirection = Direction.GetChildAt(1) as Spinner;
                                EditText eCreditLimit = CreditLimit.GetChildAt(1) as EditText;
                                EditText eYearTerm = YearPercent.GetChildAt(1) as EditText;
                                columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + q1[Convert.ToInt32(eDirection.SelectedItemId)];
                                res = TableContents.InsertData("Purse", columns, query);
                                if (res)
                                {
                                    if (eSumm.Text != "")
                                    {
                                        columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                        query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                        res = TableContents.InsertData("PurseRecord", columns, query);
                                    }
                                }
                                try
                                {
                                    columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                    query = "'bank_name','" + eBankName.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                    TableContents.InsertData("PurseProp", columns, query);

                                    columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                    query = "'card_number','" + eCardNum.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                    TableContents.InsertData("PurseProp", columns, query);

                                    columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                    query = "'credit_limit','" + eCreditLimit.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                    TableContents.InsertData("PurseProp", columns, query);

                                    columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                    query = "'year_proc_proterm','" + eYearTerm.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                    TableContents.InsertData("PurseProp", columns, query);
                                }
                                catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                GlobalVariables.StateActivity = true;
                                this.Finish();
                                Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };

                        Buttons.GetChildAt(1).Click += delegate { Finish(); };


                        break;
                    #endregion
                    #region Дебетна картка
                    case 12:
                        Institution = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Institution));
                        rez.AddView(Institution);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        CardNumber = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Card_Number), "numeric");
                        rez.AddView(CardNumber);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        MonthCommission = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Month_Comission), "numeric");
                        rez.AddView(MonthCommission);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);

                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eInstitution = Institution.GetChildAt(1) as EditText;
                                EditText eCardNum = CardNumber.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eInstitution.Text != "" && eCardNum.Text != "" && eSumm.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'institution','" + eInstitution.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'card_number','" + eCardNum.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };
                        break;
                        
                    #endregion
                    #region Розрахунковий рахунок
                    case 13:
                        Institution = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Institution));
                        rez.AddView(Institution);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        MonthCommission = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Month_Comission), "numeric");
                        rez.AddView(MonthCommission);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);

                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eInstitution = Institution.GetChildAt(1) as EditText;
                                EditText eMonthCommission = MonthCommission.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eInstitution.Text != "" && eMonthCommission.Text != "" && eSumm.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'institution','" + eInstitution.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'monthly_commision','" + eMonthCommission.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };
                        break;
                    #endregion
                    #region Електронний гаманець
                    case 14:
                        Institution = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Institution));
                        rez.AddView(Institution);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        CardNumber = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Card_Number), "numeric");
                        rez.AddView(CardNumber);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        MonthCommission = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Month_Comission), "numeric");
                        rez.AddView(MonthCommission);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);
                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eInstitution = Institution.GetChildAt(1) as EditText;
                                EditText eCardNum = CardNumber.GetChildAt(1) as EditText;
                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eCardNum.Text != "" && eSumm.Text != "" && eInstitution.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'institution','" + eInstitution.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'card_number','" + eCardNum.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);

                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };
                        break;
                    #endregion
                    #region Власні збереження
                    case 15:
                        Institution = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Institution));
                        rez.AddView(Institution);
                        Name = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Name));
                        rez.AddView(Name);
                        Summ = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Summ), "numeric");
                        rez.AddView(Summ);
                        sCurrency = TableContents.CreateSpinner(this, Resources.GetString(Resource.String.Currency), AppCurrency.getAppCurrSTR(new SqliteConnection(GlobalVariables.dbPath)));
                        rez.AddView(sCurrency);
                        #region Date
                        date = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Start_Date), "date");
                        date.GetChildAt(1).Focusable = false;
                        date1 = date.GetChildAt(1) as EditText;
                        date1.Click += delegate {
                            date1.Text = "";
                            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                            {
                                date1.Text = time.ToLongDateString();
                                dateinsert = time;
                            });
                            frag.Show(FragmentManager, DatePickerFragment.TAG);
                        };
                        rez.AddView(date);
                        #endregion
                        MonthCommission = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Month_Comission), "numeric");
                        rez.AddView(MonthCommission);
                        Description = TableContents.CreateEditTextRow(this, Resources.GetString(Resource.String.Description), "multiline");
                        rez.AddView(Description);
                        Buttons = TableContents.CreateButtons(this);
                        rez.AddView(Buttons);
                        Buttons.GetChildAt(0).Click += delegate
                        {

                            try
                            {
                                EditText eName = Name.GetChildAt(1) as EditText;
                                EditText eInstitution = Institution.GetChildAt(1) as EditText;

                                EditText eSumm = Summ.GetChildAt(1) as EditText;
                                EditText eDescription = Description.GetChildAt(1) as EditText;
                                Spinner eCurrency = sCurrency.GetChildAt(1) as Spinner;
                                if (eName.Text != "" && eInstitution.Text != "" && eSumm.Text != "")
                                {
                                    columns = "Name,IdCurrency,IdPurseCat,DateStart,Description,IdUser,Status,IdDirection";
                                    int idPurseCat = pc[Convert.ToInt32(Category.SelectedItemId)].Id;
                                    int idCurr = AppCurrency.getAppCurrId(new SqliteConnection(GlobalVariables.dbPath))[Convert.ToInt32(eCurrency.SelectedItemId)];
                                    query = "'" + eName.Text + "'," + idCurr + "," + idPurseCat + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "','" + eDescription.Text + "'," + GlobalVariables.current_user + "," + 1 + "," + GlobalVariables.current_dir;
                                    res = TableContents.InsertData("Purse", columns, query);
                                    if (res)
                                    {
                                        if (eSumm.Text != "")
                                        {
                                            columns = "IdPurse,IdCategory,DateRecord,Price,Description,Summ,Amount,IdContragent";
                                            query = Purse.purseMaxValueId().ToString() + "," + 1 + ",'" + dateinsert.ToString("yyyy-MM-dd HH:mm:ss") + "'," + eSumm.Text + ",'" + eDescription.Text + "'," + eSumm.Text + "," + "1,1";
                                            res = TableContents.InsertData("PurseRecord", columns, query);
                                        }
                                    }
                                    try
                                    {
                                        columns = "MetaKey,MetaValue,IdPurse,IdUser";
                                        query = "'institution','" + eInstitution.Text + "'," + Purse.purseMaxValueId().ToString() + "," + GlobalVariables.current_user;
                                        TableContents.InsertData("PurseProp", columns, query);



                                    }
                                    catch (Exception ex) { TableContents.CleanData("PurseProp", "IdPurse = " + Purse.purseMaxValueId().ToString()); }
                                    GlobalVariables.StateActivity = true;
                                    this.Finish();
                                    Toast.MakeText(this, Resources.GetString(Resource.String.Success), ToastLength.Short).Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, Resources.GetString(Resource.String.Error), ToastLength.Short).Show();

                            }
                        };
                        Buttons.GetChildAt(1).Click += delegate { Finish(); };
                        break;
                    #endregion
                }

            };

            var Back = FindViewById<View>(Resource.Id.backbutton);
            Back.Click += delegate {
                Finish();

            };
        }
    }
    #endregion


}