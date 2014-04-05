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

    [MenuItem("Tool/Sample", false, 0)]
    static public void OpenSample()
    {
        EditorWindow.GetWindow<Sample>(false, "Sample", true);
    }

    #endregion
}
