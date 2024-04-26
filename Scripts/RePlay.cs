using UnityEngine;
using UnityEngine.SceneManagement;
using System;

class RePlay : MonoBehaviour
{
  public string sceneName = "";
  public void ReplayButton()
  {
    GameMain.Store.serifs = new AudioClip[2];

    short CharacterCount=(short)Math.Min(GameMain.ScoreStore.tracks.Length, 2);
    CharaSerif charaSerif = new CharaSerif();
    string[] uuids;
    int[] styleids;
    if (CharacterCount == 1)
    {
      uuids = new string[] { GameMain.Store.characters[0].speaker_uuid};
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