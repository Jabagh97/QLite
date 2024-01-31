namespace Quavis.QorchLite.Hwlib.Display
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Globalization;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Quavis.QorchLite.Common;
    using Quavis.QorchLite.Hwlib.Hardware;
    using Quavis.QorchLite.Data.Dto;
    using Newtonsoft.Json;

    public class EmseDisplayHDot: IDisplay
    {
        public string Name { get; } = "EmseDisplayHDot";

        private byte[] TicketNoDigits;
        private byte[] TerminalNoDigits;
        private int standardBlank = 1;
        private int serviceCodeBlank = 1;

        private string m_AcceptedChars0 = "0123456789AbCdEFhJLPnortUuY";
        private string m_AcceptedChars12 = "0123456789";

        private byte[] arrowDirection = new byte[5] { 82, 76, 68, 85, 78 };
        public readonly static List<byte> EndOfProtocol = new List<byte>() { 10, 13 };

        List<MainQueNum> MainQueNumList = new List<MainQueNum>();
        Dictionary<string, List<DisplayRow>> MainQueNumRows = new Dictionary<string, List<DisplayRow>>();
        VCQueNumSettings Settings = new VCQueNumSettings();

        /// <summary>
        /// 1254 => windows-1254 Turkish (Windows) => okunan byteların char' a dönüştürülmesinde 144 den sonra sorun oluyor 
        /// 28599 => iso-8859-9 Turkish (ISO)
        /// </summary>
        public Encoding DefaultProtocolEncoding { get; set; }
        public ProtocolHFont Font { get; set; }
        public bool ServiceCodeIsShowing { get; set; } = true;
        public bool TerminalNoIsShowing { get; set; } = true;
        public DisplayFunction DisplayType { get; set; } = DisplayFunction.CounterDisplay;
        public DisplayArrowDirection CurrentDirection => DisplayType == DisplayFunction.CounterDisplay ? SelfDirection : MainDirection;
        public DisplayArrowDirection MainDirection { get; set; }
        public DisplayArrowDirection SelfDirection { get; set; }
        public int CurrentDotWidth { get; set; }
        public int CurrentDotHeight { get; set; }



        IConfiguration _config;
        public EmseUsbQueNumDevice Device;

        RealDeviceBase IDisplay.Device => Device;

        public Type SettingsType => typeof(VCQueNumSettings);

        public EmseDisplayHDot(IConfiguration config, EmseUsbQueNumDevice dev)
        {
            _config = config;
            Device = dev;
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

       

        public void Initialize(object settings)
        {
            SetDisplay();
            Settings = settings as VCQueNumSettings;
            
            LoggerAdapter.Debug($"{Name} initializing:" + JsonConvert.SerializeObject(Settings));

            Config();

            if (Device.Initialize(Settings.VendorId, Settings.ProductId))
            {
                LoggerAdapter.Debug($"{Name}  initialized");
                SendInitialMessage();
            }


            Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;
        }

        private void Device_DeviceConnectionEvent(object sender, bool? e)
        {
            SendInitialMessage();
        }

        private void SetDisplay()
        {
            DefaultProtocolEncoding = ASCIIEncoding.GetEncoding(28591);
            Font = new ProtocolHFont(DisplayFontStyle.Thin, DisplayArrowStyle.Thin);
            MainDirection = DisplayArrowDirection.Right;
            SelfDirection = DisplayArrowDirection.Right;
            ServiceCodeIsShowing = true;
            TerminalNoIsShowing = true;
            DisplayType = DisplayFunction.CounterDisplay;
        }

        //public bool ValidateSend(ApplicationData data)
        //{
        //    var qnumData = data.DataRecords[0].Data;
        //    var qnumDataArr = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(qnumData);
        //    if (((Newtonsoft.Json.Linq.JContainer)qnumDataArr).Count != 2 || string.IsNullOrEmpty(qnumDataArr.DisplayNo.Value) || string.IsNullOrEmpty(qnumDataArr.TicketNo.Value))
        //        return false;
        //    return true;
        //}

        public void Send(QueNumData qnumDataObject)
        {
            LoggerAdapter.Error($"DotDisplay qnumDataObject"+qnumDataObject?.TicketNo);

            var dispNo = qnumDataObject.DisplayNo;
            var qnum = qnumDataObject.TicketNo;

            int.TryParse(dispNo, out int dNo);

            SetDisplay();
            var queNum = MainQueNumList.Select(x => x.QueNums.FirstOrDefault(x => x.DisplayRow.Id == dNo)).FirstOrDefault();
            if (queNum == null)
            {
                LoggerAdapter.Error($"Display not fount in settings {dispNo}");
            }
            DisplayType = DisplayFunction.CounterDisplay;
            SelfDirection = (DisplayArrowDirection)queNum.SelfDirection;
            MainDirection = (DisplayArrowDirection)queNum.Direction;
            CurrentDotWidth = queNum.DotWidth;
            CurrentDotHeight = queNum.DotHeight;
            if (string.IsNullOrEmpty(qnum))
                DisplaySleepModeLogo(queNum.DisplayRow.Id, queNum.DotHeight, queNum.DotWidth, GetNumberDotValues(null, queNum.DotHeight, queNum.DotWidth));//SendScrollingMessage(dNo,Settings.BreakMessage);
            else if (Display(dNo, dNo, qnum) && qnumDataObject.SendToMain)
                SendMainDisplay(dNo, qnum);

        }

        private bool Display(int displayno, int bankoNo, string ticketno)
        {
            LoggerAdapter.Debug("display :"+displayno);
            List<byte> DisplayPackage = new List<byte>();
            List<byte> PartialPackage = CheckTotalItemWidth(bankoNo, ticketno);

            LoggerAdapter.Debug("display after CheckTotalItemWidth");

            if (PartialPackage != null)
            {
                DisplayPackage.Add(Convert.ToByte(48 + displayno));
                DisplayPackage.Add(Convert.ToByte(72));
                DisplayPackage.AddRange(PartialPackage);
                DisplayPackage.AddRange(EndOfProtocol);
            }
            else
            {
                Console.WriteLine("Package preperation is failed.");
                return false;
            }

            LoggerAdapter.Debug("display writing");
            var res = Device.Write(DisplayPackage.ToArray());
            LoggerAdapter.Debug("display writing done");
            return res;
        }

        protected byte[] ToDigits(string value)
        {
            if (value == null)
            {
                return null;
            }

            char[] digits = value.ToCharArray();
            byte[] output = DefaultProtocolEncoding.GetBytes(digits);

            return output;
        }

        private byte BitsToByte(bool[] bits)
        {
            byte output = 0x00;

            for (int i = 0; i < bits.Length; i++)
            {
                output += Convert.ToByte(Convert.ToByte(bits[i]) << i);
            }

            return output;

        }

        private List<byte> CheckTotalItemWidth(int displayno, string ticketno)
        {
            bool startCalculation = true;
            SortedList<string, int> StartingPoints = new SortedList<string, int>(2)
            {
                {"TicketNoSection",0},
                {"TerminalNoSection",0}
            };
            while (startCalculation)
            {
                int RemainingDotCount = CurrentDotWidth;
                int TerminalNoSectionItemCount = 0;
                int TicketNoSectionWidth = 0;
                int TerminalNoSectionWidth = 0;
                int DisableServiceCode = 1;
                StartingPoints = new SortedList<string, int>(2)
                {
                    {"TicketNoSection",0},
                    {"TerminalNoSection",0}
                };

                TicketNoDigits = ToDigits(ticketno);

                if (TicketNoDigits.IsNullOrEmpty())
                {
                    return null;
                }

                if (CurrentDirection != DisplayArrowDirection.NoArrow && TerminalNoIsShowing)
                {
                    TerminalNoSectionWidth = TerminalNoSectionWidth + Font.FullSetArrowWidth[(int)CurrentDirection + Font.ArrowReferenceValue - 1];
                    TerminalNoSectionItemCount++;
                }

                this.TerminalNoDigits = ToDigits(displayno.ToString());
                if (TerminalNoDigits.IsNullOrEmpty())
                {
                    return null;
                }

                for (int i = 0; i < TerminalNoDigits.Length; i++)
                {
                    TerminalNoSectionWidth = TerminalNoSectionWidth + Font.TerminalNoDigitWidth[Convert.ToInt32(Convert.ToChar(TerminalNoDigits[i]).ToString())];

                    TerminalNoSectionItemCount++;
                }

                TerminalNoSectionWidth = TerminalNoSectionWidth + (TerminalNoSectionItemCount - 1) * standardBlank;


                RemainingDotCount = RemainingDotCount - TerminalNoSectionWidth;

                if (ServiceCodeIsShowing || TicketNoDigits.Length < 4)
                {
                    DisableServiceCode = 0;
                }

                //Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi
                char[] TicketNoCharArray = DefaultProtocolEncoding.GetChars(TicketNoDigits);

                for (int i = DisableServiceCode; i < TicketNoDigits.Length; i++)
                {
                    //TicketNoSectionWidth = TicketNoSectionWidth + base.Display.Font.TicketNoDigitWidth[Convert.ToChar(TicketNoDigits[i])];

                    //Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi

                    if (Font.TicketNoDigitWidth.ContainsKey(TicketNoCharArray[i]))
                    {
                        TicketNoSectionWidth = TicketNoSectionWidth + Font.TicketNoDigitWidth[TicketNoCharArray[i]];
                    }

                }

                RemainingDotCount = RemainingDotCount - TicketNoSectionWidth - (TicketNoDigits.Length - DisableServiceCode) * standardBlank;

                if (TicketNoDigits.Length > 3 && ServiceCodeIsShowing)
                {
                    RemainingDotCount = RemainingDotCount - serviceCodeBlank + 1;
                }

                if (RemainingDotCount < 0)
                {
                    Console.WriteLine("RemainingDotCount < 0");
                    if (TerminalNoIsShowing && CurrentDirection != DisplayArrowDirection.NoArrow)
                    {
                        if (DisplayType == DisplayFunction.CounterDisplay)
                            SelfDirection = DisplayArrowDirection.NoArrow;
                        else
                            MainDirection = DisplayArrowDirection.NoArrow;

                        startCalculation = true;
                    }
                    else if (TerminalNoIsShowing)
                    {
                        TerminalNoIsShowing = false;
                        startCalculation = true;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    StartingPoints["TicketNoSection"] = RemainingDotCount / 2 + 1;
                    StartingPoints["TerminalNoSection"] = CurrentDotWidth - TerminalNoSectionWidth + 1;
                    startCalculation = false;
                }
            }

            return PreparePartialPackage(StartingPoints, CurrentDirection);
        }

        private List<byte> PreparePartialPackage(SortedList<string, int> StartingPoints, DisplayArrowDirection direction)
        {
            List<byte> ItemPackage = new List<byte>();
            int Point = StartingPoints["TicketNoSection"];
            int TicketNoLength = TicketNoDigits.Length;
            int SelectedArrow;
            int DisableServiceCode = 1;

            if (TicketNoLength < 4 || ServiceCodeIsShowing)
            {
                DisableServiceCode = 0;
            }

            //Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi
            char[] TicketNoCharArray = DefaultProtocolEncoding.GetChars(TicketNoDigits);

            for (int i = DisableServiceCode; i < TicketNoLength; i++)
            {
                ItemPackage.Add(Font.TicketNoFontCode);
                ItemPackage.Add(Convert.ToByte(48 + Point));
                ItemPackage.Add(TicketNoDigits[i]);

                Point = Point + Font.TicketNoDigitWidth[Convert.ToChar(TicketNoDigits[i])];

                ////Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi
                //if (Font.TicketNoDigitWidth.ContainsKey(TicketNoCharArray[i]))
                //{
                //    Point = Point + Font.TicketNoDigitWidth[TicketNoCharArray[i]];
                //}

                if (TicketNoLength == 4 && i == 0 && ServiceCodeIsShowing)
                {
                    Point = Point + serviceCodeBlank;
                }
                else
                {
                    Point = Point + standardBlank;
                }

            }

            if (TerminalNoIsShowing)
            {
                Point = StartingPoints["TerminalNoSection"];

                if (direction != DisplayArrowDirection.NoArrow)
                {
                    SelectedArrow = (int)direction + Font.ArrowReferenceValue;
                    ItemPackage.Add(Font.ArrowFontCode);
                    ItemPackage.Add(Convert.ToByte(48 + Point));
                    ItemPackage.Add(Convert.ToByte(48 + SelectedArrow));
                    Point = Point + Font.FullSetArrowWidth[SelectedArrow - 1] + standardBlank;
                }

                for (int i = 0; i < TerminalNoDigits.Length; i++)
                {
                    ItemPackage.Add(Font.TerminalNoFontCode);
                    ItemPackage.Add(Convert.ToByte(48 + Point));
                    ItemPackage.Add(TerminalNoDigits[i]);
                    Point = Point + Font.TerminalNoDigitWidth[Convert.ToInt32(Convert.ToChar(TerminalNoDigits[i]).ToString())] + standardBlank;
                }
                Point--;
                if (Point > CurrentDotWidth + 1)
                    return null;
            }

            return ItemPackage;
        }

        private List<string> SendMainDisplay(int dNo, string qnum)
        {
            LoggerAdapter.Debug("SendMainDisplay");
            try
            {
                List<string> list = new List<string>();
                foreach (var mainQueNum in MainQueNumList)
                {
                    var queNum = mainQueNum.QueNums.FirstOrDefault(x => x.DisplayRow.Id == dNo);
                    MainDirection = (DisplayArrowDirection)queNum.Direction;
                    SelfDirection = (DisplayArrowDirection)queNum.SelfDirection;
                    DisplayType = DisplayFunction.MainDisplay;
                    CurrentDotWidth = mainQueNum.DotWidth;
                    CurrentDotHeight = mainQueNum.DotHeight;

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
                        if (queNum != null)
                        {
                            LoggerAdapter.Debug("Main:");
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
                                    LoggerAdapter.Debug("dis:" + item.Id + " banko:" + bankoNo + " num:" + dq[1]);
                                    value = temp;
                                    Thread.Sleep(20);
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error(ex);
                throw;
            }
        }


        void Config()
        {
            MainQueNumList = Settings.MainQueNums;

            foreach (var main in MainQueNumList)
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
                            LoggerAdapter.Error($"Main:{main.Name} queue numarator relations not valid.");
                            //TODO:Devam etmeli mi
                        }
                    }
                }
            }            
        }


        private void SendInitialMessage()
        {
            if (Device.Connected != true)
                return;
            foreach (var main in MainQueNumList)
            {
                foreach (var item in main.DisplayRows)
                {
                    SetDisplay(item.Id + 111);
                    if(!string.IsNullOrEmpty(item.Text))
                        SendScrollingMessage(item.Id + 111, item.Text);
                    Thread.Sleep(50);
                    //DisplaySleepModeLogo(item.Id + 111, main.DotHeight, main.DotWidth, GetNumberDotValues(item.Id, main.DotHeight, main.DotWidth));
                }
            }
            foreach (var mainQueNum in MainQueNumList)
            {
                foreach (var queNum in mainQueNum.QueNums)
                {
                    SetDisplay(queNum.DisplayRow.Id);
                    if (!string.IsNullOrEmpty(queNum.DisplayRow.Text))
                        SendScrollingMessage(queNum.DisplayRow.Id, queNum.DisplayRow.Text);
                    Thread.Sleep(50);
                    //DisplaySleepModeLogo(queNum.DisplayRow.Id, queNum.DotHeight, queNum.DotWidth, GetNumberDotValues(queNum.DisplayRow.Id, queNum.DotHeight, queNum.DotWidth));
                }
            }
        }

        private int[,] GetNumberDotValues(int? id, int dotHeight, int dotWidth)
        {
            int[,] data = new int[dotHeight, dotWidth];
            for (int i = 0; i < dotHeight; i++)
            {
                for (int j = 0; j < dotWidth; j++)
                {
                    data[i, j] = 0;
                }
            }
            if (id == null)
                return data;

            byte col_no;
            byte row_no;
            byte digit_ones, digit_tens, id_number_width, start_point, x_pos, y_pos, char_col_no;


            //// Return error, if parameters are not correct!
            if (id == 0 || id > 99) return null;
            if (dotWidth != 40 && dotWidth != 48 && dotWidth != 56) return null;

            // Get id_number digits
            digit_ones = (byte)(id % (byte)10);
            digit_tens = (byte)(id / (byte)10);
            id_number_width = ProtocolHFont.IdNumbers_Width[digit_ones];

            id_number_width += (byte)(ProtocolHFont.IdNumbers_Width[digit_tens]);
            id_number_width += (byte)(ProtocolHFont.BLANK_IdNumbers);

            // Write Tens of id number
            start_point = (byte)((byte)(dotWidth - id_number_width) / 2);
            char_col_no = 7;
            for (x_pos = start_point; x_pos < ProtocolHFont.IdNumbers_Width[digit_tens] + start_point; x_pos++)
            {

                for (y_pos = 0; y_pos < dotHeight; y_pos++)
                {
                    data[y_pos, x_pos] = (int)GetBitVal((byte)(ProtocolHFont.IdNumbers_Content[ProtocolHFont.IdNumbers_Index[(byte)digit_tens] + y_pos]), char_col_no);
                }
                char_col_no--;
            }

            // Write Ones of id number
            start_point = (byte)(start_point + ProtocolHFont.IdNumbers_Width[digit_tens] + ProtocolHFont.BLANK_IdNumbers);
            char_col_no = 7;
            for (x_pos = start_point; x_pos < ProtocolHFont.IdNumbers_Width[digit_ones] + start_point; x_pos++)
            {

                for (y_pos = 0; y_pos < dotHeight; y_pos++)
                {
                    data[y_pos, x_pos] = (int)GetBitVal((byte)(ProtocolHFont.IdNumbers_Content[ProtocolHFont.IdNumbers_Index[(byte)digit_ones] + y_pos]), char_col_no);
                }

                char_col_no--;
            }

            return data;
        }


        byte GetBitVal(byte val, byte bit_no)
        {
            val >>= bit_no;
            val &= (byte)0x01;

            if (val != 0)
                val = 75;
            else
                val = 0;
            return val;
        }



        private int SetDimmingTime(int dimmingtime)
        {
            if (dimmingtime == 1 || dimmingtime == 2)
            {
                dimmingtime = 1;
            }
            else
            {
                dimmingtime = dimmingtime / 2;
            }

            return dimmingtime;
        }

        public bool SetDisplay(int displayno)
        {
            int flashcount = this.Settings.FlashingCount - 1;
            flashcount = flashcount <= 0 ? 1 : flashcount;
            int dimmingtime = this.Settings.DimmingTime;
            List<byte> DisplayPackage = new List<byte>();

            if (flashcount >= 0 && flashcount < 100 && dimmingtime >= 0 && dimmingtime < 100)
            {
                byte[] flashcount_ary = this.ToDigits(flashcount.ToString("00"));
                byte[] dimmingtime_ary = this.ToDigits(SetDimmingTime(dimmingtime).ToString("00"));

                DisplayPackage.Add(Convert.ToByte(48 + displayno));
                DisplayPackage.Add(Convert.ToByte(119));
                DisplayPackage.Add(flashcount_ary[0]);
                DisplayPackage.Add(flashcount_ary[1]);
                DisplayPackage.Add(dimmingtime_ary[0]);
                DisplayPackage.Add(dimmingtime_ary[1]);
                DisplayPackage.AddRange(EndOfProtocol);
                return Device.Write(DisplayPackage.ToArray());
            }
            return false;
        }


        public bool SendScrollingMessage(int displayNo, string messageText)
        {
            List<byte> DisplayPackage = new List<byte>();

            //if (this.Display.MessageIdleTime == 0)
            //{
            //    this.Display.MessageText = this.Display.DisplayID.ToString(); // MessageIdleTime 0 girildiğinde kayanyazı devreden çıkarılır. Bu esnada göstergelerde ortalanmış olarak DisplayID bilgisi gösterilir.
            //}

            byte[] IdleTimeBeforeScrollingDigits = this.ToDigits(Settings.MessageIdleTime.ToString("000"));

            DisplayPackage.Add(Convert.ToByte(48 + displayNo));
            DisplayPackage.Add(Convert.ToByte(110));
            DisplayPackage.Add(IdleTimeBeforeScrollingDigits[0]);
            DisplayPackage.Add(IdleTimeBeforeScrollingDigits[1]);
            DisplayPackage.Add(IdleTimeBeforeScrollingDigits[2]);

            if (this.Settings.MessageIdleTime != 0)
            {
                DisplayPackage.Add(Convert.ToByte(' ')); // Mesaj bilgisi öncesinde Boşluk karakteri gönderilir (13.07.2011)
            }

            DisplayPackage.AddRange(DefaultProtocolEncoding.GetBytes(messageText.ToCharArray()));
            DisplayPackage.AddRange(EndOfProtocol);

            return Device.Write(DisplayPackage.ToArray());
        }

        public bool DisplaySleepModeLogo(int displayNo, int dotHeight, int dotWidth, int[,] data)
        {
            List<byte> DisplayPackage = new List<byte>();
            //int[,] data = new int[dotHeight, dotWidth];

            const int DivideLineConstant = 8;
            int[] PartOfLine;

            List<byte> PartialPackage = new List<byte>();

            //if (ID.Count == 0 || DotHeight != ColorData.GetLength(0) || DotWidth != ColorData.GetLength(1) ||
            //    this.Display.TechnicalData.ColorType == Enums.DisplayColorType.MultiColor ||
            //    this.Display.TechnicalData.DotWidth % DivideLineConstant != 0)
            //{
            //    return NewPackage;
            //}

            for (int z = 0; z < dotWidth; z = z + DivideLineConstant) //Her satırı 8nokta 8nokta 
            {
                for (int i = 0; i < dotHeight; i++)
                {
                    PartOfLine = new int[DivideLineConstant];

                    int l = 0;
                    for (int y = 0 + z; y < DivideLineConstant + z; y++)
                    {
                        PartOfLine[l] = data[i, y];
                        l++;
                    }

                    PartialPackage.AddRange(LogoPartialPacketforNormalDisplay(PartOfLine));
                }
            }

            DisplayPackage.Add(Convert.ToByte(48 + displayNo));
            DisplayPackage.Add(Convert.ToByte(83)); // 'S'
            DisplayPackage.AddRange(PartialPackage);
            DisplayPackage.AddRange(EndOfProtocol);

            return Device.Write(DisplayPackage.ToArray());
        }

        private List<byte> LogoPartialPacketforNormalDisplay(int[] PartOfLine)
        {
            int length = PartOfLine.Length;
            bool[] bits = new bool[length];
            string HexValue = null;
            List<byte> Packet = new List<byte>();

            for (int i = 0; i < length; i++)
            {
                int y = length - 1 - i;

                bits[y] = false;

                //if (PartOfLine[i] == Enums.DisplayLedColor.Red)
                if (PartOfLine[i] != 0)
                {
                    bits[y] = true;
                }
            }

            HexValue = Convert.ToString(BitsToByte(bits), 16);

            if (HexValue.Length == 1)
            {
                HexValue = "0" + HexValue;
            }

            foreach (var item in HexValue.ToCharArray())
            {
                string tmp = "3" + item.ToString();
                Packet.Add(Byte.Parse(tmp, NumberStyles.HexNumber));
            }


            return Packet;
        }

    }

}
