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
    
    
    public static List<(string PersonID, string time1, string time2)> DoubleScanWarningsList = new();
    // public static List<(string ReaderID, int count)> HighReaderUsageWarningsList = new List<(string, int)>();
    
    public static List<(ReaderEvent re1, ReaderEvent re2)> DoubleScansReaderEvents = new List<(ReaderEvent, ReaderEvent)>();
    public static List<HighReaderUsageWarning> HighReaderUsageReaderEvents = new();

    
    public static void ClearWarnings()
    {
        // HighReaderUsageWarningsList.Clear();
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
                    .Select(pair => (pair.a, pair.b)) // Convert anonymous object to tuple
                    // .Select(pair => (PersonID: entry.Key, time1: pair.a.EventTime, time2: pair.b.EventTime))
            )
            .ToList();
        
        // foreach (var scan in doubleScans)
        // {
        //     Console.WriteLine($"Double Scan detected: PersonID: {scan.PersonID}, Time: {scan.time1}");
        // }
        
        // DoubleScanWarningsList.AddRange(doubleScans);
        DoubleScansReaderEvents.AddRange(doubleScans);
        Console.WriteLine("Amount of Double Scans: " + DoubleScanWarningsList.Count);
    }
    
    public static void CheckTooManyReaderScans(Dictionary<string, List<ReaderEvent>> sourceData, int threshold)
    {
        Console.WriteLine("CheckTooManyReaderScans called...");
        var warnings = sourceData
            .Where(entry => entry.Value.Count > threshold)
            .Select(entry =>
            {
                var sample = entry.Value.FirstOrDefault();
                return new HighReaderUsageWarning
                {
                    ReaderEvent = sample,
                    ScanCount = entry.Value.Count
                };
            })
            .ToList();
        
        HighReaderUsageReaderEvents.AddRange(warnings);
        Console.WriteLine("Amount of Reader Scans: " + HighReaderUsageReaderEvents.Count);
        // HighReaderUsageWarningsList.AddRange(warnings);
    }

    
}