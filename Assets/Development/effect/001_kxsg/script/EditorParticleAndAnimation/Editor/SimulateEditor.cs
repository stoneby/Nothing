using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Simulate))]
class SimulateEditor : Editor
{
    private Simulate simulator;

    private void OnEnable()
    {
        simulator = target as Simulate;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = !simulator.IsPlaying;
        if (GUILayout.Button("Start"))
        {
            simulator.Reset();
            simulator.RegisterEvent();
        }

        GUI.enabled = simulator.IsPlaying;
        if (GUILayout.Button("Stop"))
        {
            simulator.UnregisterEvent();
            simulator.Stop();
        }
    }
}
