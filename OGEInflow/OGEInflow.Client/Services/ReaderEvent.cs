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
    public static Dictionary<string, List<ReaderEvent>> ListDayOfWeekReaderEvents = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> readerIdDict = new Dictionary<string, List<ReaderEvent>>();
    //My notes: initially empty, but scan events either create or append to the dictionary.


    // public List<ReaderEvent> ReaderEventsList
    // {
    //     get
    //     {
    //         readerEventsList
    //     }
    // }
    //
    
    public ReaderEvent(string eventTime, string location, string readerDesc, string id, string devid, string machine)
    {
        EventTime = eventTime;
        Location = location;
        ReaderDesc = readerDesc;
        ID = id;
        DEVID = devid;
        MACHINE = machine;
    }
    
    

    public static void GenerateReaderIdDict(List<ReaderEvent> eventsList)
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
    
    public static void GenerateDayOfWeekReaderEvents(List<ReaderEvent> eventsList)
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
}