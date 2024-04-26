using UnityEngine;
using UnityEngine.SceneManagement;

class NextScene : MonoBehaviour{
  public string sceneName = "";
  public void LoadScene()
  {
    if (sceneName == "") { return; }
    SceneManager.LoadScene(sceneName);
  }
}