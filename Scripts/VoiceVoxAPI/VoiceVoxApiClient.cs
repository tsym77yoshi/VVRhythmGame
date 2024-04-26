/// お借りしたコード: https://qiita.com/Haruyama_Dev/items/5b8ac0260cdfeff47121

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using vvapi;

/// <summary>
/// VOICEVOXのREST-APIクライアント
/// </summary>
public class VoiceVoxApiClient
{
  /// <summary> 基本 URL </summary>
  private const string BASE = "http://localhost:50021";
  /// <summary> 音声クエリ取得 URL </summary>
  private const string AUDIO_QUERY_URL = BASE + "/audio_query";
  private const string SYNTHESIS_URL = BASE + "/synthesis";
  /// <summary> 歌唱クエリ取得 URL </summary>
  private const string SONG_QUERY_URL = BASE + "/sing_frame_audio_query";
  private const string SONG_SYNTHESIS_URL = BASE + "/frame_synthesis";

  /// <summary> 音声クエリ（Byte配列） </summary>
  private byte[] _audioTalkQueryBytes;
  public byte[] AudioTalkQueryBytes { get => _audioTalkQueryBytes; }
  private vvapiMusicQuery _audioMusicQuery;
  public vvapiMusicQuery AudioMusicQuery { get => _audioMusicQuery; }
  /// <summary> 音声クリップ </summary>
  private AudioClip _audioClip;

  /// <summary> 音声クリップ </summary>
  public AudioClip AudioClip { get => _audioClip; }

  /// <summary>
  /// 指定したテキストを音声合成、AudioClipとして出力
  /// </summary>
  /// <param name="speakerId">話者ID</param>
  /// <param name="text">テキスト</param>
  /// <returns></returns>
  [Obsolete]
  public IEnumerator TextToAudioClip(int speakerId, string text)
  {
    // 音声クエリを生成
    yield return PostAudioQuery(speakerId, text);

    // 音声クエリから音声合成
    yield return PostSynthesis(speakerId, _audioTalkQueryBytes, SYNTHESIS_URL);
  }
  [Obsolete]
  public IEnumerator ScoreToQuery(string score)
  {
    // 音声クエリを生成
    yield return PostMusicQuery(score);
  }
  public IEnumerator QueryToAudioClip(int speakerId, vvapiMusicQuery query)
  {
    // 音声クエリから音声合成
    yield return PostSynthesis(speakerId, query.ToJsonBytes(), SONG_SYNTHESIS_URL);
  }

  /// <summary>
  /// 音声合成用のクエリ生成
  /// </summary>
  /// <param name="speakerId">話者ID</param>
  /// <param name="text">テキスト</param>
  /// <returns></returns>
  private IEnumerator PostAudioQuery(int speakerId, string text)
  {
    _audioMusicQuery = null;
    // URL
    string webUrl = $"{AUDIO_QUERY_URL}?speaker={speakerId}&text={text}";

    // POST通信
    using (UnityWebRequest request = new UnityWebRequest(webUrl, "POST"))
    {
      request.downloadHandler = new DownloadHandlerBuffer();
      // リクエスト（レスポンスがあるまで待機）
      yield return request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.ConnectionError)
      {
        // 接続エラー
        ReportWindow.reportBug("AudioQuery:" + request.error);
        yield return "Error";
      }
      else
      {
        if (request.responseCode == 200)
        {
          // リクエスト成功
          _audioTalkQueryBytes = request.downloadHandler.data;
          Debug.Log("AudioQuery:" + request.downloadHandler.text);
          yield return _audioTalkQueryBytes;
        }
        else
        {
          // リクエスト失敗
          ReportWindow.reportBug("AudioQuery:" + request.responseCode + ", Description:" + request.downloadHandler.text);
          yield return "Error";
        }
      }
    }
  }
  private IEnumerator PostMusicQuery(string music)
  {
    _audioMusicQuery = null;
    // speaker=6000の波音リツさんの歌唱データからしかクエリ生成は対応していない
    string webUrl = $"{SONG_QUERY_URL}?speaker=6000";

    // POST通信
    using (UnityWebRequest request = new UnityWebRequest(webUrl, "POST"))
    {
      //ヘッダーにタイプを設定
      request.SetRequestHeader("Content-Type", "application/json");
      //json(string)をbyte[]に変換
      byte[] bodyRaw = Encoding.UTF8.GetBytes(music);
      //jsonを設定
      request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
      request.downloadHandler = new DownloadHandlerBuffer();
      // リクエスト（レスポンスがあるまで待機）
      yield return request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.ConnectionError)
      {
        // 接続エラー
        ReportWindow.reportBug("AudioQuery:" + request.error);
      }
      else
      {
        if (request.responseCode == 200)
        {
          // リクエスト成功
          _audioMusicQuery = vvapiMusicQuery.CreateFromJSON(request.downloadHandler.text);
          Debug.Log("AudioQuery:" + request.downloadHandler.text);
        }
        else
        {
          // リクエスト失敗
          ReportWindow.reportBug("AudioQuery:" + request.responseCode);
        }
      }
    }
  }

  /// <summary>
  /// 音声合成
  /// </summary>
  /// <param name="speakerId">話者ID</param>
  /// <param name="audioQuery">音声クエリ(Byte配列)</param>
  /// <returns></returns>
  [Obsolete]
  private IEnumerator PostSynthesis(int speakerId, byte[] audioQuery, string URL)
  {
    _audioClip = null;
    // URL
    string webUrl = $"{URL}?speaker={speakerId}";
    // ヘッダー情報
    Dictionary<string, string> headers = new Dictionary<string, string>();
    headers.Add("Content-Type", "application/json");

    using (WWW www = new WWW(webUrl, audioQuery, headers))
    {
      // レスポンスが返るまで待機
      yield return www;

      if (!string.IsNullOrEmpty(www.error))
      {
        // エラー
        ReportWindow.reportBug("Synthesis : " + www.error);
      }
      else
      {
        // レスポンス結果をAudioClipで取得
        _audioClip = www.GetAudioClip(false, false, AudioType.WAV);
      }
    }
  }
}