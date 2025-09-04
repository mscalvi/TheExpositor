using System;
using System.IO;
using System.Text.Json;

namespace HardStopGuardian
{
    internal static class Persistence
    {
        private static readonly string BaseDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HardStopGuardian");
        private static readonly string StatePath = Path.Combine(BaseDir, "state.json");

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented = true
        };

        internal static void Save(StateModel state)
        {
            Directory.CreateDirectory(BaseDir);
            var json = JsonSerializer.Serialize(state, JsonOpts);
            File.WriteAllText(StatePath, json);
        }

        internal static StateModel? Load()
        {
            if (!File.Exists(StatePath)) return null;
            var json = File.ReadAllText(StatePath);
            return JsonSerializer.Deserialize<StateModel>(json, JsonOpts);
        }
    }

    internal class StateModel
    {
        public bool Armed { get; set; }
        public DateTime NextHardStop { get; set; }
        public int BlockMinutes { get; set; }
    }
}
