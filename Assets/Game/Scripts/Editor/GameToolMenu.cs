using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool menu shows on unity menu bar.
/// </summary>
public static class GameToolMenu
{
    #region Open

    [MenuItem("Tool/Window Configurator", false, 0)]
    static public void OpenWindowConfigurator()
    {
        var window = EditorWindow.GetWindow<WindowConfigurator>(false, "Window Configurator", true);
    }

    [MenuItem("Tool/Window Prefab Generator", false, 0)]
    static public void OpenWindowPrefabGenerator()
    {
        EditorWindow.GetWindow<WindowPrefabGenerator>(false, "Window Prefab Generator", true);
    }

    #endregion
}
