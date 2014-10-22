using System.Collections;
using UnityEngine;

/// <summary>
/// Top data controller in top right of battle face.
/// </summary>
public class TopDataController : MonoBehaviour
{
    public UILabel BoxLabel;
    public UILabel HeroLabel;

    public EffectSpawner BoxEffect;
    public EffectSpawner HeroEffect;
    public EffectSpawner GoldEffect;
    public EffectSpawner SparkleEffect;

    public GameObject EffectParent;

    public float OverlayDuration;
    public float SparkleSpeed;

    public float TotalDuration;

    public float BoxCount
    {
        get { return boxCount; }
        set
        {
            if (boxCount < value)
            {
                showBox = true;
            }
            boxCount = value;
        }
    }

    public float HeroCount
    {
        get { return heroCount; }
        set
        {
            if (heroCount < value)
            {
                showHero = true;
            }
            heroCount = value;
        }
    }

    private float boxCount;
    private float heroCount;

    private bool showBox;
    private bool showHero;

    public void Show(bool animate = false)
    {
        if (animate)
        {
            StartCoroutine(DoShow());
        }
        else
        {
            DisplayLabel();
        }
    }

    private void DisplayLabel()
    {
        BoxLabel.text = "" + BoxCount;
        HeroLabel.text = "" + HeroCount;
    }

    public void Reset()
    {
        BoxCount = 0;
        HeroCount = 0;

        Show();
    }

    private IEnumerator DoShow()
    {
        GameObject target = null;
        if (showBox)
        {
            showBox = false;

            BoxEffect.transform.position = EffectParent.transform.position;

            BoxEffect.Play();
            yield return new WaitForSeconds(BoxEffect.Duration - OverlayDuration);
            BoxEffect.Stop();

            target = BoxLabel.gameObject;
        }

        if (showHero)
        {
            showHero = false;
            HeroEffect.transform.position = EffectParent.transform.position;

            HeroEffect.Play();
            yield return new WaitForSeconds(HeroEffect.Duration - OverlayDuration);
            HeroEffect.Stop();

            target = HeroLabel.gameObject;
        }

        // we got hero or box.
        if (target != null)
        {
            SparkleEffect.transform.position = EffectParent.transform.position;

            iTween.MoveTo(SparkleEffect.gameObject, iTween.Hash("position", target.transform.position, "speed", SparkleSpeed));
            SparkleEffect.Play();
            yield return new WaitForSeconds(SparkleSpeed);
            SparkleEffect.Stop();
        }
        // then we got gold instead.
        else
        {
            GoldEffect.transform.position = EffectParent.transform.position;
            GoldEffect.Play();
            yield return new WaitForSeconds(GoldEffect.Duration);
            GoldEffect.Stop();
        }

        DisplayLabel();
    }
}
