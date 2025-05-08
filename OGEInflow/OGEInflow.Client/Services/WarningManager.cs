using OGEInflow.Services;

namespace OGEInflow.Client.Services
{
    public class WarningManager
    {
        public static List<Warning> AllWarnings { get; private set; } = new();

        public static Dictionary<string, List<Warning>> DoubleScanWarnings { get; private set; } = new();       // by PersonID
        public static Dictionary<string, List<Warning>> PastReaderThresholdWarnings { get; private set; } = new();  // by ReaderId
        public static Dictionary<string, List<Warning>> NearReaderThresholdWarnings { get; private set; } = new();  // by ReaderId
        public static Dictionary<string, List<Warning>> PastPanelThresholdWarnings { get; private set; } = new();    // by MACHINE
        public static Dictionary<string, List<Warning>> NearPanelThresholdWarnings { get; private set; } = new();    // by MACHINE
        
        public static Dictionary<string, List<Warning>> ReaderDoubleScanThresholdWarnings { get; private set; } = new(); // by ReaderId
        public static Dictionary<string, List<Warning>> OutsideUseHourWarnings { get; private set; } = new();   // by PersonID
        
        
        //Flattened Lists
        public static IEnumerable<Warning> FlattenedDoubleScanWarnings => DoubleScanWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedPastReaderThresholdWarnings => PastReaderThresholdWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedNearReaderThresholdWarnings => NearReaderThresholdWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedPastPanelThresholdWarnings => PastPanelThresholdWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedNearPanelThresholdWarnings => NearPanelThresholdWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedReaderDoubleScanThresholdWarnings => ReaderDoubleScanThresholdWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedOutsideUseHourWarnings => OutsideUseHourWarnings.SelectMany(kv => kv.Value);

        public static void LoadWarnings()
        {
            GenerateDoubleScanWarnings(ReaderEvent.PersonIDDict);
            GenerateThresholdWarnings(ReaderEvent.ReaderIDDict, Settings.ReaderThreshold, WarningType.PastReaderThreshold);
            GenerateThresholdWarnings(ReaderEvent.ReaderIDDict, Settings.ReaderNearThreshold, WarningType.NearReaderThreshold);
            GenerateThresholdWarnings(ReaderEvent.MachineDict, Settings.PanelThreshold, WarningType.PastPanelThreshold);
            GenerateThresholdWarnings(ReaderEvent.MachineDict, Settings.PanelNearThreshold, WarningType.NearPanelThreshold);
            GenerateReaderDoubleScanThresholdWarnings(ReaderEvent.ReaderIDDict, Settings.ReaderDoubleScanThreshold);
            if (!Settings.isOpenAllDay)
                GenerateOutsideUseHourWarnings(ReaderEvent.PersonIDDict, Settings.OpeningTime, Settings.ClosingTime);
        }
        
        public static void ClearWarnings()
        {
            AllWarnings.Clear();
            DoubleScanWarnings.Clear();
            PastReaderThresholdWarnings.Clear();
            NearReaderThresholdWarnings.Clear();
            PastPanelThresholdWarnings.Clear();
            NearPanelThresholdWarnings.Clear();
            ReaderDoubleScanThresholdWarnings.Clear();
            OutsideUseHourWarnings.Clear();
        }

        private static void AddToDictionary(Dictionary<string, List<Warning>> dict, string key, Warning warning)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<Warning>();

            dict[key].Add(warning);
        }

