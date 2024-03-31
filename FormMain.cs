using DBVO_JPVoice_Tool;
using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DBVO_JPVoice_Tool
{
    public enum VoiceSoft
    {
        Voicevox,
        Coeiroink,
        Nemo,
        Sharevoice,
        Itvoice,
        StyleBertVITS2 = 100
    }

    public partial class FormMain : Form
    {
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

        private struct MakeLipData
        {
            public string[] strArrWavFiles;
            public string strOutPutPath;
            public string strTemporaryPath;
            public string strFaceFXWrapper;
            public string strFonixData;
        }

        private struct MakeFuzData
        {
            public string[] strArrWavFiles;
            public string strOutPutPath;
            public string strTemporaryPath;
            public string strWMAEnc;
            public string strFuzEnc;
            public bool isbatch;
            public bool isdelete;
        }

        private CancellationTokenSource? CancelToken;

        private const string FaceFXWrapper = "FaceFXWrapper.exe";
        private const string FonixData = "FonixData.cdf";

        private const string xWMAEncode = "xWMAEncode.exe";
        private const string BmlFuzEncode = "BmlFuzEncode.exe";

        private const string DefaultDictionary = @".\DefaultDictionary.csv";

        private const int ListBuffer = 10;

        private void EnabledButtons(bool _b, bool _enableCancel)
        {
            buttonProcCancel.Visible = _enableCancel;

            buttonBatchFolder.Enabled = _b;
            buttonFaceFXOpen.Enabled = _b;
            buttonGetChar.Enabled = _b;
            buttonMakeFuz.Enabled = _b && IsInstallYakitori();
            buttonMakeLip.Enabled = _b && IsInstallFaceFX();
            buttonMakePack.Enabled = _b;
            buttonMakeWav.Enabled = _b && comboBoxPreset.SelectedIndex != -1;
            buttonSample.Enabled = _b && buttonMakeWav.Enabled;
            buttonVoiceControl.Enabled = _b;
            //buttonXmltoJson.Enabled = _b;
            buttonYakitoriOpen.Enabled = _b;

            buttonBatch.Enabled = _b && buttonMakeLip.Enabled && buttonMakeFuz.Enabled && textBoxBatch.Text.Length > 0;
            buttonBatchAll.Enabled = _b && buttonMakeWav.Enabled && buttonBatch.Enabled;

            ToolStripMenuTool.Enabled = _b;
            ToolStripMenuReadDctionary.Enabled = _b;
        }

        private void ProgressStart(string _text, int _min, int _max)
        {
            labelProgress.Visible = true;
            progressBar1.Visible = true;

            progressBar1.Minimum = _min;
            progressBar1.Maximum = _max;
            progressBar1.Value = _min;

            labelProgress.Text = $"{_text}…{progressBar1.Minimum} / {progressBar1.Maximum}";

            EnabledButtons(false, true);
        }

        private void ProgressProc(int _value)
        {
            progressBar1.Value = _value;

            string str = labelProgress.Text;
            labelProgress.Text = $"{str[..str.IndexOf("…")]}…{progressBar1.Value} / {progressBar1.Maximum}";
            labelProgress.Update();
        }
        private void ProgressProcPlus(int _value = 1)
        {
            ProgressProc(progressBar1.Value + _value);
        }


        private void ProgressEnd()
        {
            buttonProcCancel.Visible = false;
            labelProgress.Visible = false;
            progressBar1.Visible = false;

            EnabledButtons(true, false);
        }

        private void LoggingStart(string _title)
        {
            Logging();
            Logging($"処理開始 《{_title}》");
            Logging();
        }

        private void LoggingEnd(string _title)
        {
            Logging($"処理終了 《{_title}》");
            Logging('#', true);
        }

        private static string OpenFolderDialog(string _title)
        {
            using CommonOpenFileDialog off = new()
            {
                Title = _title,
                IsFolderPicker = true,
                RestoreDirectory = false
            };

            if (off.ShowDialog() == CommonFileDialogResult.Cancel) { return string.Empty; }
            return off.FileName;
        }

        private bool CheckUseBatch()
        {
            if (ToolStripMenuUseBatchOutput.Checked && textBoxBatch.Text == string.Empty)
            {
                MessageBox.Show($"オプションで一括処理の出力先が有効になっているため\n一括処理のファイル出力先を選択してください",
                    "オプションの確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private VoiceFunction? GetVoicesoft(int _voiceSoft = -1)
        {
            return (_voiceSoft switch { -1 => (int)comboBoxVoiceSoft.SelectedValue, _ => _voiceSoft }) switch
            {
                (int)VoiceSoft.Voicevox => new VoicevoxFunction(),
                (int)VoiceSoft.Nemo => new VoicevoxNemoFunction(),
                (int)VoiceSoft.Sharevoice => new SharevoiceFunction(),
                (int)VoiceSoft.Itvoice => new ItvoiceFunction(),
                (int)VoiceSoft.Coeiroink => new CoeiroinkFunction(),
                (int)VoiceSoft.StyleBertVITS2 => new StyleBertVITS2Function(),
                _ => null
            };
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
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
            ToolStripMenuUseBatchOutput.Checked = ConfigurationManager.AppSettings["IsUseBatchOutput"] == "True";

            Logging('*');
            Logging($"{this.Text}");
            Logging('*');

            if (File.Exists(DefaultDictionary))
            {
                await ReadDictionary(DefaultDictionary);
            }

            Logging("音声ファイルを作成したい場合は、まず最初に「音声合成ソフト」を選択してから「キャラ取得」ボタンを押してください");
            Logging("各ボタンの上にマウスカーソルを乗っけるとヒントが表示されます", true);

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

        private async Task SubThreadMakeWav(MakeWavData[] _data)
        {
            int iCounter = 0;
            int iErrCounter = 0;

            List<string> strListLog = new(ListBuffer);

            Action<string, bool> actionLogging = this.Logging;

            foreach (var data in _data)
            {
                try
                {
                    CancelToken?.Token.ThrowIfCancellationRequested();

                    iCounter++;
                    if (iCounter != 1 && iCounter % 500 == 1) this.Invoke(() => Logging('-'));

                    string strJsonJPWord = data.JPWord.Trim();
                    string strJsonEnWord = data.EnWord.Trim();

                    if (!IsJapanese(strJsonJPWord)) { continue; }

                    strJsonJPWord = Regex.Replace(strJsonJPWord, "・", string.Empty);
                    strJsonJPWord = Regex.Replace(strJsonJPWord, @"（.*）", string.Empty);
                    strJsonJPWord = Regex.Replace(strJsonJPWord, @"\(.*\)", string.Empty);
                    strJsonJPWord = Regex.Replace(strJsonJPWord, @"_Alias.*__", string.Empty);

                    if (strJsonJPWord.Trim() == string.Empty) { continue; }

                    HttpStatusCode? statusCode = HttpStatusCode.OK;

                    if (data.voiceFunc.GetType() == typeof(StyleBertVITS2Function))
                    {
                        statusCode = await ((StyleBertVITS2Function)data.voiceFunc).MakeSound($@"{data.strOutPutPath}\{strJsonEnWord}", strJsonJPWord, data.style, data.speaker, data.dParams);
                    }
                    else
                    {
                        statusCode = await data.voiceFunc.MakeSound($@"{data.strOutPutPath}\{strJsonEnWord}", strJsonJPWord, true, data.speaker, data.dParams);
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

                    this.Invoke(() => ProgressProc(iCounter));
                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
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
                this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            this.Invoke(() => Logging($"◆データ件数：{iCounter}  エラー件数：{iErrCounter}", true));
            this.Invoke(() => Logging('-'));
        }

        private double[] GetVoiceParams(string _params = "")
        {
            double[] dParams = { 1, 0, 1, 1 };

            string strParams = _params switch { "" => _params, _ => textBoxParam.Text };
            //string strParams = _params != string.Empty ? _params : textBoxParam.Text;

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

        private async Task<bool> MakeWavAsync(string _outputpath = "")
        {

            if (comboBoxPreset.SelectedIndex == -1)
            {
                Logging($"ボイスキャラクターが選択されていません");
                return false;
            }

            if (!CheckUseBatch()) { return false; }
            _outputpath = textBoxBatch.Text;

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


            Dictionary<string, Dictionary<string, string>>? listJsondata = new();

            List<string> listDataCount = new();
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
                iTotal += dicJsondata.Count;
                listDataCount.Add($"{Path.GetFileName(jsonPath)}(データ件数：{dicJsondata.Count})");
                listJsondata.Add(jsonPath, dicJsondata);
            }

            if (iTotal == 0)
            {
                Logging($"辞書ファイルに該当するデータが１件もありません");
                Logging();
                return false;
            }

            if (_outputpath == string.Empty)
            {
                if (MessageBox.Show(
                $"{ofd.FileNames.Length}件の辞書ファイルが選択されました"
                + $"\n\n{listDataCount.Aggregate((x, y) => $"{x}\n{y}")}\n全データ件数：{iTotal}"
                + ((strSearch != string.Empty) ? $"\n指定文字列：{strSearch}" : "")
                + $"\nボイスキャラクター：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}"
                + "\n\n音声ファイルを生成します。よろしいですか？"
                + "\n\n※「はい」の場合、次のダイアログで出力先のフォルダを指定します"
                + "\n\n※既に同名のフォルダやファイルが存在する場合、上書きされますのでご注意ください",
                buttonMakeWav.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(
                $"{ofd.FileNames.Length}件の辞書ファイルが選択されました"
                + $"\n\n{listDataCount.Aggregate((x, y) => $"{x}\n{y}")}\n全データ件数：{iTotal}"
                + ((strSearch != string.Empty) ? $"\n指定文字列：{strSearch}" : "")
                + $"\nボイスキャラクター：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}"
                + "\n\n音声(WAV)、LIP生成、およびFUZ変換を一括で行います。\nよろしいですか？"
                + "\n\n※全てのファイルは、一括処理のFUZファイル出力先に出力されます"
                + $"\n出力先：{_outputpath}"
                + "\n\n※既に同名のフォルダやファイルが存在する場合、上書きされますのでご注意ください",
                buttonBatchAll.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return false;
                }
            }

            string strOutputPath = _outputpath switch
            {
                "" => OpenFolderDialog("出力先のフォルダを選択してください"),
                _ => _outputpath
            };

            if (strOutputPath == string.Empty) { return false; }

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
                string strStyle = comboBoxPreset.SelectedIndex switch { not -1 => comboBoxPreset.Text.Contains(':') ? comboBoxPreset.Text.Split(":")[2] : "", _ => "" };
                var dParams = GetVoiceParams();

                VoiceFunction? voiceFunc = GetVoicesoft();
                if (voiceFunc is null) { return false; }

                ProgressStart($"辞書ファイル({strJsonFile})==>音声ファイル作成中", 0, dicJson.Value.Count);

                MakeWavData[] listWavData = dicJson.Value
                    .Select(x => new MakeWavData()
                    {
                        JPWord = x.Key,
                        EnWord = x.Value,
                        strOutPutPath = strOutputPath,
                        voiceFunc = voiceFunc,
                        speaker = ispeaker,
                        style = strStyle,
                        dParams = dParams
                    }).ToArray();

                if(listWavData.Length > 0)
                {
                    Logging($"辞書ファイル：{strJsonFile}", true);
                    await Task.Run(() => SubThreadMakeWav(listWavData));
                }
            }

            LoggingEnd(buttonMakeWav.Text);
            Logging($"出力フォルダ：{strOutputPath}", true);
            ProgressEnd();

            return !CancelToken.Token.IsCancellationRequested;
        }

        private int SubThreadMakeLip(MakeLipData _data)
        {
            int iCounter = 0;
            List<string> strListLog = new(ListBuffer);

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

                    iCounter++;

                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{Path.GetFileName(wavfile)} ==> {Path.GetFileName(strLipPath)}");
                    this.Invoke(() => ProgressProcPlus());

                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
                        strListLog.Clear();
                    }

                }
                catch (OperationCanceledException)
                {
                    //strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇処理がキャンセルされました");
                    //strListLog.Add(new string('=', 80));
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
                this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            return iCounter;
            //this.Invoke(() => Logging($"データ件数：{iCounter}", true));
        }

        private int SubThreadMakeFuz(MakeFuzData _data)
        {
            int iCounter = 0;
            List<string> strListLog = new(ListBuffer);

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
                        //this.Invoke(() => Logging($"{iCounter:D3}：{strXwmPath}の作成に失敗しました", true));
                    }

                    pInfo.FileName = _data.strFuzEnc;
                    pInfo.Arguments = $"\"{strFuzPath}\" \"{strXwmPath}\" \"{(File.Exists(strLipPath) ? strLipPath : "")}\"";

                    var proc2 = Process.Start(pInfo);
                    proc2?.WaitForExit();

                    if (proc2?.ExitCode != 0 || !File.Exists(strFuzPath))
                    {
                        strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{strFuzPath}の作成に失敗しました");
                        continue;
                        //this.Invoke(() => Logging($"{iCounter:D3}：{strFuzPath}の作成に失敗しました", true));
                    }

                    if (_data.isbatch && _data.isdelete && File.Exists(wavfile)) { File.Delete(wavfile); }
                    if (_data.isbatch && _data.isdelete && File.Exists(strLipPath)) { File.Delete(strLipPath); }

                    iCounter++;
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{Path.GetFileName(wavfile)} ==> {Path.GetFileName(strFuzPath)}");
                    //this.Invoke(() => Logging($"{iCounter:D3}：{Path.GetFileName(wavfile)} ==> {Path.GetFileName(strFuzPath)}", true));
                    this.Invoke(() => ProgressProcPlus());

                    if (strListLog.Count % ListBuffer == 0)
                    {
                        this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
                        strListLog.Clear();
                    }

                }
                catch (OperationCanceledException)
                {
                    //strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇処理がキャンセルされました");
                    //strListLog.Add(new string('=', 80));
                    //this.Invoke(() => Logging($"処理がキャンセルされました", true));
                    //this.Invoke(() => Logging('!'));
                    break;
                }
                catch (Exception ex)
                {
                    //this.Invoke(() => Logging($"エラーが発生しました。\nファイル：{wavfile}\n{ex.Message}", true));
                    strListLog.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇エラーが発生しました。\nファイル：{wavfile}\n{ex.Message}");
                    strListLog.Add(new string('!', 80));
                    //this.Invoke(() => Logging('!'));
                    continue;
                }
            }

            if (strListLog.Count > 0)
            {
                this.Invoke(() => LoggingRange(strListLog.ToArray(), true));
                strListLog.Clear();
            }
            //this.Invoke(() => Logging($"データ件数：{iCounter}", true));
            return iCounter;
        }

        private bool IsInstallFaceFX(bool _isLog = false)
        {
            return IsInstallRequired(textBoxFaceFXPath.Text, _isLog, FaceFXWrapper, FonixData);
        }

        private bool IsInstallYakitori(bool _isLog = false)
        {
            return IsInstallRequired(textBoxYakitoriPath.Text, _isLog, xWMAEncode, BmlFuzEncode);
        }

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

        private async Task<bool> MakeLipAsync(string _wavfilepath = "", string _output = "", bool _isbatch = false)
        {
            if (!this.IsInstallFaceFX(true))
            {
                return false;
            }

            if (!CheckUseBatch()) { return false; }
            _output = textBoxBatch.Text;

            string strWavPath = _wavfilepath switch
            {
                "" => OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください"),
                _ => _wavfilepath
            };

            if (strWavPath == string.Empty) { return false; }

            string strTemporaryPath = $@"{strWavPath}\_wav";

            string[] strArrWavFiles = Directory.GetFiles(strWavPath, @"*.wav");
            if (strArrWavFiles.Length == 0)
            {
                Logging($"選択されたフォルダ：{strWavPath}にWAVファイルがありません", true);
                return false;
            }

            if (!_isbatch && MessageBox.Show(
            $"WAVフォルダ：{strWavPath}"
            + $"\nファイル総数：{strArrWavFiles.Length}"
            + "\n\n上記フォルダにあるWAVファイルのLIPファイルを作成します。\nよろしいですか？"
            + "\n\n※「はい」の場合、次のダイアログで出力先フォルダを指定します"
            + "\n\n※既に同名のファイルが存在する場合、上書きされますのでご注意ください",
            buttonMakeWav.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            { return false; }

            string strOutPutPath = _output switch
            {
                "" => OpenFolderDialog("出力先のフォルダを選択してください"),
                _ => _output
            };
            if (strOutPutPath == string.Empty) { return false; }

            LoggingStart(buttonMakeLip.Text);
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);
            listBoxLog.Update();

            ProgressStart("Lipファイル作成中", 0, strArrWavFiles.Length);

            const int iSplitCount = 2;
            int ichunkSize = (strArrWavFiles.Length / iSplitCount) + (strArrWavFiles.Length % iSplitCount);

            var chunkWavFiles = strArrWavFiles.Select((v, i) => new { v, i })
                .GroupBy(x => x.i / ichunkSize)
                .Select(g => g.Select(x => x.v)).ToArray();

            MakeLipData[] lipData = new MakeLipData[iSplitCount];
            int[] iCount = new int[iSplitCount];

            for (int i = 0; i < iSplitCount; i++)
            {
                lipData[i].strArrWavFiles = chunkWavFiles[i].ToArray();
                lipData[i].strOutPutPath = strOutPutPath;
                lipData[i].strFaceFXWrapper = $@"{textBoxFaceFXPath.Text}\{FaceFXWrapper}";
                lipData[i].strFonixData = $@"{textBoxFaceFXPath.Text}\{FonixData}";
                lipData[i].strTemporaryPath = strTemporaryPath;
                iCount[i] = 0;
            }

            CancelToken = new CancellationTokenSource();
            //await Task.Run(() => SubThreadMakeLip(lipData));
            await Task.Run(() => Parallel.Invoke(
                () => iCount[0] = SubThreadMakeLip(lipData[0])
                , () => iCount[1] = SubThreadMakeLip(lipData[1])
                //,() => iCount[2] = SubThreadMakeLip(lipData[2])
                //,() => iCount[3] = SubThreadMakeLip(lipData[3])
                //,() => iCount[4] = SubThreadMakeLip(lipData[4])
                ));

            if (Directory.Exists(strTemporaryPath)) { Directory.Delete(strTemporaryPath, true); }

            if (CancelToken.Token.IsCancellationRequested)
            {
                Logging($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇処理がキャンセルされました");
                Logging('=');
            }

            LoggingEnd(buttonMakeFuz.Text);
            Logging($"データ件数：{iCount.Sum()}"
                + (CancelToken.Token.IsCancellationRequested ? "" : $"  エラー件数：{strArrWavFiles.Length - iCount.Sum()}"));
            Logging($"出力先：{strOutPutPath}", true);

            ProgressEnd();

            return !CancelToken.Token.IsCancellationRequested;
        }

        private async Task<bool> MakeFuzAsync(string _wavfilepath = "", string _output = "", bool _isbatch = false, bool _isdelete = false)
        {
            if (!IsInstallYakitori(true))
            {
                return false;
            }

            if (!CheckUseBatch()) { return false; }
            _output = textBoxBatch.Text;

            string strWavPath = _wavfilepath switch
            {
                "" => OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください"),
                _ => _wavfilepath
            };
            if (strWavPath == string.Empty) { return false; }

            string strTemporaryPath = $@"{_wavfilepath}\_xwm";

            string[] strArrWavFiles = Directory.GetFiles(strWavPath, @"*.wav");
            if (strArrWavFiles.Length == 0)
            {
                Logging($"選択されたフォルダ：{strWavPath}にWAVファイルがありません", true);
                return false;
            }

            if (!_isbatch && MessageBox.Show(
            $"WAVフォルダ：{strWavPath}"
            + $"\nファイル総数：{strArrWavFiles.Length}"
            + "\n\n上記フォルダにあるWAVファイルを全てFUZファイルへ変換します。\nよろしいですか？"
            + "\n\n※「はい」の場合、次のダイアログで出力先フォルダを指定します"
            + "\n\n※既に同名のフォルダやファイルが存在する場合、上書きされますのでご注意ください",
            buttonMakeWav.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            { return false; }

            string strOutPutPath = _output switch
            {
                "" => OpenFolderDialog("出力先のフォルダを選択してください"),
                _ => _output
            };
            if (strOutPutPath == string.Empty) { return false; }

            LoggingStart(buttonMakeFuz.Text);
            Logging($"処理に時間がかかりますのでしばらくお待ち下さい…経過は左下のバーに表示されます", true);
            listBoxLog.Update();

            ProgressStart("FUZファイルへ変換中", 0, strArrWavFiles.Length);

            const int iSplitCount = 2;
            int ichunkSize = (strArrWavFiles.Length / iSplitCount) + (strArrWavFiles.Length % iSplitCount);

            var chunkWavFiles = strArrWavFiles.Select((v, i) => new { v, i })
                .GroupBy(x => x.i / ichunkSize)
                .Select(g => g.Select(x => x.v)).ToArray();

            MakeFuzData[] makeData = new MakeFuzData[iSplitCount];
            int[] iCount = new int[iSplitCount];

            for (int i = 0; i < iSplitCount; i++)
            {
                makeData[i].strArrWavFiles = chunkWavFiles[i].ToArray();
                makeData[i].strOutPutPath = strOutPutPath;
                makeData[i].strWMAEnc = $@"{textBoxYakitoriPath.Text}\{xWMAEncode}";
                makeData[i].strFuzEnc = $@"{textBoxYakitoriPath.Text}\{BmlFuzEncode}";
                makeData[i].strTemporaryPath = strTemporaryPath;
                makeData[i].isbatch = _isbatch;
                makeData[i].isdelete = _isdelete;
                iCount[i] = 0;
            }

            CancelToken = new CancellationTokenSource();
            await Task.Run(() => Parallel.Invoke(
                () => iCount[0] = SubThreadMakeFuz(makeData[0])
                , () => iCount[1] = SubThreadMakeFuz(makeData[1])
                //,() => iCount[2] = SubThreadMakeLip(lipData[2])
                //,() => iCount[3] = SubThreadMakeLip(lipData[3])
                //,() => iCount[4] = SubThreadMakeLip(lipData[4])
                ));

            if (Directory.Exists(strTemporaryPath)) { Directory.Delete(strTemporaryPath, true); }

            if (CancelToken.Token.IsCancellationRequested)
            {
                Logging($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇処理がキャンセルされました");
                Logging('=');
            }

            LoggingEnd(buttonMakeFuz.Text);
            Logging($"データ件数：{iCount.Sum()}"
                + (CancelToken.Token.IsCancellationRequested ? "" : $"  エラー件数：{strArrWavFiles.Length - iCount.Sum()}"));
            Logging($"出力先：{strOutPutPath}", true);

            ProgressEnd();

            return !CancelToken.Token.IsCancellationRequested;
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

        private async void ButtonXmltoJson_Click(object sender, EventArgs e)
        {
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

            if (MessageBox.Show(
                $"{ofd.FileNames.Length}件のXMLファイルが選択されました"
                + "\n\n上記ファイルを読み込みDBVO用の辞書ファイル(JSON)を作成します\nよろしいですか？"
                + "\n\n※「はい」の場合、続けて辞書ファイルの出力先を決定します",
                ToolStripMenuXmltoJson.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            var outputFolder = OpenFolderDialog("辞書ファイル(JSON)の出力先を指定してください");
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

            if (ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", ToolStripMenuXmltoJson.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //JSONから音声ファイルを作成
        private async void ButtonMakeWav_Click(object sender, EventArgs e)
        {
            if (await MakeWavAsync() && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", buttonMakeWav.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }
        }

        //WAV→LIPファイル作成
        private async void ButtonMakeLip_Click(object sender, EventArgs e)
        {
            if (await MakeLipAsync() && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", buttonMakeLip.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }
        }

        //WAV→FUZへの変換
        private async void ButtonMakeFuz_Click(object sender, EventArgs e)
        {
            if (await MakeFuzAsync() && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", buttonMakeFuz.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }
        }

        private static bool IsJapanese(string text)
        {
            var isJapanese = Regex.IsMatch(text, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+");
            return isJapanese;
        }

        private void Logging(char chr = '-', bool isScroll = false)
        {
            listBoxLog.Items.Add(new String(chr, 80));
            if (isScroll)
            {
                listBoxLog.TopIndex = listBoxLog.Items.Count - 1;
            }
        }
        private void Logging(string log, bool isScroll = false)
        {
            listBoxLog.Items.Add($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}◇{log}");
            if (isScroll)
            {
                listBoxLog.TopIndex = listBoxLog.Items.Count - 1;
            }
        }

        private void LoggingRange(string[] logs, bool isScroll = false)
        {
            listBoxLog.Items.AddRange(logs);
            if (isScroll)
            {
                listBoxLog.TopIndex = listBoxLog.Items.Count - 1;
            }
        }

        private void TextBoxYakitoriPath_TextChanged(object sender, EventArgs e)
        {
            if (!IsInstallYakitori(true))
            {
                labelYakitori.Text = "NG";
                labelYakitori.BackColor = Color.Red;
                return;
            }

            labelYakitori.Text = "OK";
            labelYakitori.BackColor = Color.Green;

            buttonMakeFuz.Enabled = labelYakitori.Text == "OK";
            buttonBatch.Enabled = labelFaceFX.Text == "OK" && labelYakitori.Text == "OK" && textBoxBatch.Text.Length > 0;
        }

        private void ButtonYakitoriOpen_Click(object sender, EventArgs e)
        {
            string str = OpenFolderDialog("Yakitori Audio Converterのインストール先のフォルダを選択してください");
            if (str != string.Empty) { textBoxYakitoriPath.Text = str; }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
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
                config.AppSettings.Settings["CheckboxOnlyMoji"].Value = checkBoxOnlyMoji.Checked.ToString();
                config.AppSettings.Settings["CheckboxBatch"].Value = checkBoxBatch.Checked.ToString();
                config.AppSettings.Settings["IsUseBatchOutput"].Value = ToolStripMenuUseBatchOutput.Checked.ToString();

                config.Save();
            }
            catch(Exception) { 
            }
        }

        private void TextBoxFaceFXPath_TextChanged(object sender, EventArgs e)
        {
            if (!IsInstallFaceFX(true))
            {
                labelFaceFX.Text = "NG";
                labelFaceFX.BackColor = Color.Red;
                return;
            }

            labelFaceFX.Text = "OK";
            labelFaceFX.BackColor = Color.Green;

            buttonMakeLip.Enabled = labelFaceFX.Text == "OK";
            buttonBatch.Enabled = labelFaceFX.Text == "OK" && labelYakitori.Text == "OK" && textBoxBatch.Text.Length > 0;
        }

        private void ButtonFaceFXOpen_Click(object sender, EventArgs e)
        {
            string str = OpenFolderDialog("FaceFXWrapperのインストール先のフォルダを選択してください");
            if (str != string.Empty) { textBoxFaceFXPath.Text = str; }
        }

        private void LabelFaceFX_TextChanged(object sender, EventArgs e)
        {
            buttonMakeLip.Enabled = labelFaceFX.Text == "OK";
            buttonBatch.Enabled = (labelFaceFX.Text == "OK" && labelYakitori.Text == "OK");
        }

        private void LabelYakitori_TextChanged(object sender, EventArgs e)
        {
            buttonMakeFuz.Enabled = labelYakitori.Text == "OK";
            buttonBatch.Enabled = (labelFaceFX.Text == "OK" && labelYakitori.Text == "OK");
        }

        //一括処理
        private async void ButtonBatch_Click(object sender, EventArgs e)
        {
            string strOutputBatch = textBoxBatch.Text;
            if (strOutputBatch == string.Empty)
            {
                Logging($"一括処理の出力先フォルダを指定してください", true);
                return;
            }

            string strWavPath = OpenFolderDialog("WAVファイルが格納されているフォルダを選択してください");
            if (strWavPath == string.Empty) { return; }

            int iFileCount = Directory.GetFiles(strWavPath, @"*.wav").Length;

            if (MessageBox.Show(
                $"WAVフォルダ：{strWavPath}"
                + $"\n出力先：{strOutputBatch}"
                + $"\nデータ件数：{iFileCount}"
                + $"\n\n上記フォルダにあるWAVファイルに対してLIP作成、FUZ変換を行います\nよろしいですか？"
                + "\n\nLIPファイルはWAVファイルと同フォルダ、\nFUZファイルは出力先フォルダにそれぞれ出力されます。"
                + "\n\n※データ件数が多いと時間がかかります\n途中でキャンセルしたい場合は左下の処理中止ボタンを押してください"
                + "\n\n※出力先に同名ファイルが既に存在している場合、上書きされます",
                buttonBatch.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            { return; }

            bool isNotCancel = true;

            if (isNotCancel)
            {
                isNotCancel = await MakeLipAsync($@"{strWavPath}", strWavPath, true);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeFuzAsync($@"{strWavPath}", strOutputBatch, true, checkBoxBatch.Checked);
                listBoxLog.Update();
            }

            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }

            LoggingEnd(buttonBatch.Text);
            Logging($"フォルダ：{strOutputBatch}をご確認ください", true);
            listBoxLog.Update();

            if (isNotCancel && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", buttonBatch.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonMakePack_Click(object sender, EventArgs e)
        {
            FormVoicePack formVP = new();
            formVP.ShowDialog(this);
        }

        private void ComboBoxPreset_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            EnabledButtons(true, false);
            //buttonMakeWav.Enabled = comboBoxPreset.SelectedIndex != -1;
            //buttonSample.Enabled = comboBoxPreset.SelectedIndex != -1;
        }

        //キャラクター取得ボタン
        private async void ButtonGetCharList_Click(object sender, EventArgs e)
        {
            dynamic? speakers;

            VoiceFunction? voiceFunc = GetVoicesoft();
            if (voiceFunc is null) { return; }

            speakers = await voiceFunc.GetSpeakers();
            if (speakers is null)
            {
                Logging($"キャラクター一覧の取得に失敗しました。{voiceFunc.Name}が起動していることを確認してください", true);
                return;
            }

            List<ItemSet> listSrc = new();

            if (voiceFunc.GetType() == typeof(StyleBertVITS2Function))
            {
                var item = speakers.ToObject<Dictionary<string, object>>();
                foreach (var id in item.Keys)
                {
                    var item2 = item[id]["style2id"].ToObject<Dictionary<string, string>>();
                    foreach (var style in item2.Keys)
                    {
                        listSrc.Add(new ItemSet(Convert.ToInt32(id), $"{id,3}:{item[id]["id2spk"]["0"]}:{style}"));
                    }
                }
            }
            else
            {
                foreach (dynamic speaker in speakers)
                {
                    foreach (dynamic style in speaker[voiceFunc.ColumNameStyles])
                    {
                        listSrc.Add(
                            new ItemSet((int)style[voiceFunc.ColumNameStyleId],
                            $"{style[voiceFunc.ColumNameStyleId],3}:{speaker[voiceFunc.ColumNameSpeakerName]}:{style[voiceFunc.ColumNameStyleName]}"));
                    }
                }
            }

            if (listSrc.Count == 0)
            {
                Logging($"キャラクター一覧の取得に失敗しました。{voiceFunc.Name}が起動していることを確認してください", true);
                return;
            }

            // ComboBoxに表示と値をセット
            comboBoxPreset.DataSource = listSrc;
            comboBoxPreset.DisplayMember = "ItemDisp";
            comboBoxPreset.ValueMember = "ItemValue";

            comboBoxPreset.SelectedIndex = 0;
            ComboBoxPreset_SelectedIndexChanged(null, null);

            Logging($"{comboBoxVoiceSoft.Text}のキャラクター一覧を取得しました", true);
            if (voiceFunc.GetType() == typeof(StyleBertVITS2Function))
            {
                Logging($"※このボイスソフトではボイス調整は話速のみ適用され、値を上げると話速が遅くなります", true);
            }
        }

        private async void ButtonSample_Click(object sender, EventArgs e)
        {
            string[] strArrSampleWord = { "当ててやろうか？、誰かにスイートロールを盗まれたかな？",
                                        "ここに来るべきじゃなかったな",
                                        "勝利か、ソブンガルデかだ！！",
                                        "ガラクタだって言う人もいるけど、私はたからものって呼んでるわ",
                                        "お前のゴールドを数えるのが楽しみだ！" };
            string strSampleWord = strArrSampleWord[new Random().Next(strArrSampleWord.Length)];

            buttonSample.Enabled = false;
            Logging($"サンプル再生中：{((ItemSet)comboBoxPreset.SelectedItem).ItemDisp}", true);

            VoiceFunction? voiceFunc = GetVoicesoft();
            if (voiceFunc is null) { return; }

            int iSpeaker = (int)comboBoxPreset.SelectedValue;
            string strStyle = comboBoxPreset.Text.Contains(':') ? comboBoxPreset.Text.Split(':')[2] : "";
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
            else if (statusCode != HttpStatusCode.OK)
            {
                Logging($"処理に失敗しました。{comboBoxVoiceSoft.Text}が起動していることを確認してください", true);
            }

            await Task.Delay(2000);

            buttonSample.Enabled = true;
        }

        private void ButtonBatchFolder_Click(object sender, EventArgs e)
        {
            string str = OpenFolderDialog("一括処理の出力先フォルダを指定してください");
            if (str != string.Empty) { textBoxBatch.Text = str; }
        }

        private void TextBoxBatch_TextChanged(object sender, EventArgs e)
        {
            EnabledButtons(true, false);
            //buttonBatch.Enabled = labelFaceFX.Text == "OK" && labelYakitori.Text == "OK" && textBoxBatch.Text.Length > 0;
        }

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

            if (MessageBox.Show(
                $"{ofd.FileNames.Length}件のXMLファイルが選択されました"
                + "\n\n上記ファイルを読み込み日本語訳の文章から半角スペースを削除します\nよろしいですか？"
                + "\n\n※「はい」の場合、続けて変更後のファイル出力先を決定します",
                toolStripMenuDelXmlSpace.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            var outputFolder = OpenFolderDialog("変更後のXMLファイルの出力先を指定してください");
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

            if (ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", toolStripMenuDelXmlSpace.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void ToolStripMenuMakeWavFromText_Click(object sender, EventArgs e)
        {
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

            if (MessageBox.Show(
                $"{ofd.FileNames.Length}件のテキストファイルが選択されました"
                + "\n\n上記ファイルを読み込み音声(WAV)を作成します\nよろしいですか？"
                + "\n\n※「はい」の場合、続けてファイル出力先を決定します",
                ToolStripMenuMakeWavFromText.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            var outputFolder = OpenFolderDialog("音声(WAV)ファイルの出力先を指定してください");
            if (outputFolder == string.Empty) { return; }

            EnabledButtons(false, false);

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
                        Logging($"{iCounter:D3}：設定されている項目が足りないためスキップされます");
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

                await Task.Run(() => SubThreadMakeWav(listWavData.ToArray()));
            }

            LoggingEnd(ToolStripMenuMakeWavFromText.Text);
            Logging($"出力フォルダ：{outputFolder}", true);
            ProgressEnd();

            EnabledButtons(true, false);

            if (!CancelToken.Token.IsCancellationRequested && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", ToolStripMenuMakeWavFromText.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return;
        }

        private void listBoxLog_Leave(object sender, EventArgs e)
        {
            listBoxLog.Update();
        }

        private async void ButtonBatchAll_Click(object sender, EventArgs e)
        {
            string strOutputBatch = textBoxBatch.Text;
            if (strOutputBatch == string.Empty)
            {
                Logging($"一括処理の出力先フォルダを指定してください", true);
                return;
            }

            bool isNotCancel = true;

            if (isNotCancel)
            {
                isNotCancel = await MakeWavAsync(strOutputBatch);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeLipAsync(strOutputBatch, strOutputBatch, true);
                listBoxLog.Update();
            }

            if (isNotCancel)
            {
                isNotCancel = await MakeFuzAsync(strOutputBatch, strOutputBatch, true, checkBoxBatch.Checked);
                listBoxLog.Update();
            }

            if (CancelToken is not null)
            {
                CancelToken.Dispose();
                CancelToken = null;
            }

            if (isNotCancel)
            {
                LoggingEnd(buttonBatchAll.Text);
                Logging($"フォルダ：{strOutputBatch}をご確認ください", true);
                listBoxLog.Update();
            }

            if (isNotCancel && ToolStripMenuItemIsShowMsg.Checked)
            {
                MessageBox.Show($"処理が完了しました", buttonBatchAll.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task ReadDictionary(string _path = "")
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
                LoggingStart($"読み&アクセント辞書の読み込み");
            }
            else
            {
                strDicPath = _path;
                LoggingStart($"デフォルトの読み&アクセント辞書の読み込み");
            }

            EnabledButtons(false, false);

            string strTxtFile = Path.GetFileName(strDicPath);

            Logging($"◆◆ファイルの書式チェック・・・");
            var txtAllData = new List<string>();

            try
            {
                string? strData = string.Empty;

                using FileStream fStrean = new(strDicPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using TextReader tReader = new StreamReader(fStrean, Encoding.UTF8);

                while ((strData = tReader.ReadLine()) is not null)
                {
                    txtAllData.Add(strData);
                }
                if (txtAllData.Count == 0)
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

                if (txtDataSplit.Length < 4)
                {
                    Logging($"{iCounter:D3}：設定されている項目が足りないためスキップされます");
                    iErrCounter++;
                    continue;
                }

                if (!int.TryParse(txtDataSplit[2], out int iAccent))
                {
                    Logging($"{iCounter:D3}：3列目の項目(アクセント)が間違っているためスキップされます");
                    iErrCounter++;
                    continue;
                }

                if (txtDataSplit.Length >= 4 && !int.TryParse(txtDataSplit[3], out int iMoranum))
                {
                    Logging($"{iCounter:D3}：4列目の項目(音数)が間違っているためスキップされます");
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
                Logging($"{iCounter:D3}：{voiceDict.Text},{voiceDict.Yomi},{voiceDict.Accent},{voiceDict.Moranum}");
            }

            Logging($"◆データ件数：{iCounter}  スキップ件数：{iErrCounter}", true);
            Logging();
            Logging($"◆◆COEIROINKの辞書を登録しています・・・", true);

            var jsonObj = new JObject();
            var jsonArray = new JArray();

            foreach (var voice in listData)
            {
                var jObject = new JObject()
                {
                    ["word"] = voice.Text,
                    ["yomi"] = voice.Yomi,
                    ["accent"] = voice.Accent,
                    ["numMoras"] = voice.Moranum
                };

                jsonArray.Add(jObject);
            }

            jsonObj.Add("dictionaryWords", jsonArray);

            var coeiroFunc = new CoeiroinkFunction();
            var resCoe = await coeiroFunc.SetDictionary(jsonObj);
            if (resCoe is null)
            {
                Logging("処理に失敗しました");
            }
            else if (resCoe == HttpStatusCode.RequestTimeout)
            {
                Logging($"COEIROINKが起動していないため登録をスキップします");
            }
            else if (resCoe != HttpStatusCode.OK)
            {
                Logging($"処理に失敗しました：{resCoe}");
            }

            Logging($"◆◆VOICEVOXの辞書を登録しています・・・", true);
            var voicevoxFunc = new VoicevoxFunction();

            var resvox = await voicevoxFunc.SetDictionary(listData);
            if (resvox is null)
            {
                Logging("処理に失敗しました");
            }
            else if (resvox == HttpStatusCode.RequestTimeout)
            {
                Logging($"VOICEVOXが起動していないため登録をスキップします");
            }
            else if (resvox != HttpStatusCode.OK)
            {
                Logging($"処理に失敗しました コード：{resvox}");
            }

            LoggingEnd("読み&アクセント辞書の読み込み");
            EnabledButtons(true, false);
        }

        private async void ToolStripMenuReadDicDefault_Click(object sender, EventArgs e)
        {
            if (File.Exists(DefaultDictionary))
            {
                await ReadDictionary(DefaultDictionary);
            }
            else
            {
                Logging("デフォルトの読み&アクセント辞書「DefaultDictionary.csv」がありません", true);
            }
        }

        private async void ToolStripMenuReadDicTxt_Click(object sender, EventArgs e)
        {
            await ReadDictionary();
        }

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
    }

    public struct VoiceDictionary
    {
        public string Text { get; set; }
        public string Yomi { get; set; }
        public int Accent { get; set; }
        public int Moranum {  get; set; }
    }

    public class ItemSet
    {
        // DisplayMemberとValueMemberにはプロパティで指定する仕組み
        public String ItemDisp { get; set; }
        public int ItemValue { get; set; }

        // プロパティをコンストラクタでセット
        public ItemSet(int v, String s)
        {
            ItemValue = v;
            ItemDisp = s;
        }

        public ItemSet(string v, String s)
        {
            ItemValue = int.Parse(v);
            ItemDisp = s;
        }
    }

    public class VoiceFunction
    {
        private const string strDefaultIP = "127.0.0.1";

        protected readonly string ipPort;
        protected static readonly HttpClient httpClient = new();

        public virtual string? Name { get; }
        public virtual string? ColumNameStyles { get; }
        public virtual string? ColumNameStyleId { get;  }
        public virtual string? ColumNameStyleName { get;  }
        public virtual string? ColumNameSpeakerName { get;  }

        public VoiceFunction(int _port) : this(strDefaultIP, _port)
        {
            //ipPort = $"http://{strDefaultIP}:{_port}";
            //httpClient = new HttpClient();
        }

        public VoiceFunction(string _ipAdress, int _port)
        {
            //HttpClientHandler handler = new()
            //{
            //    UseProxy = false
            //};
            ipPort = $"http://{_ipAdress}:{_port}";
            //httpClient = new HttpClient(handler);
        }

        private static async Task<string> GetSpeakersAsJson(string _reqUri)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), _reqUri);
            request.Headers.TryAddWithoutValidation("accept", "application/json");

            try
            {
                var response = await httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static async Task<HttpResponseMessage> GetRequest(string _url, string _header)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);

            return await httpClient.SendAsync(request);
        }

        private static async Task<HttpResponseMessage> PutRequest(string _url, string _header)
        {
            using var request = new HttpRequestMessage(new HttpMethod("PUT"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);

            return await httpClient.SendAsync(request);
        }

        private static async Task<HttpResponseMessage> SendRequest(string _url, string _header, string _ctype, string _content = "")
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _url);

            request.Headers.TryAddWithoutValidation("accept", _header);
            request.Content = new StringContent(_content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(_ctype);

            return await httpClient.SendAsync(request);
        }

        private static async Task<HttpResponseMessage> SendRequest(string _url, string _header, MultipartFormDataContent _content)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);
            request.Content = _content;

            return await httpClient.SendAsync(request);
        }

        protected static async Task<HttpResponseMessage?> GetJson(string _url)
        {
            return await GetRequest(_url, "application/json");
        }

        protected static async Task<HttpResponseMessage?> PutJson(string _url)
        {
            return await PutRequest(_url, "*/*");
        }

        protected static async Task<HttpResponseMessage?> AcceptJson(string _url)
        {
            return await SendRequest(_url, "application/json", "application/x-www-form-urlencoded");
        }

        protected static async Task<HttpResponseMessage?> AcceptJson(string _url, string _content)
        {
            return await SendRequest(_url, "application/json", "application/json", _content);
        }

        protected static async Task<HttpResponseMessage?> AcceptAudio(string _url, string _content)
        {
            return await SendRequest(_url, "audio/wav", "application/json", _content);
        }

        protected static async Task<HttpResponseMessage?> AcceptAudio(string _url)
        {
            return await SendRequest(_url, "audio/wav", "application/x-www-form-urlencoded");
        }

        protected static async Task<HttpResponseMessage?> AcceptAudio(string _url, MultipartFormDataContent _content)
        {
            return await SendRequest(_url, "audio/wav", _content);
        }

        protected static async Task<byte[]?> AcceptAudioByte(string _url)
        {
            try
            {
                var response = await SendRequest(_url, "audio/wav", "application/x-www-form-urlencoded");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        public virtual dynamic? GetSpeakers() { return null; }

        public async Task<dynamic?> GetSpeakersAsync(string _command)
        {
            var strJsonStr = await Task.Run(() => GetSpeakersAsJson($"{ipPort}/{_command}"));

            return JsonConvert.DeserializeObject(strJsonStr);
        }

        public virtual Task<HttpStatusCode?>? MakeSound(string _title, string _text, bool _upspeak, int _speakerId, double[] param)
        { 
            return null;
        }

        protected static void Play(in Stream stream)
        {
            var player = new SoundPlayer(stream);
            player.Play();
        }

        protected static void Save(string _title, in Stream _stream)
        {
            var dirName = Path.GetDirectoryName(_title);
            if (dirName is null || dirName == string.Empty) { return; }

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            using var fileStream = File.Create($"{_title}.wav");

            _stream.CopyTo(fileStream);
            fileStream.Flush();
        }
    }

    public class VoicevoxFunction : VoiceFunction
    {
        private const int DefaultPort = 50021;

        public override string Name
        {
            get { return "VOICEVOX"; }
        }

        public override string ColumNameStyles
        {
            get { return "styles"; }
        }
        public override string ColumNameStyleId
        {
            get { return "id"; }
        }

        public override string ColumNameStyleName
        {
            get { return "name"; }
        }

        public override string ColumNameSpeakerName
        {
            get { return "name"; }
        }


        public VoicevoxFunction() : base(DefaultPort) { }

        public VoicevoxFunction(int _port) : base(_port) { }

        public VoicevoxFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override dynamic? GetSpeakers()
        {
            return base.GetSpeakersAsync("speakers");
        }

        public async Task<HttpStatusCode?> SetDictionary(List<VoiceDictionary> _list)
        {
            try
            {
                var resGet = await GetJson($"{ipPort}/user_dict");
                if (resGet is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }

                var strResGet = await resGet.Content.ReadAsStringAsync();
                if (strResGet is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }
                var dctResGet = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(strResGet);
                if (dctResGet is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }

                foreach (var voice in _list)
                {
                    string strUuid = string.Empty;

                    foreach (var key in dctResGet.Keys)
                    {
                        if (dctResGet[key].Where(x => x.Key == "surface" && x.Value == voice.Text).Count() > 0)
                        {
                            strUuid = key;
                            break;
                        }
                    }

                    HttpResponseMessage? res;

                    if (strUuid == string.Empty)
                    {
                        res = await AcceptJson($"{ipPort}/user_dict_word?surface={voice.Text}&pronunciation={voice.Yomi}&accent_type={voice.Accent}"
                            + "&word_type=COMMON_NOUN&priority=9");
                    }
                    else
                    {
                        res = await PutJson($"{ipPort}/user_dict_word/{strUuid}?surface={voice.Text}&pronunciation={voice.Yomi}&accent_type={voice.Accent}"
                            + "&word_type=COMMON_NOUN&priority=9");
                    }
                    if (res is null)
                    {
                        return HttpStatusCode.RequestTimeout;
                    }
                    else if (res.StatusCode != HttpStatusCode.OK && res.StatusCode != HttpStatusCode.NoContent)
                    {
                        return res.StatusCode;
                    }
                }
            }
            catch (SocketException)
            {
                return HttpStatusCode.RequestTimeout;
            }
            catch (Exception)
            {
                return HttpStatusCode.RequestTimeout;
            }
            return HttpStatusCode.OK;
        }

        public override async Task<HttpStatusCode?> MakeSound(string _title, string _text, bool _upspeak, int _speakerId, double[] param)
        {
            try
            {
                var res = await AcceptJson($"{ipPort}/audio_query?text={_text}&speaker={_speakerId}");
                if (res is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }
                else if (res.StatusCode != HttpStatusCode.OK)
                {
                    return res.StatusCode;
                }
            
                //ボイス調整
                var audioQuery = JsonConvert.DeserializeObject<Dictionary<string, object>>(await res.Content.ReadAsStringAsync());
                if(audioQuery is null) { return null; }

                audioQuery["speedScale"] = param[0];
                audioQuery["pitchScale"] = param[1];
                audioQuery["intonationScale"] = param[2];
                audioQuery["volumeScale"] = param[3];

                string strChangeQuery = JsonConvert.SerializeObject(audioQuery, Formatting.Indented);

                var httpStream = await AcceptAudio($"{ipPort}/synthesis?speaker={_speakerId}&enable_interrogative_upspeak={_upspeak}", strChangeQuery);
                if (httpStream is not null)
                {
                    if(httpStream.StatusCode != HttpStatusCode.OK)
                    {
                        return httpStream.StatusCode;
                    }
                    if (_title == string.Empty)
                    {
                        VoiceFunction.Play(httpStream.Content.ReadAsStream());
                    }
                    else
                    {
                        VoiceFunction.Save(_title, httpStream.Content.ReadAsStream());
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException)
            {
                return HttpStatusCode.RequestTimeout;
            }

            return HttpStatusCode.OK;
        }
    }

    public class VoicevoxNemoFunction : VoicevoxFunction
    {
        private const int DefaultPort = 50121;

        public override string Name
        {
            get { return "VOICEVOX Nemo"; }
        }

        public VoicevoxNemoFunction() : base(DefaultPort) { }

        public VoicevoxNemoFunction(int _port) : base(_port) { }

        public VoicevoxNemoFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class SharevoiceFunction : VoicevoxFunction
    {
        private const int DefaultPort = 50025;

        public override string Name
        {
            get { return "SHAREVOICE"; }
        }

        public SharevoiceFunction() : base(DefaultPort) { }

        public SharevoiceFunction(int _port) : base(_port) { }

        public SharevoiceFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class ItvoiceFunction : VoicevoxFunction
    {
        private const int DefaultPort = 49540;

        public override string Name
        {
            get { return "ITVOICE"; }
        }

        public ItvoiceFunction() : base(DefaultPort) { }

        public ItvoiceFunction(int _port) : base(_port) { }

        public ItvoiceFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class CoeiroinkFunction : VoiceFunction
    {
        private const int DefaultPort = 50032;

        public override string Name
        {
            get { return "COEIROINK"; }
        }

        public override string ColumNameStyles
        {
            get { return "styles"; }
        }
        public override string ColumNameStyleId
        {
            get { return "styleId"; }
        }

        public override string ColumNameStyleName
        {
            get { return "styleName"; }
        }

        public override string ColumNameSpeakerName
        {
            get { return "speakerName"; }
        }

        public CoeiroinkFunction() : base(DefaultPort) { }

        public CoeiroinkFunction(int _port) : base(_port) { }

        public CoeiroinkFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override dynamic? GetSpeakers()
        {
            return base.GetSpeakersAsync("v1/speakers");
        }

        private async Task<HttpResponseMessage?> SpeakerInfoFromIdAsync(int _id)
        {
            return await AcceptJson($"{ipPort}/v1/style_id_to_speaker_meta?styleId={_id}");
        }

        public async Task<HttpStatusCode?> SetDictionary(JObject _dic)
        {
            try
            {
                var res = await AcceptJson($"{ipPort}/v1/set_dictionary", JsonConvert.SerializeObject(_dic, Formatting.Indented));
                if (res is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }
                else if (res.StatusCode != HttpStatusCode.OK)
                {
                    return res.StatusCode;
                }
            }
            catch (Exception )
            {
                return HttpStatusCode.RequestTimeout;
            }
            return HttpStatusCode.OK;
        }

        public override async Task<HttpStatusCode?> MakeSound(string _title, string _text, bool _upspeak, int _speakerId, double[] param)
        {
            try
            {
                var res = await SpeakerInfoFromIdAsync(_speakerId);
                if(res is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }
                else if (res.StatusCode != HttpStatusCode.OK)
                {
                    return res.StatusCode;
                }

                var speakerInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(await res.Content.ReadAsStringAsync());
                if (speakerInfo is null) { return null; }

                //音声ファイルの取得
                var dicRoot = new Dictionary<string, object>
                {
                    { "speakerUuid", speakerInfo["speakerUuid"] },
                    { "styleId", _speakerId },
                    { "text", _text },
                    { "speedScale", param[0] }
                };

                string strChangeQuery = JsonConvert.SerializeObject(dicRoot, Formatting.Indented);

                var resAudio = await AcceptAudio($"{ipPort}/v1/predict", strChangeQuery);
                if (resAudio is null) { return null; }

                using var multipartContent = new MultipartFormDataContent();
                var audioFile = new ByteArrayContent(resAudio.Content.ReadAsByteArrayAsync().Result);
                audioFile.Headers.Add("Content-Type", "audio/wav");
                multipartContent.Add(audioFile, "wav", "sample");
                multipartContent.Add(new StringContent(param[3].ToString()), "volumeScale");
                multipartContent.Add(new StringContent(param[1].ToString()), "pitchScale");
                multipartContent.Add(new StringContent(param[2].ToString()), "intonationScale");
                multipartContent.Add(new StringContent("0.1"), "prePhonemeLength");
                multipartContent.Add(new StringContent("0.1"), "postPhonemeLength");
                multipartContent.Add(new StringContent("44100"), "outputSamplingRate");

                var resAudioControl = await AcceptAudio($"{ipPort}/v1/process", multipartContent);

                if (resAudioControl is null) { return null;}
                if (resAudioControl.StatusCode != HttpStatusCode.OK)
                {
                    return resAudioControl.StatusCode;
                }

                using var httpStream = await resAudioControl.Content.ReadAsStreamAsync();
                if(httpStream is null) { return null;}

                if (_title == string.Empty)
                {
                    VoiceFunction.Play(httpStream);
                }
                else
                {
                    VoiceFunction.Save(_title, httpStream);
                }
            }
            catch (HttpRequestException)
            {
                return HttpStatusCode.RequestTimeout;
            }

            return HttpStatusCode.OK;
        }
    }

    public class StyleBertVITS2Function : VoiceFunction
    {
        private const int DefaultPort = 5000;

        public override string Name
        {
            get { return "Style-Bert-VITS2"; }
        }

        public StyleBertVITS2Function() : base(DefaultPort) { }

        public StyleBertVITS2Function(int _port) : base(_port) { }

        public StyleBertVITS2Function(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override dynamic? GetSpeakers()
        {
            return base.GetSpeakersAsync("models/info");
        }

        public async Task<HttpStatusCode?> MakeSound(string _title, string _text, string _style, int _speakerId, double[] param)
        {
            StringBuilder strParam = new($"text={_text}");
            strParam.Append($"&model_id={_speakerId}");
            strParam.Append($"&length={param[0]}");
            if (_style != string.Empty)
            {
                strParam.Append($"&style={_style}");
            }
            //strParam.Append($"&speaker_id=0&sdp_ratio=0.2&noise=0.6&noisew=0.8&language=JP&auto_split=true&split_interval=0.5&assist_text_weight=1&style_weight=5");

            try { 
                var httpStream = await AcceptAudio($"{ipPort}/voice?{strParam}");
                if (httpStream is not null)
                {
                    if (httpStream.StatusCode != HttpStatusCode.OK)
                    {
                        return httpStream.StatusCode;
                    }
                    if (_title == string.Empty)
                    {
                        VoiceFunction.Play(httpStream.Content.ReadAsStream());
                    }
                    else
                    {
                        VoiceFunction.Save(_title, httpStream.Content.ReadAsStream());
                    }
                }
                else
                {
                    return HttpStatusCode.RequestTimeout;
                }
            }
            catch (HttpRequestException)
            {
                return HttpStatusCode.RequestTimeout;
            }

            return HttpStatusCode.OK;
        }
    }

    internal class FlickerFreeListBox : System.Windows.Forms.ListBox
    {
        public FlickerFreeListBox()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.Items.Count > 0)
            {
                e.DrawBackground();
                e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(this.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
