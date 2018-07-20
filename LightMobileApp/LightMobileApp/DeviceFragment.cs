using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace LightMobileApp
{
    public class DeviceFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static DeviceFragment NewInstance()
        {
            var frag1 = new DeviceFragment { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate( Resource.Layout.fragment_devices , container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}