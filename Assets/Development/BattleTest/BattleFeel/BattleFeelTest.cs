using UnityEngine;

public class BattleFeelTest : MonoBehaviour
{
    public TeamSelectController TeamController;

    public UISlider FactorSlider;
    public UILabel FactorLabel;

    public void OnValueChanged()
    {
        var factor = FactorSlider.value / 2 + 0.5f;
        FactorLabel.text = "" + factor;
        TeamController.ColliderFactor = factor;
    }
}
