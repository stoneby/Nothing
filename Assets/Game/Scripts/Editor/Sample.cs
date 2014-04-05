using UnityEditor;
using UnityEngine;

/// <summary>
/// Sample tool for reference.
/// </summary>
public class Sample : EditorWindow
{
    void OnGUI()
    {
        GUILayout.Label("This is a test.");
        GUILayout.Button("Hit");
    }
}
