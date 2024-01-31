namespace Quavis.QorchLite.Hwlib.Printer
{


    public class VCPrinterEmseSettings
    {
        public bool TestMode { get; set; }
        public string VendorId { get; set; }
        public string ProductId { get; set; }

        public CuttingType CuttingType { get; set; }
        public PrinterMaxPixel PrinterMaxPixel { get; set; }
        public bool SaveImage { get; set; }
    }


}
