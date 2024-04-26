using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using vvapi;

public class StartScene : MonoBehaviour
{
  void Start()
  {

    // リセット
    GameMain.ScoreStore.queries = new vvapiMusicQuery[2];
    GameMain.Store.serifs = new AudioClip[2];
    GameMain.Store.clips = new AudioClip[2];
    GameMain.Store.backMusic = null;
    GameMain.Store.BackChorus = new List<AudioClip>();
    GameMain.Store.noteFlow = new List<GameNote>();
    GameMain.Store.characters = new Character[2];
  }
}