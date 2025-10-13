using System;
using System.Collections.Generic;
using PdfSharpCore.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace BingoCreator.Services
{
    internal static class ThemeCatalog
    {
        static XColor Hex(string hex, double alpha = 1)
        {
            hex = hex.TrimStart('#');
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            byte a = (byte)Math.Round(alpha * 255);
            return XColor.FromArgb(a, r, g, b);
        }

        public static readonly Theme Green = new(
            "GREEN", "Verde",
            Hex("ECF8F1"), // Primary: fundo do cartão
            Hex("D6F3E5"), // HeaderBg: faixa do título
            Hex("2E7E68"), // Border: linhas/bordas
            Hex("111111"), // Text
            Hex("111111"), // Accent: detalhes/ID
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false,
            estilo: BackgroundStyle.Liso
        );

        public static readonly Theme Pink = new(
            "PINK", "Rosa",
            Hex("FFF0F6"), // Primary 
            Hex("FFE1EC"), // HeaderBg 
            Hex("C04A7A"), // Border
            Hex("111111"), // Text
            Hex("111111"), // Accent
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false,
            estilo: BackgroundStyle.Liso
        );

        public static readonly Theme Black = new(
            "BLACK", "Preto",
            Hex("F6F7F9"), Hex("E9ECEF"), Hex("4F4F4F"),
            Hex("000000"), Hex("111111"),
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false,
            estilo: BackgroundStyle.Liso
        );

        public static readonly Theme Blue = new(
            "BLUE", "Azul",
            Hex("EDF4FF"), Hex("D6E7FF"), Hex("3A7BD5"),
            Hex("111111"), Hex("111111"),
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false,
            estilo: BackgroundStyle.Liso
        );

        public static readonly Theme Orange = new(
            "ORANGE", "Laranja",
            Hex("FFF6E5"), Hex("FFE7BF"), Hex("7A5C4B"),
            Hex("111111"), Hex("111111"),
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false,
            estilo: BackgroundStyle.Liso
        );

        public static readonly Theme OrangeStrips = new(
            "ORANGESTRIPS", "Laranja Xadrez",
            Hex("FFFBF3"), // Primary (mais claro)
            Hex("FFEBD1"), // HeaderBg (mais claro)
            Hex("8E5E2A"), // Border (marrom suave, menos contrastado)
            Hex("111111"), // Text (preto)
            Hex("FFD5A6"), // Accent (laranja claro - usado no xadrez)
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false, estilo: BackgroundStyle.Xadrez
        );

        public static readonly Theme RedStrips = new(
            "REDSTRIPS", "Vermelho Xadrez",
            Hex("FFF9FA"), // Primary
            Hex("FFEFF2"), // HeaderBg 
            Hex("A86262"), // Border
            Hex("111111"), // Text
            Hex("FFC9CF"), // Accent
            fontTitle: "Poppins", fontBody: "Nunito",
            eco: false, estilo: BackgroundStyle.Xadrez
        );


        private static readonly IReadOnlyDictionary<string, Theme> _all =
            new Dictionary<string, Theme>(StringComparer.OrdinalIgnoreCase)
            {
                ["GREEN"] = Green,
                ["PINK"] = Pink,
                ["BLACK"] = Black,
                ["BLUE"] = Blue,
                ["ORANGE"] = Orange,
                ["ORANGESTRIPS"] = OrangeStrips,
                ["REDSTRIPS"] = RedStrips
            };



        public static Theme Get(string key) =>
            _all.TryGetValue(key ?? "", out var t) ? t : Black;

        internal static IReadOnlyDictionary<string, Theme> All => _all;
    }
}
