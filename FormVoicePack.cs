using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBVO_JPVoice_Tool
{
    public partial class FormVoicePack : Form
    {
        public FormVoicePack()
        {
            InitializeComponent();
        }

        private void ButtonWavPath_Click(object sender, EventArgs e)
        {
            using CommonOpenFileDialog ofd = new()
            {
                IsFolderPicker = true,
                Title = "音声ファイル(FUZ)の格納フォルダを選択してください"
            };

            if (ofd.ShowDialog() != CommonFileDialogResult.Ok) { return; }

            textBoxFuz.Text = ofd.FileName;
        }

        private void ButtonAddJson_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "JSONファイル(*.json)|*.json",
                FilterIndex = 1,
                Title = "JSONファイルを選択してください(複数可)",
                Multiselect = true,
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) { return; }

            foreach (var item in ofd.FileNames)
            {
                textBoxJson.Text += textBoxJson.Text.Length > 0 ? $"|{item}" : item;
            }

        }

        private void ButtonOutput_Click(object sender, EventArgs e)
        {
            using CommonOpenFileDialog ofd = new()
            {
                IsFolderPicker = true,
                Title = "出力先のフォルダを選択してください"
            };

            if (ofd.ShowDialog() != CommonFileDialogResult.Ok) { return; }

            textBoxOutput.Text = ofd.FileName;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            string strOutput = $@"{textBoxOutput.Text}\dbvo_jpvoice_tool_tmp";
            string strDBVOPath = $@"{strOutput}\DragonbornVoiceOver";
            string strSoundPath = $@"{strOutput}\Sound";
            string strOutputZipFile = $@"{textBoxOutput.Text}\{textBoxID.Text}.zip";

            if (File.Exists(strOutputZipFile))
            {

                if (MessageBox.Show(
                    $"フォルダ：{textBoxOutput.Text}には同名のファイル{textBoxID.Text}.zipが存在します"
                    + "\n上書きしますか？", "ボイスパック作成",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                { return; }

                File.Delete(strOutputZipFile);
            }

            if (!Directory.Exists(strDBVOPath)) { Directory.CreateDirectory(strDBVOPath); }
            if (!Directory.Exists(strSoundPath)) { Directory.CreateDirectory(strSoundPath); }

            string strVoicePacks = $@"{strDBVOPath}\voice_packs";
            string strLocalePacks = $@"{strDBVOPath}\locale_packs";
            string strLocaleJA = $@"{strLocalePacks}\ja";

            if (!Directory.Exists(strVoicePacks)) { Directory.CreateDirectory(strVoicePacks); }
            if (!Directory.Exists(strLocalePacks)) { Directory.CreateDirectory(strLocalePacks); }
            if (!Directory.Exists(strLocaleJA)) { Directory.CreateDirectory(strLocaleJA); }

            string strSoundDBVOPath = $@"{strSoundPath}\DBVO";
            string strSoundIDPath = $@"{strSoundDBVOPath}\{textBoxID.Text}";

            if (!Directory.Exists(strSoundDBVOPath)) { Directory.CreateDirectory(strSoundDBVOPath); }
            if (!Directory.Exists(strSoundIDPath)) { Directory.CreateDirectory(strSoundIDPath); }

            var dict1 = new Dictionary<string, string>
            {
                { "voice_pack_name", textBoxName.Text },
                { "voice_pack_id", textBoxID.Text }
            };

            File.WriteAllText($@"{strVoicePacks}\{textBoxID.Text.ToLower()}_voice_pack.json", JsonConvert.SerializeObject(dict1, Formatting.Indented));

            if (textBoxJson.Text != string.Empty)
            {
                foreach (var file in textBoxJson.Text.Split('|'))
                {
                    File.Copy(file, $@"{strLocaleJA}\{Path.GetFileName(file)}");
                }
            }

            if (textBoxFuz.Text != string.Empty)
            {
                foreach (string file in Directory.GetFiles(textBoxFuz.Text))
                {
                    if (Path.GetExtension(file) is ".fuz")
                    {
                        File.Copy(file, $@"{strSoundIDPath}\{Path.GetFileName(file)}");
                    }
                }
            }

            ZipFile.CreateFromDirectory($@"{strOutput}", $@"{textBoxOutput.Text}\{textBoxID.Text}.zip");
            if (Directory.Exists(strOutput)) { Directory.Delete(strOutput, true); }

            MessageBox.Show(
                $"フォルダ：{textBoxOutput.Text}に{textBoxID.Text}.zipが作成されました。"
                + "\nご確認ください", "ボイスパック作成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void TextBoxID_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (textBoxID.Text.Length > 0 && textBoxName.Text.Length > 0 && textBoxOutput.Text.Length > 0);
        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (textBoxID.Text.Length > 0 && textBoxName.Text.Length > 0 && textBoxOutput.Text.Length > 0);
        }

        private void TextBoxOutput_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (textBoxID.Text.Length > 0 && textBoxName.Text.Length > 0 && textBoxOutput.Text.Length > 0);
        }

        private void FormVoicePack_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
