using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using vvproj;
using vvrojIsValid;
using GameMain;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
public class FileLoader : MonoBehaviour
{
  [SerializeField]
  private Button backMusicImportButton;
  [SerializeField]
  private Button nextButton;
  void Start()
  {
    var button = GetComponent<Button>();
    button.onClick.AddListener(() => OpenFile());
    backMusicImportButton.onClick.AddListener(() => OpenMusicFile());
  }

#if UNITY_EDITOR

  public void OpenFile()
  {
    // パスの取得
    var path = EditorUtility.OpenFilePanel("Open vvproj", "", "vvproj");
    if (string.IsNullOrEmpty(path))
    {
      return;
    }

    // 読み込み
    using (var reader = new StreamReader(path))
    {
      vvprojType saveData = vvprojType.CreateFromJSON(reader.ReadLine());
      ReadFile(saveData);
    }
  }
  public void OpenMusicFile(){

  }

#elif UNITY_WEBGL

  [DllImport("__Internal")]
  private static extern string FileLoad();
  [DllImport("__Internal")]
  private static extern string MusicFileLoad();

  public void OpenFile(){
    FileLoad();
  }
  public void onFileLoaded(string json){
    ReadFile(vvprojType.CreateFromJSON(json));
  }
  public void OpenMusicFile(){
    MusicFileLoad();
  }
  public void onMusicFileLoaded(byte[]musicData){
    GameMain.Store.backMusic = WavUtility.ToAudioClip(musicData, "backMusic");
    backMusicImportButton.transform.GetChild(0).gameObject.SetActive(true);
  }

#endif

  void ReadFile(vvprojType saveData)
  {
    // Scoreが実行可能か確認
    if (!ScoreValidator.IsValidScore(saveData.song))
    {
      ReportWindow.reportBug("ファイルが有効ではありません", false);
      return;
    }
    if (saveData.song.tracks[0].notes.Length == 0)
    {
      ReportWindow.reportBug("トラック0のスコアの中身がありません", false);
      return;
    }
    NoteManager noteManager = new NoteManager();
    // ノーツ作成
    List<GameNote> noteFlow = noteManager.makeGameNotes(
      saveData.song.tracks[0].notes,
      saveData.song.tempos,
      saveData.song.tpqn, GameMain.Setting.restDurationSeconds
    );
    if (saveData.song.tracks.Length > 1)
    {
      noteFlow = noteManager.adjustGameNotes(
        noteFlow,
        noteManager.makeGameNotes(
          saveData.song.tracks[1].notes,
          saveData.song.tempos,
          saveData.song.tpqn,
          GameMain.Setting.restDurationSeconds
        )
      );
    }
    GameMain.Store.noteFlow = noteFlow;

    // 音声
    GameMain.ScoreStore.tracks = saveData.song.tracks;
    GameMain.ScoreStore.tempos = saveData.song.tempos;
    GameMain.ScoreStore.tpqn = saveData.song.tpqn;

    // 「済」表示にする
    this.transform.GetChild(0).gameObject.SetActive(true);
    // interactiveをtrue
    nextButton.interactable = true;
  }
  bool IsLaterVersion(string userVersion, string searchVersion)
  {
    string[] userVersionNums = userVersion.Split(".");
    string[] searchVersionNums = searchVersion.Split(".");
    for (short i = 0; i < userVersionNums.Length; i++)
    {
      if (int.Parse(userVersionNums[i]) < int.Parse(searchVersionNums[i]))
      {
        return false;
      }
    }
    return true;
  }
}