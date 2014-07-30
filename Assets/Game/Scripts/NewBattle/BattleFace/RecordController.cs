using UnityEngine;

public class RecordController : MonoBehaviour
{
    public UILabel Label;
    public UISprite Background;

    public float CurrentValue;
    public float TotalValue;

    public void Show()
    {
        Label.text = CurrentValue + "/" + TotalValue;
    }

    public void Reset()
    {
        CurrentValue = 0;
        TotalValue = 0;
    }
}
