using Microsoft.AspNetCore.Components;
using OGEInflow.Services;
using OGEInflow.Client.Components;
using OGEInflow.Client.Services;

namespace OGEInflow.Client.Pages;

public partial class Map : ComponentBase
{
    public string IconName { get; set; } = "";
    public string Usage { get; set; } = "";
    public List<string> Issues { get; set; } = new();
    public List<string> ConnectedReaders { get; set; } = new();
    
    private bool ShowPopup {get; set;} = false;
    
    private int testoutput = 0;

    private  void TestOutputVal()
    {
        testoutput++;
        ToggleGradientColor();
    }
    
    private void ToggleGradientColor()
    {
        Pin.UseGradientColor = !Pin.UseGradientColor;
        StateHasChanged();   // force re-render to update style
    }

    private void EditPopup(string iconName, string usage, List<string>? issues, List<string>? connectedReaders)
    {
        IconName = iconName;
        Usage = usage;
        if (issues is not null)
        Issues = issues;
        if (connectedReaders is not null)
        ConnectedReaders = connectedReaders;
        TogglePopup();
    }

    private void TogglePopup()
    {
        ShowPopup = !ShowPopup;
    }
    
    

}