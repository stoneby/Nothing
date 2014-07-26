using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract battle ground looper.
/// </summary>
/// <remarks>
/// Head and tail is one time showing, body is designed to loop.
/// </remarks>
public abstract class AbstractBattlegroundLooper : MonoBehaviour
{
    public UIWidget Head;
    public UIWidget Body;
    public UIWidget Tail;

    /// <summary>
    ///  Speed of the whole moving.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Duration from one step to another.
    /// </summary>
    public float Duration;

    /// <summary>
    /// Step of loop in the middle.
    /// </summary>
    public int Step;

    public abstract void PlayBegin();

    public abstract void PlayOnce();

    public abstract void PlayEnd();

    public abstract void Reset();

    public float GetDuration(float width)
    {
        return (width / (Body.width * Speed));
    }

    public void PlayLoop()
    {
        StartCoroutine(DoPlayLoop());
    }

    private IEnumerator DoPlayLoop()
    {
        PlayBegin();
        yield return new WaitForSeconds(GetDuration(Head.width));
        for (var i = 0; i < Step; ++i)
        {
            PlayOnce();
            yield return new WaitForSeconds(Duration);
        }
        PlayEnd();
        yield return new WaitForSeconds(GetDuration(Tail.width));
    }

    protected void PlayTween(TweenPosition tween, Vector3 from, Vector3 to, float duration)
    {
        tween.ResetToBeginning();
        tween.from = from;
        tween.to = to;
        tween.PlayForward();
        tween.duration = duration;
    }

    protected abstract void Awake();
}
