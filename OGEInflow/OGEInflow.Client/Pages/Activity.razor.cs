using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace OGEInflow.Client.Pages
{
    public partial class Activity : ComponentBase
    {
        /* Graphs Section */
        public static MudBlazorGraph ScanActivationsGraph,
            RankedPersonIDGraph,
            RankedReaderIDGraph,
            RankedMachineGraph;

        private static MudDatePicker StartPicker = new MudDatePicker();
        private static MudDatePicker EndPicker = new MudDatePicker();

        private static DateTime? MinDate => ReaderEvent.MinDate;
        private static DateTime? MaxDate => ReaderEvent.MaxDate;

        private static int dateRange;
        
        

        private static DateTime? StartDate { get; set; }
        private static DateTime? EndDate { get; set; }
        private static bool _autoClose = false;

        private static List<ReaderEvent> filteredReaderEvents;
        
        [Inject]
        NavigationManager NavigationManager { get; set; }
        
        /* Count variables */
        private static int PersonCountFromFilter { get; set; }

        // private int avgUniquePeople;
        private int AvgUniquePeople
        {
            get
            {
                if (dateRange != 0)
                {
                    return PersonCountFromFilter / dateRange;
                }
                return PersonCountFromFilter;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadGraphsAsync();
        }
        
        private bool _firstRender = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Used to set initial dates after rendering
            if (firstRender && MinDate.HasValue && MaxDate.HasValue)
            {
                StartPicker.Date = MinDate;
                
                EndPicker.Date = MaxDate;
                
                _firstRender = false;
            }
        }
        

        public static void InitializeDateBounds(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public void RefreshGraphs()
        {
            if (ReaderEvent.readerEventsList == null) return;

            StartDate = StartPicker.Date;
            EndDate = EndPicker.Date;
            
            LoadGraphsAsync();
            StateHasChanged();
            Console.WriteLine(" (LoadGraphs) Date Range: " + dateRange);
        }

        private static async Task LoadGraphsAsync()
        {
            if (StartDate != null && EndDate != null)
            {
                TimeSpan diff = EndDate.Value - StartDate.Value;
                dateRange = (int)diff.TotalDays;
            }

            FilterReaderEvents();

            await Task.WhenAll(
                createScanActivationGraphAsync(),
                createRankedPersonIDGraphAsync(),
                createRankedReaderIDGraphAsync(),
                createRankedMachineGraphAsync()
            );
        }

        private static void FilterReaderEvents()
        {
            var effectiveStart = StartDate.Value.Date;
            var effectiveEnd = EndDate.Value.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999

            filteredReaderEvents = ReaderEvent.readerEventsList
                .Where(re =>
                    re.EventTime >= effectiveStart && re.EventTime <= effectiveEnd)
                .ToList();
        }

        private static Dictionary<string, List<ReaderEvent>> GroupEventsByKey(Func<ReaderEvent, string> keySelector)
        {
            return filteredReaderEvents
                .GroupBy(keySelector)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        
        // private static Task createScanActivationGraphAsync()
        // {
        //     ChartOptions options = new ChartOptions();
        //
        //     string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        //
        //     var sortedDayOfWeekReaderEvents = ReaderEvent.DayOfWeekReaderEventsDict
        //         .Where(entry => daysOfWeek.Contains(entry.Key))
        //         .OrderBy(entry => Array.IndexOf(daysOfWeek, entry.Key))
        //         .ToList();
        //
        //     List<ChartSeries> series = new List<ChartSeries>
        //     {
        //         new()
        //         {
        //             Name = "Scan Activations per Day",
        //             Data = sortedDayOfWeekReaderEvents
        //                 .Select(entry => (double)entry.Value.Count)
        //                 .ToArray()
        //         }
        //     };
        //
        //     ScanActivationsGraph = MudBlazorGraph.CreateGraph(series, ReaderEvent.DayOfWeekReaderEventsDict, daysOfWeek, options);
        //     return Task.CompletedTask;
        // }
        
        private static Task createScanActivationGraphAsync()
        {
            ChartOptions options = new ChartOptions();
        
            string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
            // Parse the events once and group by the day of the week
            var dayOfWeekDict = filteredReaderEvents
                .Select(re =>
                {
                        return new { re, dayOfWeek = re.EventTime.DayOfWeek.ToString() };
                })
                .Where(x => x != null)  // Only keep valid events
                .GroupBy(x => x.dayOfWeek)
                .ToDictionary(g => g.Key, g => g.Select(x => x.re).ToList());
        
            // Sort based on the order of the days
            var sortedDayOfWeekReaderEvents = dayOfWeekDict
                .Where(entry => daysOfWeek.Contains(entry.Key))
                .OrderBy(entry => Array.IndexOf(daysOfWeek, entry.Key))
                .ToList();
        
            // Prepare chart series data
            List<ChartSeries> series = new List<ChartSeries>
            {
                new()
                {
                    Name = "Total Scans per Day",
                    Data = sortedDayOfWeekReaderEvents
                        .Select(entry => (double)entry.Value.Count)
                        .ToArray()
                }
            };
        
            // Create the graph
            ScanActivationsGraph = MudBlazorGraph.CreateGraph(series, dayOfWeekDict, daysOfWeek, options);
            return Task.CompletedTask;
        }


        private static Task createRankedPersonIDGraphAsync()
        {
            var personIDDict = GroupEventsByKey(re => re.ID);
            var rankedPersonIDGraph = GetTopRanked(personIDDict, 5);
            
            //Updates PersonCounter
            PersonCountFromFilter = personIDDict.Count;

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
            return Task.CompletedTask;
        }

        private static Task createRankedReaderIDGraphAsync()
        {
            var readerIDDict = GroupEventsByKey(re => re.ReaderID);
            var rankedReaderIDGraph = GetTopRanked(readerIDDict, 5);

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
            return Task.CompletedTask;
        }

        private static Task createRankedMachineGraphAsync()
        {
            var machineDict = GroupEventsByKey(re => re.MACHINE);
            var rankedMachineGraph = GetTopRanked(machineDict, 5);

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
            return Task.CompletedTask;
        }

        private static Dictionary<string, List<ReaderEvent>> GetTopRanked(Dictionary<string, List<ReaderEvent>> inputDict, int topCount)
        {
            return inputDict
                .Where(entry => entry.Value.Any())
                .OrderByDescending(entry => entry.Value.Count)
                .Take(topCount)
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }
}
