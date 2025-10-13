using System.Collections.Generic;
using System.Linq;
using BingoPlayer.Models;

namespace BingoPlayer.Services
{
    /// <summary>
    /// Serviço de estado do jogo: mantém elementos sorteados e faz verificações de Bingo.
    /// Depende do PackageService para acessar GameModel/Cartelas/Elementos carregados de wwwroot/game.
    /// </summary>
    public sealed class PlayService
    {
        private readonly PackageService _pkg;

        // Estado do sorteio (em memória)
        private readonly HashSet<int> _drawnElements = new(); // IDs de elementos sorteados (toggle)
        private readonly HashSet<int> _drawnCards = new();    // reservado p/ uso futuro, se quiser marcar cartelas

        public PlayService(PackageService pkg) => _pkg = pkg;

        /// <summary>Lista imutável dos IDs sorteados até agora.</summary>
        public IReadOnlyCollection<int> DrawnElements => _drawnElements;

        /// <summary>Alterna um elemento: se não estiver sorteado, adiciona; se estiver, remove.</summary>
        public void ToggleElement(int elementId)
        {
            if (!_drawnElements.Add(elementId))
                _drawnElements.Remove(elementId);
        }

        /// <summary>Marca explicitamente um elemento como sorteado.</summary>
        public void AddElement(int elementId) => _drawnElements.Add(elementId);

        /// <summary>Desmarca explicitamente um elemento sorteado.</summary>
        public void RemoveElement(int elementId) => _drawnElements.Remove(elementId);

        /// <summary>Cartelas (números) que contêm o elemento informado.</summary>
        public List<int> CheckCards(int elementId)
        {
            var game = _pkg.Game ?? throw new System.InvalidOperationException("Jogo não carregado.");
            var list = new List<int>();

            foreach (var card in game.GameCards)
            {
                if (game.GameSize == 5)
                {
                    if (card.BElements.Contains(elementId) ||
                        card.IElements.Contains(elementId) ||
                        card.NElements.Contains(elementId) ||
                        card.GElements.Contains(elementId) ||
                        card.OElements.Contains(elementId))
                    {
                        list.Add(card.CardNumber);
                    }
                }
                else if (game.GameSize == 4)
                {
                    if (card.AllElements.Contains(elementId))
                        list.Add(card.CardNumber);
                }
            }
            return list;
        }

        /// <summary>
        /// Verifica Bingo nas cartelas escolhidas.
        /// bingoPhase: 1 = Quina (linha/coluna), 2 = Cartela Cheia.
        /// compId: elemento recém-sorteado (ajuda a localizar linha/coluna em 5×5).
        /// </summary>
        public List<int> CheckBingo(List<int> chosenCards, int bingoPhase, int compId)
        {
            var game = _pkg.Game ?? throw new System.InvalidOperationException("Jogo não carregado.");
            var winners = new List<int>();

            foreach (var cardNum in chosenCards)
            {
                var card = game.GameCards.FirstOrDefault(c => c.CardNumber == cardNum);
                if (card is null) continue;

                if (bingoPhase == 2) // Cartela Cheia
                {
                    bool full =
                        (game.GameSize == 5)
                            ? (card.BElements.Concat(card.IElements).Concat(card.NElements)
                                  .Concat(card.GElements).Concat(card.OElements))
                                  .All(x => _drawnElements.Contains(x))
                            : (card.AllElements.All(x => _drawnElements.Contains(x)));
                    if (full)
                    {
                        winners.Add(cardNum);
                        continue;
                    }
                }

                if (bingoPhase == 1) // Quina (linha ou coluna)
                {
                    if (game.GameSize == 5)
                    {
                        // Descobre a linha e a coluna onde compId está
                        var rows = GetRows5(card);       // 5 linhas de 5 elementos
                        var cols = GetColumns5(card);    // 5 colunas de 5 elementos

                        int rowIndex = rows.FindIndex(r => r.Contains(compId));
                        int colIndex = cols.FindIndex(c => c.Contains(compId));

                        bool rowComplete = rowIndex >= 0 && rows[rowIndex].All(x => _drawnElements.Contains(x));
                        bool colComplete = colIndex >= 0 && cols[colIndex].All(x => _drawnElements.Contains(x));

                        if (rowComplete || colComplete)
                            winners.Add(cardNum);
                    }
                    else if (game.GameSize == 4)
                    {
                        // 4×4: sem B/I/N/G/O. Considera linha/coluna da grade 4×4.
                        var rows = GetRows4(card);       // 4 linhas de 4 elementos
                        var cols = GetColumns4(card);    // 4 colunas de 4 elementos

                        int rowIndex = rows.FindIndex(r => r.Contains(compId));
                        int colIndex = cols.FindIndex(c => c.Contains(compId));

                        bool rowComplete = rowIndex >= 0 && rows[rowIndex].All(x => _drawnElements.Contains(x));
                        bool colComplete = colIndex >= 0 && cols[colIndex].All(x => _drawnElements.Contains(x));

                        if (rowComplete || colComplete)
                            winners.Add(cardNum);
                    }
                }
            }

            return winners.Distinct().OrderBy(x => x).ToList();
        }

