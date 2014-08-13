using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool menu shows on unity menu bar.
/// </summary>
public static class BuildMenu
{
    #region Open

    [MenuItem("Package/Android", false, 0)]
    static public void BuildAndroid()
    {
        //EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
        BuildManager.BuildAndroid();
    }

    [MenuItem("Package/Window", false, 0)]
    static public void BuildExe()
    {
        //EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
        BuildManager.BuildExe();
    }

    [MenuItem("Package/IOS", false, 0)]
    static public void BuildIos()
    {
        //EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
        BuildManager.BuildIos();
    }

    [MenuItem("Package/Advance", false, 0)]
    static public void BuildAdvance()
    {
        //EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
        EditorWindow.GetWindow(typeof(PackageWindow));
    }

    [MenuItem("Package/Save Big Map", false, 0)]
    static public void SaveBigMap()
    {
        //EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
        EditorWindow.GetWindow(typeof(GetMapDataWindow));
    }

    #endregion
}
