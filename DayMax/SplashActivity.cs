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
using Android.Util;
using System.Threading.Tasks;
using System.Threading;

namespace DayMax
{
    [Activity(Theme = "@style/Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            new Thread(new ThreadStart(() =>
            {
                this.RunOnUiThread(() =>
                {
                  StartActivity(typeof(MainActivity));
                  this.Finish();
                });
            })).Start();
        }
    }
}