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
using System.Globalization;
using Mono.Data.Sqlite;


namespace DayMax.Models
{
    public static class TableContents
    {
        public static LinearLayout CashFlowContent(Context context, PurseRecord a, int cnt, string TypeMenu, int PreviousActivity)
        {

            LinearLayout ContentItem = new LinearLayout(context);
            ContentItem.Orientation = Orientation.Vertical;
            LinearLayout buf = new LinearLayout(context);
            buf.Id = cnt;
            buf.Tag = cnt;
            buf.Orientation = Orientation.Horizontal;
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            buf.LayoutParameters = ll;
            LinearLayout buf2 = new LinearLayout(context);
            ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            ll.Gravity = GravityFlags.CenterVertical;
            ll.LeftMargin = 10;
            buf2.LayoutParameters = ll;


            LinearLayout buf1 = new LinearLayout(context);
            buf1.Orientation = Orientation.Vertical;


            TextView categ = new TextView(context);
            categ.TextSize = 12;
            categ.Text = a.Category + " " + a.Purse;
            ContentItem.Tag = a.Id;
            buf.Click += delegate
            {
                GlobalVariables.CurrentViewcashFlow = a.Id;
                GlobalVariables.CurrentViewCashFlowAboutItem = TypeMenu;
                GlobalVariables.CurrentTitleName = a.Category;
                GlobalVariables.PreviousActivity = PreviousActivity;
                GlobalVariables.CurrentIdCachFlow = a.Id;
                context.StartActivity(typeof(CashFlowItemActivity));
            };
            Dictionary<int, string> dic = AppCurrency.getAppCurr(new SqliteConnection(GlobalVariables.dbPath));
            buf.Orientation = Android.Widget.Orientation.Horizontal;
            TextView typeCat = new TextView(context);
            try
            {
                typeCat.Text = a.Price.ToString() + dic[a.ShortName];
            }
            catch (Exception ex)
            { typeCat.Text = a.Price.ToString(); }
            typeCat.TextSize = 20;

            typeCat.SetWidth(300);
            TextView dateview = new TextView(context);
            dateview.Text = a.DateRecord.ToString("dd-MM-yyyy");
            dateview.TextSize = 16;
            if (a.type_write == "cat_color_1")
            {

                typeCat.SetTextColor(context.Resources.GetColor(Resource.Color.GreenColor));
            }
            else
            {

                typeCat.SetTextColor(context.Resources.GetColor(Resource.Color.RedColor));
            }
            LinearLayout br = new LinearLayout(context);
            LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(30, LinearLayout.LayoutParams.MatchParent);
            br.LayoutParameters = parms;

            buf2.AddView(typeCat);
            buf1.AddView(dateview);
            buf1.AddView(categ);
            buf.AddView(buf2);
            buf.AddView(buf1);
            View v = new View(context);
            v.SetBackgroundColor(Android.Graphics.Color.White);
            parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
            v.LayoutParameters = parms;

            ContentItem.AddView(buf);

            ContentItem.AddView(v);

            return ContentItem;
        }
        public static LinearLayout LoansContent(Context context, Dictionary<string, string> a, int cnt)
        {

            LinearLayout tmp = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            ll.SetMargins(10, 10, 10, 10);
            tmp.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.background_with_shadow));
            tmp.LayoutParameters = ll;
            tmp.Id = cnt;
            tmp.Orientation = Android.Widget.Orientation.Horizontal;
            //tmp.SetBackgroundColor(Android.Graphics.Color.Rgb(67, 160, 71));
            tmp.Click += delegate
            {
                GlobalVariables.CurrentTitleName = a["Name"];
                GlobalVariables.CurrentViewLoan = Convert.ToInt32(a["Id"]);
                GlobalVariables.PreviousActivity = 1;
                context.StartActivity(typeof(LoanItemActivity));
            };

            ImageView Status = new ImageView(context);
            LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            ll1.SetMargins(0, 0, 20, 0);
            Status.LayoutParameters = ll1;


