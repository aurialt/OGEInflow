using MudBlazor;
namespace OGEInflow.Services;

public class MudBlazorGraph
{
    public ChartOptions Options {get; set;}
    public List<ChartSeries> Series { get; set; }
    public string[] XAxisLabels {get; set;}

    public Dictionary<string, int> EventCounts {get; set;}

    public MudBlazorGraph(ChartOptions options, List<ChartSeries> series, string[] xAxisLabels, Dictionary<string, int> eventCounts)
    {
        Options = options;
        Series = series;
        XAxisLabels = xAxisLabels;
        EventCounts = eventCounts;
    }
}