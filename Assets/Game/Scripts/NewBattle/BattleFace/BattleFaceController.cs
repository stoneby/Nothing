using UnityEngine;

public class BattleFaceController : MonoBehaviour
{
    public NextFootManager FootManager;
    public MagicBarController HPController;
    public MagicBarController MPController;
    public TopDataController TopController;

    public PopMenuController PopController;
    public LeaderGroupController LeaderController;

    public RecordController StepRecord;
    public StarController StarController;

    public UILabel AttackLabel;

    public void Reset()
    {
        HPController.Reset();
        MPController.Reset();
        FootManager.Reset();
        TopController.Reset();
        PopController.Reset();
        LeaderController.Reset();
        StepRecord.Reset();
        StarController.Reset();
        ResetAttackLabel();
    }

    public void ResetAttackLabel()
    {
        SetAttackLabel(string.Empty);
    }

    public void SetAttackLabel(string text)
    {
        AttackLabel.text = text;
    }

    public void OnMenuButtonClicked()
    {
        var visible = PopController.IsVisible;
        PopController.Show(!visible);
    }
}
