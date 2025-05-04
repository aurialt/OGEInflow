using Microsoft.AspNetCore.Components;
using OGEInflow.Client.Services;
namespace OGEInflow.Client.Pages;

public partial class Warnings : ComponentBase
{
    

// Quick filter for Double Scan (based on Warning properties)
    private string _searchStringDoubleScans;
    private Func<Warning, bool> quickFilterDoubleScan => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringDoubleScans))
            return true;

        return
            (x.ReaderId?.Contains(_searchStringDoubleScans, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.Message?.Contains(_searchStringDoubleScans, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.Timestamp.ToString().Contains(_searchStringDoubleScans);
    };

// Quick filter for High Reader Usage
    private string _searchStringHighUsageScans;
    private Func<Warning, bool> quickFilterHighUsageScans => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchStringHighUsageScans))
            return true;

        return
            (x.ReaderId?.Contains(_searchStringHighUsageScans, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.Message?.Contains(_searchStringHighUsageScans, StringComparison.OrdinalIgnoreCase) ?? false) ||
            x.Timestamp.ToString().Contains(_searchStringHighUsageScans);
    };
}