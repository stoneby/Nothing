using UnityEditor;
using UnityEngine;

/// <summary>
/// Window configurator
/// </summary>
/// <remarks>
/// Write essential informations to xml, including:
/// - Window path and layer configuration.
/// </remarks>
public class WindowConfigurator : EditorWindow
{
    void OnGUI()
    {
        GUILayout.Label("This is a window configurator.\n Write essential informations to xml for game reference.");
    }
}
