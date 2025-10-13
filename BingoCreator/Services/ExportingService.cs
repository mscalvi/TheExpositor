using BingoCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoCreator.Services
{
    internal class ExportingService
    {
        public static void ExportDataBase(int cardsetid)
        {
            CardSetModel cards = DataService.GetCardSetById(cardsetid);

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"{cards.Name}DB.db";
            var filePath = Path.Combine(folder, fileName);

            DataService.ExportGameDatabaseToPath(cards.Id, filePath);
        }
        public static void ExportDataBaseSubset(int setId, IEnumerable<int> cardNumbers)
        {
            var cards = DataService.GetCardSetById(setId);
            if (cards == null) return;

            var wanted = new HashSet<int>((cardNumbers ?? Enumerable.Empty<int>()).Distinct());
            if (wanted.Count == 0) return;

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);

            // arquivo diferenciado para subset
            var fileName = $"{cards.Name}DB_novas.db";
            var filePath = Path.Combine(folder, fileName);

            DataService.ExportGameDatabaseSubsetToPath(setId, wanted, filePath);
        }
        static string Sanitize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "Cartelas";
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

    }
}
