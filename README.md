## 概要
音声合成ソフト（VOICEVOX、COEIROINK、Style-Bert-VITS2）と連携して、  
「Dragonborn Voice Over」の日本語ボイスパックを好きな声でなるべく簡単に作成するための補助ツールです。
## 前提
### NET Runtime 6以上(起動しない場合は導入してください)
https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0  
上記URLの右側の一番下「.NET Runtime 6.X.XX」のインストーラーでインストール

### 音声合成ソフト(どれか一つまたは全て)
* VOICEVOX
https://voicevox.hiroshiba.jp/
* COEIROINK
https://coeiroink.com/  
どちらも無料で使用でき、多彩なキャラクターの音声ファイルが簡単に作成できます。  
利用規約をしっかり確認してからご使用ください。
* Style-Bert-VITS2
https://github.com/litagin02/Style-Bert-VITS2  
どちらかというと玄人向けですが、無料で使用することができ、高品質の音声を作成したり学習もできます。
### FaceFXWrapper(口パク用のLIPファイル作成に必要)
https://github.com/Nukem9/FaceFXWrapper  
ネクサスにもありますがこちらのほうがバージョンが新しいです。  
お好きな場所に展開してください。
### FonixData.cdf
上記のFaceFXWrapperとセットで使用します。FaceFXWrapper.exeがあるフォルダに入れてください。  
https://www.nexusmods.com/skyrimspecialedition/mods/40971  
または https://www.nexusmods.com/newvegas/mods/61248  
または DBVO作者のMathiewMay様のDiscordのページ
### Yakitori Audio Converter(FUZファイルの作成に必要)
https://www.nexusmods.com/skyrimspecialedition/mods/17765  
Main filesとOptional filesどちらも必要となります。  
お好きな場所に展開してください。  
(Optional FilesのBmlFuzToolsも同じフォルダに入れてください)
## 推奨
### DBVO Japanese Girl Voice Pack
Optional Filesの「DialogMenu JP Patch」  
https://www.nexusmods.com/skyrimspecialedition/mods/112293  
プレイヤーが話し終える前に会話が切り替わらないよう調整します。  
自分でdialoguemenu.swfを編集しない方は、こちらをおすすめします(ENDORSEもお忘れなく)

### VOICEVOXマルチエンジン機能対応ソフト
必須ではありませんがキャラクターの選択肢が増えます。要VOICEVOX本体
* VOICEVOX Nemo
https://voicevox.hiroshiba.jp/nemo/  
* SHAREVOICE
https://www.sharevox.app/  
ダウンロードを押して「ダウンロード選択」画面のVVPPを選択
* ITVOICE
http://itvoice.starfree.jp/  
右下の「VVPPファイルのダウンロードはこちら」

## 使い方
DBVO JPVoice Tool.exeを起動してください。各ボタンにカーソルを合わせると説明がでます。  
使いたい音声合成ソフトを起動してください(キャラクターの取得や音声(WAV)ファイルの作成のために必要になります)
### 基本的な流れ
* <バニラ音声の場合>
1. 音声合成ソフトを選んで「キャラ取得」ボタン → 好きなキャラクターを選ぶ → 「辞書(JSON)→音声(WAV)」ボタンを押す
バニラ音声の辞書ファイル5つを選択、出力先も選択 → できるまで待つ
2. 「WAV→LIPファイルを作成」ボタンを押す(1で作成したWAVファイルのフォルダ、出力先も同じ) → できるまで待つ
3. 「WAV→FUZファイルへ変換」ボタンを押す(1で作成したWAVファイルのフォルダ、出力先は自由) → できるまで待つ
4. 「ボイスパック作成」ボタンでZIPファイル出力
5. MOD管理ソフト(Mod Organizer2など)でインストール

バニラ音声の辞書ファイル(JSON)は本家MOD「Dragonborn Voice Over」の
Dragonborn Voice Over\DragonbornVoiceOver\locale_packs\jaの中の5つのファイルです。

※最初はツールの動作を確認するためにも全件読み込むととても時間がかかるので、小さめの辞書ファイルで試すか、
処理途中でキャンセルして、出力されたWAVファイルを視聴してみてください。

