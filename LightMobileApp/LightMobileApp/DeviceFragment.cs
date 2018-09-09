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
    public class DeviceFragment : Fragment, ListView.IOnItemClickListener
    {

        DeviceController dControl = DeviceController.getInstance();
        public override void OnCreate(Bundle savedInstanceState)
        { 
            base.OnCreate(savedInstanceState);
        }

        public override void OnResume()
        {
            base.OnResume();

            ListView listView = this.Activity.FindViewById<ListView>(Resource.Id.devicelist);
            listView.Adapter = new DeviceScreenAdapter(this.Activity, dControl.getDevices().ToArray<BTDevice>()) ;

            listView.OnItemClickListener = this;



        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {   
            CheckedTextView ctv = (CheckedTextView)view;
            ctv.Checked = !ctv.Checked;
            dControl.setDeviceActive(dControl.getDevices()[position].id, ctv.Checked);

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
        }
    }



    class DeviceScreenAdapter : BaseAdapter<BTDevice>
    {
        BTDevice[] items;
        Activity context;
        public DeviceScreenAdapter(Activity context, BTDevice[] items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override BTDevice this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemMultipleChoice, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].name;
            view.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            return view;
        }
    }


}


