namespace DBVO_JPVoice_Tool
{
    partial class FormVoicePack
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonOK = new Button();
            textBoxID = new TextBox();
            textBoxName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            textBoxFuz = new TextBox();
            buttonWavPath = new Button();
            buttonAddJson = new Button();
            textBoxJson = new TextBox();
            textBoxOutput = new TextBox();
            buttonOutput = new Button();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.BackColor = Color.Linen;
            buttonOK.Enabled = false;
            buttonOK.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonOK.Image = Properties.Resources.FolderSuppressed;
            buttonOK.Location = new Point(427, 175);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(75, 23);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "作成";
            buttonOK.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonOK.UseVisualStyleBackColor = false;
            buttonOK.Click += ButtonOK_Click;
            // 
            // textBoxID
            // 
            textBoxID.Location = new Point(133, 16);
            textBoxID.Name = "textBoxID";
            textBoxID.Size = new Size(152, 23);
            textBoxID.TabIndex = 2;
            textBoxID.TextChanged += TextBoxID_TextChanged;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(133, 49);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(152, 23);
            textBoxName.TabIndex = 3;
            textBoxName.TextChanged += TextBoxName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 5;
            label1.Text = "ボイスパックID(必須)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(6, 48);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 6;
            label2.Text = "ボイスパック名(必須)\r\n";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(6, 128);
            label3.Name = "label3";
            label3.Size = new Size(104, 15);
            label3.TabIndex = 7;
            label3.Text = "辞書ファイル(JSON)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(5, 91);
            label4.Name = "label4";
            label4.Size = new Size(95, 30);
            label4.TabIndex = 9;
            label4.Text = "音声ファイル(FUZ)\r\n格納フォルダ";
            // 
            // textBoxFuz
            // 
            textBoxFuz.BackColor = SystemColors.ActiveBorder;
            textBoxFuz.Location = new Point(133, 88);
            textBoxFuz.Name = "textBoxFuz";
            textBoxFuz.ReadOnly = true;
            textBoxFuz.Size = new Size(224, 23);
            textBoxFuz.TabIndex = 8;
            // 
            // buttonWavPath
            // 
            buttonWavPath.Image = Properties.Resources.OpenFolder;
            buttonWavPath.Location = new Point(363, 87);
            buttonWavPath.Name = "buttonWavPath";
            buttonWavPath.Size = new Size(40, 23);
            buttonWavPath.TabIndex = 11;
            buttonWavPath.UseVisualStyleBackColor = true;
            buttonWavPath.Click += ButtonWavPath_Click;
            // 
            // buttonAddJson
            // 
            buttonAddJson.Image = Properties.Resources.OpenFolder;
            buttonAddJson.Location = new Point(363, 124);
            buttonAddJson.Name = "buttonAddJson";
            buttonAddJson.Size = new Size(40, 23);
            buttonAddJson.TabIndex = 12;
            buttonAddJson.UseVisualStyleBackColor = true;
            buttonAddJson.Click += ButtonAddJson_Click;
            // 
            // textBoxJson
            // 
            textBoxJson.BackColor = SystemColors.ActiveBorder;
            textBoxJson.Location = new Point(133, 125);
            textBoxJson.Name = "textBoxJson";
            textBoxJson.ReadOnly = true;
            textBoxJson.Size = new Size(224, 23);
            textBoxJson.TabIndex = 13;
            // 
            // textBoxOutput
            // 
            textBoxOutput.BackColor = SystemColors.ActiveBorder;
            textBoxOutput.Location = new Point(5, 176);
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.Size = new Size(224, 23);
            textBoxOutput.TabIndex = 15;
            textBoxOutput.TextChanged += TextBoxOutput_TextChanged;
            // 
            // buttonOutput
            // 
            buttonOutput.Image = Properties.Resources.OpenFolder;
            buttonOutput.Location = new Point(234, 175);
            buttonOutput.Name = "buttonOutput";
            buttonOutput.Size = new Size(40, 23);
            buttonOutput.TabIndex = 14;
            buttonOutput.UseVisualStyleBackColor = true;
            buttonOutput.Click += ButtonOutput_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = SystemColors.ControlLightLight;
            label5.Location = new Point(6, 158);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 16;
            label5.Text = "出力先";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = SystemColors.ControlLightLight;
            label6.Location = new Point(291, 45);
            label6.Name = "label6";
            label6.Size = new Size(212, 30);
            label6.TabIndex = 17;
            label6.Text = "MCMのボイスパック一覧に表示される名前\r\n日本語も可";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Yu Gothic UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = SystemColors.ControlLightLight;
            label7.Location = new Point(291, 19);
            label7.Name = "label7";
            label7.Size = new Size(205, 15);
            label7.TabIndex = 18;
            label7.Text = "他のボイスパックのIDと被らない一意のID";
            // 
            // FormVoicePack
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(514, 211);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(textBoxOutput);
            Controls.Add(buttonOutput);
            Controls.Add(textBoxJson);
            Controls.Add(buttonAddJson);
            Controls.Add(buttonWavPath);
            Controls.Add(label4);
            Controls.Add(textBoxFuz);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxName);
            Controls.Add(textBoxID);
            Controls.Add(buttonOK);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "FormVoicePack";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ボイスパック作成";
            KeyPress += FormVoicePack_KeyPress;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonOK;
        private TextBox textBoxID;
        private TextBox textBoxName;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox textBoxFuz;
        private Button buttonWavPath;
        private Button buttonAddJson;
        private TextBox textBoxJson;
        private TextBox textBoxOutput;
        private Button buttonOutput;
        private Label label5;
        private Label label6;
        private Label label7;
    }
}