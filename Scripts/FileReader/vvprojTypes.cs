using UnityEngine;
using System;

namespace vvproj
{
  public class vvprojType
  {
    public string appVersion;
    public object talk;
    public vvprojSongType song;

    public static vvprojType CreateFromJSON(string jsonString)
    {
      return JsonUtility.FromJson<vvprojType>(jsonString);
    }
  }
  [System.Serializable]
  public class vvprojSongType
  {
    public int tpqn;
    public vvprojTempoType[] tempos;
    public vvprojTimeSignaturesType[] timeSignatures;
    public vvprojTrackType[] tracks;
  }
  [System.Serializable]
  public class vvprojTempoType
  {
    public int position;
    public int bpm;
  }
  [System.Serializable]
  public class vvprojTimeSignaturesType
  {
    public int measureNumber;
    public int beats;
    public int beatType;
  }
  [System.Serializable]
  public class vvprojTrackType
  {
    public vvprojSpeakerType singer;
    public int keyRangeAdjustment;
    public int volumeRangeAdjustment;
    public vvprojNoteType[] notes;
  }
  [System.Serializable]
  public class vvprojSpeakerType
  {
    public string engineId;
    public int styleId;
  }
  [System.Serializable]
  public class vvprojNoteType : IComparable<vvprojNoteType>
  {
    public string id;
    public int position;
    public int duration;
    public int noteNumber;
    public string lyric;
    // 並べ替えに使用
    public int CompareTo(vvprojNoteType other)
    {
      return position.CompareTo(other.position);
    }
  }
}