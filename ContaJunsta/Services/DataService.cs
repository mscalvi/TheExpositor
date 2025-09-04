using Microsoft.JSInterop;
using ContaJunsta.Models;

namespace ContaJunsta.Services;

public class DataService
{
    private readonly IJSRuntime _js;
    public DataService(IJSRuntime js) => _js = js;

    public async Task<string> CreateEventAsync(string name, IEnumerable<string> participantNames)
    {
        var evt = new EventModel { Name = name };
        await _js.InvokeVoidAsync("ContaJunstaDb.addEvent", evt);

        var persons = participantNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => new PersonModel { EventId = evt.Id, Name = n.Trim() })
            .ToList();

        if (persons.Count > 0)
            await _js.InvokeVoidAsync("ContaJunstaDb.addPersons", persons);

        return evt.Id;
    }

    public Task<List<EventModel>> GetAllEventsAsync(bool onlyOpen = true) =>
        _js.InvokeAsync<List<EventModel>>("ContaJunstaDb.getAllEvents", onlyOpen).AsTask();

    public Task<List<PersonModel>> GetPersonsByEventAsync(string eventId) =>
        _js.InvokeAsync<List<PersonModel>>("ContaJunstaDb.getPersonsByEvent", eventId).AsTask();

    public Task AddBillAsync(BillModel bill) =>
        _js.InvokeVoidAsync("ContaJunstaDb.addBill", bill).AsTask();

    public Task<List<BillModel>> GetBillsByEventAsync(string eventId) =>
        _js.InvokeAsync<List<BillModel>>("ContaJunstaDb.getBillsByEvent", eventId).AsTask();

    public async Task CloseEventAsync(string eventId)
    {
        var all = await GetAllEventsAsync(false);
        var evt = all.FirstOrDefault(e => e.Id == eventId);
        if (evt is null) return;
        evt.Status = "Closed";
        evt.ClosedAt = DateTime.UtcNow.ToString("o");
        await _js.InvokeVoidAsync("ContaJunstaDb.updateEvent", evt);
    }

    public Task DeleteEventCascadeAsync(string eventId) =>
        _js.InvokeVoidAsync("ContaJunstaDb.deleteEventCascade", eventId).AsTask();

    public Task DeleteBillAsync(string billId) =>
        _js.InvokeVoidAsync("ContaJunstaDb.deleteBill", billId).AsTask();
}
