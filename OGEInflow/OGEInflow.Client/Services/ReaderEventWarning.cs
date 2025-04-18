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
    public static List<(string ReaderID, int count)> HighReaderUsageWarningsList = new List<(string, int)>();

    
    public static void ClearWarnings()
    {
        HighReaderUsageWarningsList.Clear();
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
                    .Select(pair => (PersonID: entry.Key, time1: pair.a.EventTime, time2: pair.b.EventTime))
            )
            .ToList();
        
        // foreach (var scan in doubleScans)
        // {
        //     Console.WriteLine($"Double Scan detected: PersonID: {scan.PersonID}, Time: {scan.time1}");
        // }
        
        DoubleScanWarningsList.AddRange(doubleScans);
        Console.WriteLine("Amount of Double Scans: " + DoubleScanWarningsList.Count);
    }
    
    public static void CheckTooManyReaderScans(Dictionary<string, List<ReaderEvent>> sourceData, int threshold)
    {
        var filteredDict = sourceData
            .Where(entry => entry.Value.Count > threshold)
            .ToList();
        Console.WriteLine("CheckTooManyReaderScans called...");
        foreach (var entry in filteredDict)
        {
            Console.WriteLine($"ReaderID: {entry.Key} Event Count: {entry.Value.Count}");
            HighReaderUsageWarningsList.Add((entry.Key, entry.Value.Count));
        }
            
    }
    
}