using Microsoft.AspNetCore.Components;
using OGEInflow.Services;
using OGEInflow.Services.WarningTypes;

namespace OGEInflow.Client.Pages;

public partial class Warnings : ComponentBase
{
    private List<(string, ReaderEvent)> DoubleScanList = new();

    private void addDoubleScanWarning(ReaderEvent re)
    {
        
    }
    
    /* HighReaderUsageWarning MudDataGrid */
    private string _searchString;

    private Func<HighReaderUsageWarning, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        return
            (x.ReaderEvent.ReaderID?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderEvent.Location?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.ReaderEvent.ReaderDesc?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.ScanCount.ToString().Contains(_searchString);
    };
    
    
}