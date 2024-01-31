using HidSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using Quavis.QorchLite.Common;

namespace Quavis.QorchLite.Hwlib.Hardware
{
    public class EmseUsbQueNumDevice : RealDeviceBase
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
                if (Dev == null)
                    DeviceList.Local.TryGetHidDevice(out Dev, VID, PID);
            }
            else if (connected == false)
            {
                LoggerAdapter.Warning($"QueNumDevice disconnected {vendorIdStr} {productIdStr}");
                DevStream?.Dispose();
                DevStream = null;
                Dev = null;
            }

        }

        private void OpenStream()
        {
            if (Dev == null)
                throw new Exception($"EmseUsbQueNumDevice not found, vid:{ vendorIdStr } pid: { productIdStr}");

            if (DevStream == null)
            {
                if (!Dev.TryOpen(out DevStream))
                {
                    throw new Exception($"Cannot Open EmseUsbQueNumDevice vid:{ vendorIdStr } pid: { productIdStr}");
                }
                DevStream.ReadTimeout = 100;
            }
        }

        public bool Write(byte[] data)
        {
            lock (lo)
            {
                try
                {
                    int packetSize = 30;
                    OpenStream();
                    for (int i = 1; i <= (data.Length / packetSize) + 1; i++)
                    {
                        var outputBuffer = new List<byte>();
                        outputBuffer.Add(Convert.ToByte(2));
                        //outputBuffer.Add(Convert.ToByte(data.Length));
                        //outputBuffer.AddRange(data);
                        int length = data.Length / (i * packetSize) > 0 ? packetSize : data.Length % packetSize;
                        outputBuffer.Add(Convert.ToByte(length));
                        for (int j = 0; j < length; j++)
                        {
                            outputBuffer.Add(data[(i - 1) * packetSize + j]);
                        }

                        DevStream.Write(outputBuffer.ToArray(), 0, outputBuffer.Count);
                        Thread.Sleep(2 * outputBuffer.Count + 10);

                    }

                    //LoggerAdapter.Debug($"{data.Length} writing DONE: {DateTime.Now.ToString("HH:mm:ss:ffff")}");

                    return true;
                }
                catch (Exception ex)
                {
                    LoggerAdapter.Error(ex);
                    return false;
                }

            }
        }

        private static object lo = new object();

        private const int MaxPacketLength = 50;

        public override string Name
        {
            get
            {
                return DeviceSearchStr;
            }
        }

        public override string DisplayName => Dev?.GetFriendlyName() ?? DeviceSearchStr;



        public byte[] WriteAndWaitResponse(byte[] data, CancellationToken ct, int timeout = 0)
        {
            if (ct.IsCancellationRequested)
                return null;

            lock (lo)
            {
                List<byte> result = new List<byte>();
                byte[] res;
                bool timeoutExpired = false;
                bool readCompleted = false;
                OpenStream();
                try
                {
                    var outputBuffer = new List<byte>();
                    outputBuffer.Add(Convert.ToByte(2));
                    outputBuffer.Add(Convert.ToByte(data.Length));
                    outputBuffer.AddRange(data);

                    if (ct.IsCancellationRequested)
                        return null;

                    DevStream.Write(outputBuffer.ToArray(), 0, outputBuffer.Count);
                    DateTime start = DateTime.Now;
                    Thread.Sleep(2 * data.Length + 10);
                    while (!timeoutExpired && !readCompleted)
                    {
                        if (ct.IsCancellationRequested)
                            break;

                        res = DevStream.Read();
                        readCompleted = ParseInterfaceData(res, ref result);

                        if ((DateTime.Now - start).TotalMilliseconds > timeout)
                            timeoutExpired = true;
                        Thread.Sleep(10);
                    }
                    return result.ToArray();
                }
                catch (Exception ex)
                {
                    //throw new RealDeviceException("error while writing to USB", "EmseUsbQNumDevice-" + DeviceSearchStr, ex);
                    LoggerAdapter.Warning("error while writing to USB EmseUsbQNumDevice-" + DeviceSearchStr + ex.Message);
                }
                return result.ToArray();
            }
        }

        private bool ParseInterfaceData(byte[] data, ref List<byte> result)
        {
            if (data[0] == 0x01)
            {
                for (int i = 0; i < Convert.ToInt32(data[1]); i++)
                {
                    byte CurrentByte = data[i + 2];

                    if (CurrentByte == 0x00)
                    {

                    }
                    else if (CurrentByte != 0x00)
                    {
                        result.Add(CurrentByte);
                    }

                    int Count = result.Count;

                    if (Count > 2 && ((result[Count - 2] == 10 && result[Count - 1] == 13) || (result[Count - 2] == 13 && result[Count - 1] == 10))) // Fix for old APS printers (EndofProtocol is "\r\n" {13,10})
                    {
                        return true;
                    }
                }
            }

            return false;

        }


    }
}
