using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OGEInflow.Client.Pages;

public partial class Home : ComponentBase
{
    /* Graphs Section */
    private static readonly ChartOptions options = new ChartOptions();
    public static MudBlazorGraph ScanActivationsGraph, ReaderDescGraph;

    protected override void OnInitialized()
    {
        if (ReaderEvent.readerEventsList != null)
        {
            LoadGraphs();
        }
    }

    private static void LoadGraphs()
    {
        createReaderDescGraph();
        createScanActivationGraph();
    }
    
    public static void createReaderDescGraph()
    {
        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Reader Description",
                Data = ReaderEvent.ReaderDescDict.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };
        
        ReaderDescGraph = MudBlazorGraph.CreateGraph(series, ReaderEvent.ReaderDescDict, null, options);
    }

    public static void createScanActivationGraph()
    {
        string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
        var sortedDayOfWeekReaderEvents = ReaderEvent.DayOfWeekReaderEventsDict
            .Where(entry => daysOfWeek.Contains(entry.Key))
            .OrderBy(entry => Array.IndexOf(daysOfWeek, entry.Key)) 
            .ToList();
        
        List<ChartSeries> series = new List<ChartSeries>
        {
            new ()
            {
                Name = "Scan Activations",
                Data = sortedDayOfWeekReaderEvents
                    .Select(entry => (double)entry.Value.Count) 
                    .ToArray()
            }
        };
        
        ScanActivationsGraph = MudBlazorGraph.CreateGraph(series, ReaderEvent.DayOfWeekReaderEventsDict, daysOfWeek, options);
    }
    
}