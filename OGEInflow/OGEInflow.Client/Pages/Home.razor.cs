using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OGEInflow.Client.Pages;

public partial class Home : ComponentBase
{
    /* Graphs Section */
    public static MudBlazorGraph ScanActivationsGraph,
        RankedPersonIDGraph,
        RankedReaderIDGraph,
        RankedMachineGraph,
        RankedAvgAreaGraph;

    private static MudDatePicker StartPicker;
    private static MudDatePicker EndPicker;

    //Date Boundaries for the file given
    private static DateTime? FirstDate => ReaderEvent.MinDate;
    private static DateTime? LastDate => ReaderEvent.MaxDate;
    
    private static int dateRange;

    //Dates user can filter (Date Bounds)
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
        Console.WriteLine(" (LoadGraphs) Date Range: " + dateRange);
    }

    //Will cause page error if all create graphs aren't put into here
    private static void LoadGraphs()
    {
        if (StartDate != null && EndDate != null)
        {
            TimeSpan diff = EndDate.Value - StartDate.Value;
            dateRange = (int)diff.TotalDays;
        }

        createRankedAvgAreaGraph();
        createScanActivationGraph();
        createRankedPersonIDGraph();
        createRankedReaderIDGraph();
        // createRankedDevIDGraph();
        createRankedMachineGraph();
    }
    
    
    
    /* Graph Creation Functions */
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
        // ReaderEventWarning.CheckDoubleScans(ReaderEvent.PersonIDDict);
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
        ReaderEventWarning.CheckTooManyReaderScans(rankedReaderIDGraph,5);
    }

    public static void createRankedMachineGraph()
    {
        var rankedMachineGraph = GetTopRankedEventsFiltered(ReaderEvent.MachineDict, 5);
        
        ChartOptions options = new ChartOptions();
    
        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Top 5 Panels (Based on Machine)",
                Data = rankedMachineGraph.Values
                    .Select(entry => (double)entry.Count)
                    .ToArray()
            }
        };
        
        RankedMachineGraph = MudBlazorGraph.CreateGraph(series, rankedMachineGraph, null, options);
    }

    public static void createRankedAvgAreaGraph()
    {
        var rankedAvgAreaGraph = GetTopRankedEventsFiltered(ReaderEvent.ReaderDescDict, 5);
        
        ChartOptions options = new ChartOptions();
    
        List<ChartSeries> series = new List<ChartSeries>
        {
            new()
            {
                Name = "Top 5 Areas (Based on ReaderDesc)",
                Data = rankedAvgAreaGraph.Values
                    .Select(entry => (double)entry.Count / dateRange)
                    .ToArray()
            }
        };
        
        RankedAvgAreaGraph = MudBlazorGraph.CreateGraph(series, rankedAvgAreaGraph, null, options);
    }

    
    

    /* Helper functions */
    public static Dictionary<string, List<ReaderEvent>> GetTopRankedEventsFiltered(Dictionary<string, List<ReaderEvent>> inputDict, int topCount)
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