using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(PaticleDelay))]
public class ParticleDelayEditor : Editor
{
    private PaticleDelay paticleDelay;

    public override void OnInspectorGUI()
    {
        paticleDelay = target as PaticleDelay;
        base.OnInspectorGUI();
        //GUI.enabled = !paticleDelay.IsPlaying;
        if (GUILayout.Button("Play"))
        {
            if(!Application.isPlaying)
            {
                paticleDelay.Reset();
                paticleDelay.RegisterEvent();
            }
            else
            {
                paticleDelay.PlayOnRun();
            }
        }


        //GUI.enabled = paticleDelay.IsPlaying;
        if (GUILayout.Button("Stop"))
        {
            if (!Application.isPlaying)
            {
                paticleDelay.UnregisterEvent();
                paticleDelay.Stop();
            }
            else
            {
                paticleDelay.StopOnRun();
            }

        }
    }
	
}
