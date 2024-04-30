using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noteFlowSetting : MonoBehaviour
{
  private float starttime;
  private float duration;
  private float speed;
  private float adjustSeconds;
  float pos_x;
  float sound_pos_x;
  [SerializeField]
  private RectTransform noteObj;
  [SerializeField]
  private Slider hanteiSlider;
  [SerializeField]
  private RectTransform hanteiPosition;// audioSourceつき
  private Coroutine hanteiPlaySoundCoroutine;
  // Start is called before the first frame update
  void Start()
  {
    speed = GameMain.Store.speed;
    this.gameObject.GetComponent<Slider>().value = speed;
    hanteiSlider.value = GameMain.Store.adjustSeconds;
    pos_x = noteObj.localPosition.x;
    sound_pos_x = hanteiPosition.localPosition.x;
    noteFlowReset();
  }

  // Update is called once per frame
  void Update()
  {
    noteObj.localPosition = new Vector3(pos_x - speed * (Time.time - starttime), noteObj.localPosition.y);
    if (noteObj.localPosition.x < -100)
    {
      noteFlowReset();
    }
  }
  private void noteFlowReset()
  {
    starttime = Time.time;
    if (hanteiPlaySoundCoroutine != null)
    {
      StopCoroutine(hanteiPlaySoundCoroutine);
    }
    float noteArriveTime = (pos_x - sound_pos_x) / speed;
    hanteiPlaySoundCoroutine = StartCoroutine(playSound(noteArriveTime - adjustSeconds));
  }
  public void changeSpeed(float value)
  {
    speed = value;
    GameMain.Store.speed = speed;
    noteFlowReset();
  }
  IEnumerator playSound(float waitSeconds)
  {
    yield return new WaitForSeconds(waitSeconds);
    hanteiPosition.gameObject.GetComponent<AudioSource>().Play();
  }
  public void changeHantei(float value)
  {
    adjustSeconds = value;
    GameMain.Store.adjustSeconds = adjustSeconds;
    noteFlowReset();
  }
}
