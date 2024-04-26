using UnityEngine;
using UnityEngine.UI;
public class CharaSelectButton : MonoBehaviour
{
  public Texture2D portrait;
  public RawImage portrait_target;
  public void ApplyPortrait()
  {
    if (portrait_target != null && this.GetComponent<Toggle>().isOn)
    {
      portrait_target.texture = portrait;
      RectTransform rectTransform = portrait_target.gameObject.GetComponent<RectTransform>();
      float targetWidth = portrait.width * rectTransform.sizeDelta.y / portrait.height;
      float targetHeight = rectTransform.sizeDelta.y;/* 
      float targetHeight = portrait.height * rectTransform.sizeDelta.x / portrait.width;
      float targetWidth = rectTransform.sizeDelta.x; */

      rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
  }
}