using BingoCreator.Models;
using BingoCreator.Services;
using System.Data;
using System.Text;

namespace BingoCreator
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();

            DataService.InitializeDatabase();

            DesignService.UseDefaultLogo();

            CreatePageLoad();
            EditPageLoad();
        }

        // Métodos de Carregamento
        // Método para carregar as ComboBox
        private void CreatePageLoad()
        {
            // Listas para criação
            var dtLists = DataService.GetLists();
            var lists = dtLists.AsEnumerable()
                .Select(r => new ListModel
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? "",
                    Description = r["Description"]?.ToString() ?? "",
                    ImageName = r["ImageName"]?.ToString() ?? ""
                }).ToArray();

            cboElementList.Items.Clear();
            cboCardsList.Items.Clear();
            cboCardsList.Items.Clear();
            foreach (var lm in lists)
            {
                cboElementList.Items.Add(lm);
                cboCardsList.Items.Add(lm);
                cboAlocateList.Items.Add(lm);
            }
            cboElementList.DisplayMember = "Name";
            cboCardsList.DisplayMember = "Name";
            cboAlocateList.DisplayMember = "Name";

            var themeOptions = ThemeCatalog.All
                .Select(k => new ThemeOption { Key = k.Key, Name = k.Value.DisplayName })
                .ToList();
            cboCardsTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsTheme.DisplayMember = "Name";
            cboCardsTheme.ValueMember = "Key";
            cboCardsTheme.DataSource = themeOptions;

            cboCardsModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsModel.DisplayMember = "Text";
            cboCardsModel.ValueMember = "Value";
            cboCardsModel.DataSource = new[] {
                new { Text = "Quadradas (fundo branco)",     Value = "SQUARE"  },
                new { Text = "Arredondadas (fundo do tema)", Value = "ROUNDED" }
            };
            cboCardsModel.SelectedValue = "SQUARE";

            cboCardsHeader.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsHeader.DisplayMember = "Text";
            cboCardsHeader.ValueMember = "Value";
            cboCardsHeader.DataSource = new[] {
                new { Text = "SORTE", Value = "SORTE" },
                new { Text = "BINGO", Value = "BINGO" }
            };
            cboCardsHeader.SelectedValue = "SORTE";

            cboCardsSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsSize.DisplayMember = "Text";
            cboCardsSize.ValueMember = "Value";
            cboCardsSize.DataSource = new[] {
                new { Text = "4x4", Value = "4" },
                new { Text = "5x5", Value = "5" }
            };
            cboCardsSize.SelectedValue = "5";

            var dt = DataService.GetAllCardSets();
            cboAddCardsList.DisplayMember = "Name";
            cboAddCardsList.ValueMember = "Id";
            cboAddCardsList.DataSource = dt;
            cboAddCardsList.SelectedIndex = -1;
        }
        private void EditPageLoad()
        {
            // cbo1
            cboEdit1.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEdit1.Items.Clear();
            cboEdit1.Items.AddRange(new[] { "Conjuntos", "Listas", "Elementos" });

            // cbo2 / cbo3
            cboEdit2.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEdit3.DropDownStyle = ComboBoxStyle.DropDownList;

            // desinscreve e reinscreve (evita múltiplos handlers)
            cboEdit1.SelectedIndexChanged -= cboEdit1_SelectedIndexChanged;
            cboEdit2.SelectedIndexChanged -= cboEdit2_SelectedIndexChanged;
            cboEdit3.SelectedIndexChanged -= cboEdit3_SelectedIndexChanged;

            cboEdit1.SelectedIndexChanged += cboEdit1_SelectedIndexChanged;
            cboEdit2.SelectedIndexChanged += cboEdit2_SelectedIndexChanged;
            cboEdit3.SelectedIndexChanged += cboEdit3_SelectedIndexChanged;

            cboEdit1.SelectedIndex = -1;
            cboEdit2.DataSource = null; cboEdit2.Enabled = false;
            cboEdit3.DataSource = null; cboEdit3.Enabled = false;

            ClearEditFields();
        }
        private void cboEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearEditFields();

            var sel = cboEdit1.SelectedItem?.ToString();
            cboEdit2.DataSource = null; cboEdit2.Enabled = false;
            cboEdit3.DataSource = null; cboEdit3.Enabled = false;

            if (string.IsNullOrEmpty(sel)) return;

            switch (sel)
            {
                case "Conjuntos":
                    {
                        var dt = DataService.GetAllCardSets();
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;
                        break;
                    }
                case "Listas":
                    {
                        var dt = DataService.GetLists();
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;
                        break;
                    }
                case "Elementos":
                    {
                        var dt = DataService.GetAllElements(); // ordenado por Name
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;

                        // Situação 3: cbo3 desabilitada
                        cboEdit3.DataSource = null;
                        cboEdit3.Enabled = false;
                        break;
                    }
            }
        }
        private void cboEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearEditFields();

            var sel1 = cboEdit1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(sel1)) return;

            if (cboEdit2.SelectedValue == null || !int.TryParse(cboEdit2.SelectedValue.ToString(), out int id))
            {
                cboEdit3.DataSource = null; cboEdit3.Enabled = false;
                return;
            }

            switch (sel1)
            {
                case "Conjuntos":
                    {
                        // carrega set e elementos dele
                        var set = DataService.GetCardSetById(id);
                        var items = new List<(int Id, string Name, string Kind)>();

                        // 1º o próprio conjunto
                        items.Add((set.Id, set.Name, "SET"));

                        // depois os elementos do conjunto (use os GroupB.. ou AllElements)
                        var elems = (set.CardsSize == 5)
                            ? new[] { set.GroupB, set.GroupI, set.GroupN, set.GroupG, set.GroupO }
                                .Where(g => g != null).SelectMany(g => g).ToList()
                            : (set.AllElements ?? new List<ElementModel>());
                        foreach (var el in elems.DistinctBy(x => x.Id))
                            items.Add((el.Id, el.CardName?.Trim().Length > 0 ? el.CardName : el.Name, "ELE"));

                        cboEdit3.DisplayMember = "Name";
                        cboEdit3.ValueMember = "Id";
                        cboEdit3.DataSource = items.Select(t => new { t.Id, t.Name, t.Kind }).ToList();
                        cboEdit3.SelectedIndex = 0;
                        cboEdit3.Enabled = true;

                        ApplySetFields(set);
                        break;
                    }

                case "Listas":
                    {
                        // carrega lista + elementos da lista
                        var list = DataService.GetListById(id);
                        var rows = DataService.GetElementsInList(id);

                        var items = new List<(int Id, string Name, string Kind)>();
                        items.Add((list.Id, list.Name, "LIST"));

                        foreach (var r in rows)
                        {
                            int eid = Convert.ToInt32(r["Id"]);
                            string name = r["CardName"]?.ToString();
                            if (string.IsNullOrWhiteSpace(name))
                                name = r["Name"]?.ToString() ?? "";
                            items.Add((eid, name, "ELE"));
                        }

                        cboEdit3.DisplayMember = "Name";
                        cboEdit3.ValueMember = "Id";
                        cboEdit3.DataSource = items.Select(t => new { t.Id, t.Name, t.Kind }).ToList();
                        cboEdit3.SelectedIndex = 0; // própria lista
                        cboEdit3.Enabled = true;

                        // Preenche campos da lista
                        ApplyListFields(list, rows.Count);
                        break;
                    }

                case "Elementos":
                    {
                        // Sem “filhos”: preenche direto pelo elemento
                        var row = DataService.GetElementById(id);
                        if (row != null)
                        {
                            var em = new ElementModel
                            {
                                Id = id,
                                Name = row["Name"]?.ToString() ?? "",
                                CardName = row["CardName"]?.ToString() ?? "",
                                Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                                Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                            };
                            ApplyElementFields(em);
                        }
                        cboEdit3.DataSource = null;
                        cboEdit3.Enabled = false;
                        break;
                    }
            }
        }
        private void cboEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sel1 = cboEdit1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(sel1)) return;
            if (cboEdit2.SelectedValue == null || !int.TryParse(cboEdit2.SelectedValue.ToString(), out int parentId)) return;

            var data = cboEdit3.SelectedItem;
            if (data == null) return;

            var propId = data.GetType().GetProperty("Id");
            var propKind = data.GetType().GetProperty("Kind");
            int id = (int)(propId?.GetValue(data) ?? 0);
            string kind = propKind?.GetValue(data)?.ToString() ?? "";

            if (sel1 == "Conjuntos")
            {
                if (kind == "SET")
                {
                    var set = DataService.GetCardSetById(id);
                    ApplySetFields(set);
                }
                else
                {
                    var row = DataService.GetElementById(id);
                    if (row != null)
                    {
                        var em = new ElementModel
                        {
                            Id = id,
                            Name = row["Name"]?.ToString() ?? "",
                            CardName = row["CardName"]?.ToString() ?? "",
                            Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                            Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                        };
                        ApplyElementFields(em);
                    }
                }
            }
            else if (sel1 == "Listas")
            {
                if (kind == "LIST")
                {
                    var list = DataService.GetListById(id);
                    int count = DataService.GetElementsInList(id).Count;
                    ApplyListFields(list, count);
                }
                else // "ELE"
                {
                    var row = DataService.GetElementById(id);
                    if (row != null)
                    {
                        var em = new ElementModel
                        {
                            Id = id,
                            Name = row["Name"]?.ToString() ?? "",
                            CardName = row["CardName"]?.ToString() ?? "",
                            Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                            Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                        };
                        ApplyElementFields(em);
                    }
                }
            }
        }
        private void ClearEditFields()
        {
            ActiveSetId = -1;
            ActiveListId = -1;
            ActiveElementId = -1;

            btnEditEdit.Enabled = false;
            btnEditExclude.Enabled = false;
            btnEditDeleteAll.Enabled = false;

            lblEditText1.Text = lblEditText2.Text = lblEditText3.Text = lblEditText4.Text = lblEditText5.Text = "";
            boxEditText1.Text = boxEditText2.Text = boxEditText3.Text = boxEditText4.Text = boxEditText5.Text = "";
            lblEditImage.Text = "";
            lblEditMessage.Text = "Nada Selecionado";

            boxEditText1.Enabled = true;
            boxEditText2.Enabled = true;
            boxEditText3.Enabled = true;
            boxEditText4.Enabled = true;
            boxEditText5.Enabled = true;

            // mostra tudo por padrão
            SetFieldVisible(1, false);
            SetFieldVisible(2, false);
            SetFieldVisible(3, false);
            SetFieldVisible(4, false);
            SetFieldVisible(5, false);
        }

        private void SetFieldVisible(int idx, bool visible)
        {
            switch (idx)
            {
                case 1: lblEditText1.Visible = boxEditText1.Visible = visible; break;
                case 2: lblEditText2.Visible = boxEditText2.Visible = visible; break;
                case 3: lblEditText3.Visible = boxEditText3.Visible = visible; break;
                case 4: lblEditText4.Visible = boxEditText4.Visible = visible; break;
                case 5: lblEditText5.Visible = boxEditText5.Visible = visible; break;
            }
        }
        private void ApplySetFields(CardSetModel set)
        {
            ClearEditFields();

            ActiveSetId = set.Id;
            btnEditEdit.Enabled = true;
            btnEditExclude.Enabled = true;

            lblEditMessage.Text = $"Id do Conjunto selecionado: {set.Id.ToString()}";

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Título:";
            lblEditText3.Text = "Final:";
            lblEditText4.Text = "Estilo:";
            lblEditText5.Text = "Lista e Total de Cartelas:";

            boxEditText1.Text = set.Name;
            boxEditText2.Text = set.Title;
            boxEditText3.Text = set.End;
            boxEditText4.Text = $"{set.Model}, {set.Theme}, Tamanho {set.CardsSize}";
            boxEditText5.Text = $"{set.ListName}, {set.ListSize} Elementos, {set.Quantity.ToString()} Cartelas";

            boxEditText4.Enabled = false;
            boxEditText5.Enabled = false;

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);
            SetFieldVisible(4, true);
            SetFieldVisible(5, true);
        }
        private void ApplyListFields(ListModel list, int elementCount)
        {
            ClearEditFields();

            ActiveListId = list.Id;
            btnEditEdit.Enabled = true;
            btnEditExclude.Enabled = true;
            btnEditDeleteAll.Enabled = true;

            lblEditMessage.Text = $"Id da Lista selecionada: {list.Id.ToString()}";

            var setsDt = DataService.GetCardSetsByListId(list.Id);

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Descrição:";
            lblEditText3.Text = "Total de Elementos:";
            lblEditText4.Text = "Conjuntos Criados:";

            boxEditText1.Text = list?.Name ?? "";
            boxEditText2.Text = list?.Description ?? "";
            boxEditText3.Text = elementCount.ToString();
            if (setsDt.Rows.Count > 0)
            {
                var linhas = setsDt.AsEnumerable()
                    .Select(r =>
                    {
                        var setId = Convert.ToInt32(r["SetId"]);
                        var name = r["Name"]?.ToString() ?? "";
                        var title = r["Title"]?.ToString() ?? "";
                        var size = Convert.ToInt32(r["CardsSize"]);
                        var qnt = Convert.ToInt32(r["Quantity"]);
                        var when = r["AddTime"]?.ToString() ?? "";
                        return $"#{setId} – {name}  |  {title}  |  {size}x{size}  |  {qnt} cartelas  |  {when}";
                    });

                boxEditText4.Text = string.Join(Environment.NewLine, linhas);
                btnEditExclude.Enabled = false;
            }
            else
            {
                boxEditText4.Text = "Nenhum conjunto usa esta lista.";
                btnEditExclude.Enabled = true;
            }

            boxEditText3.Enabled = false;
            boxEditText4.Enabled = false;

            UpdatePicEdit(list.ImageName);

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);
            SetFieldVisible(4, true);

            SetFieldVisible(5, false);
        }
        private void ApplyElementFields(ElementModel em)
        {
            ClearEditFields();

            ActiveElementId = em.Id;
            btnEditEdit.Enabled = true;
            btnEditExclude.Enabled = true;

            lblEditMessage.Text = $"Id do Elemento selecionado: {em.Id.ToString()}";

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Nome para Cartela:";
            lblEditText3.Text = "Anotação 1:";
            lblEditText4.Text = "Anotação 2:";
            lblEditText5.Text = "Listas:";

            boxEditText1.Text = em?.Name ?? "";
            boxEditText2.Text = em?.CardName ?? "";
            boxEditText3.Text = em?.Note1 ?? "";
            boxEditText4.Text = em?.Note2 ?? "";

            UpdatePicEdit(em.ImageName);

            boxEditText5.Enabled = false;

            var lists = DataService.GetListsForElement(em.Id);
            boxEditText5.Text = lists;

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);
            SetFieldVisible(4, true);
            SetFieldVisible(5, true);
        }


        // Métodos de Criação
        // Criar uma Elemento
        private void btnElementCreat_Clicked(object sender, EventArgs e)
        {
            ElementModel element = new ElementModel();
            var list = new ListModel();

            element.Name = boxElementName.Text.Trim();
            element.CardName = boxElementCardName.Text.Trim();
            element.Note1 = boxElementNote1.Text.Trim();
            element.Note2 = boxElementNote1.Text.Trim();

            if (cboElementList.SelectedIndex > -1)
            {
                list = cboElementList.SelectedItem as ListModel;
            }
            else
            {
                list.Id = 0;
            }

            try
            {
                var creation = CreatingService.CreateElement(element, list);

                lblElementMessage.Text = creation.Message;

                if (creation.Success)
                {
                    boxElementName.Text = "";
                    boxElementCardName.Text = "";
                    boxElementNote1.Text = "";
                    boxElementNote2.Text = "";
                    cboElementList.SelectedIndex = -1;

                    EditPageLoad();
                    CreatePageLoad();
                }
                else
                {
                    lblElementMessage.Text = $"{creation.Message}";
                    return;
                }
            }
            catch (Exception ex)
            {
                lblElementMessage.Text = "Erro inesperado ao criar elemento: " + ex.Message;
            }
        }
        // Criar Lista
        private void btnListCreate_Clicked(object sender, EventArgs e)
        {
            ListModel list = new ListModel();

            list.Name = boxListName.Text.ToUpper().Trim();
            list.Description = boxListDescription.Text.Trim();

            int maxNameLength = 100;
            int maxDescriptionLength = 300;

            try
            {
                var creation = CreatingService.CreateList(list);

                lblListMessage.Text = creation.Message;

                if (creation.Success)
                {
                    boxListName.Text = "";
                    boxListDescription.Text = "";

                    CreatePageLoad();
                    EditPageLoad();
                }
                else
                {
                    lblListMessage.Text = $"{creation.Message}";
                    return;
                }
            }
            catch (Exception ex)
            {
                lblListMessage.Text = "Erro inesperado ao criar lista: " + ex.Message;
            }
        }
        // Criar Cartelas
        private void btnExportCards_Click(object sender, EventArgs e)
        {
            btnCardsExport.Enabled = false;

            CardSetModel cards = new CardSetModel();
            ListModel list = new ListModel();

            list = cboCardsList.SelectedItem as ListModel;
            if (list == null)
            {
                lblCardsMessage.Text = "Dados insuficientes";
                btnCardsExport.Enabled = true;
                return;
            }
            cards.ListId = list.Id;

            cards.Name = boxCardsName.Text.Trim();
            cards.Title = boxCardsTitle.Text.Trim();
            cards.End = boxCardsEnd.Text.Trim();
            cards.Quantity = (int)boxCardsQuantity.Value;

            cards.Theme = cboCardsTheme.SelectedValue as string ?? "MINIMAL";
            cards.Model = (cboCardsModel.SelectedValue as string) ?? "SQUARE";
            cards.Header = (cboCardsHeader.SelectedValue as string) ?? "SORTE";

            int size = 5;
            if (cboCardsSize.SelectedValue is int v)
                size = v;
            else if (int.TryParse(cboCardsSize.SelectedValue?.ToString(), out var parsed))
                size = parsed;
            cards.CardsSize = size;

            try
            {
                var creation = CreatingService.CreateCards(cards);

                if (creation.Success)
                {
                    boxCardsName.Text = string.Empty;
                    boxCardsQuantity.Value = 100;
                    boxCardsTitle.Text = string.Empty;
                    boxCardsEnd.Text = string.Empty;
                    cboCardsList.SelectedIndex = -1;
                    cboCardsTheme.SelectedIndex = -1;
                    btnCardsExport.Enabled = true;
                    cboCardsList.SelectedIndex = -1;
                }
                else
                {
                    lblCardsMessage.Text = $"{creation.Message}";

                    boxCardsName.Text = string.Empty;
                    boxCardsQuantity.Value = 100;
                    boxCardsTitle.Text = string.Empty;
                    boxCardsEnd.Text = string.Empty;
                    cboCardsList.SelectedIndex = -1;
                    cboCardsTheme.SelectedIndex = -1;
                    btnCardsExport.Enabled = true;
                    cboCardsList.SelectedIndex = -1;
                    return;
                }

                var printAns = MessageBox.Show(
                    "Deseja imprimir as cartelas agora?",
                    "Imprimir cartelas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (printAns == DialogResult.Yes)
                {
                    try
                    {
                        PrintingService.PrintCards(creation.Id);
                        lblCardsMessage.Text = "PDFs gerados com sucesso";

                        var exportAns = MessageBox.Show(
                            "Deseja exportar o banco de dados do jogo também?",
                            "Exportar DB",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (exportAns == DialogResult.Yes)
                        {
                            try
                            {
                                ExportingService.ExportDataBase(creation.Id);
                                lblCardsMessage.Text = "Banco de dados exportado com sucesso";
                                EditPageLoad();
                                CreatePageLoad();
                            }
                            catch (Exception ex)
                            {
                                lblCardsMessage.Text = "Erro inesperado ao exportar o banco de dados: " + ex.Message;
                            }
                        }
                        else
                        {
                            EditPageLoad();
                            CreatePageLoad();
                        }
                    }
                    catch (Exception ex)
                    {
                        lblCardsMessage.Text = "Erro inesperado ao gerar PDFs: " + ex.Message;
                    }
                }
                else
                {
                    EditPageLoad();
                    CreatePageLoad();
                }
            }
            catch (Exception ex)
            {
                lblCardsMessage.Text = "Erro inesperado ao criar cartelas: " + ex.Message;
            }
        }
        // Adicionar Cartelas a um Conjunto Existente
        private void btnAddCards_Click(object sender, EventArgs e)
        {
            try
            {
                // --- 1) qual conjunto e quantas cartelas? ---
                if (!TryGetSetIdFromCombo(cboAddCardsList, out int setId, out string setName))
                {
                    MessageBox.Show("Selecione um conjunto válido.", "Adicionar cartelas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int howMany = 0;
                if (boxAddQuant is NumericUpDown nud)
                    howMany = (int)nud.Value;
                else
                    howMany = int.TryParse(boxAddQuant?.Text, out var v) ? v : 0;

                if (howMany <= 0)
                {
                    MessageBox.Show("Informe uma quantidade maior que zero.", "Adicionar cartelas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- 2) captura total antes, gera, lê total depois ---
                var before = DataService.GetCardSetById(setId);
                int oldTotal = before?.Quantity ?? 0;

                Cursor.Current = Cursors.WaitCursor;
                int created = GeneratingService.AddCardsToSet(setId, howMany); // deve evitar duplicatas e atualizar Quantity
                Cursor.Current = Cursors.Default;

                var after = DataService.GetCardSetById(setId);
                int newTotal = after?.Quantity ?? oldTotal;   // se falhar leitura, cai pro old

                // Números das novas cartelas
                var newCardNumbers = Enumerable.Range(oldTotal + 1, Math.Max(0, newTotal - (oldTotal)))
                                               .ToList();

                MessageBox.Show(
                    $"Adicionadas {created} cartelas ao conjunto \"{setName}\".\n" +
                    $"Total agora: {newTotal}.",
                    "Adicionar cartelas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- 3) imprimir? (Completa / Novas / Não) ---
                var printChoice = AskThreeChoice(
                    "Imprimir cartelas",
                    "Deseja imprimir as cartelas?\n\n" +
                    "Sim = Imprimir todas\nNão = Imprimir apenas as novas\nCancelar = Não imprimir");

                switch (printChoice)
                {
                    case ThreeChoice.Full:
                        PrintingService.PrintCards(setId); // imprime o set completo
                        break;
                    case ThreeChoice.New:
                        if (newCardNumbers.Count > 0)
                        {
                            // implemente no seu PrintingService:
                            // public static void PrintCardsSubset(int setId, IEnumerable<int> cardNumbers)
                            PrintingService.PrintCardsSubset(setId, newCardNumbers);
                        }
                        else
                        {
                            MessageBox.Show("Não há cartelas novas para imprimir.", "Imprimir",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    case ThreeChoice.None:
                        break;
                }

                // --- 4) exportar DB? (Completa / Novas / Não) ---
                var exportChoice = AskThreeChoice(
                    "Exportar banco de dados",
                    "Deseja exportar o banco de dados do jogo?\n\n" +
                    "Sim = Exportar todas as cartelas\nNão = Exportar apenas as novas\nCancelar = Não exportar");

                switch (exportChoice)
                {
                    case ThreeChoice.Full:
                        ExportingService.ExportDataBase(setId); // exporta tudo
                        break;
                    case ThreeChoice.New:
                        if (newCardNumbers.Count > 0)
                        {
                            // implemente no seu ExportingService (mesma ideia do completo, mas filtrando pelos números):
                            // public static void ExportDataBaseSubset(int setId, IEnumerable<int> cardNumbers)
                            ExportingService.ExportDataBaseSubset(setId, newCardNumbers);
                        }
                        else
                        {
                            MessageBox.Show("Não há cartelas novas para exportar.", "Exportar",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    case ThreeChoice.None:
                        break;
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Falha ao adicionar/imprimir/exportar: " + ex.Message,
                    "Adicionar cartelas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Reaproveite este helper (igual ao que te passei antes)
        private bool TryGetSetIdFromCombo(ComboBox combo, out int id, out string name)
        {
            id = 0; name = "";
            if (combo == null || combo.SelectedItem == null) return false;

            var it = combo.SelectedItem;

            if (it is CardSetModel m)
            {
                id = m.Id; name = m.Name ?? "";
                return id > 0;
            }
            if (it is DataRowView drv)
            {
                if (drv.DataView?.Table?.Columns.Contains("Id") == true)
                    id = Convert.ToInt32(drv["Id"]);
                name = drv.DataView?.Table?.Columns.Contains("Name") == true
                     ? (drv["Name"]?.ToString() ?? "")
                     : (combo.Text ?? "");
                return id > 0;
            }
            if (combo.SelectedValue is int v && v > 0)
            {
                id = v; name = combo.Text ?? "";
                return true;
            }
            if (int.TryParse(combo.SelectedValue?.ToString(), out var v2) && v2 > 0)
            {
                id = v2; name = combo.Text ?? "";
                return true;
            }
            return false;
        }


        // Métodos de Importação
        // Importar Lista por Pasta de Imagens
        private void btnListImport_Clicked(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Selecione a pasta contendo os arquivos da lista (capa .Capa e elementos)."
            };

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            string folder = fbd.SelectedPath;
            string listName = Path.GetFileName(folder);

            // 1) Coleta arquivos de imagem (png primeiro, depois jpg e jpeg)
            var imageFiles = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" }
                .SelectMany(p => Directory.EnumerateFiles(folder, p))
                .ToList();

            if (imageFiles.Count == 0)
            {
                MessageBox.Show("Nenhuma imagem .png ou .jpg encontrada na pasta selecionada.",
                                "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ErrorModel creation = ImportingService.ImportImages(imageFiles, listName);

                if (creation.Success)
                {
                    boxListName.Text = "";
                    boxListDescription.Text = "";

                    MessageBox.Show("Sucesso ao importar a Lista", "Importar Pasta de Imagens", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    EditPageLoad();
                    CreatePageLoad();
                }
                else
                {
                    MessageBox.Show($"Erro ao importar a Lista: {creation.Message}", "Importar Pasta de Imagens", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro desconhecido ao importar a Lista", "Importar Pasta de Imagens", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Importar Lista por TXT, remove acentos e caracteres não permitidos
        private void btnListTxt_Clicked(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo de lista (TXT/CSV)",
                Filter = "Texto/CSV (*.txt;*.csv)|*.txt;*.csv|Todos os arquivos (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string path = ofd.FileName;
            string listName = Path.GetFileNameWithoutExtension(path);

            try
            {
                ErrorModel creation = ImportingService.ImportTxt(path, listName);

                if (creation.Success)
                {
                    boxListName.Text = "";
                    boxListDescription.Text = "";

                    MessageBox.Show("Sucesso ao importar a Lista", "Importar Arquivo TXT", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    EditPageLoad();
                    CreatePageLoad();
                }
                else
                {
                    MessageBox.Show($"Erro ao importar a Lista: {creation.Message}", "Importar Arquivo TXT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro desconhecido ao importar a Lista", "Importar Arquivo TXT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // Métodos de Edição
        private int ActiveSetId = -1;
        private int ActiveListId = -1;
        private int ActiveElementId = -1;
        private void btnEditExclude_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActiveElementId > 0)
                {
                    var confirm = MessageBox.Show(
                        $"Tem certeza que deseja excluir esse elemento?",
                        "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (confirm != DialogResult.Yes) return;

                    bool ok = DataService.DeleteElement(ActiveElementId);

                    if (ok)
                    {
                        MessageBox.Show("Elemento excluído com sucesso.", "Excluir Elemento",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                    else
                    {
                        MessageBox.Show("Não é possível excluir o Elemento.",
                            "Excluir Elemento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (ActiveListId > 0)
                {
                    var confirm = MessageBox.Show(
                        $"Tem certeza que deseja excluir essa lista?",
                        "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (confirm != DialogResult.Yes) return;

                    bool ok = DataService.DeleteListAndAllocations(ActiveListId);

                    if (ok)
                    {
                        MessageBox.Show("Lista excluída com sucesso.", "Excluir Lista",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                    else
                    {
                        MessageBox.Show("Não é possível excluir a Lista.",
                            "Excluir Lista", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (ActiveSetId > 0)
                {
                    var confirm = MessageBox.Show(
                        $"Tem certeza que deseja excluir esse conjunto?",
                        "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (confirm != DialogResult.Yes) return;

                    DataService.DeleteCardsBySetId(ActiveSetId);
                    bool ok = DataService.DeleteCardSet(ActiveSetId);

                    if (ok)
                    {
                        MessageBox.Show("Conjunto excluído com sucesso.", "Excluir Conjunto",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                    else
                    {
                        MessageBox.Show("Não é possível excluir o Conjunto.",
                            "Excluir Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEditDeleteAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActiveListId <= 0)
                {
                    MessageBox.Show("Nenhuma lista selecionada.", "Excluir Elementos",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    "Tem certeza que deseja excluir TODOS os elementos desta lista?",
                    "Confirmar Exclusão",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (confirm != DialogResult.Yes) return;

                // Se o método retorna uma tupla (int removedAllocations, int removedElements)
                var (removedAllocations, removedElements) = DataService.DeleteElementsInList(
                    ActiveListId, deleteOrphanElements: true);

                MessageBox.Show(
                    $"Remoções concluídas.\n" +
                    $"- Alocações removidas: {removedAllocations}\n" +
                    $"- Elementos removidos: {removedElements}",
                    "Excluir Elementos", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cboEdit1.SelectedIndex = -1;
                cboEdit2.SelectedIndex = -1;
                cboEdit3.SelectedIndex = -1;
                ClearEditFields();
                EditPageLoad();
                CreatePageLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir elementos: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEditEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Elemento
                if (ActiveElementId > 0)
                {
                    var oldElement = DataService.GetElementModelById(ActiveElementId);
                    if (oldElement == null)
                    {
                        MessageBox.Show("Elemento não encontrado.", "Editar Elemento",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newElement = new ElementModel
                    {
                        Name = boxEditText1.Text?.Trim(),
                        CardName = boxEditText2.Text?.Trim(),
                        Note1 = boxEditText3.Text?.Trim(),
                        Note2 = boxEditText4.Text?.Trim(),
                        ImageName = $"{boxEditText2.Text?.Trim()}.png"
                    };

                    int newId = DataService.EditElement(ActiveElementId, newElement);
                    if (newId > 0)
                    {
                        int updList = DataService.EditElementInList(oldElement.Id, newId);
                        int updCards = DataService.EditElementInCardSet(oldElement.Id, newId);

                        MessageBox.Show(
                            $"Elemento editado (nova versão criada: ID {newId}).\n" +
                            $"- Alocações atualizadas: {updList}\n" +
                            $"- Cartelas/Conjuntos atualizados: {updCards}",
                            "Editar Elemento", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível criar a nova versão do elemento.",
                            "Editar Elemento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Lista
                else if (ActiveListId > 0)
                {
                    var list = DataService.GetListById(ActiveListId);
                    if (list == null)
                    {
                        MessageBox.Show("Lista não encontrada.", "Editar Lista",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    list.Name = boxEditText1.Text?.Trim();
                    list.Description = boxEditText2.Text?.Trim();
                    list.ImageName = $"capa.{(list.Name ?? "lista")}.png";

                    bool ok = DataService.EditList(list);
                    MessageBox.Show(ok ? "Lista atualizada com sucesso." : "Nenhuma alteração aplicada.",
                        "Editar Lista", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    if (ok)
                    {
                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                }
                // Conjunto
                else if (ActiveSetId > 0)
                {
                    var set = DataService.GetCardSetById(ActiveSetId);
                    if (set == null)
                    {
                        MessageBox.Show("Conjunto não encontrado.", "Editar Conjunto",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    set.Name = boxEditText1.Text?.Trim();
                    set.Title = boxEditText2.Text?.Trim();
                    set.End = boxEditText3.Text?.Trim();

                    bool ok = DataService.EditCardSet(set);
                    MessageBox.Show(ok ? "Conjunto atualizado com sucesso." : "Nenhuma alteração aplicada.",
                        "Editar Conjunto", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    if (ok)
                    {
                        cboEdit1.SelectedIndex = -1;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit3.SelectedIndex = -1;
                        ClearEditFields();
                        EditPageLoad();
                        CreatePageLoad();
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum item selecionado para edição.", "Editar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao editar: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Alocar Elementos
        private void cboAlocateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListModel list = new ListModel();

            list = cboAlocateList.SelectedItem as ListModel;

            RenderAlocateListElements(list.Id);
            RenderNotAlocatedListElements(list.Id);
        }
        private void btnAlocateRemove_Click(object sender, EventArgs e)
        {
            if (cboAlocateList.SelectedItem is not ListModel list || list.Id <= 0)
            {
                MessageBox.Show("Selecione uma lista primeiro.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ids = GetCheckedElementIds(flwAlocateList);
            if (ids.Count == 0)
            {
                MessageBox.Show("Nenhum elemento marcado para remover.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var removed = DataService.RemoveElementsFromList(list.Id, ids);
                // removed pode ser int (linhas afetadas) ou bool, dependendo de como você implementou
                // aqui só atualizamos a UI.
                RenderAlocateListElements(list.Id);
                RenderNotAlocatedListElements(list.Id);

                MessageBox.Show("Elementos removidos da lista.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao remover elementos: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAlocateAdd_Click(object sender, EventArgs e)
        {
            if (cboAlocateList.SelectedItem is not ListModel list || list.Id <= 0)
            {
                MessageBox.Show("Selecione uma lista primeiro.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ids = GetCheckedElementIds(flwAlocateElements);
            if (ids.Count == 0)
            {
                MessageBox.Show("Nenhum elemento marcado para adicionar.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DataService.AlocateElements(list.Id, ids);
                RenderAlocateListElements(list.Id);
                RenderNotAlocatedListElements(list.Id);

                MessageBox.Show("Elementos adicionados à lista.", "Alocação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao adicionar elementos: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Helpers
        private sealed class ThemeOption
        {
            public string Key { get; init; }
            public string Name { get; init; }
        }
        private enum ThreeChoice
        {
            Full,   // Imprimir/Exportar Completa
            New,    // Imprimir/Exportar Novas
            None    // Não
        }
        private void UpdatePicEdit(string imageName)
        {
            // Limpa imagem anterior (evita lock)
            if (picEdit.Image != null)
            {
                var old = picEdit.Image;
                picEdit.Image = null;
                old.Dispose();
            }

            if (string.IsNullOrWhiteSpace(imageName))
            {
                picEdit.Image = null;
                return;
            }

            // Resolva caminho: se não for absoluto, torne relativo ao app
            string path = imageName;
            if (!Path.IsPathRooted(path))
            {
                // Se no DB você salva só "images/..." relativo ao app:
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    imageName.Replace('/', Path.DirectorySeparatorChar));
            }

            if (!File.Exists(path))
            {
                picEdit.Image = null; // ou uma imagem “placeholder” se preferir
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    picEdit.Image = Image.FromStream(fs);
                }
                picEdit.SizeMode = PictureBoxSizeMode.Zoom; // ajuste se preferir StretchImage, etc.
            }
            catch
            {
                picEdit.Image = null;
            }
        }
        private void cboCardsSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCardsSize.SelectedValue == "4")
            {
                cboCardsHeader.Enabled = false;
            }
            if (cboCardsSize.SelectedValue == "5")
            {
                cboCardsHeader.Enabled = true;
            }
        }
        private void RenderAlocateListElements(int listId)
        {
            flwAlocateList.SuspendLayout();
            try
            {
                flwAlocateList.Controls.Clear();

                // elementos da lista (já alocados)
                var rows = DataService.GetElementsInList(listId); // Id, Name, CardName, ImageName

                foreach (var r in rows)
                {
                    int id = Convert.ToInt32(r["Id"]);
                    string card = r.Table.Columns.Contains("CardName") ? r["CardName"]?.ToString() : null;
                    string name = r.Table.Columns.Contains("Name") ? r["Name"]?.ToString() : null;
                    string text = string.IsNullOrWhiteSpace(card) ? (name ?? "") : card!.Trim();

                    var cb = new CheckBox
                    {
                        Text = text,
                        Tag = id,
                        AutoSize = true,
                        Checked = false,
                        Margin = new Padding(8, 4, 8, 4)
                    };

                    flwAlocateList.Controls.Add(cb);
                }
            }
            finally
            {
                flwAlocateList.ResumeLayout();
            }
        }
        private void RenderNotAlocatedListElements(int listId)
        {
            flwAlocateElements.SuspendLayout();
            try
            {
                flwAlocateElements.Controls.Clear();

                var allocatedIds = DataService.GetElementsInList(listId)
                                              .Select(r => Convert.ToInt32(r["Id"]))
                                              .ToHashSet();

                var all = DataService.GetAllElements();

                foreach (DataRow r in all.Rows)
                {
                    int id = Convert.ToInt32(r["Id"]);
                    if (allocatedIds.Contains(id)) continue;

                    string card = r.Table.Columns.Contains("CardName") ? r["CardName"]?.ToString() : null;
                    string name = r.Table.Columns.Contains("Name") ? r["Name"]?.ToString() : null;
                    string text = string.IsNullOrWhiteSpace(card) ? (name ?? "") : card!.Trim();

                    var cb = new CheckBox
                    {
                        Text = text,
                        Tag = id,
                        AutoSize = true,
                        Checked = false,
                        Margin = new Padding(8, 4, 8, 4)
                    };

                    flwAlocateElements.Controls.Add(cb);
                }
            }
            finally
            {
                flwAlocateElements.ResumeLayout();
            }
        }
        private static List<int> GetCheckedElementIds(Control container)
        {
            var ids = new List<int>();

            foreach (Control c in container.Controls)
            {
                if (c is CheckBox cb && cb.Checked && cb.Tag is int id && id > 0)
                    ids.Add(id);
            }

            return ids;
        }
        private ThreeChoice AskThreeChoice(string title, string message)
        {
            // mapeia: Yes = Completa, No = Novas, Cancel = Não
            var r = MessageBox.Show(
                message + "\n\n[Sim = Completa | Não = Novas | Cancelar = Não]",
                title,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button3);

            return r switch
            {
                DialogResult.Yes => ThreeChoice.Full,
                DialogResult.No => ThreeChoice.New,
                _ => ThreeChoice.None
            };
        }

    }
}
