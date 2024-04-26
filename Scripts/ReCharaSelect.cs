using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using GameMain;
using vvapi;

class ReCharaSelect : MonoBehaviour{
  public string sceneName = "";
  public void LoadScene()
  {
    if (sceneName == "") { return; }
    GameMain.Store.characters = new Character[2];
    GameMain.ScoreStore.queries = new vvapiMusicQuery[2];
    GameMain.Store.clips = new AudioClip[2];
    SceneManager.LoadScene(sceneName);
  }
}