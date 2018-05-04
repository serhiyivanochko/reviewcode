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

namespace DayMax.Models
{
    public static class GlobalVariables
    {
        public static int user = 1;
        public static int current_user = 1;
        public static int current_dir = 1;
        public static string dbPath = "Data Source=" + FileAccessHelper.GetLocalFilePath("daymax.db");
        public static int CurrentViewcashFlow = -1;
        public static int CurrentViewEarning = -1;
        public static int CurrentViewExpense = -1;
        public static int CurrentViewPurse = -1;
        public static int CurrentViewAboutPurse = -1;
        public static int CurrentViewLoan = -1;
        public static int CurrentViewLoanHistory = -1;
        public static int CurrentViewAsset = -1;
        public static string CurrentPurseName = "";
        public static string CurrentTitleName = "";
        public static int CurrentViewAssetHistory = -1;
        public static int CurrentTabTypeWrite = 2;
        public static int CurrentTabView = 0;
        public static int SelectionSpinner = -1;
        public static int idPurseToInsert = -1;
        public static string CurrentViewCashFlowAboutItem = "";
        public static int CurrentInsertTransfer = -1;
        public static int CurrentPurseToTransfer = -1;
        public static int idPurseIns = -1;
        public static int CurrentViewTabLoan = 0;
        public static int GiveTakeLoanIndex = -1;
        public static int CurrentIdCachFlow = -1;
        public static int PreviousActivity = -1;
        public static string PreviousTitleActivity = "";
        public static string Where = "";
        public static bool StateActivity = false;
        public static List<Dictionary<string, string>> insert;
        public static List<Asset> insertass;
        public static KeyValuePair<int, Dictionary<string, string>> insertpurse;
        public static string CurrentShortName = "";
    }
}