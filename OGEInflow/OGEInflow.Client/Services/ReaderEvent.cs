namespace OGEInflow.Services
{
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

        public static List<ReaderEvent> readerEventsList = new List<ReaderEvent>();

        private static Dictionary<string, List<ReaderEvent>> dayOfWeekReaderEventsDict = new();
        private static Dictionary<string, List<ReaderEvent>> personIDDict = new();
        private static Dictionary<string, List<ReaderEvent>> readerIdDict = new();
        private static Dictionary<string, List<ReaderEvent>> readerDescDict = new();
        private static Dictionary<string, List<ReaderEvent>> devIdDict = new();
        private static Dictionary<string, List<ReaderEvent>> machineDict = new();

        public static Dictionary<string, List<ReaderEvent>> DayOfWeekReaderEventsDict => dayOfWeekReaderEventsDict;
        public static Dictionary<string, List<ReaderEvent>> PersonIDDict => personIDDict;
        public static Dictionary<string, List<ReaderEvent>> ReaderIDDict => readerIdDict;
        public static Dictionary<string, List<ReaderEvent>> ReaderDescDict => readerDescDict;
        public static Dictionary<string, List<ReaderEvent>> DevIdDict => devIdDict;
        public static Dictionary<string, List<ReaderEvent>> MachineDict => machineDict;

        public DateTime ParsedEventTime => DateTime.Parse(EventTime);  // Parsed once per object

        public string ReaderID => $"{DEVID}-{MACHINE}"; // Centralized string concat

        
        public ReaderEvent()
        {
            //For Testing
            EventTime = DateTime.MinValue.ToString();
            Location = "Default Location";
            ReaderDesc = "Default Reader Description";
            ID = "Default ID";
            DEVID = "Default DEV ID";
            MACHINE = "Default MACHINE";

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
            foreach (var e in eventList)
                AddEventToDict(personIDDict, e.ID, e);
            Console.WriteLine("GeneratePersonIDDict Count: " + personIDDict.Count);
        }

        private static void GenerateReaderIdDict(List<ReaderEvent> eventsList)
        {
            readerIdDict.Clear();
            foreach (var e in eventsList)
                AddEventToDict(readerIdDict, e.ReaderID, e);
            Console.WriteLine("GenerateReaderIdDict Count: " + readerIdDict.Count);
        }

        private static void GenerateDayOfWeekReaderEventsDict(List<ReaderEvent> eventsList)
        {
            dayOfWeekReaderEventsDict.Clear();
            foreach (var e in eventsList)
            {
                string dayOfWeek = e.ParsedEventTime.DayOfWeek.ToString();
                AddEventToDict(dayOfWeekReaderEventsDict, dayOfWeek, e);
            }
        }

        private static void GenerateReaderDescDict(List<ReaderEvent> eventsList)
        {
            readerDescDict.Clear();
            foreach (var e in eventsList)
                AddEventToDict(readerDescDict, e.ReaderDesc, e);
            Console.WriteLine("GenerateReaderDescDict Count: " + readerDescDict.Count);
        }

        private static void GenerateDevIdDict(List<ReaderEvent> eventsList)
        {
            devIdDict.Clear();
            foreach (var e in eventsList)
                AddEventToDict(devIdDict, e.DEVID, e);
            Console.WriteLine("GenerateDevIdDict Count: " + devIdDict.Count);
        }

        private static void GenerateMachineDict(List<ReaderEvent> eventsList)
        {
            machineDict.Clear();
            foreach (var e in eventsList)
                AddEventToDict(machineDict, e.MACHINE, e);
            Console.WriteLine("GenerateMachineDict Count: " + machineDict.Count);
        }

        private static void AddEventToDict(Dictionary<string, List<ReaderEvent>> targetDict, string key, ReaderEvent eventItem)
        {
            if (!targetDict.TryGetValue(key, out var list))
            {
                list = new List<ReaderEvent>();
                targetDict[key] = list;
            }
            list.Add(eventItem);
        }

        public static void printDescList()
        {
            Console.WriteLine("printDescList : ===");
            foreach (var item in readerDescDict.Values)
            {
                Console.WriteLine("printDescList - eventItem: " + item.Count);
            }
        }
    }
}
