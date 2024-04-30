using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using GameMain;
using TMPro;
using unityroom.Api;

public class ResultScene : MonoBehaviour
{
  [SerializeField]
  private Transform noteResultParent;
  [SerializeField]
  private TextMeshProUGUI resultPanel;
  [SerializeField]
  private GameObject[] charaPortraitParent = new GameObject[2];
  [SerializeField]
  private Button[] NextButtons;

  void Start()
  {
    RawImage[] portraits = charaPortraitParent[0].transform.GetComponentsInChildren<RawImage>();
    if (GameMain.Store.characters[1] != null)
    {
      charaPortraitParent[0].SetActive(false);
      charaPortraitParent[1].SetActive(true);
      portraits = charaPortraitParent[1].GetComponentsInChildren<RawImage>();
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
    StartCoroutine(ResultDisplay());
  }
  private float[] resultValue = { 1, 0.8f, 0.5f, 0 };
  IEnumerator ResultDisplay()
  {
    int[] noteResultDisplay = { 0, 0, 0, 0 };
    while (true)
    {
      short isAllEnd = 0;
      for (short i = 0; i < GameMain.ResultStore.results.Length; i++)
      {
        if (noteResultDisplay[i] == GameMain.ResultStore.results[i])
        {
          isAllEnd++;
        }
        else
        {
          noteResultDisplay[i] = Math.Min(GameMain.ResultStore.results[i], noteResultDisplay[i] + 3);
          noteResultParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = noteResultDisplay[i].ToString();
        }
      }
      if (isAllEnd == GameMain.ResultStore.results.Length)
      {
        break;
      }
      else
      {
        yield return null;
      }
    }

    int noteCount = 0;
    foreach (short item in GameMain.ResultStore.results)
    {
      noteCount += item;
    }
    double resultMax = 12000 - 2000 / (double)noteCount;
    double result = 0;
    for (short i = 0; i < GameMain.ResultStore.results.Length; i++)
    {
      result += resultMax * GameMain.ResultStore.results[i] * resultValue[i] / noteCount;
    }
    int trueResult = (int)(result);
    // resultを表示
    int resultDisplayNum = 0;
    while (true)
    {
      resultDisplayNum = Math.Min(resultDisplayNum + 123, trueResult);
      resultPanel.text=resultDisplayNum.ToString();
      if (resultDisplayNum == trueResult)
      {
        break;
      }
      yield return null;
    }
    Debug.Log("result:" + (int)(result));
    SendScore(trueResult);
    foreach(Button nextButton in NextButtons)
    {
      nextButton.interactable = true;
    }
    // playVoice
    AudioSource aus = this.gameObject.AddComponent<AudioSource>();
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

  public void SendScore(int trueResult){
    if(trueResult!=null){
      UnityroomApiClient.Instance.SendScore(1, (float)trueResult, ScoreboardWriteMode.HighScoreDesc);
    }
  }
}