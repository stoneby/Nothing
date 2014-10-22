using UnityEngine;

public class LevelUpEffectControl : MonoBehaviour
{
    public EffectSequnce EffectSequnce;
    public AudioSource LevelupSound;

    public void Play()
    {
        EffectSequnce.Play();
        LevelupSound.Play();
    }

    public void Stop()
    {
        EffectSequnce.Stop();
        LevelupSound.Stop();
    }

}
