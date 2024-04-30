using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using GameMain;

[RequireComponent(typeof(AudioSource))]
public class GameScene : MonoBehaviour
{
  [SerializeField]
  private RectTransform hanteiBar;
  [SerializeField]
  private RectTransform NoteParent;
  [SerializeField]
  private GameObject notePrefab;
  [SerializeField]
  private GameObject longNotePrefab;
  [SerializeField]
  private Sprite[] sprites = new Sprite[4];
  [SerializeField]
  private float CharaUpAnimationLength = 0.15f;
  [SerializeField]
  private RawImage[] portraits = new RawImage[2];
  private Animator[] animators = new Animator[2];
  [SerializeField]
  private Animator hanteiAnimator;

  [SerializeField]
  private float[] HanteiRanges = { 0.05f, 0.08f, 0.1f };

  [SerializeField]
  private GameObject NowLoading;

  private List<GameNote>[] notes = {
    new List<GameNote>(),
    new List<GameNote>(),
    new List<GameNote>(),
    new List<GameNote>()
  };
  private List<RectTransform>[] noteObjs = {
    new List<RectTransform>(),
    new List<RectTransform>(),
    new List<RectTransform>(),
    new List<RectTransform>()
  };

  private float noteFlowTime;// ノーツが流れるまでの時間
  private float speed;
  private AudioSource[] audioSources;
  private float StartTime;
  private float HanteiTime;
  void Start()
  {
    speed = GameMain.Store.speed;
    noteFlowTime = (notePrefab.GetComponent<RectTransform>().position.x - hanteiBar.position.x) / speed;
    audioSources = this.GetComponents<AudioSource>();
    if (GameMain.Store.characters[1] != null)
    {
      portraits[1].gameObject.SetActive(true);
      RawImage temp = portraits[0];
      portraits[0] = portraits[1];
      portraits[1] = temp;
    }
    for (short i = 0; i < 2; i++)
    {
      if (GameMain.Store.characters[i] != null)
      {
        if (GameMain.Store.characters[i].portrait != null)
        {
          portraits[i].texture = GameMain.Store.characters[i].portrait;

          RectTransform rectTransform = portraits[i].gameObject.GetComponent<RectTransform>();
          float targetWidth = GameMain.Store.characters[i].portrait.width * rectTransform.sizeDelta.y / GameMain.Store.characters[i].portrait.height;
          float targetHeight = rectTransform.sizeDelta.y;

          rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
        }
      }
    }
    for (short i = 0; i < 2; i++)
    {
      animators[i] = portraits[i].GetComponent<Animator>();
    }

    StartCoroutine(game());
    // result
    for (short i = 0; i < GameMain.ResultStore.results.Length; i++)
    {
      GameMain.ResultStore.results[i] = 0;
    }
  }
  void Update()
  {
    KeyCode[] keyCodes = { KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow };
    for (short i = 0; i < keyCodes.Length; i++)
    {
      if (Input.GetKeyDown(keyCodes[i]))
      {
        OnKeyDown(i);
      }
      if (Input.GetKeyUp(keyCodes[i]))
      {
        OnKeyUp(i);
      }
    }

    float time_duration = Time.time - StartTime;
    float pos_x = notePrefab.transform.localPosition.x;
    for (short i = 0; i < noteObjs.Length; i++)
    {
      if (notes[i].Count > 0)
      {
        float HanteiDuration = Time.time - HanteiTime;
        if (notes[i][0].longState)// ロングノーツの前の方が終わっているやつはEndTimeで判定
        {
          HanteiDuration -= notes[i][0].EndTime;
        }
        else
        {
          HanteiDuration -= notes[i][0].StartTime;
        }
        if (HanteiDuration > HanteiRanges[HanteiRanges.Length - 1])
        {
          RemoveNote(i, (short)(HanteiRanges.Length));
        }
      }
      for (short j = 0; j < noteObjs[i].Count; j++)
      {
        noteObjs[i][j].localPosition = new Vector2(pos_x - speed * (time_duration - notes[i][j].StartTime), noteObjs[i][j].localPosition.y);
      }
    }
  }
  void OnKeyDown(short typeDirection)
  {
    typeDirection = (short)(typeDirection % 10);
    if (notes[typeDirection].Count == 0) return;
    if (notes[typeDirection][0].longState) return;// longNoteで既に処理が終わっているものならば
    float timeDifference = notes[typeDirection][0].StartTime - (Time.time - HanteiTime);
    for (short i = 0; i < HanteiRanges.Length; i++)
    {
      if (HanteiRanges[i] > Math.Abs(timeDifference))
      {
        if (notes[typeDirection][0].type % 20 >= 10)// longNoteなら
        {
          notes[typeDirection][0].longState = true;
          HanteiResult(typeDirection, i);
        }
        else
        {
          RemoveNote(typeDirection, i);
        }
        break;
      }
    }
  }
  void OnKeyUp(short typeDirection)
  {
    typeDirection = (short)(typeDirection % 10);
    if (notes[typeDirection].Count == 0) return;
    if (!notes[typeDirection][0].longState) return;// longNoteの前の方の処理が終わっていなければ
    float timeDifference = notes[typeDirection][0].EndTime - (Time.time - HanteiTime);
    for (short i = 0; i < HanteiRanges.Length; i++)
    {
      if (HanteiRanges[i] > Math.Abs(timeDifference))
      {
        RemoveNote(typeDirection, i);
        return;
      }
    }
    RemoveNote(typeDirection, (short)(HanteiRanges.Length));// Miss
  }
  void RemoveNote(short typeDirection, short result)
  {
    HanteiResult(typeDirection, result);
    Destroy(noteObjs[typeDirection][0].gameObject);
    notes[typeDirection].RemoveAt(0);
    noteObjs[typeDirection].RemoveAt(0);
  }
  void HanteiResult(short typeDirection, short result)
  {
    hanteiAnimator.SetTrigger($"result_0{result}");
    Debug.Log(result);
    GameMain.ResultStore.results[result]++;
  }

