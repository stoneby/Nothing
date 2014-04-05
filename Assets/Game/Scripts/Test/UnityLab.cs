﻿using UnityEditor;
using UnityEngine;

/// <summary>
/// Lab for debugging or logging or testing unity APIs.
/// </summary>
public class UnityLab : MonoBehaviour
{
    public Object Object;

    private void SendMessage()
    {
        gameObject.SendMessage("Hello", SendMessageOptions.DontRequireReceiver);
    }

    void Hello()
    {
        //Debug.Log("Hello");
    }

    #region Mono

    void Start()
    {
        Debug.Log("Application path:\n" + Application.dataPath + "\n" + Application.persistentDataPath + "\n"
            + Application.streamingAssetsPath + "\n" + Application.temporaryCachePath);

        Debug.Log("Layer:\n" + "Layer 0 to name: " + LayerMask.LayerToName(0) + "\nLayer 1 to name: " + LayerMask.LayerToName(1)
            + "\nLayer 100 to name will be out of boundary." + "\nName xxx to layer: " + LayerMask.NameToLayer("xxx")
            + "\nName Screen to layer: " + LayerMask.NameToLayer("Screen"));

        Debug.LogWarning("Object asset path: " + AssetDatabase.GetAssetPath(Object));
        Debug.LogWarning("Object asset scene path: " + AssetDatabase.GetAssetOrScenePath(Object));

        SendMessage();
    }

    #endregion
}
