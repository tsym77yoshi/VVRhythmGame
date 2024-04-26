using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using vvapi;
using vvproj;

namespace GameMain
{
  public class GameNote : IComparable<GameNote>
  {
    public float StartTime;
    public float EndTime;// shortNoteならStartとEnd一致させるべし
    // [firstChara] 0~3, 10~13(long) [SecondChara] 20~23, 30~33(long)
    public short type;
    public GameNote(float startTime, float endTime, short Type)
    {
      StartTime = startTime;
      EndTime = endTime;
      type = Type;
    }
    // 並べ替えに使用
    public int CompareTo(GameNote other)
    {
      return (int)(StartTime.CompareTo(other.StartTime) * 100);
    }
  }
  public class Character
  {
    public string speaker_uuid;
    public int speaker_id;
    public int singer_id;
    public Texture2D portrait;
  }
  public static class Store
  {
    public static AudioClip[] serifs = new AudioClip[2];

    public static AudioClip[] clips = new AudioClip[2];
    public static AudioClip backMusic = null;
    public static List<AudioClip> BackChorus = new List<AudioClip>();// track数が3以上で適用
    public static Character[] characters = new Character[2];

    public static List<GameNote> noteFlow = new List<GameNote>();

    public static float speed = 200;
    public static float adjustSeconds = 0;// 判定調節。正ならnoteが流れてくるのが遅くなる
  }
  public static class Setting
  {
    public static float restDurationSeconds = 1;// 音を最初にどれだけ開けるか
    public static float minLongNoteSecond = 1;// 一音の長さがどれより長いとロングノーツにするか
  }
  public static class ScoreStore
  {
    public static vvprojTrackType[] tracks;
    public static vvapiMusicQuery[] queries = new vvapiMusicQuery[2];
    public static vvprojTempoType[] tempos;
    public static int tpqn;
  }
  public static class ResultStore
  {
    public static short[] results = new short[4];
    public static string[] evaluation()
    {
      short totalNote = 0;
      foreach (short result in results)
      {
        totalNote += result;
      }
      if (totalNote == results[0])
      {
        return new string[]{"allexce"};
      }
      if (results[3] == 0)
      {
        return new string[]{"fullcom"};
      }
      if (results[0] + results[1] >= totalNote * 0.8){
        return new string[]{"great01","great02"};
      }
      if(results[0]+results[1]>totalNote*0.5){
        return new string[]{"notgood"};
      }
      return new string[]{"mystery"};
    }
    // Excellent, Great, Good, Miss
    // 0.1s 0.15s 0.2s over
  }
}