/// 参考にしたコード: https://qiita.com/Haruyama_Dev/items/5b8ac0260cdfeff47121

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using vvapi;
using System;

// キャラクターなどの取得
public class VoiceVoxInfoApiClient
{
  private const string BASE = "http://localhost:50021";
  private const string SPEAKERS_URL = BASE + "/speakers";
  private const string SINGERS_URL = BASE + "/singers";
  private const string SINGER_INFO_URL = BASE + "/singer_info";
  private string _json;
  public string Json { get => _json; }

  public IEnumerator GetSpeakers()
  {
    string webUrl = $"{SPEAKERS_URL}";
    yield return GetJson(webUrl);
  }
  public IEnumerator GetSingers()
  {
    string webUrl = $"{SINGERS_URL}";
    yield return GetJson(webUrl);
  }
  public IEnumerator GetSingerInfo(string speaker_uuid)
  {
    string webUrl = $"{SINGER_INFO_URL}?speaker_uuid={speaker_uuid}";
    yield return GetJson(webUrl);
  }
  private IEnumerator GetJson(string webUrl)
  {
    // POST通信
    using (UnityWebRequest request = new UnityWebRequest(webUrl, "GET"))
    {
      request.downloadHandler = new DownloadHandlerBuffer();
      // リクエスト（レスポンスがあるまで待機）
      yield return request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.ConnectionError)
      {
        // 接続エラー
        ReportWindow.reportBug("接続エラー:"+request.error);
        yield return "";
      }
      else
      {
        if (request.responseCode == 200)
        {
          // リクエスト成功
          _json = request.downloadHandler.text;
          yield return _json;
        }
        else
        {
          // リクエスト失敗
          ReportWindow.reportBug(request.responseCode + ", Description:" + request.downloadHandler.text);
          yield return "";
        }
      }
    }
  }
  public Texture2D Base64ToTexture(string base64Data)
  {
    // Base64データをバイト配列にデコード
    byte[] bytes = Convert.FromBase64String(base64Data);

    // バイト配列をTexture2Dに変換
    Texture2D texture = new Texture2D(1, 1);
    texture.LoadImage(bytes);
    return texture;
  }
}