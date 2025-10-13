using BingoPlayer.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BingoPlayer.Services
{
    public sealed class PackageService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        // versão atual do pacote (lida de game/version.txt)
        private string _ver = "dev";

        public string? CoverImageName { get; private set; }

        // jogo único por build: arquivos ficam em wwwroot/game/
        private const string BasePath = "game/";
        private const string ImagesPath = "images/";

        public PackageService(HttpClient http) => _http = http;

        public GameModel? Game { get; private set; }

        // ------------ API ------------
        public async Task LoadAsync(CancellationToken ct = default)
        {
            // 0) ler versão do pacote (sem cache)
            await ReadVersionAsync(ct);

            // 1) game.json
            var gameRaw = await GetJsonAsync<GameJson>(BasePath + "game.json", ct)
                         ?? throw new InvalidOperationException("game.json não encontrado/ inválido.");

            CoverImageName = gameRaw.ImageName;

            // 2) elements.json
            var elements = await GetJsonAsync<List<ElementJson>>(BasePath + "elements.json", ct) ?? new();

            // 3) cards (4x4 ou 5x5)
            List<CardModel> cards = new();
            if (gameRaw.CardsSize == 5)
            {
                var cards5 = await GetJsonAsync<List<Card5Json>>(BasePath + "cards-5x5.json", ct) ?? new();
                foreach (var c in cards5)
                {
                    cards.Add(new CardModel
                    {
                        CardId = c.Id,
                        CardNumber = c.CardNumber,
                        BElements = c.EleB?.ToList() ?? new(),
                        IElements = c.EleI?.ToList() ?? new(),
                        NElements = c.EleN?.ToList() ?? new(),
                        GElements = c.EleG?.ToList() ?? new(),
                        OElements = c.EleO?.ToList() ?? new(),
                        // linhas (opcional): primeira posição de cada coluna vira linha 1, etc.
                        Elements1 = MakeRow(c, 0),
                        Elements2 = MakeRow(c, 1),
                        Elements3 = MakeRow(c, 2),
                        Elements4 = MakeRow(c, 3),
                        Elements5 = MakeRow(c, 4),
                        AllElements = new() // não usado no 5x5
                    });
                }
            }
            else if (gameRaw.CardsSize == 4)
            {
                var cards4 = await GetJsonAsync<List<Card4Json>>(BasePath + "cards-4x4.json", ct) ?? new();
                foreach (var c in cards4)
                {
                    cards.Add(new CardModel
                    {
                        CardId = c.Id,
                        CardNumber = c.CardNumber,
                        AllElements = c.Ele?.ToList() ?? new(),
                        BElements = new(),
                        IElements = new(),
                        NElements = new(),
                        GElements = new(),
                        OElements = new(),
                        Elements1 = new(),
                        Elements2 = new(),
                        Elements3 = new(),
                        Elements4 = new(),
                        Elements5 = new()
                    });
                }
            }
            else
            {
                throw new InvalidOperationException($"CardsSize inválido: {gameRaw.CardsSize}");
            }

            // 4) montar seu GameModel
            Game = new GameModel
            {
                GameName = gameRaw.Name ?? "",
                GameTitle = gameRaw.Title ?? "",
                GameQuant = gameRaw.Qnt,
                GameSize = gameRaw.CardsSize,
                GameTheme = gameRaw.Theme ?? "",
                GameHeader = gameRaw.Header ?? "",
                GameCards = cards,

                AllElements = elements.Select(e => new ElementModel
                {
                    Id = e.Id,
                    Name = e.Name ?? "",
                    CardName = e.CardName ?? "",
                    ImageName = e.ImageName ?? ""
                }).ToList(),

                BElements = (gameRaw.CardsSize == 5) ? MapGroup(elements, gameRaw.GroupB) : new(),
                IElements = (gameRaw.CardsSize == 5) ? MapGroup(elements, gameRaw.GroupI) : new(),
                NElements = (gameRaw.CardsSize == 5) ? MapGroup(elements, gameRaw.GroupN) : new(),
                GElements = (gameRaw.CardsSize == 5) ? MapGroup(elements, gameRaw.GroupG) : new(),
                OElements = (gameRaw.CardsSize == 5) ? MapGroup(elements, gameRaw.GroupO) : new(),
            };
        }

        // URL da capa já com ?v=
        public string? GetCoverUrl() => ResolveImageUrlV(CoverImageName);

        // ------------ helpers ------------
        private async Task ReadVersionAsync(CancellationToken ct)
        {
            try
            {
                // só o version.txt é buscado com no-cache explícito
                var verTxt = await _http.GetStringAsync($"{BasePath}version.txt?nc={Guid.NewGuid()}", ct);
                if (!string.IsNullOrWhiteSpace(verTxt))
                    _ver = verTxt.Trim();
            }
            catch
            {
                // ok: fallback em dev/local
                _ver = _ver is { Length: > 0 } ? _ver : "dev";
            }
        }

        // Anexa ?v=... em recursos locais (game/* e images/*)
        private string AddVersion(string path)
        {
            // não duplica se já tiver v=
            if (path.Contains("?v=")) return path;
            var sep = path.Contains('?') ? "&" : "?";
            return $"{path}{sep}v={_ver}";
        }

        private async Task<T?> GetJsonAsync<T>(string path, CancellationToken ct)
        {
            // garante versionamento dos JSONs do pacote
            var p = AddVersion(path);
            return await _http.GetFromJsonAsync<T>(p, _json, ct);
        }

        // Resolve imagem + versão (instância, usa _ver)
        private string? ResolveImageUrlV(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return null;

            var name = System.IO.Path.GetFileName(imageName.Trim())
                        .Normalize(System.Text.NormalizationForm.FormC);
            var encoded = Uri.EscapeDataString(name);
            return AddVersion($"{ImagesPath}{encoded}");
        }

        // Mantém o estático antigo por compat (sem versão). Evite usar.
        public static string ResolveImageUrl(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return "";
            var name = System.IO.Path.GetFileName(imageName.Trim())
                       .Normalize(System.Text.NormalizationForm.FormC);

            var encoded = Uri.EscapeDataString(name);
            return $"images/{encoded}";
        }

        private static List<ElementModel> MapGroup(List<ElementJson> all, string? csv)
        {
            var dict = all.ToDictionary(x => x.Id);
            var list = new List<ElementModel>();
            foreach (var id in ParseCsv(csv))
            {
                if (dict.TryGetValue(id, out var e))
                {
                    list.Add(new ElementModel
                    {
                        Id = e.Id,
                        Name = e.Name ?? "",
                        CardName = e.CardName ?? "",
                        ImageName = e.ImageName ?? ""
                    });
                }
                else
                {
                    // id não listado em elements.json — ainda assim preserva o ID
                    list.Add(new ElementModel { Id = id, Name = "", CardName = "", ImageName = "" });
                }
            }
            return list;
        }

        private static IEnumerable<int> ParseCsv(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) yield break;
            foreach (var s in csv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(s.Trim(), out var id)) yield return id;
        }

        private static List<int> MakeRow(Card5Json c, int i)
        {
            // linha i = B[i], I[i], N[i], G[i], O[i]
            var row = new List<int>(5);
            if (c.EleB is { Length: >= 5 }) row.Add(c.EleB[i]);
            if (c.EleI is { Length: >= 5 }) row.Add(c.EleI[i]);
            if (c.EleN is { Length: >= 5 }) row.Add(c.EleN[i]);
            if (c.EleG is { Length: >= 5 }) row.Add(c.EleG[i]);
            if (c.EleO is { Length: >= 5 }) row.Add(c.EleO[i]);
            return row;
        }

        // -------- tipos privados p/ desserializar os JSONs do pacote --------
        private sealed class GameJson
        {
            public int SetId { get; set; }
            public int CardsSize { get; set; }
            public string? Name { get; set; }
            public string? Title { get; set; }
            public string? Header { get; set; }
            public string? Theme { get; set; }
            public int Qnt { get; set; }
            public string? ImageName { get; set; }
            public string? Elements { get; set; }
            public string? GroupB { get; set; }
            public string? GroupI { get; set; }
            public string? GroupN { get; set; }
            public string? GroupG { get; set; }
            public string? GroupO { get; set; }
            public string? Version { get; set; }
            public string? ExportedAtUtc { get; set; }
        }

        private sealed class ElementJson
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? CardName { get; set; }
            public string? ImageName { get; set; }
        }

        private sealed class Card5Json
        {
            public int Id { get; set; }
            public int CardNumber { get; set; }
            public int SetId { get; set; }
            public int[]? EleB { get; set; }
            public int[]? EleI { get; set; }
            public int[]? EleN { get; set; }
            public int[]? EleG { get; set; }
            public int[]? EleO { get; set; }
        }

        private sealed class Card4Json
        {
            public int Id { get; set; }
            public int CardNumber { get; set; }
            public int SetId { get; set; }
            public int[]? Ele { get; set; } // 16 ids
        }
    }
}
