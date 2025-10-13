namespace DeltaDaily.Components.Records;

public sealed class ProjectRecord
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Phase { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public string Payload { get; set; } = default!;
}
