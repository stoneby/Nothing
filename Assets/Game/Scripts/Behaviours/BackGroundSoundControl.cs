using UnityEngine;

public class BackGroundSoundControl : Singleton<BackGroundSoundControl>
{
    #region Public Fields

    public AudioSource BgSound;
    public float TransitionTime = 2;

    #endregion

    #region Public Methods

    public void PlaySound()
    {
        BgSound.volume = 0;
        BgSound.Play();
        iTween.AudioTo(BgSound.gameObject, 1, 1, TransitionTime);  
    }

    public void StopSound()
    {
        iTween.AudioTo(BgSound.gameObject, iTween.Hash("volume", 0, "time", TransitionTime, "oncomplete", "FadeOutFinished"));
    }

    #endregion

    #region Private Methods

    private void FadeOutFinished()
    {
        BgSound.Stop();
    }

    #endregion
}
