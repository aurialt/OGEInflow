namespace OGEInflow.Services;

public class ReaderEvent
{
    public string EventTime { get; set; }
    public string Location { get; set; }
    public string ReaderDesc { get; set; }
    public string ID { get; set; }
    public string DEVID { get; set; }
    public string MACHINE { get; set; }
    
    //To be accessed by multiple files 
    public static List<ReaderEvent> readerEventsList = new List<ReaderEvent>();
    
    /* Dictionaries dependent on readerEvent List*/
    private static Dictionary<string, List<ReaderEvent>> dayOfWeekReaderEventsDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> readerIdDict = new Dictionary<string, List<ReaderEvent>>();
    //My notes: initially empty, but scan events either create or append to the dictionary.


    public static Dictionary<string, List<ReaderEvent>> DayOfWeekReaderEventsDict
    {
        get { return dayOfWeekReaderEventsDict; }
    }

    public static Dictionary<string, List<ReaderEvent>> ReaderIdDict
    {
        get { return readerIdDict; } 
    }
        
    
    public ReaderEvent(string eventTime, string location, string readerDesc, string id, string devid, string machine)
    {
        EventTime = eventTime;
        Location = location;
        ReaderDesc = readerDesc;
        ID = id;
        DEVID = devid;
        MACHINE = machine;
    }

    public static void GenerateDictionaries()
    {
        GenerateReaderIdDict(readerEventsList);
        GenerateDayOfWeekReaderEvents(readerEventsList);
    }
    
    private static void GenerateReaderIdDict(List<ReaderEvent> eventsList)
    {

        foreach (var eventItem in eventsList)
        {
            string readerID = $"{eventItem.DEVID}-{eventItem.MACHINE}";
            
            if (!readerIdDict.ContainsKey(readerID))
            {
                readerIdDict[readerID] = new List<ReaderEvent> { eventItem };
            }
            else
            {
                readerIdDict[readerID].Add(eventItem);
            }
        }
    }
    
    private static void GenerateDayOfWeekReaderEvents(List<ReaderEvent> eventsList)
    {
        foreach (var eventItem in eventsList)
        {
            string date = eventItem.EventTime;
            DateTime dateTime = DateTime.Parse(date);
            string dayOfWeek = dateTime.DayOfWeek.ToString();

            if (!dayOfWeekReaderEventsDict.ContainsKey(dayOfWeek))
            {
                dayOfWeekReaderEventsDict[dayOfWeek] = new List<ReaderEvent>();
            }
            else
            {
                dayOfWeekReaderEventsDict[dayOfWeek].Add(eventItem);
            }
        }
    }
}