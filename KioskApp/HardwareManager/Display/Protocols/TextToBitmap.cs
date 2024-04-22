using KioskApp.HardwareManager.Display.Settings;
using System.Drawing.Imaging;
using System.Drawing;

namespace KioskApp.HardwareManager.Display.Protocols
{
    internal static class TextToBitmap
    {
        /// <summary>
        /// Setting nesnesi içinde verilen BreakMessage'ı bitmap yapar. 
        /// Şu an hardcode olarak sadece arapça için çalışıyor.
        /// </summary>
        public static byte[] MessageBitmap(string message, bool rightToLeft, VCTerminalSettings settings)
        {
            if (String.IsNullOrEmpty(message) || message.Length > 90)
            {
                return Array.Empty<byte>();
            }

            Bitmap BitmapFromText = CreateBitmapFromText(message, settings);
            BitmapFromText = RemoveEmptyLines(BitmapFromText);
            BitmapFromText = ResizedBitmap(BitmapFromText, settings);

            int DotWidth = BitmapFromText.Width + (8 - BitmapFromText.Width % 8);
            int DotHeight = BitmapFromText.Height;

            var BitmapAsBits = new bool[DotHeight, DotWidth];

            for (int y = 0; y < BitmapFromText.Height; y++)
            {
                BitmapData ImageData = BitmapFromText.LockBits(new Rectangle(0, y, BitmapFromText.Width, 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                IntPtr ptr = ImageData.Scan0;

                int bytes = BitmapFromText.Width * 3;
                byte[] rgbValues = new byte[bytes];
                bool[] TemporaryLineData = new bool[DotWidth];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                int count = 0;

                for (int i = 0; i < bytes; i = i + 3)
                {
                    if (rgbValues[i] <= settings.Font.BitmapContrast)
                    {
                        TemporaryLineData[count] = true;
                    }

                    count++;
                }

                if (rightToLeft) // && _settings.TechnicalData.ColorType == Enums.DisplayColorType.Normal) // multicolor ise ters olmuyor
                {
                    Array.Reverse(TemporaryLineData);
                }

                for (int i = 0; i < TemporaryLineData.Length; i++)
                {
                    BitmapAsBits[y, i] = TemporaryLineData[i];
                }

                BitmapFromText.UnlockBits(ImageData);
            }

            return ScanEachColumnBy8Bits(BitmapAsBits);
        }

        private static Bitmap CreateBitmapFromText(string textContent, VCTerminalSettings settings)
        {
            Font font = new Font(settings.Font.FontName, settings.Font.FontSize, (FontStyle)settings.Font.FontStyle);

            Bitmap objBmpImage = new Bitmap(1, 1, PixelFormat.Format24bppRgb);
            Graphics objGraphics = Graphics.FromImage(objBmpImage);
            int intWidth = (int)objGraphics.MeasureString(textContent, font).Width;
            int intHeight = (int)objGraphics.MeasureString(textContent, font).Height;
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));
            objGraphics = Graphics.FromImage(objBmpImage);
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            objGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            objGraphics.DrawString(textContent, font, new SolidBrush(Color.Black), 0, 0);
            objGraphics.Flush();

            return objBmpImage;
        }

