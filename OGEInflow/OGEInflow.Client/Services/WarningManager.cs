using OGEInflow.Services;

namespace OGEInflow.Client.Services
{
    public class WarningManager
    {
        public static List<Warning> AllWarnings { get; private set; } = new();

        public static Dictionary<string, List<Warning>> DoubleScanWarnings { get; private set; } = new();       // by PersonID
        public static Dictionary<string, List<Warning>> HighReaderUsageWarnings { get; private set; } = new();  // by ReaderId
        public static Dictionary<string, List<Warning>> OutsideUseHourWarnings { get; private set; } = new();   // by PersonID
        
        
        //Flattened Lists
        public static IEnumerable<Warning> FlattenedDoubleScanWarnings => DoubleScanWarnings.SelectMany(kv => kv.Value);
        public static IEnumerable<Warning> FlattenedHighReaderUsageWarnings => WarningManager.HighReaderUsageWarnings.SelectMany(kv => kv.Value);

        public static void LoadWarnings()
        {
            GenerateDoubleScanWarnings(ReaderEvent.PersonIDDict);
            GenerateHighUsageWarnings(ReaderEvent.ReaderIDDict, Settings.ReaderThreshold);
            GenerateOutsideUseHourWarnings(ReaderEvent.PersonIDDict);
        }
        
        public static void ClearWarnings()
        {
            AllWarnings.Clear();
            DoubleScanWarnings.Clear();
            HighReaderUsageWarnings.Clear();
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

                case WarningType.HighReaderUsage:
                    if (!string.IsNullOrEmpty(warning.ReaderId))
                        AddToDictionary(HighReaderUsageWarnings, warning.ReaderId, warning);
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
                            ReaderId = b.DEVID,
                            Message = $"Double scan at reader {b.DEVID} within 10 seconds."
                        });
                    }
                }
            }
        }

        public static void GenerateHighUsageWarnings(Dictionary<string, List<ReaderEvent>> sourceData, int threshold = 50)
        {
            foreach (var (readerId, events) in sourceData)
            {
                if (events.Count > threshold)
                {
                    AddWarning(new Warning
                    {
                        WarningID = Guid.NewGuid().ToString(),
                        Type = WarningType.HighReaderUsage,
                        Severity = WarningSeverity.High,
                        Timestamp = DateTime.Now,
                        PersonID = "", // N/A
                        ReaderId = readerId,
                        Message = $"Reader {readerId} had {events.Count} scans (>{threshold})."
                    });
                }
            }
        }

        public static void GenerateOutsideUseHourWarnings(Dictionary<string, List<ReaderEvent>> sourceData, int startHour = 6, int endHour = 20)
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
