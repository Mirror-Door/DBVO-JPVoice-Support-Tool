namespace DBVO_JPVoice_Tool
{
    partial class FormMain
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            listBoxLog = new FlickerFreeListBox();
            label2 = new Label();
            buttonMakeWav = new Button();
            buttonMakeFuz = new Button();
            progressBar1 = new ProgressBar();
            labelProgress = new Label();
            groupBox2 = new GroupBox();
            labelYakitori = new Label();
            buttonYakitoriOpen = new Button();
            textBoxYakitoriPath = new TextBox();
            toolTip = new ToolTip(components);
            buttonFaceFXOpen = new Button();
            buttonBatch = new Button();
            buttonMakePack = new Button();
            buttonMakeLip = new Button();
            buttonBatchFolder = new Button();
            checkBoxBatch = new CheckBox();
            buttonSample = new Button();
            buttonGetChar = new Button();
            buttonVoiceControl = new Button();
            buttonProcCancel = new Button();
            buttonBatchAll = new Button();
            checkBoxOnlyMoji = new CheckBox();
            groupBox3 = new GroupBox();
            labelFaceFX = new Label();
            textBoxFaceFXPath = new TextBox();
            groupBox4 = new GroupBox();
            label5 = new Label();
            textBoxBatch = new TextBox();
            comboBoxVoiceSoft = new ComboBox();
            comboBoxPreset = new ComboBox();
            textBoxParam = new TextBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            buttonLogSave = new Button();
            menuStrip1 = new MenuStrip();
            ToolStripMenuTool = new ToolStripMenuItem();
            toolStripMenuDelXmlSpace = new ToolStripMenuItem();
            ToolStripMenuXmltoJson = new ToolStripMenuItem();
            ToolStripMenuMakeWavFromText = new ToolStripMenuItem();
            ToolStripMenuOption = new ToolStripMenuItem();
            ToolStripMenuItemIsShowMsg = new ToolStripMenuItem();
            ToolStripMenuUseBatchOutput = new ToolStripMenuItem();
            ToolStripMenuReadDctionary = new ToolStripMenuItem();
            toolStripMenuReadDicDef = new ToolStripMenuItem();
            ToolStripMenuReadDicTxt = new ToolStripMenuItem();
            ToolStripMenuOpenDictionary = new ToolStripMenuItem();
            ToolStripMenuOpenDictDef = new ToolStripMenuItem();
            ToolStripMenuOpenDictOther = new ToolStripMenuItem();
            textBoxSearch = new TextBox();
            buttonLogClear = new Button();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxLog
            // 
            listBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxLog.BackColor = Color.DimGray;
            listBoxLog.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxLog.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            listBoxLog.ForeColor = Color.Yellow;
            listBoxLog.FormattingEnabled = true;
            listBoxLog.HorizontalScrollbar = true;
            listBoxLog.ItemHeight = 15;
            listBoxLog.Location = new Point(392, 48);
            listBoxLog.Name = "listBoxLog";
            listBoxLog.Size = new Size(745, 484);
            listBoxLog.TabIndex = 1;
            listBoxLog.Leave += listBoxLog_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(392, 28);
            label2.Name = "label2";
            label2.Size = new Size(81, 17);
            label2.TabIndex = 5;
            label2.Text = "情報＆ログ：";
            // 
            // buttonMakeWav
            // 
            buttonMakeWav.Enabled = false;
            buttonMakeWav.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonMakeWav.Image = Properties.Resources.TextFile;
            buttonMakeWav.Location = new Point(223, 149);
            buttonMakeWav.Name = "buttonMakeWav";
            buttonMakeWav.Size = new Size(163, 23);
            buttonMakeWav.TabIndex = 12;
            buttonMakeWav.Text = "辞書(JSON)→音声(WAV)";
            buttonMakeWav.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonMakeWav, "機能：\r\n辞書ファイル(JSON)から日本語の文章を抜き出し、音声合成ソフトと連携して音声ファイルを作成します\r\n(ファイル名は辞書ファイルの英語の文章となります)\r\n※事前に音声合成ソフトのインストールと起動している必要があります");
            buttonMakeWav.UseVisualStyleBackColor = true;
            buttonMakeWav.Click += ButtonMakeWav_Click;
            // 
            // buttonMakeFuz
            // 
            buttonMakeFuz.Enabled = false;
            buttonMakeFuz.ForeColor = SystemColors.ControlText;
            buttonMakeFuz.Image = Properties.Resources.TextFile;
            buttonMakeFuz.Location = new Point(204, 53);
            buttonMakeFuz.Name = "buttonMakeFuz";
            buttonMakeFuz.Size = new Size(160, 23);
            buttonMakeFuz.TabIndex = 14;
            buttonMakeFuz.Text = "WAV→FUZファイルへ変換";
            buttonMakeFuz.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonMakeFuz, resources.GetString("buttonMakeFuz.ToolTip"));
            buttonMakeFuz.UseVisualStyleBackColor = true;
            buttonMakeFuz.Click += ButtonMakeFuz_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            progressBar1.Location = new Point(8, 536);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(332, 23);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 16;
            progressBar1.Visible = false;
            // 
            // labelProgress
            // 
            labelProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelProgress.AutoSize = true;
            labelProgress.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelProgress.ForeColor = SystemColors.ControlLightLight;
            labelProgress.Location = new Point(12, 516);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(93, 15);
            labelProgress.TabIndex = 17;
            labelProgress.Text = "progressbar info";
            labelProgress.Visible = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(labelYakitori);
            groupBox2.Controls.Add(buttonYakitoriOpen);
            groupBox2.Controls.Add(textBoxYakitoriPath);
            groupBox2.Controls.Add(buttonMakeFuz);
            groupBox2.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox2.ForeColor = SystemColors.ControlLightLight;
            groupBox2.Location = new Point(2, 110);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(370, 86);
            groupBox2.TabIndex = 18;
            groupBox2.TabStop = false;
            groupBox2.Text = "Yakitori Audio Converter";
            // 
            // labelYakitori
            // 
            labelYakitori.AutoSize = true;
            labelYakitori.BackColor = Color.Red;
            labelYakitori.Font = new Font("Yu Gothic UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            labelYakitori.ForeColor = SystemColors.ButtonFace;
            labelYakitori.Location = new Point(6, 52);
            labelYakitori.Name = "labelYakitori";
            labelYakitori.Size = new Size(27, 17);
            labelYakitori.TabIndex = 23;
            labelYakitori.Text = "NG";
            labelYakitori.TextChanged += LabelYakitori_TextChanged;
            // 
            // buttonYakitoriOpen
            // 
            buttonYakitoriOpen.Image = Properties.Resources.OpenFolder;
            buttonYakitoriOpen.Location = new Point(320, 22);
            buttonYakitoriOpen.Name = "buttonYakitoriOpen";
            buttonYakitoriOpen.Size = new Size(44, 23);
            buttonYakitoriOpen.TabIndex = 16;
            toolTip.SetToolTip(buttonYakitoriOpen, "「Yakitori Audio Converter」がインストールされているフォルダを指定してください。\r\nそのフォルダ内に次の2つのexeがあることを確認してください。\r\nxWMAEncode.exe\r\nBmlFuzEncode.exe");
            buttonYakitoriOpen.UseVisualStyleBackColor = true;
            buttonYakitoriOpen.Click += ButtonYakitoriOpen_Click;
            // 
            // textBoxYakitoriPath
            // 
            textBoxYakitoriPath.BackColor = SystemColors.ControlLight;
            textBoxYakitoriPath.Location = new Point(6, 24);
            textBoxYakitoriPath.Name = "textBoxYakitoriPath";
            textBoxYakitoriPath.ReadOnly = true;
            textBoxYakitoriPath.Size = new Size(308, 23);
            textBoxYakitoriPath.TabIndex = 15;
            textBoxYakitoriPath.TextChanged += TextBoxYakitoriPath_TextChanged;
            // 
            // toolTip
            // 
            toolTip.BackColor = SystemColors.ControlDark;
            toolTip.ForeColor = SystemColors.HighlightText;
            toolTip.IsBalloon = true;
            // 
            // buttonFaceFXOpen
            // 
            buttonFaceFXOpen.Image = Properties.Resources.OpenFolder;
            buttonFaceFXOpen.Location = new Point(319, 23);
            buttonFaceFXOpen.Name = "buttonFaceFXOpen";
            buttonFaceFXOpen.Size = new Size(44, 23);
            buttonFaceFXOpen.TabIndex = 21;
            toolTip.SetToolTip(buttonFaceFXOpen, "「FaceFXWrapper」がインストールされているフォルダを指定してください。\r\nそのフォルダ内に次の2つのファイルがあることを確認してください。\r\n・FaceFXWrapper.exe\r\n・FonixData.cdf");
            buttonFaceFXOpen.UseVisualStyleBackColor = true;
            buttonFaceFXOpen.Click += ButtonFaceFXOpen_Click;
            // 
            // buttonBatch
            // 
            buttonBatch.BackColor = Color.LightSteelBlue;
            buttonBatch.Enabled = false;
            buttonBatch.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonBatch.Image = Properties.Resources.TextFile;
            buttonBatch.Location = new Point(9, 270);
            buttonBatch.Name = "buttonBatch";
            buttonBatch.Size = new Size(163, 23);
            buttonBatch.TabIndex = 21;
            buttonBatch.Text = "LIP/FUZ 一括処理";
            buttonBatch.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonBatch, "機能：\r\nLIPファイル作成→FUZファイル変換(下記の2つのボタンの機能)を一括で行います\r\n「WAV→LIPファイルを作成」\r\n「WAV→FUZファイルへ変換」\r\nLIPファイルはWAVフォルダと同じフォルダ、FUZファイルは出力先フォルダに出力されます。\r\n\r\nファイルの使用用途：\r\n生成されたFUZをボイスパックに含めるとダイアログ選択時に口パクありで音声が出るようになります。");
            buttonBatch.UseVisualStyleBackColor = false;
            buttonBatch.Click += ButtonBatch_Click;
            // 
            // buttonMakePack
            // 
            buttonMakePack.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonMakePack.Image = Properties.Resources.FolderSuppressed;
            buttonMakePack.Location = new Point(8, 483);
            buttonMakePack.Name = "buttonMakePack";
            buttonMakePack.Size = new Size(158, 23);
            buttonMakePack.TabIndex = 22;
            buttonMakePack.Text = "ボイスパック作成";
            buttonMakePack.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonMakePack, "機能：\r\nボイスパック作成用のダイアログが開きます。\r\nこのダイアログでDBVOで動作するようにフォルダを構成しZIPファイルとして出力することができます\r\n\r\nファイルの使用用途：\r\n出力されたZIPファイルをMODとしてインストールすればボイスパックとして機能します");
            buttonMakePack.UseVisualStyleBackColor = true;
            buttonMakePack.Click += ButtonMakePack_Click;
            // 
            // buttonMakeLip
            // 
            buttonMakeLip.Enabled = false;
            buttonMakeLip.ForeColor = SystemColors.ControlText;
            buttonMakeLip.Image = Properties.Resources.TextFile;
            buttonMakeLip.Location = new Point(200, 52);
            buttonMakeLip.Name = "buttonMakeLip";
            buttonMakeLip.Size = new Size(163, 23);
            buttonMakeLip.TabIndex = 19;
            buttonMakeLip.Text = "WAV→LIPファイルを作成";
            buttonMakeLip.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonMakeLip, resources.GetString("buttonMakeLip.ToolTip"));
            buttonMakeLip.UseVisualStyleBackColor = true;
            buttonMakeLip.Click += ButtonMakeLip_Click;
            // 
            // buttonBatchFolder
            // 
            buttonBatchFolder.Image = Properties.Resources.OpenFolder;
            buttonBatchFolder.Location = new Point(322, 217);
            buttonBatchFolder.Name = "buttonBatchFolder";
            buttonBatchFolder.Size = new Size(44, 23);
            buttonBatchFolder.TabIndex = 25;
            toolTip.SetToolTip(buttonBatchFolder, "LIP/FUZ一括処理の出力先を選択してください");
            buttonBatchFolder.UseVisualStyleBackColor = true;
            buttonBatchFolder.Click += ButtonBatchFolder_Click;
            // 
            // checkBoxBatch
            // 
            checkBoxBatch.AutoSize = true;
            checkBoxBatch.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            checkBoxBatch.ForeColor = SystemColors.ControlLightLight;
            checkBoxBatch.Location = new Point(9, 245);
            checkBoxBatch.Name = "checkBoxBatch";
            checkBoxBatch.Size = new Size(204, 19);
            checkBoxBatch.TabIndex = 32;
            checkBoxBatch.Text = "処理の後WAV/LIPを削除する(危険)";
            toolTip.SetToolTip(checkBoxBatch, "一括処理が終了したあとWAVファイルとLIPファイルを削除します\r\nWAVファイルの格納先フォルダの全ファイルが対象となります\r\n\r\n※意図しないファイルが削除されないよう、よくわからない場合はこの機能を有効にしないでください");
            checkBoxBatch.UseVisualStyleBackColor = true;
            // 
            // buttonSample
            // 
            buttonSample.Enabled = false;
            buttonSample.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonSample.Image = Properties.Resources.AudioPlayback;
            buttonSample.Location = new Point(317, 79);
            buttonSample.Name = "buttonSample";
            buttonSample.Size = new Size(69, 23);
            buttonSample.TabIndex = 30;
            buttonSample.Text = "サンプル";
            buttonSample.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonSample, "選択されたキャラクターのサンプルボイスを再生します\r\n\r\n※音声が再生されるので注意してください");
            buttonSample.UseVisualStyleBackColor = true;
            buttonSample.Click += ButtonSample_Click;
            // 
            // buttonGetChar
            // 
            buttonGetChar.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonGetChar.Location = new Point(317, 47);
            buttonGetChar.Name = "buttonGetChar";
            buttonGetChar.Size = new Size(69, 23);
            buttonGetChar.TabIndex = 31;
            buttonGetChar.Text = "キャラ取得";
            toolTip.SetToolTip(buttonGetChar, "選択された音声合成ソフトのキャラクターの一覧を取得します\r\n事前に音声合成ソフトを起動していないと取得に失敗します");
            buttonGetChar.UseVisualStyleBackColor = true;
            buttonGetChar.Click += ButtonGetCharList_Click;
            // 
            // buttonVoiceControl
            // 
            buttonVoiceControl.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonVoiceControl.Location = new Point(98, 111);
            buttonVoiceControl.Name = "buttonVoiceControl";
            buttonVoiceControl.Size = new Size(54, 23);
            buttonVoiceControl.TabIndex = 32;
            buttonVoiceControl.Text = "調整";
            toolTip.SetToolTip(buttonVoiceControl, "ボイス調整用のダイアログを表示します");
            buttonVoiceControl.UseVisualStyleBackColor = true;
            buttonVoiceControl.Click += ButtonVoiceControl_Click;
            // 
            // buttonProcCancel
            // 
            buttonProcCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonProcCancel.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonProcCancel.Image = Properties.Resources.Cancel;
            buttonProcCancel.Location = new Point(346, 536);
            buttonProcCancel.Name = "buttonProcCancel";
            buttonProcCancel.Size = new Size(80, 23);
            buttonProcCancel.TabIndex = 33;
            buttonProcCancel.Text = "処理中止";
            buttonProcCancel.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonProcCancel, "作業中の処理をキャンセルしたい場合に押してください\r\n中断されるまでの処理で出来たファイルは出力されます");
            buttonProcCancel.UseVisualStyleBackColor = true;
            buttonProcCancel.Click += ButtonProcCancel_Click;
            // 
            // buttonBatchAll
            // 
            buttonBatchAll.BackColor = Color.SandyBrown;
            buttonBatchAll.Enabled = false;
            buttonBatchAll.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonBatchAll.Image = Properties.Resources.TextFile;
            buttonBatchAll.ImeMode = ImeMode.NoControl;
            buttonBatchAll.Location = new Point(193, 270);
            buttonBatchAll.Name = "buttonBatchAll";
            buttonBatchAll.Size = new Size(173, 23);
            buttonBatchAll.TabIndex = 33;
            buttonBatchAll.Text = "WAV/LIP/FUZ 一括処理";
            buttonBatchAll.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonBatchAll, "機能：\r\nLIPファイル作成→FUZファイル変換(下記の2つのボタンの機能)を一括で行います\r\n「WAV→LIPファイルを作成」\r\n「WAV→FUZファイルへ変換」\r\nLIPファイルはWAVフォルダと同じフォルダ、FUZファイルは出力先フォルダに出力されます。\r\n\r\nファイルの使用用途：\r\n生成されたFUZをボイスパックに含めるとダイアログ選択時に口パクありで音声が出るようになります。");
            buttonBatchAll.UseVisualStyleBackColor = false;
            buttonBatchAll.Click += ButtonBatchAll_Click;
            // 
            // checkBoxOnlyMoji
            // 
            checkBoxOnlyMoji.AutoSize = true;
            checkBoxOnlyMoji.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            checkBoxOnlyMoji.ForeColor = SystemColors.ControlLightLight;
            checkBoxOnlyMoji.ImeMode = ImeMode.NoControl;
            checkBoxOnlyMoji.Location = new Point(272, 117);
            checkBoxOnlyMoji.Name = "checkBoxOnlyMoji";
            checkBoxOnlyMoji.RightToLeft = RightToLeft.No;
            checkBoxOnlyMoji.Size = new Size(114, 19);
            checkBoxOnlyMoji.TabIndex = 33;
            checkBoxOnlyMoji.Text = "文字列を指定する";
            toolTip.SetToolTip(checkBoxOnlyMoji, resources.GetString("checkBoxOnlyMoji.ToolTip"));
            checkBoxOnlyMoji.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(labelFaceFX);
            groupBox3.Controls.Add(buttonFaceFXOpen);
            groupBox3.Controls.Add(textBoxFaceFXPath);
            groupBox3.Controls.Add(buttonMakeLip);
            groupBox3.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox3.ForeColor = SystemColors.ControlLightLight;
            groupBox3.Location = new Point(3, 10);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(369, 85);
            groupBox3.TabIndex = 20;
            groupBox3.TabStop = false;
            groupBox3.Text = "FaceFXWrapper";
            // 
            // labelFaceFX
            // 
            labelFaceFX.AutoSize = true;
            labelFaceFX.BackColor = Color.Red;
            labelFaceFX.Font = new Font("Yu Gothic UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            labelFaceFX.ForeColor = SystemColors.ButtonFace;
            labelFaceFX.ImageAlign = ContentAlignment.MiddleRight;
            labelFaceFX.Location = new Point(6, 51);
            labelFaceFX.Name = "labelFaceFX";
            labelFaceFX.Size = new Size(27, 17);
            labelFaceFX.TabIndex = 22;
            labelFaceFX.Text = "NG";
            labelFaceFX.TextChanged += LabelFaceFX_TextChanged;
            // 
            // textBoxFaceFXPath
            // 
            textBoxFaceFXPath.BackColor = SystemColors.ControlLight;
            textBoxFaceFXPath.Location = new Point(6, 23);
            textBoxFaceFXPath.Name = "textBoxFaceFXPath";
            textBoxFaceFXPath.ReadOnly = true;
            textBoxFaceFXPath.Size = new Size(307, 23);
            textBoxFaceFXPath.TabIndex = 20;
            textBoxFaceFXPath.TextChanged += TextBoxFaceFXPath_TextChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(buttonBatchAll);
            groupBox4.Controls.Add(checkBoxBatch);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(buttonBatchFolder);
            groupBox4.Controls.Add(groupBox2);
            groupBox4.Controls.Add(textBoxBatch);
            groupBox4.Controls.Add(groupBox3);
            groupBox4.Controls.Add(buttonBatch);
            groupBox4.Location = new Point(8, 173);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(378, 303);
            groupBox4.TabIndex = 23;
            groupBox4.TabStop = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = SystemColors.ControlLightLight;
            label5.Location = new Point(6, 200);
            label5.Name = "label5";
            label5.Size = new Size(136, 15);
            label5.TabIndex = 32;
            label5.Text = "一括処理のファイル出力先";
            // 
            // textBoxBatch
            // 
            textBoxBatch.BackColor = SystemColors.ControlLight;
            textBoxBatch.Location = new Point(9, 218);
            textBoxBatch.Name = "textBoxBatch";
            textBoxBatch.ReadOnly = true;
            textBoxBatch.Size = new Size(307, 23);
            textBoxBatch.TabIndex = 24;
            textBoxBatch.TextChanged += TextBoxBatch_TextChanged;
            // 
            // comboBoxVoiceSoft
            // 
            comboBoxVoiceSoft.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxVoiceSoft.Font = new Font("メイリオ", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            comboBoxVoiceSoft.FormattingEnabled = true;
            comboBoxVoiceSoft.Location = new Point(96, 47);
            comboBoxVoiceSoft.Name = "comboBoxVoiceSoft";
            comboBoxVoiceSoft.Size = new Size(215, 25);
            comboBoxVoiceSoft.TabIndex = 24;
            comboBoxVoiceSoft.TextChanged += ComboBoxVoiceSoft_TextChanged;
            // 
            // comboBoxPreset
            // 
            comboBoxPreset.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPreset.Font = new Font("メイリオ", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            comboBoxPreset.FormattingEnabled = true;
            comboBoxPreset.Location = new Point(96, 79);
            comboBoxPreset.Name = "comboBoxPreset";
            comboBoxPreset.Size = new Size(215, 25);
            comboBoxPreset.TabIndex = 25;
            comboBoxPreset.SelectedIndexChanged += ComboBoxPreset_SelectedIndexChanged;
            // 
            // textBoxParam
            // 
            textBoxParam.Font = new Font("メイリオ", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxParam.Location = new Point(158, 112);
            textBoxParam.Name = "textBoxParam";
            textBoxParam.ReadOnly = true;
            textBoxParam.Size = new Size(107, 24);
            textBoxParam.TabIndex = 26;
            textBoxParam.Text = "1,0,1,1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(10, 50);
            label1.Name = "label1";
            label1.Size = new Size(80, 15);
            label1.TabIndex = 27;
            label1.Text = "音声合成ソフト";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(32, 82);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 28;
            label3.Text = "キャラクター";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(31, 115);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 29;
            label4.Text = "ボイス調整";
            // 
            // buttonLogSave
            // 
            buttonLogSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonLogSave.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonLogSave.Image = Properties.Resources.Save;
            buttonLogSave.Location = new Point(1057, 538);
            buttonLogSave.Name = "buttonLogSave";
            buttonLogSave.Size = new Size(80, 23);
            buttonLogSave.TabIndex = 34;
            buttonLogSave.Text = "ログ保存";
            buttonLogSave.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonLogSave.UseVisualStyleBackColor = true;
            buttonLogSave.Click += ButtonLogSave_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { ToolStripMenuTool, ToolStripMenuOption, ToolStripMenuReadDctionary });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1149, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuTool
            // 
            ToolStripMenuTool.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuDelXmlSpace, ToolStripMenuXmltoJson, ToolStripMenuMakeWavFromText });
            ToolStripMenuTool.Name = "ToolStripMenuTool";
            ToolStripMenuTool.Size = new Size(46, 20);
            ToolStripMenuTool.Text = "ツール";
            // 
            // toolStripMenuDelXmlSpace
            // 
            toolStripMenuDelXmlSpace.Name = "toolStripMenuDelXmlSpace";
            toolStripMenuDelXmlSpace.Size = new Size(289, 22);
            toolStripMenuDelXmlSpace.Text = "翻訳用XMLの空白削除";
            toolStripMenuDelXmlSpace.Click += ToolStripMenuDelXmlSpace_Click;
            // 
            // ToolStripMenuXmltoJson
            // 
            ToolStripMenuXmltoJson.Name = "ToolStripMenuXmltoJson";
            ToolStripMenuXmltoJson.Size = new Size(289, 22);
            ToolStripMenuXmltoJson.Text = "翻訳用XMLからDBVO用辞書(JSON)を作成";
            ToolStripMenuXmltoJson.ToolTipText = resources.GetString("ToolStripMenuXmltoJson.ToolTipText");
            ToolStripMenuXmltoJson.Click += ButtonXmltoJson_Click;
            // 
            // ToolStripMenuMakeWavFromText
            // 
            ToolStripMenuMakeWavFromText.Name = "ToolStripMenuMakeWavFromText";
            ToolStripMenuMakeWavFromText.Size = new Size(289, 22);
            ToolStripMenuMakeWavFromText.Text = "テキストファイルから音声(WAV)作成";
            ToolStripMenuMakeWavFromText.Click += ToolStripMenuMakeWavFromText_Click;
            // 
            // ToolStripMenuOption
            // 
            ToolStripMenuOption.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemIsShowMsg, ToolStripMenuUseBatchOutput });
            ToolStripMenuOption.Name = "ToolStripMenuOption";
            ToolStripMenuOption.Size = new Size(62, 20);
            ToolStripMenuOption.Text = "オプション";
            // 
            // ToolStripMenuItemIsShowMsg
            // 
            ToolStripMenuItemIsShowMsg.CheckOnClick = true;
            ToolStripMenuItemIsShowMsg.Name = "ToolStripMenuItemIsShowMsg";
            ToolStripMenuItemIsShowMsg.Size = new Size(268, 22);
            ToolStripMenuItemIsShowMsg.Text = "処理完了時にメッセージ表示";
            // 
            // ToolStripMenuUseBatchOutput
            // 
            ToolStripMenuUseBatchOutput.CheckOnClick = true;
            ToolStripMenuUseBatchOutput.Name = "ToolStripMenuUseBatchOutput";
            ToolStripMenuUseBatchOutput.Size = new Size(268, 22);
            ToolStripMenuUseBatchOutput.Text = "個別処理でも一括処理の出力先を使用";
            // 
            // ToolStripMenuReadDctionary
            // 
            ToolStripMenuReadDctionary.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuReadDicDef, ToolStripMenuReadDicTxt, ToolStripMenuOpenDictionary });
            ToolStripMenuReadDctionary.Name = "ToolStripMenuReadDctionary";
            ToolStripMenuReadDctionary.Size = new Size(123, 20);
            ToolStripMenuReadDctionary.Text = "読み＆アクセント辞書";
            // 
            // toolStripMenuReadDicDef
            // 
            toolStripMenuReadDicDef.Name = "toolStripMenuReadDicDef";
            toolStripMenuReadDicDef.Size = new Size(186, 22);
            toolStripMenuReadDicDef.Text = "デフォルト辞書を再読込";
            toolStripMenuReadDicDef.Click += ToolStripMenuReadDicDefault_Click;
            // 
            // ToolStripMenuReadDicTxt
            // 
            ToolStripMenuReadDicTxt.Name = "ToolStripMenuReadDicTxt";
            ToolStripMenuReadDicTxt.Size = new Size(186, 22);
            ToolStripMenuReadDicTxt.Text = "テキストファイルから読込";
            ToolStripMenuReadDicTxt.Click += ToolStripMenuReadDicTxt_Click;
            // 
            // ToolStripMenuOpenDictionary
            // 
            ToolStripMenuOpenDictionary.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuOpenDictDef, ToolStripMenuOpenDictOther });
            ToolStripMenuOpenDictionary.Name = "ToolStripMenuOpenDictionary";
            ToolStripMenuOpenDictionary.Size = new Size(186, 22);
            ToolStripMenuOpenDictionary.Text = "外部アプリで開く";
            // 
            // ToolStripMenuOpenDictDef
            // 
            ToolStripMenuOpenDictDef.Name = "ToolStripMenuOpenDictDef";
            ToolStripMenuOpenDictDef.Size = new Size(165, 22);
            ToolStripMenuOpenDictDef.Text = "デフォルト辞書";
            ToolStripMenuOpenDictDef.Click += ToolStripMenuOpenDictDef_Click;
            // 
            // ToolStripMenuOpenDictOther
            // 
            ToolStripMenuOpenDictOther.Name = "ToolStripMenuOpenDictOther";
            ToolStripMenuOpenDictOther.Size = new Size(165, 22);
            ToolStripMenuOpenDictOther.Text = "他のテキストファイル";
            ToolStripMenuOpenDictOther.Click += ToolStripMenuOpenDictOther_Click;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(117, 150);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(100, 23);
            textBoxSearch.TabIndex = 36;
            textBoxSearch.Visible = false;
            // 
            // buttonLogClear
            // 
            buttonLogClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonLogClear.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonLogClear.Image = Properties.Resources.ClearWindowContent;
            buttonLogClear.Location = new Point(971, 538);
            buttonLogClear.Name = "buttonLogClear";
            buttonLogClear.Size = new Size(80, 23);
            buttonLogClear.TabIndex = 37;
            buttonLogClear.Text = "ログ消去";
            buttonLogClear.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonLogClear.UseVisualStyleBackColor = true;
            buttonLogClear.Click += ButtonLogClear_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1149, 566);
            Controls.Add(buttonLogClear);
            Controls.Add(textBoxSearch);
            Controls.Add(checkBoxOnlyMoji);
            Controls.Add(buttonLogSave);
            Controls.Add(buttonProcCancel);
            Controls.Add(buttonVoiceControl);
            Controls.Add(buttonGetChar);
            Controls.Add(buttonSample);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(textBoxParam);
            Controls.Add(comboBoxPreset);
            Controls.Add(comboBoxVoiceSoft);
            Controls.Add(buttonMakePack);
            Controls.Add(labelProgress);
            Controls.Add(progressBar1);
            Controls.Add(buttonMakeWav);
            Controls.Add(label2);
            Controls.Add(listBoxLog);
            Controls.Add(groupBox4);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "FormMain";
            Opacity = 0.95D;
            Text = "DBVO JPVoice Support Tool";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlickerFreeListBox listBoxLog;
        private Label label2;
        private Button buttonMakeFuz;
        private ProgressBar progressBar1;
        private Label labelProgress;
        private GroupBox groupBox2;
        private Button buttonYakitoriOpen;
        private TextBox textBoxYakitoriPath;
        private ToolTip toolTip;
        private Button buttonMakeLip;
        private GroupBox groupBox3;
        private Button buttonFaceFXOpen;
        private TextBox textBoxFaceFXPath;
        private Label labelFaceFX;
        private Label labelYakitori;
        private Button buttonBatch;
        private Button buttonMakePack;
        private GroupBox groupBox4;
        private Button buttonMakeWav;
        private ComboBox comboBoxVoiceSoft;
        private ComboBox comboBoxPreset;
        private Label label1;
        private Label label3;
        private Label label4;
        private Button buttonSample;
        private Button buttonGetChar;
        private Button buttonBatchFolder;
        private TextBox textBoxBatch;
        private Label label5;
        private CheckBox checkBoxBatch;
        private Button buttonVoiceControl;
        private TextBox textBoxParam;
        private Button buttonProcCancel;
        private Button buttonLogSave;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ToolStripMenuTool;
        private ToolStripMenuItem toolStripMenuDelXmlSpace;
        private ToolStripMenuItem ToolStripMenuMakeWavFromText;
        private CheckBox checkBoxOnlyMoji;
        private TextBox textBoxSearch;
        private ToolStripMenuItem ToolStripMenuOption;
        private ToolStripMenuItem ToolStripMenuItemIsShowMsg;
        private Button buttonBatchAll;
        private Button buttonLogClear;
        private ToolStripMenuItem ToolStripMenuUseBatchOutput;
        private ToolStripMenuItem ToolStripMenuXmltoJson;
        private ToolStripMenuItem ToolStripMenuReadDctionary;
        private ToolStripMenuItem toolStripMenuReadDicDef;
        private ToolStripMenuItem ToolStripMenuReadDicTxt;
        private ToolStripMenuItem ToolStripMenuOpenDictionary;
        private ToolStripMenuItem ToolStripMenuOpenDictDef;
        private ToolStripMenuItem ToolStripMenuOpenDictOther;
    }
}
