using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Media;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

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

    public struct VoiceDictionary
    {
        public string Text { get; set; }
        public string Yomi { get; set; }
        public int Accent { get; set; }
        public int Moranum { get; set; }
    }

    public class VoiceFunction
    {
        private const string strDefaultIP = "127.0.0.1";

        protected readonly string ipPort;
        protected static readonly HttpClient httpClient = new();

        public virtual string? Name { get; }
        public virtual string? ExeName { get; }
        public virtual string? ColumNameStyles { get; }
        public virtual string? ColumNameStyleId { get; }
        public virtual string? ColumNameStyleName { get; }
        public virtual string? ColumNameSpeakerName { get; }

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

        private async Task<string> GetSpeakersAsJson(string _reqUri)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), _reqUri);
            request.Headers.TryAddWithoutValidation("accept", "application/json");

            try
            {
                using var response = await httpClient.SendAsync(request);
                var str = await response.Content.ReadAsStringAsync();
                return str;
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

        private async Task<HttpResponseMessage> GetRequest(string _url, string _header)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);

            return await httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> PutRequest(string _url, string _header)
        {
            using var request = new HttpRequestMessage(new HttpMethod("PUT"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);

            return await httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequest(string _url, string _header, string _ctype, string _content = "")
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _url);

            request.Headers.TryAddWithoutValidation("accept", _header);
            request.Content = new StringContent(_content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(_ctype);

            return await httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequest(string _url, string _header, MultipartFormDataContent _content)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _url);
            request.Headers.TryAddWithoutValidation("accept", _header);
            request.Content = _content;

            return await httpClient.SendAsync(request);
        }

        protected async Task<HttpResponseMessage?> GetJson(string _url)
        {
            return await GetRequest(_url, "application/json");
        }

        protected async Task<HttpResponseMessage?> PutJson(string _url)
        {
            return await PutRequest(_url, "*/*");
        }

        protected async Task<HttpResponseMessage?> AcceptJson(string _url)
        {
            return await SendRequest(_url, "application/json", "application/x-www-form-urlencoded");
        }

        protected async Task<HttpResponseMessage?> AcceptJson(string _url, string _content)
        {
            return await SendRequest(_url, "application/json", "application/json", _content);
        }

        protected async Task<HttpResponseMessage?> AcceptAudio(string _url, string _content)
        {
            return await SendRequest(_url, "audio/wav", "application/json", _content);
        }

        protected async Task<HttpResponseMessage?> AcceptAudio(string _url)
        {
            return await SendRequest(_url, "audio/wav", "application/x-www-form-urlencoded");
        }

        protected async Task<HttpResponseMessage?> AcceptAudio(string _url, MultipartFormDataContent _content)
        {
            return await SendRequest(_url, "audio/wav", _content);
        }

        public virtual async Task<HttpStatusCode?> SetDictionary(List<VoiceDictionary> _list)
        => HttpStatusCode.OK;

        protected async Task<byte[]?> AcceptAudioByte(string _url)
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

        public virtual async Task<string> GetSpeakers()
        {
            await Task.Delay(100);
            return string.Empty;
        }

        public async Task<string> GetSpeakersAsync(string _command)
        {
            return await Task.Run(() => GetSpeakersAsJson($"{ipPort}/{_command}"));
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
        public override string Name => "VOICEVOX";
        public override string ExeName => "VOICEVOX.exe";
        public override string ColumNameStyles => "styles";
        public override string ColumNameStyleId => "id";
        public override string ColumNameStyleName => "name";
        public override string ColumNameSpeakerName => "name";

        public VoicevoxFunction() : base(DefaultPort) { }

        public VoicevoxFunction(int _port) : base(_port) { }

        public VoicevoxFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override async Task<string> GetSpeakers() => await GetSpeakersAsync("speakers");

        public override async Task<HttpStatusCode?> SetDictionary(List<VoiceDictionary> _list)
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
                if (audioQuery is null) { return null; }

                audioQuery["speedScale"] = param[0];
                audioQuery["pitchScale"] = param[1];
                audioQuery["intonationScale"] = param[2];
                audioQuery["volumeScale"] = param[3];

                string strChangeQuery = JsonConvert.SerializeObject(audioQuery, Formatting.Indented);

                var httpStream = await AcceptAudio($"{ipPort}/synthesis?speaker={_speakerId}&enable_interrogative_upspeak={_upspeak}", strChangeQuery);
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
        public override string Name => "VOICEVOX Nemo";

        public VoicevoxNemoFunction() : base(DefaultPort) { }

        public VoicevoxNemoFunction(int _port) : base(_port) { }

        public VoicevoxNemoFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class SharevoiceFunction : VoicevoxFunction
    {
        private const int DefaultPort = 50025;
        public override string Name => "SHAREVOICE";

        public SharevoiceFunction() : base(DefaultPort) { }

        public SharevoiceFunction(int _port) : base(_port) { }

        public SharevoiceFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class ItvoiceFunction : VoicevoxFunction
    {
        private const int DefaultPort = 49540;
        public override string Name => "ITVOICE";

        public ItvoiceFunction() : base(DefaultPort) { }

        public ItvoiceFunction(int _port) : base(_port) { }

        public ItvoiceFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }
    }

    public class CoeiroinkFunction : VoiceFunction
    {
        private const int DefaultPort = 50032;

        public override string Name => "COEIROINK";
        public override string ExeName => "COEIROINKv2.exe"; 
        public override string ColumNameStyles => "styles";
        public override string ColumNameStyleId => "styleId";
        public override string ColumNameStyleName => "styleName";
        public override string ColumNameSpeakerName => "speakerName";

        public CoeiroinkFunction() : base(DefaultPort) { }

        public CoeiroinkFunction(int _port) : base(_port) { }

        public CoeiroinkFunction(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override async Task<string> GetSpeakers() => await GetSpeakersAsync("v1/speakers");

        private async Task<HttpResponseMessage?> SpeakerInfoFromIdAsync(int _id)
            => await AcceptJson($"{ipPort}/v1/style_id_to_speaker_meta?styleId={_id}");

        public override async Task<HttpStatusCode?> SetDictionary(List<VoiceDictionary> _list)
        {
            var jsonObj = new JObject();
            var jsonArray = new JArray();

            foreach (var voice in _list)
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

            try
            {
                var res = await AcceptJson($"{ipPort}/v1/set_dictionary", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
                if (res is null)
                {
                    return HttpStatusCode.RequestTimeout;
                }
                else if (res.StatusCode != HttpStatusCode.OK)
                {
                    return res.StatusCode;
                }
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
                var res = await SpeakerInfoFromIdAsync(_speakerId);
                if (res is null)
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

                if (resAudioControl is null) { return null; }
                if (resAudioControl.StatusCode != HttpStatusCode.OK)
                {
                    return resAudioControl.StatusCode;
                }

                using var httpStream = await resAudioControl.Content.ReadAsStreamAsync();
                if (httpStream is null) { return null; }

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

        public override string Name => "Style-Bert-VITS2";

        public StyleBertVITS2Function() : base(DefaultPort) { }

        public StyleBertVITS2Function(int _port) : base(_port) { }

        public StyleBertVITS2Function(string _ipAdress, int _port) : base(_ipAdress, _port) { }

        public override async Task<string> GetSpeakers() => await GetSpeakersAsync("models/info");

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

            try
            {
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
}
