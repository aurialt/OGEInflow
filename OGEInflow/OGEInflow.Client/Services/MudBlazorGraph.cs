using MudBlazor;
namespace OGEInflow.Services;

public class MudBlazorGraph
{
    public ChartOptions Options { get; set; }
    public List<ChartSeries> Series { get; set; }
    public string[] XAxisLabels { get; set; }
    public Dictionary<string, int> EventCounts { get; set; }

    public MudBlazorGraph(ChartOptions options, string[] xAxisLabels, Dictionary<string, int> eventCounts, List<ChartSeries> series)
    {
        Options = options;
        XAxisLabels = xAxisLabels;
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
