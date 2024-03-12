namespace Quavis.QorchLite.Hwlib.Printer
{

    public enum ErrorCodes
    {
        PrinterBusy = 1,
        CutterError = 2,
        RollPaperDecreased = 3,
        PrinterHeadOpenOptical = 4,
        PrinterHeadOpenMechanical = 5,
        Overheating = 6,
        PaperJam = 7,
        RollPaperEmpty = 8
    }
    public enum CuttingType
    {
        HalfCut = 1,
        FullCut = 2,
        SpecialFullCut = 3,
        PrintOnly = 4
    }

    public enum PrinterMaxPixel
    {
        Pixel448 = 448, // 2 inch model
        Pixel640 = 640  // 3 inch model
    }

}
