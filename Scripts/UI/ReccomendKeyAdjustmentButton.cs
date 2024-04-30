using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

class ReccomendKeyAdjustmentButton :MonoBehaviour
{
  private int recommendKeyAdjustmentValue;
  private bool isAutoChange = false;
  [SerializeField]
  private TextMeshProUGUI recommendKeyAdjustmentText;
  [SerializeField]
  private Slider keyAdjustmentSlider;
  [SerializeField]
  private int order;// 何番目のキャラクターのボタンか
  [SerializeField]
  private ToggleGroup toggleGroup;
  void Start()
  {
    if (
      CharaKeyRangeAdjustmentValue.GetCharaKeyRangeAdjustmentValue(
        GameMain.ScoreStore.tracks[order].singer.styleId
      ) == GameMain.ScoreStore.tracks[order].keyRangeAdjustment
    )// もし元のファイルがおすすめ値を利用しているなら
    {
      isAutoChange = true;
    }
  }
  public void PressedButton()
  {
    if (recommendKeyAdjustmentValue == null) return;
    keyAdjustmentSlider.value = recommendKeyAdjustmentValue;
  }
  private Toggle formerToggle;
  public void Update()
  {
    Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
    if (toggle!=formerToggle)
    {
      selectedCharaChange(int.Parse(toggle.name.Split("_")[1]));
      formerToggle=toggle;
    }
  }
  private void selectedCharaChange(int styleId)// これはsingerのid
  {
    recommendKeyAdjustmentValue = CharaKeyRangeAdjustmentValue.GetCharaKeyRangeAdjustmentValue(styleId);
    recommendKeyAdjustmentText.text = recommendKeyAdjustmentValue.ToString();
    if (isAutoChange)
    {
      PressedButton();
    }
  }
}