using Microsoft.AspNetCore.Components;
using OGEInflow.Services;

namespace OGEInflow.Client.Pages;

public partial class Warnings : ComponentBase
{
    private List<(string, ReaderEvent)> DoubleScanList = new();

    private void addDoubleScanWarning(ReaderEvent re)
    {
        
    }
}