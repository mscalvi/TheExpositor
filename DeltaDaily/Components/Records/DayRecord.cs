namespace DeltaDaily.Components.Records;

public sealed class DayRecord
{
    public string Id { get; set; } = default!;
    public string Date { get; set; } = default!;
    public bool Closed { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public string Payload { get; set; } = "";
}
