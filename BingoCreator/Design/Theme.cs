using PdfSharpCore.Drawing;

namespace BingoCreator.Services
{
    // Fundo do cartão: liso (sólido) ou xadrez (gingham)
    internal enum BackgroundStyle { Liso, Xadrez }

    internal sealed class Theme
    {
        public string Key { get; }
        public string DisplayName { get; }
        public XColor Primary { get; }   // fundo do cartão
        public XColor HeaderBg { get; }  // faixa do título
        public XColor Border { get; }    // linhas/bordas
        public XColor Text { get; }      // texto principal
        public XColor Accent { get; }    // detalhes/ID
        public string FontTitle { get; }
        public string FontBody { get; }
        public bool Eco { get; }


        // NOVO: estilo do fundo
        public BackgroundStyle Estilo { get; set; }

        public Theme(string key, string display, XColor primary, XColor headerBg, XColor border,
                     XColor text, XColor accent, string fontTitle, string fontBody,
                     bool eco = false, BackgroundStyle estilo = BackgroundStyle.Liso)
        {
            Key = key; DisplayName = display; Primary = primary; HeaderBg = headerBg;
            Border = border; Text = text; Accent = accent;
            FontTitle = fontTitle; FontBody = fontBody; Eco = eco;
            Estilo = estilo; // padrão: Liso
        }
    }
}
