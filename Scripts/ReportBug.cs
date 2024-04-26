using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public static class ReportWindow{
  
#if UNITY_EDITOR

  public static void reportBug(string message, bool doesMoveToStart = true){
    Debug.LogError(message);
    if(doesMoveToStart){
      SceneManager.LoadScene("01.Start");
    }
  }

#elif UNITY_WEBGL

  [DllImport("__Internal")]
  private static extern string ReportBugWebGL(string message);

  public static void reportBug(string message, bool doesMoveToStart = true){
    ReportBugWebGL(message);
    if(doesMoveToStart){
      SceneManager.LoadScene("01.Start");
    }
  }

#endif
}