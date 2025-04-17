using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OGEInflow.Client.Pages;

public partial class Home : ComponentBase
{
    /* Graphs Section */
    public static MudBlazorGraph ScanActivationsGraph,
        ReaderDescGraph,
        PersonIDGraph,
        RankedPersonIDGraph,
        RankedReaderIDGraph;

    private static MudDatePicker StartPicker;
    private static MudDatePicker EndPicker;

    //Date Boundaries for the file given
    private static DateTime? FirstDate => ReaderEvent.MinDate;
    private static DateTime? LastDate => ReaderEvent.MaxDate;

    //Dates User can Filter (Date Bounds)
    private static DateTime? StartDate { get; set; }
    private static DateTime? EndDate { get; set; }
    private static bool _autoClose = false;

    protected override void OnInitialized()
    {
        if (ReaderEvent.readerEventsList != null)
        {
            LoadGraphs();
        }
    }


    public static void InitializeDateBounds(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public void RefreshGraphs()
    {
        LoadGraphs();
        StateHasChanged();
    }

    //Will cause page error if all create graphs aren't put into here
    private static void LoadGraphs()
    {
        createReaderDescGraph();
        createScanActivationGraph();
        createPersonIDGraph();
        createRankedPersonIDGraph();
        createRankedReaderIDGraph();
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
            new()
            {
                Name = "Scan Activations per Day",
                Data = sortedDayOfWeekReaderEvents
                    .Select(entry => (double)entry.Value.Count)
                    .ToArray()
            }
        };

        ScanActivationsGraph =
            MudBlazorGraph.CreateGraph(series, ReaderEvent.DayOfWeekReaderEventsDict, daysOfWeek, options);
    }

    private static void createPersonIDGraph()
    {
        ChartOptions options = new ChartOptions();

        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Scan Activations per Person",
                Data = ReaderEvent.PersonIDDict.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };

        PersonIDGraph = MudBlazorGraph.CreateGraph(series, ReaderEvent.PersonIDDict, null, options);
    }

    private static void createRankedPersonIDGraph()
    {
        //Gets Top 5 Scan Activations
        var rankedPersonIDGraph = GetTopRankedEventsFiltered(ReaderEvent.PersonIDDict, 5);
        
        //TESTING SECTION
        // Console.WriteLine("RankedPersonIDGraph FILTERED DICTIONARY ");
        // int rank = 1;
        // foreach (var entry in rankedPersonIDGraph)
        // {
        //     Console.WriteLine($"Rank #{rank}");
        //     Console.WriteLine($"PersonID: {entry.Key}");
        //     Console.WriteLine($"Event Count: {entry.Value.Count}");
        //
        //     foreach (var readerEvent in entry.Value)
        //     {
        //         Console.WriteLine($" - EventTime: {readerEvent.EventTime}");
        //         // You can print other properties of readerEvent here as needed
        //     }
        //
        //     rank++;
        // }

        Console.WriteLine("FORLOOP END - RankedPersonIDGraph FILTERED DICTIONARY ");

        ChartOptions options = new ChartOptions();

        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Top 5 People",
                Data = rankedPersonIDGraph.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };

        RankedPersonIDGraph = MudBlazorGraph.CreateGraph(series, rankedPersonIDGraph, null, options);
    }


    public static void createRankedReaderIDGraph()
    {
        var rankedReaderIDGraph = GetTopRankedEventsFiltered(ReaderEvent.ReaderIDDict, 5);
        
        ChartOptions options = new ChartOptions();
    
        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Top 5 Readers (Based on ReaderID)",
                Data = rankedReaderIDGraph.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };
        
        RankedReaderIDGraph = MudBlazorGraph.CreateGraph(series, rankedReaderIDGraph, null, options);
    }


    /* Helper functions */
    public static  Dictionary<string, List<ReaderEvent>> GetTopRankedEventsFiltered(Dictionary<string, List<ReaderEvent>> inputDict, int topCount)
    {
        var rankedDict = inputDict
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value
                    .Where(re =>
                    {
                        if (DateTime.TryParse(re.EventTime, out DateTime eventDate))
                        {
                            return eventDate >= StartDate.Value && eventDate <= EndDate.Value;
                        }
                        return false;
                    })
                    .ToList()
            )
            .Where(entry => entry.Value.Any())
            .OrderByDescending(entry => entry.Value.Count)
            .Take(topCount)
            .ToDictionary(entry => entry.Key, entry => entry.Value);

        return rankedDict;
    }

}