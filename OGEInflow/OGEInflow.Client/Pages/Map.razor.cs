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

    private bool ShowPanels { get; set; } = true;
    private bool ShowReaders { get; set; } = true;
    private bool ShowHotPoints { get; set; } = false;
    
    private bool ShowPopup {get; set;} = false;
    
    private int testoutput = 0;

    private  void TestOutputVal()
    {
        testoutput++;
    }
    
    private void ToggleGradientColor()
    {
        Pin.UseGradientColor = !Pin.UseGradientColor;
        StateHasChanged();   // force re-render to update style
    }

    private void TogglePanels()
    {
        ShowPanels = !ShowPanels;
        StateHasChanged();
    }
    private void ToggleReaders()
    {
        ShowReaders = !ShowReaders;
        StateHasChanged();
    }

    private void ToggleHotPoints()
    {
        ShowHotPoints = !ShowHotPoints;
        ToggleGradientColor();
        StateHasChanged();
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