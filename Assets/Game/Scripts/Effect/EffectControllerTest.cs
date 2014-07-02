using UnityEngine;

public class EffectControllerTest : MonoBehaviour
{
    public EffectController ParticleController;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            ParticleController.Play(false);
        }

        if (GUILayout.Button("Play Loop"))
        {
            ParticleController.Play(true);
        }

        if (GUILayout.Button("Stop"))
        {
            ParticleController.Stop();
        }
    }
#endif
}
