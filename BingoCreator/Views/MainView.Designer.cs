namespace BingoCreator
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlMainView = new Panel();
            tabControlMain = new TabControl();
            tabCreatePage = new TabPage();
            tabControlCreate = new TabControl();
            tabCreateElement = new TabPage();
            btnElementCreate = new Button();
            lblElementMessage = new Label();
            lblElementTitle = new Label();
            cboElementList = new ComboBox();
            lblElementList = new Label();
            boxElementNote2 = new TextBox();
            boxElementNote1 = new TextBox();
            boxElementCardName = new TextBox();
            boxElementName = new TextBox();
            lblElementNote2 = new Label();
            lblElementNote1 = new Label();
            lblElementCardName = new Label();
            lblElementName = new Label();
            tabCreateList = new TabPage();
            btnListTxt = new Button();
            btnListImport = new Button();
            btnListCreate = new Button();
            lblListMessage = new Label();
            boxListDescription = new TextBox();
            boxListName = new TextBox();
            lblListDescription = new Label();
            lblListName = new Label();
            lblListTitle = new Label();
            tabCreateAlocation = new TabPage();
            btnAlocateRemove = new Button();
            btnAlocateAdd = new Button();
            pnlAlocateElements = new Panel();
            flwAlocateElements = new FlowLayoutPanel();
            pnlAlocateList = new Panel();
            flwAlocateList = new FlowLayoutPanel();
            lblAlocateTitle = new Label();
            cboAlocateList = new ComboBox();
            tabCreateCards = new TabPage();
            lblCardsSize = new Label();
            cboCardsSize = new ComboBox();
            boxCardsQuantity = new NumericUpDown();
            cboCardsHeader = new ComboBox();
            lblCardsHeader = new Label();
            cboCardsModel = new ComboBox();
            lblCardsModel = new Label();
            cboCardsTheme = new ComboBox();
            lblCardsTheme = new Label();
            btnCardsExport = new Button();
            lblCardsList = new Label();
            cboCardsList = new ComboBox();
            lblCardsQuant = new Label();
            lblCardsEnd = new Label();
            boxCardsEnd = new TextBox();
            lblCardsPage = new Label();
            lblCardsMessage = new Label();
            boxCardsTitle = new TextBox();
            boxCardsName = new TextBox();
            lblCardsTitle = new Label();
            lblCardsName = new Label();
            tabCreateMoreCards = new TabPage();
            btnAddCards = new Button();
            boxAddQuant = new NumericUpDown();
            lblAddQuant = new Label();
            lblAddSet = new Label();
            lblAddCards = new Label();
            cboAddCardsList = new ComboBox();
            tabEditPage = new TabPage();
            btnEditDeleteAll = new Button();
            cboEdit3 = new ComboBox();
            cboEdit2 = new ComboBox();
            boxEditText5 = new TextBox();
            lblEditText5 = new Label();
            btnEditExclude = new Button();
            btnEditEdit = new Button();
            lblEditMessage = new Label();
            cboEdit1 = new ComboBox();
            picEdit = new PictureBox();
            boxEditText4 = new TextBox();
            boxEditText3 = new TextBox();
            boxEditText2 = new TextBox();
            boxEditText1 = new TextBox();
            lblEditImage = new Label();
            lblEditText4 = new Label();
            lblEditText3 = new Label();
            lblEditText2 = new Label();
            lblEditText1 = new Label();
            lblEditHeader = new Label();
            pnlMainView.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabCreatePage.SuspendLayout();
            tabControlCreate.SuspendLayout();
            tabCreateElement.SuspendLayout();
            tabCreateList.SuspendLayout();
            tabCreateAlocation.SuspendLayout();
            pnlAlocateElements.SuspendLayout();
            pnlAlocateList.SuspendLayout();
            tabCreateCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)boxCardsQuantity).BeginInit();
            tabCreateMoreCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)boxAddQuant).BeginInit();
            tabEditPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picEdit).BeginInit();
            SuspendLayout();
            // 
            // pnlMainView
            // 
            pnlMainView.AutoSize = true;
            pnlMainView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlMainView.Controls.Add(tabControlMain);
            pnlMainView.Dock = DockStyle.Fill;
            pnlMainView.Location = new Point(0, 0);
            pnlMainView.Name = "pnlMainView";
            pnlMainView.Size = new Size(1190, 708);
            pnlMainView.TabIndex = 0;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabCreatePage);
            tabControlMain.Controls.Add(tabEditPage);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1190, 708);
            tabControlMain.TabIndex = 2;
            // 
            // tabCreatePage
            // 
            tabCreatePage.Controls.Add(tabControlCreate);
            tabCreatePage.Location = new Point(4, 24);
            tabCreatePage.Name = "tabCreatePage";
            tabCreatePage.Padding = new Padding(3);
            tabCreatePage.Size = new Size(1182, 680);
            tabCreatePage.TabIndex = 1;
            tabCreatePage.Text = "Criar";
            tabCreatePage.UseVisualStyleBackColor = true;
            // 
            // tabControlCreate
            // 
            tabControlCreate.Controls.Add(tabCreateElement);
            tabControlCreate.Controls.Add(tabCreateList);
            tabControlCreate.Controls.Add(tabCreateAlocation);
            tabControlCreate.Controls.Add(tabCreateCards);
            tabControlCreate.Controls.Add(tabCreateMoreCards);
            tabControlCreate.Dock = DockStyle.Fill;
            tabControlCreate.Location = new Point(3, 3);
            tabControlCreate.Name = "tabControlCreate";
            tabControlCreate.SelectedIndex = 0;
            tabControlCreate.Size = new Size(1176, 674);
            tabControlCreate.TabIndex = 0;
            // 
            // tabCreateElement
            // 
            tabCreateElement.Controls.Add(btnElementCreate);
            tabCreateElement.Controls.Add(lblElementMessage);
            tabCreateElement.Controls.Add(lblElementTitle);
            tabCreateElement.Controls.Add(cboElementList);
            tabCreateElement.Controls.Add(lblElementList);
            tabCreateElement.Controls.Add(boxElementNote2);
            tabCreateElement.Controls.Add(boxElementNote1);
            tabCreateElement.Controls.Add(boxElementCardName);
            tabCreateElement.Controls.Add(boxElementName);
            tabCreateElement.Controls.Add(lblElementNote2);
            tabCreateElement.Controls.Add(lblElementNote1);
            tabCreateElement.Controls.Add(lblElementCardName);
            tabCreateElement.Controls.Add(lblElementName);
            tabCreateElement.Location = new Point(4, 24);
            tabCreateElement.Name = "tabCreateElement";
            tabCreateElement.Padding = new Padding(3);
            tabCreateElement.Size = new Size(1168, 646);
            tabCreateElement.TabIndex = 0;
            tabCreateElement.Text = "Elementos";
            tabCreateElement.UseVisualStyleBackColor = true;
            // 
            // btnElementCreate
            // 
            btnElementCreate.Anchor = AnchorStyles.Top;
            btnElementCreate.Location = new Point(787, 485);
            btnElementCreate.Name = "btnElementCreate";
            btnElementCreate.Size = new Size(295, 76);
            btnElementCreate.TabIndex = 14;
            btnElementCreate.Text = "Criar";
            btnElementCreate.UseVisualStyleBackColor = true;
            btnElementCreate.Click += btnElementCreat_Clicked;
            // 
            // lblElementMessage
            // 
            lblElementMessage.Anchor = AnchorStyles.Top;
            lblElementMessage.Font = new Font("Segoe UI", 12F);
            lblElementMessage.Location = new Point(787, 196);
            lblElementMessage.Name = "lblElementMessage";
            lblElementMessage.Size = new Size(295, 216);
            lblElementMessage.TabIndex = 13;
            lblElementMessage.Text = "Mensagem";
            lblElementMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblElementTitle
            // 
            lblElementTitle.Dock = DockStyle.Top;
            lblElementTitle.Font = new Font("Segoe UI", 16F);
            lblElementTitle.Location = new Point(3, 3);
            lblElementTitle.Name = "lblElementTitle";
            lblElementTitle.Size = new Size(1162, 87);
            lblElementTitle.TabIndex = 12;
            lblElementTitle.Text = "Criar Elemento";
            lblElementTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboElementList
            // 
            cboElementList.Anchor = AnchorStyles.Top;
            cboElementList.FormattingEnabled = true;
            cboElementList.Location = new Point(275, 418);
            cboElementList.Name = "cboElementList";
            cboElementList.Size = new Size(395, 23);
            cboElementList.TabIndex = 5;
            // 
            // lblElementList
            // 
            lblElementList.Anchor = AnchorStyles.Top;
            lblElementList.Font = new Font("Segoe UI", 12F);
            lblElementList.Location = new Point(32, 407);
            lblElementList.Name = "lblElementList";
            lblElementList.Size = new Size(237, 38);
            lblElementList.TabIndex = 8;
            lblElementList.Text = "Lista:";
            lblElementList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // boxElementNote2
            // 
            boxElementNote2.Anchor = AnchorStyles.Top;
            boxElementNote2.Font = new Font("Segoe UI", 12F);
            boxElementNote2.Location = new Point(275, 348);
            boxElementNote2.Name = "boxElementNote2";
            boxElementNote2.Size = new Size(395, 29);
            boxElementNote2.TabIndex = 4;
            // 
            // boxElementNote1
            // 
            boxElementNote1.Anchor = AnchorStyles.Top;
            boxElementNote1.Font = new Font("Segoe UI", 12F);
            boxElementNote1.Location = new Point(275, 282);
            boxElementNote1.Name = "boxElementNote1";
            boxElementNote1.Size = new Size(395, 29);
            boxElementNote1.TabIndex = 3;
            // 
            // boxElementCardName
            // 
            boxElementCardName.Anchor = AnchorStyles.Top;
            boxElementCardName.Font = new Font("Segoe UI", 12F);
            boxElementCardName.Location = new Point(275, 219);
            boxElementCardName.Name = "boxElementCardName";
            boxElementCardName.Size = new Size(395, 29);
            boxElementCardName.TabIndex = 2;
            // 
            // boxElementName
            // 
            boxElementName.Anchor = AnchorStyles.Top;
            boxElementName.Font = new Font("Segoe UI", 12F);
            boxElementName.Location = new Point(275, 160);
            boxElementName.Name = "boxElementName";
            boxElementName.Size = new Size(395, 29);
            boxElementName.TabIndex = 1;
            // 
            // lblElementNote2
            // 
            lblElementNote2.Anchor = AnchorStyles.Top;
            lblElementNote2.Font = new Font("Segoe UI", 12F);
            lblElementNote2.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementNote2.Location = new Point(32, 342);
            lblElementNote2.Name = "lblElementNote2";
            lblElementNote2.Size = new Size(237, 38);
            lblElementNote2.TabIndex = 3;
            lblElementNote2.Text = "Anotação 2:";
            lblElementNote2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementNote1
            // 
            lblElementNote1.Anchor = AnchorStyles.Top;
            lblElementNote1.Font = new Font("Segoe UI", 12F);
            lblElementNote1.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementNote1.Location = new Point(32, 273);
            lblElementNote1.Name = "lblElementNote1";
            lblElementNote1.Size = new Size(237, 38);
            lblElementNote1.TabIndex = 2;
            lblElementNote1.Text = "Anotação 1:";
            lblElementNote1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementCardName
            // 
            lblElementCardName.Anchor = AnchorStyles.Top;
            lblElementCardName.Font = new Font("Segoe UI", 12F);
            lblElementCardName.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementCardName.Location = new Point(32, 210);
            lblElementCardName.Name = "lblElementCardName";
            lblElementCardName.Size = new Size(237, 38);
            lblElementCardName.TabIndex = 1;
            lblElementCardName.Text = "Nome para Cartela:";
            lblElementCardName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementName
            // 
            lblElementName.Anchor = AnchorStyles.Top;
            lblElementName.Font = new Font("Segoe UI", 12F);
            lblElementName.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementName.Location = new Point(32, 151);
            lblElementName.Name = "lblElementName";
            lblElementName.Size = new Size(237, 38);
            lblElementName.TabIndex = 0;
            lblElementName.Text = "Nome:";
            lblElementName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabCreateList
            // 
            tabCreateList.Controls.Add(btnListTxt);
            tabCreateList.Controls.Add(btnListImport);
            tabCreateList.Controls.Add(btnListCreate);
            tabCreateList.Controls.Add(lblListMessage);
            tabCreateList.Controls.Add(boxListDescription);
            tabCreateList.Controls.Add(boxListName);
            tabCreateList.Controls.Add(lblListDescription);
            tabCreateList.Controls.Add(lblListName);
            tabCreateList.Controls.Add(lblListTitle);
            tabCreateList.Location = new Point(4, 24);
            tabCreateList.Name = "tabCreateList";
            tabCreateList.Padding = new Padding(3);
            tabCreateList.Size = new Size(1168, 646);
            tabCreateList.TabIndex = 1;
            tabCreateList.Text = "Listas";
            tabCreateList.UseVisualStyleBackColor = true;
            // 
            // btnListTxt
            // 
            btnListTxt.Anchor = AnchorStyles.Top;
            btnListTxt.Location = new Point(390, 505);
            btnListTxt.Name = "btnListTxt";
            btnListTxt.Size = new Size(295, 76);
            btnListTxt.TabIndex = 24;
            btnListTxt.Text = "Importar Txt";
            btnListTxt.UseVisualStyleBackColor = true;
            btnListTxt.Click += btnListTxt_Clicked;
            // 
            // btnListImport
            // 
            btnListImport.Anchor = AnchorStyles.Top;
            btnListImport.Location = new Point(47, 505);
            btnListImport.Name = "btnListImport";
            btnListImport.Size = new Size(295, 76);
            btnListImport.TabIndex = 23;
            btnListImport.Text = "Importar Pasta de Imagens";
            btnListImport.UseVisualStyleBackColor = true;
            btnListImport.Click += btnListImport_Clicked;
            // 
            // btnListCreate
            // 
            btnListCreate.Anchor = AnchorStyles.Top;
            btnListCreate.Location = new Point(217, 385);
            btnListCreate.Name = "btnListCreate";
            btnListCreate.Size = new Size(295, 76);
            btnListCreate.TabIndex = 22;
            btnListCreate.Text = "Criar";
            btnListCreate.UseVisualStyleBackColor = true;
            btnListCreate.Click += btnListCreate_Clicked;
            // 
            // lblListMessage
            // 
            lblListMessage.Anchor = AnchorStyles.Top;
            lblListMessage.Font = new Font("Segoe UI", 12F);
            lblListMessage.Location = new Point(817, 172);
            lblListMessage.Name = "lblListMessage";
            lblListMessage.Size = new Size(295, 289);
            lblListMessage.TabIndex = 21;
            lblListMessage.Text = "Mensagem";
            lblListMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // boxListDescription
            // 
            boxListDescription.Anchor = AnchorStyles.Top;
            boxListDescription.Font = new Font("Segoe UI", 12F);
            boxListDescription.Location = new Point(290, 237);
            boxListDescription.Name = "boxListDescription";
            boxListDescription.Size = new Size(395, 29);
            boxListDescription.TabIndex = 17;
            // 
            // boxListName
            // 
            boxListName.Anchor = AnchorStyles.Top;
            boxListName.Font = new Font("Segoe UI", 12F);
            boxListName.Location = new Point(290, 181);
            boxListName.Name = "boxListName";
            boxListName.Size = new Size(395, 29);
            boxListName.TabIndex = 15;
            // 
            // lblListDescription
            // 
            lblListDescription.Anchor = AnchorStyles.Top;
            lblListDescription.Font = new Font("Segoe UI", 12F);
            lblListDescription.ImageAlign = ContentAlignment.MiddleLeft;
            lblListDescription.Location = new Point(47, 231);
            lblListDescription.Name = "lblListDescription";
            lblListDescription.Size = new Size(237, 38);
            lblListDescription.TabIndex = 16;
            lblListDescription.Text = "Descrição:";
            lblListDescription.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblListName
            // 
            lblListName.Anchor = AnchorStyles.Top;
            lblListName.Font = new Font("Segoe UI", 12F);
            lblListName.ImageAlign = ContentAlignment.MiddleLeft;
            lblListName.Location = new Point(47, 172);
            lblListName.Name = "lblListName";
            lblListName.Size = new Size(237, 38);
            lblListName.TabIndex = 14;
            lblListName.Text = "Nome:";
            lblListName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblListTitle
            // 
            lblListTitle.Dock = DockStyle.Top;
            lblListTitle.Font = new Font("Segoe UI", 16F);
            lblListTitle.Location = new Point(3, 3);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(1162, 87);
            lblListTitle.TabIndex = 13;
            lblListTitle.Text = "Criar Lista";
            lblListTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tabCreateAlocation
            // 
            tabCreateAlocation.Controls.Add(btnAlocateRemove);
            tabCreateAlocation.Controls.Add(btnAlocateAdd);
            tabCreateAlocation.Controls.Add(pnlAlocateElements);
            tabCreateAlocation.Controls.Add(pnlAlocateList);
            tabCreateAlocation.Controls.Add(lblAlocateTitle);
            tabCreateAlocation.Controls.Add(cboAlocateList);
            tabCreateAlocation.Location = new Point(4, 24);
            tabCreateAlocation.Name = "tabCreateAlocation";
            tabCreateAlocation.Size = new Size(1168, 646);
            tabCreateAlocation.TabIndex = 3;
            tabCreateAlocation.Text = "Alocar";
            tabCreateAlocation.UseVisualStyleBackColor = true;
            // 
            // btnAlocateRemove
            // 
            btnAlocateRemove.Anchor = AnchorStyles.Top;
            btnAlocateRemove.Location = new Point(737, 585);
            btnAlocateRemove.Name = "btnAlocateRemove";
            btnAlocateRemove.Size = new Size(428, 58);
            btnAlocateRemove.TabIndex = 18;
            btnAlocateRemove.Text = "Remover";
            btnAlocateRemove.UseVisualStyleBackColor = true;
            btnAlocateRemove.Click += btnAlocateRemove_Click;
            // 
            // btnAlocateAdd
            // 
            btnAlocateAdd.Anchor = AnchorStyles.Top;
            btnAlocateAdd.Location = new Point(307, 585);
            btnAlocateAdd.Name = "btnAlocateAdd";
            btnAlocateAdd.Size = new Size(428, 58);
            btnAlocateAdd.TabIndex = 17;
            btnAlocateAdd.Text = "Adicionar";
            btnAlocateAdd.UseVisualStyleBackColor = true;
            btnAlocateAdd.Click += btnAlocateAdd_Click;
            // 
            // pnlAlocateElements
            // 
            pnlAlocateElements.Anchor = AnchorStyles.Top;
            pnlAlocateElements.Controls.Add(flwAlocateElements);
            pnlAlocateElements.Location = new Point(307, 90);
            pnlAlocateElements.Name = "pnlAlocateElements";
            pnlAlocateElements.Size = new Size(858, 489);
            pnlAlocateElements.TabIndex = 16;
            // 
            // flwAlocateElements
            // 
            flwAlocateElements.AutoScroll = true;
            flwAlocateElements.AutoSize = true;
            flwAlocateElements.Dock = DockStyle.Fill;
            flwAlocateElements.Location = new Point(0, 0);
            flwAlocateElements.Name = "flwAlocateElements";
            flwAlocateElements.Size = new Size(858, 489);
            flwAlocateElements.TabIndex = 0;
            // 
            // pnlAlocateList
            // 
            pnlAlocateList.Anchor = AnchorStyles.Top;
            pnlAlocateList.Controls.Add(flwAlocateList);
            pnlAlocateList.Location = new Point(3, 119);
            pnlAlocateList.Name = "pnlAlocateList";
            pnlAlocateList.Size = new Size(298, 524);
            pnlAlocateList.TabIndex = 15;
            // 
            // flwAlocateList
            // 
            flwAlocateList.AutoScroll = true;
            flwAlocateList.AutoSize = true;
            flwAlocateList.Dock = DockStyle.Fill;
            flwAlocateList.Location = new Point(0, 0);
            flwAlocateList.Name = "flwAlocateList";
            flwAlocateList.Size = new Size(298, 524);
            flwAlocateList.TabIndex = 0;
            // 
            // lblAlocateTitle
            // 
            lblAlocateTitle.Dock = DockStyle.Top;
            lblAlocateTitle.Font = new Font("Segoe UI", 16F);
            lblAlocateTitle.Location = new Point(0, 0);
            lblAlocateTitle.Name = "lblAlocateTitle";
            lblAlocateTitle.Size = new Size(1168, 87);
            lblAlocateTitle.TabIndex = 14;
            lblAlocateTitle.Text = "Alocar Elementos";
            lblAlocateTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboAlocateList
            // 
            cboAlocateList.Anchor = AnchorStyles.Top;
            cboAlocateList.FormattingEnabled = true;
            cboAlocateList.Location = new Point(3, 90);
            cboAlocateList.Name = "cboAlocateList";
            cboAlocateList.Size = new Size(298, 23);
            cboAlocateList.TabIndex = 0;
            cboAlocateList.SelectedIndexChanged += cboAlocateList_SelectedIndexChanged;
            // 
            // tabCreateCards
            // 
            tabCreateCards.Controls.Add(lblCardsSize);
            tabCreateCards.Controls.Add(cboCardsSize);
            tabCreateCards.Controls.Add(boxCardsQuantity);
            tabCreateCards.Controls.Add(cboCardsHeader);
            tabCreateCards.Controls.Add(lblCardsHeader);
            tabCreateCards.Controls.Add(cboCardsModel);
            tabCreateCards.Controls.Add(lblCardsModel);
            tabCreateCards.Controls.Add(cboCardsTheme);
            tabCreateCards.Controls.Add(lblCardsTheme);
            tabCreateCards.Controls.Add(btnCardsExport);
            tabCreateCards.Controls.Add(lblCardsList);
            tabCreateCards.Controls.Add(cboCardsList);
            tabCreateCards.Controls.Add(lblCardsQuant);
            tabCreateCards.Controls.Add(lblCardsEnd);
            tabCreateCards.Controls.Add(boxCardsEnd);
            tabCreateCards.Controls.Add(lblCardsPage);
            tabCreateCards.Controls.Add(lblCardsMessage);
            tabCreateCards.Controls.Add(boxCardsTitle);
            tabCreateCards.Controls.Add(boxCardsName);
            tabCreateCards.Controls.Add(lblCardsTitle);
            tabCreateCards.Controls.Add(lblCardsName);
            tabCreateCards.Location = new Point(4, 24);
            tabCreateCards.Name = "tabCreateCards";
            tabCreateCards.Size = new Size(1168, 646);
            tabCreateCards.TabIndex = 2;
            tabCreateCards.Text = "Cartelas";
            tabCreateCards.UseVisualStyleBackColor = true;
            // 
            // lblCardsSize
            // 
            lblCardsSize.Anchor = AnchorStyles.Top;
            lblCardsSize.Font = new Font("Segoe UI", 12F);
            lblCardsSize.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsSize.Location = new Point(393, 537);
            lblCardsSize.Name = "lblCardsSize";
            lblCardsSize.Size = new Size(104, 38);
            lblCardsSize.TabIndex = 45;
            lblCardsSize.Text = "Tamanho:";
            lblCardsSize.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCardsSize
            // 
            cboCardsSize.Anchor = AnchorStyles.Top;
            cboCardsSize.FormattingEnabled = true;
            cboCardsSize.Location = new Point(503, 548);
            cboCardsSize.Name = "cboCardsSize";
            cboCardsSize.Size = new Size(202, 23);
            cboCardsSize.TabIndex = 44;
            cboCardsSize.SelectedIndexChanged += cboCardsSize_SelectedIndexChanged;
            // 
            // boxCardsQuantity
            // 
            boxCardsQuantity.Anchor = AnchorStyles.Top;
            boxCardsQuantity.Font = new Font("Segoe UI", 12F);
            boxCardsQuantity.Location = new Point(310, 339);
            boxCardsQuantity.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            boxCardsQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            boxCardsQuantity.Name = "boxCardsQuantity";
            boxCardsQuantity.Size = new Size(395, 29);
            boxCardsQuantity.TabIndex = 43;
            boxCardsQuantity.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // cboCardsHeader
            // 
            cboCardsHeader.Anchor = AnchorStyles.Top;
            cboCardsHeader.FormattingEnabled = true;
            cboCardsHeader.Location = new Point(912, 147);
            cboCardsHeader.Name = "cboCardsHeader";
            cboCardsHeader.Size = new Size(202, 23);
            cboCardsHeader.TabIndex = 42;
            // 
            // lblCardsHeader
            // 
            lblCardsHeader.Anchor = AnchorStyles.Top;
            lblCardsHeader.Font = new Font("Segoe UI", 12F);
            lblCardsHeader.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsHeader.Location = new Point(802, 136);
            lblCardsHeader.Name = "lblCardsHeader";
            lblCardsHeader.Size = new Size(104, 38);
            lblCardsHeader.TabIndex = 41;
            lblCardsHeader.Text = "Cabeçalho:";
            lblCardsHeader.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCardsModel
            // 
            cboCardsModel.Anchor = AnchorStyles.Top;
            cboCardsModel.FormattingEnabled = true;
            cboCardsModel.Location = new Point(503, 414);
            cboCardsModel.Name = "cboCardsModel";
            cboCardsModel.Size = new Size(202, 23);
            cboCardsModel.TabIndex = 40;
            // 
            // lblCardsModel
            // 
            lblCardsModel.Anchor = AnchorStyles.Top;
            lblCardsModel.Font = new Font("Segoe UI", 12F);
            lblCardsModel.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsModel.Location = new Point(393, 403);
            lblCardsModel.Name = "lblCardsModel";
            lblCardsModel.Size = new Size(104, 38);
            lblCardsModel.TabIndex = 39;
            lblCardsModel.Text = "Modelo:";
            lblCardsModel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCardsTheme
            // 
            cboCardsTheme.Anchor = AnchorStyles.Top;
            cboCardsTheme.FormattingEnabled = true;
            cboCardsTheme.Location = new Point(177, 414);
            cboCardsTheme.Name = "cboCardsTheme";
            cboCardsTheme.Size = new Size(202, 23);
            cboCardsTheme.TabIndex = 38;
            // 
            // lblCardsTheme
            // 
            lblCardsTheme.Anchor = AnchorStyles.Top;
            lblCardsTheme.Font = new Font("Segoe UI", 12F);
            lblCardsTheme.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsTheme.Location = new Point(67, 403);
            lblCardsTheme.Name = "lblCardsTheme";
            lblCardsTheme.Size = new Size(104, 38);
            lblCardsTheme.TabIndex = 37;
            lblCardsTheme.Text = "Cor:";
            lblCardsTheme.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCardsExport
            // 
            btnCardsExport.Anchor = AnchorStyles.Top;
            btnCardsExport.Font = new Font("Segoe UI", 12F);
            btnCardsExport.Location = new Point(819, 557);
            btnCardsExport.Name = "btnCardsExport";
            btnCardsExport.Size = new Size(295, 76);
            btnCardsExport.TabIndex = 35;
            btnCardsExport.Text = "Exportar Jogo";
            btnCardsExport.UseVisualStyleBackColor = true;
            btnCardsExport.Click += btnExportCards_Click;
            // 
            // lblCardsList
            // 
            lblCardsList.Anchor = AnchorStyles.Top;
            lblCardsList.Font = new Font("Segoe UI", 12F);
            lblCardsList.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsList.Location = new Point(67, 466);
            lblCardsList.Name = "lblCardsList";
            lblCardsList.Size = new Size(237, 38);
            lblCardsList.TabIndex = 33;
            lblCardsList.Text = "Lista:";
            lblCardsList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCardsList
            // 
            cboCardsList.Anchor = AnchorStyles.Top;
            cboCardsList.FormattingEnabled = true;
            cboCardsList.Location = new Point(310, 477);
            cboCardsList.Name = "cboCardsList";
            cboCardsList.Size = new Size(395, 23);
            cboCardsList.TabIndex = 32;
            // 
            // lblCardsQuant
            // 
            lblCardsQuant.Anchor = AnchorStyles.Top;
            lblCardsQuant.Font = new Font("Segoe UI", 12F);
            lblCardsQuant.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsQuant.Location = new Point(67, 339);
            lblCardsQuant.Name = "lblCardsQuant";
            lblCardsQuant.Size = new Size(237, 38);
            lblCardsQuant.TabIndex = 31;
            lblCardsQuant.Text = "Quantidade:";
            lblCardsQuant.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardsEnd
            // 
            lblCardsEnd.Anchor = AnchorStyles.Top;
            lblCardsEnd.Font = new Font("Segoe UI", 12F);
            lblCardsEnd.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsEnd.Location = new Point(67, 270);
            lblCardsEnd.Name = "lblCardsEnd";
            lblCardsEnd.Size = new Size(237, 38);
            lblCardsEnd.TabIndex = 29;
            lblCardsEnd.Text = "Mensagem Final:";
            lblCardsEnd.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // boxCardsEnd
            // 
            boxCardsEnd.Anchor = AnchorStyles.Top;
            boxCardsEnd.Font = new Font("Segoe UI", 12F);
            boxCardsEnd.Location = new Point(310, 276);
            boxCardsEnd.MaxLength = 300;
            boxCardsEnd.Name = "boxCardsEnd";
            boxCardsEnd.Size = new Size(395, 29);
            boxCardsEnd.TabIndex = 25;
            // 
            // lblCardsPage
            // 
            lblCardsPage.Dock = DockStyle.Top;
            lblCardsPage.Font = new Font("Segoe UI", 16F);
            lblCardsPage.Location = new Point(0, 0);
            lblCardsPage.Name = "lblCardsPage";
            lblCardsPage.Size = new Size(1168, 87);
            lblCardsPage.TabIndex = 27;
            lblCardsPage.Text = "Criar Cartelas";
            lblCardsPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCardsMessage
            // 
            lblCardsMessage.Anchor = AnchorStyles.Top;
            lblCardsMessage.Font = new Font("Segoe UI", 12F);
            lblCardsMessage.Location = new Point(819, 345);
            lblCardsMessage.Name = "lblCardsMessage";
            lblCardsMessage.Size = new Size(295, 145);
            lblCardsMessage.TabIndex = 26;
            lblCardsMessage.Text = "Mensagem";
            lblCardsMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // boxCardsTitle
            // 
            boxCardsTitle.Anchor = AnchorStyles.Top;
            boxCardsTitle.Font = new Font("Segoe UI", 12F);
            boxCardsTitle.Location = new Point(310, 207);
            boxCardsTitle.MaxLength = 300;
            boxCardsTitle.Name = "boxCardsTitle";
            boxCardsTitle.Size = new Size(395, 29);
            boxCardsTitle.TabIndex = 24;
            // 
            // boxCardsName
            // 
            boxCardsName.Anchor = AnchorStyles.Top;
            boxCardsName.Font = new Font("Segoe UI", 12F);
            boxCardsName.Location = new Point(310, 138);
            boxCardsName.MaxLength = 50;
            boxCardsName.Name = "boxCardsName";
            boxCardsName.Size = new Size(395, 29);
            boxCardsName.TabIndex = 23;
            // 
            // lblCardsTitle
            // 
            lblCardsTitle.Anchor = AnchorStyles.Top;
            lblCardsTitle.Font = new Font("Segoe UI", 12F);
            lblCardsTitle.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsTitle.Location = new Point(67, 201);
            lblCardsTitle.Name = "lblCardsTitle";
            lblCardsTitle.Size = new Size(237, 38);
            lblCardsTitle.TabIndex = 24;
            lblCardsTitle.Text = "Título para Cartela:";
            lblCardsTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardsName
            // 
            lblCardsName.Anchor = AnchorStyles.Top;
            lblCardsName.Font = new Font("Segoe UI", 12F);
            lblCardsName.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsName.Location = new Point(67, 132);
            lblCardsName.Name = "lblCardsName";
            lblCardsName.Size = new Size(237, 38);
            lblCardsName.TabIndex = 22;
            lblCardsName.Text = "Nome do Conjunto:";
            lblCardsName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabCreateMoreCards
            // 
            tabCreateMoreCards.Controls.Add(btnAddCards);
            tabCreateMoreCards.Controls.Add(boxAddQuant);
            tabCreateMoreCards.Controls.Add(lblAddQuant);
            tabCreateMoreCards.Controls.Add(lblAddSet);
            tabCreateMoreCards.Controls.Add(lblAddCards);
            tabCreateMoreCards.Controls.Add(cboAddCardsList);
            tabCreateMoreCards.Location = new Point(4, 24);
            tabCreateMoreCards.Name = "tabCreateMoreCards";
            tabCreateMoreCards.Size = new Size(1168, 646);
            tabCreateMoreCards.TabIndex = 4;
            tabCreateMoreCards.Text = "Cartelas Extras";
            tabCreateMoreCards.UseVisualStyleBackColor = true;
            // 
            // btnAddCards
            // 
            btnAddCards.Anchor = AnchorStyles.Top;
            btnAddCards.Font = new Font("Segoe UI", 12F);
            btnAddCards.Location = new Point(813, 98);
            btnAddCards.Name = "btnAddCards";
            btnAddCards.Size = new Size(265, 105);
            btnAddCards.TabIndex = 47;
            btnAddCards.Text = "Adicionar Cartelas";
            btnAddCards.UseVisualStyleBackColor = true;
            btnAddCards.Click += btnAddCards_Click;
            // 
            // boxAddQuant
            // 
            boxAddQuant.Anchor = AnchorStyles.Top;
            boxAddQuant.Font = new Font("Segoe UI", 12F);
            boxAddQuant.Location = new Point(334, 174);
            boxAddQuant.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            boxAddQuant.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            boxAddQuant.Name = "boxAddQuant";
            boxAddQuant.Size = new Size(395, 29);
            boxAddQuant.TabIndex = 46;
            boxAddQuant.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblAddQuant
            // 
            lblAddQuant.Anchor = AnchorStyles.Top;
            lblAddQuant.Font = new Font("Segoe UI", 12F);
            lblAddQuant.ImageAlign = ContentAlignment.MiddleLeft;
            lblAddQuant.Location = new Point(91, 165);
            lblAddQuant.Name = "lblAddQuant";
            lblAddQuant.Size = new Size(237, 38);
            lblAddQuant.TabIndex = 45;
            lblAddQuant.Text = "Quantidade:";
            lblAddQuant.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblAddSet
            // 
            lblAddSet.Anchor = AnchorStyles.Top;
            lblAddSet.Font = new Font("Segoe UI", 12F);
            lblAddSet.ImageAlign = ContentAlignment.MiddleLeft;
            lblAddSet.Location = new Point(91, 87);
            lblAddSet.Name = "lblAddSet";
            lblAddSet.Size = new Size(237, 38);
            lblAddSet.TabIndex = 44;
            lblAddSet.Text = "Nome do Conjunto:";
            lblAddSet.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblAddCards
            // 
            lblAddCards.Dock = DockStyle.Top;
            lblAddCards.Font = new Font("Segoe UI", 16F);
            lblAddCards.Location = new Point(0, 0);
            lblAddCards.Name = "lblAddCards";
            lblAddCards.Size = new Size(1168, 87);
            lblAddCards.TabIndex = 28;
            lblAddCards.Text = "Adicionar Cartelas em um Conjunto";
            lblAddCards.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboAddCardsList
            // 
            cboAddCardsList.Anchor = AnchorStyles.Top;
            cboAddCardsList.FormattingEnabled = true;
            cboAddCardsList.Location = new Point(334, 98);
            cboAddCardsList.Name = "cboAddCardsList";
            cboAddCardsList.Size = new Size(395, 23);
            cboAddCardsList.TabIndex = 0;
            // 
            // tabEditPage
            // 
            tabEditPage.Controls.Add(btnEditDeleteAll);
            tabEditPage.Controls.Add(cboEdit3);
            tabEditPage.Controls.Add(cboEdit2);
            tabEditPage.Controls.Add(boxEditText5);
            tabEditPage.Controls.Add(lblEditText5);
            tabEditPage.Controls.Add(btnEditExclude);
            tabEditPage.Controls.Add(btnEditEdit);
            tabEditPage.Controls.Add(lblEditMessage);
            tabEditPage.Controls.Add(cboEdit1);
            tabEditPage.Controls.Add(picEdit);
            tabEditPage.Controls.Add(boxEditText4);
            tabEditPage.Controls.Add(boxEditText3);
            tabEditPage.Controls.Add(boxEditText2);
            tabEditPage.Controls.Add(boxEditText1);
            tabEditPage.Controls.Add(lblEditImage);
            tabEditPage.Controls.Add(lblEditText4);
            tabEditPage.Controls.Add(lblEditText3);
            tabEditPage.Controls.Add(lblEditText2);
            tabEditPage.Controls.Add(lblEditText1);
            tabEditPage.Controls.Add(lblEditHeader);
            tabEditPage.Location = new Point(4, 24);
            tabEditPage.Name = "tabEditPage";
            tabEditPage.Size = new Size(1182, 680);
            tabEditPage.TabIndex = 2;
            tabEditPage.Text = "Editar";
            tabEditPage.UseVisualStyleBackColor = true;
            // 
            // btnEditDeleteAll
            // 
            btnEditDeleteAll.Anchor = AnchorStyles.Top;
            btnEditDeleteAll.Location = new Point(970, 539);
            btnEditDeleteAll.Name = "btnEditDeleteAll";
            btnEditDeleteAll.Size = new Size(167, 57);
            btnEditDeleteAll.TabIndex = 36;
            btnEditDeleteAll.Text = "Excluir Lista Completa";
            btnEditDeleteAll.UseVisualStyleBackColor = true;
            btnEditDeleteAll.Click += btnEditDeleteAll_Click;
            // 
            // cboEdit3
            // 
            cboEdit3.Anchor = AnchorStyles.Top;
            cboEdit3.FormattingEnabled = true;
            cboEdit3.Location = new Point(27, 168);
            cboEdit3.Name = "cboEdit3";
            cboEdit3.Size = new Size(391, 23);
            cboEdit3.TabIndex = 34;
            cboEdit3.SelectedIndexChanged += cboEdit3_SelectedIndexChanged;
            // 
            // cboEdit2
            // 
            cboEdit2.Anchor = AnchorStyles.Top;
            cboEdit2.FormattingEnabled = true;
            cboEdit2.Location = new Point(27, 139);
            cboEdit2.Name = "cboEdit2";
            cboEdit2.Size = new Size(391, 23);
            cboEdit2.TabIndex = 33;
            cboEdit2.SelectedIndexChanged += cboEdit2_SelectedIndexChanged;
            // 
            // boxEditText5
            // 
            boxEditText5.Anchor = AnchorStyles.Top;
            boxEditText5.Font = new Font("Segoe UI", 12F);
            boxEditText5.Location = new Point(742, 344);
            boxEditText5.Name = "boxEditText5";
            boxEditText5.Size = new Size(395, 29);
            boxEditText5.TabIndex = 32;
            // 
            // lblEditText5
            // 
            lblEditText5.Anchor = AnchorStyles.Top;
            lblEditText5.Font = new Font("Segoe UI", 12F);
            lblEditText5.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText5.Location = new Point(499, 338);
            lblEditText5.Name = "lblEditText5";
            lblEditText5.Size = new Size(237, 38);
            lblEditText5.TabIndex = 31;
            lblEditText5.Text = "Informação 5:";
            lblEditText5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnEditExclude
            // 
            btnEditExclude.Anchor = AnchorStyles.Top;
            btnEditExclude.Location = new Point(970, 476);
            btnEditExclude.Name = "btnEditExclude";
            btnEditExclude.Size = new Size(167, 57);
            btnEditExclude.TabIndex = 30;
            btnEditExclude.Text = "Excluir";
            btnEditExclude.UseVisualStyleBackColor = true;
            btnEditExclude.Click += btnEditExclude_Click;
            // 
            // btnEditEdit
            // 
            btnEditEdit.Anchor = AnchorStyles.Top;
            btnEditEdit.Location = new Point(970, 405);
            btnEditEdit.Name = "btnEditEdit";
            btnEditEdit.Size = new Size(167, 65);
            btnEditEdit.TabIndex = 28;
            btnEditEdit.Text = "Editar";
            btnEditEdit.UseVisualStyleBackColor = true;
            btnEditEdit.Click += btnEditEdit_Click;
            // 
            // lblEditMessage
            // 
            lblEditMessage.Anchor = AnchorStyles.Top;
            lblEditMessage.Font = new Font("Segoe UI", 12F);
            lblEditMessage.Location = new Point(27, 486);
            lblEditMessage.Name = "lblEditMessage";
            lblEditMessage.Size = new Size(391, 110);
            lblEditMessage.TabIndex = 27;
            lblEditMessage.Text = "Mensagem";
            lblEditMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboEdit1
            // 
            cboEdit1.Anchor = AnchorStyles.Top;
            cboEdit1.FormattingEnabled = true;
            cboEdit1.Location = new Point(27, 110);
            cboEdit1.Name = "cboEdit1";
            cboEdit1.Size = new Size(391, 23);
            cboEdit1.TabIndex = 24;
            cboEdit1.SelectedIndexChanged += cboEdit1_SelectedIndexChanged;
            // 
            // picEdit
            // 
            picEdit.Anchor = AnchorStyles.Top;
            picEdit.Location = new Point(742, 405);
            picEdit.Name = "picEdit";
            picEdit.Size = new Size(208, 191);
            picEdit.TabIndex = 26;
            picEdit.TabStop = false;
            // 
            // boxEditText4
            // 
            boxEditText4.Anchor = AnchorStyles.Top;
            boxEditText4.Font = new Font("Segoe UI", 12F);
            boxEditText4.Location = new Point(742, 287);
            boxEditText4.Name = "boxEditText4";
            boxEditText4.Size = new Size(395, 29);
            boxEditText4.TabIndex = 22;
            // 
            // boxEditText3
            // 
            boxEditText3.Anchor = AnchorStyles.Top;
            boxEditText3.Font = new Font("Segoe UI", 12F);
            boxEditText3.Location = new Point(742, 230);
            boxEditText3.Name = "boxEditText3";
            boxEditText3.Size = new Size(395, 29);
            boxEditText3.TabIndex = 20;
            // 
            // boxEditText2
            // 
            boxEditText2.Anchor = AnchorStyles.Top;
            boxEditText2.Font = new Font("Segoe UI", 12F);
            boxEditText2.Location = new Point(742, 173);
            boxEditText2.Name = "boxEditText2";
            boxEditText2.Size = new Size(395, 29);
            boxEditText2.TabIndex = 18;
            // 
            // boxEditText1
            // 
            boxEditText1.Anchor = AnchorStyles.Top;
            boxEditText1.Font = new Font("Segoe UI", 12F);
            boxEditText1.Location = new Point(742, 114);
            boxEditText1.Name = "boxEditText1";
            boxEditText1.Size = new Size(395, 29);
            boxEditText1.TabIndex = 16;
            // 
            // lblEditImage
            // 
            lblEditImage.Anchor = AnchorStyles.Top;
            lblEditImage.Font = new Font("Segoe UI", 12F);
            lblEditImage.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditImage.Location = new Point(499, 429);
            lblEditImage.Name = "lblEditImage";
            lblEditImage.Size = new Size(237, 38);
            lblEditImage.TabIndex = 23;
            lblEditImage.Text = "Imagem:";
            lblEditImage.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText4
            // 
            lblEditText4.Anchor = AnchorStyles.Top;
            lblEditText4.Font = new Font("Segoe UI", 12F);
            lblEditText4.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText4.Location = new Point(499, 281);
            lblEditText4.Name = "lblEditText4";
            lblEditText4.Size = new Size(237, 38);
            lblEditText4.TabIndex = 21;
            lblEditText4.Text = "Informação 4:";
            lblEditText4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText3
            // 
            lblEditText3.Anchor = AnchorStyles.Top;
            lblEditText3.Font = new Font("Segoe UI", 12F);
            lblEditText3.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText3.Location = new Point(499, 224);
            lblEditText3.Name = "lblEditText3";
            lblEditText3.Size = new Size(237, 38);
            lblEditText3.TabIndex = 19;
            lblEditText3.Text = "Informação 3:";
            lblEditText3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText2
            // 
            lblEditText2.Anchor = AnchorStyles.Top;
            lblEditText2.Font = new Font("Segoe UI", 12F);
            lblEditText2.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText2.Location = new Point(499, 167);
            lblEditText2.Name = "lblEditText2";
            lblEditText2.Size = new Size(237, 38);
            lblEditText2.TabIndex = 17;
            lblEditText2.Text = "Informação 2:";
            lblEditText2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText1
            // 
            lblEditText1.Anchor = AnchorStyles.Top;
            lblEditText1.Font = new Font("Segoe UI", 12F);
            lblEditText1.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText1.Location = new Point(499, 110);
            lblEditText1.Name = "lblEditText1";
            lblEditText1.Size = new Size(237, 38);
            lblEditText1.TabIndex = 15;
            lblEditText1.Text = "Informação 1:";
            lblEditText1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditHeader
            // 
            lblEditHeader.Dock = DockStyle.Top;
            lblEditHeader.Font = new Font("Segoe UI", 16F);
            lblEditHeader.Location = new Point(0, 0);
            lblEditHeader.Name = "lblEditHeader";
            lblEditHeader.Size = new Size(1182, 87);
            lblEditHeader.TabIndex = 13;
            lblEditHeader.Text = "Editar";
            lblEditHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1190, 708);
            Controls.Add(pnlMainView);
            Name = "MainView";
            Text = "BingoCreator";
            WindowState = FormWindowState.Maximized;
            pnlMainView.ResumeLayout(false);
            tabControlMain.ResumeLayout(false);
            tabCreatePage.ResumeLayout(false);
            tabControlCreate.ResumeLayout(false);
            tabCreateElement.ResumeLayout(false);
            tabCreateElement.PerformLayout();
            tabCreateList.ResumeLayout(false);
            tabCreateList.PerformLayout();
            tabCreateAlocation.ResumeLayout(false);
            pnlAlocateElements.ResumeLayout(false);
            pnlAlocateElements.PerformLayout();
            pnlAlocateList.ResumeLayout(false);
            pnlAlocateList.PerformLayout();
            tabCreateCards.ResumeLayout(false);
            tabCreateCards.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)boxCardsQuantity).EndInit();
            tabCreateMoreCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)boxAddQuant).EndInit();
            tabEditPage.ResumeLayout(false);
            tabEditPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picEdit).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlMainView;
        private TabControl tabControlMain;
        private TabPage tabCreatePage;
        private TabPage tabEditPage;
        private TabControl tabControlCreate;
        private TabPage tabCreateElement;
        private TabPage tabCreateList;
        private TabPage tabCreateCards;
        private TextBox boxElementName;
        private Label lblElementNote2;
        private Label lblElementNote1;
        private Label lblElementCardName;
        private Label lblElementName;
        private Label lblElementList;
        private TextBox boxElementNote2;
        private TextBox boxElementNote1;
        private TextBox boxElementCardName;
        private Label lblElementTitle;
        private ComboBox cboElementList;
        private Label lblElementMessage;
        private Label lblListMessage;
        private TextBox boxListDescription;
        private TextBox boxListName;
        private Label lblListDescription;
        private Label lblListName;
        private Label lblListTitle;
        private Label lblCardsPage;
        private Label lblCardsMessage;
        private TextBox boxCardsTitle;
        private TextBox boxCardsName;
        private Label lblCardsTitle;
        private Label lblCardsName;
        private TextBox boxCardsEnd;
        private Label lblCardsList;
        private ComboBox cboCardsList;
        private Label lblCardsQuant;
        private Label lblCardsEnd;
        private Button btnElementCreate;
        private Button btnListCreate;
        private Button btnCardsExport;
        private Button btnEditEdit;
        private Label lblEditMessage;
        private ComboBox cboEdit1;
        private PictureBox picEdit;
        private TextBox boxEditText4;
        private TextBox boxEditText3;
        private TextBox boxEditText2;
        private TextBox boxEditText1;
        private Label lblEditImage;
        private Label lblEditText4;
        private Label lblEditText3;
        private Label lblEditText2;
        private Label lblEditText1;
        private Label lblEditHeader;
        private Button btnEditExclude;
        private TextBox boxEditText5;
        private Label lblEditText5;
        private Button btnListImport;
        private Button btnListTxt;
        private ComboBox cboCardsTheme;
        private Label lblCardsTheme;
        private ComboBox cboCardsModel;
        private Label lblCardsModel;
        private ComboBox cboCardsHeader;
        private Label lblCardsHeader;
        private NumericUpDown boxCardsQuantity;
        private ComboBox cboEdit2;
        private ComboBox cboEdit3;
        private Label lblCardsSize;
        private ComboBox cboCardsSize;
        private Button btnEditDeleteAll;
        private TabPage tabCreateAlocation;
        private Panel pnlAlocateElements;
        private FlowLayoutPanel flwAlocateElements;
        private Panel pnlAlocateList;
        private FlowLayoutPanel flwAlocateList;
        private Label lblAlocateTitle;
        private ComboBox cboAlocateList;
        private Button btnAlocateRemove;
        private Button btnAlocateAdd;
        private TabPage tabCreateMoreCards;
        private Button btnAddCards;
        private NumericUpDown boxAddQuant;
        private Label lblAddQuant;
        private Label lblAddSet;
        private Label lblAddCards;
        private ComboBox cboAddCardsList;
    }
}
