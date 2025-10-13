namespace BingoPlayer.Models
{
    public class GameModel
    {
        //Nome do Jogo
        public string GameName { get; set; } = "";

        //Total de Cartelas
        public int GameQuant { get; set; }

        //Design do Jogo
        public int GameSize { get; set; }
        public string GameTheme { get; set; } = "";
        public string GameHeader { get; set; } = "";
        public string GameTitle { get; set; } = "";

        public List<CardModel> GameCards { get; set; } = new();

        public List<ElementModel> AllElements { get; set; } = new();
        public List<ElementModel> BElements { get; set; } = new();
        public List<ElementModel> IElements { get; set; } = new();
        public List<ElementModel> NElements { get; set; } = new();
        public List<ElementModel> GElements { get; set; } = new();
        public List<ElementModel> OElements { get; set; } = new();
    }
}
