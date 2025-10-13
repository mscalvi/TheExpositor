using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.IndexedDB;
using DeltaDaily.Components.Data;
using DeltaDaily.Components.DataBase;
using DeltaDaily.Components.Records;
using DeltaDaily.Components.Models;
using System.Globalization;

namespace DeltaDaily.Components.Services
{
    public interface IDayStoreService
    {
        Task<DayModel> LoadAsync(DateTime date);
        Task SaveAsync(DayModel day);
        Task<IReadOnlyList<(DateTime date, bool closed)>> ListByMonthAsync(int year, int month);

        // NOVO: listar todos os dias salvos
        Task<List<DayModel>> ListAllAsync();
    }

    public sealed class DayStoreService : IDayStoreService
    {
        private readonly IIndexedDbFactory _factory;
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public DayStoreService(IIndexedDbFactory factory) => _factory = factory;

        private static string Key(DateTime d) => d.ToString("yyyy-MM-dd");

        public async Task<DayModel> LoadAsync(DateTime date)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var key = Key(date);
            var rec = db.Days.SingleOrDefault(x => x.Id == key);
            if (rec is null)
                return new DayModel { Data = date };

            var model = JsonSerializer.Deserialize<DayModel>(rec.Payload, JsonOpts)
                        ?? new DayModel { Data = date };

            // garante Data coerente
            model.Data = date;
            return model;
        }

        public async Task SaveAsync(DayModel day)
        {
            // Recalcula totais por tipo (garante que Resumo/Planner vejam horas certas)
            day.TotalMinutosDedicados = 0;
            day.TotalMinutosParalelos = 0;
            day.TotalMinutosExtras = 0;

            var trabalhos = day.Trabalhos ?? new List<WorkModel>();
            foreach (var w in trabalhos)
            {
                var mins = MinutesFromDur(w.Duracao);
                switch (WorkTypeName(w.Tipo))
                {
                    case "Dedicada":
                        day.TotalMinutosDedicados += mins; break;
                    case "Paralela":
                        day.TotalMinutosParalelos += mins; break;
                    case "Extra":
                        day.TotalMinutosExtras += mins; break;
                }
            }

            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var k = Key(day.Data);
            var rec = db.Days.SingleOrDefault(x => x.Id == k);

            if (rec is null)
            {
                db.Days.Add(new DayRecord
                {
                    Id = k,
                    Date = k, // opcional
                    Closed = day.DiaFechado,
                    UpdatedAtUtc = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(day, JsonOpts)
                });
            }
            else
            {
                rec.Closed = day.DiaFechado;
                rec.UpdatedAtUtc = DateTime.UtcNow;
                rec.Payload = JsonSerializer.Serialize(day, JsonOpts);
            }

            await db.SaveChanges();
        }

        public async Task<IReadOnlyList<(DateTime date, bool closed)>> ListByMonthAsync(int year, int month)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var prefix = $"{year:D4}-{month:D2}-"; // "2025-09-"
            return db.Days
                .Where(r => r.Id.StartsWith(prefix, StringComparison.Ordinal))
                .Select(r => (DateTime.ParseExact(r.Id, "yyyy-MM-dd", CultureInfo.InvariantCulture), r.Closed))
                .ToList();
        }

        // NOVO: lista todos os DayModel do IndexedDB
        public async Task<List<DayModel>> ListAllAsync()
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var recs = db.Days
                .OrderBy(r => r.Id) // mantém cronológico
                .ToList();

            var result = new List<DayModel>(recs.Count);
            foreach (var r in recs)
            {
                DayModel? model = null;
                try
                {
                    model = JsonSerializer.Deserialize<DayModel>(r.Payload, JsonOpts);
                }
                catch
                {
                    // se der erro em algum payload antigo/corrompido, ignora só esse
                }

                // Garante Data preenchida a partir da chave caso o payload não tenha
                if (model is null)
                {
                    model = new DayModel();
                }

                if (model.Data == default)
                {
                    if (DateTime.TryParseExact(r.Id, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                               DateTimeStyles.None, out var parsed))
                    {
                        model.Data = parsed;
                    }
                }

                // Conserva status fechado da store (se o payload não tiver sincronizado)
                if (model.DiaFechado != r.Closed)
                {
                    // não forço porque o payload é fonte de verdade; mas se quiser alinhar:
                    // model.DiaFechado = r.Closed;
                }

                result.Add(model);
            }

            return result;
        }

        // --- helpers internos ---
        private static int MinutesFromDur(object? dur)
        {
            if (dur is null) return 0;
            if (dur is TimeOnly t) return t.Hour * 60 + t.Minute;
            if (dur is string s && TimeSpan.TryParse(s, out var ts)) return (int)ts.TotalMinutes;
            return 0;
        }

        private static string? WorkTypeName(object? tipo) => tipo?.ToString(); // "Dedicada", "Paralela", "Extra"
    }
}
