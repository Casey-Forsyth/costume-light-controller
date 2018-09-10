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
    public class GlowColorFragment : Fragment, SeekBar.IOnSeekBarChangeListener
    {
        public static GlowColorFragment NewInstance()
        {
            var frag1 = new GlowColorFragment { Arguments = new Bundle() };
            return frag1;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_color_glow, container, false);

        }


        SeekBar ruBar = null;
        SeekBar guBar = null;
        SeekBar buBar = null;
        SeekBar wuBar = null;


        SeekBar rdBar = null;
        SeekBar gdBar = null;
        SeekBar bdBar = null;
        SeekBar wdBar = null;

        SeekBar pBar = null;
        TextView pText = null;

        Button butSend = null;

        DeviceController dc = DeviceController.getInstance();

        const int MIN_PERIOD = 1000;
        const int MAX_PERIOD = 15000;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here


        }

        public override void OnResume()
        {
            base.OnResume();
            ruBar = this.Activity.FindViewById<SeekBar>(Resource.Id.redup_seek_button);
            guBar = this.Activity.FindViewById<SeekBar>(Resource.Id.greenup_seek_button);
            buBar = this.Activity.FindViewById<SeekBar>(Resource.Id.blueup_seek_button);
            wuBar = this.Activity.FindViewById<SeekBar>(Resource.Id.whiteup_seek_button);

            rdBar = this.Activity.FindViewById<SeekBar>(Resource.Id.reddw_seek_button);
            gdBar = this.Activity.FindViewById<SeekBar>(Resource.Id.greendw_seek_button);
            bdBar = this.Activity.FindViewById<SeekBar>(Resource.Id.bluedw_seek_button);
            wdBar = this.Activity.FindViewById<SeekBar>(Resource.Id.whitedw_seek_button);

            pBar = this.Activity.FindViewById<SeekBar>(Resource.Id.period_seek_button);
            pText = this.Activity.FindViewById<TextView>(Resource.Id.textview_period);

            SeekBar[] views = { ruBar, guBar, buBar, wuBar , rdBar, gdBar, bdBar, wdBar, pBar };
            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Blue, Color.Black,Color.Black };

            for (int i = 0; i < colors.Length; i++)
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


        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            
            
            if(seekBar == pBar)
            {

                pText.Text = "Period - " + percentToPeriod(progress)/1000 + "s";
            }
        }

        private int percentToDeviceRange(int per)
        {
            return per * 255 / 100;
        }

        private int percentToPeriod(int per)
        {
            return (int) ((MIN_PERIOD) + (MAX_PERIOD - MIN_PERIOD) * (per / 100.0));
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            //GatherPosDataAndSend();

        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            GatherPosDataAndSend();
        }


        private void GatherPosDataAndSend()
        {

            int rd = percentToDeviceRange(rdBar.Progress);
            int gd = percentToDeviceRange(gdBar.Progress);
            int bd = percentToDeviceRange(bdBar.Progress);
            int wd = percentToDeviceRange(wdBar.Progress);

            int ru = percentToDeviceRange(ruBar.Progress);
            int gu = percentToDeviceRange(guBar.Progress);
            int bu = percentToDeviceRange(buBar.Progress);
            int wu = percentToDeviceRange(wuBar.Progress);

            int p = percentToPeriod(pBar.Progress)/100;

            String msg = $"G{p:000}{rd:000}{gd:000}{bd:000}{wd:000}{ru:000}{gu:000}{bu:000}{wu:000}\n";


            Thread thread = new Thread(() =>
            {
                dc.sendMessage(msg);
            });
            thread.Start();

        }

        void addColorToJSON(JSONObject obj, String pre, SeekBar s1, SeekBar s2)
        {
            int min = percentToDeviceRange(s1.Progress);
            int max = percentToDeviceRange(s2.Progress);

            if(min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }
            obj.Put(pre+"u", max);
            obj.Put(pre + "d", min);
        }


    }
}