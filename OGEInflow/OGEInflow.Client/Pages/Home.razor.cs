using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OGEInflow.Client.Pages;

public partial class Home : ComponentBase
{
    /* Graphs Section */
    public static MudBlazorGraph ScanActivationsGraph, ReaderDescGraph, PersonIDGraph;

    protected override void OnInitialized()
    {
        if (ReaderEvent.readerEventsList != null)
        {
            LoadGraphs();
        }
    }

    //Will cause page error if all create graphs aren't put into here
    private static void LoadGraphs()
    {
        createReaderDescGraph();
        createScanActivationGraph();
        createPersonIDGraph();
    }
    
    private static void createReaderDescGraph()
    {
        ChartOptions options = new ChartOptions();
        
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

    private static void createScanActivationGraph()
    {
        ChartOptions options = new ChartOptions();
        
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

    private static void createPersonIDGraph()
    {
        ChartOptions options = new ChartOptions();
        
        List<ChartSeries> series = new List<ChartSeries>
        {
            new ()
            {
                Name = "Scan Activations",
                Data = ReaderEvent.PersonIDDict.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };
        
        PersonIDGraph = MudBlazorGraph.CreateGraph(series, ReaderEvent.PersonIDDict, null, options);
    }
    
}