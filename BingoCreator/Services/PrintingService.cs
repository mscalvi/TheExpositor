using BingoCreator.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Drawing.Layout.enums;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace BingoCreator.Services
{
    internal class PrintingService
    {

        // Imprimir Cartelas e Listas   
        public static void PrintCards(int setId)
        {
            var cards = DataService.GetCardSetById(setId);
            if (cards == null)
            {
                return;
            }

            List<DataRow> setCards = DataService.GetCardsBySetId (cards.Id);

            List<List<ElementModel>> cardElements = DataService.GetCardElementsBySet(setCards);

            if (cards.CardsSize == 4)
            {
                PrintCards4x4(cards, cardElements);
            } else if (cards.CardsSize == 5)
            {
                PrintCards5x5(cards, cardElements);
            } else
            {
                return;
            }
        }

        public static void PrintCards5x5(CardSetModel cards, List<List<ElementModel>> cardElements)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"Cartelas - {cards.Name}.pdf";
            var filePath = Path.Combine(folder, fileName);

            var theme = ThemeCatalog.Get(cards.Theme);
            DesignService.Header5x5 = string.IsNullOrWhiteSpace(cards.Header) ? "SORTE" : cards.Header.Trim();
            DesignService.CellStyle = string.IsNullOrWhiteSpace(cards.Model) ? "SQUARE" : cards.Model.Trim();

            var document = new PdfDocument();
            document.Info.Title = $"Cartelas - {cards.Title}";

            const double margin = 40;
            const double cellHeight = 40;
            const double gapY = 100;
            const bool showCut = true;
            const bool dottedCut = true;

            double cardHeight = cellHeight * 8;
            double pageWidth = 0, pageHeight = 0, cardWidth = 0;

            var titleFont = DesignService.CreateFont(theme.FontTitle, 17, XFontStyle.Bold);
            var headerFont = DesignService.CreateFont(theme.FontTitle, 15, XFontStyle.Bold);
            var compFont = DesignService.CreateFont(theme.FontBody, 10, XFontStyle.Bold);
            var footerFont = DesignService.CreateFont(theme.FontTitle, 12, XFontStyle.Bold);
            var numberFont = DesignService.CreateFont(theme.FontTitle, 12, XFontStyle.Bold);
            var pen = DesignService.Pen(theme, 0.8);

            XGraphics gfx = null;
            PdfPage page = null;

            // variáveis por página
            double midY = 0;          // linha central da área útil
            double yTop = 0, yBottom = 0;
            bool hasSecondOnPage = false;

            for (int i = 0; i < cards.Quantity; i++)
            {
                // Nova página a cada par
                if (i % 2 == 0)
                {
                    page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;

                    pageWidth = page.Width;
                    pageHeight = page.Height;
                    cardWidth = pageWidth - 2 * margin;

                    gfx = XGraphics.FromPdfPage(page);

                    // Esta página terá 2 cartelas?
                    hasSecondOnPage = (i + 1 < cards.Quantity);

                    // Centro da área útil
                    double usableH = pageHeight - 2 * margin;
                    midY = margin + usableH / 2.0;

                    if (hasSecondOnPage)
                    {
                        // Posicionamento simétrico em torno de midY
                        yTop = midY - (gapY / 2.0) - cardHeight;
                        yBottom = midY + (gapY / 2.0);
                    }
                    else
                    {
                        // Página com uma cartela só: centraliza
                        yTop = midY - cardHeight / 2.0;
                    }
                }

                // Qual Y usar nesta cartela?
                double y0 = (!hasSecondOnPage)
                            ? yTop
                            : (i % 2 == 0 ? yTop : yBottom);

                DrawCards5x5(
                    gfx, margin, y0, cardWidth, cardHeight,
                    cardElements[i], i + 1, cards.Title, cards.End,
                    theme, pen, titleFont, headerFont, compFont, footerFont, numberFont
                );

                // Depois de desenhar a 2ª cartela da página, desenhe a linha de corte exatamente em midY
                if (showCut && hasSecondOnPage && (i % 2 == 1))
                {
                    var cutPen = new XPen(XColors.Gray, 0.6)
                    {
                        DashStyle = dottedCut ? XDashStyle.Dot : XDashStyle.Solid
                    };
                    gfx.DrawLine(cutPen, margin, midY, pageWidth - margin, midY);
                }
            }

            document.Save(filePath);
            PrintList5(cards);
        }

        public static void PrintCards4x4(CardSetModel cards, List<List<ElementModel>> cardElements)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"Cartelas - {cards.Name}.pdf";
            var filePath = Path.Combine(folder, fileName);
            var theme = ThemeCatalog.Get(cards.Theme);
            DesignService.CellStyle = string.IsNullOrWhiteSpace(cards.Model) ? "SQUARE" : cards.Model.Trim();

            int teste = cards.AllElements.Count;

            var document = new PdfDocument();
            document.Info.Title = $"Cartelas 4×4 – {cards.Name}";

            const double margin = 36;
            const double gap = 14;
            const bool showCutLines = true;  
            const bool dottedCut = true;      

            XGraphics gfx = null;
            PdfPage page = null;
            double pageWidth = 0, pageHeight = 0, cardWidth = 0, cardHeight = 0;

            var titleFont = DesignService.CreateFont(theme.FontTitle, 16, XFontStyle.Bold);
            var compFont = DesignService.CreateFont(theme.FontBody, 10, XFontStyle.Regular);
            var footerFont = DesignService.CreateFont(theme.FontTitle, 11, XFontStyle.Bold);
            var numberFont = DesignService.CreateFont(theme.FontTitle, 11, XFontStyle.Bold);

            var pen = DesignService.Pen(theme, 0.8);   // bordas na cor do tema

            for (int i = 0; i < cards.Quantity; i++)
            {
                // ✅ nova página a cada 3 cartelas
                if (i % 3 == 0)
                {
                    page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4; // retrato
                    gfx = XGraphics.FromPdfPage(page);

                    pageWidth = page.Width;
                    pageHeight = page.Height;

                    cardWidth = pageWidth - 2 * margin;
                    // ✅ 3 cartelas empilhadas (2 gaps entre elas)
                    double available = pageHeight - 2 * margin - 2 * gap;
                    cardHeight = available / 3.0;
                    if (showCutLines)
                    {
                        double yCut1 = margin + cardHeight + gap / 2.0;
                        double yCut2 = margin + 2 * cardHeight + 1.5 * gap;
                        DrawHorizontalCutLine(gfx, margin, pageWidth - margin, yCut1, dottedCut);
                        DrawHorizontalCutLine(gfx, margin, pageWidth - margin, yCut2, dottedCut);
                    }
                }

                int rowInPage = i % 3; // 0..2
                double y0 = margin + rowInPage * (cardHeight + gap);

                DrawCards4x4(gfx, margin, y0, cardWidth, cardHeight,
                             cardElements[i], i + 1, cards.Title, cards.End,
                             theme, pen, titleFont, compFont, footerFont, numberFont);
            }

            document.Save(filePath);
            PrintList4(cards);
        }

        public static void PrintList5(CardSetModel cards)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"Lista - {cards.Name}.pdf";
            var filePath = Path.Combine(folder, fileName);

            var document = new PdfDocument();
            document.Info.Title = $"Lista de Elementos {cards.Name}";

            var font = new XFont("Segoe UI", 14, XFontStyle.Regular);
            var titleFont = new XFont("Segoe UI", 18, XFontStyle.Bold);

            // Header dinâmico: se inválido, cai para "SORTE"
            string header = string.IsNullOrWhiteSpace(cards.Header) ? "SORTE" : cards.Header.Trim().ToUpperInvariant();
            if (header.Length != 5) header = "SORTE";

            // Converte grupos para List<string> (CardName com fallback para Name)
            var gB = (cards.GroupB ?? new()).Select(e => (e?.CardName ?? e?.Name ?? "").Trim()).Where(s => s.Length > 0).ToList();
            var gI = (cards.GroupI ?? new()).Select(e => (e?.CardName ?? e?.Name ?? "").Trim()).Where(s => s.Length > 0).ToList();
            var gN = (cards.GroupN ?? new()).Select(e => (e?.CardName ?? e?.Name ?? "").Trim()).Where(s => s.Length > 0).ToList();
            var gG = (cards.GroupG ?? new()).Select(e => (e?.CardName ?? e?.Name ?? "").Trim()).Where(s => s.Length > 0).ToList();
            var gO = (cards.GroupO ?? new()).Select(e => (e?.CardName ?? e?.Name ?? "").Trim()).Where(s => s.Length > 0).ToList();

            var groups = new (string Title, List<string> Items)[]
            {
        ($"Coluna {header[0]}", gB),
        ($"Coluna {header[1]}", gI),
        ($"Coluna {header[2]}", gN),
        ($"Coluna {header[3]}", gG),
        ($"Coluna {header[4]}", gO),
            };

            int index = 1;
            const double margin = 40;
            const double lineSpacingExtra = 6;

            foreach (var (title, items) in groups)
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                double y = margin;
                gfx.DrawString(title, titleFont, XBrushes.Black,
                    new XRect(margin, y, page.Width - 2 * margin, 24), XStringFormats.TopCenter);
                y += 24;

                double lineHeight = gfx.MeasureString("Ag", font).Height + lineSpacingExtra;

                foreach (var row in items)
                {
                    if (y + lineHeight + margin > page.Height)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = margin;

                        gfx.DrawString(title, titleFont, XBrushes.Black,
                            new XRect(margin, y, page.Width - 2 * margin, 24), XStringFormats.TopCenter);
                        y += 24;
                    }

                    string text = $"{index}- {row}";
                    gfx.DrawString(text, font, XBrushes.Black,
                        new XRect(margin, y, page.Width - 2 * margin, lineHeight), XStringFormats.TopLeft);

                    y += lineHeight;
                    index++;
                }
            }

            document.Save(filePath);
            PrintCutPapers(cards);
        }

        public static void PrintList4(CardSetModel cards)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"Lista - {cards.Name}.pdf";
            var filePath = Path.Combine(folder, fileName);

            // Monta a lista de nomes (CardName -> Name)
            var items = (cards.AllElements ?? new List<ElementModel>())
                .Select(e => (e?.CardName ?? e?.Name ?? string.Empty).Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var document = new PdfDocument();
            document.Info.Title = $"Relação – {cards.Name}";

            const double margin = 40;
            const double colGap = 20;

            var theme = ThemeCatalog.Get(cards.Theme);
            var titleFont = DesignService.CreateFont(theme.FontTitle, 18, XFontStyle.Bold);
            var itemFont = DesignService.CreateFont(theme.FontBody, 12, XFontStyle.Regular);
            var textBrush = DesignService.TextBrush(theme);
            var accent = DesignService.AccentBrush(theme);

            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            double pageW = page.Width;
            double pageH = page.Height;

            // Título
            var titleRect = new XRect(margin, margin, pageW - 2 * margin, 28);
            gfx.DrawString($"Relação de Elementos – {cards.Name}", titleFont, accent, titleRect, XStringFormats.Center);

            // Área útil abaixo do título
            double top = titleRect.Bottom + 12;
            double usableH = pageH - margin - top;

            // Duas colunas
            double colW = (pageW - 2 * margin - colGap) / 2.0;
            var col1 = new XRect(margin, top, colW, usableH);
            var col2 = new XRect(margin + colW + colGap, top, colW, usableH);

            // Desenho linha a linha, com quebra e paginação
            int idx = 0;
            DrawListColumn(gfx, items, ref idx, col1, itemFont, textBrush);
            DrawListColumn(gfx, items, ref idx, col2, itemFont, textBrush);

            while (idx < items.Count)
            {
                page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                gfx = XGraphics.FromPdfPage(page);

                // Título por página
                titleRect = new XRect(margin, margin, page.Width - 2 * margin, 28);
                gfx.DrawString($"Relação de Elementos – {cards.Name}", titleFont, accent, titleRect, XStringFormats.Center);

                top = titleRect.Bottom + 12;
                usableH = page.Height - margin - top;
                colW = (page.Width - 2 * margin - colGap) / 2.0;
                col1 = new XRect(margin, top, colW, usableH);
                col2 = new XRect(margin + colW + colGap, top, colW, usableH);

                DrawListColumn(gfx, items, ref idx, col1, itemFont, textBrush);
                DrawListColumn(gfx, items, ref idx, col2, itemFont, textBrush);
            }

            document.Save(filePath);
            PrintCutPapers(cards);
        }

        public static void PrintCutPapers(
            CardSetModel cards,
            int copiesPerItem = 1,
            int cols = 4,
            int rows = 10,
            string preferColumn = "CardName",
            bool showCropMarks = true,
            bool showColumnLetterPerCell = false) 
        {
            // Pasta destino
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var folder = Path.Combine(desktop, Sanitize(cards.Name));
            Directory.CreateDirectory(folder);
            var fileName = $"Fichas - {cards.Name}.pdf";
            var filePath = Path.Combine(folder, fileName);

            // 1) Extrai nomes (CardName > Name) a partir do CardSetModel
            //    - 5x5: une B,I,N,G,O
            //    - 4x4: usa AllElements
            IEnumerable<ElementModel> src =
                cards.CardsSize == 5
                    ? new[]
                      {
                  cards.GroupB ?? new List<ElementModel>(),
                  cards.GroupI ?? new List<ElementModel>(),
                  cards.GroupN ?? new List<ElementModel>(),
                  cards.GroupG ?? new List<ElementModel>(),
                  cards.GroupO ?? new List<ElementModel>()
                      }.SelectMany(x => x)
                    : (cards.AllElements ?? new List<ElementModel>());

            bool preferName = string.Equals(preferColumn, "Name", StringComparison.OrdinalIgnoreCase);

            string header = string.IsNullOrWhiteSpace(cards.Header) ? "SORTE" : cards.Header.Trim().ToUpperInvariant();
            if (header.Length != 5) header = "SORTE";


            List<(char? Col, string Text)> baseItems;

            if (cards.CardsSize == 5)
            {
                // Cria pares (coluna, elemento) preservando de onde vem
                var groups = new List<(char Col, IEnumerable<ElementModel> Elems)>
    {
        (header[0], cards.GroupB ?? new List<ElementModel>()),
        (header[1], cards.GroupI ?? new List<ElementModel>()),
        (header[2], cards.GroupN ?? new List<ElementModel>()),
        (header[3], cards.GroupG ?? new List<ElementModel>()),
        (header[4], cards.GroupO ?? new List<ElementModel>())
    };

                baseItems = groups.SelectMany(g =>
                    g.Elems.Select(e =>
                    {
                        var first = preferName ? e?.Name : e?.CardName;
                        var fallback = preferName ? e?.CardName : e?.Name;
                        var text = (first ?? fallback ?? string.Empty).Trim();
                        return (text.Length > 0) ? ((char?)g.Col, text) : ((char?)null, null);
                    })
                    .Where(t => t.Item2 != null)!
                ).ToList();
            }
            else
            {
                var srcAll = cards.AllElements ?? new List<ElementModel>();
                baseItems = srcAll.Select(e =>
                {
                    var first = preferName ? e?.Name : e?.CardName;
                    var fallback = preferName ? e?.CardName : e?.Name;
                    var text = (first ?? fallback ?? string.Empty).Trim();
                    return (text.Length > 0) ? ((char?)null, text) : ((char?)null, null);
                })
                .Where(t => t.Item2 != null)!
                .ToList();
            }

            // 2) Replica para múltiplas cópias por item
            copiesPerItem = Math.Max(1, copiesPerItem);
            var items = new List<(char? Col, string Text)>(baseItems.Count * copiesPerItem);
            foreach (var t in baseItems)
                for (int k = 0; k < copiesPerItem; k++)
                    items.Add(t);

            // 3) PDF e layout
            var doc = new PdfDocument();
            doc.Info.Title = $"Fichas de Sorteio – {cards.Name}";

            const double margin = 36;   // ~0,5"
            const double gapYTitle = 8; // espaço após o título

            var theme = ThemeCatalog.Get(cards.Theme);
            var titleFont = DesignService.CreateFont(theme.FontTitle, 14, XFontStyle.Bold);
            var cellFont = DesignService.CreateFont(theme.FontBody, 11, XFontStyle.Regular);
            var pen = DesignService.Pen(theme, 0.6);
            var accent = DesignService.AccentBrush(theme);

            cols = Math.Max(1, cols);
            rows = Math.Max(1, rows);
            int perPage = cols * rows;
            int total = items.Count;
            int pages = (int)Math.Ceiling(total / (double)perPage);

            int idx = 0;
            for (int p = 0; p < pages; p++)
            {
                var page = doc.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);

                double pageW = page.Width;
                double pageH = page.Height;

                // Título
                var titleRect = new XRect(margin, margin, pageW - 2 * margin, 22);
                gfx.DrawString(
                    $"Fichas de Sorteio – {cards.Name}",
                    titleFont, accent, titleRect, XStringFormats.Center
                );

                // Área do grid
                double top = titleRect.Bottom + gapYTitle;
                double gridW = pageW - 2 * margin;
                double gridH = pageH - margin - top;

                double cellW = gridW / cols;
                double cellH = gridH / rows;

                // Grid + texto
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        double x = margin + c * cellW;
                        double y = top + r * cellH;

                        var cellRect = new XRect(x, y, cellW, cellH);
                        gfx.DrawRectangle(pen, cellRect);

                        if (idx < items.Count)
                        {
                            var (colLetter, text) = items[idx];

                            // Retângulo da célula
                            var inner = new XRect(cellRect.X + 4, cellRect.Y + 3, cellRect.Width - 8, cellRect.Height - 6);

                            if (showColumnLetterPerCell && cards.CardsSize == 5 && colLetter.HasValue)
                            {
                                // Desenha a letra da coluna no topo (um pouco menor/mais forte)
                                var letterFont = DesignService.CreateFont(theme.FontTitle, 9, XFontStyle.Bold);
                                var letterRect = new XRect(inner.X, inner.Y, inner.Width, 12);
                                // Centralizado no topo
                                gfx.DrawString(colLetter.Value.ToString(), letterFont, XBrushes.Black, letterRect, XStringFormats.TopCenter);

                                // Empurra a área de texto do item para baixo da letra
                                inner = new XRect(inner.X, letterRect.Bottom + 2, inner.Width, inner.Height - (letterRect.Height + 2));
                            }

                            // Nome do item (quebra de linha centralizada)
                            DrawWrappedCenteredText(gfx, text, cellFont, inner, maxLines: 2, minPoint: 8);

                            idx++;
                        }

                    }
                }

                if (showCropMarks)
                    DrawCropMarks(gfx, margin, pageW, pageH);
            }

            doc.Save(filePath);
        }


        // Desenho das Cartelas e Listas
        private static void DrawHorizontalCutLine(XGraphics gfx, double x1, double x2, double y, bool dotted = true, double thickness = 0.6)
        {
            var pen = new XPen(XColors.Gray, thickness);
            pen.DashStyle = dotted ? XDashStyle.Dot : XDashStyle.Solid;
            gfx.DrawLine(pen, x1, y, x2, y);
        }

        private static void DrawCards5x5(XGraphics gfx, double x, double y, double width, double height, List<ElementModel> cardsElements, int cardNumber, string titleText, string footerText, Theme theme, XPen pen, XFont titleFont, XFont headerFont, XFont elementFont, XFont footerFont, XFont numberFont)
        {
            double padding = 8;
            double cellH = 40;     // altura fixa que você usa hoje
            double headerH = cellH;

            var cardRect = new XRect(x, y, width, height);
            DesignService.DrawCardBackground(gfx, cardRect, theme, cornerRadius: 12);

            // Faixa de título
            var titleRect = new XRect(x + padding, y + padding, width - 2 * padding, headerH - padding);
            DesignService.DrawHeaderBand(gfx, titleRect, theme, titleText, titleFont);

            // Cabeçalho (SORTE/BINGO) conforme seleção
            string[] headers = DesignService.GetHeader5x5Letters();
            double cellW = width / 5.0;

            for (int j = 0; j < 5; j++)
            {
                var r = new XRect(x + j * cellW, y + headerH, cellW, cellH);
                var headOverlay = new XSolidBrush(DesignService.WithOpacity(theme.HeaderBg, DesignService.CellOverlayOpacity));
                gfx.DrawRectangle(headOverlay, r);
                gfx.DrawRectangle(pen, r);
                gfx.DrawString(headers[j], headerFont, DesignService.TextBrush(theme), r, XStringFormats.Center);

            }

            // Grid 5×5 com estilo de célula
            var modelKey = DesignService.CellStyle; // "SQUARE" ou "ROUNDED"
            double radius = 9;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    double xx = x + col * cellW;
                    double yy = y + 2 * cellH + row * cellH;
                    var cell = new XRect(xx, yy, cellW, cellH);

                    DesignService.FillCellBackground(gfx, cell, theme, modelKey, radius);
                    DesignService.StrokeCellBorder(gfx, cell, theme, pen, modelKey, radius);

                    int idx = col * 5 + row;
                    if (idx < cardsElements.Count)
                    {
                        var el = cardsElements[idx];
                        string name = (el?.CardName ?? el?.Name ?? string.Empty).Trim();

                        DrawWrappedCenteredText(
                            gfx, name, elementFont,
                            new XRect(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6),
                            maxLines: 3, minPoint: 7.5
                        );
                    }
                }
            }

            // Rodapé (faixa + texto responsivo em 2 linhas, limitado a 80% do título)
            double footerY = y + 7 * cellH;
            double bandPad = padding;

            var bandRect = new XRect(x + bandPad, footerY + bandPad / 2.0, width - 2 * bandPad, cellH - bandPad);

            // faixa com transparência (como no header)
            var footerBg = DesignService.WithOpacity(theme.HeaderBg, DesignService.FooterBandOpacity);
            gfx.DrawRectangle(new XSolidBrush(footerBg), bandRect);

            // 72% / 28% (mensagem / número)
            double leftW = bandRect.Width * 0.72;
            double rightW = bandRect.Width - leftW;

            var footerRect = new XRect(bandRect.X, bandRect.Y, leftW, bandRect.Height);
            var numRect = new XRect(footerRect.Right, bandRect.Y, rightW, bandRect.Height);

            // limite da mensagem: máx 80% do título
            double footerMaxPt = Math.Min(Math.Min(cellH * 0.60, 18), titleFont.Size * 0.80);

            // fonte da MENSAGEM (até 2 linhas)
            var footerFontFit = DesignService.FitFontForTwoLines(
                gfx, footerText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: footerRect.Width - 6,
                maxHeight: footerRect.Height - 4,
                maxPointSize: footerMaxPt,
                minPointSize: 9
            );

            DrawWrappedCenteredText(
                gfx,
                footerText,
                footerFontFit,
                new XRect(footerRect.X + 3, footerRect.Y + 2, footerRect.Width - 6, footerRect.Height - 4),
                maxLines: 2, minPoint: 8
            );

            // NÚMERO = 80% do tamanho FINAL da mensagem (e ainda respeita a caixa)
            string idText = $"Cartela {cardNumber:0000}";
            double numTargetPt = footerFontFit.Size * 0.80;                    // << regra pedida
            double numberMaxPt = Math.Min(numTargetPt, Math.Min(cellH * 0.55, 16));

            var numberFontFit = DesignService.FitFontToRect(
                gfx, idText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: numRect.Width - 6, maxHeight: numRect.Height - 4,
                maxPointSize: numberMaxPt, minPointSize: 8
            );

            // textos em preto
            gfx.DrawString(idText, numberFontFit, DesignService.TextBrush(theme), numRect, XStringFormats.Center);

        }

        private static void DrawCards4x4( XGraphics gfx, double x, double y, double width, double height, List<ElementModel> cardsElements, int cardNumber, string titleText, string footerText, Theme theme, XPen pen, XFont titleFont, XFont elementFont, XFont footerFont, XFont numberFont)
        {
            double padding = 8;
            double cellH = height / 6.0; // 1 título + 4 grid + 1 rodapé
            double headerH = cellH;

            var cardRect = new XRect(x, y, width, height);
            DesignService.DrawCardBackground(gfx, cardRect, theme, cornerRadius: 12);

            // Cabeçalho
            var titleRect = new XRect(x + padding, y + padding, width - 2 * padding, headerH - padding);
            DesignService.DrawHeaderBand(gfx, titleRect, theme, titleText, titleFont);

            // Grid 4×4 com estilo de célula
            double gridTop = y + headerH;
            double cellW = width / 4.0;

            var modelKey = DesignService.CellStyle; // "SQUARE" ou "ROUNDED"
            double radius = 9;

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    double xx = x + col * cellW;
                    double yy = gridTop + row * cellH;
                    var cell = new XRect(xx, yy, cellW, cellH);

                    DesignService.FillCellBackground(gfx, cell, theme, modelKey, radius);
                    DesignService.StrokeCellBorder(gfx, cell, theme, pen, modelKey, radius);

                    int idx = row * 4 + col;
                    if (idx < cardsElements.Count)
                    {
                        var el = cardsElements[idx];
                        string name = (el?.CardName ?? el?.Name ?? string.Empty).Trim();

                        DrawWrappedCenteredText(
                            gfx, name, elementFont,
                            new XRect(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6),
                            maxLines: 3, minPoint: 7.5
                        );
                    }

                }
            }

            // Rodapé (faixa + texto responsivo em 2 linhas, limitado a 80% do título)
            double footerY = y + headerH + 4 * cellH;
            double bandPad = padding;

            var bandRect = new XRect(x + bandPad, footerY + bandPad / 2.0, width - 2 * bandPad, cellH - bandPad);

            // faixa com transparência
            var footerBg = DesignService.WithOpacity(theme.HeaderBg, DesignService.FooterBandOpacity);
            gfx.DrawRectangle(new XSolidBrush(footerBg), bandRect);

            double leftW = bandRect.Width * 0.72;
            double rightW = bandRect.Width - leftW;

            var footerRect = new XRect(bandRect.X, bandRect.Y, leftW, bandRect.Height);
            var numRect = new XRect(footerRect.Right, bandRect.Y, rightW, bandRect.Height);

            // máx 80% do título
            double footerMaxPt = Math.Min(Math.Min(cellH * 0.60, 16), titleFont.Size * 0.80);

            // fonte da MENSAGEM (até 2 linhas)
            var footerFontFit = DesignService.FitFontForTwoLines(
                gfx, footerText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: footerRect.Width - 6,
                maxHeight: footerRect.Height - 4,
                maxPointSize: footerMaxPt,
                minPointSize: 9
            );

            DrawWrappedCenteredText(
                gfx,
                footerText,
                footerFontFit,
                new XRect(footerRect.X + 3, footerRect.Y + 2, footerRect.Width - 6, footerRect.Height - 4),
                maxLines: 2, minPoint: 8
            );

            // NÚMERO = 80% do tamanho FINAL da mensagem
            string idText = $"Cartela {cardNumber:0000}";
            double numTargetPt = footerFontFit.Size * 0.80;
            double numberMaxPt = Math.Min(numTargetPt, Math.Min(cellH * 0.55, 14));

            var numberFontFit = DesignService.FitFontToRect(
                gfx, idText, theme.FontTitle, XFontStyle.Bold,
                maxWidth: numRect.Width - 6, maxHeight: numRect.Height - 4,
                maxPointSize: numberMaxPt, minPointSize: 8
            );

            gfx.DrawString(idText, numberFontFit, DesignService.TextBrush(theme), numRect, XStringFormats.Center);

        }

        private static void DrawListColumn(XGraphics gfx, IList<string> items, ref int idx, XRect col, XFont font, XBrush textBrush)
        {
            double lineH = gfx.MeasureString("Ag", font).Height * 1.35;
            double y = col.Y;

            while (idx < items.Count)
            {
                string line = $"{idx + 1:00} – {items[idx]}";
                var wrapped = WrapByWidth(gfx, line, font, col.Width);

                double blockH = wrapped.Count * lineH;
                if (y + blockH > col.Bottom) break;

                foreach (var ln in wrapped)
                {
                    gfx.DrawString(ln, font, textBrush,  // << usar a brush recebida
                        new XRect(col.X, y, col.Width, lineH), XStringFormats.TopLeft);
                    y += lineH;
                }
                idx++;
            }
        }

        private static void DrawWrappedCenteredText(XGraphics gfx, string text, XFont baseFont, XRect rect, int maxLines = 10, double minPoint = 7.5, double lineSpacing = 1.10, XBrush brush = null)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            brush ??= XBrushes.Black;

            var f = baseFont;

            while (true)
            {
                double lineH = gfx.MeasureString("Ag", f).Height * lineSpacing;
                var lines = WrapByWidth(gfx, text, f, rect.Width);

                int allowedByHeight = Math.Max(1, (int)Math.Floor(rect.Height / lineH));
                int allowed = Math.Min(maxLines, allowedByHeight);

                if (lines.Count <= allowed)
                {
                    double totalH = lines.Count * lineH;
                    double y = rect.Y + (rect.Height - totalH) / 2.0;

                    foreach (var ln in lines)
                    {
                        gfx.DrawString(ln, f, brush,
                            new XRect(rect.X, y, rect.Width, lineH),
                            XStringFormats.TopCenter);
                        y += lineH;
                    }
                    return;
                }

                // diminui o tamanho da fonte
                double nextPt = f.Size - 0.5;
                if (nextPt < minPoint)
                {
                    // chegou no limite: desenha o que couber (sem "…")
                    lines = lines.Take(allowed).ToList();
                    double totalH = lines.Count * lineH;
                    double y = rect.Y + (rect.Height - totalH) / 2.0;

                    foreach (var ln in lines)
                    {
                        gfx.DrawString(ln, f, brush,
                            new XRect(rect.X, y, rect.Width, lineH),
                            XStringFormats.TopCenter);
                        y += lineH;
                    }
                    return;
                }

                // ✅ NÃO use f.Options (não existe em PdfSharpCore)
                f = new XFont(f.Name, nextPt, f.Style);
                // Se precisar garantir Unicode/Embedding, use o overload com XPdfFontOptions:
                // using PdfSharpCore.Pdf;
                // var opts = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                // f = new XFont(f.Name, nextPt, f.Style, opts);
            }
        }

        private static List<string> WrapByWidth(XGraphics gfx, string text, XFont font, double maxW)
        {
            var tokens = Regex.Split(text, @"(\s+)"); // preserva espaços
            var lines = new List<string>();
            var sb = new StringBuilder();

            foreach (var tok in tokens)
            {
                string candidate = sb.Length == 0 ? tok.TrimStart() : sb.ToString() + tok;
                if (gfx.MeasureString(candidate, font).Width <= maxW)
                {
                    sb.Clear(); sb.Append(candidate);
                    continue;
                }

                // fecha a linha atual (se tiver algo)
                if (sb.Length > 0)
                {
                    lines.Add(sb.ToString().TrimEnd());
                    sb.Clear();
                }

                // token sozinho não cabe: quebra por caracteres
                string t = tok.Trim();
                if (t.Length == 0) continue;

                int start = 0;
                while (start < t.Length)
                {
                    int len = 1;
                    while (start + len <= t.Length &&
                           gfx.MeasureString(t.AsSpan(start, len).ToString(), font).Width <= maxW)
                        len++;
                    if (len > 1) len--; // último que coube
                    lines.Add(t.Substring(start, len));
                    start += len;
                }
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString().TrimEnd());

            return lines;
        }

        private static void DrawCropMarks(XGraphics gfx, double margin, double pageW, double pageH, double markLen = 10)
        {
            var cropPen = new XPen(XColors.Gray, 0.6);

            // topo-esquerda
            gfx.DrawLine(cropPen, margin - markLen, margin, margin, margin);
            gfx.DrawLine(cropPen, margin, margin - markLen, margin, margin);

            // topo-direita
            gfx.DrawLine(cropPen, pageW - margin + markLen, margin, pageW - margin, margin);
            gfx.DrawLine(cropPen, pageW - margin, margin - markLen, pageW - margin, margin);

            // base-esquerda
            gfx.DrawLine(cropPen, margin - markLen, pageH - margin, margin, pageH - margin);
            gfx.DrawLine(cropPen, margin, pageH - margin + markLen, margin, pageH - margin);

            // base-direita
            gfx.DrawLine(cropPen, pageW - margin + markLen, pageH - margin, pageW - margin, pageH - margin);
            gfx.DrawLine(cropPen, pageW - margin, pageH - margin + markLen, pageW - margin, pageH - margin);
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name)
                sb.Append(invalid.Contains(ch) ? '_' : ch);
            return sb.ToString();
        }

        static string Sanitize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "Cartelas";
            foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

        public static void PrintCardsSubset(int setId, IEnumerable<int> cardNumbers)
        {
            var cards = DataService.GetCardSetById(setId);
            if (cards == null) return;

            // normaliza e filtra as linhas de cartelas
            var wanted = new HashSet<int>((cardNumbers ?? Enumerable.Empty<int>()).Distinct());
            if (wanted.Count == 0) return;

            var allRows = DataService.GetCardsBySetId(setId);
            var rows = allRows
                .Where(r => wanted.Contains(Convert.ToInt32(r["CardNumber"])))
                .OrderBy(r => Convert.ToInt32(r["CardNumber"]))
                .ToList();

            if (rows.Count == 0) return;

            // monta os elementos (na ordem) para cada cartela
            var cardElements = DataService.GetCardElementsBySet(rows);

            // ⚠️ seus métodos PrintCards4x4/5x5 usam cards.Quantity para iterar.
            // Ajustamos temporariamente para imprimir só o subset.
            int originalQty = cards.Quantity;
            cards.Quantity = cardElements.Count;

            if (cards.CardsSize == 5)
                PrintCards5x5(cards, cardElements);  // já existente no seu projeto
            else if (cards.CardsSize == 4)
                PrintCards4x4(cards, cardElements);  // já existente no seu projeto

            cards.Quantity = originalQty; // restaura
        }


    }
}
