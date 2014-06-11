using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeamFormationController), true)]
public class TeamFormationControllerEditor : Editor
{
    private TeamFormationController controller;

    private void OnEnable()
    {
        controller = target as TeamFormationController;

        if (string.IsNullOrEmpty(controller.XmlName))
        {
            Logger.LogError("Xml name should not be null.");
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Spawn"))
        {
            controller.Spawn();
        }

        if (GUILayout.Button("Clean"))
        {
            controller.Clean();
        }

        if (GUILayout.Button("Generate Xml"))
        {
            controller.WriteXml();
        }

        if (GUILayout.Button("Load"))
        {
            controller.ReadXml();
        }
    }
}
