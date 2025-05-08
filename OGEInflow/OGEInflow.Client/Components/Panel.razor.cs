using Microsoft.AspNetCore.Components;
using OGEInflow.Client.Pages;
using OGEInflow.Client.Services;
using OGEInflow.Services;

namespace OGEInflow.Client.Components;

public partial class Panel : ComponentBase
{
    [Parameter]
    public string Top { get; set; } = "0px";

    [Parameter]
    public string Left { get; set; } = "0px";

    [Parameter] 
    public string Bottom { get; set; } = "0px";

    [Parameter]
    public string Right { get; set; } = "0px";

    [Parameter] public string PanelColor { get; set; } = "rgb(201, 61, 255);";

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
            Color = Map.GetColorFromGradientRatioValue(Map.GetGradientRatio(label, ReaderEvent.MachineDict, Settings.PanelThreshold));
        }
    }

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public Action OnClick { get; set; }

    public static bool UseGradientColor { get; set; } = false;
    private string ComputedPanelColor => UseGradientColor ? Color : PanelColor;

    public static void PanelSetMapPopup(string tag)
    {
        Map.EditMapPopup(tag,
            Map.GetUsage(tag, ReaderEvent.MachineDict, Settings.PanelThreshold),
            GetWarningsForIssuesList(tag),
            null);
    }

    private static List<string> GetWarningsForIssuesList(string tag)
    {
        List<string> warnings = new List<string>();
        
        if(WarningManager.PastPanelThresholdWarnings.ContainsKey(tag))
        {
            warnings.AddRange(WarningManager.PastPanelThresholdWarnings[tag][0].Message);
        }
        else if (WarningManager.NearPanelThresholdWarnings.ContainsKey(tag))
        {
            warnings.AddRange(WarningManager.NearPanelThresholdWarnings[tag][0].Message);
        }
        
        return warnings;
    }
}