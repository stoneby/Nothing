using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class TextureLooper : MonoBehaviour
{
    public enum Style
    {
        Horizontal,
        Vertical,
        Both
    }

    public Style LoopStyle;

    public float HorizontalDuration;
    public float VerticalDuration;

    public float CurrentPosition { get; set; }

    private UITexture texture;

    public void GoOne()
    {
        CurrentPosition = 0f;
        var duration = (LoopStyle == Style.Horizontal) ? HorizontalDuration : VerticalDuration;
        GoTo(0f, 1f, duration);
    }

    public void GoByTime(float time)
    {
        var from = CurrentPosition;
        var to = CurrentPosition + ((LoopStyle == Style.Horizontal)
            ? (time / HorizontalDuration)
            : (time / VerticalDuration));
        GoTo(from, to, time);
    }

    public void GoByDistance(float distance)
    {
        var from = CurrentPosition;
        var to = CurrentPosition + distance;
        var time = distance * ((LoopStyle == Style.Horizontal) ? HorizontalDuration : VerticalDuration);
        GoTo(from, to, time);
    }

    public void Loop()
    {
        StartCoroutine(DoLoop());
    }

    public void Reset()
    {
        CurrentPosition = 0f;
        GoTo(0f, 0f, 0f);
        StopAllCoroutines();
    }

    public void LoopGo()
    {
        var ps = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
        ps.ResetToBeginning();
        ps.from = Vector3.zero;
        ps.to = new Vector3(-640, 0, 0);
        ps.duration = HorizontalDuration;
        ps.Play(true);
    }

    /// <summary>
    /// Go to target potion of the whole texture.
    /// </summary>
    /// <param name="from">From value in between range [0, 1]</param>
    /// <param name="to">To value in between range [0, 1]</param>
    /// <param name="duration">Duration from from to to.</param>
    private void GoTo(float from, float to, float duration)
    {
        Logger.Log("Go by time: " + duration + ", from: " + CurrentPosition + ", to: " + to);
        if (LoopStyle == Style.Horizontal)
        {
            iTween.ValueTo(gameObject,
                iTween.Hash("from", from, "to", to, "time", duration, "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>) (value =>
            {
                CurrentPosition = (float)value;
                texture.uvRect = new Rect(CurrentPosition, texture.uvRect.y, texture.uvRect.width,
                    texture.uvRect.height);
            })));
        }
        else if (LoopStyle == Style.Vertical)
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", from, "to", to, "time", duration, "easetype", iTween.EaseType.linear,
                "onupdate", (Action<object>)(value =>
                {
                    CurrentPosition = (float)value;
                    texture.uvRect = new Rect(texture.uvRect.x, CurrentPosition, texture.uvRect.width,
                        texture.uvRect.height);
                })));
        }
        else if (LoopStyle == Style.Both)
        {
            LoopStyle = Style.Horizontal;
            GoTo(from, to, HorizontalDuration);
            LoopStyle = Style.Vertical;
            GoTo(from, to, VerticalDuration);
            LoopStyle = Style.Both;
        }
    }

    private IEnumerator DoLoop()
    {
        while (true)
        {
            GoOne();
            yield return new WaitForSeconds(HorizontalDuration);
        }
    }

    private void Awake()
    {
        texture = GetComponent<UITexture>();
        CurrentPosition = 0f;
    }
}
