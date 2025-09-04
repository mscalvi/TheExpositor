namespace ContaJunsta.Models;
public class PersonModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = "";
    public string Name { get; set; } = "";
}
