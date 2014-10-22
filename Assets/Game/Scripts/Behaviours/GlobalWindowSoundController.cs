using UnityEngine;

public class GlobalWindowSoundController : Singleton<GlobalWindowSoundController>
{
    public AudioSource WindowOpenSound;
    public AudioSource WindowCloseSound;

    public AudioSource GetRewardSound;
    public AudioSource ExpGrowSound;
    public AudioSource BattleWinSound;
    public AudioSource BattleLoseSound;
    public AudioSource OpenBoxSound;

    public void PlayOpenSound()
    {
        WindowOpenSound.Play();
    }

    public void PlayCloseSound()
    {
        WindowCloseSound.Play();
    }

    public void PlayRewardSound()
    {
        GetRewardSound.Play();
    }

    public void PlayExpGrowSound()
    {
        ExpGrowSound.Play();
    }

    public void PlayWinSound()
    {
        BattleWinSound.Play();
    }

    public void PlayLoseSound()
    {
        BattleLoseSound.Play();
    }

    public void PlayOpenBoxSound()
    {
        OpenBoxSound.Play();
    }

    private void Awake()
    {
        if(WindowOpenSound == null || WindowCloseSound == null)
        {
            Logger.LogError("The Window open or close sound is null.");
        }
    }
}
