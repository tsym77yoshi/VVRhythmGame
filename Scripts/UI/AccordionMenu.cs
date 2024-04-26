using UnityEngine;
using UnityEngine.UI;

class AccordionMenu : MonoBehaviour
{
  public void Accordion(bool value)
  {
    float height = this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
    if (value)
    {
      height *= this.transform.childCount - 2;
    }
    this.GetComponent<RectTransform>().sizeDelta =
      new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, height);
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.GetComponent<RectTransform>());
  }
}