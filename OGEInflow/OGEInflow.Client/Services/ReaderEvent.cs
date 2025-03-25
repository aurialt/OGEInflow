namespace OGEInflow.Services;

public class ReaderEvent
{
    public string EventTime { get; set; }
    public string Location { get; set; }
    public string ReaderDesc { get; set; }
    public string ID { get; set; }
    public string DEVID { get; set; }
    public string MACHINE { get; set; }
    
    public ReaderEvent(string eventTime, string location, string readerDesc, string id, string devid, string machine)
    {
        EventTime = eventTime;
        Location = location;
        ReaderDesc = readerDesc;
        ID = id;
        DEVID = devid;
        MACHINE = machine;
    }
}