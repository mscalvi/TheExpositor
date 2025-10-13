using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace BingoCreator.Services
{
    public static class WASMService
    {
        // =========================
        // DTOs do pacote WASM
        // =========================
        public sealed class GameDto
        {
            public int SetId { get; set; } = 0;         // sempre 0 no pacote
            public int CardsSize { get; set; }           // 4 ou 5
            public string Name { get; set; } = "";
            public string Title { get; set; } = "";
            public string Header { get; set; } = "";
            public string Theme { get; set; } = "";
            public int Qnt { get; set; }                 // total de cartelas no pacote
            public string ImageName { get; set; } = "";  // capa (arquivo)
            public string Elements { get; set; } = "";   // útil em 4x4; pode vir vazio em 5x5
            // Grupos (5x5)
            public string GroupB { get; set; } = "";
            public string GroupI { get; set; } = "";
            public string GroupN { get; set; } = "";
            public string GroupG { get; set; } = "";
            public string GroupO { get; set; } = "";
            // Metadados
            public string Version { get; set; } = "1.0";
            public string ExportedAtUtc { get; set; } = DateTime.UtcNow.ToString("o");
        }

        public sealed class ElementDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string CardName { get; set; } = "";
            public string ImageName { get; set; } = ""; // somente o nome do arquivo (ou subcaminho relativo)
        }

        public sealed class Card5x5Dto
        {
            public int Id { get; set; }
            public int CardNumber { get; set; }
            public int SetId { get; set; } = 0;
            public int[] EleB { get; set; } = Array.Empty<int>();
            public int[] EleI { get; set; } = Array.Empty<int>();
            public int[] EleN { get; set; } = Array.Empty<int>();
            public int[] EleG { get; set; } = Array.Empty<int>();
            public int[] EleO { get; set; } = Array.Empty<int>();
        }

        public sealed class Card4x4Dto
        {
            public int Id { get; set; }
            public int CardNumber { get; set; }
            public int SetId { get; set; } = 0;
            public int[] Ele { get; set; } = Array.Empty<int>(); // 16 elementos
        }

        // =========================
        // API principal
        // =========================
        /// <summary>
        /// Gera o pacote WASM (.zip) contendo game.json, elements.json, cards-*.json e assets.json.
        /// NÃO copia imagens; você adiciona a pasta /images manualmente depois.
        /// </summary>
        public static void ExportWasmPackage(
            int setId,
            string outputZipPath,
            bool isSubset = false,
            HashSet<int>? subsetCardNumbers = null)
        {
            // 1) Detecta se é 5x5 ou 4x4 no banco mestre
            var set5 = DataService.GetCardSet5ById(setId);
            var set4 = (set5 == null) ? DataService.GetCardSet4ById(setId) : null;

            if (set5 == null && set4 == null)
                throw new InvalidOperationException($"Set {setId} não encontrado como 5×5 nem 4×4.");

            // 2) Diretório temporário
            var tempDir = Path.Combine(Path.GetTempPath(), "BingoWasm_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var dataDir = tempDir; // JSONs na raiz do pacote
            // (opcional) cria a pasta images vazia, porém diretórios vazios não entram no zip por padrão
            var imagesDir = Path.Combine(tempDir, "images");
            Directory.CreateDirectory(imagesDir);

            try
            {
                var jsonOpts = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                List<string> assetsList; // preenchido a partir dos JSONs

                if (set5 != null)
                {
                    // ========= 5x5 =========
                    string title = set5["Title"]?.ToString() ?? "";
                    string name = set5["Name"]?.ToString() ?? "";
                    string header = set5.Table.Columns.Contains("Header") ? set5["Header"]?.ToString() ?? "" : "";
                    string theme = set5["Theme"]?.ToString() ?? "";
                    string elements = set5["Elements"]?.ToString() ?? "";
                    string image = set5["ImageName"]?.ToString() ?? "";
                    string groupB = set5["GroupB"]?.ToString() ?? "";
                    string groupI = set5["GroupI"]?.ToString() ?? "";
                    string groupN = set5["GroupN"]?.ToString() ?? "";
                    string groupG = set5["GroupG"]?.ToString() ?? "";
                    string groupO = set5["GroupO"]?.ToString() ?? "";

                    var allCards = DataService.GetCards5BySetId(setId);
                    var cards = (isSubset && subsetCardNumbers != null)
                        ? allCards.Where(r => subsetCardNumbers.Contains(Convert.ToInt32(r["CardNumber"])))
                                  .OrderBy(r => Convert.ToInt32(r["CardNumber"])).ToList()
                        : allCards;

                    int qnt = cards.Count;

                    // game.json
                    var game = new GameDto
                    {
                        SetId = 0,
                        CardsSize = 5,
                        Name = name,
                        Title = title,
                        Header = header,
                        Theme = theme,
                        Qnt = qnt,
                        ImageName = ToFileName(image),
                        Elements = elements ?? "",
                        GroupB = groupB,
                        GroupI = groupI,
                        GroupN = groupN,
                        GroupG = groupG,
                        GroupO = groupO
                    };
                    File.WriteAllText(Path.Combine(dataDir, "game.json"), JsonSerializer.Serialize(game, jsonOpts));

                    // elements.json
                    // IDs vindos dos grupos + garantia via cartelas
                    var groupIds = CsvToIntList(groupB)
                        .Concat(CsvToIntList(groupI))
                        .Concat(CsvToIntList(groupN))
                        .Concat(CsvToIntList(groupG))
                        .Concat(CsvToIntList(groupO))
                        .Distinct()
                        .ToList();

                    var usedIds = new HashSet<int>(groupIds);
                    foreach (var card in cards)
                        for (int i = 1; i <= 5; i++)
                        {
                            usedIds.Add(Convert.ToInt32(card[$"EleB{i}"]));
                            usedIds.Add(Convert.ToInt32(card[$"EleI{i}"]));
                            usedIds.Add(Convert.ToInt32(card[$"EleN{i}"]));
                            usedIds.Add(Convert.ToInt32(card[$"EleG{i}"]));
                            usedIds.Add(Convert.ToInt32(card[$"EleO{i}"]));
                        }

                    var elems = DataService.GetElementsByIds(usedIds.ToList());
                    var dtoElems = new List<ElementDto>();
                    foreach (DataRow e in elems)
                    {
                        var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                        var file = ToFileName(img);
                        dtoElems.Add(new ElementDto
                        {
                            Id = Convert.ToInt32(e["Id"]),
                            Name = e["Name"]?.ToString() ?? "",
                            CardName = e["CardName"]?.ToString() ?? "",
                            ImageName = file
                        });
                    }
                    File.WriteAllText(Path.Combine(dataDir, "elements.json"), JsonSerializer.Serialize(dtoElems, jsonOpts));

                    // cards-5x5.json
                    var dtoCards = new List<Card5x5Dto>();
                    foreach (var card in cards)
                    {
                        dtoCards.Add(new Card5x5Dto
                        {
                            Id = Convert.ToInt32(card["Id"]),
                            CardNumber = Convert.ToInt32(card["CardNumber"]),
                            SetId = 0,
                            EleB = Enumerable.Range(1, 5).Select(i => Convert.ToInt32(card[$"EleB{i}"])).ToArray(),
                            EleI = Enumerable.Range(1, 5).Select(i => Convert.ToInt32(card[$"EleI{i}"])).ToArray(),
                            EleN = Enumerable.Range(1, 5).Select(i => Convert.ToInt32(card[$"EleN{i}"])).ToArray(),
                            EleG = Enumerable.Range(1, 5).Select(i => Convert.ToInt32(card[$"EleG{i}"])).ToArray(),
                            EleO = Enumerable.Range(1, 5).Select(i => Convert.ToInt32(card[$"EleO{i}"])).ToArray()
                        });
                    }
                    File.WriteAllText(Path.Combine(dataDir, "cards-5x5.json"), JsonSerializer.Serialize(dtoCards, jsonOpts));

                    // assets.json a partir dos JSONs (sem copiar imagens)
                    assetsList = BuildAssetsList(game.ImageName, dtoElems);
                    File.WriteAllText(Path.Combine(dataDir, "assets.json"),
                        JsonSerializer.Serialize(assetsList, jsonOpts));
                }
                else
                {
                    // ========= 4x4 =========
                    string title = set4!["Title"]?.ToString() ?? "";
                    string name = set4!["Name"]?.ToString() ?? "";
                    string image = set4!["ImageName"]?.ToString() ?? "";
                    string theme = set4!["Theme"]?.ToString() ?? "";
                    string elements = set4!["Elements"]?.ToString() ?? "";

                    var allCards = DataService.GetCards4BySetId(setId);
                    var cards = (isSubset && subsetCardNumbers != null)
                        ? allCards.Where(r => subsetCardNumbers.Contains(Convert.ToInt32(r["CardNumber"])))
                                  .OrderBy(r => Convert.ToInt32(r["CardNumber"])).ToList()
                        : allCards;

                    int qnt = cards.Count;

                    var game = new GameDto
                    {
                        SetId = 0,
                        CardsSize = 4,
                        Name = name,
                        Title = title,
                        Header = "", // 4x4 não usa
                        Theme = theme,
                        Qnt = qnt,
                        ImageName = ToFileName(image),
                        Elements = elements ?? ""
                    };
                    File.WriteAllText(Path.Combine(dataDir, "game.json"), JsonSerializer.Serialize(game, jsonOpts));

                    // elements.json (IDs declarados + garantia via cartelas)
                    var allIds = new HashSet<int>(CsvToIntList(elements));
                    foreach (var card in cards)
                        for (int i = 1; i <= 16; i++)
                            allIds.Add(Convert.ToInt32(card[$"Ele{i}"]));

                    var elems = DataService.GetElementsByIds(allIds.ToList());
                    var dtoElems = new List<ElementDto>();
                    foreach (DataRow e in elems)
                    {
                        var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                        var file = ToFileName(img);
                        dtoElems.Add(new ElementDto
                        {
                            Id = Convert.ToInt32(e["Id"]),
                            Name = e["Name"]?.ToString() ?? "",
                            CardName = e["CardName"]?.ToString() ?? "",
                            ImageName = file
                        });
                    }
                    File.WriteAllText(Path.Combine(dataDir, "elements.json"), JsonSerializer.Serialize(dtoElems, jsonOpts));

                    // cards-4x4.json
                    var dtoCards = new List<Card4x4Dto>();
                    foreach (var card in cards)
                    {
                        var arr = new int[16];
                        for (int i = 1; i <= 16; i++)
                            arr[i - 1] = Convert.ToInt32(card[$"Ele{i}"]);
                        dtoCards.Add(new Card4x4Dto
                        {
                            Id = Convert.ToInt32(card["Id"]),
                            CardNumber = Convert.ToInt32(card["CardNumber"]),
                            SetId = 0,
                            Ele = arr
                        });
                    }
                    File.WriteAllText(Path.Combine(dataDir, "cards-4x4.json"), JsonSerializer.Serialize(dtoCards, jsonOpts));

                    // assets.json a partir dos JSONs (sem copiar imagens)
                    assetsList = BuildAssetsList(game.ImageName, dtoElems);
                    File.WriteAllText(Path.Combine(dataDir, "assets.json"),
                        JsonSerializer.Serialize(assetsList, jsonOpts));
                }

                // 3) Compacta em ZIP
                if (File.Exists(outputZipPath)) File.Delete(outputZipPath);
                ZipFile.CreateFromDirectory(tempDir, outputZipPath, CompressionLevel.Optimal, includeBaseDirectory: false);
            }
            finally
            {
                // limpeza do temp
                try { Directory.Delete(tempDir, recursive: true); } catch { /* ignore */ }
            }
        }

        // =========================
        // Helpers
        // =========================
        private static List<int> CsvToIntList(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return new List<int>();
            return csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => s.Trim())
                      .Where(s => int.TryParse(s, out _))
                      .Select(int.Parse)
                      .ToList();
        }

        private static string ToFileName(string? pathOrName)
        {
            if (string.IsNullOrWhiteSpace(pathOrName)) return "";
            return Path.GetFileName(pathOrName);
        }

        private static List<string> BuildAssetsList(string coverImageName, List<ElementDto> elements)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(coverImageName))
                list.Add($"images/{coverImageName}");

            foreach (var el in elements)
                if (!string.IsNullOrWhiteSpace(el.ImageName))
                    list.Add($"images/{el.ImageName}");

            return list.Distinct(StringComparer.OrdinalIgnoreCase)
                       .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                       .ToList();
        }
    }
}
