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

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            return loadFragment(item.ItemId);
        }

        private bool loadFragment(int id)
        {
            Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.navigation_devices:
                    fragment = DeviceFragment.NewInstance();
                    break;
                case Resource.Id.navigation_single_color:
                    fragment = SingleColorFragment.NewInstance();
                    break;
                case Resource.Id.navigation_glow:
                    fragment = GlowColorFragment.NewInstance();
                    break;
                case Resource.Id.navigation_notifications:
                    fragment = NotificationsFragment.NewInstance();
                    break;
                default:
                    return false;
            }

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.content_frame, fragment);
            fragmentTx.Commit();


            return true;

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }
    }
}

