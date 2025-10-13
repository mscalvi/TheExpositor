using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Drawing.Layout.enums;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BingoCreator.Services
{
    internal enum CellStyle
    {
        SquareWhite,    // quadrada, fundo branco (como está hoje)
        RoundedColor    // cantos arredondados, fundo = Primary do tema
    }

    internal static class DesignService
    {
        private static XImage? _logo;  // cache em memória
        public static string Header5x5 { get; set; } = "SORTE";   // "SORTE" ou "BINGO"
        public static string CellStyle { get; set; } = "SQUARE";   // "SQUARE" ou "ROUNDED"

        public static double CellFillLighten = 0.80; // 0 = sem clarear, 1 = branco; 0.80 ≈ quase branco
        public static double GinghamTile { get; set; } = 6; // experimente 6–8
        public static double HeaderBandOpacity { get; set; } = 0.80;
        public static double FooterBandOpacity { get; set; } = 0.8;
        public static double CellOverlayOpacity { get; set; } = 0.70;


        // Normalizações simples
        private static bool IsBingo(string? s) =>
            string.Equals(s, "BINGO", StringComparison.OrdinalIgnoreCase);

        private static bool IsRounded(string? s) =>
            string.Equals(s, "ROUNDED", StringComparison.OrdinalIgnoreCase);

        // Letras do cabeçalho 5x5 conforme seleção
        public static string[] GetHeader5x5Letters() =>
            IsBingo(Header5x5) ? new[] { "B", "I", "N", "G", "O" }
                               : new[] { "S", "O", "R", "T", "E" };

        public static void UseDefaultLogo(string relativePath = "assets\\logo.png")
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var full = Path.Combine(baseDir, relativePath);
                SetLogo(File.Exists(full) ? full : null);  // se não existir, volta para a “bolinha”
            }
            catch
            {
                SetLogo(null);
            }
        }

        // Passe um caminho .png/.jpg. Se null ou inválido, limpa o logo.
        public static void SetLogo(string? path)
        {
            _logo?.Dispose();
            _logo = null;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            // evita lock no arquivo: carrega em memória
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            _logo = XImage.FromStream(() => new MemoryStream(bytes));
        }

        public static void ClearLogo()
        {
            _logo?.Dispose();
            _logo = null;
        }

        // cria XFont com embed garantindo (fallback para Arial/Segoe se a fonte não estiver instalada)
        public static XFont CreateFont(string family, double size, XFontStyle style)
        {
            try
            {
                var opts = new XPdfFontOptions(PdfFontEncoding.Unicode); // sua versão aceita 1 argumento
                return new XFont(family, size, style, opts);
            }
            catch
            {
                var fb = OperatingSystem.IsWindows() ? "Segoe UI" : "Arial";
                var opts = new XPdfFontOptions(PdfFontEncoding.Unicode);
                return new XFont(fb, size, style, opts);
            }
        }

        public static void DrawCardBackground(XGraphics gfx, XRect rect, Theme theme, double cornerRadius = 0)
        {
            if (theme.Estilo == BackgroundStyle.Xadrez)
            {
                DrawGinghamBackground(gfx, rect, theme, cornerRadius, tile: GinghamTile);
                return;
            }

            // Liso (sólido) — seu código original
            var bg = new XSolidBrush(theme.Primary);
            if (cornerRadius <= 0)
            {
                gfx.DrawRectangle(bg, rect);
            }
            else
            {
                double r = cornerRadius;
                var full = rect;

                var mid = new XRect(full.X + r, full.Y, full.Width - 2 * r, full.Height);
                var left = new XRect(full.X, full.Y + r, r, full.Height - 2 * r);
                var right = new XRect(full.X + full.Width - r, full.Y + r, r, full.Height - 2 * r);
                gfx.DrawRectangle(bg, mid);
                gfx.DrawRectangle(bg, left);
                gfx.DrawRectangle(bg, right);

                gfx.DrawEllipse(bg, full.X, full.Y, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X + full.Width - 2 * r, full.Y, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
                gfx.DrawEllipse(bg, full.X + full.Width - 2 * r, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
            }
        }

        public static void DrawHeaderBand(XGraphics gfx, XRect rect, Theme theme, string title, XFont titleFont)
        {
            // faixa de fundo
            var headerBg = WithOpacity(theme.HeaderBg, HeaderBandOpacity);
            gfx.DrawRectangle(new XSolidBrush(headerBg), rect);

            // área reservada à esquerda para ícone/logo
            double leftSlot = 0;

            if (_logo != null)
            {
                // escala proporcional ao retângulo do cabeçalho
                double maxH = Math.Max(0, rect.Height - 6); // respiro
                double ratio = _logo.PointWidth / _logo.PointHeight;   // unidades já em "points" do PDF
                double logoH = Math.Min(maxH, _logo.PointHeight);
                double logoW = logoH * ratio;

                double lx = rect.X + 6;
                double ly = rect.Y + (rect.Height - logoH) / 2.0;

                gfx.DrawImage(_logo, lx, ly, logoW, logoH);
                leftSlot = logoW + 12; // reserva espaço do texto
            }
            else
            {
                // fallback: “bolha”
                double size = Math.Min(rect.Height - 4, 24);
                double cx = rect.X + 8 + size / 2;
                double cy = rect.Y + rect.Height / 2;

                gfx.DrawEllipse(new XSolidBrush(theme.Accent), cx - size / 2, cy - size / 2, size, size);
                gfx.DrawEllipse(new XSolidBrush(theme.HeaderBg), cx - size / 3, cy - size / 3, (2 * size) / 3, (2 * size) / 3);
                leftSlot = size + 12;
            }

            // título centralizado no espaço restante (sem sobrepor logo)
            var textRect = new XRect(rect.X + leftSlot, rect.Y, rect.Width - leftSlot - 6, rect.Height);
            gfx.DrawString(title, titleFont, new XSolidBrush(theme.Text), textRect, XStringFormats.Center);
        }

        public static XPen Pen(Theme theme, double width = 0.8) =>
            new XPen(theme.Border, width);

        public static XBrush TextBrush(Theme theme) => new XSolidBrush(theme.Text);
        public static XBrush AccentBrush(Theme theme) => new XSolidBrush(theme.Accent);

        // Fundo da célula conforme estilo
        public static void FillCellBackground(XGraphics gfx, XRect rect, Theme theme, string modelKey, double radius = 6)
        {
            // Célula QUADRADA: mantém branco sólido (legibilidade máxima)
            if (!string.Equals(modelKey, "ROUNDED", StringComparison.OrdinalIgnoreCase))
            {
                gfx.DrawRectangle(XBrushes.White, rect);
                return;
            }

            // Célula ARREDONDADA: bloco translúcido "tipo header/footer",
            // usando a cor do HeaderBg do tema com opacidade controlada
            var overlay = WithOpacity(theme.HeaderBg, CellOverlayOpacity);
            var brush = new XSolidBrush(overlay);

            DrawRoundedFill(gfx, rect, radius, brush);
        }

        // Borda da célula conforme estilo
        public static void StrokeCellBorder(XGraphics gfx, XRect rect, Theme theme, XPen pen, string modelKey, double radius = 6)
        {
            if (!IsRounded(modelKey))
            {
                gfx.DrawRectangle(pen, rect);  // SQUARE
                return;
            }

            // ROUNDED: 4 lados + 4 arcos (cantos)
            double x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height, r = Math.Min(radius, Math.Min(w, h) / 2.0);

            gfx.DrawLine(pen, x + r, y, x + w - r, y);
            gfx.DrawLine(pen, x + w, y + r, x + w, y + h - r);
            gfx.DrawLine(pen, x + r, y + h, x + w - r, y + h);
            gfx.DrawLine(pen, x, y + r, x, y + h - r);

            gfx.DrawArc(pen, x + w - 2 * r, y, 2 * r, 2 * r, 270, 90);
            gfx.DrawArc(pen, x + w - 2 * r, y + h - 2 * r, 2 * r, 2 * r, 0, 90);
            gfx.DrawArc(pen, x, y + h - 2 * r, 2 * r, 2 * r, 90, 90);
            gfx.DrawArc(pen, x, y, 2 * r, 2 * r, 180, 90);
        }

        // Util interno: preencher com cantos arredondados (mesma técnica do fundo do cartão)
        private static void DrawRoundedFill(XGraphics gfx, XRect full, double radius, XBrush brush)
        {
            double r = radius;

            var mid = new XRect(full.X + r, full.Y, full.Width - 2 * r, full.Height);
            var left = new XRect(full.X, full.Y + r, r, full.Height - 2 * r);
            var right = new XRect(full.X + full.Width - r, full.Y + r, r, full.Height - 2 * r);

            gfx.DrawRectangle(brush, mid);
            gfx.DrawRectangle(brush, left);
            gfx.DrawRectangle(brush, right);

            gfx.DrawEllipse(brush, full.X, full.Y, 2 * r, 2 * r);
            gfx.DrawEllipse(brush, full.X + full.Width - 2 * r, full.Y, 2 * r, 2 * r);
            gfx.DrawEllipse(brush, full.X, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
            gfx.DrawEllipse(brush, full.X + full.Width - 2 * r, full.Y + full.Height - 2 * r, 2 * r, 2 * r);
        }

        // Xadrez (gingham)
        private static void DrawGinghamBackground(XGraphics gfx, XRect rect, Theme theme, double cornerRadius, double tile)
        {
            var path = RoundedRectPath(rect, cornerRadius);
            gfx.Save();
            gfx.IntersectClip(path);

            // base
            gfx.DrawRectangle(new XSolidBrush(theme.Primary), rect);

            // listras semi-transparentes
            double s = Math.Max(6, tile);
            var color = theme.Accent;
            color.A = (byte)Math.Round(0.12 * 255);
            var band = new XSolidBrush(color);

            // verticais
            for (double x0 = rect.X; x0 < rect.Right; x0 += 2 * s)
                gfx.DrawRectangle(band, new XRect(x0, rect.Y, s, rect.Height));

            // horizontais
            for (double y0 = rect.Y; y0 < rect.Bottom; y0 += 2 * s)
                gfx.DrawRectangle(band, new XRect(rect.X, y0, rect.Width, s));

            gfx.Restore();
        }

        private static XGraphicsPath RoundedRectPath(XRect rect, double radius)
        {
            var path = new XGraphicsPath();
            if (radius <= 0) { path.AddRectangle(rect); return path; }

            double x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
            double r = Math.Min(radius, Math.Min(w, h) / 2.0), d = 2 * r;

            path.AddArc(x, y, d, d, 180, 90);
            path.AddLine(x + r, y, x + w - r, y);
            path.AddArc(x + w - d, y, d, d, 270, 90);
            path.AddLine(x + w, y + r, x + w, y + h - r);
            path.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            path.AddLine(x + w - r, y + h, x + r, y + h);
            path.AddArc(x, y + h - d, d, d, 90, 90);
            path.AddLine(x, y + h - r, x, y + r);

            path.CloseFigure();
            return path;
        }

        private static XColor LerpToWhite(XColor c, double t)
        {
            // clamp 0..1
            t = Math.Max(0, Math.Min(1, t));

            // pegue componentes como double
            double a = c.A, rr = c.R, gg = c.G, bb = c.B;

            // Se vierem em 0..1, converte para 0..255
            bool is01 = (rr <= 1.0 && gg <= 1.0 && bb <= 1.0 && a <= 1.0);
            if (is01)
            {
                rr *= 255.0;
                gg *= 255.0;
                bb *= 255.0;
                a *= 255.0;
            }

            // interpola cada canal até o branco
            byte r = (byte)Math.Round(rr + (255.0 - rr) * t);
            byte g = (byte)Math.Round(gg + (255.0 - gg) * t);
            byte b = (byte)Math.Round(bb + (255.0 - bb) * t);
            byte ab = (byte)Math.Round(Math.Max(0.0, Math.Min(255.0, a))); // mantém a mesma opacidade

            return XColor.FromArgb(ab, r, g, b);
        }

        // Encontra a MAIOR fonte que cabe em até 2 linhas dentro do retângulo
        public static XFont FitFontForTwoLines(
            XGraphics gfx, string text, string family, XFontStyle style,
            double maxWidth, double maxHeight,
            double maxPointSize, double minPointSize = 8, double step = 0.5)
        {
            maxPointSize = Math.Max(minPointSize, maxPointSize);

            for (double size = maxPointSize; size >= minPointSize; size -= step)
            {
                var f = CreateFont(family, size, style);
                double lineH = gfx.MeasureString("Ag", f).Height;

                var lines = WrapByWidthLocal(gfx, text, f, maxWidth);
                int linesCount = Math.Min(2, lines.Count);
                double totalH = linesCount * lineH;

                if (linesCount <= 2 && totalH <= maxHeight)
                    return f;
            }

            return CreateFont(family, minPointSize, style);
        }

        // ---- util local de word-wrap (similar ao seu, mas interno ao DesignService) ----
        private static List<string> WrapByWidthLocal(XGraphics gfx, string text, XFont font, double maxW)
        {
            var tokens = Regex.Split(text ?? string.Empty, @"(\s+)");
            var lines = new List<string>();
            var sb = new StringBuilder();

            foreach (var tok in tokens)
            {
                string cand = sb.Length == 0 ? tok.TrimStart() : sb.ToString() + tok;
                if (gfx.MeasureString(cand, font).Width <= maxW)
                {
                    sb.Clear(); sb.Append(cand);
                    continue;
                }

                if (sb.Length > 0)
                {
                    lines.Add(sb.ToString().TrimEnd());
                    sb.Clear();
                }

                string t = tok.Trim();
                if (t.Length == 0) continue;

                int start = 0;
                while (start < t.Length)
                {
                    int len = 1;
                    while (start + len <= t.Length &&
                           gfx.MeasureString(t.AsSpan(start, len).ToString(), font).Width <= maxW)
                        len++;
                    if (len > 1) len--;
                    lines.Add(t.Substring(start, len));
                    start += len;
                }
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString().TrimEnd());

            return lines;
        }

        public static XFont FitFontToRect(
            XGraphics gfx, string text, string family, XFontStyle style,
            double maxWidth, double maxHeight,
            double maxPointSize, double minPointSize = 8, double step = 0.5)
        {
            double size = Math.Max(minPointSize, maxPointSize);
            while (size >= minPointSize)
            {
                var f = CreateFont(family, size, style);
                var lineH = gfx.MeasureString("Ag", f).Height;
                var textW = gfx.MeasureString(text, f).Width;

                if (textW <= maxWidth && lineH <= maxHeight)
                    return f;

                size -= step;
            }
            return CreateFont(family, minPointSize, style);
        }

        public static XColor WithOpacity(XColor c, double op)
        {
            op = Math.Max(0, Math.Min(1, op));

            double a = c.A, rr = c.R, gg = c.G, bb = c.B;
            bool is01 = (rr <= 1.0 && gg <= 1.0 && bb <= 1.0 && a <= 1.0);
            if (is01) { rr *= 255; gg *= 255; bb *= 255; a *= 255; }

            byte r = (byte)Math.Round(rr);
            byte g = (byte)Math.Round(gg);
            byte b = (byte)Math.Round(bb);
            byte ab = (byte)Math.Round(op * 255.0);

            return XColor.FromArgb(ab, r, g, b);

        }
    }
}
