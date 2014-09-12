using UnityEngine;

public class MonsterControlTest : MonoBehaviour
{
    public MonsterControl MonsterController;

    private string currentValue = string.Empty;
    private string maxValue = string.Empty;
    private string cd = string.Empty;
    private string highlight = string.Empty;

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUILayout.Button("PlayAttack"))
        {
            MonsterController.PlayAttack();
        }
        else if (GUILayout.Button("Play shake"))
        {
            MonsterController.PlayShake();
        }
        else if (GUILayout.Button("Play Big Shake"))
        {
            MonsterController.PlayBigShake();
        }
        else if (GUILayout.Button("Show Aim To"))
        {
            MonsterController.ShowAimTo(true);
        }
        else if (GUILayout.Button("Hide Aim To"))
        {
            MonsterController.ShowAimTo(false);
        }
        else if (GUILayout.Button("Show Blood Bar"))
        {
            MonsterController.ShowBlood(true);
        }
        else if (GUILayout.Button("Hide Blood Bar"))
        {
            MonsterController.ShowBlood(false);
        }

        currentValue = (GUILayout.TextField(currentValue, 40));
        maxValue = (GUILayout.TextField(maxValue, 40));
        if (GUILayout.Button("Set blood bar"))
        {
            MonsterController.SetBloodBar(float.Parse(currentValue), float.Parse(maxValue));
        }

        cd = (GUILayout.TextField(cd, 40));
        if (GUILayout.Button("Set cd label"))
        {
            MonsterController.SetCdLabel(int.Parse(cd));
        }

        highlight = (GUILayout.TextField(highlight, 40));
        if (GUILayout.Button("Set highlight"))
        {
        }
    }
#endif
}
