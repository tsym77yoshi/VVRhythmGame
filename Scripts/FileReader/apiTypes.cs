using UnityEngine;
using System;
using System.Linq;

namespace vvapi
{
  public class vvapiSpeakerType
  {
    public object supported_features;
    public string name;
    public string speaker_uuid;
    public vvapiSpeakerStyleType[] styles;
    public string version;
  }
  [System.Serializable]
  public class vvapiSpeakerStyleType
  {
    public string name;
    public int id;
    public string type;
  }

  public class vvapiSpeakerInfo
  {
    public string policy;
    public string portrait;
    public vvapiSpeakerStyleInfo[] style_infos;
  }
  [System.Serializable]
  public class vvapiSpeakerStyleInfo
  {
    public int id;
    public string icon;
    public string portrait;// 存在しないときはnullが返ってくる
  }

  public class vvapiMusicQuery
  {
    public double[] f0;
    public double[] volume;
    public vvapiphoneme[] phonemes;
    public float volumeScale;
    public int outputSamplingRate;
    public bool outputStereo;
    public static vvapiMusicQuery CreateFromJSON(string jsonString)
    {
      return JsonUtility.FromJson<vvapiMusicQuery>(jsonString);
    }
    public byte[] ToJsonBytes()
    {
      return System.Text.Encoding.GetEncoding("utf-8").GetBytes(JsonUtility.ToJson(this));
    }
  }
  [System.Serializable]
  public class vvapiphoneme
  {
    public string phoneme;
    public int frame_length;
    public string ToStr()
    {
      return "{\"phoneme\":\"" + phoneme + "\",\"frame_length\":" + frame_length + "}";
    }
  }
}