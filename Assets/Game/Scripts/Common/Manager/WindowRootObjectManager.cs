using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Window game object hiarachy manager, which game object layer under NguiCamera object root.
/// - Screen.
/// - Popup.
/// - TabPanel.
/// - Face.
/// </summary>
public class WindowRootObjectManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Path layer mapping.
    /// </summary>
    public AbstractPathLayerMapping Mapping;

    /// <summary>
    /// Parent game object of all windows.
    /// </summary>
    public GameObject Root;

    #endregion

    #region Public Properties

    public Dictionary<WindowGroupType, GameObject> WindowObjectMap = new Dictionary<WindowGroupType, GameObject>();

    #endregion

    #region Mono

    void Awake()
    {
        if (Root == null)
        {
            Debug.LogError("Root game object should not be null.");
        }
    }

    void Start()
    {
        var trans = Root.transform;

        foreach (var layer in Mapping.LayerPathMap.Keys)
        {
            var layerName = LayerMask.LayerToName((int)layer);
            var layerTrans = trans.Find(layerName);
            if (layerTrans == null)
            {
                var layerObject = new GameObject();
                layerObject.transform.parent = trans;
                layerTrans = layerObject.transform;

                Debug.Log("Adding game object to root - " + trans.name);
            }

            layerTrans.name = layerName;
            layerTrans.gameObject.layer = (int)layer;

            Debug.Log("Game object is - " + layerTrans.name);

            WindowObjectMap[layer] = layerTrans.gameObject;
        }

        EventManager.Instance.Post(new WindowManagerReady());
    }

    #endregion
}
