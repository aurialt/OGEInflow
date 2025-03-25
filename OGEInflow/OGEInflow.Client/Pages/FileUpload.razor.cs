using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace OGEInflow.Client.Pages;


public partial class FileUpload : ComponentBase
{
    public static string FileName { get; set; } = "";
    public static long FileSize { get; set; }
    public static string FileType { get; set; } = "";
    public static DateTimeOffset LastModified { get; set; }
    public static string ErrorMessage { get; set; } = "";
    
    const int MAX_FILESIZE = 5000 * 1024; // 2 MB

    // Handles multiple files
    public static async Task FileUploaded(InputFileChangeEventArgs e)
    {
        var browserFiles = e.GetMultipleFiles();

        foreach (var browserFile in browserFiles)
        {
            if (browserFile != null)
            {
                FileSize = browserFile.Size;
                FileType = browserFile.ContentType;
                FileName = browserFile.Name;
                LastModified = browserFile.LastModified;

                try
                {
                    var fileStream = browserFile.OpenReadStream(MAX_FILESIZE);

                    var projectRoot = Directory.GetCurrentDirectory();
                    var uploadFolder = Path.Combine(projectRoot, "Files");
                    Directory.CreateDirectory(uploadFolder);
                    var targetFilePath = Path.Combine(uploadFolder, FileName); 

                    var destinationStream = new FileStream(targetFilePath, FileMode.Create);
                    await fileStream.CopyToAsync(destinationStream);
                    destinationStream.Close();

                    Console.WriteLine("File uploaded Name:" + FileName);
                    Console.WriteLine("File project root" + projectRoot);
                    Console.WriteLine("Target file path: " + targetFilePath);
                    
                    populateEvents(targetFilePath);
                    Console.WriteLine($"Lines in csv from {targetFilePath}: {readerEvents.Count}");

                    ProcessReaderEvents(readerEvents);
                    Console.WriteLine("ProcessReaderEvents called");
                    
                    DayOfWeekReaderEvents(readerEvents);
                    Console.WriteLine("DayOfWeekReaderEvents called");
                    
                    LoadReaderEventsLineGraph();
                    Console.WriteLine("LoadReaderEventsLineGraph called");
                    
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
            }
        }
    }
    
    
    /* ReaderEvent Section */
    
    private static List<ReaderEvent> readerEvents = new List<ReaderEvent>();

    public static void populateEvents(string file)
    {
        if (Path.Exists(file) && Path.GetExtension(file) == ".csv")
        {
            using (StreamReader sr = new StreamReader(file))
            {
                string str = sr.ReadLine();
                while ((str = sr.ReadLine()) != null) // Skips the first line since ReadLine was already Called
                {
                    List<string> fields = str.Split(',').ToList();
                    
                    if (fields.Count >= 6)
                    {
                        ReaderEvent re = new ReaderEvent(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5]);
                        readerEvents.Add(re);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid row: {str}");
                    }
                }
            }

            Console.WriteLine("Events populated");
            Console.WriteLine($"Total events: {readerEvents.Count}");
        }
        else
        {
            ErrorMessage = $"File {file} does not exist or is not a .csv file.";
        }
    }
    
    /*# 7 */
    private static Dictionary<string, List<ReaderEvent>> readerEventsDict = new Dictionary<string, List<ReaderEvent>>();
    //My notes: initially empty, but scan events either create or append to the dictionary.

    public static void ProcessReaderEvents(List<ReaderEvent> eventsList)
    {

        foreach (var eventItem in eventsList)
        {
            string readerID = $"{eventItem.DEVID}-{eventItem.MACHINE}";
            
            if (!readerEventsDict.ContainsKey(readerID))
            {
                readerEventsDict[readerID] = new List<ReaderEvent> { eventItem };
            }
            else
            {
                readerEventsDict[readerID].Add(eventItem);
            }
        }
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
    
    private static ChartOptions options = new ChartOptions();
    public static List<ChartSeries> Series;
    public static string[] XAxisLabels = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    private static Dictionary<string, int> eventCounts = new();

    protected override void OnInitialized()
    {
        // Call static method to load data
        LoadReaderEventsLineGraph();
    }

    public static void LoadReaderEventsLineGraph()
    {
        // options.YAxisFormat = "n0"; // Displays numbers normally

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