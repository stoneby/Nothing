using System.Collections;
using System.Xml;
using UnityEngine;

/// <summary>
/// Abstract battle ground looper.
/// </summary>
/// <remarks>
/// Head and tail is one time showing, body is designed to loop.
/// </remarks>
public abstract class AbstractBattlegroundLooper : MonoBehaviour
{
    /// <summary>
    ///  Speed of moving.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Distance of one moving.
    /// </summary>
    public float Distance;

    /// <summary>
    /// Duration of moving.
    /// </summary>
    public float Duration
    {
        get { return Distance / Speed; }
    }

    /// <summary>
    /// Step of loop in the middle.
    /// </summary>
    public int Step;

    public abstract void PlayBegin();

    public abstract void PlayOnce();

    public abstract void PlayEnd();

    public abstract void Reset();

    public void PlayLoop()
    {
        StartCoroutine(DoPlayLoop());
    }

    private IEnumerator DoPlayLoop()
    {
        PlayBegin();
        yield return new WaitForSeconds(Duration);
        for (var i = 0; i < Step; ++i)
        {
            PlayOnce();
            yield return new WaitForSeconds(Duration);
        }
        PlayEnd();
        yield return new WaitForSeconds(Duration);
    }

    protected void PlayTween(TweenPosition tween, Vector3 from, Vector3 to)
    {
        tween.ResetToBeginning();
        tween.from = from;
        tween.to = to;
        tween.PlayForward();
        tween.duration = Duration;
    }

    protected abstract void Awake();
}
