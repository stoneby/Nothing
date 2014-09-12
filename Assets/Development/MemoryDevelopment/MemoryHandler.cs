using System;
using UnityEngine;

public class MemoryHandler : MonoBehaviour
{
    public GameObject ToBeCollected;
    public GameObject ToBeCollected2;

    void OnGUI()
    {
        if (GUILayout.Button("Destroy"))
        {
            Destroy(ToBeCollected);
        }

        if (GUILayout.Button("Destroy2"))
        {
            Destroy(ToBeCollected2);
        }

        if (GUILayout.Button("Unload Resources"))
        {
            Resources.UnloadUnusedAssets();
        }

        if (GUILayout.Button("GC cleanup"))
        {
            GC.Collect();
        }
    }
}
