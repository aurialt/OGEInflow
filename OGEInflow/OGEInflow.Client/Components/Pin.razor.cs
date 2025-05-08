using Microsoft.AspNetCore.Components;
using OGEInflow.Client.Pages;
using OGEInflow.Client.Services;
using OGEInflow.Services;

namespace OGEInflow.Client.Components;

public partial class Pin : ComponentBase
{
    [Parameter] public string Top { get; set; } = "0px";
    [Parameter] public string Left { get; set; } = "0px";

    [Parameter] public string Bottom { get; set; } = "0px";

    [Parameter] public string Right { get; set; } = "0px";

     private string PinColor { get; set; } = "rgb(50, 189, 211);";
     
    public string Color { get; set; } = "";
    private string label = "";

    [Parameter]
    public string Label
    {
        get
        {
            return label;
        }
        set
        {
            label = value;
            Color = Map.GetColorFromGradientRatioValue(Map.GetGradientRatio(Label, ReaderEvent.ReaderIDDict, Settings.ReaderThreshold));
        }
    }
    
    [Parameter] public string Class { get; set; } = "";

    [Parameter] public Action OnClick { get; set; }

    public static bool UseGradientColor { get; set; } = false;
    private string ComputedPinColor => UseGradientColor ? Color : PinColor;

    public static void ReaderSetMapPopup(string tag)
    {
        Map.EditMapPopup(tag,
            Map.GetUsage(tag, ReaderEvent.ReaderIDDict, Settings.ReaderThreshold),
            GetWarningsForIssuesList(tag),
            null);
    }

    private static List<string> GetWarningsForIssuesList(string tag)
    {
        List<string> warnings = new List<string>();
        if (WarningManager.PastReaderThresholdWarnings.ContainsKey(tag))
        {
            warnings.Add(WarningManager.PastReaderThresholdWarnings[tag][0].Message);
        }
        else if (WarningManager.NearReaderThresholdWarnings.ContainsKey(tag))
        {
            warnings.Add(WarningManager.NearReaderThresholdWarnings[tag][0].Message);
        }

        if (WarningManager.ReaderDoubleScanThresholdWarnings.ContainsKey(tag))
        {
            warnings.Add(WarningManager.ReaderDoubleScanThresholdWarnings[tag][0].Message);
        }
        
        return warnings;
    }
}