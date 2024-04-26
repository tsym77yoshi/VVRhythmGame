using UnityEngine;
using vvproj;
using vvapi;
using System;
using System.Collections;
using System.Collections.Generic;

class VVNetworking : MonoBehaviour
{
  // 単一であることを保証
  public static VVNetworking instance;
  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  // 外部から呼ぶ
  public void talkCommunication(int styleid, string text, short order)
  {
    StartCoroutine(talkCommunicationIEnumerator(styleid, text, order));
  }
  public void trackCommunication(vvprojTrackType[] tracks, vvprojTempoType[] tempos, int tpqn, int target/*0なら選択キャラ、1ならバックコーラス*/)
  {
    StartCoroutine(trackCommunicationIEnumerator(tracks, tempos, tpqn, target));
  }

  private IEnumerator talkCommunicationIEnumerator(int styleid, string text, short order)
  {
    VoiceVoxApiClient client = new VoiceVoxApiClient();
    yield return client.TextToAudioClip(styleid, text);
    GameMain.Store.serifs[order] = client.AudioClip;
  }

  private bool isQueryGetting = false;
  private IEnumerator trackCommunicationIEnumerator(vvprojTrackType[] tracks, vvprojTempoType[] tempos, int tpqn, int target)
  {
    // 実行中状態にする
    isQueryGetting = true;

    VoiceVoxApiClient client = new VoiceVoxApiClient();
    if (target == 0)
    {
      GameMain.ScoreStore.queries = new vvapiMusicQuery[2];
    }
    else if (target == 1)
    {
      GameMain.Store.BackChorus = new List<AudioClip>();
    }

    for (int i = 0; i < tracks.Length; i++)
    {
      if (tracks[i].notes.Length != 0)
      {
        string communicationdata = NoteManager.FetchQuery(
          tracks[i].notes,
          tempos,
          tpqn,
          tracks[i].keyRangeAdjustment,// keyRangeAdjustment, 後で
          GameMain.Setting.restDurationSeconds// restDurationSeconds
        );
        Debug.Log(communicationdata);
        int singerStyleId = tracks[i].singer.styleId;
        // MusicToAudioClip の結果を待つ
        yield return client.ScoreToQuery(communicationdata);
        vvapiMusicQuery query = client.AudioMusicQuery;

        // 声量調整
        for (int j = 0; j < query.f0.Length; j++)
        {
          query.f0[j] = query.f0[j] * Math.Pow(2, tracks[i].volumeRangeAdjustment / 12);
        }

        if (target == 0)
        {
          GameMain.ScoreStore.queries[i] = query;
          yield return client.QueryToAudioClip(singerStyleId, query);
          GameMain.Store.clips[i] = client.AudioClip;
        }
        else if (target == 1)
        {
          yield return client.QueryToAudioClip(singerStyleId, query);
          GameMain.Store.BackChorus.Add(client.AudioClip);
        }
      }
    }
    isQueryGetting = false;
    yield break;
  }
}