        private static Bitmap RemoveEmptyLines(Bitmap ScrollingMessageImage)
        {
            int imgheight = ScrollingMessageImage.Height;
            int imgwidth = ScrollingMessageImage.Width;
            Rectangle CroppedArea = new Rectangle(0, 0, imgwidth, imgheight);
            Bitmap CroppedBitmap;

            // Üstten itibaren, içerisinde siyah renk bulunduran satırı bulmaya çalış.
            for (int y = 0; y < imgheight; y++)
            {

                BitmapData ImageData = ScrollingMessageImage.LockBits(new Rectangle(0, y, imgwidth, 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                IntPtr ptr = ImageData.Scan0;

                int bytes = Math.Abs(ImageData.Stride) * ImageData.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int i = 0; i < imgwidth * 3; i = i + 3)
                {
                    if (rgbValues[i] != 0xFF)
                    {
                        CroppedArea.Y = y;
                        ScrollingMessageImage.UnlockBits(ImageData);
                        goto ScanFromBelow; // Siyah içeriği bulunca taramayı sonlandır ve alt sıralardan tekrar başla
                    }
                }

                ScrollingMessageImage.UnlockBits(ImageData);
            }

        ScanFromBelow:

            // Alttan itibaren, içerisinde siyah renk bulunduran satırı bulmaya çalış.
            for (int y = imgheight - 1; y >= 0; y--)
            {

                BitmapData ImageData = ScrollingMessageImage.LockBits(new Rectangle(0, y, imgwidth, 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                IntPtr ptr = ImageData.Scan0;

                int bytes = Math.Abs(ImageData.Stride) * ImageData.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int i = 0; i < imgwidth * 3; i = i + 3)
                {
                    if (rgbValues[i] != 0xFF)
                    {
                        CroppedArea.Height = y - CroppedArea.Y + 1;
                        ScrollingMessageImage.UnlockBits(ImageData);
                        goto FinishCropping; // Siyah içeriği bulunca taramayı bitir
                    }

                }

                ScrollingMessageImage.UnlockBits(ImageData);
            }

        FinishCropping:

            //Beyaz satırlardan arındırılmış bitmapi oluştur
            CroppedBitmap = ScrollingMessageImage.Clone(CroppedArea, PixelFormat.Format24bppRgb);

            return CroppedBitmap;
        }

        private static Bitmap ResizedBitmap(Bitmap bitmapToResize, VCTerminalSettings settings)
        {
            int maxBitmapWidth = 800;
            int bitmapHeight = settings.DotHeight ?? 10;

            if (!settings.Font.BitmapStrech)
            {
                var resizedBitmap = ResizeImageWithAspectRatio(bitmapToResize, new Size(maxBitmapWidth, bitmapHeight));
                if (resizedBitmap.Width > maxBitmapWidth || resizedBitmap.Height > bitmapHeight)
                {
                    resizedBitmap = resizedBitmap.Clone(new Rectangle(0, 0, maxBitmapWidth, bitmapHeight), PixelFormat.Format24bppRgb);
                }
                return resizedBitmap;
            }

            return ResizeImageWithStretch(bitmapToResize, new Size(maxBitmapWidth, bitmapHeight));
        }

        private static Bitmap ResizeImageWithAspectRatio(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            float nPercentW = size.Width / (float)sourceWidth;
            float nPercentH = size.Height / (float)sourceHeight;
            float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            int destWidth = (int)Math.Round(sourceWidth * nPercent);
            int destHeight = (int)Math.Round(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        private static Bitmap ResizeImageWithStretch(Image imgToResize, Size MaxSize)
        {
            if (imgToResize.Width < MaxSize.Width)
            {
                return new Bitmap(imgToResize, new Size(imgToResize.Width, MaxSize.Height));
            }

            return new Bitmap(imgToResize, new Size(MaxSize.Width, MaxSize.Height));
        }

        private static byte[] ScanEachColumnBy8Bits(bool[,] scrollingMessageBitmapData)
        {
            const int DIVIDE_COLUMN_CONSTANT = 8;
            List<byte> partialPackage = new List<byte>();
            bool[] partOfLine = new bool[DIVIDE_COLUMN_CONSTANT];
            int l = 0;

            int dotWidth = scrollingMessageBitmapData.GetLength(1);
            int dotHeight = scrollingMessageBitmapData.GetLength(0);

            for (int y = 0; y < dotWidth; y++) //Her sütun 8nokta 8nokta 
            {
                for (int x = 0; x < dotHeight; x++)
                {
                    if (l == DIVIDE_COLUMN_CONSTANT)
                    {
                        partialPackage.AddRange(SMPartialPacketforNormalDisplay(partOfLine));
                        Array.Clear(partOfLine, 0, DIVIDE_COLUMN_CONSTANT);
                        l = 0;
                    }

                    partOfLine[DIVIDE_COLUMN_CONSTANT - 1 - l] = scrollingMessageBitmapData[x, y];

                    l++;
                }
            }

            return partialPackage.ToArray();
        }

        private static List<byte> SMPartialPacketforNormalDisplay(bool[] bits)
        {
            List<byte> packet = new List<byte>();

            string hexValue = Convert.ToString(BitsToByte(bits), 16);

            if (hexValue.Length == 1) { hexValue = "0" + hexValue; }

            foreach (var item in hexValue.ToCharArray())
            {
                string tmp = "3" + item.ToString();
                packet.Add(Byte.Parse(tmp, System.Globalization.NumberStyles.HexNumber));
            }

            return packet;
        }

        private static byte BitsToByte(bool[] bits)
        {
            byte output = 0x00;

            for (int i = 0; i < bits.Length; i++)
            {
                output += Convert.ToByte(Convert.ToByte(bits[i]) << i);
            }

            return output;
        }
    }

}
