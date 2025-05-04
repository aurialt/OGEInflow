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
    private List<ReaderEvent> readerEventsSummary => ReaderEvent.readerEventsList;
    
    private string _searchStringReaderEvent;

    private Func<ReaderEvent, bool> quickFilterReaderEvent => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringReaderEvent))
            return true;

        return
            (x.ReaderDesc?.Contains(_searchStringReaderEvent, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ID?.Contains(_searchStringReaderEvent, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderID?.Contains(_searchStringReaderEvent, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.EventTime.ToString("g").Contains(_searchStringReaderEvent, StringComparison.OrdinalIgnoreCase);
    };

    
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

    private string _searchStringReader;

    private Func<ReaderScanSummary, bool> quickFilterReaderSummary => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringReader))
            return true;

        return
            (x.ReaderID?.Contains(_searchStringReader, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.Location?.Contains(_searchStringReader, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderDesc?.Contains(_searchStringReader, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.ScanCount.ToString().Contains(_searchStringReader);
    };

    
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
    private Func<MachineScanSummary, bool> quickFilterMachineSummary => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringMachine))
            return true;

        return
            (x.MachineID?.Contains(_searchStringMachine, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.Location?.Contains(_searchStringMachine, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.ScanCount.ToString().Contains(_searchStringMachine);
    };

    
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
    private Func<PersonScanSummary, bool> quickFilterPersonSummary => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringPerson))
            return true;

        return
            (x.PersonID?.Contains(_searchStringPerson, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderDesc?.Contains(_searchStringPerson, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.ScanCount.ToString().Contains(_searchStringPerson);
    };


}