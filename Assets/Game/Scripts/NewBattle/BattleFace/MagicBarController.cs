using UnityEngine;

/// <summary>
/// Bar controller to bars like hp / mp in face of the battle.
/// </summary>
public class MagicBarController : MonoBehaviour
{
    public UISprite ForgroundBar;
    public UISprite BackgroundBar;

    public RecordController DataController;

    public float CurrentValue
    {
        get { return DataController.CurrentValue; }
        set
        {
            DataController.CurrentValue = value;

            // make sure current value less than or equal to total value.
            if (DataController.CurrentValue > DataController.TotalValue)
            {
                DataController.CurrentValue = DataController.TotalValue;
            }
        }
    }

    public float TotalValue
    {
        get { return DataController.TotalValue; }
        set
        {
            DataController.TotalValue = value;
        }
    }

    private int totalWidth;

    public void ShowForgroundBar(float value)
    {
        ShowBar(ForgroundBar, value);

        // set total label.
        CurrentValue = value;
        DataController.Show();
    }

    public void ShowBackgroundBar(float value)
    {
        ShowBar(BackgroundBar, CurrentValue + value);
    }

    public void Reset()
    {
        DataController.Reset();

        if (totalWidth <= 0)
        {
            totalWidth = ForgroundBar.width;
        }
    }

    private void ShowBar(UIWidget bar, float value)
    {
        if (value > TotalValue)
        {
            value = TotalValue;
        }

        // set bar width.
        bar.width = (int)((totalWidth) * value / TotalValue);
    }
}
