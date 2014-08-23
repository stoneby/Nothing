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
        EditorWindow.GetWindow<WindowMappingXmlGeneratorWindow>(false, "Window Mapping XML Generator", true);
    }

    [MenuItem("Tool/Window Prefab Generator", false, 0)]
    static public void OpenWindowPrefabGenerator()
    {
        EditorWindow.GetWindow<WindowPrefabGeneratorWindow>(false, "Window Prefab Generator", true);
    }

    [MenuItem("Tool/Game Event Generator", false, 0)]
    static public void OpenGameEventGenerator()
    {
        EditorWindow.GetWindow<GameEventGenerator>(false, "Game Event Generator", true);
    }

    [MenuItem("Tool/Code Analyzer", false, 0)]
    static public void OpenCodeAnalyzer()
    {
        EditorWindow.GetWindow<CodeAnalyzerEditorWindow>(false, "Code analyzer", true);
    }

    [MenuItem("Tool/Switch Font", false, 0)]
    static public void OpenSwitchFont()
    {
        EditorWindow.GetWindow<SwitchFontEditorWindow>(false, "Switch Font", true);
    }

    [MenuItem("Tool/Localization Manager", false, 0)]
    static public void OpenAddLocalization()
    {
        EditorWindow.GetWindow<LocalizationManagerEditorWindow>(false, "Localization Manager", true);
    }

    [MenuItem("Tool/AtlasReplacer", false, 0)]
    static public void OpenAtlasReplacer()
    {
        EditorWindow.GetWindow<AtlasReplacerEditorWindow>(false, "Atlas Replacer", true);
    }

    [MenuItem("Tool/AtlasRenamer", false, 0)]
    static public void OpenAtlasRenamer()
    {
        EditorWindow.GetWindow<AtlasRenamerEditorWindow>(false, "Atlas Renamer", true);
    }

    #endregion
}
