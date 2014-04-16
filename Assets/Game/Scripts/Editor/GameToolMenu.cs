using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool menu shows on unity menu bar.
/// </summary>
public static class GameToolMenu
{
    #region Open

    [MenuItem("Tool/Window Mapping XML Generator", false, 0)]
    static public void OpenWindowConfigurator()
    {
        var window = EditorWindow.GetWindow<WindowMappingXmlGenerator>(false, "Window Mapping XML Generator", true);
    }

    [MenuItem("Tool/Window Prefab Generator", false, 0)]
    static public void OpenWindowPrefabGenerator()
    {
        EditorWindow.GetWindow<WindowPrefabGenerator>(false, "Window Prefab Generator", true);
    }

    #endregion
}
