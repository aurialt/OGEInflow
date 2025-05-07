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

     private string PinColor { get; set; } = "rgb(42, 121, 19);";
     
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
            Color = Map.GetColorFromValue(Map.GetGradientRatio(Label, ReaderEvent.ReaderIDDict, Settings.ReaderThreshold));
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
        if (WarningManager.HighReaderUsageWarnings.ContainsKey(tag))
        {
            warnings.Add(WarningManager.HighReaderUsageWarnings[tag][0].Message);
        }
        
        return warnings;
    }
}