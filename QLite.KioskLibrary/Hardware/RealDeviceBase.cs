using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quavis.QorchLite.Hwlib.Hardware
{
    public class RealDeviceBase
    {

        public string Port { get; set; }

        public event EventHandler<bool?> DeviceConnectionEvent;

        public bool? Connected = false; 
        public virtual string CurrentStateInfo => Connected == null ? "-" : (Connected.Value ? "Connected" : "Disconnected");
        public void OnDeviceConnectionEvent(bool? connected)
        {
            if (connected == Connected)
            {
                return;
            }
            

            Connected = connected;

            RaalDeviceConnected(connected); 
            DeviceConnectionEvent?.Invoke(this, connected);
        }

        protected virtual void RaalDeviceConnected(bool? connected)
        {

        }

        protected ConcurrentDictionary<string, Action<object, object>> Listeners = new ConcurrentDictionary<string, Action<object, object>>();

        //Birileri canlı data dinlemek istiyor. Bu method override eden canlı data sağlamak için ne gerekiyorsa yapsın.
        //Örneğin seriport kapalıysa açsın ve receive event'ine register olsun. 
        protected virtual void OnListenersRegistered()
        {
        }

        public virtual bool StartListeningDeviceData(Action<object, object> callBack, string subComp = null)
        {
            
            OnListenersRegistered();

            if (string.IsNullOrEmpty(subComp))
            {
                if (Listeners.ContainsKey("-"))
                {
                    Listeners["-"] += callBack;
                }
                else
                {
                    Listeners.TryAdd("-", callBack);
                }
            }
            else
            {
                if (Listeners.ContainsKey(subComp))
                {
                    Listeners[subComp] += callBack;
                }
                else
                {
                    Listeners.TryAdd(subComp, callBack);
                }
            }
            return true;
        }

        public virtual void StopListeningDeviceData(Action<object, object> callBack, string subComp = null)
        {
            if (string.IsNullOrEmpty(subComp))
            {
                if (Listeners.ContainsKey("-"))
                {
                    Listeners["-"] -= callBack;
                }
                else
                {
                    Listeners.TryRemove("-", out callBack);
                }
            }
            else
            {
                if (Listeners.ContainsKey(subComp))
                {
                    Listeners[subComp] -= callBack;
                }
                else
                {
                    Listeners.TryRemove(subComp, out callBack);
                }
            }
        }

        protected void PublishDeviceData(object data, string subComp = null)
        {
            Action<object, object> cb = null;

            if (string.IsNullOrEmpty(subComp))
                Listeners.TryGetValue("-", out cb);
            else
                Listeners.TryGetValue(subComp, out cb);

            cb?.Invoke(this, data);
        }

        public T ParseEnum<T>(JObject settings, string enumStr) where T : struct
        {
            return (T)Enum.Parse(typeof(T), settings[enumStr].Value<string>());
        }

        

        public virtual bool Close()
        {
            return false;
        }

       

        public virtual string Name => ""; //OsLevelUniq
        public virtual string DisplayName => "";

        public int VID { get; internal set; }
        public int PID { get; internal set; }

        public virtual Dictionary<string, string> GetSettings()
        {
            return null;
        }


    }
}
