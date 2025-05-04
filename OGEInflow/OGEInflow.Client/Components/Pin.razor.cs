using Microsoft.AspNetCore.Components;

namespace OGEInflow.Client.Components;

public partial class Pin : ComponentBase
{
    [Parameter] public string Top { get; set; } = "0px";
    [Parameter] public string Left { get; set; } = "0px";

    [Parameter] public string Bottom { get; set; } = "0px";

    [Parameter] public string Right { get; set; } = "0px";

     private string PinColor { get; set; } = "rgb(42, 121, 19);";
     
    [Parameter] public string Color { get; set; } = "rgb(42, 121, 19);";
    [Parameter] public string Label { get; set; } = "";
    [Parameter] public string Class { get; set; } = "";

    [Parameter] public Action OnClick { get; set; }

    public static bool UseGradientColor { get; set; } = false;
    private string ComputedPinColor => UseGradientColor ? Color : PinColor;
    
}