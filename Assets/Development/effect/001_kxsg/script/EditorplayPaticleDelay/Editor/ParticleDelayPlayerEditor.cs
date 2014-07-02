using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(PaticleDelayPlayer))]
public class ParticleDelayPlayerEditor  : Editor
{
    private PaticleDelayPlayer paticleDelayPlayer;

    public override void OnInspectorGUI()
    {
        paticleDelayPlayer = target as PaticleDelayPlayer;
        base.OnInspectorGUI();
        //GUI.enabled = !paticleDelayPlayer.IsPlaying;
        if (GUILayout.Button("Play"))
        {
            paticleDelayPlayer.Reset();
            paticleDelayPlayer.RegisterEvent();
        }


        //GUI.enabled = paticleDelayPlayer.IsPlaying;
        if (GUILayout.Button("Stop"))
        {
            paticleDelayPlayer.UnRegisterEvent();
            paticleDelayPlayer.Stop();
        }
    }
}
