using Microsoft.AspNetCore.Components;
using OGEInflow.Services;

namespace OGEInflow.Client.Pages;

public class ReaderScanSummary
{
    public string ReaderID { get; set; }
    public string Location { get; set; }
    public string ReaderDesc { get; set; }
    public int ScanCount { get; set; }
}

public class MachineScanSummary
{
    public string MachineID { get; set; }
    public string Location { get; set; }
    public int ScanCount { get; set; }
}

public class PersonScanSummary
{
    public string PersonID { get; set; }
    public string ReaderDesc { get; set; }
    public int ScanCount { get; set; }
}



public partial class CSVData : ComponentBase
{
    private List<ReaderScanSummary> readerSummaries => ReaderEvent.ReaderIDDict.Select(kvp =>
    {
        var firstEvent = kvp.Value.FirstOrDefault(); // to extract Location and Description
        return new ReaderScanSummary
        {
            ReaderID = kvp.Key,
            Location = firstEvent?.Location,
            ReaderDesc = firstEvent?.ReaderDesc,
            ScanCount = kvp.Value.Count
        };
    }).ToList();

    private string _searchString;
    
    private List<MachineScanSummary> machineSummaries => ReaderEvent.MachineDict.Select(kvp =>
    {
        var firstEvent = kvp.Value.FirstOrDefault();
        return new MachineScanSummary
        {
            MachineID = kvp.Key,
            Location = firstEvent?.Location, //Need to fix for different locations
            ScanCount = kvp.Value.Count
        };
    }).ToList();

    private string _searchStringMachine;
    
    
    private List<PersonScanSummary> personSummaries => ReaderEvent.PersonIDDict.Select(kvp =>
    {
        // Find the most common location
        var mostCommonReaderDesc = kvp.Value
            .GroupBy(x => x.ReaderDesc)  // Group by the Location property
            .OrderByDescending(g => g.Count())  // Order by the count of each group
            .FirstOrDefault()?.Key;  // Take the first group (most frequent) and get the location

        return new PersonScanSummary
        {
            PersonID = kvp.Key,
            ReaderDesc = mostCommonReaderDesc ?? "Unknown",  // Handle null if no locations are found
            ScanCount = kvp.Value.Count
        };
    }).ToList();


    private string _searchStringPerson;

}