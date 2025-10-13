using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel.Design;
using BingoCreator.Models;
using System.ComponentModel.DataAnnotations;

namespace BingoCreator.Services
{
    public static class DataService
    {
        private static readonly string _connectionString;

        // Conexão
        // Método principal de conexão
        static DataService()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string databaseFolder = Path.Combine(baseDir, "Database");

                Directory.CreateDirectory(databaseFolder);

                string databasePath = Path.Combine(databaseFolder, "BingoManager.db");
                _connectionString = $"Data Source={databasePath};Version=3;";

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
                throw;
            }
        }

        // Método para abrir uma conexão com o banco de dados
        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        // Método para inicializar o banco de dados (criar as tabelas se não existirem)
        public static void InitializeDatabase()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                }

                var createTableCommands = new List<string>
        {
            // ----- Elements -----
            @"
            CREATE TABLE IF NOT EXISTS ElementsTable (
                Id INTEGER PRIMARY KEY NOT NULL UNIQUE,
                Name TEXT NOT NULL,
                CardName TEXT NOT NULL,
                Note1 TEXT,
                Note2 TEXT,
                ImageName TEXT,
                Obsolete INTEGER NOT NULL DEFAULT 0,
                ParentId INTEGER NULL,
                Version INTEGER NOT NULL DEFAULT 1,
                AddTime TEXT NOT NULL
            );",

            // ----- Lists -----
            @"
            CREATE TABLE IF NOT EXISTS ListsTable (
                Id INTEGER PRIMARY KEY,
                Name TEXT,
                Description TEXT,
                Obsolete INTEGER NOT NULL DEFAULT 0, 
                ParentId INTEGER NULL, 
                Version INTEGER NOT NULL DEFAULT 1,
                ImageName TEXT
            );",

            // ----- Alocation (Element <-> List) -----
            @"
            CREATE TABLE IF NOT EXISTS AlocationTable (
                ElementId INTEGER REFERENCES ElementsTable(Id),
                ListId INTEGER  REFERENCES ListsTable(Id),
                Obsolete INTEGER,
                PRIMARY KEY (ElementId, ListId)
            );",

            // ===== Unified Card Sets (4x4 and 5x5) =====
            @"
            CREATE TABLE IF NOT EXISTS CardsSets (
                SetId     INTEGER PRIMARY KEY NOT NULL UNIQUE,
                ListId    INTEGER NOT NULL REFERENCES ListsTable(Id),
                Title     TEXT NOT NULL,
                End       TEXT,
                Quantity  INTEGER NOT NULL,
                ImageName TEXT,
                Name      TEXT UNIQUE,
                CardsSize INTEGER NOT NULL,
                AddTime   TEXT,
                -- 5x5
                GroupB    TEXT,
                GroupI    TEXT,
                GroupN    TEXT,
                GroupG    TEXT,
                GroupO    TEXT,
                -- 4x4
                Elements  TEXT,
                -- NOVOS CAMPOS
                Theme     TEXT,
                Header    TEXT,
                Model     TEXT,
                Obsolete INTEGER NOT NULL DEFAULT 0, 
                ParentId INTEGER NULL, 
                Version INTEGER NOT NULL DEFAULT 1
            );",

            // ----- Cards 5x5 (referência agora em CardsSets) -----
            @"
            CREATE TABLE IF NOT EXISTS CardsList5Table (
                Id INTEGER PRIMARY KEY,
                SetId INTEGER NOT NULL REFERENCES CardsSets(SetId),
                ListId INTEGER NOT NULL REFERENCES ListsTable(Id),
                CardNumber INTEGER NOT NULL,
                EleB1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleB5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleI5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleN5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleG5 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO1 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO2 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO3 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO4 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                EleO5 INTEGER NOT NULL REFERENCES ElementsTable(Id)
            );",

            // ----- Cards 4x4 (referência agora em CardsSets) -----
            @"
            CREATE TABLE IF NOT EXISTS CardsList4Table (
                Id INTEGER PRIMARY KEY,
                SetId INTEGER NOT NULL REFERENCES CardsSets(SetId),
                ListId INTEGER NOT NULL REFERENCES ListsTable(Id),
                CardNumber INTEGER NOT NULL,
                Ele1  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele2  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele3  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele4  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele5  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele6  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele7  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele8  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele9  INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele10 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele11 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele12 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele13 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele14 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele15 INTEGER NOT NULL REFERENCES ElementsTable(Id),
                Ele16 INTEGER NOT NULL REFERENCES ElementsTable(Id)
            );"
        };

                foreach (var commandText in createTableCommands)
                {
                    using (var command = new SQLiteCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        // Criação
        // Criar Elemento
        public static int CreateElement(string name, string cardName, string note1, string note2, string imageName, string addTime)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO ElementsTable (Name, CardName, Note1, Note2, ImageName, AddTime)
            VALUES (@Name, @CardName, @Note1, @Note2, @ImageName, @AddTime);
            SELECT last_insert_rowid();";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CardName", cardName);
                    command.Parameters.AddWithValue("@Note1", note1);
                    command.Parameters.AddWithValue("@Note2", note2);
                    command.Parameters.AddWithValue("@ImageName", imageName);
                    command.Parameters.AddWithValue("@AddTime", addTime);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Criar Lista
        public static int CreateList(string name, string description, string imagename)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO ListsTable (Name, Description, ImageName)
            VALUES (@Name, @Description, @ImageName);
            SELECT last_insert_rowid();";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@ImageName", imagename);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Criar Conjunto de Cartelas
        public static int CreateCardsSet(CardSetModel cards)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (cards.CardsSize != 4 && cards.CardsSize != 5)
                throw new ArgumentException("CardsSize deve ser 4 ou 5.", nameof(cards.CardsSize));

            string addTime = string.IsNullOrWhiteSpace(cards.AddDate)
                ? DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                : cards.AddDate;

            using var connection = GetConnection();
            connection.Open();

            if (cards.CardsSize == 5)
            {
                string sql = @"
            INSERT INTO CardsSets
                (ListId, Name, Title, End, Quantity, CardsSize, ImageName,
                 GroupB, GroupI, GroupN, GroupG, GroupO, Elements,
                 AddTime, Theme, Header, Model)
            VALUES
                (@ListId, @Name, @Title, @End, @Quantity, @CardsSize, @ImageName,
                 @GroupB, @GroupI, @GroupN, @GroupG, @GroupO, @Elements,
                 @AddTime, @Theme, @Header, @Model);
            SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);
                cmd.Parameters.AddWithValue("@ImageName", cards.ImageName ?? "");

                string groupB = !string.IsNullOrWhiteSpace(cards.GroupBIds) ? cards.GroupBIds
                                 : string.Join(",", (cards.GroupB ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupI = !string.IsNullOrWhiteSpace(cards.GroupIIds) ? cards.GroupIIds
                                 : string.Join(",", (cards.GroupI ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupN = !string.IsNullOrWhiteSpace(cards.GroupNIds) ? cards.GroupNIds
                                 : string.Join(",", (cards.GroupN ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupG = !string.IsNullOrWhiteSpace(cards.GroupGIds) ? cards.GroupGIds
                                 : string.Join(",", (cards.GroupG ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                string groupO = !string.IsNullOrWhiteSpace(cards.GroupOIds) ? cards.GroupOIds
                                 : string.Join(",", (cards.GroupO ?? Enumerable.Empty<ElementModel>()).Select(e => e.Id));
                
                string ElementsCsv()
                {
                    var seen = new HashSet<string>();
                    var order = new[] { groupB, groupI, groupN, groupG, groupO };
                    var list = new List<string>();

                    foreach (var part in order)
                    {
                        if (string.IsNullOrWhiteSpace(part)) continue;
                        foreach (var tok in part.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var id = tok.Trim();
                            if (id.Length == 0) continue;
                            if (seen.Add(id)) list.Add(id);
                        }
                    }
                    return string.Join(",", list);
                }
                string elements = ElementsCsv();

                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);
                cmd.Parameters.AddWithValue("@Elements", elements);
                cmd.Parameters.AddWithValue("@AddTime", addTime);
                cmd.Parameters.AddWithValue("@Theme", cards.Theme ?? "");
                cmd.Parameters.AddWithValue("@Header", cards.Header ?? "");
                cmd.Parameters.AddWithValue("@Model", cards.Model ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            else // 4x4
            {
                // Fonte primária: o pool completo vindo no modelo
                // (cards.AllElements deve conter TODOS os elementos candidatos do set 4x4)
                IEnumerable<int> idsSource;

                if (cards.AllElements != null && cards.AllElements.Count > 0)
                {
                    idsSource = cards.AllElements.Select(e => e.Id);
                }
                else
                {
                    // Fallback: carrega TODOS os elementos da lista associada (não apenas 16)
                    // Ordena por CardName (fallback Name) para manter determinismo
                    idsSource = GetElementsInList(cards.ListId)
                        .OrderBy(r =>
                        {
                            var card = r.Table.Columns.Contains("CardName") ? r["CardName"]?.ToString() : null;
                            var name = r.Table.Columns.Contains("Name") ? r["Name"]?.ToString() : null;
                            return string.IsNullOrWhiteSpace(card) ? name : card;
                        })
                        .Select(r => Convert.ToInt32(r["Id"]));
                }

                // Limpa, remove duplicados — **sem** limitar a 16
                var idsAll = idsSource
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                if (idsAll.Count < 16)
                    throw new InvalidOperationException("Para 4x4, é necessário pelo menos 16 elementos no pool.");

                string elementsStr = string.Join(",", idsAll);

                string sql = @"
        INSERT INTO CardsSets
            (ListId, Name, Title, End, Quantity, CardsSize, Elements, ImageName,
             AddTime, Theme, Header, Model)
        VALUES
            (@ListId, @Name, @Title, @End, @Quantity, @CardsSize, @Elements, @ImageName,
             @AddTime, @Theme, @Header, @Model);
        SELECT last_insert_rowid();";

                using var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ListId", cards.ListId);
                cmd.Parameters.AddWithValue("@Name", cards.Name ?? "");
                cmd.Parameters.AddWithValue("@Title", cards.Title ?? "");
                cmd.Parameters.AddWithValue("@End", cards.End ?? "");
                cmd.Parameters.AddWithValue("@Quantity", cards.Quantity);
                cmd.Parameters.AddWithValue("@CardsSize", cards.CardsSize);
                cmd.Parameters.AddWithValue("@ImageName", cards.ImageName ?? "");
                cmd.Parameters.AddWithValue("@Elements", elementsStr);
                cmd.Parameters.AddWithValue("@AddTime", addTime);
                cmd.Parameters.AddWithValue("@Theme", cards.Theme ?? "");
                cmd.Parameters.AddWithValue("@Header", cards.Header ?? "");
                cmd.Parameters.AddWithValue("@Model", cards.Model ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }

        }

        // Criar Cartelas 5x5
        public static void CreateCard5(int listId, List<int> elementsIds, int cardNumber, int setId)
        {
            string query = @"INSERT INTO CardsList5Table 
                     (ListId, SetId, CardNumber, EleB1, EleB2, EleB3, EleB4, EleB5,
                      EleI1, EleI2, EleI3, EleI4, EleI5,
                      EleN1, EleN2, EleN3, EleN4, EleN5,
                      EleG1, EleG2, EleG3, EleG4, EleG5,
                      EleO1, EleO2, EleO3, EleO4, EleO5) 
                     VALUES 
                     (@ListId, @SetId, @CardNumber, @EleB1, @EleB2, @EleB3, @EleB4, @EleB5,
                      @EleI1, @EleI2, @EleI3, @EleI4, @EleI5,
                      @EleN1, @EleN2, @EleN3, @EleN4, @EleN5,
                      @EleG1, @EleG2, @EleG3, @EleG4, @EleG5,
                      @EleO1, @EleO2, @EleO3, @EleO4, @EleO5)";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);
                    command.Parameters.AddWithValue("@SetId", setId);
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    for (int i = 0; i < 5; i++)
                    {
                        command.Parameters.AddWithValue($"@EleB{i + 1}", elementsIds[i]);
                        command.Parameters.AddWithValue($"@EleI{i + 1}", elementsIds[i + 5]);
                        command.Parameters.AddWithValue($"@EleN{i + 1}", elementsIds[i + 10]);
                        command.Parameters.AddWithValue($"@EleG{i + 1}", elementsIds[i + 15]);
                        command.Parameters.AddWithValue($"@EleO{i + 1}", elementsIds[i + 20]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // Criar Cartelas 4x4
        public static void CreateCard4(int listId, List<int> elementsIds, int cardNumber, int setId)
        {
            string query = @"INSERT INTO CardsList4Table 
                     (ListId, SetId, CardNumber, Ele1, Ele2, Ele3, Ele4, Ele5,
                      Ele6, Ele7, Ele8, Ele9, Ele10,
                      Ele11, Ele12, Ele13, Ele14, Ele15,
                      Ele16) 
                     VALUES 
                     (@ListId, @SetId, @CardNumber, @Ele1, @Ele2, @Ele3, @Ele4, @Ele5,
                      @Ele6, @Ele7, @Ele8, @Ele9, @Ele10,
                      @Ele11, @Ele12, @Ele13, @Ele14, @Ele15,
                      @Ele16)";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);
                    command.Parameters.AddWithValue("@SetId", setId);
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    for (int i = 0; i <= 15; i++)
                    {
                        command.Parameters.AddWithValue($"@Ele{i + 1}", elementsIds[i]);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // Inserir Elementos em uma Lista
        public static void AlocateElements(int listId, List<int> elementsIds)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (int elementId in elementsIds)
                {
                    string insertQuery = "INSERT INTO AlocationTable (ListId, ElementId) VALUES (@ListId, @ElementId)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ListId", listId);
                        command.Parameters.AddWithValue("@ElementId", elementId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Confere se Elemento já está em uma Lista
        public static bool IsElementInList(int listId, int elementId)
        {
            const string sql = @"SELECT 1 FROM AlocationTable 
                         WHERE ListId=@ListId AND ElementId=@ElementId LIMIT 1;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ListId", listId);
            cmd.Parameters.AddWithValue("@ElementId", elementId);
            return cmd.ExecuteScalar() != null;
        }

        // Remove lista de Elementos de uma Lista
        public static int RemoveElementsFromList(int listId, IEnumerable<int> elementIds)
        {
            var ids = elementIds?.Distinct().ToList();
            if (listId <= 0 || ids == null || ids.Count == 0) return 0;

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            // cria parâmetros dinâmicos para o IN
            var paramNames = new List<string>();
            for (int i = 0; i < ids.Count; i++) paramNames.Add($"@E{i}");

            string sql = $@"DELETE FROM AlocationTable
                    WHERE ListId = @ListId AND ElementId IN ({string.Join(",", paramNames)});";

            using var cmd = new SQLiteCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@ListId", listId);
            for (int i = 0; i < ids.Count; i++)
                cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

            int removed = cmd.ExecuteNonQuery();
            tx.Commit();
            return removed;
        }

        // Máximo número de cartela já existente no set (0 se não há)
        public static int GetMaxCardNumberBySetId(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            // tenta nas 5x5
            using (var cmd = new SQLiteCommand(
                "SELECT IFNULL(MAX(CardNumber),0) FROM CardsList5Table WHERE SetId=@SetId;", conn))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                var val = Convert.ToInt32(cmd.ExecuteScalar());
                if (val > 0) return val;
            }

            // tenta nas 4x4
            using (var cmd = new SQLiteCommand(
                "SELECT IFNULL(MAX(CardNumber),0) FROM CardsList4Table WHERE SetId=@SetId;", conn))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Assinaturas existentes (5x5): "B1-B2-...-O5"
        public static HashSet<string> GetExistingSignatures5(int setId)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(
                "SELECT EleB1,EleB2,EleB3,EleB4,EleB5," +
                "       EleI1,EleI2,EleI3,EleI4,EleI5," +
                "       EleN1,EleN2,EleN3,EleN4,EleN5," +
                "       EleG1,EleG2,EleG3,EleG4,EleG5," +
                "       EleO1,EleO2,EleO3,EleO4,EleO5 " +
                "FROM CardsList5Table WHERE SetId=@SetId;", conn);
            cmd.Parameters.AddWithValue("@SetId", setId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ids = new List<int>(25);
                for (int i = 0; i < 25; i++) ids.Add(Convert.ToInt32(r.GetValue(i)));
                set.Add(string.Join("-", ids));
            }
            return set;
        }

        // Assinaturas existentes (4x4): "E1-E2-...-E16"
        public static HashSet<string> GetExistingSignatures4(int setId)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(
                "SELECT Ele1,Ele2,Ele3,Ele4,Ele5,Ele6,Ele7,Ele8," +
                "       Ele9,Ele10,Ele11,Ele12,Ele13,Ele14,Ele15,Ele16 " +
                "FROM CardsList4Table WHERE SetId=@SetId;", conn);
            cmd.Parameters.AddWithValue("@SetId", setId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ids = new List<int>(16);
                for (int i = 0; i < 16; i++) ids.Add(Convert.ToInt32(r.GetValue(i)));
                set.Add(string.Join("-", ids));
            }
            return set;
        }

        // Atualiza a quantidade declarada no conjunto
        public static void UpdateCardSetQuantity(int setId, int newQuantity)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE CardsSets SET Quantity=@Qnt WHERE SetId=@SetId;", conn);
            cmd.Parameters.AddWithValue("@Qnt", newQuantity);
            cmd.Parameters.AddWithValue("@SetId", setId);
            cmd.ExecuteNonQuery();
        }



        // Exportação
        // Método de Conexão
        public static void ExportGameDatabaseToPath(
            int setId,
            string outputPath,
            bool alsoGenerateWasm = true,
            string? wasmZipOutputPath = null)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            SQLiteConnection.CreateFile(outputPath);
            string connStr = $"Data Source={outputPath};Version=3;";

            using var conn = new SQLiteConnection(connStr);
            conn.Open();

            var set = GetCardSetById(setId) ?? throw new Exception($"Conjunto com SetId={setId} não encontrado.");

            if (set.CardsSize == 5)
            {
                CreateGameDB5(conn);
                ExportGame5(conn, setId);
            }
            else if (set.CardsSize == 4)
            {
                CreateGameDB4(conn);
                ExportGame4(conn, setId);
            }
            else
            {
                throw new Exception($"CardsSize inválido ({set.CardsSize}) para SetId={setId}.");
            }

            // ---> Gera o pacote WASM (opcional)
            if (alsoGenerateWasm)
            {
                var zipPath = wasmZipOutputPath ?? MakeZipPathFromDb(outputPath);
                BingoCreator.Services.WASMService.ExportWasmPackage(
                    setId,
                    zipPath,
                    isSubset: false,
                    subsetCardNumbers: null
                );
            }
        }

        public static void ExportGameDatabaseSubsetToPath(
            int setId,
            HashSet<int> cardNumbers,
            string outputPath,
            bool alsoGenerateWasm = true,
            string? wasmZipOutputPath = null)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            SQLiteConnection.CreateFile(outputPath);
            string connStr = $"Data Source={outputPath};Version=3;";

            using var conn = new SQLiteConnection(connStr);
            conn.Open();

            var set = GetCardSetById(setId) ?? throw new Exception($"Conjunto {setId} não encontrado.");

            if (set.CardsSize == 5)
            {
                CreateGameDB5(conn);
                ExportGame5Subset(conn, setId, cardNumbers);
            }
            else if (set.CardsSize == 4)
            {
                CreateGameDB4(conn);
                ExportGame4Subset(conn, setId, cardNumbers);
            }
            else
            {
                throw new Exception($"CardsSize inválido ({set.CardsSize}).");
            }

            // ---> Gera o pacote WASM (opcional), já indicando que é subset
            if (alsoGenerateWasm)
            {
                var zipPath = wasmZipOutputPath ?? MakeZipPathFromDb(outputPath);
                BingoCreator.Services.WASMService.ExportWasmPackage(
                    setId,
                    zipPath,
                    isSubset: true,
                    subsetCardNumbers: cardNumbers
                );
            }
        }

        // helper simples para nome do ZIP ao lado do .db
        private static string MakeZipPathFromDb(string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath) ?? "";
            var name = Path.GetFileNameWithoutExtension(dbPath);
            return Path.Combine(dir, $"{name}_wasm.zip");
        }

        // Criar Banco do Jogo 
        public static void CreateGameDB5(SQLiteConnection conn)
        {
            var commands = new List<string>
    {
        @"CREATE TABLE ElementsTable (
            Id INTEGER PRIMARY KEY,
            Name TEXT,
            CardName TEXT,
            ImageName TEXT
        );",

        @"CREATE TABLE CardsSets (
            SetId INTEGER PRIMARY KEY,
            Name TEXT,
            Title TEXT,
            Qnt INTEGER,
            ImageName TEXT,
            Header TEXT,
            Theme TEXT,
            Elements TEXT,
            GroupB TEXT,
            GroupI TEXT,
            GroupN TEXT,
            GroupG TEXT,
            GroupO TEXT
        );",

        @"CREATE TABLE CardsList (
            Id INTEGER PRIMARY KEY,
            CardNumber INTEGER,
            SetId INTEGER,
            EleB1 INTEGER, EleB2 INTEGER, EleB3 INTEGER, EleB4 INTEGER, EleB5 INTEGER,
            EleI1 INTEGER, EleI2 INTEGER, EleI3 INTEGER, EleI4 INTEGER, EleI5 INTEGER,
            EleN1 INTEGER, EleN2 INTEGER, EleN3 INTEGER, EleN4 INTEGER, EleN5 INTEGER,
            EleG1 INTEGER, EleG2 INTEGER, EleG3 INTEGER, EleG4 INTEGER, EleG5 INTEGER,
            EleO1 INTEGER, EleO2 INTEGER, EleO3 INTEGER, EleO4 INTEGER, EleO5 INTEGER
        );"
    };

            foreach (var cmdText in commands)
            {
                using var cmd = new SQLiteCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }
        }
        public static void CreateGameDB4(SQLiteConnection conn)
        {
            var commands = new List<string>
    {
        @"CREATE TABLE ElementsTable (
            Id INTEGER PRIMARY KEY,
            Name TEXT,
            CardName TEXT,
            ImageName TEXT
        );",

        @"CREATE TABLE CardsSets (
            SetId INTEGER PRIMARY KEY,
            Name TEXT,
            Title TEXT,
            Qnt INTEGER,
            ImageName TEXT,
            Theme TEXT,
            Elements TEXT
        );",

        @"CREATE TABLE CardsList (
            Id INTEGER PRIMARY KEY,
            CardNumber INTEGER,
            SetId INTEGER,
            Ele1 INTEGER, Ele2 INTEGER, Ele3 INTEGER, Ele4 INTEGER,
            Ele5 INTEGER, Ele6 INTEGER, Ele7 INTEGER, Ele8 INTEGER,
            Ele9 INTEGER, Ele10 INTEGER, Ele11 INTEGER, Ele12 INTEGER,
            Ele13 INTEGER, Ele14 INTEGER, Ele15 INTEGER, Ele16 INTEGER
        );"
    };

            foreach (var cmdText in commands)
            {
                using var cmd = new SQLiteCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }
        }
        // Selecionar informações do Jogo
        public static DataRow GetCardSet5ById(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = @"
        SELECT SetId, Name, Title, ImageName, Quantity, Elements, GroupB, GroupI, GroupN, GroupG, GroupO, Theme, Header
        FROM CardsSets
        WHERE SetId = @SetId AND CardsSize = 5
        LIMIT 1;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        public static List<DataRow> GetCards5BySetId(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = "SELECT * FROM CardsList5Table WHERE SetId = @SetId ORDER BY CardNumber;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.AsEnumerable().ToList();
        }
        public static DataRow GetCardSet4ById(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = @"
        SELECT SetId, Name, Title, Quantity, Elements, Theme, ImageName
        FROM CardsSets
        WHERE SetId = @SetId AND CardsSize = 4
        LIMIT 1;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        public static List<DataRow> GetCards4BySetId(int setId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = "SELECT * FROM CardsList4Table WHERE SetId = @SetId ORDER BY CardNumber;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SetId", setId);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.AsEnumerable().ToList();
        }
        // Preenche Banco do Jogo
        public static void ExportGame5(SQLiteConnection conn, int setId)
        {
            DataRow setRow = GetCardSet5ById(setId)
                ?? throw new Exception($"Conjunto 5×5 SetId={setId} não encontrado.");

            string title = setRow["Title"]?.ToString() ?? "";
            string name = setRow["Name"]?.ToString() ?? "";
            string header = setRow.Table.Columns.Contains("Header") ? setRow["Header"]?.ToString() ?? "" : "";
            string theme = setRow["Theme"]?.ToString() ?? "";
            string elements = setRow["Elements"]?.ToString() ?? "";
            int quantity = Convert.ToInt32(setRow["Quantity"]);
            string image = setRow["ImageName"]?.ToString() ?? "";
            string groupB = setRow["GroupB"]?.ToString() ?? "";
            string groupI = setRow["GroupI"]?.ToString() ?? "";
            string groupN = setRow["GroupN"]?.ToString() ?? "";
            string groupG = setRow["GroupG"]?.ToString() ?? "";
            string groupO = setRow["GroupO"]?.ToString() ?? "";

            // CardsSets (DB de saída; SetId=0)
            const string insertSet = @"
        INSERT INTO CardsSets
            (SetId, Name, Title, Header, Theme, Elements, Qnt, ImageName,
             GroupB, GroupI, GroupN, GroupG, GroupO)
        VALUES
            (0, @Name, @Title, @Header, @Theme, @Elements, @Qnt, @ImageName,
             @GroupB, @GroupI, @GroupN, @GroupG, @GroupO);";
            using (var cmd = new SQLiteCommand(insertSet, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Header", header);
                cmd.Parameters.AddWithValue("@Theme", theme);
                cmd.Parameters.AddWithValue("@Elements", elements);
                cmd.Parameters.AddWithValue("@Qnt", quantity);
                cmd.Parameters.AddWithValue("@ImageName", image);
                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);
                cmd.ExecuteNonQuery();
            }

            // Elementos únicos usados (pelos grupos)
            var allIds = new[] { groupB, groupI, groupN, groupG, groupO }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Where(s => int.TryParse(s, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            var allElements = GetElementsByIds(allIds);
            foreach (var e in allElements)
            {
                const string insertElement = @"
            INSERT INTO ElementsTable (Id, Name, CardName, ImageName)
            VALUES (@Id, @Name, @CardName, @ImageName);";
                using var cmd = new SQLiteCommand(insertElement, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e["Id"]));
                cmd.Parameters.AddWithValue("@Name", e["Name"]?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@CardName", e["CardName"]?.ToString() ?? "");
                var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(img));
                cmd.ExecuteNonQuery();
            }

            // Cartelas (5x5)
            var cards = GetCards5BySetId(setId);
            foreach (var card in cards)
            {
                const string insertCard = @"
            INSERT INTO CardsList (
                Id, CardNumber, SetId,
                EleB1, EleB2, EleB3, EleB4, EleB5,
                EleI1, EleI2, EleI3, EleI4, EleI5,
                EleN1, EleN2, EleN3, EleN4, EleN5,
                EleG1, EleG2, EleG3, EleG4, EleG5,
                EleO1, EleO2, EleO3, EleO4, EleO5)
            VALUES (
                @Id, @CardNumber, 0,
                @B1, @B2, @B3, @B4, @B5,
                @I1, @I2, @I3, @I4, @I5,
                @N1, @N2, @N3, @N4, @N5,
                @G1, @G2, @G3, @G4, @G5,
                @O1, @O2, @O3, @O4, @O5);";
                using var cmd = new SQLiteCommand(insertCard, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(card["Id"]));
                cmd.Parameters.AddWithValue("@CardNumber", Convert.ToInt32(card["CardNumber"]));
                for (int i = 1; i <= 5; i++)
                {
                    cmd.Parameters.AddWithValue($"@B{i}", Convert.ToInt32(card[$"EleB{i}"]));
                    cmd.Parameters.AddWithValue($"@I{i}", Convert.ToInt32(card[$"EleI{i}"]));
                    cmd.Parameters.AddWithValue($"@N{i}", Convert.ToInt32(card[$"EleN{i}"]));
                    cmd.Parameters.AddWithValue($"@G{i}", Convert.ToInt32(card[$"EleG{i}"]));
                    cmd.Parameters.AddWithValue($"@O{i}", Convert.ToInt32(card[$"EleO{i}"]));
                }
                cmd.ExecuteNonQuery();
            }
        }
        public static void ExportGame4(SQLiteConnection conn, int setId)
        {
            DataRow setRow = GetCardSet4ById(setId)
                ?? throw new Exception($"Conjunto 4×4 SetId={setId} não encontrado.");

            string title = setRow["Title"]?.ToString() ?? "";
            string name = setRow["Name"]?.ToString() ?? "";
            string image = setRow["ImageName"]?.ToString() ?? "";
            int quantity = Convert.ToInt32(setRow["Quantity"]);
            string elements = setRow["Elements"]?.ToString() ?? "";
            string theme = setRow["Theme"]?.ToString() ?? "";

            // CardsSets (DB de saída; SetId=0)
            const string insertSet = @"
        INSERT INTO CardsSets (SetId, Name, Title, Qnt, ImageName, Theme, Elements)
        VALUES (0, @Name, @Title, @Qnt, @ImageName, @Theme, @Elements);";
            using (var cmd = new SQLiteCommand(insertSet, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Qnt", quantity);
                cmd.Parameters.AddWithValue("@ImageName", image);
                cmd.Parameters.AddWithValue("@Theme", theme);
                cmd.Parameters.AddWithValue("@Elements", elements);
                cmd.ExecuteNonQuery();
            }

            // Elementos necessários (garantia: pelos IDs do elements + varredura das cartelas)
            var allIds = (elements ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => int.TryParse(s, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            var cardsRows = GetCards4BySetId(setId);
            foreach (var card in cardsRows)
                for (int i = 1; i <= 16; i++)
                {
                    int id = Convert.ToInt32(card[$"Ele{i}"]);
                    if (!allIds.Contains(id)) allIds.Add(id);
                }

            var allElements = GetElementsByIds(allIds);
            foreach (var e in allElements)
            {
                const string insertElement = @"
            INSERT INTO ElementsTable (Id, Name, CardName, ImageName)
            VALUES (@Id, @Name, @CardName, @ImageName);";
                using var cmd = new SQLiteCommand(insertElement, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e["Id"]));
                cmd.Parameters.AddWithValue("@Name", e["Name"]?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@CardName", e["CardName"]?.ToString() ?? "");
                var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(img));
                cmd.ExecuteNonQuery();
            }

            // Cartelas (4x4)
            foreach (var card in cardsRows)
            {
                const string insertCard = @"
            INSERT INTO CardsList (
                Id, CardNumber, SetId,
                Ele1, Ele2, Ele3, Ele4,
                Ele5, Ele6, Ele7, Ele8,
                Ele9, Ele10, Ele11, Ele12,
                Ele13, Ele14, Ele15, Ele16)
            VALUES (
                @Id, @CardNumber, 0,
                @E1, @E2, @E3, @E4,
                @E5, @E6, @E7, @E8,
                @E9, @E10, @E11, @E12,
                @E13, @E14, @E15, @E16);";
                using var cmd = new SQLiteCommand(insertCard, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(card["Id"]));
                cmd.Parameters.AddWithValue("@CardNumber", Convert.ToInt32(card["CardNumber"]));
                for (int i = 1; i <= 16; i++)
                    cmd.Parameters.AddWithValue($"@E{i}", Convert.ToInt32(card[$"Ele{i}"]));
                cmd.ExecuteNonQuery();
            }
        }
        private static void ExportGame5Subset(SQLiteConnection conn, int setId, HashSet<int> cardNumbers)
        {
            var setRow = GetCardSet5ById(setId)
                ?? throw new Exception($"Conjunto 5×5 SetId={setId} não encontrado.");

            string title = setRow["Title"]?.ToString() ?? "";
            string name = setRow["Name"]?.ToString() ?? "";
            string header = setRow.Table.Columns.Contains("Header") ? setRow["Header"]?.ToString() ?? "" : "";
            string theme = setRow["Theme"]?.ToString() ?? "";
            string elements = setRow["Elements"]?.ToString() ?? "";
            string image = setRow["ImageName"]?.ToString() ?? "";
            string groupB = setRow["GroupB"]?.ToString() ?? "";
            string groupI = setRow["GroupI"]?.ToString() ?? "";
            string groupN = setRow["GroupN"]?.ToString() ?? "";
            string groupG = setRow["GroupG"]?.ToString() ?? "";
            string groupO = setRow["GroupO"]?.ToString() ?? "";

            var allCards = GetCards5BySetId(setId);
            var subset = allCards
                .Where(r => cardNumbers.Contains(Convert.ToInt32(r["CardNumber"])))
                .OrderBy(r => Convert.ToInt32(r["CardNumber"]))
                .ToList();

            int qnt = subset.Count;

            // CardsSets (DB de saída; SetId=0) — mantém metadados completos e Qnt do subset
            const string insertSet = @"
        INSERT INTO CardsSets
            (SetId, Name, Title, Header, Theme, Elements, Qnt, ImageName,
             GroupB, GroupI, GroupN, GroupG, GroupO)
        VALUES
            (0, @Name, @Title, @Header, @Theme, @Elements, @Qnt, @ImageName,
             @GroupB, @GroupI, @GroupN, @GroupG, @GroupO);";
            using (var cmd = new SQLiteCommand(insertSet, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Header", header);
                cmd.Parameters.AddWithValue("@Theme", theme);
                cmd.Parameters.AddWithValue("@Elements", elements);
                cmd.Parameters.AddWithValue("@Qnt", qnt);
                cmd.Parameters.AddWithValue("@ImageName", image);
                cmd.Parameters.AddWithValue("@GroupB", groupB);
                cmd.Parameters.AddWithValue("@GroupI", groupI);
                cmd.Parameters.AddWithValue("@GroupN", groupN);
                cmd.Parameters.AddWithValue("@GroupG", groupG);
                cmd.Parameters.AddWithValue("@GroupO", groupO);
                cmd.ExecuteNonQuery();
            }

            // elementos efetivamente usados no subset
            var usedIds = new HashSet<int>();
            foreach (var card in subset)
                for (int i = 1; i <= 5; i++)
                {
                    usedIds.Add(Convert.ToInt32(card[$"EleB{i}"]));
                    usedIds.Add(Convert.ToInt32(card[$"EleI{i}"]));
                    usedIds.Add(Convert.ToInt32(card[$"EleN{i}"]));
                    usedIds.Add(Convert.ToInt32(card[$"EleG{i}"]));
                    usedIds.Add(Convert.ToInt32(card[$"EleO{i}"]));
                }

            var elems = GetElementsByIds(usedIds.ToList());
            foreach (DataRow e in elems)
            {
                const string insertElement = @"
            INSERT INTO ElementsTable (Id, Name, CardName, ImageName)
            VALUES (@Id, @Name, @CardName, @ImageName);";
                using var cmd = new SQLiteCommand(insertElement, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e["Id"]));
                cmd.Parameters.AddWithValue("@Name", e["Name"]?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@CardName", e["CardName"]?.ToString() ?? "");
                var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(img));
                cmd.ExecuteNonQuery();
            }

            // grava somente as cartelas do subset
            foreach (var card in subset)
            {
                const string insertCard = @"
            INSERT INTO CardsList (
                Id, CardNumber, SetId,
                EleB1, EleB2, EleB3, EleB4, EleB5,
                EleI1, EleI2, EleI3, EleI4, EleI5,
                EleN1, EleN2, EleN3, EleN4, EleN5,
                EleG1, EleG2, EleG3, EleG4, EleG5,
                EleO1, EleO2, EleO3, EleO4, EleO5)
            VALUES (
                @Id, @CardNumber, 0,
                @B1, @B2, @B3, @B4, @B5,
                @I1, @I2, @I3, @I4, @I5,
                @N1, @N2, @N3, @N4, @N5,
                @G1, @G2, @G3, @G4, @G5,
                @O1, @O2, @O3, @O4, @O5);";
                using var cmd = new SQLiteCommand(insertCard, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(card["Id"]));
                cmd.Parameters.AddWithValue("@CardNumber", Convert.ToInt32(card["CardNumber"]));
                for (int i = 1; i <= 5; i++)
                {
                    cmd.Parameters.AddWithValue($"@B{i}", Convert.ToInt32(card[$"EleB{i}"]));
                    cmd.Parameters.AddWithValue($"@I{i}", Convert.ToInt32(card[$"EleI{i}"]));
                    cmd.Parameters.AddWithValue($"@N{i}", Convert.ToInt32(card[$"EleN{i}"]));
                    cmd.Parameters.AddWithValue($"@G{i}", Convert.ToInt32(card[$"EleG{i}"]));
                    cmd.Parameters.AddWithValue($"@O{i}", Convert.ToInt32(card[$"EleO{i}"]));
                }
                cmd.ExecuteNonQuery();
            }
        }
        private static void ExportGame4Subset(SQLiteConnection conn, int setId, HashSet<int> cardNumbers)
        {
            var setRow = GetCardSet4ById(setId)
                ?? throw new Exception($"Conjunto 4×4 SetId={setId} não encontrado.");

            string name = setRow["Name"]?.ToString() ?? "";
            string title = setRow["Title"]?.ToString() ?? "";
            string theme = setRow["Theme"]?.ToString() ?? "";
            string image = setRow["ImageName"]?.ToString() ?? "";

            var allCards = GetCards4BySetId(setId);
            var subset = allCards
                .Where(r => cardNumbers.Contains(Convert.ToInt32(r["CardNumber"])))
                .OrderBy(r => Convert.ToInt32(r["CardNumber"]))
                .ToList();

            int qnt = subset.Count;

            // elementos efetivamente usados no subset
            var usedIds = new HashSet<int>();
            foreach (var card in subset)
                for (int i = 1; i <= 16; i++)
                    usedIds.Add(Convert.ToInt32(card[$"Ele{i}"]));

            string elementsCsv = string.Join(",", usedIds.OrderBy(x => x));

            // CardsSets (DB de saída; SetId=0) — inclui todos campos do schema 4×4
            const string insertSet = @"
        INSERT INTO CardsSets (SetId, Name, Title, Qnt, ImageName, Theme, Elements)
        VALUES (0, @Name, @Title, @Qnt, @ImageName, @Theme, @Elements);";
            using (var cmd = new SQLiteCommand(insertSet, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Qnt", qnt);
                cmd.Parameters.AddWithValue("@ImageName", image);
                cmd.Parameters.AddWithValue("@Theme", theme);
                cmd.Parameters.AddWithValue("@Elements", elementsCsv);
                cmd.ExecuteNonQuery();
            }

            var elems = GetElementsByIds(usedIds.ToList());
            foreach (DataRow e in elems)
            {
                const string insertElement = @"
            INSERT INTO ElementsTable (Id, Name, CardName, ImageName)
            VALUES (@Id, @Name, @CardName, @ImageName);";
                using var cmd = new SQLiteCommand(insertElement, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e["Id"]));
                cmd.Parameters.AddWithValue("@Name", e["Name"]?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@CardName", e["CardName"]?.ToString() ?? "");
                var img = e.Table.Columns.Contains("ImageName") ? e["ImageName"]?.ToString() ?? "" : "";
                cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(img));
                cmd.ExecuteNonQuery();
            }

            foreach (var card in subset)
            {
                const string insertCard = @"
            INSERT INTO CardsList (
                Id, CardNumber, SetId,
                Ele1, Ele2, Ele3, Ele4,
                Ele5, Ele6, Ele7, Ele8,
                Ele9, Ele10, Ele11, Ele12,
                Ele13, Ele14, Ele15, Ele16)
            VALUES (
                @Id, @CardNumber, 0,
                @E1, @E2, @E3, @E4,
                @E5, @E6, @E7, @E8,
                @E9, @E10, @E11, @E12,
                @E13, @E14, @E15, @E16);";
                using var cmd = new SQLiteCommand(insertCard, conn);
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(card["Id"]));
                cmd.Parameters.AddWithValue("@CardNumber", Convert.ToInt32(card["CardNumber"]));
                for (int i = 1; i <= 16; i++)
                    cmd.Parameters.AddWithValue($"@E{i}", Convert.ToInt32(card[$"Ele{i}"]));
                cmd.ExecuteNonQuery();
            }
        }
        // Exporta o WASM
        // Exemplo: exporta DB e também o pacote WASM (.zip)
        public static void ExportGameWithWasmPackage(
            int setId,
            string dbOutputPath,
            string wasmZipOutputPath,
            bool subset = false,
            HashSet<int>? subsetCardNumbers = null)
        {
            // 1) Exporta o .db como você já faz hoje
            if (!subset)
                ExportGameDatabaseToPath(setId, dbOutputPath);
            else
                ExportGameDatabaseSubsetToPath(setId, subsetCardNumbers ?? new HashSet<int>(), dbOutputPath);

            // 2) Gera o pacote WASM (.zip) – sem imagens (você adiciona a pasta /images manualmente)
            BingoCreator.Services.WASMService.ExportWasmPackage(
                setId,
                wasmZipOutputPath,
                isSubset: subset,
                subsetCardNumbers: subsetCardNumbers
            );
        }





        // Métodos de Busca
        // Encontrar Elemento pelo ID
        public static DataRow GetElementById(int elementId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM ElementsTable WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", elementId);

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable elementsTable = new DataTable();
                        adapter.Fill(elementsTable);

                        if (elementsTable.Rows.Count > 0)
                        {
                            return elementsTable.Rows[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        // Retornar todos os Elementos em uma Lista
        public static List<DataRow> GetElementsByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<DataRow>();

            string idList = string.Join(",", ids);
            string sql = $"SELECT Id, Name, CardName, ImageName FROM ElementsTable WHERE Id IN ({idList})";

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt.AsEnumerable().ToList();
        }
        // Método para retornar o ID de um Elemento pelo CardName
        public static int GetElementByCardName(string cardName)
        {
            using var connection = GetConnection();
            connection.Open();

            string query = "SELECT Id FROM ElementsTable WHERE CardName = @CardName LIMIT 1";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@CardName", cardName);

            object result = command.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int id))
            {
                return id;
            }

            return 0;
        }
        // Conferir se um Elemento já existe em uma Lista
        public static bool ElementExist(string cardName, string listName)
        {
            // Conferir se já existe um elemento em uma lista com o mesmo cardName
            throw new NotImplementedException();
        }
        // Método para retornar todas as Listas
        public static DataTable GetLists()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string selectQuery = "SELECT * FROM ListsTable;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }               
        // Método para retornar todos os Elementos de uma Lista
        public static List<DataRow> GetElementsInList(int listId)
        {
            List<DataRow> elementsList = new List<DataRow>();

            string query = "SELECT c.Id, c.Name, c.CardName, c.ImageName " +
                           "FROM ElementsTable c " +
                           "INNER JOIN AlocationTable a ON c.Id = a.ElementId " +
                           "WHERE a.ListId = @ListId";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable elementsTable = new DataTable();
                        adapter.Fill(elementsTable);

                        elementsList = elementsTable.AsEnumerable().ToList();
                    }
                }
            }

            return elementsList;
        }
        // Método para retornar uma Lista pelo Id
        public static ListModel? GetListById(int listId)
        {
            const string sql = @"
        SELECT 
            l.Id,
            l.Name,
            l.Description,
            l.ImageName,
            COUNT(a.ElementId) AS ElementCount
        FROM ListsTable l
        LEFT JOIN AlocationTable a ON a.ListId = l.Id
        WHERE l.Id = @ListId
        GROUP BY l.Id, l.Name, l.Description, l.ImageName
        LIMIT 1;";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ListId", listId);

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count == 0) return null;

                        var row = dt.Rows[0];
                        return new ListModel
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"]?.ToString() ?? "",
                            Description = row["Description"]?.ToString() ?? "",
                            ImageName = row["ImageName"]?.ToString() ?? "",
                            ElementCount = row["ElementCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["ElementCount"])
                        };
                    }
                }
            }
        }
        // Método para retornar um Card Set pelo Id
        public static CardSetModel? GetCardSetById(int setId)
        {
            const string sql = @"
        SELECT
            SetId,
            ListId,
            Name,
            Title,
            End,
            Quantity,
            CardsSize,
            AddTime,
            GroupB,
            GroupI,
            GroupN,
            GroupG,
            GroupO,
            Elements,
            Theme,
            Header,
            Model
        FROM CardsSets
        WHERE SetId = @SetId
        LIMIT 1;";

            using var connection = GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@SetId", setId);

            using var adapter = new SQLiteDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];

            // ————— Helpers locais —————
            static List<int> ParseIds(string? csv)
            {
                if (string.IsNullOrWhiteSpace(csv)) return new List<int>();
                return csv
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0 && int.TryParse(s, out _))
                    .Select(int.Parse)
                    .ToList();
            }

            static List<ElementModel> LoadElements(List<int> ids)
            {
                if (ids.Count == 0) return new List<ElementModel>();
                var rows = GetElementsByIds(ids); // já existe no seu DataService
                return rows.Select(r => new ElementModel
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? "",
                    CardName = r["CardName"]?.ToString() ?? "",
                    // Se precisar: ImageName = r.Table.Columns.Contains("ImageName") ? r["ImageName"]?.ToString() ?? "" : ""
                }).ToList();
            }
            // ————————————————————————

            var model = new CardSetModel
            {
                Id = Convert.ToInt32(row["SetId"]),
                ListId = Convert.ToInt32(row["ListId"]),
                Name = row["Name"]?.ToString() ?? "",
                Title = row["Title"]?.ToString() ?? "",
                End = row["End"]?.ToString() ?? "",
                Quantity = Convert.ToInt32(row["Quantity"]),
                CardsSize = Convert.ToInt32(row["CardsSize"]),
                AddDate = row["AddTime"]?.ToString() ?? "",

                // CSVs dos grupos/elements
                GroupBIds = row["GroupB"]?.ToString() ?? "",
                GroupIIds = row["GroupI"]?.ToString() ?? "",
                GroupNIds = row["GroupN"]?.ToString() ?? "",
                GroupGIds = row["GroupG"]?.ToString() ?? "",
                GroupOIds = row["GroupO"]?.ToString() ?? "",

                // 4x4
                // Elements está na coluna "Elements" — será usado para preencher AllElements

                // Estilo/tema/cabeçalho
                Theme = row.Table.Columns.Contains("Theme") ? (row["Theme"]?.ToString() ?? "") : "",
                Header = row.Table.Columns.Contains("Header") ? (row["Header"]?.ToString() ?? "") : "",
                Model = row.Table.Columns.Contains("Model") ? (row["Model"]?.ToString() ?? "") : ""
            };

            // Preencher ListName e ListSize (ElementCount) via ListsTable
            var list = GetListById(model.ListId); // seu método que retorna ListModel?
            model.ListName = list?.Name ?? "";
            model.ListSize = list?.ElementCount ?? 0;

            // Carregar elementos conforme o tamanho das cartelas
            if (model.CardsSize == 5)
            {
                var idsB = ParseIds(model.GroupBIds);
                var idsI = ParseIds(model.GroupIIds);
                var idsN = ParseIds(model.GroupNIds);
                var idsG = ParseIds(model.GroupGIds);
                var idsO = ParseIds(model.GroupOIds);

                model.GroupB = LoadElements(idsB);
                model.GroupI = LoadElements(idsI);
                model.GroupN = LoadElements(idsN);
                model.GroupG = LoadElements(idsG);
                model.GroupO = LoadElements(idsO);

                // União para AllElements (sem duplicatas)
                model.AllElements = new[] { model.GroupB, model.GroupI, model.GroupN, model.GroupG, model.GroupO }
                    .Where(g => g != null)
                    .SelectMany(g => g!)
                    .GroupBy(e => e.Id)
                    .Select(g => g.First())
                    .ToList();
            }
            else if (model.CardsSize == 4)
            {
                var rows = GetElementsInList(model.ListId); // DataRows: Id, Name, CardName, ImageName
                model.AllElements = rows
                    .Select(r => new ElementModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        Name = r["Name"]?.ToString() ?? "",
                        CardName = r["CardName"]?.ToString() ?? ""
                    })
                    .OrderBy(e => string.IsNullOrWhiteSpace(e.CardName) ? e.Name : e.CardName)
                    .ToList();

                // Mantém Group* como null em 4x4
                model.GroupB = null;
                model.GroupI = null;
                model.GroupN = null;
                model.GroupG = null;
                model.GroupO = null;
            }
            else
            {
                // Se algum dado antigo tiver CardsSize diferente, apenas evita nulls
                model.AllElements ??= new List<ElementModel>();
            }

            return model;
        }
        // Método par retornar todas as Cards de um Set
        public static List<DataRow> GetCardsBySetId(int setId)
        {
            const string metaSql = @"
        SELECT CardsSize
        FROM CardsSets
        WHERE SetId = @SetId
        LIMIT 1;";

            using var conn = GetConnection();
            conn.Open();

            using (var metaCmd = new SQLiteCommand(metaSql, conn))
            {
                metaCmd.Parameters.AddWithValue("@SetId", setId);
                object? cs = metaCmd.ExecuteScalar();
                if (cs == null) return new List<DataRow>();

                int cardsSize = Convert.ToInt32(cs);

                string listSql = cardsSize == 5
                    ? "SELECT * FROM CardsList5Table WHERE SetId = @SetId ORDER BY CardNumber;"
                    : "SELECT * FROM CardsList4Table WHERE SetId = @SetId ORDER BY CardNumber;";

                using var listCmd = new SQLiteCommand(listSql, conn);
                listCmd.Parameters.AddWithValue("@SetId", setId);

                using var adp = new SQLiteDataAdapter(listCmd);
                var dt = new DataTable();
                adp.Fill(dt);
                return dt.AsEnumerable().ToList();
            }
        }
        // Método para retornar todos os Elementos de uma cartela por CardSet
        public static List<List<ElementModel>> GetCardElementsBySet(List<DataRow> setCards)
        {
            var result = new List<List<ElementModel>>();
            if (setCards == null || setCards.Count == 0) return result;

            // 1) Coletar TODOS os IDs utilizados em todas as cartelas (para uma única ida ao DB)
            var allIds = setCards
                .SelectMany(GetIdsFromCardRowAuto) // detecta 5x5 ou 4x4 pela presença das colunas
                .Distinct()
                .ToList();

            if (allIds.Count == 0) return result;

            // 2) Buscar os elementos e indexar por Id
            var elementRows = DataService.GetElementsByIds(allIds); // seu método existente
            var elemById = elementRows.ToDictionary(r => Convert.ToInt32(r["Id"]));

            // 3) Montar, para cada cartela, a lista de ElementModel na ordem esperada
            foreach (var row in setCards)
            {
                var ids = GetIdsFromCardRowAuto(row);
                var oneCard = new List<ElementModel>();

                foreach (var id in ids)
                {
                    if (!elemById.TryGetValue(id, out var er)) continue;

                    oneCard.Add(new ElementModel
                    {
                        Id = id,
                        Name = er["Name"]?.ToString() ?? "",
                        CardName = er["CardName"]?.ToString() ?? "",
                        // Se precisar:
                        // ImageName = er.Table.Columns.Contains("ImageName") ? er["ImageName"]?.ToString() ?? "" : ""
                    });
                }

                result.Add(oneCard);
            }

            return result;

            // ===== Helpers =====

            // Detecta automaticamente o tipo de cartela pela existência das colunas
            static IEnumerable<int> GetIdsFromCardRowAuto(DataRow r)
            {
                if (r.Table.Columns.Contains("EleB1"))
                {
                    // 5x5: B1..B5, I1..I5, N1..N5, G1..G5, O1..O5
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleB{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleI{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleN{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleG{i}"]);
                    for (int i = 1; i <= 5; i++) yield return ToId(r[$"EleO{i}"]);
                }
                else
                {
                    // 4x4: Ele1..Ele16
                    for (int i = 1; i <= 16; i++) yield return ToId(r[$"Ele{i}"]);
                }
            }

            static int ToId(object value)
            {
                // defensivo contra DBNull/strings
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }
        // Método para retornar todos os Elementos criados
        public static DataTable GetAllElements()
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = @"
        SELECT Id,
               CASE WHEN IFNULL(TRIM(CardName),'') <> '' THEN CardName ELSE Name END AS CardName,
               Name
        FROM ElementsTable
        ORDER BY CardName, Name;";
            using var cmd = new SQLiteCommand(sql, conn);
            using var adp = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        // Método para retornar todos os Card Sets criados
        public static DataTable GetAllCardSets()
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            const string sql = @"
        SELECT SetId AS Id, Name, Title, CardsSize, Quantity, AddTime
        FROM CardsSets
        ORDER BY datetime(AddTime) DESC;";
            using var cmd = new SQLiteCommand(sql, conn);
            using var adp = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        // Método para retornar todas as Listas que contém determinado Elemento
        public static string GetListsForElement(int elementId)
        {
            const string sql = @"
        SELECT l.Name
        FROM AlocationTable a
        INNER JOIN ListsTable l ON l.Id = a.ListId
        WHERE a.ElementId = @ElementId
        ORDER BY l.Name;";

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ElementId", elementId);

            using var reader = cmd.ExecuteReader();

            var names = new List<string>();
            while (reader.Read())
            {
                var name = reader["Name"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name.Trim());
            }

            // retorna string vazia se não houver listas
            return string.Join("; ", names);
        }
        // Método para retornar todos os CardSets que usam determinada Lista
        public static DataTable GetCardSetsByListId(int listId)
        {
            using var conn = GetConnection();
            conn.Open();

            // Obs.: se quiser por data, troque para ORDER BY datetime(AddTime) DESC,
            // mas seu formato "MMddyyyy - HH:mm:ss" pode não ordenar bem como datetime do SQLite.
            const string sql = @"
        SELECT SetId, Name, Title, CardsSize, Quantity, AddTime
        FROM CardsSets
        WHERE ListId = @ListId
        ORDER BY SetId DESC;";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ListId", listId);

            using var adp = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }


        // Métodos de Edição

        // Apaga TODAS as cartelas (4x4 e 5x5) de um CardSet
        public static int DeleteCardsBySetId(int setId)
        {
            using var conn = GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            int total = 0;

            using (var cmd = new SQLiteCommand("DELETE FROM CardsList5Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                total += cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("DELETE FROM CardsList4Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                total += cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return total; // total de linhas removidas das tabelas de cartelas
        }

        // Apaga o CardSet e todas as suas cartelas
        public static bool DeleteCardSet(int setId)
        {
            using var conn = GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // 1) Apaga cartelas relacionadas
            using (var cmd = new SQLiteCommand("DELETE FROM CardsList5Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM CardsList4Table WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                cmd.ExecuteNonQuery();
            }

            // 2) Apaga o CardSet em si
            int affected;
            using (var cmd = new SQLiteCommand("DELETE FROM CardsSets WHERE SetId = @SetId;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@SetId", setId);
                affected = cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return affected > 0; // true se o CardSet existia e foi apagado
        }

        // Apaga uma Lista e todas suas Alocações
        public static bool DeleteListAndAllocations(int listId)
        {
            using var conn = GetConnection();
            conn.Open();
            int affected = 0;

            using var tx = conn.BeginTransaction();

            // 1) Checa se a lista está sendo usada por algum conjunto
            const string checkSql = @"SELECT 1 FROM CardsSets WHERE ListId = @ListId LIMIT 1;";
            using (var checkCmd = new SQLiteCommand(checkSql, conn, tx))
            {
                checkCmd.Parameters.AddWithValue("@ListId", listId);
                var inUse = checkCmd.ExecuteScalar();
                if (inUse != null) // há pelo menos um registro
                {
                    tx.Rollback();
                    throw new InvalidOperationException("Esta lista está em uso por um ou mais conjuntos e não pode ser excluída.");
                }
            }

            // 2) Remove alocações (ElementId <-> ListId)
            const string delAllocSql = @"DELETE FROM AlocationTable WHERE ListId = @ListId;";
            using (var delAlloc = new SQLiteCommand(delAllocSql, conn, tx))
            {
                delAlloc.Parameters.AddWithValue("@ListId", listId);
                delAlloc.ExecuteNonQuery();
            }

            // 3) Remove a própria lista
            const string delListSql = @"DELETE FROM ListsTable WHERE Id = @ListId;";
            using (var delList = new SQLiteCommand(delListSql, conn, tx))
            {
                delList.Parameters.AddWithValue("@ListId", listId);
                affected = delList.ExecuteNonQuery();
                if (affected == 0)
                {
                    tx.Rollback();
                    throw new InvalidOperationException("Lista não encontrada para exclusão.");
                }
            }

            tx.Commit();
            return affected > 0;
        }

        // Apaga um Elemento
        public static bool DeleteElement(int elementId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            // 1) Bloqueia exclusão se o elemento estiver alocado em alguma lista
            const string sqlAlloc = "SELECT 1 FROM AlocationTable WHERE ElementId = @Id LIMIT 1;";
            using (var cmd = new SQLiteCommand(sqlAlloc, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var hasAlloc = cmd.ExecuteScalar();
                if (hasAlloc != null)
                    return false; // ainda está em alguma lista
            }

            // 2) Bloqueia exclusão se o elemento aparece em alguma cartela 5x5
            //    (qualquer uma das 25 colunas EleB*,EleI*,EleN*,EleG*,EleO*)
            var where5 = new StringBuilder();
            string[] colsB = Enumerable.Range(1, 5).Select(i => $"EleB{i}").ToArray();
            string[] colsI = Enumerable.Range(1, 5).Select(i => $"EleI{i}").ToArray();
            string[] colsN = Enumerable.Range(1, 5).Select(i => $"EleN{i}").ToArray();
            string[] colsG = Enumerable.Range(1, 5).Select(i => $"EleG{i}").ToArray();
            string[] colsO = Enumerable.Range(1, 5).Select(i => $"EleO{i}").ToArray();
            string[] all5 = colsB.Concat(colsI).Concat(colsN).Concat(colsG).Concat(colsO).ToArray();

            for (int i = 0; i < all5.Length; i++)
            {
                if (i > 0) where5.Append(" OR ");
                where5.Append(all5[i]).Append(" = @Id");
            }

            string sql5 = $"SELECT 1 FROM CardsList5Table WHERE {where5} LIMIT 1;";
            using (var cmd = new SQLiteCommand(sql5, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var inCards5 = cmd.ExecuteScalar();
                if (inCards5 != null)
                    return false; // ainda está em alguma cartela 5x5
            }

            // 3) Bloqueia exclusão se o elemento aparece em alguma cartela 4x4 (Ele1..Ele16)
            var where4 = new StringBuilder();
            string[] all4 = Enumerable.Range(1, 16).Select(i => $"Ele{i}").ToArray();
            for (int i = 0; i < all4.Length; i++)
            {
                if (i > 0) where4.Append(" OR ");
                where4.Append(all4[i]).Append(" = @Id");
            }

            string sql4 = $"SELECT 1 FROM CardsList4Table WHERE {where4} LIMIT 1;";
            using (var cmd = new SQLiteCommand(sql4, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                var inCards4 = cmd.ExecuteScalar();
                if (inCards4 != null)
                    return false; // ainda está em alguma cartela 4x4
            }

            // 4) Pode excluir
            const string sqlDel = "DELETE FROM ElementsTable WHERE Id = @Id;";
            using (var cmd = new SQLiteCommand(sqlDel, conn))
            {
                cmd.Parameters.AddWithValue("@Id", elementId);
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
        }

        // Apaga todos os Elementos de uma Lista
        public static (int unallocated, int deleted) DeleteElementsInList(int listId, bool deleteOrphanElements = false)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            // 1) Coletar IDs de elementos alocados nessa lista
            var elementIds = new List<int>();
            const string selSql = @"SELECT ElementId FROM AlocationTable WHERE ListId = @ListId;";
            using (var selCmd = new SQLiteCommand(selSql, conn))
            {
                selCmd.Parameters.AddWithValue("@ListId", listId);
                using var rd = selCmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["ElementId"] != DBNull.Value)
                        elementIds.Add(Convert.ToInt32(rd["ElementId"]));
                }
            }

            if (elementIds.Count == 0)
                return (0, 0); // nada para remover

            // 2) Remover TODAS as alocações desta lista em transação
            int unallocated = 0;
            using (var tx = conn.BeginTransaction())
            {
                const string delAllocSql = @"DELETE FROM AlocationTable WHERE ListId = @ListId;";
                using var delCmd = new SQLiteCommand(delAllocSql, conn, tx);
                delCmd.Parameters.AddWithValue("@ListId", listId);
                unallocated = delCmd.ExecuteNonQuery();
                tx.Commit();
            }

            // 3) Opcional: tentar excluir elementos órfãos (sem refs em outras listas/cartelas)
            int deleted = 0;
            if (deleteOrphanElements)
            {
                foreach (var eid in elementIds.Distinct())
                {
                    // Se ainda restar alocação em outra lista, nem tenta.
                    const string stillAllocSql = @"SELECT 1 FROM AlocationTable WHERE ElementId = @Id LIMIT 1;";
                    using (var checkCmd = new SQLiteCommand(stillAllocSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", eid);
                        var hasAlloc = checkCmd.ExecuteScalar();
                        if (hasAlloc != null)
                            continue; // ainda alocado em outra lista
                    }

                    // Usa o DeleteElement que você já tem (bloqueia se estiver em cartelas)
                    if (DeleteElement(eid))
                        deleted++;
                }
            }

            return (unallocated, deleted);
        }

        // Edita um Elemento e salva versão anterior
        public static int EditElement(int oldElementId, ElementModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            // carrega o antigo
            const string sel = @"
        SELECT Id, Name, CardName, Note1, Note2, ImageName,
               COALESCE(ParentId,0) AS ParentId,
               COALESCE(Version,1)  AS Version
        FROM ElementsTable
        WHERE Id=@Id
        LIMIT 1;";
            DataRow old;
            using (var cmd = new SQLiteCommand(sel, conn))
            {
                cmd.Parameters.AddWithValue("@Id", oldElementId);
                using var adp = new SQLiteDataAdapter(cmd);
                var dt = new DataTable(); adp.Fill(dt);
                if (dt.Rows.Count == 0)
                    throw new InvalidOperationException($"Elemento {oldElementId} não encontrado.");
                old = dt.Rows[0];
            }

            int oldId = Convert.ToInt32(old["Id"]);
            int oldParent = Convert.ToInt32(old["ParentId"]);
            int oldVersion = Convert.ToInt32(old["Version"]);
            int newParent = oldParent > 0 ? oldParent : oldId;
            int newVersion = (oldVersion >= 1 ? oldVersion + 1 : 2);
            string now = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");

            using var tx = conn.BeginTransaction();

            // obsoleta antigo
            using (var upd = new SQLiteCommand("UPDATE ElementsTable SET Obsolete=1 WHERE Id=@Id;", conn, tx))
            {
                upd.Parameters.AddWithValue("@Id", oldId);
                upd.ExecuteNonQuery();
            }

            // cria o novo (usa valores do model; fallback pros antigos se vierem nulos)
            const string ins = @"
        INSERT INTO ElementsTable
            (Name, CardName, Note1, Note2, ImageName, AddTime, Obsolete, ParentId, Version)
        VALUES
            (@Name, @CardName, @Note1, @Note2, @ImageName, @AddTime, 0, @ParentId, @Version);
        SELECT last_insert_rowid();";
            using (var cmd = new SQLiteCommand(ins, conn, tx))
            {
                string name = model.Name ?? old["Name"]?.ToString() ?? "";
                string cardName = model.CardName ?? old["CardName"]?.ToString() ?? "";
                string note1 = model.Note1 ?? old["Note1"]?.ToString() ?? "";
                string note2 = model.Note2 ?? old["Note2"]?.ToString() ?? "";
                string imageName = model.ImageName ?? old["ImageName"]?.ToString() ?? "";

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@CardName", cardName);
                cmd.Parameters.AddWithValue("@Note1", note1);
                cmd.Parameters.AddWithValue("@Note2", note2);
                cmd.Parameters.AddWithValue("@ImageName", imageName);
                cmd.Parameters.AddWithValue("@AddTime", now);
                cmd.Parameters.AddWithValue("@ParentId", newParent);
                cmd.Parameters.AddWithValue("@Version", newVersion);

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                tx.Commit();
                return newId;
            }
        }

        // Edita um Elemento em todas as Listas
        public static int EditElementInList(int oldElementId, int newElementId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            // pega todas as listas onde o elemento antigo está alocado
            var listIds = new List<int>();
            using (var cmd = new SQLiteCommand("SELECT ListId FROM AlocationTable WHERE ElementId=@E;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@E", oldElementId);
                using var rd = cmd.ExecuteReader();
                while (rd.Read()) listIds.Add(Convert.ToInt32(rd["ListId"]));
            }

            int affected = 0;
            foreach (var listId in listIds)
            {
                // insere novo par se não existir
                using (var ins = new SQLiteCommand(
                    "INSERT OR IGNORE INTO AlocationTable (ElementId, ListId) VALUES (@NewE, @L);", conn, tx))
                {
                    ins.Parameters.AddWithValue("@NewE", newElementId);
                    ins.Parameters.AddWithValue("@L", listId);
                    ins.ExecuteNonQuery();
                }
                // remove o antigo
                using (var del = new SQLiteCommand(
                    "DELETE FROM AlocationTable WHERE ElementId=@OldE AND ListId=@L;", conn, tx))
                {
                    del.Parameters.AddWithValue("@OldE", oldElementId);
                    del.Parameters.AddWithValue("@L", listId);
                    affected += del.ExecuteNonQuery();
                }
            }

            tx.Commit();
            return affected; // número de remoções realizadas (aprox. listas afetadas)
        }

        // Edita um Elemento em todas as Cartelas
        public static int EditElementInCardSet(int oldElementId, int newElementId)
        {
            if (oldElementId <= 0 || newElementId <= 0 || oldElementId == newElementId)
                return 0;

            using var conn = GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            int total = 0;

            // --- 5x5: atualiza todas as 25 colunas possíveis ---
            string[] cols5 =
            {
        "EleB1","EleB2","EleB3","EleB4","EleB5",
        "EleI1","EleI2","EleI3","EleI4","EleI5",
        "EleN1","EleN2","EleN3","EleN4","EleN5",
        "EleG1","EleG2","EleG3","EleG4","EleG5",
        "EleO1","EleO2","EleO3","EleO4","EleO5"
    };

            foreach (var col in cols5)
            {
                string sql = $"UPDATE CardsList5Table SET {col} = @NewId WHERE {col} = @OldId;";
                using var cmd = new SQLiteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@NewId", newElementId);
                cmd.Parameters.AddWithValue("@OldId", oldElementId);
                total += cmd.ExecuteNonQuery();
            }

            // --- 4x4: Ele1..Ele16 ---
            for (int i = 1; i <= 16; i++)
            {
                string col = $"Ele{i}";
                string sql = $"UPDATE CardsList4Table SET {col} = @NewId WHERE {col} = @OldId;";
                using var cmd = new SQLiteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@NewId", newElementId);
                cmd.Parameters.AddWithValue("@OldId", oldElementId);
                total += cmd.ExecuteNonQuery();
            }

            tx.Commit();
            return total;
        }

        // Edita uma Lista (usa ListModel: Id, Name, Description, ImageName)
        public static bool EditList(ListModel list)
        {
            if (list == null || list.Id <= 0) return false;

            using var connection = GetConnection();
            connection.Open();

            const string sql = @"
        UPDATE ListsTable
        SET Name       = @Name,
            Description= @Description,
            ImageName  = @ImageName
        WHERE Id = @Id;";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", list.Id);
            cmd.Parameters.AddWithValue("@Name", (list.Name ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@Description", (list.Description ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@ImageName", (list.ImageName ?? string.Empty).Trim());

            int affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        // Edita um Conjunto (usa CardSetModel: Id, Name, Title, End)
        public static bool EditCardSet(CardSetModel set)
        {
            if (set == null || set.Id <= 0) return false;

            using var connection = GetConnection();
            connection.Open();

            const string sql = @"
        UPDATE CardsSets
        SET Name  = @Name,
            Title = @Title,
            End   = @End
        WHERE SetId = @SetId;";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@SetId", set.Id);
            cmd.Parameters.AddWithValue("@Name", (set.Name ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@Title", (set.Title ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@End", (set.End ?? string.Empty).Trim());

            int affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }

        // Helper para trocar Elemento das Cartelas
        private static string ReplaceIdInCsv(string csv, int oldId, int newId, out bool changed)
        {
            changed = false;
            if (string.IsNullOrWhiteSpace(csv)) return string.Empty;

            var outList = new List<string>();
            foreach (var tok in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = tok.Trim();
                if (t.Length == 0) continue;
                if (int.TryParse(t, out int v) && v == oldId)
                {
                    outList.Add(newId.ToString());
                    changed = true;
                }
                else
                {
                    outList.Add(t);
                }
            }
            return string.Join(",", outList);
        }

        // Helpers para Retornar ElementModel
        public static ElementModel? GetElementModelById(int elementId)
        {
            var row = GetElementById(elementId);
            if (row == null) return null;
            return MapElementRowToModel(row);
        }
        private static ElementModel MapElementRowToModel(DataRow r)
        {
            string Get(string col) =>
                (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value) ? r[col]!.ToString()! : string.Empty;

            int GetInt(string col, int def = 0)
            {
                if (r.Table?.Columns.Contains(col) == true && r[col] != DBNull.Value)
                    return Convert.ToInt32(r[col]);
                return def;
            }

            return new ElementModel
            {
                Id = GetInt("Id"),
                Name = Get("Name"),
                CardName = Get("CardName"),
                Note1 = Get("Note1"),
                Note2 = Get("Note2"),
                ImageName = Get("ImageName"),
                AddTime = Get("AddTime"),
                Obsolete = GetInt("Obsolete", 0) != 0,
                ParentId = GetInt("ParentId", 0),
                Version = GetInt("Version", 1),
            };
        }
    }
}
