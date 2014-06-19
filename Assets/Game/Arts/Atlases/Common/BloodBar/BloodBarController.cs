using UnityEngine;

/// <summary>
/// Blood bar controller.
/// </summary>
public class BloodBarController : MonoBehaviour
{
    public GameObject BloodBar;
    public GameObject BloodLabel;

    private bool isShowBlood = true;

    public float CurrentValue;
    public float MaxValue;

    public void ShowBlood(bool flag)
    {
        if (isShowBlood != flag)
        {
            isShowBlood = flag;
            BloodBar.SetActive(isShowBlood);
            BloodLabel.SetActive(isShowBlood);
        }
    }

    public void ShowValue()
    {
        var sd = BloodBar.GetComponent<UISlider>();
        sd.value = CurrentValue / MaxValue;

        var lb = BloodLabel.GetComponent<UILabel>();
        lb.text = CurrentValue + "/" + MaxValue;
    }

}
