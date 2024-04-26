using UnityEngine;
using UnityEngine.SceneManagement;
using vvproj;

class NextWithLoadAudio : MonoBehaviour{
  public string sceneName = "";
  public void LoadScene()
  {
    if (sceneName == "") { return; }
    int trackLength=GameMain.ScoreStore.tracks.Length;
    if (trackLength > 2)
    {
      vvprojTrackType[] sendTracks = new vvprojTrackType[trackLength - 2];
      for (int i = 0; i < trackLength - 2; i++)
      {
        sendTracks[i] = GameMain.ScoreStore.tracks[i + 2];
      }
      VVNetworking.instance.trackCommunication(sendTracks, GameMain.ScoreStore.tempos, GameMain.ScoreStore.tpqn, 1);
    }
    SceneManager.LoadScene(sceneName);
  }
}