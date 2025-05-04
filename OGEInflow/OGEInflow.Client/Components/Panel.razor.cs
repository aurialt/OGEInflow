using Microsoft.AspNetCore.Components;

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

    [Parameter]
    public string PanelColor { get; set; } = "#ff0000";

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public Action OnClick { get; set; }

    public static bool UseGradientColorPanel { get; set; } = false;
    private string ComputedPanelColor => UseGradientColorPanel ? GetColorFromValue(0) : PanelColor;

    private string GetColorFromValue(double value)
    {
        // Normalize value between 0 and 1
        value = Math.Clamp(value, 0.0, 1.0);

        // Example: Gradient from red (bad) to green (good)
        int r = (int)(255 * (1 - value));
        int g = (int)(255 * value);
        int b = 0;

        return $"rgb({r}, {g}, {b})";
    }
}