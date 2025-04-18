namespace OGEInflow.Services;


public static class ReaderEventWarning
{
    /*
     *Plan:
     * Takes ReaderEvent dictionaries and returns a list of warnings
     * 
     * 
     */ 

    
    public static void CheckTooManyReaderScans(int threshold)
    {
        var filteredDict = ReaderEvent.ReaderIDDict
            .Where(entry => entry.Value.Count > threshold)
            .ToList();
        Console.WriteLine("CheckTooManyReaderScans called...");
        foreach (var entry in filteredDict)
        {
            Console.WriteLine($"ReaderID: {entry.Key} Event Count: {entry.Value.Count}");
        }
            
    }
    
}