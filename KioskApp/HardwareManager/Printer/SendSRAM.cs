using System.Collections.Generic;

namespace Quavis.QorchLite.Hwlib.Printer
{
    public class SendSRAM
    {
        public List<byte> Data { get; set; }

        public int Address { get; set; }

        public SendSRAM()
        {
            this.Data = new List<byte>();
            this.Address = 0;
        }

    }
}
