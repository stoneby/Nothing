using UnityEngine;

/// <summary>
/// Top data controller in top right of battle face.
/// </summary>
public class TopDataController : MonoBehaviour
{
    public UILabel BoxLabel;
    public UILabel HeroLabel;

    public float BoxCount;
    public float HeroCount;

    public void Show()
    {
        BoxLabel.text = "" + BoxCount;
        HeroLabel.text = "" + HeroCount;
    }

    public void Reset()
    {
        BoxCount = 0;
        HeroCount = 0;

        Show();
    }
}