  IEnumerator game()
  {
    // 喋り
    while (GameMain.Store.serifs[0] == null || (GameMain.Store.serifs[1] == null && GameMain.Store.characters[1] != null))
    {
      yield return null;
    }
    AudioSource talkaus = this.gameObject.AddComponent<AudioSource>();
    StartCoroutine(playCharacterVoice(talkaus));
    // 歌
    while (true)
    {
      short finishedLength = 0;
      short requiredLength = (short)GameMain.ScoreStore.tracks.Length;
      foreach (AudioClip ac in GameMain.Store.clips)
      {
        if (ac != null)
        {
          finishedLength++;
        }
      }
      foreach (AudioClip ac in GameMain.Store.BackChorus)
      {
        if (ac != null)
        {
          finishedLength++;
        }
      }
      if (requiredLength != finishedLength)
      {
        yield return null;
      }
      else
      {
        break;
      }
    }
    Destroy(NowLoading);
    while (talkaus.isPlaying)
    {
      yield return null;
    }
    Debug.Log("Game Start!");
    float waitSeconds = GameMain.Store.adjustSeconds - noteFlowTime;
    if (waitSeconds >= 0)
    {
      playAudioSources();
      yield return new WaitForSeconds(waitSeconds);
    }
    else
    {
      StartCoroutine(WaitPlayAudioSource(-waitSeconds));
    }
    StartTime = Time.time;
    HanteiTime = StartTime + noteFlowTime;

    // キャラの動き
    StartCoroutine(CharaMotion());

    // ノーツ生成
    foreach (GameNote note in GameMain.Store.noteFlow)
    {
      while (note.StartTime > Time.time - StartTime)
      {
        yield return null;
      }
      notes[note.type % 10].Add(note);

      GameObject newChild = Instantiate(notePrefab);

      newChild.transform.SetParent(NoteParent, false);
      newChild.transform.localPosition = notePrefab.transform.position - new Vector3(0, 30 * (note.type % 10), 0);
      newChild.GetComponent<Image>().sprite = sprites[note.type % 10];
      if (note.type % 20 >= 10)// longNoteなら
      {
        float noteWidth = speed * (note.EndTime - note.StartTime);

        GameObject newLongChild = Instantiate(longNotePrefab);
        newLongChild.transform.SetParent(newChild.transform, false);
        RectTransform rectTransform = newLongChild.GetComponent<RectTransform>();
        float targetHeight = rectTransform.sizeDelta.y;
        rectTransform.sizeDelta = new Vector2(noteWidth, targetHeight);
        newLongChild.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        GameObject newLongNoteChild = Instantiate(notePrefab, newChild.transform);
        newLongNoteChild.transform.SetParent(newChild.transform, false);
        newLongNoteChild.GetComponent<Image>().sprite = sprites[note.type % 10];
        newLongNoteChild.GetComponent<RectTransform>().anchoredPosition = new Vector2(noteWidth, 0);
      }
      noteObjs[note.type % 10].Add(newChild.GetComponent<RectTransform>());
    }

    // これでゲームは終了
    while (true)// ノーツ側で検証
    {
      short remainNoteCount = 0;
      foreach (List<GameNote> note in notes)
      {
        remainNoteCount += (short)note.Count;
      }
      if (remainNoteCount == 0)
      {
        break;
      }
      yield return null;
    }
    /* AudioSource[] isPlayingAudioSources = this.GetComponents<AudioSource>();
    while (true)// 音声側で検証
    {
      bool isPlayFinished = false;
      foreach (AudioSource aus in isPlayingAudioSources)
      {
        if (aus.isPlaying == true){
          isPlayFinished = true;
          break;
        }
      }
      yield return null;
    } */
    // GameNoteのおかたずけ
    foreach(GameNote note in GameMain.Store.noteFlow){
      note.longState = false;
    }

    // TODO: audio生成開始 badcode
    CharaSerif charaSerif = new CharaSerif();
    short CharacterCount = (short)Math.Min(GameMain.ScoreStore.tracks.Length, 2);
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
    string[] texts = charaSerif.getSpeechTexts(uuids, GameMain.ResultStore.evaluation(), styleids);
    for (short i = 0; i < CharacterCount; i++)
    {
      // もしトーク以外のものをとるならここ
      VVNetworking.instance.talkCommunication(styleids[i], texts[i], i);
    }

    // TODO: full combo/ all excellentの実装
    yield return new WaitForSeconds(2);// 余韻
    SceneManager.LoadScene("06.Result");
  }

