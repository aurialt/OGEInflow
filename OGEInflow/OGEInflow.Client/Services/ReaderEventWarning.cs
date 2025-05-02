using OGEInflow.Services.WarningTypes;

namespace OGEInflow.Services;


public static class ReaderEventWarning
{
    /*
     *Plan:
     * Takes ReaderEvent dictionaries and returns a list of warnings
     * 
     * 
     */ 
    
    
    public static List<ReaderEvent> DoubleScansReaderEvents = new();
    public static List<WarningTypes.WarningTypes> HighReaderUsageReaderEvents = new();

    
    public static void ClearWarnings()
    {
        DoubleScansReaderEvents.Clear();
        HighReaderUsageReaderEvents.Clear();
    }
    
    public static void CheckDoubleScans(Dictionary<string, List<ReaderEvent>> sourceData)
    {
        Console.WriteLine("CheckDoubleScans called...");

        var doubleScans = sourceData
            .SelectMany(entry =>
                entry.Value
                    .OrderBy(ev => ev.EventTime)
                    .Zip(entry.Value.OrderBy(ev => ev.EventTime).Skip(1), (a, b) => new { a, b })
                    .Where(pair => pair.a.EventTime == pair.b.EventTime)
                    .Select(entry => entry.a)
            )
            .ToList();
        
        DoubleScansReaderEvents.AddRange(doubleScans);
        Console.WriteLine("Amount of Double Scans: " + DoubleScansReaderEvents.Count);
    }
    
    public static void CheckTooManyReaderScans(Dictionary<string, List<ReaderEvent>> sourceData, int threshold)
    {
        Console.WriteLine("CheckTooManyReaderScans called...");
        var warnings = sourceData
            .Where(entry => entry.Value.Count > threshold)
            .Select(entry =>
            {
                var sample = entry.Value.FirstOrDefault();
                return new WarningTypes.WarningTypes
                {
                    ReaderEvent = sample,
                    ScanCount = entry.Value.Count
                };
            })
            .ToList();
        
        HighReaderUsageReaderEvents.AddRange(warnings);
        Console.WriteLine("Amount of Reader Scans: " + HighReaderUsageReaderEvents.Count);
    }

    
}