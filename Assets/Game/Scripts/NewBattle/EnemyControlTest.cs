using System.Collections;
using UnityEngine;

public class EnemyControlTest : MonoBehaviour
{
    public EnemyControl EnemyController;

    private string currentValue = string.Empty;
    private string maxValue = string.Empty;
    private string cd = string.Empty;

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUILayout.Button("PlayAttack"))
        {
            EnemyController.PlayAttrack();
        } else if (GUILayout.Button("Play shake"))
        {
            EnemyController.PlayShake();
        } else if (GUILayout.Button("Play Big Shake"))
        {
            EnemyController.PlayBigShake();
        } else if (GUILayout.Button("Show Aim To"))
        {
            EnemyController.ShowAimTo(true);
        } else if (GUILayout.Button("Hide Aim To"))
        {
            EnemyController.ShowAimTo(false);
        } else if (GUILayout.Button("Show Blood Bar"))
        {
            EnemyController.ShowBlood(true);
        } else if (GUILayout.Button("Hide Blood Bar"))
        {
            EnemyController.ShowBlood(false);
        }

        currentValue = (GUILayout.TextField(currentValue, 40));
        maxValue = (GUILayout.TextField(maxValue, 40));
        if (GUILayout.Button("Set blood bar"))
        {
            EnemyController.SetBloodBar(float.Parse(currentValue), float.Parse(maxValue));
        }

        cd = (GUILayout.TextField(cd, 40));
        if (GUILayout.Button("Set cd label"))
        {
            EnemyController.SetCdLabel(int.Parse(cd));
        }
    }
#endif
}