            TextView Name = new TextView(context);
            Name.TextSize = 18;
            Name.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));
            Name.SetWidth(400);

            TextView Balance = new TextView(context);
            Balance.TextSize = 18;
            Balance.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));


            Name.Text = a["Name"];
            Balance.Text = a["summ_return"];
            int ddd = ((DateTime.ParseExact(a["date_return"], "dd-MM-yyyy", CultureInfo.InvariantCulture) - DateTime.Now).Days);
            if (ddd > 0) Status.SetImageResource(Resource.Drawable.Done);
            else Status.SetImageResource(Resource.Drawable.None);

            tmp.AddView(Status);
            tmp.AddView(Name);
            tmp.AddView(Balance);

            return tmp;



        }
        public static LinearLayout AssetsContent(Context context, Asset a, int cnt)
        {
            LinearLayout tmp = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            ll.SetMargins(10, 10, 10, 10);
            tmp.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.background_with_shadow));
            tmp.LayoutParameters = ll;
            tmp.Id = cnt;
            tmp.Orientation = Android.Widget.Orientation.Horizontal;

            tmp.Click += delegate
            {
                GlobalVariables.CurrentViewAsset = a.Id;
                GlobalVariables.CurrentTitleName = a.Name;

                context.StartActivity(typeof(AssetItemActivity));
            };

            ImageView Status = new ImageView(context);
            LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            ll1.SetMargins(0, 0, 20, 0);
            Status.LayoutParameters = ll1;

            TextView Name = new TextView(context);
            Name.TextSize = 18;
            Name.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));
            Name.SetWidth(400);

            TextView Balance = new TextView(context);
            Balance.TextSize = 18;
            Balance.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));


            Name.Text = a.Name;
            Balance.Text = a.Summ_buy.ToString();

            if (a.Status == 1) Status.SetImageResource(Resource.Drawable.Done);
            else Status.SetImageResource(Resource.Drawable.None);

            tmp.AddView(Status);
            tmp.AddView(Name);
            tmp.AddView(Balance);



            return tmp;

        }
        public static LinearLayout BillsContent(Context context, KeyValuePair<int, Dictionary<string, string>> a)
        {



            LinearLayout tmp1 = new LinearLayout(context);

            List<PurseCat> pc = PurseCat.GetCatList(2);
            List<string> catts = new List<string>();
            foreach (var b in pc)
            {
                catts.Add(b.Name);
            }
            if (catts.Contains(a.Value["Type"]))
            {

                LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                ll.SetMargins(10, 10, 10, 10);
                tmp1.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.background_with_shadow));
                tmp1.LayoutParameters = ll;
                tmp1.Orientation = Android.Widget.Orientation.Vertical;
                ImageView image = new ImageView(context);
                image.SetBackgroundResource(Resource.Drawable.Default_Purse_Icon);
                LinearLayout.LayoutParams il = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                il.Gravity = GravityFlags.CenterHorizontal;
                image.LayoutParameters = il;

                RelativeLayout tmp = new RelativeLayout(context);
                RelativeLayout.LayoutParams ltmp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);

                tmp.LayoutParameters = ltmp;
                tmp1.Click += delegate
                {
                    GlobalVariables.CurrentTitleName = a.Value["Name"];
                    GlobalVariables.CurrentPurseName = a.Value["Name"];
                    GlobalVariables.CurrentViewPurse = Convert.ToInt32(a.Value["Id"]);
                    context.StartActivity(typeof(PurseItemActivity));
                };


                RelativeLayout.LayoutParams ln = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

                TextView Name = new TextView(context);
                Name.TextSize = 18;
                Name.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));


                ln.AddRule(LayoutRules.AlignParentRight);

                TextView Balance = new TextView(context);
                Balance.TextSize = 18;
                Balance.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));
                Balance.LayoutParameters = ln;

                Name.Text = a.Value["Name"];
                Balance.Text = a.Value["SummCurrency"];


                tmp.AddView(Name);
                tmp.AddView(Balance);



                View v = new View(context);
                v.SetBackgroundColor(Android.Graphics.Color.White);

                LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
                v.LayoutParameters = parms;
                //item.AddView(tmp);
                tmp1.AddView(image);
                tmp1.AddView(v);
                tmp1.AddView(tmp);

            }

            return tmp1;
        }

        public static bool InsertData(string table, string columns = "", string query = "", string where = "")
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            SqliteCommand cmd = new SqliteCommand(@"INSERT INTO " + table + " (" + columns + ") VALUES(" + query + ");", conn2);
            //try
            //{
                cmd.ExecuteNonQuery();
                conn2.Close();
                return true;
            //}
            //catch (Exception ex) { conn2.Close(); return false; }

        }
        public static bool UpdateData(string table, string query = "", string where = "")
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            SqliteCommand cmd = new SqliteCommand(@"UPDATE " + table + " SET " + query + where + ";", conn2);
            try
            {
                cmd.ExecuteNonQuery();
                conn2.Close();
                return true;
            }
            catch (Exception ex) { conn2.Close(); return false; }

        }
        public static bool DeleteData(string table, int where)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            SqliteCommand cmd = new SqliteCommand(@"DELETE FROM " + table + " WHERE Id = " + where + ";", conn2);
            try
            {
                cmd.ExecuteNonQuery();
                conn2.Close();
                return true;
            }
            catch (Exception ex) { conn2.Close(); return false; }

        }
        public static bool CleanData(string table, string where)
        {
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            SqliteCommand cmd = new SqliteCommand(@"DELETE FROM " + table + " WHERE " + where + ";", conn2);
            try
            {
                cmd.ExecuteNonQuery();
                conn2.Close();
                return true;
            }
            catch (Exception ex) { conn2.Close(); return false; }

        }


        public static EditText PurseTree(Context context, EditText ePurseSpinner)
        {

            #region ListView AlertDialog
            List<string> CatNamePurse;
            List<List<string>> tmp;
            AlertDialog dlgAlert;
            int positionPurse;

            List<List<int>> IdPurseSelect;
            #endregion
            tmp = new List<List<string>>();
            Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, string>>>> pListBuf = Purse.getPursesTreeSTR(new SqliteConnection(GlobalVariables.dbPath), GlobalVariables.current_user, GlobalVariables.current_dir);
            CatNamePurse = new List<string>();
            IdPurseSelect = new List<List<int>>();

            foreach (int keyDir in pListBuf.Keys)
            {
                foreach (int keyType in pListBuf[keyDir].Keys)
                {

                    CatNamePurse.Add(Purse.listType[keyType]);
                    List<string> buff = new List<string>();
                    List<int> idbuff = new List<int>();
                    foreach (int idPurse in pListBuf[keyDir][keyType].Keys)
                    {
                        string isert = Purse.allPurse[idPurse]["Name"] + "(" + Purse.allPurse[idPurse]["Summ"] + " " + Purse.allPurse[idPurse]["ShortName"] + ")";
                        buff.Add(isert);
                        idbuff.Add(idPurse);
                    }
                    IdPurseSelect.Add(idbuff);
                    tmp.Add(buff);
                }
            }


            ListView _listView = new ListView(context);
            int ind = 0;
            ListView PurseSpinner = new ListView(context);
            ePurseSpinner.Click += (object sender, EventArgs e) =>
            {
                dlgAlert = (new AlertDialog.Builder(context)).Create();
                var listView = new ListView(context);
                listView.Adapter = new AlertListViewAdapter((Activity)context, CatNamePurse);
                listView.ItemClick += (object sender2, AdapterView.ItemClickEventArgs e2) =>
                {
                    dlgAlert.Dismiss();
                    positionPurse = e2.Position;
                    dlgAlert = (new AlertDialog.Builder(context)).Create();
                    listView = new ListView(context);
                    listView.Adapter = new AlertListViewAdapter((Activity)context, tmp[e2.Position]);
                    listView.ItemClick += (object sender1, AdapterView.ItemClickEventArgs e1) =>
                    {
                        dlgAlert.Dismiss();
                        ePurseSpinner.Tag = IdPurseSelect[positionPurse][e1.Position];
                        ePurseSpinner.Text = tmp[positionPurse][e1.Position];
                        GlobalVariables.idPurseIns = IdPurseSelect[positionPurse][e1.Position];

                    };
                    dlgAlert.SetView(listView);
                    dlgAlert.Show();
                };
                dlgAlert.SetView(listView);
                dlgAlert.Show();
            };
            return ePurseSpinner;
        }
        public static EditText CattsTree(Context context, EditText ePurseSpinner)
        {

            #region ListView AlertDialog

            AlertDialog dlgAlert;
            int positionCatt;
            List<Category> Catts = PurseRecord.listCatts(new SqliteConnection(GlobalVariables.dbPath), 0, GlobalVariables.CurrentTabTypeWrite, GlobalVariables.Where);

            #endregion
            List<string> listcatts = new List<string>();
            foreach (var a in Catts) { listcatts.Add(a.Name); }


            ListView _listView = new ListView(context);
            int ind = 0;
            ListView PurseSpinner = new ListView(context);
            ePurseSpinner.Click += (object sender, EventArgs e) =>
            {
                dlgAlert = (new AlertDialog.Builder(context)).Create();
                var listView = new ListView(context);
                listView.Adapter = new AlertListViewAdapter((Activity)context, listcatts);
                listView.ItemClick += (object sender2, AdapterView.ItemClickEventArgs e2) =>
                {
                    dlgAlert.Dismiss();
                    positionCatt = e2.Position;
                    dlgAlert = (new AlertDialog.Builder(context)).Create();
                    listView = new ListView(context);
                    if (Catts[positionCatt].Children.Count > 0)
                    {
                        List<string> SubCatts = new List<string>();
                        foreach (var b in Catts[positionCatt].Children) { SubCatts.Add(b.Name); }
                        listView.Adapter = new AlertListViewAdapter((Activity)context, SubCatts);
                        listView.ItemClick += (object sender1, AdapterView.ItemClickEventArgs e1) =>
                        {
                            dlgAlert.Dismiss();
                            ePurseSpinner.Tag = Catts[positionCatt].Children[e1.Position].Id;
                            ePurseSpinner.Text = Catts[positionCatt].Children[e1.Position].Name;


                        };
                        dlgAlert.SetView(listView);
                        dlgAlert.Show();
                    }
                    else {
                        ePurseSpinner.Tag = Catts[positionCatt].Id;
                        ePurseSpinner.Text = Catts[positionCatt].Name;
                    }
                };
                dlgAlert.SetView(listView);
                dlgAlert.Show();
            };
            return ePurseSpinner;
        }

        public static List<LinearLayout> Business(Context context) {
            List<LinearLayout> buf = new List<LinearLayout>();
            SqliteConnection conn2 = new SqliteConnection(GlobalVariables.dbPath);
            conn2.Open();
            string query = @"SELECT * FROM Directions";
            SqliteCommand cmd = new SqliteCommand(query, conn2);

            SqliteDataReader r = cmd.ExecuteReader();
            while (r.Read()) {



                ImageView image = new ImageView(context);
                image.SetBackgroundResource(Resource.Drawable.Default_Purse_Icon);
                LinearLayout.LayoutParams il = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                il.Gravity = GravityFlags.CenterHorizontal;
                image.LayoutParameters = il;

                View v = new View(context);
                v.SetBackgroundColor(Android.Graphics.Color.White);

                LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
                v.LayoutParameters = parms;

                LinearLayout tmp = new LinearLayout(context);
                LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                ll.SetMargins(10, 10, 10, 10);
                tmp.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.background_with_shadow));
                tmp.LayoutParameters = ll;
                tmp.Id = Convert.ToInt32(r["Id"]);
                tmp.Orientation = Android.Widget.Orientation.Vertical;


                tmp.Click += delegate
                {

                    GlobalVariables.current_dir = Convert.ToInt32(tmp.Id);
                    context.StartActivity(typeof(MainActivity));
                };
                TextView Name = new TextView(context);
                Name.TextSize = 18;
                Name.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));
                Name.Text = r["Name"].ToString();
                if (Convert.ToInt32(r["Default"]) == 1) Name.Text = (r["Name"].ToString() + " (" + context.Resources.GetString(Resource.String.Default) + ")");
                tmp.AddView(image);
                tmp.AddView(v);
                tmp.AddView(Name);
                buf.Add(tmp);
            }

            conn2.Close();
            return buf;
        }
        public static List<LinearLayout> Investment(Context context)
        {
            List<LinearLayout> buf = new List<LinearLayout>();

            Dictionary<int, Dictionary<string, string>> purses = Purse.purseList("", "", "p.IdPurseCat = 1");

            foreach (var a in purses)
            {



                ImageView image = new ImageView(context);
                image.SetBackgroundResource(Resource.Drawable.Default_Purse_Icon);
                LinearLayout.LayoutParams il = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                il.Gravity = GravityFlags.CenterHorizontal;
                image.LayoutParameters = il;

                View v = new View(context);
                v.SetBackgroundColor(Android.Graphics.Color.White);

                LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
                v.LayoutParameters = parms;

                LinearLayout tmp = new LinearLayout(context);
                LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                ll.SetMargins(10, 10, 10, 10);
                tmp.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.background_with_shadow));
                tmp.LayoutParameters = ll;
                tmp.Id = Convert.ToInt32(a.Value["Id"].ToString());
                tmp.Orientation = Android.Widget.Orientation.Vertical;


                tmp.Click += delegate
                {
                    GlobalVariables.CurrentShortName = a.Value["ShortName"];
                    GlobalVariables.CurrentTitleName = a.Value["Name"];
                    GlobalVariables.CurrentViewAboutPurse = Convert.ToInt32(a.Value["Id"]);
                    context.StartActivity(typeof(InvestmentItemActivity));
                };


                RelativeLayout tmp1 = new RelativeLayout(context);
                RelativeLayout.LayoutParams ltmp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);

                tmp1.LayoutParameters = ltmp;
                RelativeLayout.LayoutParams ln = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

                TextView Name = new TextView(context);
                Name.TextSize = 18;
                Name.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));


                ln.AddRule(LayoutRules.AlignParentRight);

                TextView Balance = new TextView(context);
                Balance.TextSize = 18;
                Balance.SetTextColor(context.Resources.GetColor(Resource.Color.TextColor));
                Balance.LayoutParameters = ln;

                Name.Text = a.Value["Name"];
                Balance.Text = a.Value["SummCurrency"];


                tmp1.AddView(Name);
                tmp1.AddView(Balance);


                tmp.AddView(image);
                tmp.AddView(v);
                tmp.AddView(tmp1);
                buf.Add(tmp);
            }
            return buf;
        }
        public static LinearLayout CategoryLinear(Context context, Category a) {


            LinearLayout ContentItem = new LinearLayout(context);
            ContentItem.Orientation = Orientation.Vertical;
            LinearLayout buf = new LinearLayout(context);

            //RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //lp.AddRule(LayoutRules.AlignParentLeft);

            TextView name = new TextView(context);
            
            name.Text = a.Name;
            name.SetMaxWidth(350);
            //name.LayoutParameters = lp;

            //lp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //lp.AddRule(LayoutRules.AlignParentRight);

            TextView ePurseType = new TextView(context);
            name.TextSize = 12;
            Cattype p = Cattype.getCurrentCattype(a.CatType);
            ePurseType.Text = p.Name;
            ePurseType.SetMaxWidth(350);
            //ePurseType.LayoutParameters = lp;

            ContentItem.Tag = a.Id;
            buf.Click += delegate
            {
                //GlobalVariables.CurrentViewcashFlow = a.Id;
                //GlobalVariables.CurrentViewCashFlowAboutItem = TypeMenu;
                //GlobalVariables.CurrentTitleName = a.Category;
                //GlobalVariables.PreviousActivity = PreviousActivity;
                //GlobalVariables.CurrentIdCachFlow = a.Id;
                //context.StartActivity(typeof(CashFlowItemActivity));
            };


            buf.AddView(name);
            buf.AddView(ePurseType);

            View v = new View(context);
            v.SetBackgroundColor(Android.Graphics.Color.White);
            LinearLayout.LayoutParams parms = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 2);
            v.LayoutParameters = parms;

            ContentItem.AddView(buf);

            ContentItem.AddView(v);

            return ContentItem;
            
        }
    
        
        
        public static LinearLayout CreateEditTextRow(Context context, string name, string type = "string")
        {
            LinearLayout rez = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            rez.LayoutParameters = ll;
            rez.Orientation = Orientation.Vertical;
            //rez.SetPadding(10,10,10,10);
            TextView text = new TextView(context);
            text.TextSize = 14;
            text.Text = name;
            text.SetPadding(10, 10, 10, 10);
            EditText edit = new EditText(context);
            edit.Background = context.Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);

            if (type == "numeric") edit.InputType = Android.Text.InputTypes.ClassNumber;
            if (type == "multiline") { edit.InputType = Android.Text.InputTypes.TextFlagMultiLine; edit.SetHeight(150); }

            edit.SetPadding(20, 20, 20, 20);
            rez.AddView(text);
            rez.AddView(edit);
            //text.SetPadding(10, 10, 10, 10);

            return rez;
        }
        public static LinearLayout CreateSpinner(Context context, string name, List<string> values)
        {
            LinearLayout rez = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            rez.LayoutParameters = ll;
            rez.Orientation = Orientation.Vertical;
            //rez.SetPadding(10,10,10,10);
            TextView text = new TextView(context);
            text.TextSize = 14;
            text.Text = name;
            text.SetPadding(10, 10, 10, 10);
            Spinner edit = new Spinner(context);
            edit.Background = context.Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            var adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, values);
            edit.Adapter = adapter;
            edit.LayoutParameters = ll1;
            edit.SetPadding(20, 20, 20, 20);
            rez.AddView(text);
            rez.AddView(edit);
            return rez;
        }
        public static LinearLayout CreateCheckBox(Context context, string name)
        {
            LinearLayout rez = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            rez.LayoutParameters = ll;
            rez.Orientation = Orientation.Horizontal;
            //rez.SetPadding(10,10,10,10);
            TextView text = new TextView(context);
            text.TextSize = 14;
            text.Text = name;
            text.SetPadding(10, 10, 10, 10);
            RadioButton edit = new RadioButton(context);
            
            rez.AddView(text);
            rez.AddView(edit);
            return rez;
        }
        public static LinearLayout CreateRadio(Context context, string name)
        {
            LinearLayout rez = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            rez.LayoutParameters = ll;
            rez.Orientation = Orientation.Vertical;
            rez.SetPadding(10,10,10,10);
            LinearLayout rez1 = new LinearLayout(context);
            rez1.Orientation = Orientation.Horizontal;
            RadioButton text = new RadioButton(context);
            text.TextSize = 14;
            text.Text = name;
            text.SetPadding(10, 10, 10, 10);
            EditText edit = new EditText(context);
            edit.SetWidth(300);
            edit.Background = context.Resources.GetDrawable(Resource.Drawable.rounded_edittext);
            LinearLayout.LayoutParams ll1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            edit.InputType = Android.Text.InputTypes.ClassNumber;
            edit.SetPadding(20,20,20,20);
            rez1.AddView(text);
            rez1.AddView(edit);
            rez.AddView(rez1);
            return rez;
        }
        public static LinearLayout CreateButtons(Context context) {
            LinearLayout rez = new LinearLayout(context);
            LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            ll.SetMargins(10,10,10,10);
            rez.Orientation = Orientation.Vertical;
            Button btnOk = new Button(context);
            btnOk.Text = context.Resources.GetString(Resource.String.OK);
            btnOk.SetPadding(10,10,10,10);
            btnOk.LayoutParameters = ll;
            btnOk.Background = context.Resources.GetDrawable(Resource.Drawable.rounded_button);
            rez.AddView(btnOk);
            Button btnCancel = new Button(context);
            btnCancel.Text = context.Resources.GetString(Resource.String.Cancel);
            btnCancel.SetPadding(10, 10, 10, 10);
            btnCancel.LayoutParameters = ll;
            btnCancel.Background = context.Resources.GetDrawable(Resource.Drawable.rounded_button);
            rez.AddView(btnCancel);
            return rez;
        }
    }
}