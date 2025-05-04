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

    private string MapFilterClass => ShowHotPoints ? "map-image map-image-hotpoints-filter" : "map-image";
    
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

    private void TogglePopup()
    {
        ShowPopup = !ShowPopup;
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

    
    /* Miscellaneous Methods */

    private double GetGradientRatio(string iconTypeID, Dictionary<string, List<ReaderEvent>> dictionary, int threshold)
    {
        if (dictionary.TryGetValue(iconTypeID, out var events))
        {
            Console.WriteLine($"ID: {iconTypeID} - {events.Count} events");
            return (double)events.Count / threshold;
        }
        Console.WriteLine("Error in finding hot-point gradient ratio with iconTypeID: " + iconTypeID);
        return 0;
    }
    
    public  string GetColorFromValue(double value)
    {
        // Normalize value between 0 and 1
        value = Math.Clamp(value, 0.0, 1.0);

        // Example: Gradient from red (bad) to green (good)
        int r = (int)(255 * (1 - value));
        int g = (int)(255 * value);
        int b = 0;
    
        Console.Write($" Ratio: {value} rgb: ({r}, {g}, {b})");
        return $"rgb({r}, {g}, {b})";
    }


    //Usages
    public string GetUsage(string iconTypeID, Dictionary<string, List<ReaderEvent>> dictionary, int threshold)
    {
        if (dictionary.TryGetValue(iconTypeID, out var events))
        {
            Console.WriteLine($"ID: {iconTypeID} - {events.Count} events");
            return $"{events.Count} / {threshold} scans";
        }
        Console.WriteLine("Failed to find in dictionary with iconTypeID: " + iconTypeID);
        return "";
    }
    

}