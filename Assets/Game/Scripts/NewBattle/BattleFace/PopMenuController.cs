using UnityEngine;

public class PopMenuController : MonoBehaviour
{
    public UILabel EnergyLabel;
    public UILabel FPLabel;

    public float EnergyCount;
    public float FPCount;

    public bool IsVisible
    {
        get { return gameObject.activeSelf; }
    }

    public void Show(bool visible)
    {
        gameObject.SetActive(visible);

        if (visible)
        {
            ShowData();
        }
    }

    private void ShowData()
    {
        EnergyLabel.text = "" + EnergyCount;
        FPLabel.text = "" + FPCount;
    }

    public void Reset()
    {
        EnergyCount = 0;
        FPCount = 0;
    }
}
