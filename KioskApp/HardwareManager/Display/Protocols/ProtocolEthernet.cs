using KioskApp.HardwareManager.Display.Settings;
using static QLite.Data.Models.Enums;
using System.Globalization;
using System.Text;

namespace KioskApp.HardwareManager.Display.Protocols
{
    public class ProtocolEthernet : IProtocolEthernet
    {

        private byte[] TicketNoDigits;
        private byte[] TerminalNoDigits;

        private readonly int standardBlank = 1;
        private readonly int serviceCodeBlank = 1;
        private readonly byte[] _endOfProtocol = { 10, 13 };

        /// <summary>
        /// 1254 => windows-1254 Turkish (Windows) => okunan byteların char' a dönüştürülmesinde 144 den sonra sorun oluyor 
        /// 28599 => iso-8859-9 Turkish (ISO)
        /// </summary>
        private readonly Encoding _defaultProtocolEncoding;

        private bool ServiceCodeIsShowing { get; set; } = true;
        private bool TerminalNoIsShowing { get; set; } = true;

        public static byte BaseTerminalID { get; } = 48;

        public static byte BaseMainID { get; } = 159;

       

        private const byte MARQUE_DIRECTION_RIGHT_TO_LEFT = 105;
        private const byte MARQUE_DIRECTION_LEFT_TO_RIGHT = 112;

        public ProtocolEthernet()
        {
            _defaultProtocolEncoding = Encoding.GetEncoding(28591);
            ServiceCodeIsShowing = true;
            TerminalNoIsShowing = true;
        }

        public byte[] ProduceArabicDelayTime(int slotNo, byte displayNo, int delayTime)
        {
            List<byte> PackageToSend = new List<byte>();

            byte[] ArabicDelayTime = this.ToDigits(delayTime.ToString("00"));
            int totalLength = 4 + _endOfProtocol.Length;


            PackageToSend.Add(0x03);
            PackageToSend.Add((byte)(0xA0 + slotNo));
            PackageToSend.Add((byte)totalLength); // Package length of all the below items 
            PackageToSend.Add(displayNo);
            PackageToSend.Add(Convert.ToByte(150));
            PackageToSend.Add(ArabicDelayTime[0]);
            PackageToSend.Add(ArabicDelayTime[1]);
            PackageToSend.AddRange(_endOfProtocol);

            return PackageToSend.ToArray();
        }

        public byte[] ProduceDingDongMessage()
        {
            throw new NotImplementedException();
        }

        public byte[] ProduceDisplayNumberMessage(byte displayNo, int rowId, int slotNo, int dotHeight, int dotWidth)
        {
            var data = GetNumberDotValues(rowId, dotHeight, dotWidth);

            List<byte> DisplayPackage = new List<byte>();
            //int[,] data = new int[dotHeight, dotWidth];

            const int DIVIDE_LINE_CONSTANT = 8;
            int[] PartOfLine;

            List<byte> PartialPackage = new List<byte>();



            for (int z = 0; z < dotWidth; z = z + DIVIDE_LINE_CONSTANT) //Her satırı 8nokta 8nokta 
            {
                for (int i = 0; i < dotHeight; i++)
                {
                    PartOfLine = new int[DIVIDE_LINE_CONSTANT];

                    int l = 0;
                    for (int y = 0 + z; y < DIVIDE_LINE_CONSTANT + z; y++)
                    {
                        PartOfLine[l] = data[i, y];
                        l++;
                    }

                    PartialPackage.AddRange(LogoPartialPacketforNormalDisplay(PartOfLine));
                }
            }
            int totalLength = 2 + PartialPackage.Count + _endOfProtocol.Length;

            DisplayPackage.Add(0x03);
            DisplayPackage.Add((byte)(0xA0 + slotNo));
            DisplayPackage.Add((byte)totalLength); // Package length of all the below items 
            DisplayPackage.Add(displayNo);
            DisplayPackage.Add(Convert.ToByte(83)); // 'S'
            DisplayPackage.AddRange(PartialPackage);
            DisplayPackage.AddRange(_endOfProtocol);

            return DisplayPackage.ToArray();
        }


