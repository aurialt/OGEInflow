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

    [Parameter] public string PanelColor { get; set; } = "#ff0000";

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
            Color = Map.GetColorFromValue(Map.GetGradientRatio(label, ReaderEvent.MachineDict, Settings.PanelThreshold));
        }
    }

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public Action OnClick { get; set; }

    public static bool UseGradientColorPanel { get; set; } = false;
    private string ComputedPanelColor => UseGradientColorPanel ? Color : PanelColor;

    public static void PanelSetMapPopup(string tag)
    {
        Map.EditMapPopup(tag,
            Map.GetUsage(tag, ReaderEvent.MachineDict, Settings.PanelThreshold),
            null,
            null);
    }
}