using UnityEngine;

public class BackgroundLooperTest : MonoBehaviour
{
    public BackgroundLooperVersion1 Looper;
    public BackgroundLooperVersion2 Looper2;

#if UNITY_EDITOR

    private void OnGUI()
    {
        GUILayout.Label("Background looper version 1 test: ");

        GUILayout.Space(1f);
        
        if (GUILayout.Button("Begin"))
        {
            Looper.Begin();
        }

        if (GUILayout.Button("Loop"))
        {
            Looper.Loop();
        }

        if (GUILayout.Button("End"))
        {
            Looper.End();
        }

        if (GUILayout.Button("Reset"))
        {
            Looper.Reset();
        }

        GUILayout.Space(5f);

        GUILayout.Label("Background looper version 2 test: ");

        GUILayout.Space(1f);

        if (GUILayout.Button("Begin"))
        {
            Looper2.Begin();
        }

        if (GUILayout.Button("Loop"))
        {
            Looper2.Loop();
        }

        if (GUILayout.Button("End"))
        {
            Looper2.End();
        }

        if (GUILayout.Button("Reset"))
        {
            Looper2.Reset();
        }
    }

#endif
}
