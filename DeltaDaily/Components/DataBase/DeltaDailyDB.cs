using Blazor.IndexedDB;
using DeltaDaily.Components.Records;
using Microsoft.JSInterop;

namespace DeltaDaily.Components.DataBase;

public sealed class DeltaDailyDB : IndexedDb
{
    // use a MESMA versão em todo lugar (1)
    public DeltaDailyDB(IJSRuntime js, string name, int version) : base(js, name, version) { }
    public DeltaDailyDB(IJSRuntime js) : base(js, "deltaDaily", 1) { }

    // os nomes destas propriedades viram o nome das stores
    public IndexedSet<DayRecord> Days { get; set; } = default!;
    public IndexedSet<ProjectRecord> Projects { get; set; } = default!;
}
