namespace OGEInflow.Services;

public class ReaderEvent
{
    public string EventTime { get; set; }
    public string Location { get; set; }
    public string ReaderDesc { get; set; }
    public string ID { get; set; }
    public string DEVID { get; set; }
    public string MACHINE { get; set; }
    
    public static DateTime MinDate { get; set; } 
    public static DateTime MaxDate { get; set; }
    
    //To be accessed by multiple files 
    public static List<ReaderEvent> readerEventsList = new List<ReaderEvent>();
    
    /* Dictionaries dependent on readerEvent List*/
    private static Dictionary<string, List<ReaderEvent>> dayOfWeekReaderEventsDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> personIDDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> readerIdDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> readerDescDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> devIdDict = new Dictionary<string, List<ReaderEvent>>();
    private static Dictionary<string, List<ReaderEvent>> machineDict = new Dictionary<string, List<ReaderEvent>>();
    //My notes: initially empty, but scan events either create or append to the dictionary.


    /* Property access to private dictionaries  */
    public static Dictionary<string, List<ReaderEvent>> DayOfWeekReaderEventsDict { get { return dayOfWeekReaderEventsDict; } }

    public static Dictionary<string, List<ReaderEvent>> PersonIDDict { get { return personIDDict; } }

    public static Dictionary<string, List<ReaderEvent>> ReaderIDDict { get { return readerIdDict; } }

    public static Dictionary<string, List<ReaderEvent>> ReaderDescDict { get { return readerDescDict; } }
    
    public static Dictionary<string, List<ReaderEvent>> DevIdDict { get { return devIdDict; } }
    
    public static Dictionary<string, List<ReaderEvent>> MachineDict { get { return machineDict; } }
        
    
    public ReaderEvent(string eventTime, string location, string readerDesc, string id, string devid, string machine)
    {
        EventTime = eventTime;
        Location = location;
        ReaderDesc = readerDesc;
        ID = id;
        DEVID = devid;
        MACHINE = machine;
    }
    
    
    /* Generate Dictionaries Implentations */
    public static void GenerateDictionaries()
    {
        GenerateDayOfWeekReaderEventsDict(readerEventsList);
        GeneratePersonIDDict(readerEventsList);
        GenerateReaderIdDict(readerEventsList);
        GenerateReaderDescDict(readerEventsList);
        GenerateDevIdDict(readerEventsList);
        GenerateMachineDict(readerEventsList);
    }

    private static void GeneratePersonIDDict(List<ReaderEvent> eventList)
    {
        personIDDict.Clear();
        foreach (var eventItem in eventList)
        {
            AddEventToDict(personIDDict, eventItem.ID, eventItem);
            // Console.WriteLine("PersonID: " + eventItem.ID);
        }
        Console.WriteLine("GeneratePersonIDDict Count: " + personIDDict.Count);
    }
    
    private static void GenerateReaderIdDict(List<ReaderEvent> eventsList)
    {
        readerIdDict.Clear();
        foreach (var eventItem in eventsList)
        {
            string readerID = $"{eventItem.DEVID}-{eventItem.MACHINE}";
            AddEventToDict(readerIdDict, readerID, eventItem);
        }
        Console.WriteLine("GenerateReaderIdDict Count: " + readerIdDict.Count);
    }
    
    private static void GenerateDayOfWeekReaderEventsDict(List<ReaderEvent> eventsList)
    {
        dayOfWeekReaderEventsDict.Clear();
        foreach (var eventItem in eventsList)
        {
            string date = eventItem.EventTime;
            DateTime dateTime = DateTime.Parse(date);
            string dayOfWeek = dateTime.DayOfWeek.ToString();

            AddEventToDict(dayOfWeekReaderEventsDict, dayOfWeek, eventItem);
        }
    }

    private static void GenerateReaderDescDict(List<ReaderEvent> eventsList)
    {
        readerDescDict.Clear();
        foreach (var eventItem in eventsList)
        {
            AddEventToDict(readerDescDict, eventItem.ReaderDesc, eventItem);
        }
        Console.WriteLine("GenerateReaderDescDict Count: " + readerDescDict.Count);
    }

    private static void GenerateDevIdDict(List<ReaderEvent> eventsList)
    {
        devIdDict.Clear();
        foreach (var eventItem in eventsList)
        {
            AddEventToDict(devIdDict, eventItem.DEVID, eventItem);
        }
        Console.WriteLine("GenerateDevIdDict Count: " + devIdDict.Count);
    }
    
    private static void GenerateMachineDict(List<ReaderEvent> eventsList)
    {
        machineDict.Clear();
        foreach (var eventItem in eventsList)
        {
            AddEventToDict(machineDict, eventItem.MACHINE, eventItem);
        }
        Console.WriteLine("GenerateMachineDict Count: " + machineDict.Count);
    }
    
    
    /* Helper Methods */
    private static void AddEventToDict(Dictionary<string, List<ReaderEvent>> targetDict, string key, ReaderEvent eventItem)
    {
        if (!targetDict.ContainsKey(key))
        {
            targetDict[key] = new List<ReaderEvent> { eventItem };
        }
        else
        {
            targetDict[key].Add(eventItem);
        }
    }
    
    

    /*Testing Code */
    public static void printDescList()
    {
        Console.WriteLine("printDescList : ===");
        foreach (var eventItem in readerDescDict.Values)
        {
            Console.WriteLine("printDescList - eventItem: " + eventItem.Count);
        }
    }
    
    
    
}