        public void ResetGame()
        {
            _drawnElements.Clear();
            _drawnCards.Clear();
        }

        // Mostrar Capa - Bingo
        public enum StageMode { Cover, Element }
        public StageMode CurrentStage { get; private set; } = StageMode.Cover;
        public int? CurrentElementId { get; private set; }
        public event Action? OnStageChange;

        public void RequestShowCover()
        {
            CurrentStage = StageMode.Cover;
            CurrentElementId = null;
            OnStageChange?.Invoke();
        }

        public void ShowElement(int elementId)
        {
            CurrentStage = StageMode.Element;
            CurrentElementId = elementId;
            OnStageChange?.Invoke();
        }

        // ========================= Helpers 5×5 =========================
        private static List<List<int>> GetRows5(CardModel c)
        {
            // monta linhas a partir das colunas B I N G O:
            // linha 0 = B[0], I[0], N[0], G[0], O[0]; etc.
            var rows = new List<List<int>>(5);
            for (int i = 0; i < 5; i++)
            {
                var row = new List<int>(5);
                if (c.BElements.Count > i) row.Add(c.BElements[i]);
                if (c.IElements.Count > i) row.Add(c.IElements[i]);
                if (c.NElements.Count > i) row.Add(c.NElements[i]);
                if (c.GElements.Count > i) row.Add(c.GElements[i]);
                if (c.OElements.Count > i) row.Add(c.OElements[i]);
                rows.Add(row);
            }
            return rows;
        }

        private static List<List<int>> GetColumns5(CardModel c)
        {
            // as colunas já existem no modelo
            return new List<List<int>>
            {
                c.BElements.ToList(),
                c.IElements.ToList(),
                c.NElements.ToList(),
                c.GElements.ToList(),
                c.OElements.ToList()
            };
        }

        // ========================= Helpers 4×4 =========================
        private static List<List<int>> GetRows4(CardModel c)
        {
            // AllElements em ordem linear de 16 IDs -> grid 4x4 (linha major)
            var rows = new List<List<int>>(4);
            for (int r = 0; r < 4; r++)
            {
                var row = new List<int>(4);
                for (int col = 0; col < 4; col++)
                {
                    int idx = r * 4 + col;
                    if (idx < c.AllElements.Count)
                        row.Add(c.AllElements[idx]);
                }
                rows.Add(row);
            }
            return rows;
        }

        private static List<List<int>> GetColumns4(CardModel c)
        {
            var cols = new List<List<int>>(4) { new(), new(), new(), new() };
            for (int r = 0; r < 4; r++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int idx = r * 4 + col;
                    if (idx < c.AllElements.Count)
                        cols[col].Add(c.AllElements[idx]);
                }
            }
            return cols;
        }
    }
}
