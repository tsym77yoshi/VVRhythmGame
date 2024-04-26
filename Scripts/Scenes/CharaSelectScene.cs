using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using GameMain;
using vvapi;
using System.Linq;
using UnityEngine.SceneManagement;
using vvproj;

public class CharaSelectScene : MonoBehaviour
{
  [SerializeField]
  private Transform[] CharacterSet = new Transform[2];
  private short CharacterCount = 1;// tracksに2キャラいたら2になる

  [SerializeField]
  private GameObject CharacterButtonTemplate;
  [SerializeField]
  private GameObject CharacterStyleButtonTemplate;

  private VoiceVoxInfoApiClient client = new VoiceVoxInfoApiClient();
  void Start()
  {
    CharacterCount = (short)Math.Min(GameMain.ScoreStore.tracks.Length, 2);
    if (CharacterCount == 2)
    {
      RectTransform rectTransform = CharacterSet[0].gameObject.GetComponent<RectTransform>();
      rectTransform.sizeDelta = new Vector2(rectTransform.rect.x, -270f);
      CharacterSet[1].gameObject.SetActive(true);
    }
    for (int i = 0; i < CharacterCount; i++)
    {
      CharacterSet[i].Find("keyAdjustment").gameObject.GetComponent<Slider>().value = GameMain.ScoreStore.tracks[i].keyRangeAdjustment;
      CharacterSet[i].Find("voiceShiftKey").gameObject.GetComponent<Slider>().value = GameMain.ScoreStore.tracks[i].volumeRangeAdjustment;
    }
    StartCoroutine(networking());
  }
  // シーン開始時の通信
  private IEnumerator networking()
  {
    yield return client.GetSingers();
    // ボタンの追加処理
    vvapiSpeakersType vvapiSpeakersData = JsonUtility.FromJson<vvapiSpeakersType>("{\"vvapiSpeakers\":"+client.Json+"}");
    vvapiSpeakerType[] singers =vvapiSpeakersData.vvapiSpeakers;
    List<Transform>[] CharacterButtons = { new List<Transform>(), new List<Transform>() };


    string result = "";
    int indexa = 0;
    foreach (vvapiSpeakerType singer in singers)
    {
      if (false)
      {
        result += "VOICEVOX:" + singer.name + ", ";
        if (indexa % 3 == 2) result += "\n";
        indexa++;

        VoiceVoxApiClient client = new VoiceVoxApiClient();
        AudioSource aus = this.gameObject.AddComponent<AudioSource>();
        yield return client.TextToAudioClip(singer.styles[0].id - 3000, "VOICEVOXソング機能リリース、おめでとうございます！");
        aus.clip = client.AudioClip;
      }
      for (int i = 0; i < CharacterCount; i++)
      {
        Transform CharacterButtonParent = CharacterSet[i].Find("Scroll View").Find("Viewport").Find("Content");
        ToggleGroup CharacterButtonToggleGroup = CharacterSet[i].Find("ToggleGroup").GetComponent<ToggleGroup>();
        GameObject CharacterButton = Instantiate(CharacterButtonTemplate);

        CharacterButton.transform.SetParent(CharacterButtonParent, false);
        CharacterButton.transform.GetChild(0).GetComponent<Toggle>().group = CharacterButtonToggleGroup;
        CharacterButton.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = singer.name;
        CharacterButton.transform.GetChild(0).gameObject.name = $"{singer.speaker_uuid}_{singer.styles[0].id}_{singer.styles[0].id - 3000}";


        short index = 0;
        foreach (vvapiSpeakerStyleType speakerStyle in singer.styles)
        {
          GameObject CharacterStyleButton = Instantiate(CharacterStyleButtonTemplate);

          CharacterStyleButton.transform.SetParent(CharacterButton.transform, false);
          CharacterStyleButton.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, CharacterStyleButton.GetComponent<RectTransform>().sizeDelta.y * index);
          CharacterStyleButton.GetComponent<Toggle>().group = CharacterButtonToggleGroup;
          if (speakerStyle.type == "frame_decode")
          {
            CharacterStyleButton.name = $"{singer.speaker_uuid}_{speakerStyle.id}_{speakerStyle.id - 3000}_style";
            CharacterStyleButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = speakerStyle.name;
          }
          else if (speakerStyle.type == "sing")
          {
            int speaker_id = singer.styles[0].id - 3000;
            CharacterStyleButton.name = $"{singer.speaker_uuid}_{speakerStyle.id}_{speaker_id}_style";
            CharacterStyleButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = speakerStyle.name + "(ソング)";
          }
          else Debug.Log(speakerStyle.type);
          index++;
        }
        CharacterButtons[i].Add(CharacterButton.transform);
      }
    }
    if (false)
    {
      Debug.Log(result);
      AudioSource[] auses = this.gameObject.GetComponents<AudioSource>();
      foreach (AudioSource aus in auses)
      {
        aus.Play();
      }
    }