* <バニラ音声以外の場合(CCコンテンツやMOD)>
辞書ファイルが存在しないので完成度の高いDBVO Japanese Girl Voice Pack等のJSONファイルを利用する、または自作となります。  
自作する場合は「翻訳用XML→辞書(JSON)」ボタンで辞書ファイルを作成します。  
MODデータベースで提供されている翻訳用のXMLなどを利用してください。  
自作の翻訳でも構いません。実際にゲーム内で使われている文章と一致している必要があります。  

## 音声の精度をあげるコツ
音声合成ソフトにはそれぞれ辞書登録の機能がありますので、  
こちらにスカイリム関係の名詞を登録すれば、発音や読み方がかなり改善されます。  
ver.2.5.0.0より本ツールから辞書を読み込めるようになりました。  

## 既知の問題点や追加予定の機能
可変する文章は読みあげません(～ゴールドやミカエルと話してみよう等)  
 → 対処予定は今のところありません。個別で辞書ファイルを作るなど。なにか法則性があれば辞書ファイル作成時に盛り込めるかも？  
日本語翻訳にスペース＋記号が連続して入っていると読み上げない  
 → (対処)本ツールのメニューにある「翻訳用XMLから空白削除」をお試しください。なおプラグイン(.esp)もこのファイルで翻訳し直す必要があります。  

## 更新履歴
2024/03/08 ver.1.0.0.0 作成(非公開)  
2024/03/15 ver.2.0.0.0 音声合成ソフトとの連携、処理の途中キャンセル機能など追加  
2024/03/16 ver.2.1.0.1 辞書ファイル、WAVファイル作成時に複数ファイルの選択機能追加  
2024/03/17 ver.2.1.2.0 VOICEVOXのマルチエンジン対応ソフトを3つ追加(VOICEVOX Nemo、SHAREVOICE、ITVOICE)  
        翻訳用XMLから半角スペースを削除する機能追加  
2024/03/23 ver.2.3.0.0 音声合成＆学習ソフト「Style-Bert-VITS2」を追加  
        テキストファイルからWAV音声ファイルを作成する機能を追加  
        LIPとFUZファイル作成速度が少しだけ向上  
        その他、バグ修正など  

2024/03/24 ver.2.3.0.1 バグ修正  
﻿テキストファイルからWAV音声ファイルを作成する機能に置いて、  
﻿音声合成ソフトが正しく選択されていなかったのを修正  

2024/03/27 ver.2.4.0.0 機能追加  
﻿指定した文字列に該当するWAVファイルのみ作成する機能を追加(一部差し替えるための機能)  
﻿処理完了時メッセージを表示するオプション追加  
﻿WAV/LIP/FUZ一括処理追加  

2024/03/30 ver.2.5.0.0 機能追加  
﻿読み&アクセント辞書の読込み機能を追加  
﻿オプションに出力先の設定を追加  
﻿アプリケーション終了時に各種設定を保存  
﻿インターフェースの調整  
  
バグや要望などありましたらご連絡いただけると嬉しいです。  
またその際どんなデータを読み込んだか等の条件を教えてもらえると助かります。

## クレジット
MathiewMay様 「Dragonborn Voice Over」の作者様  
ヒホ（ヒロシバ）様 VOICEVOX、VOICEVOX Nemoの開発者様  
シロワニさん様 COEIROINKの開発者様  
litagin02様 Style-Bert-VITS2の開発者様  
Yちゃん様 SHAREVOICEの開発者様  
いたほび様 ITVOICEの開発者様  
Nukem9様 「FaceFXWrapper」の作者様  
BowmoreLover様 「Yakitori Audio Converter」「Lazy Voice Finder」他、たくさんの有用なツールの作者様  
Gonsuke様 「DBVO Japanese Girl Voice Pack」の作者様、沢山のMODに対応していて、ボイスパックを作る上で非常に参考になります。  
NewTestAc2様 「DBVO Japanese Voice Pack Proof of Concept」の作者様、可愛らしいボイスでかなり初期の頃から公開されています。  
shilohmc33様、DingraThePishvaz様 「FonixData.cdf」の提供  
Modデータベースで変換ツールをアップしてくれた作者様  
Modデータベースで翻訳をアップしてくれている有志の方々  
