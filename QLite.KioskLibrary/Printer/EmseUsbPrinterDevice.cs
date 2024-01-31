using HidSharp;
using Quavis.QorchLite.Common;
using Quavis.QorchLite.Hwlib.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Quavis.QorchLite.Hwlib.Printer
{
    public class EmseUsbPrinterDevice: RealDeviceBase
    {
        string vendorIdStr;
        string productIdStr;
        string DeviceSearchStr = null;

        HidDevice Dev;
        HidStream DevStream;

        /// <summary>
        /// Initializes device and get connection
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>return true if device get connected</returns>
        public bool Initialize(string vid, string pid)
        {
            vendorIdStr = vid;
            productIdStr = pid;
            if (vendorIdStr.Length == 6)
                vendorIdStr = vendorIdStr.Substring(2);
            if (productIdStr.Length == 6)
                productIdStr = productIdStr.Substring(2);
            DeviceSearchStr = $"VID_{vendorIdStr}&PID_{productIdStr}";


            VID = int.Parse(vendorIdStr, System.Globalization.NumberStyles.HexNumber);
            PID = int.Parse(productIdStr, System.Globalization.NumberStyles.HexNumber);

            DeviceList.Local.TryGetHidDevice(out Dev, VID, PID);
            return Dev != null;
        }


        protected override void RaalDeviceConnected(bool? connected)
        {
            if (connected == true)
            {
                if(Dev == null)
                    DeviceList.Local.TryGetHidDevice(out Dev, VID, PID);

            }
            else if (connected == false)
            {
                LoggerAdapter.Warning($"print disconnected {vendorIdStr} {productIdStr}");
                
                DevStream?.Dispose();
                DevStream = null;
                Dev = null;
            }
        }

        #region util
        private List<bool> ByteTobits(byte value)
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < 8; i++)
            {
                bits.Add(Convert.ToBoolean(value % 2));
                value = Convert.ToByte(value / 2);
            }

            return bits;
        }

        private List<bool> byteTobits(byte value)
        {
            List<bool> bits = new List<bool>();
            int i = 0;

            while (value != 0)
            {
                bits.Add(Convert.ToBoolean(value % 2));
                value = Convert.ToByte(value / 2);

                i++;
            }

            while (bits.Count < 8)
            {
                bits.Add(false);
            }

            return bits;
        }

        private byte bitsTobyte(bool[] bits)
        {
            byte output = 0x00;

            for (int i = 0; i < bits.Length; i++)
            {
                output += Convert.ToByte(Convert.ToByte(bits[i]) << i);
            }

            return output;

        }

        #endregion


        private static object lo = new object();

        private void OpenStream()
        {
            if (Dev == null)
                throw new Exception($"EmseUsbPrinterDevice not found, vid:{ vendorIdStr } pid: { productIdStr}");

            if (DevStream == null)
            {                
                if (!Dev.TryOpen(out DevStream))
                {
                    throw new Exception($"Cannot Open EmseUsbPrinterDevice vid:{ vendorIdStr } pid: { productIdStr}");                                        
                }
            }
        }
        public byte[] WriteAndWaitResponse(byte[] data)
        {
            
            lock (lo)
            {
                byte[] result;               

                OpenStream();
                try
                {
                    DevStream.Write(data);
                    Thread.Sleep(100);
                    result = DevStream.Read();

                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("error while writing to USB EmseUsbPrinterDevice-" + DeviceSearchStr, ex);
                }
            }
        }

        public bool Write(byte[] data)
        {

            lock (lo)
            {

                byte[] result;               

                OpenStream();


                try
                {
                    DevStream.Write(data);
                    return true;
                }
                catch (Exception ex)
                {
                    LoggerAdapter.Error(ex, "");
                    return false;
                }

            }
        }

        //private const int MaxPacketLength = 50;
        //private static volatile bool _busy = false;
        //private static volatile object dataSending = new object();

        public override string Name
        {
            get
            {
                return DeviceSearchStr;
            }
        }

        public override string DisplayName => Dev?.GetFriendlyName() ?? DeviceSearchStr;


        public string Port => string.Empty;

        public object CustomStateData => this.Connected == true ? this.errorCodeList : null;

        public EmseUsbPrinterDevice()
        {
        }


        public Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>() { { "Name", Name },
                { "VendorId", vendorIdStr },
                { "ProductId", productIdStr }
            };
        }

        public bool Write(string data)
        {
            throw new NotImplementedException();
        }

        //public bool IsItYou(object settings)
        //{
        //    var sett = settings as UsbDeviceSettings;
        //    if (sett == null)
        //        return false;

        //    return sett.ProductId.Contains(this.productIdStr) && sett.VendorId.Contains(this.vendorIdStr);
        //}

        public void ReadPrinterStatus()
        {
            List<ErrorCodes> codes = new List<ErrorCodes>();
            var resp = WriteAndWaitResponse(new byte[] { 0x06, 0x01, 0x00 });
            if (resp == null)
                return;

            List<bool> m_errorbits = ByteTobits(resp[1]);
            for (int i = 0; i < m_errorbits.Count; i++)
            {
                if (m_errorbits[i])
                {
                    codes.Add((ErrorCodes)(i + 1));
                }
            }
            SetStatusCodes(codes);
        }
        public List<ErrorCodes> errorCodeList = new List<ErrorCodes>();
        private void SetStatusCodes(List<ErrorCodes> newCodes)
        {
            string newC = string.Join(",", newCodes);
            string oldC = string.Join(",", errorCodeList);
            if (newC != oldC)
            {
                errorCodeList.Clear();
                errorCodeList.AddRange(newCodes);
                OnPrinterErrorEvent(errorCodeList);
            }
        }

        public event EventHandler<List<ErrorCodes>> PrinterErrorEvent;
        protected void OnPrinterErrorEvent(List<ErrorCodes> errCodes)
        {
            PrinterErrorEvent?.Invoke(this, errCodes);
        }

        public byte[] Read()
        {
            throw new NotImplementedException();
        }

    }
}