        public byte[] ProduceSettingsMessage(int slotNo, byte displayNo, int flashCountVal, int dimmingTimeVal)
        {
            int flashcount = flashCountVal - 1;
            flashcount = flashcount <= 0 ? 1 : flashcount;
            int dimmingtime = dimmingTimeVal;
            List<byte> DisplayPackage = new List<byte>();

            if (flashcount >= 0 && flashcount < 100 && dimmingtime >= 0 && dimmingtime < 100)
            {
                byte[] flashcount_ary = this.ToDigits(flashcount.ToString("00"));
                byte[] dimmingtime_ary = this.ToDigits(SetDimmingTime(dimmingtime).ToString("00"));

                int totalLength = 6 + _endOfProtocol.Length;
                DisplayPackage.Add(0x03);
                DisplayPackage.Add((byte)(0xA0 + slotNo));
                DisplayPackage.Add((byte)totalLength);
                DisplayPackage.Add(displayNo);
                DisplayPackage.Add(Convert.ToByte(119));
                DisplayPackage.Add(flashcount_ary[0]);
                DisplayPackage.Add(flashcount_ary[1]);
                DisplayPackage.Add(dimmingtime_ary[0]);
                DisplayPackage.Add(dimmingtime_ary[1]);
                DisplayPackage.AddRange(_endOfProtocol);
                return DisplayPackage.ToArray();
            }

            return Array.Empty<byte>();

            int SetDimmingTime(int dimmingtime)
            {
                if (dimmingtime == 1 || dimmingtime == 2)
                {
                    dimmingtime = 1;
                }
                else
                {
                    dimmingtime /= 2;
                }

                return dimmingtime;
            }
        }

        public byte[] ProduceMarquee(int slotNo, byte displayNo, string messageText, int messageIdleTime)
        {
            var buffer = new List<byte>();

            byte[] messageIdleTimeDigits = GetDigits(messageIdleTime, 0, 999); // Min 0, Max 999
            char[] messageChars = messageText?.ToCharArray() ?? Array.Empty<char>();
            byte[] breakMessage = messageChars.Any() ? _defaultProtocolEncoding.GetBytes(messageChars) : Array.Empty<byte>();

            int totalLength = 5 + breakMessage.Length + _endOfProtocol.Length;
            totalLength += (messageIdleTime != 0) ? 1 : 0;

            buffer.Add(0x03);
            buffer.Add((byte)(0xA0 + slotNo));
            buffer.Add((byte)totalLength);
            buffer.Add(displayNo);
            buffer.Add(Convert.ToByte(110));
            buffer.Add(messageIdleTimeDigits[0]);
            buffer.Add(messageIdleTimeDigits[1]);
            buffer.Add(messageIdleTimeDigits[2]);
            if (messageIdleTime != 0)
            {
                buffer.Add(Convert.ToByte(' ')); // Mesaj bilgisi öncesinde Boşluk karakteri gönderilir (13.07.2011)
            }
            buffer.AddRange(breakMessage);
            buffer.AddRange(_endOfProtocol);

            return buffer.ToArray();
        }

        public byte[] ProduceTicketMessage(TicketInfo ticket)
        {
            List<byte> DisplayPackage = new List<byte>();
            byte[] PartialPackage = ProduceTicketNumber(ticket);

            if (PartialPackage != null)
            {
                int totalLength = 2 + PartialPackage.Length + _endOfProtocol.Length;

                DisplayPackage.Add(0x03);
                DisplayPackage.Add((byte)(0xA0 + ticket.SlotNumber));
                DisplayPackage.Add((byte)totalLength);
                DisplayPackage.Add(Convert.ToByte(ticket.DisplayNo));
                DisplayPackage.Add(Convert.ToByte(72)); // H Protocol
                DisplayPackage.AddRange(PartialPackage);
                DisplayPackage.AddRange(_endOfProtocol);
            }
            else
            {
                Console.WriteLine("Package preperation is failed.");
                return Array.Empty<byte>();
            }


            return DisplayPackage.ToArray();
        }

        private byte[] ToDigits(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return Array.Empty<byte>();
            }

