using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QLite.Data.Dtos;
using QLite.Kio;
using QLite.KioskLibrary.Hardware;
using Quavis.QorchLite.Hwlib.Display;
using Quavis.QorchLite.Hwlib.Hardware;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.KioskLibrary.Display
{
    public class EmseDisplayGHid : IDisplay
    {
        public string Name { get; } = "EmseDisplayGHid";

        private string m_AcceptedChars0 = "0123456789AbCdEFhJLPnortUuY";
        private string m_AcceptedChars12 = "0123456789";
        private byte[] arrowDirection = new byte[5] { 82, 76, 68, 85, 78 };
        List<MainQueNum> MainQueNumSettings = new List<MainQueNum>();
        Dictionary<string, List<DisplayRow>> MainQueNumRows = new Dictionary<string, List<DisplayRow>>();
        VCQueNumSettings Settings = new VCQueNumSettings();

        IConfiguration _config;
        public EmseUsbQueNumDevice Device { get; private set; }
        RealDeviceBase IDisplay.Device => Device;

        public EmseDisplayGHid(IConfiguration config, EmseUsbQueNumDevice dev)
        {
            _config = config;
            Device = dev;
        }

       

        public Type SettingsType => typeof(VCQueNumSettings);

        public void Initialize(object settings)
        {
            var dispSettings = settings as VCQueNumSettings;

            Settings = settings as VCQueNumSettings;

            Config();

            Log.Debug($"{Name} initializing:" + JsonConvert.SerializeObject(dispSettings));
            if (Device.Initialize(dispSettings.VendorId, dispSettings.ProductId))
            {
                Log.Debug($"{Name}  initialized");
            }
        }

        void Config()
        {
            MainQueNumSettings = Settings.MainQueNums;

            foreach (var main in MainQueNumSettings)
            {
                var firstFilled = !string.IsNullOrEmpty(main.QueNums[0].MainQueNumId);
                var rows = new List<DisplayRow>();
                if (main?.DisplayRows != null)
                {
                    foreach (var item in main.DisplayRows)
                    {
                        rows.Add(new DisplayRow { Id = item.Id, Text = item.Id + "|" + item.Text });
                    }
                    MainQueNumRows.Add(main.Name, rows);

                    foreach (var queNum in main.QueNums)
                    {
                        var filled = !string.IsNullOrEmpty(queNum.MainQueNumId);
                        if (filled != firstFilled)
                        {
                            Log.Error($"Main:{main.Name} queue numarator relations not valid.");
                            //TODO:Devam etmeli mi
                        }
                    }
                }
            }
        }

        public HwStatusDto GetHwStatus()
        {
            var connected = Device.Connected != false;
            var notok = !connected;
            return new HwStatusDto
            {
                Device = QLDevice.Display,
                Connected = connected,
                Ok = !notok
            };

        }

        public void Send(QueNumData qnumDataObject)
        {
            var dispNo = qnumDataObject.DisplayNo;
            var qnum = qnumDataObject.TicketNo;
            var toMain = qnumDataObject.SendToMain;

            int.TryParse(dispNo, out int dNo);
            if (Display(dNo, dNo, qnum) && toMain)
                SendMainDisplay(dNo, qnum);
        }

        private bool Display(int displayno, int bankoNo, string ticketno)
        {
            List<byte> DisplayPackage = new List<byte>();
            byte[] ticketno_ary = CheckDigits(ticketno);
            if (ticketno_ary == null)
                return false;
            DisplayPackage.Add(Convert.ToByte(48 + displayno));//Display Id(Gösterge=48+id,Ana gösterge için 159+id)
            DisplayPackage.Add(Convert.ToByte(71));//G sabit
            DisplayPackage.Add(Convert.ToByte(48 + bankoNo));//bankoNo
            DisplayPackage.Add(ticketno_ary[0]);//Digit 100 ler
            DisplayPackage.Add(ticketno_ary[1]);//Digit 10lar
            DisplayPackage.Add(ticketno_ary[2]);//Digit 1ler
            DisplayPackage.Add(Convert.ToByte(82));//Ok yönü =>
            DisplayPackage.Add(Convert.ToByte(10));//Paket sonu
            DisplayPackage.Add(Convert.ToByte(13));//Paket sonu

            return Device.Write(DisplayPackage.ToArray());
        }

        private byte[] CheckDigits(string ticketno)
        {
            if (ticketno.Length > 3)
            {
                ticketno = ticketno.Substring(0, 3);
            }

            ticketno = string.Format("{0,3}", ticketno);
            ticketno = ticketno.Replace(" ", "0");

            byte[] output = new byte[3];
            List<string> AcceptedChars = new List<string>(3) { m_AcceptedChars0, m_AcceptedChars12, m_AcceptedChars12 };
            char[] digits = ticketno.ToCharArray();

            for (int i = 0; i < digits.Length; i++)
            {
                if (AcceptedChars[i].IndexOf(digits[i]) != -1)
                {
                    output[i] = Convert.ToByte(digits[i]);
                }
                else
                {
                    //Hatalı formatta bilet numarası girişinde göstergelere bilgi gönderilmez
                    return null;
                }

            }

            return output;
        }

        private List<string> SendMainDisplay(int dNo, string qnum)
        {
            List<string> list = new List<string>();
            foreach (var mainQueNum in MainQueNumSettings)
            {
                if (mainQueNum.IsDedicated)
                {
                    var mainIdList = mainQueNum.QueNums.Where(x => x.DisplayRow.Id == dNo && !string.IsNullOrEmpty(x.MainQueNumId)).Select(x => x.MainQueNumId).ToList();
                    if (mainIdList.Count != 0)
                    {
                        foreach (var mainId in mainIdList.Where(x => !list.Contains(x)))
                        {
                            int.TryParse(mainId, out int mainNo);
                            Display(mainNo, dNo, qnum);
                        }
                    }
                }
                else
                {
                    if (mainQueNum.QueNums.Any(x => x.DisplayRow.Id == dNo))
                    {
                        Log.Debug("Main:");
                        var rows = MainQueNumRows[mainQueNum.Name];
                        string temp = "", value = dNo.ToString() + "|" + qnum;
                        foreach (var item in rows.OrderBy(d => d.Id))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                temp = item.Text;
                                item.Text = value;
                                //int.TryParse(item.Id, out int displayno);
                                var dq = value.Split("|");
                                int.TryParse(dq[0], out int bankoNo);
                                Display(item.Id + 111, bankoNo, dq[1]);
                                Log.Debug("dis:" + item.Id + " banko:" + bankoNo + " num:" + dq[1]);
                                value = temp;
                                Thread.Sleep(20);
                            }
                        }
                    }
                }
            }
            return list;
        }




    }

}
