using Microsoft.AspNetCore.Components;
using OGEInflow.Services;
using OGEInflow.Services.WarningTypes;

namespace OGEInflow.Client.Pages;

public partial class Warnings : ComponentBase
{
    
    /* HighReaderUsageWarning MudDataGrid */
    private string _searchString;

    private Func<WarningTypes, bool> quickFilterHighUsageScans => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        return
            (x.ReaderEvent.ReaderID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderEvent.Location?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderEvent.ReaderDesc?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.ScanCount.ToString().Contains(_searchString);
    };
    
    private Func<ReaderEvent, bool> quickFilterDoubleScan => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        return
            (x.ReaderID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.Location?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderDesc?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.EventTime?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false);
    };
    
    
}