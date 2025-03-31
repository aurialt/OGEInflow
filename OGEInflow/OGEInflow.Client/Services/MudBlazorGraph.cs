using MudBlazor;
namespace OGEInflow.Services;

public class MudBlazorGraph
{
    public ChartOptions Options { get; set; }
    public List<ChartSeries> Series { get; set; }
    public string[] Labels { get; set; }
    public Dictionary<string, int> EventCounts { get; set; }

    public MudBlazorGraph(ChartOptions options, string[] labels, Dictionary<string, int> eventCounts, List<ChartSeries> series)
    {
        Options = options;
        Labels = labels;
        EventCounts = eventCounts;
        Series = series;
    }

    public static MudBlazorGraph CreateGraph( List<ChartSeries> series, Dictionary<string, List<ReaderEvent>> sourceData, string[]? xAxisLabels, ChartOptions options)
    {
        // If xAxisLabels is null, generate labels from the source data
        string[] labels = xAxisLabels ?? ExtractLabels(sourceData);
        Dictionary<string, int> eventCounts = ComputeEventCounts(sourceData, labels);
        foreach (var ec in eventCounts)
        {
            Console.WriteLine("CreateGraph() ec: " + ec.Key + " - " + ec.Value);
        }

        return new MudBlazorGraph(options, labels, eventCounts, series);
    }
    
    public static List<ChartSeries> CreatePieSeries(Dictionary<string, int> eventCounts)
    {
        return new List<ChartSeries>
        {
            new ChartSeries
            {
                Name = "Pie Chart Data",  // Optional, could be used for the chart's legend
                Data = eventCounts.Values.Select(count => (double)count).ToArray(),
            }
        };
    }
    
    public static MudBlazorGraph CreateGraphForPieChart(
        Dictionary<string, List<ReaderEvent>> sourceData,
        string[]? xAxisLabels,
        ChartOptions options)
    {
        // Create the event counts and pie chart series
        Dictionary<string, int> eventCounts = ComputeEventCounts(sourceData, xAxisLabels ?? ExtractLabels(sourceData));
        List<ChartSeries> series = CreatePieSeries(eventCounts);

        // Return a MudBlazorGraph instance with pie chart data
        return new MudBlazorGraph(options, xAxisLabels ?? ExtractLabels(sourceData), eventCounts, series);
    }
    
    public static string[] ExtractLabels(Dictionary<string, List<ReaderEvent>> sourceData)
    {
        return sourceData.Keys.ToArray();
    }

    public static Dictionary<string, int> ComputeEventCounts(
        Dictionary<string, List<ReaderEvent>> sourceData, 
        string[] labels)
    {
        return labels.ToDictionary(
            label => label, 
            label => sourceData.ContainsKey(label) ? sourceData[label].Count : 0);
    }
}