    // アイコンの追加処理
    for (short i = 0; i < singers.Length; i++)
    {
      yield return client.GetSingerInfo(singers[i].speaker_uuid);
      vvapiSpeakerInfo speaker_info = JsonUtility.FromJson<vvapiSpeakerInfo>(client.Json);
      for (short chara = 0; chara < CharacterCount; chara++)
      {
        Transform CharacterButtonOpen = CharacterButtons[chara][i];
        Transform CharacterButton = CharacterButtonOpen.GetChild(0);
        RawImage PortraitTarget = CharacterSet[chara].Find("Portrait").GetComponent<RawImage>();

        CharacterButton.GetChild(2).GetComponent<RawImage>().texture = client.Base64ToTexture(speaker_info.style_infos[0].icon);
        CharacterButton.gameObject.GetComponent<CharaSelectButton>().portrait = client.Base64ToTexture(speaker_info.portrait);
        CharacterButton.gameObject.GetComponent<CharaSelectButton>().portrait_target = PortraitTarget;

        string singer_uuid = CharacterButton.name.Split("_")[0];
        for (short j = 0; j < speaker_info.style_infos.Length; j++)
        {
          Transform styleButtonTransform = CharacterButtonOpen.GetChild(j + 3);
          if (styleButtonTransform != null)
          {
            styleButtonTransform.GetChild(2).GetComponent<RawImage>().texture = client.Base64ToTexture(speaker_info.style_infos[j].icon);
            if (speaker_info.style_infos[j].portrait == "")
            {
              styleButtonTransform.gameObject.GetComponent<CharaSelectButton>().portrait = client.Base64ToTexture(speaker_info.portrait);
            }
            else
            {
              styleButtonTransform.gameObject.GetComponent<CharaSelectButton>().portrait = client.Base64ToTexture(speaker_info.style_infos[j].portrait);
            }
            styleButtonTransform.gameObject.GetComponent<CharaSelectButton>().portrait_target = PortraitTarget;

            // test用コード
            if (false)
            {
              string[] finishedsingeruuid = {
                ""
              };
              bool isSkip = false;
              foreach (string matchtext in finishedsingeruuid)
              {
                if (matchtext == singer_uuid)
                {
                  isSkip = true;
                }
              }
              if (!isSkip)
              {
                CharaSerif cs = new CharaSerif();
                VoiceVoxApiClient vvapi = new VoiceVoxApiClient();
                AudioSource aus = this.gameObject.AddComponent<AudioSource>();
                foreach (string serifScene in new string[] { "start01", "start02", "start03", "fullcom", "great01", "great02", "so-sosc", "worsesc" })
                {
                  string speechText = cs.getSpeechText(singer_uuid, new string[] { serifScene }, -1, "");
                  yield return vvapi.TextToAudioClip(speaker_info.style_infos[j].id, speechText);
                  aus.clip = vvapi.AudioClip;
                  aus.Play();
                  while (aus.isPlaying)
                  {
                    yield return null;
                  }
                }
              }
            }
          }
        }
      }
    }
  }
  private IEnumerator GetSpeakerId()
  {
    yield return client.GetSpeakers();
    vvapiSpeakerType[] speakers = JsonUtility.FromJson<vvapiSpeakerType[]>(client.Json);
    // 話すときの処理
  }

  [SerializeField]
  private string sceneName = "";
  public void LoadScene()// 次のシーンへ移動
  {
    for (short i = 0; i < CharacterCount; i++)
    {
      ToggleGroup CharacterButtonToggleGroup = CharacterSet[i].Find("ToggleGroup").GetComponent<ToggleGroup>();
      GameObject selectedObj = CharacterButtonToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
      GameMain.Store.characters[i] = new GameMain.Character();
      GameMain.Store.characters[i].speaker_uuid = selectedObj.name.Split('_')[0];
      GameMain.Store.characters[i].speaker_id = int.Parse(selectedObj.name.Split('_')[2]);
      GameMain.Store.characters[i].singer_id = int.Parse(selectedObj.name.Split('_')[1]);
      GameMain.Store.characters[i].portrait = selectedObj.GetComponent<CharaSelectButton>().portrait;

      GameMain.ScoreStore.tracks[i].singer.styleId = GameMain.Store.characters[i].singer_id;
      GameMain.ScoreStore.tracks[i].keyRangeAdjustment = (int)CharacterSet[i].Find("keyAdjustment").gameObject.GetComponent<Slider>().value;
      GameMain.ScoreStore.tracks[i].volumeRangeAdjustment = (int)CharacterSet[i].Find("voiceShiftKey").gameObject.GetComponent<Slider>().value;
    }
    vvprojTrackType[] sendTracks;
    if (CharacterCount == 1)
    {
      sendTracks = new vvprojTrackType[] { GameMain.ScoreStore.tracks[0] };
    }
    else
    {
      sendTracks = new vvprojTrackType[] { GameMain.ScoreStore.tracks[0], GameMain.ScoreStore.tracks[1] };
    }
    VVNetworking.instance.trackCommunication(sendTracks, GameMain.ScoreStore.tempos, GameMain.ScoreStore.tpqn, 0);

    // セリフ取得処理
    CharaSerif charaSerif = new CharaSerif();
    string[] uuids;
    int[] styleids;
    if (CharacterCount == 1)
    {
      uuids = new string[] { GameMain.Store.characters[0].speaker_uuid };
      styleids = new int[] { GameMain.Store.characters[0].speaker_id };
    }
    else
    {
      uuids = new string[]{
        GameMain.Store.characters[0].speaker_uuid,
        GameMain.Store.characters[1].speaker_uuid
      };
      styleids = new int[] {
        GameMain.Store.characters[0].speaker_id,
        GameMain.Store.characters[1].speaker_id
      };
    }
    string[] texts = charaSerif.getSpeechTexts(uuids, new string[] { "start01", "start02", "start03" }, styleids);
    for (short i = 0; i < CharacterCount; i++)
    {
      // もしトーク以外のものをとるならここ
      VVNetworking.instance.talkCommunication(styleids[i], texts[i], i);
    }

    SceneManager.LoadScene(sceneName);
  }
}
class vvapiSpeakersType
{
  public vvapiSpeakerType[] vvapiSpeakers;
}