  // 開始時の音声再生
  IEnumerator playCharacterVoice(AudioSource aus)
  {
    aus.clip = GameMain.Store.serifs[0];
    aus.Play();
    while (aus.isPlaying)
    {
      yield return null;
    }
    if (GameMain.Store.serifs[1] != null)
    {
      aus.clip = GameMain.Store.serifs[1];
      aus.Play();
      while (aus.isPlaying)
      {
        yield return null;
      }
    }
    GameMain.Store.serifs = new AudioClip[2];
  }

  // キャラクターのモーション
  IEnumerator CharaMotion()
  {
    foreach (GameNote note in GameMain.Store.noteFlow)
    {
      float tempTime = note.StartTime + HanteiTime - CharaUpAnimationLength;
      while (tempTime > Time.time)
      {
        yield return null;
      }
      short charaType = (short)(note.type < 20 ? 0 : 1);
      animators[charaType].SetTrigger("Up");
      if (note.type % 20 < 10)
      {// 短ノーツなら
        animators[charaType].SetTrigger("Down");
      }
      else
      {
        animators[charaType].ResetTrigger("Down");
        StartCoroutine(CharaDownMotion(charaType, note.EndTime));
      }
    }
  }
  IEnumerator CharaDownMotion(short charaType, float time)
  {
    float tempTime = time + HanteiTime;
    while (tempTime > Time.time)
    {
      yield return null;
    }
    animators[charaType].SetTrigger("Down");
  }

  // 音関連
  IEnumerator WaitPlayAudioSource(float waitSeconds)
  {
    yield return new WaitForSeconds(waitSeconds);
    playAudioSources();
    yield return new WaitForSeconds(GameMain.Setting.restDurationSeconds);
    audioSources[2].volume = GameMain.Store.volumes[2];
    playAudioSource(GameMain.Store.backMusic, audioSources[2]);
  }
  void playAudioSources()
  {
    for (short i = 0; i < GameMain.Store.clips.Length; i++)
    {
      audioSources[i].volume = GameMain.Store.volumes[i];
      playAudioSource(GameMain.Store.clips[i], audioSources[i]);
    }
    for (short i = 0; i < GameMain.Store.BackChorus.Count; i++)
    {
      AudioClip audioClip = GameMain.Store.BackChorus[i];
      AudioSource aus = this.gameObject.AddComponent<AudioSource>() as AudioSource;
      aus.volume = GameMain.Store.volumes[3];
      playAudioSource(audioClip, aus);
    }
  }
  void playAudioSource(AudioClip audioClip, AudioSource audioSource)
  {
    if (audioClip != null)
    {
      audioSource.PlayOneShot(audioClip);
    }
  }
}