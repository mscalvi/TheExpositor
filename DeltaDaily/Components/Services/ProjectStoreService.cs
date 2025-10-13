using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.IndexedDB;
using DeltaDaily.Components.DataBase;
using DeltaDaily.Components.Models;
using DeltaDaily.Components.Records;

namespace DeltaDaily.Components.Services
{

    public interface IProjectStoreService
    {
        Task<ProjectModel?> LoadAsync(string id);
        Task SaveAsync(ProjectModel project);
        Task SetActiveAsync(string id);
        Task<ProjectModel?> LoadActiveAsync();
        Task<IReadOnlyList<ProjectModel>> ListAllAsync();
    }

    public sealed class ProjectStoreService : IProjectStoreService
    {
        private readonly IIndexedDbFactory _factory;
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public ProjectStoreService(IIndexedDbFactory factory) => _factory = factory;

        public async Task<ProjectModel?> LoadAsync(string id)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);
            var rec = db.Projects.SingleOrDefault(p => p.Id == id);
            if (rec is null) return null;

            var model = JsonSerializer.Deserialize<ProjectModel>(rec.Payload, JsonOpts);
            return model;
        }

        public async Task SaveAsync(ProjectModel project)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);
            var rec = db.Projects.SingleOrDefault(p => p.Id == project.Id);

            var nowUtc = DateTime.UtcNow;
            var payload = JsonSerializer.Serialize(project, JsonOpts);

            if (rec is null)
            {
                db.Projects.Add(new ProjectRecord
                {
                    Id = project.Id,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc,
                    Payload = payload
                });
            }
            else
            {
                rec.UpdatedAtUtc = nowUtc;
                rec.Payload = payload;
            }

            await db.SaveChanges();
        }
        public async Task SetActiveAsync(string id)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            // carrega todos para ajustar ativo/inativo
            var all = db.Projects.ToList();
            foreach (var rec in all)
            {
                var model = JsonSerializer.Deserialize<ProjectModel>(rec.Payload, JsonOpts);
                if (model is null) continue;

                bool shouldBeActive = rec.Id == id;
                if (model.Ativo != shouldBeActive)
                {
                    model.Ativo = shouldBeActive;
                    rec.Payload = JsonSerializer.Serialize(model, JsonOpts);
                    rec.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            await db.SaveChanges();
        }

        public async Task<ProjectModel?> LoadActiveAsync()
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            foreach (var rec in db.Projects)
            {
                var model = JsonSerializer.Deserialize<ProjectModel>(rec.Payload, JsonOpts);
                if (model?.Ativo == true)
                    return model;
            }
            return null;
        }

        public async Task<IReadOnlyList<ProjectModel>> ListAllAsync()
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);
            var list = new List<ProjectModel>();
            foreach (var rec in db.Projects)
            {
                var model = JsonSerializer.Deserialize<ProjectModel>(rec.Payload, JsonOpts);
                if (model is null) continue;

                // garante que o ID do modelo exista (caso não esteja serializado)
                if (string.IsNullOrWhiteSpace(model.Id))
                    model.Id = rec.Id;

                list.Add(model);
            }
            return list;
        }


    }
}
