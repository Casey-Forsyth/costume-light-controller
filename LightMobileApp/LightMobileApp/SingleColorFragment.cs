using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Org.Json;
using System.Threading;

namespace LightMobileApp
{
    
    public class SingleColorFragment : Fragment, SeekBar.IOnSeekBarChangeListener
    {
        SeekBar rBar = null;
        SeekBar gBar = null;
        SeekBar bBar = null;
        SeekBar wBar = null;
        DeviceController dc = DeviceController.getInstance();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            

        }

        public override void OnResume()
        {
            base.OnResume();
            rBar = this.Activity.FindViewById<SeekBar>(Resource.Id.red_seek_button);
            gBar = this.Activity.FindViewById<SeekBar>(Resource.Id.green_seek_button);
            bBar = this.Activity.FindViewById<SeekBar>(Resource.Id.blue_seek_button);
            wBar = this.Activity.FindViewById<SeekBar>(Resource.Id.white_seek_button);

            SeekBar[] views = { rBar, gBar, bBar, wBar };
            Color[] colors = {Color.Red, Color.Green, Color.Blue, Color.Black};

            for(int i = 0; i < colors.Length; i++)
            {

                views[i].ProgressDrawable.SetColorFilter(colors[i], PorterDuff.Mode.SrcIn);
                views[i].Thumb.SetColorFilter(colors[i], PorterDuff.Mode.SrcIn);

                views[i].SetOnSeekBarChangeListener(this);
            }

        }

        public static SingleColorFragment NewInstance()
        {
            var frag1 = new SingleColorFragment { Arguments = new Bundle() };
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_single_color, container, false);
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            GatherPosDataAndSend();

        }

        private int percentToDeviceRange(int per)
        {
            return per * 255 / 100;
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            GatherPosDataAndSend();

        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            GatherPosDataAndSend();
        }


        private void GatherPosDataAndSend()
        {
            JSONObject msg = new JSONObject();

            msg.Put("modeType", "setColor");


            JSONObject data = new JSONObject();
            data.Put("r", percentToDeviceRange(rBar.Progress));
            data.Put("b", percentToDeviceRange(bBar.Progress));
            data.Put("g", percentToDeviceRange(gBar.Progress));
            data.Put("w", percentToDeviceRange(wBar.Progress));


            msg.Put("modeData", data);
            Log.Info("Single Color", "On Progress Change : " + msg.ToString());

            Thread thread = new Thread(() =>
            {
                dc.sendMessage(msg);
            });
            thread.Start();
            
        }
    }
}