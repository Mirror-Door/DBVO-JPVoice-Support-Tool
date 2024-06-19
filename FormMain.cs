using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DBVO_JPVoice_Tool
{

    public partial class FormMain : Form
    {
        /// <summary>
        /// WAV作成用データ
        /// </summary>
        private struct MakeWavData
        {
            public string JPWord;
            public string EnWord;
            public VoiceFunction voiceFunc;
            public string strOutPutPath;
            public int speaker;
            public string style;
            public double[] dParams;
        }

        /// <summary>
        /// LIP作成用データ
        /// </summary>
        private struct MakeLipData
        {
            public string[] strArrWavFiles;
            public string strOutPutPath;
            public string strTemporaryPath;
            public string strFaceFXWrapper;
            public string strFonixData;
        }

        /// <summary>
        /// FUZ作成用データ
        /// </summary>
        private struct MakeFuzData
        {
            public string[] strArrWavFiles;
            public string strOutPutPath;
            public string strTemporaryPath;
            public string strWMAEnc;
            public string strFuzEnc;
            public bool isdelete;
        }

        private CancellationTokenSource? CancelToken;

        private const string FaceFXWrapper = "FaceFXWrapper.exe";
        private const string FonixData = "FonixData.cdf";

        private const string xWMAEncode = "xWMAEncode.exe";
        private const string BmlFuzEncode = "BmlFuzEncode.exe";

        private const string DefaultDictionary = @".\DefaultDictionary.csv";

        /// <summary>
        /// ログに何件ずつ表示するか
        /// </summary>
        private const int ListBuffer = 50;

        /// <summary>
        /// 処理中のUI制御
        /// </summary>
        /// <param name="_b">trueでボタンが使用可能</param>
        /// <param name="_enableCancel">キャンセルボタンを表示するか</param>
        private void EnabledButtons(bool _b, bool _enableCancel)
        {
            buttonProcCancel.Visible = _enableCancel;

            buttonBatchFolder.Enabled = _b;
            buttonFaceFXOpen.Enabled = _b;
            buttonGetChar.Enabled = _b && buttonGetChar.Text != "取得中…";
            buttonMakeFuz.Enabled = _b && IsInstallYakitori() && textBoxBatch.Text.Length > 0;
            buttonMakeLip.Enabled = _b && IsInstallFaceFX() && textBoxBatch.Text.Length > 0;
            buttonMakePack.Enabled = _b;
            buttonMakeWav.Enabled = _b && comboBoxPreset.SelectedIndex != -1;
            buttonSample.Enabled = _b && buttonMakeWav.Enabled;
            buttonSampleText.Enabled = _b && buttonMakeWav.Enabled;
            buttonVoiceControl.Enabled = _b;
            //buttonXmltoJson.Enabled = _b;
            buttonYakitoriOpen.Enabled = _b;
            buttonFaceFXFind.Enabled = _b;
            buttonYakitoriFind.Enabled = _b;

            buttonBatch.Enabled = _b && buttonMakeLip.Enabled && buttonMakeFuz.Enabled;
            buttonBatchAll.Enabled = _b && buttonMakeWav.Enabled && buttonBatch.Enabled;

            ToolStripMenuTool.Enabled = _b;
            ToolStripMenuOption.Enabled = _b;
            ToolStripMenuReadDctionary.Enabled = _b;
        }

        private void ProgressStart(string _text, int _min, int _max)
             => ProgressStart(progressBar1, labelProgress, _text, _min, _max);

        /// <summary>
        /// 処理開始時のUI制御
        /// </summary>
        /// <param name="_text">処理の名前</param>
        /// <param name="_min">最小件数</param>
        /// <param name="_max">最大件数</param>
        private void ProgressStart(ProgressBar _prog, Label _label, string _text, int _min, int _max)
        {
            _label.Visible = true;
            _prog.Visible = true;

            _prog.Minimum = _min;
            _prog.Maximum = _max;
            _prog.Value = _min;

            _label.Text = $"{_text}…{_prog.Minimum} / {_prog.Maximum}";
            _label.Update();

            EnabledButtons(false, true);
        }

        /// <summary>
        /// 処理の進行中のUI制御
        /// </summary>
        /// <param name="_value">処理済みの件数</param>
        private void ProgressProc(int _value = 1) => ProgressProc(progressBar1, labelProgress, _value);

        private void ProgressProc(ProgressBar _prog, Label _label, int _value)
        {
            _prog.Value = _value;

            string str = _label.Text;
            _label.Text = $"{str[..str.IndexOf("…")]}…{_prog.Value} / {_prog.Maximum}";
            _label.Update();
        }

        private void ProgressProcPlus(int _value = 1) => ProgressProcPlus(progressBar1, labelProgress, _value);

        /// <summary>
        /// 処理の進行中のUI制御、こちらは現在の値に加算します
        /// </summary>
        /// <param name="_value">加算する件数</param>
        private void ProgressProcPlus(ProgressBar _prog, Label _label, int _value = 1) => ProgressProc(_prog, _label, _prog.Value + _value);

        private void ProgressEnd(ProgressBar _prog, Label _label)
        {
            _prog.Visible = false;
            _label.Visible = false;

            EnabledButtons(true, false);
        }

        /// <summary>
        /// 処理終了時のUI制御
        /// </summary>
        private void ProgressEnd() => ProgressEnd(progressBar1, labelProgress);


        /// <summary>
        /// 処理開始時のログ
        /// </summary>
        /// <param name="_title">処理名</param>
        /// <param name="_isLog">falseの場合ログ出力しません</param>
        private void LoggingStart(string _title, bool _isLog = true)
        {
            Logging('-', false, _isLog);
            Logging($"処理開始 《{_title}》", false, _isLog);
            Logging('-', false, _isLog);
        }

        /// <summary>
        /// 処理終了時のログ
        /// </summary>
        /// <param name="_title">処理名</param>
        /// <param name="_isLog">falseの場合ログ出力しません</param>
        private void LoggingEnd(string _title, bool _isLog = true)
        {
            Logging($"処理終了 《{_title}》", false, _isLog);
            Logging('#', true, _isLog);
        }

        /// <summary>
        /// フォルダ選択ダイアログを表示
        /// </summary>
        /// <param name="_title">ダイアログのタイトル</param>
        /// <param name="_outPath">出力先、入力されるとダイアログは表示せずこの値を返します</param>
        /// <returns></returns>
        private static string OpenFolderDialog(string _title, string _outPath)
        {
            using CommonOpenFileDialog off = new()
            {
                Title = _title,
                IsFolderPicker = true,
                RestoreDirectory = false
            };

            if (_outPath != string.Empty) { return _outPath; }
            if (off.ShowDialog() == CommonFileDialogResult.Cancel) { return string.Empty; }
            return off.FileName;
        }

        /// <summary>
        /// バッチ出力先オプションが有効で、出力先が入力されていない場合メッセージボックスを表示
        /// </summary>
        /// <returns>チェックNGの場合false</returns>
        private bool CheckOutputPath()
        {
            if (textBoxBatch.Text == string.Empty)
            {
                MessageBox.Show($"ファイル出力先を選択してください",
                    "ファイル出力先の確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private string? FindFile(string _exeName) => FindFile(new string[] { _exeName });

        private string? FindFile(string[] _exeName)
        {
            static string? FindFileInternal(string directory, string[] fileName)
            {
                try
                {
                    foreach (string file in Directory.EnumerateFiles(directory, fileName[0]))
                    {
                        if(fileName.Length <= 1) {
                            return file;
                        }
                        foreach (string file2 in Directory.EnumerateFiles(directory, fileName[1]))
                        {
                            return file2;
                        }
                    }

                    // サブディレクトリ内を再帰的に検索
                    foreach (string subDirectory in Directory.EnumerateDirectories(directory))
                    {
                        DirectoryInfo dirInfo = new(subDirectory);
                        if ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
                            || (dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            continue;
                        }

                        string? foundFile = FindFileInternal(subDirectory, fileName);
                        if (foundFile is not null)
                        {
                            return foundFile;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // アクセス権限がない場合は無視
                    return null;
                }
                catch (PathTooLongException)
                {
                    // パスが長すぎる場合は無視
                    return null;
                }

                // ファイルが見つからなかった場合
                return null;
            }

            // すべてのドライブ情報を取得
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            // 各ドライブの情報を表示
            foreach (DriveInfo drive in allDrives)
            {
                try
                {
                    Logging($"検索「{_exeName[0]}」：{drive.Name}ドライブ内を検索しています...", true);
                    listBoxLog.Update();

                    string? foundFilePath = FindFileInternal(drive.Name, _exeName);


                    if (foundFilePath is not null)
                    {
                        Logging($"{Path.GetDirectoryName(foundFilePath)}で検出されました", true);
                        listBoxLog.Update();
                        return Path.GetDirectoryName(foundFilePath);
                    }
                    else
                    {
                        //Console.WriteLine("ファイルが見つかりませんでした。");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"エラーが発生しました: {ex.Message}");
                }
            }
            return null;
        }


        /// <summary>
        /// デフォルト辞書を読み込むための事前チェック
        /// </summary>
        /// <param name="_isUseOption">trueの場合、WAV作成前に辞書読み込みのオプションが有効の場合のみ処理します：falseの場合、オプションの有/無効関係なく処理します</param>
        /// <param name="_isLog">ログを出力するか</param>
        /// <param name="_isTextMode">trueの場合、全ての音声合成ソフトの辞書に登録：falseの場合、選択されているソフトのみ</param>
        /// <returns></returns>
        private async Task ReadDefaultDictionary(bool _isUseOption, bool _isLog, bool _isTextMode)
        {
            if (!_isUseOption || (_isUseOption && ToolStripMenuReadDicOption.Checked))
            {
                if (_isUseOption) Logging($"「{ToolStripMenuReadDicOption.Text}」オプション：有効", true);

                if (File.Exists(DefaultDictionary))
                {
                    await ReadDictionary(DefaultDictionary, _isLog, _isTextMode);
                }
                else
                {
                    Logging("デフォルトの読み&アクセント辞書「DefaultDictionary.csv」がありません", true);
                }
            }
        }

        /// <summary>
        /// アプリケーション設定をファイルに保存します
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (textBoxYakitoriPath.Text != string.Empty)
                {
                    config.AppSettings.Settings["YakitoriPath"].Value = textBoxYakitoriPath.Text;
                }
                if (textBoxFaceFXPath.Text != string.Empty)
                {
                    config.AppSettings.Settings["FaceFXPath"].Value = textBoxFaceFXPath.Text;
                }
                if (textBoxBatch.Text != string.Empty)
                {
                    config.AppSettings.Settings["BatchOutputPath"].Value = textBoxBatch.Text;
                }
                config.AppSettings.Settings["IsShowMessageBox"].Value = ToolStripMenuItemIsShowMsg.Checked.ToString();

                if (comboBoxVoiceSoft.SelectedIndex != -1)
                {
                    config.AppSettings.Settings["VoiceSoft"].Value = ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue.ToString();
                }
                if (textBoxParam.Text != string.Empty)
                {
                    config.AppSettings.Settings["VoiceParam"].Value = textBoxParam.Text;
                }
                if (textBoxVoicevoxPath.Text != string.Empty)
                {
                    config.AppSettings.Settings["VoicevoxPath"].Value = textBoxVoicevoxPath.Text;
                }
                if (textBoxCoeiroinkPath.Text != string.Empty)
                {
                    config.AppSettings.Settings["CoeiroinkPath"].Value = textBoxCoeiroinkPath.Text;
                }

                config.AppSettings.Settings["CheckboxOnlyMoji"].Value = checkBoxOnlyMoji.Checked.ToString();
                config.AppSettings.Settings["CheckboxBatch"].Value = checkBoxBatch.Checked.ToString();
                config.AppSettings.Settings["CheckboxReadDic"].Value = ToolStripMenuReadDicOption.Checked.ToString();
                config.AppSettings.Settings["IsSkipMode"].Value = ToolStripMenuItemIsSkipMode.Checked.ToString();

                config.Save();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ボイスソフトのクラスを得ます
        /// </summary>
        /// <param name="_voiceSoft">音声合成ソフトのID、-1の場合コンボボックスで選択されているソフト</param>
        /// <returns>音声合成ソフトのクラス</returns>
        private VoiceFunction? GetVoicesoft(int _voiceSoft = -1)
        => (_voiceSoft switch
        {
            -1 => (int)comboBoxVoiceSoft.SelectedValue,
            _ => _voiceSoft
        }) switch
        {
            (int)VoiceSoft.Voicevox => new VoicevoxFunction(),
            (int)VoiceSoft.Nemo => new VoicevoxNemoFunction(),
            (int)VoiceSoft.Sharevoice => new SharevoiceFunction(),
            (int)VoiceSoft.Itvoice => new ItvoiceFunction(),
            (int)VoiceSoft.Coeiroink => new CoeiroinkFunction(),
            (int)VoiceSoft.StyleBertVITS2 => new StyleBertVITS2Function(),
            _ => null
        };

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォーム起動時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var ver = assembly.Version;

            this.Text = $"{assembly.Name} ver.{ver?.Major}.{ver?.Minor}.{ver?.Build}.{ver?.Revision}";

            ProgressEnd();

            textBoxYakitoriPath.Text = ConfigurationManager.AppSettings["YakitoriPath"];
            textBoxFaceFXPath.Text = ConfigurationManager.AppSettings["FaceFXPath"];
            textBoxBatch.Text = ConfigurationManager.AppSettings["BatchOutputPath"];
            ToolStripMenuItemIsShowMsg.Checked = ConfigurationManager.AppSettings["IsShowMessageBox"] == "True";
            textBoxParam.Text = ConfigurationManager.AppSettings["VoiceParam"] ?? "1,0,1,1";
            checkBoxOnlyMoji.Checked = ConfigurationManager.AppSettings["CheckboxOnlyMoji"] == "True";
            checkBoxBatch.Checked = ConfigurationManager.AppSettings["CheckboxBatch"] == "True";
            ToolStripMenuReadDicOption.Checked = ConfigurationManager.AppSettings["CheckboxReadDic"] == "True";
            textBoxVoicevoxPath.Text = ConfigurationManager.AppSettings["VoicevoxPath"];
            textBoxCoeiroinkPath.Text = ConfigurationManager.AppSettings["CoeiroinkPath"];
            ToolStripMenuItemIsSkipMode.Checked = ConfigurationManager.AppSettings["IsSkipMode"] == "True";

            Logging('*');
            Logging($"{this.Text}");
            Logging('*');

            //await ReadDefaultDictionary(false);

            Logging("音声ファイルを作成したい場合は、まず最初に「音声合成ソフト」を選択してから「キャラ取得」ボタンを押してください");
            Logging("出力先が未入力の場合、ほとんどのボタンが有効にならないので最初に選択してください");
            Logging('=');
            Logging("※各ボタンの上にマウスカーソルを乗っけるとヒントが表示されます", true);

            List<ItemSet> listSrc = new()
            {
                new ItemSet((int)VoiceSoft.Voicevox, "VOICEVOX"),
                new ItemSet((int)VoiceSoft.Coeiroink, "COEIROINK"),
                new ItemSet((int)VoiceSoft.Nemo, "VOICEVOX Nemo"),
                new ItemSet((int)VoiceSoft.Sharevoice, "SHAREVOICE"),
                new ItemSet((int)VoiceSoft.Itvoice, "ITVOICE"),
                new ItemSet((int)VoiceSoft.StyleBertVITS2, "Style-Bert-VITS2"),
            };

            comboBoxVoiceSoft.DataSource = listSrc;
            comboBoxVoiceSoft.DisplayMember = "ItemDisp";
            comboBoxVoiceSoft.ValueMember = "ItemValue";

            if (int.TryParse(ConfigurationManager.AppSettings["VoiceSoft"], out int ivoiceSoft))
            {
                comboBoxVoiceSoft.SelectedValue = ivoiceSoft;
            }
        }

        private async Task SubThreadMakeWav(MakeWavData[] _data, bool _isSkipMode = false)
        {
            int iCounter = 0;
            int iErrCounter = 0;
            int iProgBuffer = (int)Math.Ceiling((double)_data.Length / 100);
            var strListLog = new List<string>(ListBuffer);

            foreach (var data in _data)
            {
                try
                {
                    CancelToken?.Token.ThrowIfCancellationRequested();

                    iCounter++;
                    if (iCounter != 1 && iCounter % 500 == 1) this.Invoke(() => Logging('-'));

                    string strJsonJPWord = data.JPWord.Trim();
                    string strJsonEnWord = data.EnWord.Trim();
                    string strOutputFilePath = $@"{data.strOutPutPath}\{strJsonEnWord}";

                    if (_isSkipMode && File.Exists($"{strOutputFilePath}.wav"))
                    {
                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{iCounter:D3}：(スキップ){strJsonJPWord}  ({strJsonEnWord})");
                    }
                    else
                    {
                        if (!IsJapanese(strJsonJPWord)) { continue; }

                        strJsonJPWord = Regex.Replace(strJsonJPWord, "・", string.Empty);
                        strJsonJPWord = Regex.Replace(strJsonJPWord, @"（.*）", string.Empty);
                        strJsonJPWord = Regex.Replace(strJsonJPWord, @"\(.*\)", string.Empty);
                        strJsonJPWord = Regex.Replace(strJsonJPWord, @"_Alias.*__", string.Empty);

                        if (strJsonJPWord.Trim() == string.Empty) { continue; }

                        HttpStatusCode? statusCode = HttpStatusCode.OK;

                        if (data.voiceFunc.GetType() == typeof(StyleBertVITS2Function))
                        {
                            statusCode = await ((StyleBertVITS2Function)data.voiceFunc).MakeSound(strOutputFilePath, strJsonJPWord, data.style, data.speaker, data.dParams);
                        }
                        else
                        {
                            statusCode = await data.voiceFunc.MakeSound(strOutputFilePath, strJsonJPWord, true, data.speaker, data.dParams);
                        }
                        if (statusCode is null)
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{iCounter:D3}：処理に失敗しました。{data.voiceFunc.Name}が起動していることを確認してください");
                            iErrCounter++;
                        }
                        else if (statusCode == HttpStatusCode.OK)
                        {
                            //this.Invoke(() => Logging($"{iCounter:D3}：(テキスト){strJsonJPWord}  ({strJsonEnWord})", true));
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{iCounter:D3}：(テキスト){strJsonJPWord}  ({strJsonEnWord})");
                        }
                        else if (statusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{iCounter:D3}：パラメータが間違っているためスキップされます");
                            iErrCounter++;
                        }
                        else
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{iCounter:D3}：処理に失敗しました。{data.voiceFunc.Name}が起動していることを確認してください");
                            iErrCounter++;
                        }
                    }

                    if (iCounter % iProgBuffer == 0) this.Invoke(() => ProgressProc(iCounter));
                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => Logging(strListLog.ToArray(), true));
                        strListLog.Clear();
                    }
                }
                catch (OperationCanceledException)
                {
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇処理がキャンセルされました");
                    strListLog.Add(new String('=', 80));
                    break;
                }
                catch (Exception ex)
                {
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇エラーが発生しました：{ex.Message}");
                    strListLog.Add(new String('!', 80));
                    iErrCounter++;
                    continue;
                }
            }

            if (strListLog.Count > 0)
            {
                this.Invoke(() => Logging(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            this.Invoke(() => Logging($"◆データ件数：{iCounter}  エラー件数：{iErrCounter}", true));
            this.Invoke(() => Logging('-'));
        }

        private double[] GetVoiceParams(string _params = "")
        {
            double[] dParams = { 1, 0, 1, 1 };

            string strParams = _params switch { "" => textBoxParam.Text, _ => _params };

            var strArrParam = strParams.Split(',');
            if (strArrParam.Length == 4)
            {
                for (int i = 0; i < strArrParam.Length; i++)
                {
                    try
                    {
                        dParams[i] = Convert.ToDouble(strArrParam[i]);
                    }
                    catch (FormatException)
                    {
                        //何もしない
                    }
                }
            }
            return dParams;
        }

        private async Task PlaySample(string _text = "")
        {
            string[] strArrSampleWord = {
                "当ててやろうか？、誰かにスイートロールを盗まれたかな？",
                "ここに来るべきじゃなかったな",
                "勝利か、ソブンガルデかだ！！",
                "ガラクタだって言う人もいるけど、私はたからものって呼んでるわ",
                "お前のゴールドを数えるのが楽しみだ！",
                "真のオークの実力を見せてやる！",
                "降参するなら今のうちだぞ"
            };

            string strSampleWord;

            if (_text == string.Empty)
            {
                strSampleWord = strArrSampleWord[new Random().Next(strArrSampleWord.Length * 100) / 100];
            }
            else
            {
                strSampleWord = _text;
            }

            EnabledButtons(false, false);
            Logging($"再生：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}「{strSampleWord}」", true);

            VoiceFunction? voiceFunc = GetVoicesoft();
            if (voiceFunc is null) { return; }

            int iSpeaker = (int)comboBoxPreset.SelectedValue;
            string strStyle = comboBoxPreset.Text.Contains(':') ? comboBoxPreset.Text.Split(':')[1] : "";
            var voiceParams = GetVoiceParams();
            HttpStatusCode? statusCode;

            if (voiceFunc.GetType() == typeof(StyleBertVITS2Function))
            {
                statusCode = await Task.Run(() => ((StyleBertVITS2Function)voiceFunc).MakeSound(string.Empty, strSampleWord, strStyle, iSpeaker, voiceParams));
            }
            else
            {
                statusCode = await Task.Run(() => voiceFunc.MakeSound(string.Empty, strSampleWord, true, iSpeaker, voiceParams));
            }

            if (statusCode is null)
            {
                Logging($"処理に失敗しました。{comboBoxVoiceSoft.Text}が起動していることを確認してください", true);
            }
            else if (statusCode == HttpStatusCode.UnprocessableEntity)
            {
                Logging($"送信されたパラメータが間違っています", true);
            }
            else if (statusCode == HttpStatusCode.InternalServerError)
            {
                Logging($"処理に失敗しました：{statusCode}", true);
            }
            else if (statusCode != HttpStatusCode.OK)
            {
                Logging($"処理に失敗しました。{comboBoxVoiceSoft.Text}が起動していることを確認してください", true);
            }

            await Task.Delay(3000);

            EnabledButtons(true, false);
        }

        private static string GetOutputPath(string _msg, string _title, string _output)
        {
            if (MessageBox.Show(_msg, _title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return string.Empty;
            }

            return OpenFolderDialog("出力先のフォルダを選択してください", _output);
        }

        private async Task<bool> MakeWavAsync(string _batchOutPath)
        {
            if (comboBoxPreset.SelectedIndex == -1)
            {
                Logging($"ボイスキャラクターが選択されていません");
                return false;
            }

            if (!CheckOutputPath()) { return false; }

            string strSearch = string.Empty;

            if (checkBoxOnlyMoji.Checked && (strSearch = Interaction.InputBox(
                "検索する文字列を入力してください\n正規表現を使うことができます\n複数OR条件で指定する場合は|で区切ってください\n例：ハドバル|ヘルゲン|宝箱"
                , "文字列を指定して音声(WAV)作成", textBoxSearch.Text, Left + 200, Top + 100)) == string.Empty)
            { return false; }

            textBoxSearch.Text = strSearch;

            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "JSONファイル(*.json)|*.json",
                FilterIndex = 1,
                Title = "JSONファイルを選択してください",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) { return false; }

            var listJsondata = new Dictionary<string, Dictionary<string, string>>();
            var listDataCount = new List<string>();
            int iTotal = 0;

            foreach (var jsonPath in ofd.FileNames)
            {
                using var file = new StreamReader(jsonPath, Encoding.UTF8);

                string value = file.ReadToEnd();
                if (value is null)
                {
                    Logging($"{jsonPath}：辞書ファイルにデータがありませんでした");
                    continue;
                }
                var dicJsondata = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                if (dicJsondata is null)
                {
                    Logging($"{jsonPath}：辞書ファイルにデータがありませんでした");
                    continue;
                }

                if (strSearch != string.Empty)
                {
                    dicJsondata = dicJsondata.Where(x => Regex.IsMatch(x.Key, strSearch)).ToDictionary(x => x.Key, y => y.Value);
                }

                if (dicJsondata.Count > 0)
                {
                    iTotal += dicJsondata.Count;
                    listDataCount.Add($"{Path.GetFileName(jsonPath)}(データ件数：{dicJsondata.Count})");
                    listJsondata.Add(jsonPath, dicJsondata);
                }
            }

            if (iTotal == 0)
            {
                Logging($"辞書ファイルに該当するデータが１件もありません");
                Logging();
                return false;
            }

            //var phraseList = listDataCount.Select((word, index) => (word, index))
            //              .Aggregate("", (partialPhrase, tuple) =>
            //                  partialPhrase + (tuple.index > 0 && tuple.index % 2 == 0 ? "\n" : "   ") + $"{tuple.index + 1}:" + tuple.word)
            //              .Trim();

            string strMessage = $"{ofd.FileNames.Length}件の辞書ファイルが選択されました"
                + "WORD_QUESTION"
                + $"\n\n全データ件数：{iTotal}"
                + $"\nボイスキャラクター：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}"
                + ((strSearch != string.Empty) ? $"\n指定文字列：{strSearch}" : "")
                + "WORD_OUTPUT"
                //+ $"\n\n{phraseList}";
                + $"\n\n{listDataCount.Aggregate((x, y) => $"{x}\n{y}")}";

            if (_batchOutPath == string.Empty)
            {
                strMessage = strMessage.Replace("WORD_QUESTION", "\n音声ファイルを生成します。よろしいですか？(Y:はい、N：いいえ)");
            }
            else
            {
                strMessage = strMessage.Replace("WORD_QUESTION", "\n音声(WAV)、LIP生成、およびFUZ変換を一括で行います。\nよろしいですか？(Y:はい、N：いいえ)");
            }

            var strOutputPath = GetOutputPath(GetMessageOutput(strMessage, textBoxBatch.Text), "確認", textBoxBatch.Text);
            if (strOutputPath == string.Empty) { return false; }

            await ReadDefaultDictionary(true, false, false);

            ProgressStart(progressBar2, labelAllProgress, $"音声ファイル作成 : 全体の進捗", 0, iTotal);

            LoggingStart(buttonMakeWav.Text);
            if (strSearch != string.Empty) { Logging($"指定文字列：{strSearch}"); }
            Logging($"ボイスキャラクター：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}");
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);

            CancelToken = new CancellationTokenSource();

            foreach (var dicJson in listJsondata)
            {
                if (CancelToken.Token.IsCancellationRequested) { break; }

                string strJsonFile = Path.GetFileName(dicJson.Key);

                int ispeaker = comboBoxPreset.SelectedIndex switch { not -1 => (int)comboBoxPreset.SelectedValue, _ => 0 };
                string strStyle = comboBoxPreset.SelectedIndex switch { not -1 => comboBoxPreset.Text.Contains(':') ? comboBoxPreset.Text.Split(":")[1] : "", _ => "" };
                var dParams = GetVoiceParams();

                VoiceFunction? voiceFunc = GetVoicesoft();
                if (voiceFunc is null) { return false; }

                ProgressStart($"辞書ファイル({strJsonFile})", 0, dicJson.Value.Count);

                MakeWavData[] listWavData = dicJson.Value
                    .Select(x => new MakeWavData()
                    {
                        JPWord = x.Key,
                        EnWord = x.Value,
                        strOutPutPath = $@"{strOutputPath}\{strJsonFile}",
                        voiceFunc = voiceFunc,
                        speaker = ispeaker,
                        style = strStyle,
                        dParams = dParams
                    }).ToArray();

                if (listWavData.Length > 0)
                {
                    Logging($"辞書ファイル：{strJsonFile}", true);
                    await Task.Run(() => SubThreadMakeWav(listWavData, ToolStripMenuItemIsSkipMode.Checked));
                }
                ProgressProcPlus(progressBar2, labelAllProgress, listWavData.Length);
            }

            LoggingEnd(buttonMakeWav.Text);
            Logging($"出力フォルダ：{strOutputPath}", true);
            ProgressEnd();
            ProgressEnd(progressBar2, labelAllProgress);

            return !CancelToken.Token.IsCancellationRequested;
        }

        private int SubThreadMakeLip(MakeLipData _data, bool _isSkipMode = false)
        {
            int iCounter = 0;
            int iProgBuffer = (int)Math.Ceiling((double)_data.strArrWavFiles.Length / 100);

            var strListLog = new List<string>(ListBuffer);

            ReadOnlySpan<string> spdata = _data.strArrWavFiles.AsSpan();

            foreach (string wavfile in spdata)
            {
                try
                {
                    CancelToken?.Token.ThrowIfCancellationRequested();

                    if (!Directory.Exists(_data.strOutPutPath)) { new DirectoryInfo(_data.strOutPutPath).Create(); }
                    if (!Directory.Exists(_data.strTemporaryPath)) { new DirectoryInfo(_data.strTemporaryPath).Create(); }

                    string strWavFileNoExt = Path.GetFileNameWithoutExtension(wavfile);
                    string strSampleWavPath = $@"{_data.strTemporaryPath}\{strWavFileNoExt}.wav";
                    string strLipPath = $@"{_data.strOutPutPath}\{strWavFileNoExt}.lip";

                    if (_isSkipMode && File.Exists(strLipPath))
                    {
                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇(スキップ){Path.GetFileName(strLipPath)}");
                    }
                    else
                    {
                        ProcessStartInfo pInfo = new()
                        {
                            FileName = _data.strFaceFXWrapper,
                            Arguments = $"Skyrim USEnglish \"{_data.strFonixData}\" \"{wavfile}\" \"{strSampleWavPath}\" \"{strLipPath}\" \"\"",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };

                        var proc = Process.Start(pInfo);
                        proc?.WaitForExit();

                        if (proc?.ExitCode != 0 || !File.Exists(strLipPath))
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{strLipPath}の作成に失敗しました");
                        }

                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{Path.GetFileName(wavfile)} ==> {Path.GetFileName(strLipPath)}");
                    }

                    iCounter++;
                    if(iCounter % iProgBuffer == 0) this.Invoke(() => ProgressProcPlus(iProgBuffer));

                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => Logging(strListLog.ToArray(), true));
                        strListLog.Clear();
                    }

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇エラーが発生しました。\nファイル：{wavfile}\n{ex.Message}");
                    strListLog.Add(new string('!', 80));
                    continue;
                }
            }

            if (strListLog.Count > 0)
            {
                this.Invoke(() => Logging(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            return iCounter;
        }

        private int SubThreadMakeFuz(MakeFuzData _data, bool _isSkipMode = false)
        {
            int iCounter = 0;
            int iProgBuffer = (int)Math.Ceiling((double)_data.strArrWavFiles.Length / 100);
            var strListLog = new List<string>(ListBuffer);

            ReadOnlySpan<string> spdata = _data.strArrWavFiles.AsSpan();

            foreach (string wavfile in spdata)
            {
                try
                {
                    CancelToken?.Token.ThrowIfCancellationRequested();

                    if (!Directory.Exists(_data.strOutPutPath)) { new DirectoryInfo(_data.strOutPutPath).Create(); }
                    if (!Directory.Exists(_data.strTemporaryPath)) { new DirectoryInfo(_data.strTemporaryPath).Create(); }

                    string strWavFileNoExt = Path.GetFileNameWithoutExtension(wavfile);
                    string strXwmPath = $@"{_data.strTemporaryPath}\{strWavFileNoExt}.xwm";
                    //string strXwmPath = $@"{_data.strTemporaryPath}\{iCounter:D3}.xwm";
                    string strLipPath = wavfile.Replace("wav", "lip");
                    string strFuzPath = $@"{_data.strOutPutPath}\{strWavFileNoExt}.fuz";

                    if (_isSkipMode && File.Exists(strFuzPath))
                    {
                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇(スキップ){Path.GetFileName(strFuzPath)}");
                    }
                    else
                    {
                        ProcessStartInfo pInfo = new()
                        {
                            FileName = _data.strWMAEnc,
                            Arguments = $"\"{wavfile}\" \"{strXwmPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };

                        var proc1 = Process.Start(pInfo);
                        proc1?.WaitForExit();

                        if (proc1?.ExitCode != 0 || !File.Exists(strXwmPath))
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{strXwmPath}の作成に失敗しました");
                            continue;
                        }

                        pInfo.FileName = _data.strFuzEnc;
                        pInfo.Arguments = $"{(File.Exists(strLipPath) ? "" : "-nolip")} \"{strFuzPath}\" \"{strXwmPath}\" \"{(File.Exists(strLipPath) ? strLipPath : "")}\"";

                        var proc2 = Process.Start(pInfo);
                        proc2?.WaitForExit();

                        if (proc2?.ExitCode != 0 || !File.Exists(strFuzPath))
                        {
                            strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{strFuzPath}の作成に失敗しました");
                            continue;
                        }

                        if (_data.isdelete && File.Exists(wavfile)) { File.Delete(wavfile); }
                        if (_data.isdelete && File.Exists(strLipPath)) { File.Delete(strLipPath); }

                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{Path.GetFileName(wavfile)} ==> {Path.GetFileName(strFuzPath)}");
                    }

                    iCounter++;
                    if (iCounter % iProgBuffer == 0) this.Invoke(() => ProgressProcPlus(iProgBuffer));

                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => Logging(strListLog.ToArray(), true));
                        strListLog.Clear();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇エラーが発生しました。\nファイル：{wavfile}\n{ex.Message}");
                    strListLog.Add(new string('!', 80));
                    continue;
                }
            }

            if (strListLog.Count > 0)
            {
                this.Invoke(() => Logging(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            //this.Invoke(() => Logging($"データ件数：{iCounter}", true));
            return iCounter;
        }

        private bool IsInstallFaceFX(bool _isLog = false) => IsInstallRequired(textBoxFaceFXPath.Text, _isLog, FaceFXWrapper, FonixData);

        private bool IsInstallYakitori(bool _isLog = false) => IsInstallRequired(textBoxYakitoriPath.Text, _isLog, xWMAEncode, BmlFuzEncode);

        private bool IsInstallRequired(string _path, bool _isLog, params string[] _findfiles)
        {
            foreach (var file in _findfiles)
            {
                if (!File.Exists($@"{_path}\{file}"))
                {
                    if (_isLog)
                    {
                        Logging($"※指定されたパス「{_path}」に{file}が存在しません");
                        Logging("正しくインストールされていることを確認してください");
                    }
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> MakeLipAsync(string _wavfilepath = "", string _output = "")
        {

            int getcountFunc(string _wavPath)
            => Directory.GetFiles(_wavPath, @"*.wav").Length;

            async Task<bool> makeFunc(string _wavPath)
            {
                string strTemporaryPath = $@"{_wavPath}\_wav";

                string[] strArrWavFiles = Directory.GetFiles(_wavPath, @"*.wav");
                if (strArrWavFiles.Length == 0)
                {
                    Logging($"選択されたフォルダ：{_wavPath}にWAVファイルがありません", true);
                    return true;
                }

                string strOutputPath;
                if (_output == string.Empty)
                {
                    strOutputPath = $@"{textBoxBatch.Text}\{Path.GetFileName(_wavPath)}";
                    if (strOutputPath == string.Empty) { return false; }
                }
                else
                {
                    strOutputPath = $@"{_output}\{Path.GetFileName(_wavPath)}";
                }

                ProgressStart($"{_wavPath}", 0, strArrWavFiles.Length);

                MakeLipData[] makeData = MakeData<MakeLipData>(strArrWavFiles, chunk =>
                {
                    MakeLipData[] makeData = new MakeLipData[chunk.Length];

                    for (int i = 0; i < chunk.Length; i++)
                    {
                        makeData[i].strArrWavFiles = chunk[i].ToArray();
                        makeData[i].strOutPutPath = strOutputPath;
                        makeData[i].strFaceFXWrapper = $@"{textBoxFaceFXPath.Text}\{FaceFXWrapper}";
                        makeData[i].strFonixData = $@"{textBoxFaceFXPath.Text}\{FonixData}";
                        makeData[i].strTemporaryPath = strTemporaryPath;
                    }
                    return makeData;
                });
                int[] iCount = new int[makeData.Length];

                CancelToken = new CancellationTokenSource();

                await Task.Run(() => Parallel.For(0, makeData.Length, (i) => iCount[i] = SubThreadMakeLip(makeData[i], ToolStripMenuItemIsSkipMode.Checked)));

                if (Directory.Exists(strTemporaryPath)) { Directory.Delete(strTemporaryPath, true); }

                ProgressProcPlus(progressBar2, labelAllProgress, iCount.Sum());

                Logging($"データ件数：{iCount.Sum()}"
                    + (CancelToken.Token.IsCancellationRequested ? "" : $"  エラー件数：{strArrWavFiles.Length - iCount.Sum()}"));
                Logging($"出力先：{strOutputPath}", true);
                Logging('=');
                ProgressEnd();

                return !CancelToken.Token.IsCancellationRequested;
            }

            if (!this.IsInstallFaceFX(true))
            {
                return false;
            }

            if (!CheckOutputPath()) { return false; }

            string strWavPath = OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください", _wavfilepath);
            if (strWavPath == string.Empty) { return false; }

            LoggingStart(buttonMakeLip.Text);
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);
            listBoxLog.Update();

            int iDataCount = getcountFunc(strWavPath);
            var strArrDir = Directory.GetDirectories(strWavPath);
            foreach (var strDir in strArrDir)
            {
                iDataCount += getcountFunc(strDir);
            }

            ProgressStart(progressBar2, labelAllProgress, $"{buttonMakeLip.Text} : 全体の進捗", 0, iDataCount);

            if (!await makeFunc(strWavPath))
            {
                Logging("処理がキャンセルされました");
                Logging('=');
                ProgressEnd(progressBar2, labelAllProgress);
                return false;
            }

            foreach (var strDir in strArrDir)
            {
                if (!await makeFunc(strDir))
                {
                    Logging("処理がキャンセルされました");
                    Logging('=');
                    ProgressEnd(progressBar2, labelAllProgress);
                    return false;
                }
            }

            Logging($"総データ件数：{iDataCount}");
            LoggingEnd(buttonMakeLip.Text);
            ProgressEnd(progressBar2, labelAllProgress);
            return true;
        }

        private async Task<bool> MakeFuzAsync(string _wavfilepath = "", string _output = "", bool _isdelete = false)
        {

            int getcountFunc(string _wavPath)
                => Directory.GetFiles(_wavPath, @"*.wav").Length;

            async Task<bool> makeFunc(string _wavPath)
            {

                string strTemporaryPath = $@"{_wavPath}\_xwm";

                string[] strArrWavFiles = Directory.GetFiles(_wavPath, @"*.wav");
                if (strArrWavFiles.Length == 0)
                {
                    Logging($"選択されたフォルダ：{_wavPath}にWAVファイルがありません", true);
                    return true;
                }

                string strOutputPath;
                if (_output == string.Empty)
                {
                    strOutputPath = $@"{textBoxBatch.Text}\{Path.GetFileName(_wavPath)}";
                    if (strOutputPath == string.Empty) { return false; }
                }
                else
                {
                    strOutputPath = $@"{_output}\{Path.GetFileName(_wavPath)}";
                }

                ProgressStart($"{_wavPath}", 0, strArrWavFiles.Length);

                MakeFuzData[] makeData = MakeData<MakeFuzData>(strArrWavFiles, chunk =>
                {
                    MakeFuzData[] makeData = new MakeFuzData[chunk.Length];

                    for (int i = 0; i < chunk.Length; i++)
                    {
                        makeData[i].strArrWavFiles = chunk[i].ToArray();
                        makeData[i].strOutPutPath = strOutputPath;
                        makeData[i].strWMAEnc = $@"{textBoxYakitoriPath.Text}\{xWMAEncode}";
                        makeData[i].strFuzEnc = $@"{textBoxYakitoriPath.Text}\{BmlFuzEncode}";
                        makeData[i].strTemporaryPath = strTemporaryPath;
                        makeData[i].isdelete = _isdelete;
                    }
                    return makeData;
                });
                int[] iCount = new int[makeData.Length];

                CancelToken = new CancellationTokenSource();
                await Task.Run(() => Parallel.For(0, makeData.Length, (i) => iCount[i] = SubThreadMakeFuz(makeData[i], ToolStripMenuItemIsSkipMode.Checked)));

                if (Directory.Exists(strTemporaryPath)) { Directory.Delete(strTemporaryPath, true); }

                ProgressProcPlus(progressBar2, labelAllProgress, iCount.Sum());

                Logging($"データ件数：{iCount.Sum()}"
                    + (CancelToken.Token.IsCancellationRequested ? "" : $"  エラー件数：{strArrWavFiles.Length - iCount.Sum()}"));
                Logging($"出力先：{strOutputPath}", true);
                Logging('=');
                ProgressEnd();

                return !CancelToken.Token.IsCancellationRequested;
            }

            if (!IsInstallYakitori(true))
            {
                return false;
            }

            if (!CheckOutputPath()) { return false; }

            string strWavPath = OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください", _wavfilepath);
            if (strWavPath == string.Empty) { return false; }

            LoggingStart(buttonMakeFuz.Text);
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);
            listBoxLog.Update();

            int iDataCount = getcountFunc(strWavPath);
            var strArrDir = Directory.GetDirectories(strWavPath);
            foreach (var strDir in strArrDir)
            {
                iDataCount += getcountFunc(strDir);
            }

            ProgressStart(progressBar2, labelAllProgress, $"{buttonMakeFuz.Text} : 全体の進捗", 0, iDataCount);

            if (!await makeFunc(strWavPath))
            {
                Logging("処理がキャンセルされました");
                Logging('=');
                ProgressEnd(progressBar2, labelAllProgress);
                return false;
            }

            foreach (var strDir in strArrDir)
            {

                if (!await makeFunc(strDir))
                {
                    Logging("処理がキャンセルされました");
                    Logging('=');
                    ProgressEnd(progressBar2, labelAllProgress);
                    return false;
                }
            }
            Logging($"総データ件数：{iDataCount}");
            LoggingEnd(buttonMakeFuz.Text);
            ProgressEnd(progressBar2, labelAllProgress);
            return true;
        }

        private static T[] MakeData<T>(string[] _arrWav, Func<IEnumerable<string>[], T[]> _func)
        {
            const int iSplitCountSetting = 3;
            int iSplitCount = _arrWav.Length >= iSplitCountSetting ? iSplitCountSetting : 1;
            int ichunkSize = (_arrWav.Length / iSplitCount) + (_arrWav.Length % iSplitCount);

            var chunkWavFiles = _arrWav.Select((v, i) => new { v, i })
                .GroupBy(x => x.i / ichunkSize)
                .Select(g => g.Select(x => x.v)).ToArray();

            return _func(chunkWavFiles);
        }

        private static void DelXmlSpace(string _path, string _out)
        {
            XElement xml = XElement.Load(_path);
            var dests = xml.Elements("Content").Elements("String").Elements("Dest");

            foreach (var dest in dests)
            {
                dest.Value = dest.Value.Replace(" ", string.Empty);
            }

            string newpath = $@"{_out}\{Path.GetFileName(_path).Replace(".xml", "_nospace.xml")}";
            xml.Save(newpath);
        }

        private static Dictionary<string, string> XmltoJson(string _path)
        {
            var dict = new Dictionary<string, string>();

            XElement xml = XElement.Load(_path);
            var descendants = xml.Descendants("String");
            foreach (var descendant in descendants)
            {
                if (descendant?.Element("REC")?.Value is "INFO:RNAM" or "DIAL:FULL")
                {
                    try
                    {
                        //英語
                        string? strSource = descendant?.Element("Source")?.Value;
                        if (strSource is null) continue;

                        strSource = Regex.Replace(strSource, @"\(.*\)", string.Empty);
                        strSource = strSource.Trim();
                        strSource = strSource.Replace(" ", "_");
                        strSource = strSource.Replace("?", "_");
                        strSource = strSource.Replace("!", "_");
                        strSource = strSource.Replace("\"", "_");
                        strSource = Regex.Replace(strSource, @"\._$", ".");
                        strSource = Regex.Replace(strSource, @"__$", "_");
                        strSource = Regex.Replace(strSource, @"[<>]", "_");

                        //日本語
                        string? strDest = descendant?.Element("Dest")?.Value.Trim();
                        if (strDest is null) continue;
                        strDest = strDest.Replace(" ", "_");

                        dict.Add(strDest, strSource);
                    }
                    catch (ArgumentNullException) { break; }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            return dict;
        }

        private async void ToolStripMenuXmltoJson_Click(object sender, EventArgs e)
        {
            if (!CheckOutputPath()) { return; }

            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "XMLファイル(*.xml)|*.xml",
                FilterIndex = 1,
                Title = "翻訳用XMLファイルを選択してください(複数可)",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) { return; }

            string strMessage = $"{ofd.FileNames.Length}件のXMLファイルが選択されました"
                + "\n\n上記ファイルを読み込みDBVO用の辞書ファイル(JSON)を作成します\nよろしいですか？"
                + "WORD_OUTPUT";

            if (MessageBox.Show(GetMessageOutput(strMessage, textBoxBatch.Text),
                ToolStripMenuXmltoJson.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            var outputFolder = OpenFolderDialog("辞書ファイル(JSON)の出力先を指定してください", textBoxBatch.Text);
            if (outputFolder == string.Empty) { return; }

            EnabledButtons(false, false);

            LoggingStart(ToolStripMenuXmltoJson.Text);
            Logging($"件数によっては処理に時間がかかるためしばらくお待ち下さい", true);

            foreach (var strXmlPath in ofd.FileNames)
            {
                string strXmlFile = Path.GetFileName(strXmlPath);
                string strJsonFile = strXmlFile[..strXmlFile.IndexOf("_english")].ToLower() + ".json";
                string strOutputPath = $@"{outputFolder}\{strJsonFile}";

                try
                {
                    Logging("XMLファイル：" + strXmlPath);

                    var dicJson = await Task.Run(() => XmltoJson(strXmlPath));
                    if (dicJson.Count > 0)
                    {
                        string str = JsonConvert.SerializeObject(dicJson, Formatting.Indented);
                        File.WriteAllText(strOutputPath, str);
                        listBoxLog.Items.AddRange(dicJson.Select((s, i) => $"{i:D3} {s.Key}：{s.Value}").ToArray());
                    }

                    Logging($"データ件数：{dicJson.Count}", true);
                    Logging("出力先：" + strOutputPath);
                    Logging();
                    listBoxLog.Update();
                }
                catch (Exception ex)
                {
                    Logging($"ERROR!：{ex.Message}");
                }

            }

            LoggingEnd(ToolStripMenuXmltoJson.Text);
            EnabledButtons(true, false);

            ShowEndProcMessage(true, ToolStripMenuXmltoJson.Text);
        }

        private async Task ButtonMakeProc(Func<Task<bool>> _action, string _title)
        {
            if (await _action() && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", _title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            DisposeCancelToken();
        }

        private async void ButtonMakeWav_Click(object sender, EventArgs e)
        => await ButtonMakeProc(() => MakeWavAsync(string.Empty), buttonMakeWav.Text);

        //WAV→LIPファイル作成
        private async void ButtonMakeLip_Click(object sender, EventArgs e)
         => await ButtonMakeProc(() => MakeLipAsync(), buttonMakeLip.Text);

        //WAV→FUZへの変換
        private async void ButtonMakeFuz_Click(object sender, EventArgs e)
         => await ButtonMakeProc(() => MakeFuzAsync(), buttonMakeFuz.Text);

        private static bool IsJapanese(string text)
            => Regex.IsMatch(text, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+");

        private static void Logging(ListBox _listBox, Action _action, bool _isScroll, bool _isLog)
        {
            if (!_isLog) { return; }

            _action();

            if (_isScroll)
            {
                _listBox.TopIndex = _listBox.Items.Count - 1;
            }
        }

        private void Logging(char chr = '-', bool isScroll = false, bool _isLog = true)
            => Logging(listBoxLog, () => listBoxLog.Items.Add(new String(chr, 80)), isScroll, _isLog);

        private void Logging(string log, bool isScroll = false, bool _isLog = true)
            => Logging(listBoxLog, () => listBoxLog.Items.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{log}"), isScroll, _isLog);

        private void Logging(string[] logs, bool isScroll = false, bool _isLog = true)
            => Logging(listBoxLog, () => listBoxLog.Items.AddRange(logs), isScroll, _isLog);

        private void TextBoxYakitoriPath_TextChanged(object sender, EventArgs e) => CheckInstall(IsInstallYakitori, labelYakitori);

        private void ButtonYakitoriOpen_Click(object sender, EventArgs e)
             => SetFolder("Yakitori Audio Converterのインストール先のフォルダを選択してください", textBoxYakitoriPath);

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) => SaveSettings();

        private void TextBoxFaceFXPath_TextChanged(object sender, EventArgs e) => CheckInstall(IsInstallFaceFX, labelFaceFX);

        private void CheckInstall(Func<bool, bool> _isInsall, Label _label)
        {
            if (!_isInsall(true))
            {
                _label.Text = "NG";
                _label.BackColor = Color.Red;
            }
            else
            {
                _label.Text = "OK";
                _label.BackColor = Color.Green;

                EnabledButtons(true, false);
            }
        }

        private void ButtonFaceFXOpen_Click(object sender, EventArgs e)
            => SetFolder("FaceFXWrapperのインストール先のフォルダを選択してください", textBoxFaceFXPath);

        private async void ButtonBatch_Click(object sender, EventArgs e)
        {
            string strOutputBatch = textBoxBatch.Text;
            string strWavPath = OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください", string.Empty);
            if (strWavPath == string.Empty) { return; }

            bool isNotCancel = true;

            if (isNotCancel)
            {
                isNotCancel = await MakeLipAsync(strWavPath, strWavPath);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeFuzAsync(strWavPath, strOutputBatch, checkBoxBatch.Checked);
                listBoxLog.Update();
            }

            DisposeCancelToken();

            LoggingEnd(buttonBatch.Text);
            Logging($"フォルダ：{strOutputBatch}をご確認ください", true);
            listBoxLog.Update();

            ShowEndProcMessage(isNotCancel, buttonBatch.Text);
        }

        private void ButtonMakePack_Click(object sender, EventArgs e)
        {
            FormVoicePack formVP = new();
            formVP.ShowDialog(this);
        }

        private void ComboBoxPreset_SelectedIndexChanged(object? sender, EventArgs? e) => EnabledButtons(true, false);

        private async void ButtonGetCharList_Click(object sender, EventArgs e)
        {
            VoiceFunction? voiceFunc = GetVoicesoft();
            if (voiceFunc is null) { return; }

            if (voiceFunc.GetType() == typeof(StyleBertVITS2Function))
            {
                await GetCharactorAsync(voiceFunc, (spkrs, list) =>
                {
                    var items = JObject.Parse(spkrs);
                    foreach (var item in items)
                    {
                        var Infos = (JObject)items[item.Key];
                        var styles = (JObject)Infos["style2id"];
                        foreach (var style in styles)
                        {
                            Logging($"[{item.Key}]:{Infos["id2spk"]["0"]}:{style.Key}");
                            list.Add(new ItemSet(Convert.ToInt32(item.Key), $"{Infos["id2spk"]["0"]}:{style.Key}"));
                        }
                    }
                });
            }
            else
            {
                bool isResult = await GetCharactorAsync(voiceFunc, (spkrs, list) =>
                {
                    var arrayitems = JArray.Parse(spkrs);
                    foreach (var arrayitem in arrayitems)
                    {
                        var items = (JObject)arrayitem;
                        var styles = (JArray)items[voiceFunc.ColumNameStyles];
                        foreach (var style in styles)
                        {
                            Logging($"[{style[voiceFunc.ColumNameStyleId]}]:{items[voiceFunc.ColumNameSpeakerName]}:{style[voiceFunc.ColumNameStyleName]}", true);
                            list.Add(
                                new ItemSet((int)style[voiceFunc.ColumNameStyleId],
                                $"{items[voiceFunc.ColumNameSpeakerName]}:{style[voiceFunc.ColumNameStyleName]}"));
                        }
                    }
                });

                if (!isResult)
                {
                    bool startVoiceSoft(string _exePath)
                    {
                        try
                        {
                            var proc = Process.Start(_exePath);
                            proc?.WaitForInputIdle();
                            Logging($"{voiceFunc.Name}を起動しました。再度、キャラ取得ボタンを押してください", true);
                            return true;
                        }
                            catch (FileNotFoundException e)
                        {

                        }
                            catch (Exception e)
                        {

                        }
                        return false;
                    }

                    bool isSuccess = false;

                    if(voiceFunc.GetType() == typeof(VoicevoxFunction) && textBoxVoicevoxPath.Text.Length > 0)
                    {
                        isSuccess = startVoiceSoft($@"{textBoxVoicevoxPath.Text}\{voiceFunc.ExeName}");
                    }
                    else if (voiceFunc.GetType() == typeof(CoeiroinkFunction) && textBoxCoeiroinkPath.Text.Length > 0)
                    {
                        isSuccess = startVoiceSoft($@"{textBoxCoeiroinkPath.Text}\{voiceFunc.ExeName}");
                    }

                    if (!isSuccess)
                    {
                        Logging($"{voiceFunc.Name}を検索しています…少々お待ちください", true);
                        listBoxLog.Update();
                        string? strVoiceFunc = FindFile(voiceFunc.ExeName);
                        if (strVoiceFunc is not null)
                        {
                            startVoiceSoft($@"{strVoiceFunc}\{voiceFunc.ExeName}");
                            if (voiceFunc.GetType() == typeof(VoicevoxFunction)) textBoxVoicevoxPath.Text = strVoiceFunc;
                            else if (voiceFunc.GetType() == typeof(CoeiroinkFunction)) textBoxCoeiroinkPath.Text = strVoiceFunc;
                        }
                        else
                        {
                            Logging($"{voiceFunc.Name}の検索に失敗しました。インストールされていることを確認してください");
                            Logging("インストールされている場合、手動で起動してください", true);
                        }
                    }
                }
            }
        }

        private async Task<bool> GetCharactorAsync(VoiceFunction _voiceFunv, Action<string, List<ItemSet>> _MakeSpeakerList)
        {
            buttonGetChar.Enabled = false;
            var strLabel = buttonGetChar.Text;
            buttonGetChar.Text = "取得中…";

            void RestoreButton()
            {
                buttonGetChar.Text = strLabel;
                buttonGetChar.Enabled = true;
            }

            Logging($"{comboBoxVoiceSoft.Text}のキャラクター一覧を取得します", true);

            var listSrc = new List<ItemSet>();

            try
            {
                string speakers = await _voiceFunv.GetSpeakers();

                if (speakers is null || speakers == string.Empty)
                {
                    Logging($"キャラクター一覧の取得に失敗しました。", true);
                    RestoreButton();
                    return false;
                }

                _MakeSpeakerList(speakers, listSrc);
            }
            catch { }

            if (listSrc.Count == 0)
            {
                Logging($"キャラクター一覧の取得に失敗しました。", true);
                RestoreButton();
                return false;
            }
            else
            {
                comboBoxPreset.DataSource = null;
                comboBoxPreset.DataSource = listSrc;
                comboBoxPreset.DisplayMember = "ItemDisp";
                comboBoxPreset.ValueMember = "ItemValue";

                comboBoxPreset.SelectedIndex = 0;
                ComboBoxPreset_SelectedIndexChanged(null, null);
            }

            if (_voiceFunv.GetType() == typeof(StyleBertVITS2Function))
            {
                Logging($"※このボイスソフトではボイス調整は話速のみ適用され、値を上げると話速が遅くなります", true);
            }
            Logging('-', true);

            await Task.Delay(10000);
            RestoreButton();
            GC.Collect();

            return true;
        }

        private async void ButtonSample_Click(object sender, EventArgs e)
        {
            await ReadDefaultDictionary(true, false, false);
            await PlaySample();
        }

        private static void SetFolder(string _msg, TextBox _txtBox)
        {
            string str = OpenFolderDialog(_msg, string.Empty);
            if (str != string.Empty) { _txtBox.Text = str; }
        }

        private void ButtonBatchFolder_Click(object sender, EventArgs e)
            => SetFolder("一括処理の出力先フォルダを指定してください", textBoxBatch);

        private void TextBoxBatch_TextChanged(object sender, EventArgs e) => EnabledButtons(true, false);

        private void ButtonVoiceControl_Click(object sender, EventArgs e)
        {
            FormVoiceControl f = new(this)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            f.ShowDialog(this);
            f.Dispose();
        }

        private void ComboBoxVoiceSoft_TextChanged(object sender, EventArgs e)
        {
            comboBoxPreset.DataSource = null;
            comboBoxPreset.Items.Add("");
            comboBoxPreset.Items.Clear();
        }

        private void ButtonProcCancel_Click(object sender, EventArgs e)
        {
            if (CancelToken is not null)
            {
                CancelToken.Cancel();
            }
        }

        private void ButtonLogSave_Click(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new()
            {
                FileName = "DBVOJpToolLog.txt",
                InitialDirectory = "",
                Filter = "テキストファイル(*.txt)|*.txt",
                FilterIndex = 1,
                Title = "ログファイルの出力先を指定してください",
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (sfd.ShowDialog() != DialogResult.OK) { return; }

            StreamWriter SaveFile = new(sfd.FileName);
            foreach (var item in listBoxLog.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            //SaveFile.WriteLine(listBoxLog.Items.ToString());
            //SaveFile.ToString();
            SaveFile.Close();
        }

        private async void ToolStripMenuDelXmlSpace_Click(object sender, EventArgs e)
        {
            if (!CheckOutputPath()) { return; }

            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "XMLファイル(*.xml)|*.xml",
                FilterIndex = 1,
                Title = "翻訳用XMLファイルを選択してください(複数可)",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) { return; }

            string strMessage = $"{ofd.FileNames.Length}件のXMLファイルが選択されました"
                + "\n\n上記ファイルを読み込み日本語訳の文章から半角スペースを削除します\nよろしいですか？"
                + "WORD_OUTPUT";

            if (MessageBox.Show(GetMessageOutput(strMessage, textBoxBatch.Text),
                toolStripMenuDelXmlSpace.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            var outputFolder = OpenFolderDialog("変更後のXMLファイルの出力先を指定してください", textBoxBatch.Text);
            if (outputFolder == string.Empty) { return; }

            EnabledButtons(false, false);

            LoggingStart(toolStripMenuDelXmlSpace.Text);
            Logging($"件数によっては処理に時間がかかるためしばらくお待ち下さい", true);

            foreach (var strXmlPath in ofd.FileNames)
            {
                string strXmlFile = Path.GetFileName(strXmlPath);

                try
                {
                    Logging("XMLファイル：" + strXmlPath);

                    await Task.Run(() => DelXmlSpace(strXmlPath, outputFolder));

                    Logging($@"変更後ファイル名：{outputFolder}\{Path.GetFileName(strXmlPath).Replace(".xml", "_nospace.xml")}");
                    Logging();
                    listBoxLog.Update();
                }
                catch (Exception ex)
                {
                    Logging($"ERROR!：{ex.Message}");
                }

            }

            LoggingEnd(toolStripMenuDelXmlSpace.Text);
            EnabledButtons(true, false);

            ShowEndProcMessage(true, toolStripMenuDelXmlSpace.Text);
        }

        private static string GetMessageOutput(string _msg, string _outPath)
        {
            if (_outPath == string.Empty)
            {
                _msg = _msg.Replace("WORD_OUTPUT", "\n※「はい」の場合、続けてファイル出力先を決定します");
            }
            else
            {
                _msg = _msg.Replace("WORD_OUTPUT", $"\n出力先：{_outPath}");
            }
            _msg += "\n※同名のフォルダやファイルが存在する場合、上書きされます";

            return _msg;
        }

        private async void ToolStripMenuMakeWavFromText_Click(object sender, EventArgs e)
        {
            if (!CheckOutputPath()) { return; }

            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "テキストファイル(*.txt;*.csv)|*.txt;*.csv",
                FilterIndex = 1,
                Title = "音声(WAV)を作成するためのテキストファイルを選択してください(複数可)",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) { return; }

            string outputFolder = textBoxBatch.Text;

            if (MessageBox.Show(GetMessageOutput($"{ofd.FileNames.Length}件のテキストファイルが選択されました"
                    + "\n\n上記ファイルを読み込み音声(WAV)を作成します\nよろしいですか？WORD_OUTPUT", outputFolder),
                    ToolStripMenuMakeWavFromText.Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            if ((outputFolder = OpenFolderDialog("音声(WAV)ファイルの出力先を指定してください", outputFolder)) == string.Empty)
            {
                return;
            }

            EnabledButtons(false, false);

            await ReadDefaultDictionary(true, false, true);

            LoggingStart(ToolStripMenuMakeWavFromText.Text);
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);

            CancelToken = new CancellationTokenSource();

            foreach (var txtPath in ofd.FileNames)
            {
                if (CancelToken.Token.IsCancellationRequested) { break; }

                string strTxtFile = Path.GetFileName(txtPath);

                Logging($"テキストファイル：{strTxtFile}", true);
                Logging($"◆◆テキストファイルの書式チェック・・・");

                var txtAllData = File.ReadAllLines(txtPath);
                if (txtAllData is null || txtAllData.Length == 0)
                {
                    Logging($"テキストファイルにデータがありませんでした");
                    break;
                }

                List<MakeWavData> listWavData = new(txtAllData.Length);
                int iCounter = 0;
                int iErrCounter = 0;

                foreach (var txtData in txtAllData)
                {
                    var txtDataSplit = txtData.Split('|');
                    for (int n = 0; n < txtDataSplit.Length; ++n)
                    {
                        txtDataSplit[n] = txtDataSplit[n].Trim('"').Trim(); // 先頭と最後尾の '"' を削除
                    }

                    ++iCounter;

                    if (txtDataSplit.Length < 3)
                    {
                        Logging($"{iCounter:D3}：設定項目不足のためスキップ…カンマではなくパイプ区切りなので注意してください");
                        iErrCounter++;
                        continue;
                    }
                    if (!int.TryParse(txtDataSplit[0], out int iVoicesoft))
                    {
                        Logging($"{iCounter:D3}：ボイスソフトの設定が間違っているためスキップされます");
                        iErrCounter++;
                        continue;
                    }

                    VoiceFunction? voiceFunc = GetVoicesoft(iVoicesoft);
                    if (voiceFunc is null)
                    {
                        Logging($"{iCounter:D3}：ボイスソフトの設定が間違っているためスキップされます");
                        iErrCounter++;
                        continue;
                    }

                    if (!int.TryParse(txtDataSplit[1], out int iSpeaker))
                    {
                        Logging($"{iCounter:D3}：キャラクターの設定が間違っているためスキップされます");
                        iErrCounter++;
                        continue;
                    }

                    MakeWavData wavData = new()
                    {
                        voiceFunc = voiceFunc,
                        speaker = iSpeaker,
                        JPWord = txtDataSplit[2],
                        EnWord = txtDataSplit.Length >= 4 && txtDataSplit[3] != string.Empty ? txtDataSplit[3] : $"{iCounter:D3}_{voiceFunc.Name}_{iSpeaker}",
                        style = txtDataSplit.Length >= 5 ? txtDataSplit[4] : "",
                        strOutPutPath = (txtDataSplit.Length >= 6 && txtDataSplit[5] != string.Empty) ? outputFolder + "\\" + txtDataSplit[5] : outputFolder,
                        dParams = txtDataSplit.Length >= 7 ? GetVoiceParams(txtDataSplit[6]) : GetVoiceParams("1,0,1,1")
                    };

                    listWavData.Add(wavData);
                    Logging($"{wavData.voiceFunc.Name}|{wavData.speaker}|{wavData.JPWord}|{wavData.EnWord}|{wavData.style}|{wavData.strOutPutPath}|{String.Join(@",", wavData.dParams)}");
                }

                Logging($"◆データ件数：{iCounter}  スキップ件数：{iErrCounter}", true);
                Logging();
                Logging($"◆◆音声ファイルを作成します・・・", true);
                ProgressStart("テキストファイルから音声ファイル作成中", 0, listWavData.Count);

                await Task.Run(() => SubThreadMakeWav(listWavData.ToArray(), ToolStripMenuItemIsSkipMode.Checked));
            }

            LoggingEnd(ToolStripMenuMakeWavFromText.Text);
            Logging($"出力フォルダ：{outputFolder}", true);
            ProgressEnd();

            EnabledButtons(true, false);

            ShowEndProcMessage(!CancelToken.Token.IsCancellationRequested, ToolStripMenuMakeWavFromText.Text);
        }

        private void ListBoxLog_Leave(object sender, EventArgs e) => listBoxLog.Update();

        private void DisposeCancelToken()
        {
            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }
        }

        private void ShowEndProcMessage(bool _isNotCancel, string _title)
        {
            if (_isNotCancel && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", _title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void ButtonBatchAll_Click(object sender, EventArgs e)
        {
            string strOutputBatch = textBoxBatch.Text;
            bool isNotCancel = true;

            if (isNotCancel)
            {
                isNotCancel = await MakeWavAsync(strOutputBatch);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeLipAsync(strOutputBatch, strOutputBatch);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeFuzAsync(strOutputBatch, strOutputBatch, checkBoxBatch.Checked);
                listBoxLog.Update();
            }

            DisposeCancelToken();

            if (isNotCancel)
            {
                LoggingEnd(buttonBatchAll.Text);
                Logging($"フォルダ：{strOutputBatch}をご確認ください", true);
                listBoxLog.Update();
            }

            ShowEndProcMessage(isNotCancel, buttonBatchAll.Text);
        }

        private List<string> ReadFile(string _filePath)
        {
            var list = new List<string>();
            try
            {
                string? strData = string.Empty;

                using FileStream fStrean = new(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using TextReader tReader = new StreamReader(fStrean, Encoding.UTF8);

                while ((strData = tReader.ReadLine()) is not null)
                {
                    list.Add(strData);
                }
                if (list.Count == 0)
                {
                    Logging($"テキストファイルにデータがありませんでした");
                    Logging('!', true);
                }
            }
            catch (IOException)
            {
                Logging($"ファイルが使用中のため開くことができません");
                Logging('!', true);
            }
            return list;
        }

        /// <summary>
        /// 読み＆アクセント辞書を読み込んで音声合成ソフトに登録します
        /// </summary>
        /// <param name="_path">辞書のパス</param>
        /// <param name="_isLog">ログを出力するか</param>
        /// <param name="_isTextMode">trueの場合、全ての音声合成ソフトの辞書に登録：falseの場合、選択されているソフトのみ</param>
        /// <returns></returns>
        private async Task ReadDictionary(string _path, bool _isLog, bool _isTextMode)
        {
            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "テキストファイル(*.txt;*.csv)|*.txt;*.csv",
                FilterIndex = 1,
                Title = "読み&アクセント辞書に登録するテキストファイルを選択してください",
                RestoreDirectory = false,
                Multiselect = false
            };

            string strDicPath;
            if (_path == string.Empty)
            {
                if (ofd.ShowDialog() != DialogResult.OK) { return; }
                strDicPath = ofd.FileName;
                LoggingStart($"読み&アクセント辞書の読み込み", _isLog);
            }
            else
            {
                strDicPath = _path;
                LoggingStart($"デフォルトの読み&アクセント辞書の読み込み", _isLog);
            }

            EnabledButtons(false, false);

            Logging($"◆◆ファイルの書式チェック・・・", false, _isLog);
            var txtAllData = ReadFile(strDicPath);

            var listData = new List<VoiceDictionary>(txtAllData.Count);
            int iCounter = 0;
            int iErrCounter = 0;

            foreach (var txtData in txtAllData)
            {
                var txtDataSplit = txtData.Split(',');
                for (int n = 0; n < txtDataSplit.Length; ++n)
                {
                    txtDataSplit[n] = txtDataSplit[n].Trim('"').Trim(); // 先頭と最後尾の '"' を削除
                }

                ++iCounter;

                if (txtDataSplit.Length < 3)
                {
                    Logging($"{iCounter:D3}：設定項目が不足しています「テキスト,読み,アクセント,音数」", false, _isLog);
                    iErrCounter++;
                    continue;
                }

                if (!int.TryParse(txtDataSplit[2], out int iAccent))
                {
                    Logging($"{iCounter:D3}：3列目の項目(アクセント)が間違っているためスキップされます", false, _isLog);
                    iErrCounter++;
                    continue;
                }

                if (txtDataSplit.Length >= 4 && !int.TryParse(txtDataSplit[3], out int iMoranum))
                {
                    Logging($"{iCounter:D3}：4列目の項目(音数)が間違っているためスキップされます", false, _isLog);
                    iErrCounter++;
                    continue;
                }
                VoiceDictionary voiceDict = new()
                {
                    Text = txtDataSplit[0],
                    Yomi = txtDataSplit[1],
                    Accent = int.Parse(txtDataSplit[2]),
                    Moranum = txtDataSplit.Length >= 4 ? int.Parse(txtDataSplit[3]) : -1,
                };

                listData.Add(voiceDict);
                Logging($"{iCounter:D3}：{voiceDict.Text},{voiceDict.Yomi},{voiceDict.Accent},{voiceDict.Moranum}", false, _isLog);
            }

            Logging($"◆データ件数：{iCounter}  スキップ件数：{iErrCounter}", false, _isLog);
            Logging('-', true, _isLog);

            async Task SetDictionary(VoiceFunction vfunc)
            {
                Logging($"◆◆{vfunc.Name}：辞書を登録しています・・・", true, _isLog);

                var resCoe = await vfunc.SetDictionary(listData);
                if (resCoe is null)
                {
                    Logging($"{vfunc.Name}：処理に失敗しました");
                }
                else if (resCoe == HttpStatusCode.RequestTimeout)
                {
                    Logging($"{vfunc.Name}：起動していないため登録をスキップします");
                }
                else if (resCoe != HttpStatusCode.OK)
                {
                    Logging($"{vfunc.Name}：処理に失敗しました：{resCoe}");
                }
            }

            if (_isTextMode || ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue == (int)VoiceSoft.Coeiroink)
            {
                await SetDictionary(new CoeiroinkFunction());
            }

            if (_isTextMode ||
                ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue == (int)VoiceSoft.Voicevox ||
                ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue == (int)VoiceSoft.Nemo ||
                ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue == (int)VoiceSoft.Itvoice ||
                ((ItemSet)comboBoxVoiceSoft.SelectedItem).ItemValue == (int)VoiceSoft.Sharevoice
                )
            {
                await SetDictionary(new VoicevoxFunction());
            }

            LoggingEnd("読み&アクセント辞書の読み込み", _isLog);
            EnabledButtons(true, false);
        }

        private async void ToolStripMenuReadDicDefault_Click(object sender, EventArgs e)
            => await ReadDefaultDictionary(false, true, true);

        private async void ToolStripMenuReadDicTxt_Click(object sender, EventArgs e)
            => await ReadDictionary("", true, true);

        private void ButtonLogClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ログを全て消去します。よろしいですか？",
                buttonLogClear.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                listBoxLog.Items.Clear();
            }
        }

        private void ToolStripMenuOpenDictDef_Click(object sender, EventArgs e)
        {
            if (File.Exists(DefaultDictionary))
            {
                var proc = new Process();
                proc.StartInfo.FileName = DefaultDictionary;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            else
            {
                Logging("デフォルト辞書がありませんでした");
                Logging("起動時に辞書を読み込みたい場合はexeと同じフォルダに「DefaultDictionary.csv」を配置してください", true);
            }
        }

        private void ToolStripMenuOpenDictOther_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new()
            {
                FileName = "",
                InitialDirectory = "",
                Filter = "テキストファイル(*.txt;*.csv)|*.txt;*.csv",
                FilterIndex = 1,
                Title = "読み&アクセント辞書を選択してください",
                RestoreDirectory = false,
                Multiselect = false
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var proc = new Process();
                proc.StartInfo.FileName = ofd.FileName;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

        private async void ButtonSampleText_Click(object sender, EventArgs e)
        {
            string strText;

            if ((strText = Interaction.InputBox(
                "選択されたキャラクターに喋らせたい文字列を入力してください"
                , "文字列を指定してサンプル再生", textBoxSample.Text, Left + 200, Top + 100).Trim()) == string.Empty)
            { return; }
            textBoxSample.Text = strText;

            await ReadDefaultDictionary(true, false, false);
            await PlaySample(strText);
        }

        private void ButtonFaceFXFind_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "FaceFXWrapperを自動検出します。\n環境により時間がかかる可能性があります。\n実行しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string? strPath = FindFile(new string[] { FaceFXWrapper, FonixData });
                if (strPath is not null) { textBoxFaceFXPath.Text = strPath; }
            }
        }

            private void ButtonYakitoriFind_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Yakitori Audio Converterを自動検出します。\n環境により時間がかかる可能性があります。\n実行しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string? strPath = FindFile(new string[] { xWMAEncode, BmlFuzEncode });
                if (strPath is not null) { textBoxYakitoriPath.Text = strPath; }
            }
        }
    }

    public class ItemSet
    {
        // DisplayMemberとValueMemberにはプロパティで指定する仕組み
        public string ItemDisp { get; set; }
        public int ItemValue { get; set; }

        // プロパティをコンストラクタでセット
        public ItemSet(int v, string s)
        {
            ItemValue = v;
            ItemDisp = s;
        }

        public ItemSet(string v, string s)
        {
            ItemValue = int.Parse(v);
            ItemDisp = s;
        }
    }
}
