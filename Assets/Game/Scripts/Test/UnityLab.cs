using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
        //Logger.Log("Hello");
    }

    #region Mono

    void Start()
    {
        Logger.Log("Application path:\n" + Application.dataPath + "\n" + Application.persistentDataPath + "\n"
            + Application.streamingAssetsPath + "\n" + Application.temporaryCachePath);

        Logger.Log("Layer:\n" + "Layer 0 to name: " + LayerMask.LayerToName(0) + "\nLayer 1 to name: " + LayerMask.LayerToName(1)
            + "\nLayer 100 to name will be out of boundary." + "\nName xxx to layer: " + LayerMask.NameToLayer("xxx")
            + "\nName Screen to layer: " + LayerMask.NameToLayer("Screen"));

        //Logger.LogWarning("Object asset path: " + AssetDatabase.GetAssetPath(Object));
        //Logger.LogWarning("Object asset scene path: " + AssetDatabase.GetAssetOrScenePath(Object));

        SendMessage();

        Logger.LogWarning("type-gettype() - " + Type.GetType("Window"));
        Logger.LogWarning("type-gettype() - " + Type.GetType("TemplateWindow"));

        gameObject.AddComponent(Type.GetType("TemplateWindow"));
        gameObject.AddComponent("TemplateWindow");
    }

    #endregion
}
