using Microsoft.AspNetCore.Components;
using OGEInflow.Services;
using OGEInflow.Client.Components;
using OGEInflow.Client.Services;

namespace OGEInflow.Client.Pages;

public partial class Map : ComponentBase
{
    private bool ShowPopup {get; set;} = false;
    
    private int testoutput = 0;

    private  void TestOutputVal()
    {
        testoutput++;
        ToggleColor();
    }
    
    private async Task ToggleColor()
    {
        Pin.UseGradientColor = !Pin.UseGradientColor;
        StateHasChanged();   // force re-render to update style
    }

    private void TogglePopup()
    {
        ShowPopup = !ShowPopup;
    }

}