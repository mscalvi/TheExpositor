namespace ContaJunsta.Models;
public class BillModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = "";
    public string Description { get; set; } = "";
    public int Cents { get; set; } // +despesa / -ganho
    public string ResponsiblePersonId { get; set; } = "";
    public List<string> ParticipantIds { get; set; } = new();
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
}
