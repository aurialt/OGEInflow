using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace OGEInflow.Client.Pages;


public partial class Home : ComponentBase
{
    public static string FileName { get; set; } = "";
    public static long FileSize { get; set; }
    public static string FileType { get; set; } = "";
    public static DateTimeOffset LastModified { get; set; }
    public static string ErrorMessage { get; set; } = "";
    
    const int MAX_FILESIZE = 5000 * 1024; // 2 MB

    // Handles multiple files
    public static void LoadGraphs()
    {
        DayOfWeekReaderEvents(ReaderEvent.readerEvents);
        Console.WriteLine("DayOfWeekReaderEvents called");
        
        LoadReaderEventsLineGraph();
        Console.WriteLine("LoadReaderEventsLineGraph called");
    }

    static Dictionary<string, List<ReaderEvent>> ListDayOfWeekReaderEvents = new Dictionary<string, List<ReaderEvent>>();
    public static void DayOfWeekReaderEvents(List<ReaderEvent> eventsList)
    {
        foreach (var eventItem in eventsList)
        {
            string date = eventItem.EventTime;
            DateTime dateTime = DateTime.Parse(date);
            string dayOfWeek = dateTime.DayOfWeek.ToString();

            if (!ListDayOfWeekReaderEvents.ContainsKey(dayOfWeek))
            {
                ListDayOfWeekReaderEvents[dayOfWeek] = new List<ReaderEvent>();
            }
            else
            {
                ListDayOfWeekReaderEvents[dayOfWeek].Add(eventItem);
            }
        }
    }
    
    
    /* Graphs Section */
    private static ChartOptions options = new ChartOptions();
    public static List<ChartSeries> Series;
    public static string[] XAxisLabels = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    private static Dictionary<string, int> eventCounts = new();

    protected override void OnInitialized()
    {
        if (ReaderEvent.readerEvents != null)
        {
            LoadGraphs();
        }
    }

    public static void LoadReaderEventsLineGraph()
    {
        eventCounts = XAxisLabels.ToDictionary(day => day, _ => 0);

        foreach (var entry in ListDayOfWeekReaderEvents)
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
                Name = "Events per Day",
                Data = eventCounts.Values.Select(count => (double)count).ToArray()
            }
        };
    }
}