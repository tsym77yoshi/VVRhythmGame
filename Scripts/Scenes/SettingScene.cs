using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingScene : MonoBehaviour
{
  [SerializeField]
  private Slider[] volumeSliders = new Slider[4];
  void Start()
  {
    for (short i = 0; i < Math.Min(volumeSliders.Length, GameMain.Store.volumes.Length); i++)
    {
      volumeSliders[i].value = GameMain.Store.volumes[i];
    }
    if (GameMain.ScoreStore.tracks.Length < 2)
    {
      volumeSliders[1].gameObject.SetActive(false);
    }
    if (GameMain.Store.backMusic == null)
    {
      volumeSliders[2].gameObject.SetActive(false);
    }
    if (GameMain.ScoreStore.tracks.Length < 3)
    {
      volumeSliders[3].gameObject.SetActive(false);
    }
  }
  public void changeVolumeSlider(GameObject sliderObj)
  {
    float volume = sliderObj.GetComponent<Slider>().value;
    if (sliderObj.name.Split("_")[0] == "volumeSlider")
    {
      GameMain.Store.volumes[int.Parse(sliderObj.name.Split("_")[1])] = volume;
    }
  }
}
