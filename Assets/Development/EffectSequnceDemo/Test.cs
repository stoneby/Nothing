using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public EffectSequnce EffectSequnce;
    public void OnGUI()
    {
        if(GUILayout.Button("Play"))
        {
            EffectSequnce.Play();
        }
        if(GUILayout.Button("Stop"))
        {
            EffectSequnce.Stop();
        }
    }
}
