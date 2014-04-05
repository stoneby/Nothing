using UnityEngine;

public class EffectControl : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var cam = GameObject.Find("Camera");
        EffectManager.InitEffects(cam);
    }
}
