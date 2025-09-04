using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HardStopGuardian
{
    internal class CheckupItem
    {
        public string Label { get; set; } = "";
        public TimeSpan Time { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime? LastDoneDate { get; set; } // dia em que foi marcado “Feito”
    }

    internal static class CheckupStore
    {
        private static readonly string BaseDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HardStopGuardian");
        private static readonly string FilePath = Path.Combine(BaseDir, "checkups.json");

        internal static string PathForUser => FilePath;

        private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

        public static List<CheckupItem> Load()
        {
            Directory.CreateDirectory(BaseDir);
            if (!File.Exists(FilePath))
            {
                var defaults = new List<CheckupItem>
                {
                    // Vazio
                };
                Save(defaults);
                return defaults;
            }
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<CheckupItem>>(json, JsonOpts) ?? new();
        }

        public static void Save(List<CheckupItem> items)
        {
            Directory.CreateDirectory(BaseDir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(items, JsonOpts));
        }
    }
}
