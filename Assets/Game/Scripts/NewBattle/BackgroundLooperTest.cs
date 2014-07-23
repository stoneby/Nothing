using UnityEngine;

public class BackgroundLooperTest : MonoBehaviour
{
    public AbstractBattlegroundLooper Looper;

#if UNITY_EDITOR

    private void OnGUI()
    {
        GUILayout.Label("Background looper version 1 test: ");

        GUILayout.Space(1f);

        if (GUILayout.Button("Begin"))
        {
            Looper.PlayBegin();
        }

        if (GUILayout.Button("Once"))
        {
            Looper.PlayOnce();
        }

        if (GUILayout.Button("Loop"))
        {
            Looper.PlayLoop();
        }

        if (GUILayout.Button("End"))
        {
            Looper.PlayEnd();
        }

        if (GUILayout.Button("Reset"))
        {
            Looper.Reset();
        }

        GUILayout.Space(5f);
    }

#endif
}
