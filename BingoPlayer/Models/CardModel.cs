namespace BingoPlayer.Models
{
    public class CardModel
    {
        public int CardId { get; set; }
        public int CardNumber { get; set; }
        public List<int> AllElements { get; set; } = new();

        // 5×5: use BINGO/linhas
        public List<int> BElements { get; set; } = new();
        public List<int> IElements { get; set; } = new();
        public List<int> NElements { get; set; } = new();
        public List<int> GElements { get; set; } = new();
        public List<int> OElements { get; set; } = new();

        // Alternativa por linhas (5×5) — preencha se facilitar seu layout
        public List<int> Elements1 { get; set; } = new();
        public List<int> Elements2 { get; set; } = new();
        public List<int> Elements3 { get; set; } = new();
        public List<int> Elements4 { get; set; } = new();
        public List<int> Elements5 { get; set; } = new();
    }
}
