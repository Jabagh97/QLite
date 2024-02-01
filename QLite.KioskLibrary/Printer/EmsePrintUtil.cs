namespace Quavis.QorchLite.Hwlib.Printer
{
    using Quavis.QorchLite.Common;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;

    internal class EmsePrintUtil
    {
        //EmseUsbPrinterDevice SingleDevice;
        public CuttingType CuttingType { get; set; }
        public PrinterMaxPixel PrinterMaxPixelValue { get; set; }
        private SortedList<PrinterMaxPixel, int> m_TicketImageMaxHeight = new SortedList<PrinterMaxPixel, int>() { { PrinterMaxPixel.Pixel448, 8776 }, { PrinterMaxPixel.Pixel640, 6144 } };
        private const int m_EmptyLineHeightDefault = 75;
        public bool OnlyStretchIfMaxWidthExceeded { get; private set; }
        private byte[] bitmapData;
        private const int m_bmpHeaderSize = 53;
        //private ConcurrentQueue<Bitmap> printJobs = new ConcurrentQueue<Bitmap>();

        EmseUsbPrinterDevice Device;
        public EmsePrintUtil(EmseUsbPrinterDevice rd, CuttingType ct, PrinterMaxPixel printerMaxPixelValue)
        {
            Device = rd as EmseUsbPrinterDevice;
            PrinterMaxPixelValue = printerMaxPixelValue;
            CuttingType = ct;
        }

        private readonly object _printerStatusLock = new object();
        private int m_TotalTicketHeight;
        private Bitmap m_TicketImage;
        private readonly int TryCount = 3;

        public bool TryToPrint(Bitmap img)
        {
            //printJobs.Enqueue(img);
            bool isPrinted = false;
            int tryCount = 0;
            do
            {
                try
                {
                    isPrinted = Print(img);
                }
                catch (Exception)
                {
                    tryCount++;
                   // LoggerAdapter.Debug("TryToPrint count:" + tryCount);

                    if (tryCount == TryCount)
                        throw;
                    Thread.Sleep(100);
                }
            } while (!isPrinted && tryCount < TryCount);
            return isPrinted;
        }

        public bool Print(Bitmap img)
        {

            //LoggerAdapter.Debug("emsequeprintUtil print...");

            try
            {
                if (!IsPrinterReady())
                {
                    throw new Exception("Printer is not available");
                }

                lock (_printerStatusLock)
                {
                    if (CheckBitmap(img))
                    {
                        GetImageData();
                        SendToBuffer();
                        PrintBuffer();
                    }
                }

            }
            catch (Exception ex)
            {
                //LoggerAdapter.Error(ex, "");
                throw;
            }
            return true;
        }

        public bool IsPrinterReady()
        {
            lock (_printerStatusLock)
            {
                bool result = false;
                byte[] resp = Device.WriteAndWaitResponse(new byte[] { 0x06, 0x01, 0x00 });
                //Console.WriteLine("bytes:" + resp?.Length);
                if (resp?.Length > 0 && resp[0] == 0x05)
                {
                    //Console.WriteLine("resp:" + resp[1]);
                    List<bool> bytelist = ByteTobits(resp[1]);
                    //Console.WriteLine("bytelist:" + bytelist[0]);
                    if (bytelist[0] == false)
                        result = true;
                }
                //Console.WriteLine("IsPrinterReady:" + result);
                return result;
            }
        }


        public  bool PrintBuffer()
        {
            int StartAddress = 0, EndAddress = m_TotalTicketHeight;

            List<byte> package = new List<byte>();
            byte ReportID = 0x02;

            byte[] data2 = new byte[6] {
                ReportID,
                (byte)this.CuttingType,
                Convert.ToByte(StartAddress / 256),//TODO:255 olacak diğer printer için
                Convert.ToByte(StartAddress % 256),
                Convert.ToByte(EndAddress / 256),
                Convert.ToByte(EndAddress % 256)
            };

            return Device.Write(data2);
        }
        private  bool SendLineData(int lineAddress, List<byte> data)
        {
            List<byte> package = new List<byte>();
            byte ReportID = 0x01;

            //byte[] data2 = new byte[] {
            //    ReportID,
            //    Convert.ToByte(lineAddress / 256),
            //    Convert.ToByte(lineAddress % 256),
            //    Convert.ToByte(data.Count),
            //};

            package.Add(ReportID);
            package.Add(Convert.ToByte(lineAddress / 256));//TODO:255 olacak diğer printer için
            package.Add(Convert.ToByte(lineAddress % 256));
            package.Add(Convert.ToByte(data.Count));
            package.AddRange(data);

            return Device.Write(package.ToArray());
        }

        internal  bool CheckBitmap(Bitmap ticketImage)
        {
            bool state = false;

            int TicketImageMaxWidth = (int)PrinterMaxPixelValue;
            int TicketImageMaxHeight = m_TicketImageMaxHeight[PrinterMaxPixelValue];

            int emptyLineHeight = 0;
            if (this.CuttingType == CuttingType.SpecialFullCut)// m_EmptyLineHeightDefault olmadığı için az kısa kesiyor.
            {
                emptyLineHeight = 0;
            }
            else if (emptyLineHeight == 0)
            {
                emptyLineHeight = 0;// m_EmptyLineHeightDefault;//TODO:HalfCut için mecburi boşluk kalması gerekiyor.bunu ziraat için onlardan gelecek resmin üstünde boşluk olacak şekilde gelmesini istetyecez.
            }

            if (ticketImage.Width > (int)PrinterMaxPixelValue || (!OnlyStretchIfMaxWidthExceeded && ticketImage.Width != (int)PrinterMaxPixelValue))
            {
                float HR = ticketImage.HorizontalResolution;
                float VR = ticketImage.VerticalResolution;
                ticketImage = new Bitmap(ticketImage, new Size((int)PrinterMaxPixelValue, ticketImage.Height * (int)PrinterMaxPixelValue / ticketImage.Width));
                ticketImage.SetResolution(HR, VR);
                ticketImage = ticketImage.Clone(new Rectangle(0, 0, ticketImage.Width, ticketImage.Height), PixelFormat.Format24bppRgb);
            }

            //LoggerAdapter.Debug($"EmptyLineHeight: {emptyLineHeight} -- TicketImageWidth: {ticketImage.Width} -- TicketImagePixelFormat: {ticketImage.PixelFormat}");

            if (emptyLineHeight != 0 || (int)PrinterMaxPixelValue != ticketImage.Width || ticketImage.PixelFormat != PixelFormat.Format24bppRgb)
            {
                m_TicketImage = new Bitmap((int)PrinterMaxPixelValue, ticketImage.Height + emptyLineHeight, PixelFormat.Format24bppRgb);
                m_TicketImage.SetResolution(ticketImage.HorizontalResolution, ticketImage.VerticalResolution);
                Graphics gtmp = Graphics.FromImage(m_TicketImage);
                gtmp.Clear(Color.White);
                gtmp.DrawImage(ticketImage, ((int)PrinterMaxPixelValue - ticketImage.Width) / 2, emptyLineHeight);
                m_TotalTicketHeight = m_TicketImage.Height;
                ticketImage.Dispose();
                gtmp.Dispose();
            }
            else
            {
                m_TicketImage = ticketImage;
                m_TotalTicketHeight = m_TicketImage.Height;
            }

            try
            {

                if (m_TicketImage.PixelFormat == PixelFormat.Format24bppRgb &&
                    m_TicketImage.Width == TicketImageMaxWidth &&
                    m_TotalTicketHeight <= TicketImageMaxHeight)
                {
                    state = true;
                }

            }
            catch
            {
                state = false;
            }

            return state;
        }

        internal  void GetImageData()
        {
            MemoryStream BitmapStream = new MemoryStream();
            m_TicketImage.Save(BitmapStream, ImageFormat.Bmp);

            byte[] bitmapArray = BitmapStream.ToArray();

            List<byte> bitmapList = new List<byte>();

            for (int i = m_bmpHeaderSize + 3; i < bitmapArray.Length; i = i + 3)
            {
                bitmapList.Add(bitmapArray[i]);
            }

            bitmapData = bitmapList.ToArray();
        }

        internal bool SendToBuffer()
        {

            bool success = false;

            int ByteCount = bitmapData.Length / 8;
            int ByteRest = bitmapData.Length % 8;

            SendSRAM SRAM = null;
            List<SendSRAM> DataList = new List<SendSRAM>();

            for (int i = 0; i < ByteCount; i++)
            {

                byte[] pixelbits = new byte[8];
                Array.Copy(bitmapData, i * 8, pixelbits, 0, 8);

                byte pixel = BitsTobyte(bitConversion(pixelbits));

            NewSRAM:

                if (SRAM == null)
                {
                    SRAM = new SendSRAM();
                }

                if (SRAM?.Data?.Count < (int)PrinterMaxPixelValue / 8)
                {
                    SRAM.Data.Add(pixel);
                }
                else
                {
                    DataList.Add(SRAM);
                    SRAM = null;

                    goto NewSRAM;
                }

            }

            if (SRAM?.Data?.Count <= (int)PrinterMaxPixelValue / 8)
            {
                DataList.Add(SRAM);
            }

            if (ByteRest > 0)
            {
                byte[] pixelbits = new byte[8];
                Array.Copy(bitmapData, ByteCount * 8, pixelbits, 0, ByteRest);

                byte pixel = BitsTobyte(bitConversion(pixelbits));

                SRAM = new SendSRAM();
                SRAM.Data.Add(pixel);

                DataList.Add(SRAM);
            }

            bitmapData = null;

            List<byte> TotalDataReversed = new List<byte>();

            foreach (SendSRAM item in DataList)
            {
                item.Data.Reverse();
                TotalDataReversed.AddRange(item.Data);
            }

            byte[] TotalDataReversedArray = TotalDataReversed.ToArray();
            TotalDataReversed = null;

            DataList.Clear();
            DataList = null;
            DataList = new List<SendSRAM>();
            SRAM = null;

            foreach (byte pixel in TotalDataReversedArray)
            {

            NewSRAM2:

                if (SRAM == null)
                {
                    SRAM = new SendSRAM();
                    SRAM.Address = DataList.Count;
                }

                if (SRAM?.Data.Count < 56)
                {
                    SRAM.Data.Add(pixel);
                }
                else
                {
                    DataList.Add(SRAM);
                    SRAM = null;

                    goto NewSRAM2;
                }
            }

            if (SRAM?.Data.Count <= 56)
            {
                DataList.Add(SRAM);
            }

            foreach (SendSRAM item in DataList)
            {
                success = SendLineData(item.Address, item.Data);
                if (!success)
                {
                    return false;
                }
            }

            return success;
        }


        protected List<bool> ByteTobits(byte value)
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < 8; i++)
            {
                bits.Add(Convert.ToBoolean(value % 2));
                value = Convert.ToByte(value / 2);
            }

            return bits;
        }

        protected byte BitsTobyte(bool[] bits)
        {
            byte output = 0x00;

            for (int i = 0; i < bits.Length; i++)
            {
                output += Convert.ToByte(Convert.ToByte(bits[i]) << i);
            }

            return output;
        }

        protected bool[] bitConversion(byte[] partofline)
        {
            bool[] output = Array.ConvertAll(partofline,
                delegate (byte i)
                {
                    //if (i != 0xFF) // Beyaz olmayanlar siyah kabul edilir.
                    if (i < 0x7F)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
            );

            return output;
        }


    }
}
