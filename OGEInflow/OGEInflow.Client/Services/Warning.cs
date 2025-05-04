namespace OGEInflow.Client.Services;

public class Warning
{
    public string WarningID { get; set; }
    public WarningType Type { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string PersonID { get; set; }
    public string ReaderId { get; set; }
    public WarningSeverity Severity { get; set; }
}

public enum WarningType
{
    DoubleScan,
    HighReaderUsage,
    OutsideUseHours
}

public enum WarningSeverity
{
    Low,
    Medium,
    High
}
