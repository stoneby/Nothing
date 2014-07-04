using UnityEngine;

public class PaticleDelayPlayer : MonoBehaviour
{
    //[HideInInspector]
    //public bool IsPlaying;
    private PaticleDelay[] particleDelays ;
    public void Reset()
    {
        particleDelays = GetComponentsInChildren<PaticleDelay>(true);
        foreach (var particleDelay in particleDelays)
        {
            particleDelay.Reset();
        }
    }

    public void RegisterEvent()
    {
        //IsPlaying = true;
        particleDelays = GetComponentsInChildren<PaticleDelay>(true);
        foreach (var particleDelay in particleDelays)
        {
            particleDelay.RegisterEvent();
        }
    }

    public void UnRegisterEvent()
    {
        //IsPlaying = false;
        particleDelays = GetComponentsInChildren<PaticleDelay>(true);
        foreach (var particleDelay in particleDelays)
        {
            particleDelay.UnregisterEvent();
        }
    }


    public void Play ()
	{
        foreach (var particleDelay in particleDelays)
        {
            particleDelay.Play();
        }
	}

    public void Stop()
    {
        foreach (var particleDelay in particleDelays)
        {
            particleDelay.Stop();
        }
    }
}
