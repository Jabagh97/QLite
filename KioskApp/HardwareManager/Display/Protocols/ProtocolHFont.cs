using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.HardwareManager.Display.Protocols
{
    public enum DisplayFunction
    {
        MainDisplay = 1,    //Ana gösterge
        CounterDisplay = 2  //terminal
    }


    #region DisplayStyle
    public enum DisplayArrowDirection
    {
        Right = 1,
        Left = 2,
        Down = 3,
        Up = 4,
        NoArrow = 5
    }
    public enum FontWeightType
    {
        Bold = 1,
        Thin = 4
    }

    public enum DisplayArrowStyle
    {
        Thin = 1,
        Bold = 5
    }
    #endregion

    public class ProtocolHFont
    {
        /*-------------------- IdNumbers Look Up Table Protypes --------------------*/
        public const int NOF_IdNumbers_CHARS = (10);
        public const byte BLANK_IdNumbers = (1);
        public const byte DisplayColour = (1);
        public const int DISPLAY_NOF_COL_MAX = (56);
        public const int DISPLAY_NOF_ROW_MAX = (10);

        /*-------------------- IdNumbers Look Up Table --------------------*/
        public static byte[] IdNumbers_LookUp = { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
        public static byte[] IdNumbers_Width = { 6, 3, 5, 5, 6, 5, 5, 5, 5, 5 };
        public static byte[] IdNumbers_Height = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
        public static int[] IdNumbers_Index = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        public static byte[] IdNumbers_Content = { 120, 132, 132, 140, 148, 164, 196, 132, 132, 120, 64, 192, 64, 64, 64, 64, 64, 64, 64, 224, 112, 136, 8, 8, 16, 32, 64, 128, 128, 248, 112, 136, 8, 8, 48, 8, 8, 8, 136, 112, 8, 16, 32, 64, 128, 144, 144, 252, 16, 16, 248, 128, 128, 128, 240, 8, 8, 8, 136, 112, 112, 136, 128, 128, 240, 136, 136, 136, 136, 112, 248, 8, 8, 16, 16, 32, 32, 32, 32, 32, 112, 136, 136, 136, 112, 136, 136, 136, 136, 112, 112, 136, 136, 136, 120, 8, 8, 8, 136, 112 };

        public byte TicketNoFontCode { get; }

        public byte ArrowFontCode { get; }

        public byte TerminalNoFontCode { get; }

        public int ArrowReferenceValue { get; }

        public SortedList<char, int> TicketNoDigitWidth { get; }

        public int[] TerminalNoDigitWidth { get; }

        public int[] FullSetArrowWidth { get; }

        public ProtocolHFont(FontWeightType SelectedStyle, DisplayArrowStyle SelectedArrowSet)
        {

            switch (SelectedStyle)
            {
                case FontWeightType.Bold:

                    TicketNoDigitWidth = new SortedList<char, int>(24)
                    {
                        {'0',7},{'1',6},{'2',7},{'3',7},{'4',7},{'5',7},{'6',7},{'7',7},{'8',7},{'9',7},
                        {'A',7},{'B',7},{'C',7},{'D',7},{'E',7},{'F',7},{'G',7},{'H',7},{'I',7},{'J',7},{'L',7},{'M',7},{'N',7},
                        {'P',7},{'O',7},{'R',7},{'S',7},{'U',7},{'Y',7}
                    };

                    TerminalNoDigitWidth = new int[10] { 5, 3, 5, 5, 5, 5, 5, 5, 5, 5 };

                    FullSetArrowWidth = new int[8] { 6, 6, 5, 5, 8, 8, 7, 7 }; // { → , ← , ↓ , ↑ , =► , ◄= , Bold2 ↓ , Bold2 ↑ }

                    break;

                case FontWeightType.Thin:

                    TicketNoDigitWidth = new SortedList<char, int>(24)
                    {
                        {'0',5},{'1',3},{'2',5},{'3',5},{'4',5},{'5',5},{'6',5},{'7',5},{'8',5},{'9',5},
                        {'A',5},{'B',5},{'C',5},{'D',5},{'E',5},{'F',5},{'H',5},{'J',5},{'L',5},{'N',5},
                        {'P',5},{'R',5},{'U',5},{'Y',5}
                    };

                    SortedList<char, int> ExtendedChars = new SortedList<char, int>()
                    {
                        {' ',3},{'!',1},{'"',4},{'#',6},{'$',5},{'%',5},{'&',5},{'\'',1},{'(',3},{')',3},
                        {'*',3},{'+',5},{',',2},{'-',3},{'.',2},{'/',5},{':',2},{';',2},{'<',4},{'=',4},
                        {'>',4},{'?',5},{'@',6},{'G',5},{'I',5},{'K',6},{'M',7},{'O',7},{'Q',5},{'S',6},
                        {'T',5},{'V',7},{'W',7},{'X',5},{'Z',6},{'[',2},{'\\',5},{']',2},{'^',5},{'_',4},
                        {'`',3},{'a',5},{'b',5},{'c',5},{'d',5},{'e',5},{'f',4},{'g',5},{'h',5},{'i',3},
                        {'j',4},{'k',4},{'l',2},{'m',7},{'n',5},{'o',5},{'p',5},{'q',5},{'r',4},{'s',5},
                        {'t',5},{'u',5},{'v',5},{'w',5},{'x',5},{'y',5},{'z',5},{'{',3},{'|',1},{'}',3},
                        {'~',5},{'Ç',5},{'Ğ',5},{'Ö',5},{'Ü',5},{'İ',3},{'Ş',5},{'ç',5},{'ğ',5},{'ö',5},
                        {'ü',5},{'ı',3},{'ş',5}
                    };

                    foreach (var item in ExtendedChars)
                    {
                        TicketNoDigitWidth.Add(item.Key, item.Value);
                    }


                    TerminalNoDigitWidth = new int[10] { 4, 3, 4, 4, 4, 4, 4, 4, 4, 4 };

                    FullSetArrowWidth = new int[8] { 4, 4, 5, 5, 4, 4, 5, 5 }; // { > , < , ↓ , ↑ , ► , ◄ , Bold ↓ , Bold ↑ }

                    break;

                default:
                    break;
            }

            ArrowReferenceValue = (int)SelectedArrowSet - 1;

            TicketNoFontCode = Convert.ToByte(48 + (int)SelectedStyle);
            TerminalNoFontCode = Convert.ToByte(48 + (int)SelectedStyle + 1);
            ArrowFontCode = Convert.ToByte(48 + (int)SelectedStyle + 2);
        }
    }
}
