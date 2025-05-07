using Microsoft.AspNetCore.Components;
using OGEInflow.Services;
using OGEInflow.Client.Components;

namespace OGEInflow.Client.Pages;

public partial class Map : ComponentBase
{
    public static string IconName { get; set; } = "";
    public static string Usage { get; set; } = "";
    public static List<string> Issues { get; set; } = new();
    public static List<string> ConnectedReaders { get; set; } = new();

    private bool ShowPanels { get; set; } = true;
    private bool ShowReaders { get; set; } = true;
    private bool ShowHotPoints { get; set; } = false;
    
    private static bool ShowPopup {get; set;} = false;

    private string MapFilterClass => ShowHotPoints ? "map-image map-image-hotpoints-filter" : "map-image";
    
    public static Action? OnEditMapPopup;
    protected override void OnInitialized()
    {
        OnEditMapPopup = () => InvokeAsync(() =>
        {
            TogglePopup();
            StateHasChanged();
        });
    }
    
    private void ToggleGradientColor()
    {
        Pin.UseGradientColor = !Pin.UseGradientColor;
        Panel.UseGradientColor = !Panel.UseGradientColor;
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

    private static void TogglePopup()
    {
        ShowPopup = !ShowPopup;
    }
    
    public static void EditMapPopup(string iconName, string usage, List<string>? issues, List<string>? connectedReaders)
    {
        IconName = iconName;
        Usage = usage;
        if (issues is not null)
            Issues = issues;
        if (connectedReaders is not null)
            ConnectedReaders = connectedReaders;
        
        OnEditMapPopup?.Invoke();
    }
    
    /* Miscellaneous Methods */

    public static double GetGradientRatio(string iconTypeID, Dictionary<string, List<ReaderEvent>> dictionary, int threshold)
    {
        if (dictionary.TryGetValue(iconTypeID, out var events))
        {
            Console.WriteLine($"ID: {iconTypeID} - {events.Count} events");
            return (double)events.Count / threshold;
        }
        Console.WriteLine("Error in finding hot-point gradient ratio with iconTypeID: " + iconTypeID);
        return 0;
    }
    
    public static string GetColorFromGradientRatioValue(double value)
    {
        value = Math.Clamp(value, 0.0, 1.0);
        
        int r = (int)(255 * value);
        int g = (int)(255 * (1 - value));
        int b = 0;
    
        Console.Write($" Ratio: {value} rgb: ({r}, {g}, {b})");
        return $"rgb({r}, {g}, {b})";
    }


    //Usages
    public static string GetUsage(string iconTypeID, Dictionary<string, List<ReaderEvent>> dictionary, int threshold)
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