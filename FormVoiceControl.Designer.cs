namespace DBVO_JPVoice_Tool
{
    partial class FormVoiceControl
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
            trackBarSpeed = new TrackBar();
            buttonCancel = new Button();
            buttonOK = new Button();
            labelSpeed = new Label();
            textBoxSpeed = new TextBox();
            textBoxPitch = new TextBox();
            label1 = new Label();
            trackBarPitch = new TrackBar();
            textBoxInto = new TextBox();
            label2 = new Label();
            trackBarInto = new TrackBar();
            textBoxVolume = new TextBox();
            label3 = new Label();
            trackBarVolume = new TrackBar();
            buttonReset = new Button();
            ((System.ComponentModel.ISupportInitialize)trackBarSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarInto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).BeginInit();
            SuspendLayout();
            // 
            // trackBarSpeed
            // 
            trackBarSpeed.Location = new Point(56, 15);
            trackBarSpeed.Maximum = 200;
            trackBarSpeed.Minimum = 50;
            trackBarSpeed.Name = "trackBarSpeed";
            trackBarSpeed.Size = new Size(132, 45);
            trackBarSpeed.TabIndex = 0;
            trackBarSpeed.TickFrequency = 10;
            trackBarSpeed.Value = 50;
            trackBarSpeed.Scroll += TrackBarSpeed_Scroll;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.BackColor = Color.RosyBrown;
            buttonCancel.Location = new Point(200, 189);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(84, 23);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "キャンセル";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOK.BackColor = Color.RosyBrown;
            buttonOK.Location = new Point(110, 189);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(84, 23);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = false;
            buttonOK.Click += ButtonOK_Click;
            // 
            // labelSpeed
            // 
            labelSpeed.AutoSize = true;
            labelSpeed.ForeColor = SystemColors.ButtonHighlight;
            labelSpeed.Location = new Point(22, 21);
            labelSpeed.Name = "labelSpeed";
            labelSpeed.Size = new Size(31, 15);
            labelSpeed.TabIndex = 3;
            labelSpeed.Text = "話速";
            // 
            // textBoxSpeed
            // 
            textBoxSpeed.BackColor = SystemColors.InactiveCaption;
            textBoxSpeed.Font = new Font("Yu Gothic UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxSpeed.Location = new Point(223, 21);
            textBoxSpeed.Name = "textBoxSpeed";
            textBoxSpeed.Size = new Size(53, 27);
            textBoxSpeed.TabIndex = 4;
            textBoxSpeed.TextAlign = HorizontalAlignment.Right;
            // 
            // textBoxPitch
            // 
            textBoxPitch.BackColor = SystemColors.InactiveCaption;
            textBoxPitch.Font = new Font("Yu Gothic UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxPitch.Location = new Point(223, 52);
            textBoxPitch.Name = "textBoxPitch";
            textBoxPitch.Size = new Size(53, 27);
            textBoxPitch.TabIndex = 7;
            textBoxPitch.TextAlign = HorizontalAlignment.Right;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(22, 52);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 6;
            label1.Text = "音高";
            // 
            // trackBarPitch
            // 
            trackBarPitch.Location = new Point(56, 46);
            trackBarPitch.Maximum = 15;
            trackBarPitch.Minimum = -15;
            trackBarPitch.Name = "trackBarPitch";
            trackBarPitch.Size = new Size(132, 45);
            trackBarPitch.TabIndex = 5;
            trackBarPitch.TickFrequency = 10;
            trackBarPitch.Scroll += TrackBarPitch_Scroll;
            // 
            // textBoxInto
            // 
            textBoxInto.BackColor = SystemColors.InactiveCaption;
            textBoxInto.Font = new Font("Yu Gothic UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxInto.Location = new Point(223, 83);
            textBoxInto.Name = "textBoxInto";
            textBoxInto.Size = new Size(53, 27);
            textBoxInto.TabIndex = 10;
            textBoxInto.TextAlign = HorizontalAlignment.Right;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ButtonHighlight;
            label2.Location = new Point(22, 83);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 9;
            label2.Text = "抑揚";
            // 
            // trackBarInto
            // 
            trackBarInto.Location = new Point(56, 77);
            trackBarInto.Maximum = 200;
            trackBarInto.Name = "trackBarInto";
            trackBarInto.Size = new Size(132, 45);
            trackBarInto.TabIndex = 8;
            trackBarInto.TickFrequency = 10;
            trackBarInto.Value = 100;
            trackBarInto.Scroll += TrackBarInto_Scroll;
            // 
            // textBoxVolume
            // 
            textBoxVolume.BackColor = SystemColors.InactiveCaption;
            textBoxVolume.Font = new Font("Yu Gothic UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxVolume.Location = new Point(223, 113);
            textBoxVolume.Name = "textBoxVolume";
            textBoxVolume.Size = new Size(53, 27);
            textBoxVolume.TabIndex = 13;
            textBoxVolume.TextAlign = HorizontalAlignment.Right;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ButtonHighlight;
            label3.Location = new Point(22, 113);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 12;
            label3.Text = "音量";
            // 
            // trackBarVolume
            // 
            trackBarVolume.Location = new Point(56, 107);
            trackBarVolume.Maximum = 200;
            trackBarVolume.Name = "trackBarVolume";
            trackBarVolume.Size = new Size(132, 45);
            trackBarVolume.TabIndex = 11;
            trackBarVolume.TickFrequency = 10;
            trackBarVolume.Value = 100;
            trackBarVolume.Scroll += TrackBarVolume_Scroll;
            // 
            // buttonReset
            // 
            buttonReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonReset.BackColor = Color.RosyBrown;
            buttonReset.Image = Properties.Resources.Refresh;
            buttonReset.Location = new Point(212, 146);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(66, 23);
            buttonReset.TabIndex = 14;
            buttonReset.Text = "リセット";
            buttonReset.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonReset.UseVisualStyleBackColor = false;
            buttonReset.Click += ButtonReset_Click;
            // 
            // FormVoiceControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(296, 224);
            ControlBox = false;
            Controls.Add(buttonReset);
            Controls.Add(textBoxVolume);
            Controls.Add(label3);
            Controls.Add(trackBarVolume);
            Controls.Add(textBoxInto);
            Controls.Add(label2);
            Controls.Add(trackBarInto);
            Controls.Add(textBoxPitch);
            Controls.Add(label1);
            Controls.Add(trackBarPitch);
            Controls.Add(textBoxSpeed);
            Controls.Add(labelSpeed);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(trackBarSpeed);
            KeyPreview = true;
            Name = "FormVoiceControl";
            Opacity = 0.9D;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "ボイス調整";
            Load += FormVoiceControl_Load;
            KeyPress += FormVoiceControl_KeyPress;
            ((System.ComponentModel.ISupportInitialize)trackBarSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitch).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarInto).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar trackBarSpeed;
        private Button buttonCancel;
        private Button buttonOK;
        private Label labelSpeed;
        private TextBox textBoxSpeed;
        private TextBox textBoxPitch;
        private Label label1;
        private TrackBar trackBarPitch;
        private TextBox textBoxInto;
        private Label label2;
        private TrackBar trackBarInto;
        private TextBox textBoxVolume;
        private Label label3;
        private TrackBar trackBarVolume;
        private Button buttonReset;
    }
}