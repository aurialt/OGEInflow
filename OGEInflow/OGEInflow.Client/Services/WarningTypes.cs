namespace OGEInflow.Services.WarningTypes;

public class WarningTypes
{
    public ReaderEvent ReaderEvent { get; set; }
    public int ScanCount { get; set; }
}

public class DoubleScanWarning
{
    public ReaderEvent ReaderEvent1 { get; set; }
}