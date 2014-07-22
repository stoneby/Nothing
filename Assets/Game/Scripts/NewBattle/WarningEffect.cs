using UnityEngine;
using System.Collections;

public class WarningEffect : MonoBehaviour
{
    #region Public Fields

    public GameObject WarningBg1;
    public GameObject WarningBg2;
    public GameObject WarningText;

    public ParticleSystem WarningParticle;

    #endregion

    #region Public Methods

    public float Duration
    {
        get { return 3f; }
    }

    public void Play()
    {
        StartCoroutine(DoPlay());
    }

    #endregion

    #region Private Methods

    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(.4f);
        PlayWarningAnimation(0.2f);
        yield return new WaitForSeconds(0.6f);

        WarningParticle.Play(true);

        yield return new WaitForSeconds(1.8f);
        PlayWarningEndAnimation();
        yield return new WaitForSeconds(.4f);

        HideWaring();
    }

    private void PlayWarningAnimation(float playtime)
    {
        WarningBg1.SetActive(true);
        WarningBg2.SetActive(true);

        var tp1 = WarningBg1.GetComponent<TweenPosition>();
        tp1.duration = playtime;
        tp1.PlayForward();

        var tp2 = WarningBg2.GetComponent<TweenPosition>();
        tp2.duration = playtime;
        tp2.PlayForward();

        WarningText.SetActive(true);

        var ts = WarningText.GetComponent<TweenScale>();
        var tex = WarningText.GetComponent<UITexture>();
        tex.alpha = 0;
        ts.delay = playtime;
        ts.duration = playtime;
        ts.PlayForward();

        var ta = WarningText.GetComponent<TweenAlpha>();
        ta.delay = playtime;
        ta.PlayForward();
    }

    private void PlayWarningEndAnimation()
    {
        WarningParticle.Stop();

        var ts = WarningText.GetComponent<TweenScale>();
        ts.delay = 0;
        ts.PlayReverse();
        var ta = WarningText.GetComponent<TweenAlpha>();
        ta.delay = 0;
        ta.PlayReverse();

        var effectbg1 = WarningBg1;
        var effectbg2 = WarningBg2;

        var tp1 = effectbg1.GetComponent<TweenPosition>();
        tp1.PlayReverse();

        var tp2 = effectbg2.GetComponent<TweenPosition>();
        tp2.PlayReverse();
    }

    private void HideWaring()
    {
        WarningBg1.SetActive(false);
        WarningBg2.SetActive(false);
        WarningText.SetActive(false);
    }

    #endregion
}
