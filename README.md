一応公開はしていますが、コードが汚すぎるのには注意してください😭（実力不足です）

このコードはUnityのAssetsの中身で、Unityのバージョンは2022.3.25f1です。

This game includes code derived from VOICEVOX, which is licensed under the GNU Lesser General Public License (LGPL) version 3.
Original VOICEVOX code and license details can be found at: https://github.com/Hiroshiba/voicevox

This game also includes the "源柔ゴシック" font, which is licensed under the SIL Open Font License (SIL OFL) version 1.1.
Details about the "源柔ゴシック" font can be found at: http://jikasei.me/font/genjyuu/

- VoiceVoxApiClient.cs
https://qiita.com/Haruyama_Dev/items/5b8ac0260cdfeff47121 の記事で歌唱もできるようにした上で色々いじったコードです。
2024/04/26現在ではソング機能は一度styleidが6000(波音リツさんのソング)からクエリを取得し、クエリを声量調整に応じていじった後、3000番代のハミングまたは6000番代のソングで歌ってもらうというものです。speakerに存在するキャラクターが必ずしもsingerに存在するとは限らないのでsinger一覧から取得しています。
- VoiceVoxInfoApiClient.cs
こちらは上記記事のキャラクター一覧やアイコン取得をするように改変したコードです。
画像はBase64のstringの形で取得されます。サンプルボイスも取得できるみたいです。
- VVNetWorking.cs
Startシーンのゲームオブジェクトにアタッチすると、dontdestroyonloadになるので、シーンを跨いで音声リクエストが出来ます
VVNetworking.instance.trackCommunication(○○とかでできるはず
- 歌唱関連のvvprojファイルの読み込み関連について
前提として、仕様が固定的なものでは無いのでやめた方が良いでしょう。
尚、0.18あたりの仕様ではnoteManager.csのFetchQuery(元のコードの関数名はfetchQueryだったはず)でやっています。また、始まる前後で無音を入れる必要があるので、同時に何かを流したりする場合には注意してください
- 調整に関係するノーツにするか否かの話について
調整の技として「っ」を入れることで音を切らないようにするというものがあるそうなので、「っ」をノーツにしていません
- フォントについて
GenJyuuGothicのHeavyをお借りしています