            char[] digits = value.ToCharArray();
            return _defaultProtocolEncoding.GetBytes(digits);
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
                    data[y_pos, x_pos] = GetBitVal(ProtocolHFont.IdNumbers_Content[ProtocolHFont.IdNumbers_Index[digit_tens] + y_pos], char_col_no);
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
                    data[y_pos, x_pos] = GetBitVal(ProtocolHFont.IdNumbers_Content[ProtocolHFont.IdNumbers_Index[digit_ones] + y_pos], char_col_no);
                }

                char_col_no--;
            }

            return data;

            static byte GetBitVal(byte val, byte bit_no)
            {
                val >>= bit_no;
                val &= 0x01;

                if (val != 0)
                    val = 75;
                else
                    val = 0;
                return val;
            }
        }

        private List<byte> LogoPartialPacketforNormalDisplay(int[] PartOfLine)
        {
            int length = PartOfLine.Length;
            bool[] bits = new bool[length];
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

            string HexValue = Convert.ToString(BitsToByte(bits), 16);

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

            byte BitsToByte(bool[] bits)
            {
                byte output = 0x00;

                for (int i = 0; i < bits.Length; i++)
                {
                    output += Convert.ToByte(Convert.ToByte(bits[i]) << i);
                }

                return output;
            }

        }

        private byte[] GetDigits(int value, int min, int max)
          => _defaultProtocolEncoding
              .GetBytes(Math.Clamp(value, min, max)
                            .ToString(new string('0', max.ToString().Length))
                            .ToArray())
              .ToArray();

        private byte[] ProduceTicketNumber(TicketInfo ticket)
        {
            bool calculate = true;
            SortedList<string, int> StartingPoints = new(2)
            {
                {"TicketNoSection", 0},
                {"TerminalNoSection", 0}
            };

            int displayWidth = ticket.Settings.DotWidth ?? 0;
            ProtocolHFont font = GetFont(ticket.Settings);

            while (calculate)
            {
                int RemainingDotCount = displayWidth;
                int TerminalNoSectionItemCount = 0;
                int TicketNoSectionWidth = 0;
                int TerminalNoSectionWidth = 0;
                int DisableServiceCode = 1;
                StartingPoints = new SortedList<string, int>(2)
                {
                    {"TicketNoSection",0},
                    {"TerminalNoSection",0}
                };

                TicketNoDigits = ToDigits(ticket.TicketNumber);

                //if (TicketNoDigits.IsNullOrEmpty())
                //{
                //    return null;
                //}

                if (ticket.Direction != DisplayArrowDirection.NoArrow && TerminalNoIsShowing)
                {
                    TerminalNoSectionWidth += font.FullSetArrowWidth[(int)ticket.Direction + font.ArrowReferenceValue - 1];
                    TerminalNoSectionItemCount++;
                }

                TerminalNoDigits = ToDigits(ticket.TicketDisplayNo.ToString());
                //if (TerminalNoDigits.IsNullOrEmpty())
                //{
                //    return null;
                //}

                for (int i = 0; i < TerminalNoDigits.Length; i++)
                {
                    TerminalNoSectionWidth += font.TerminalNoDigitWidth[Convert.ToInt32(Convert.ToChar(TerminalNoDigits[i]).ToString())];
                    TerminalNoSectionItemCount++;
                }

                TerminalNoSectionWidth += (TerminalNoSectionItemCount - 1) * standardBlank;
                RemainingDotCount -= TerminalNoSectionWidth;

                if (ServiceCodeIsShowing || TicketNoDigits.Length < 4)
                {
                    DisableServiceCode = 0;
                }

                //Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi
                char[] TicketNoCharArray = _defaultProtocolEncoding.GetChars(TicketNoDigits);

                for (int i = DisableServiceCode; i < TicketNoDigits.Length; i++)
                {
                    //TicketNoSectionWidth = TicketNoSectionWidth + base.Display.Font.TicketNoDigitWidth[Convert.ToChar(TicketNoDigits[i])];
                    //Ek font tanımlamaları geldikten sonra 255 üzeri byteları düzgün okumak için eklendi

                    if (font.TicketNoDigitWidth.ContainsKey(TicketNoCharArray[i]))
                    {
                        TicketNoSectionWidth += font.TicketNoDigitWidth[TicketNoCharArray[i]];
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
                    if (TerminalNoIsShowing && ticket.Direction != DisplayArrowDirection.NoArrow)
                    {
                        ticket.Direction = DisplayArrowDirection.NoArrow;
                    }
                    else if (TerminalNoIsShowing)
                    {
                        TerminalNoIsShowing = false;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    StartingPoints["TicketNoSection"] = RemainingDotCount / 2 + 1;
                    StartingPoints["TerminalNoSection"] = displayWidth - TerminalNoSectionWidth + 1;
                    calculate = false;
                }
            }

            return PreparePartialPackage(StartingPoints, ticket.Direction, displayWidth, font);
        }
        private byte[] PreparePartialPackage(SortedList<string, int> StartingPoints, DisplayArrowDirection direction, int displayDotWidth, ProtocolHFont font)
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

           

            for (int i = DisableServiceCode; i < TicketNoLength; i++)
            {
                ItemPackage.Add(font.TicketNoFontCode);
                ItemPackage.Add(Convert.ToByte(48 + Point));
                ItemPackage.Add(TicketNoDigits[i]);

                Point = Point + font.TicketNoDigitWidth[Convert.ToChar(TicketNoDigits[i])];

               

                if (TicketNoLength == 4 && i == 0 && ServiceCodeIsShowing)
                {
                    Point += serviceCodeBlank;
                }
                else
                {
                    Point += standardBlank;
                }
            }

            if (TerminalNoIsShowing)
            {
                Point = StartingPoints["TerminalNoSection"];

                if (direction != DisplayArrowDirection.NoArrow)
                {
                    SelectedArrow = (int)direction + font.ArrowReferenceValue;
                    ItemPackage.Add(font.ArrowFontCode);
                    ItemPackage.Add(Convert.ToByte(48 + Point));
                    ItemPackage.Add(Convert.ToByte(48 + SelectedArrow));
                    Point = Point + font.FullSetArrowWidth[SelectedArrow - 1] + standardBlank;
                }

                for (int i = 0; i < TerminalNoDigits.Length; i++)
                {
                    ItemPackage.Add(font.TerminalNoFontCode);
                    ItemPackage.Add(Convert.ToByte(48 + Point));
                    ItemPackage.Add(TerminalNoDigits[i]);
                    Point = Point + font.TerminalNoDigitWidth[Convert.ToInt32(Convert.ToChar(TerminalNoDigits[i]).ToString())] + standardBlank;
                }
                Point--;
                if (Point > displayDotWidth + 1)
                    return null;
            }

            return ItemPackage.ToArray();
        }
        private static ProtocolHFont GetFont(VCTerminalSettings settings)
        {
            var displayFontStyle = settings.Font.FontWeight == FontWeightType.Bold ? FontWeightType.Bold : FontWeightType.Thin;
            var displayArrowStyle = settings.Font.FontWeight == FontWeightType.Bold ? DisplayArrowStyle.Bold : DisplayArrowStyle.Thin;
            var Font = new ProtocolHFont(displayFontStyle, displayArrowStyle);
            return Font;
        }
    }

    public interface IProtocolEthernet
    {
        /// <summary>
        /// Ticket çağrı buffer'ını üretir
        /// </summary>
        /// <param name="displayNo">"Ticket"ın gösterileceği ekran</param>
        /// <param name="callerDisplayNo">Ekranda gösterilecek olan; "çağıran" displayNo; örnek 3 nolu display için şu şekilde yazacak: A101 > 3</param>
        /// <returns></returns>
        byte[] ProduceTicketMessage(TicketInfo ticket);
        byte[] ProduceMarquee(int rowId, byte displayNo, string messageText, int messageIdleTime);

        byte[] ProduceSettingsMessage(int rowId, byte displayNo, int flashCountVal, int dimmingTimeVal);
        byte[] ProduceDisplayNumberMessage(byte displayNo, int rowId, int slotNo, int dotHeight, int dotWidth);
        byte[] ProduceArabicDelayTime(int rowId, byte displayNo, int delayTime);
        byte[] ProduceDingDongMessage();
    }
}
