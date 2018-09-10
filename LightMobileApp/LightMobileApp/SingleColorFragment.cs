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

        Button butSend = null;


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

            butSend = this.Activity.FindViewById<Button>(Resource.Id.buttonSend);
            butSend.Click += delegate {
                GatherPosDataAndSend();
            };

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

        }

        private int percentToDeviceRange(int per)
        {
            return per * 255 / 100;
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {

        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {

        }


        private void GatherPosDataAndSend()
        {
            int r = percentToDeviceRange(rBar.Progress);
            int g = percentToDeviceRange(gBar.Progress);
            int b = percentToDeviceRange(bBar.Progress);
            int w = percentToDeviceRange(wBar.Progress);

            String msg = $"s{r:000}{g:000}{b:000}{w:000}\n";
          
            Thread thread = new Thread(() =>
            {
                dc.sendMessage(msg);
            });
            thread.Start();

        }
    }
}