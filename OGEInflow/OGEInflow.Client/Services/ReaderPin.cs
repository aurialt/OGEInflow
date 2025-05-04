namespace OGEInflow.Client.Services;

public class ReaderPin
{
    public string PinReaderID { get; set; }
    public string PinReaderDesc {get; set;}
    public int PinScanCount { get; set; }
    public ReaderPin(string ReaderID, string ReaderDesc, int scanCount)
    {
        PinReaderID = ReaderID;
        PinReaderDesc = ReaderDesc;
        PinScanCount = scanCount;
    }
}