using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeamFormationController), true)]
public class TeamFormationControllerEditor : Editor
{
    private TeamFormationController controller;

    private void OnEnable()
    {
        controller = target as TeamFormationController;
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

    private void OnSceneGUI()
    {
        Handles.color = Color.black;
        controller.SpawnList.ForEach(spawnObject =>
        {
            Handles.Label(spawnObject.transform.position + Vector3.up * 0.2f, spawnObject.name);
        });

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
