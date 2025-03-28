using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace OGEInflow.Client.Pages;


public partial class Home : ComponentBase
{
    private static void LoadGraphs()
    {
        LoadReaderEventsLineGraph();
    }

    
    
    
    /* Graphs Section */
    private static ChartOptions options = new ChartOptions();
    public static List<ChartSeries> Series;
    public static string[] ScanActivations_x = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    private static Dictionary<string, int> eventCounts = new();

    protected override void OnInitialized()
    {
        if (ReaderEvent.readerEventsList != null)
        {
            LoadGraphs();
        }
    }

    public static void LoadReaderEventsLineGraph()
    {
        eventCounts = ScanActivations_x.ToDictionary(day => day, _ => 0);

        foreach (var entry in ReaderEvent.DayOfWeekReaderEventsDict)
        {
            if (eventCounts.ContainsKey(entry.Key))
            {
                eventCounts[entry.Key] = entry.Value.Count;
            }
        }

        // Populates chart data
        Series = new List<ChartSeries>
        {
            new()
            {
                Name = "Scan Activations",
                Data = eventCounts.Values.Select(count => (double)count).ToArray()
            }
        };
    }

    // public MudBlazorGraph LoadReaderDescGraph()
    // {
    //     
    //     return new MudBlazorGraph();
    // }
}