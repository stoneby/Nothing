using UnityEngine;

public class CameraLikeEffectTest : MonoBehaviour
{
    public CameraLikeEffect Effect;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Look Into"))
        {
            Effect.LookInto();
        }

        if (GUILayout.Button("Look Out"))
        {
            Effect.LookOut();
        }

        if (GUILayout.Button("Look Around"))
        {
            Effect.LookAround();
        }

        if (GUILayout.Button("Shake"))
        {
            Effect.Shake();
        }
    }
#endif
}
