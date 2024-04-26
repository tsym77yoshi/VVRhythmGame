using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeTextByValue : MonoBehaviour
{
  public TextMeshProUGUI tmpro;
  public void changeTextByValueInt(float value)
  {
    tmpro.text=((int)(value)).ToString();
  }
}