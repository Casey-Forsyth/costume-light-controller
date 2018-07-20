using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace LightMobileApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private TextView _textMessage;

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_devices:
                    _textMessage.SetText(Resource.String.title_notifications);
                    return true;
                case Resource.Id.navigation_single_color:
                    _textMessage.SetText(Resource.String.title_single_color);
                    return true;
                case Resource.Id.navigation_glow:
                    _textMessage.SetText(Resource.String.title_glowing);
                    return true;
                case Resource.Id.navigation_notifications:
                    _textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }
    }
}

