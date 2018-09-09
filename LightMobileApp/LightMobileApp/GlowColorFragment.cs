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
        DeviceController dc = DeviceController.getInstance();

        const int MAX_PERIOD = 4000;

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

            SeekBar[] views = { ruBar, guBar, buBar, wuBar , rdBar, gdBar, bdBar, wdBar };
            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Red, Color.Green, Color.Blue, Color.Black };

            for (int i = 0; i < colors.Length; i++)
            {

                views[i].ProgressDrawable.SetColorFilter(colors[i], PorterDuff.Mode.SrcIn);
                views[i].Thumb.SetColorFilter(colors[i], PorterDuff.Mode.SrcIn);

                views[i].SetOnSeekBarChangeListener(this);
            }

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

            msg.Put("modeType", "glowUpAndDown");


            JSONObject data = new JSONObject();


            addColorToJSON(data, "r", ruBar, rdBar);
            addColorToJSON(data, "g", guBar, gdBar);
            addColorToJSON(data, "b", buBar, bdBar);
            addColorToJSON(data, "w", wuBar, wdBar);
            data.Put("p", (int)(pBar.Progress * MAX_PERIOD));


            msg.Put("modeData", data);
            Log.Info("Single Color", "On Progress Change : " + msg.ToString());

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
            obj.Put(pre+"u", percentToDeviceRange(max));
            obj.Put(pre + "d", percentToDeviceRange(min));
        }


    }
}