        private static void AddWarning(Warning warning)
        {
            AllWarnings.Add(warning);

            switch (warning.Type)
            {
                case WarningType.DoubleScan:
                    if (!string.IsNullOrEmpty(warning.PersonID))
                        AddToDictionary(DoubleScanWarnings, warning.PersonID, warning);
                    break;

                case WarningType.PastReaderThreshold:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(PastReaderThresholdWarnings, warning.ReaderId, warning);
                    break;
                
                case WarningType.NearReaderThreshold:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(NearReaderThresholdWarnings, warning.ReaderId, warning);
                    break;
                
                case WarningType.PastPanelThreshold:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(PastPanelThresholdWarnings, warning.ReaderId, warning);
                    break;
                
                case WarningType.NearPanelThreshold:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(NearPanelThresholdWarnings, warning.ReaderId, warning);
                    break;
                
                case WarningType.ReaderDoubleScanThreshold:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(ReaderDoubleScanThresholdWarnings, warning.ReaderId, warning);
                    break;

                case WarningType.OutsideUseHours:
                    if (!string.IsNullOrEmpty(warning.PersonID))
                        AddToDictionary(OutsideUseHourWarnings, warning.PersonID, warning);
                    break;
            }
        }

        public static void GenerateDoubleScanWarnings(Dictionary<string, List<ReaderEvent>> sourceData)
        {
            foreach (var (personId, events) in sourceData)
            {
                var ordered = events.OrderBy(ev => ev.EventTime).ToList();

                for (int i = 1; i < ordered.Count; i++)
                {
                    var a = ordered[i - 1];
                    var b = ordered[i];
                    var timeA = a.EventTime;
                    var timeB = b.EventTime;

                    if (a.DEVID == b.DEVID && (timeB - timeA).TotalSeconds <= 10)
                    {
                        AddWarning(new Warning
                        {
                            WarningID = Guid.NewGuid().ToString(),
                            Type = WarningType.DoubleScan,
                            Severity = WarningSeverity.Medium,
                            Timestamp = timeB,
                            PersonID = b.ID,
                            ReaderId = b.ReaderID,
                            Message = $"Double scan at reader {b.DEVID} within 10 seconds."
                        });
                    }
                }
            }
        }

        public static void GenerateThresholdWarnings(Dictionary<string, List<ReaderEvent>> sourceData, int threshold,
            WarningType type)
        {
            //For future reference, could make Past Calls dependent on near to make it more efficient & better performance
            foreach (var (readerId, events) in sourceData)
            {
                if (events.Count > threshold)
                {
                    AddWarning(new Warning
                    {
                        WarningID = Guid.NewGuid().ToString(),
                        Type = type,
                        Severity = WarningSeverity.High,
                        Timestamp = DateTime.Now,
                        PersonID = "", // N/A
                        ReaderId = readerId,
                        Message = $"Reader {readerId} had {events.Count} scans (>{threshold})."
                    });
                }
            }
        }
        
        public static void GenerateReaderDoubleScanThresholdWarnings(Dictionary<string, List<ReaderEvent>> sourceData, int threshold)
        {
            foreach (var (readerId, events) in sourceData)
            {
                if (events.Count > threshold)
                {
                    AddWarning(new Warning
                    {
                        WarningID = Guid.NewGuid().ToString(),
                        Type = WarningType.ReaderDoubleScanThreshold,
                        Severity = WarningSeverity.Medium,
                        Timestamp = DateTime.Now,
                        PersonID = "", // N/A
                        ReaderId = readerId,
                        Message = $"Reader {readerId} had {events.Count} scans (>{threshold}). Check for issues in reader"
                    });
                }
            }
        }
        

        public static void GenerateOutsideUseHourWarnings(Dictionary<string, List<ReaderEvent>> sourceData, int startHour, int endHour)
        {
            foreach (var (personId, events) in sourceData)
            {
                foreach (var ev in events)
                {
                    var hour = ev.EventTime.Hour;
                    if (hour < startHour || hour >= endHour)
                    {
                        AddWarning(new Warning
                        {
                            WarningID = Guid.NewGuid().ToString(),
                            Type = WarningType.OutsideUseHours,
                            Severity = WarningSeverity.Low,
                            Timestamp = ev.EventTime,
                            PersonID = ev.ID,
                            ReaderId = ev.DEVID,
                            Message = $"Use at {ev.EventTime} outside allowed hours ({startHour}–{endHour})."
                        });
                    }
                }
            }
        }
    }
}
