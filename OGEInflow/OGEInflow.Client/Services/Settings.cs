using OGEInflow.Services;

namespace OGEInflow.Client.Services;

public class Settings
{
    public static int PanelThreshold { get; set; } = 4000;
    public static int PanelNearThreshold { get; set; } = 3000;
    public static int ReaderThreshold { get; set; } = 2000;
    public static int ReaderNearThreshold { get; set; } = 1500;
    
    public static int ReaderDoubleScanThreshold { get; set; } = 60;
    
    public static bool isOpenAllDay { get; set; } = false;
    public static int OpeningTime { get; set; } = 9;
    public static int ClosingTime { get; set; } = 18;

}