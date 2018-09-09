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
using Org.Json;
using Android.Bluetooth;
using Android.Util;
using System.Threading.Tasks;
using System.Threading;

namespace LightMobileApp
{
    class DeviceController
    {
        static DeviceController instance;
        BluetoothAdapter adapter;
        BluetoothSocket temp = null;

        List<BTDevice> deviceList = new List<BTDevice>();

        public static DeviceController getInstance()
        {
            instance = instance == null ? new DeviceController() : instance;
            return instance;  
        }

        private DeviceController()
        {
            adapter = BluetoothAdapter.DefaultAdapter;

            if (adapter != null && adapter.IsEnabled)
            {
                ICollection<BluetoothDevice> bluetoothDevices = adapter.BondedDevices;
                foreach (BluetoothDevice bt in bluetoothDevices)
                {
                    if (bt.BondState == Bond.Bonded) {
                        deviceList.Add(new BTDevice(bt.Address, bt.Name, bt.Name.Contains("WOW")));
                    }
                }
            }
            else
            {
                deviceList.Add(new BTDevice("1", "Test", false));
                deviceList.Add(new BTDevice("2", "Test2", false));

            }


        }

        public List<BTDevice> getDevices()
        {
            return deviceList;
        }

        public void setDeviceActive(String id,bool state)
        {
            foreach(BTDevice d in deviceList)
            {
                if(d.id == id)
                {
                    d.active = state;
                }
            }

        }


        private Object thisLock = new Object();

        public void sendMessage(String msg)
        {
            ICollection<BluetoothDevice> bluetoothDevices = adapter.BondedDevices;

            if (Monitor.TryEnter(thisLock, new TimeSpan(0, 0, 0,0,50)))
            {
                foreach (BTDevice d in deviceList)
                {
                
                    

                    if (d.active)
                    {
                        foreach (BluetoothDevice btd in bluetoothDevices)
                        {
                            if (btd.Address == d.id)
                            {

                                if(temp == null)
                                {
                                    temp = btd.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                                }
                                BluetoothSocket socket = temp;

                                try
                                {
                                    if(!socket.IsConnected)
                                        socket.Connect();
                                }
                                catch (Exception e)
                                {

                                }


                                if (socket.IsConnected)
                                {
                                    Log.Info("BT MSG", "To: " + d.name + " MSG: " + msg);
                                    //await socket.ConnectAsync();
                                    byte[] buffer = Encoding.ASCII.GetBytes(msg);
                                    socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                                    Thread.Sleep(100);

                                }
                            }
                        }
                    }
                    

                }
            }
                
        }
        
        internal void sendMessage(JSONObject msg)
        {
            this.sendMessage(msg.ToString() + "\n");
        }
    }
}