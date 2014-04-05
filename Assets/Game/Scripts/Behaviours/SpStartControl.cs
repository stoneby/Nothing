using UnityEngine;
using System.Collections;

public class SpStartControl : MonoBehaviour
{
    public GameObject BlackTex;
    public GameObject CharacterTex;
    public GameObject Background;
    public GameObject GlowTex;

    private float tempdelay;

    public void Play(float delay)
    {
        tempdelay = delay;
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        var wd = gameObject.GetComponent<UIWidget>();
        wd.alpha = 1;
        Background.SetActive(false);
        GlowTex.SetActive(false);
        yield return new WaitForSeconds(tempdelay);
        Background.SetActive(true);

        var tp = BlackTex.AddComponent<TweenPosition>();
        tp.duration = 0.2f;
        tp.from = new Vector3(-180, -35, 0);
		tp.to = new Vector3 (-15, -15 * 35 / 180,0);
        tp.PlayForward();

        yield return new WaitForSeconds(0.2f);
		Destroy (tp);

		tp = BlackTex.AddComponent<TweenPosition>();
		tp.from = new Vector3 (-15, -15 * 35 / 180, 0);
		tp.to = new Vector3(30, 30 * 35 / 180,0);
		tp.duration = 2.0f;
		tp.PlayForward ();
        tp = CharacterTex.AddComponent<TweenPosition>();
        tp.from = new Vector3(-180, -35, 0);
		tp.to = new Vector3(10, 10 * 35 / 180,0);
        tp.duration = 0.3f;
        tp.PlayForward();

        var ta = CharacterTex.AddComponent<TweenAlpha>();
        ta.from = 0;
        ta.to = 1;
        ta.duration = 0.6f;
        ta.ResetToBeginning();
        ta.PlayForward();

        yield return new WaitForSeconds(0.6f);
		Destroy (tp);


		tp = CharacterTex.AddComponent<TweenPosition>();
		tp.from = new Vector3 (10, 10 * 35 / 180, 0);
		tp.to = new Vector3(25, 25 * 35 / 180,0);
		tp.duration = 1.6f;
		tp.PlayForward ();

		yield return new WaitForSeconds (0.3f);
        GlowTex.SetActive(true);
        ta = GlowTex.AddComponent<TweenAlpha>();
		ta.from = 1;
		ta.to = 0;
		ta.duration = 1.3f;
		ta.PlayForward ();

		var ts = GlowTex.AddComponent<TweenScale>();
		ts.from = new Vector3 (1.0f, 1.0f, 1);
		ts.to = new Vector3 (3, 3, 1);
		ts.duration = 1.4f;
		ts.PlayForward ();

        GlowTex.SetActive(true);
        ta = GlowTex.AddComponent<TweenAlpha>();
        ta.from = 1;
        ta.to = 0;
        ta.duration = 1.3f;
        ta.PlayForward();

        ts = GlowTex.AddComponent<TweenScale>();
        ts.from = new Vector3(0.5f, 0.5f, 1);
        ts.to = new Vector3(2, 2, 1);
        ts.duration = 1.4f;
        ts.PlayForward();

        yield return new WaitForSeconds(0.4f);
        ta = gameObject.AddComponent<TweenAlpha>();
        ta.duration = 0.3f;
        ta.from = 1;
        ta.to = 0;
        ta.ResetToBeginning();
        ta.PlayForward();
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
