using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically map between path and layer.
/// </summary>
public class AutoPathLayerMapping : AbstractPathLayerMapping
{
    private string absolutePath;

    private Dictionary<string, List<string>> windowMapDict;

    #region AbstractPathLayerMapping

    public override bool Load()
    {
        PathLayerMap.Clear();
        LayerPathMap.Clear();

        foreach (var pair in windowMapDict)
        {
            var group = pair.Key;
            var prefabList = pair.Value;
            foreach (var prefabPath in prefabList)
            {
                var className = Utils.GetNameFromPath(prefabPath);
                var windowType = Type.GetType(Utils.PrefabNameToWindow(className));

                Debug.LogWarning("Window type - " + windowType + " with name " + className);

                PathTypeMap[prefabPath] = windowType;
                TypePathMap[windowType] = prefabPath;

                var windowGroupType = (WindowGroupType)Enum.Parse(typeof(WindowGroupType), group);
                PathLayerMap[prefabPath] = windowGroupType;
                if (!LayerPathMap.ContainsKey(windowGroupType))
                {
                    LayerPathMap[windowGroupType] = new List<string>();
                }
                LayerPathMap[windowGroupType].Add(prefabPath);
            }
        }

        Display();
        return true;
    }

    #endregion

    private void Display()
    {
        foreach (var pair in PathLayerMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in LayerPathMap)
        {
            foreach (var value in pair.Value)
            {
                Debug.Log("pair: key-" + pair.Key + ", value-" + value);
            }
        }

        foreach (var pair in PathTypeMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }

        foreach (var pair in TypePathMap)
        {
            Debug.Log("pair: key-" + pair.Key + ", value-" + pair.Value);
        }
    }

    #region Mono

    void Awake()
    {
        PathLayerMap = new Dictionary<string, WindowGroupType>();
        LayerPathMap = new Dictionary<WindowGroupType, List<string>>();
        TypePathMap = new Dictionary<Type, string>();
        PathTypeMap = new Dictionary<string, Type>();

        windowMapDict = Utils.ReadWindowMapFromXml();
        Load();
    }

    #endregion
}
