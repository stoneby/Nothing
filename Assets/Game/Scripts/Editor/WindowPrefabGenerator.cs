using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Window prefab generator tool.
/// </summary>
public class WindowPrefabGenerator : EditorWindow
{
    #region Private Fields

    private string prefabName;
    private WindowGroupType windowGroup;

    #endregion

    #region Mono

    void OnGUI()
    {
        prefabName = EditorGUILayout.TextField("Name: ", prefabName);
        windowGroup = (WindowGroupType)EditorGUILayout.EnumPopup("Window Group: ", windowGroup);
        GUILayout.Button("Hit");
    }

    #endregion
}
