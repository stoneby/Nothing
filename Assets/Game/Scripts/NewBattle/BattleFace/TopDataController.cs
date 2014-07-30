using UnityEngine;

/// <summary>
/// Top data controller in top right of battle face.
/// </summary>
public class TopDataController : MonoBehaviour
{
    public UILabel BoxLabel;
    public UILabel GoldLabel;

    public float BoxCount;
    public float GoldCount;

    public void Show()
    {
        BoxLabel.text = "" + BoxCount;
        GoldLabel.text = "" + GoldCount;
    }

    public void Reset()
    {
        BoxCount = 0;
        GoldCount = 0;
    }
}
