using System;
using UnityEngine;

public class MemoryHandler : MonoBehaviour
{
    public GameObject ToBeCollected;
    public GameObject ToBeCollected2;

    public GameObject Ref;

    void OnGUI()
    {
        if (GUILayout.Button("Destroy"))
        {
            Ref = ToBeCollected;
            Destroy(ToBeCollected);
            //ToBeCollected = null;
        }

        if (GUILayout.Button("Destroy2"))
        {
            Destroy(ToBeCollected2);
            //ToBeCollected2 = null;
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
