using UnityEngine;

public class InBattleEffectTest : MonoBehaviour
{
    public EffectSpawner WarningController;

    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            WarningController.Play();
        }
    }
}
