using UnityEngine;

public class NextFootManagerTest : MonoBehaviour
{
    public NextFootManager FootManager;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Make One Round"))
        {
            FootManager.MoveOneRound();
        }

        if (GUILayout.Button("Reset One Round"))
        {
            FootManager.ResetOneRound();
        }

        if (GUILayout.Button("Move"))
        {
            FootManager.Move();
        }
    }
#endif
}
