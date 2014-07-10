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

    private UITexture texture;

    public void GoFirstHalf()
    {
        GoTo(0f, 0.5f);
    }

    public void GoSecondHalf()
    {
        GoTo(0.5f, 1f);
    }

    public void GoOne()
    {
        GoTo(0f, 1f);
    }

    public void Loop()
    {
        StartCoroutine(DoLoop());
    }

    public void Reset()
    {
        GoTo(0f, 0f);
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

    private void OnGUI()
    {
        if (GUILayout.Button("First Half"))
        {
            GoFirstHalf();
        }

        if (GUILayout.Button("Second Half"))
        {
            GoSecondHalf();
        }

        if (GUILayout.Button("One"))
        {
            GoOne();
        }

        if (GUILayout.Button("Loop"))
        {
            Loop();
        }

        if (GUILayout.Button("Reset"))
        {
            Reset();
        }

        if (GUILayout.Button("Loop GO"))
        {
            LoopGo();
        }
    }

    /// <summary>
    /// Go to target potion of the whole texture.
    /// </summary>
    /// <param name="from">From value in between range [0, 1]</param>
    /// <param name="to">To value in between range [0, 1]</param>
    private void GoTo(float from, float to)
    {
        if (LoopStyle == Style.Horizontal)
        {
            iTween.ValueTo(gameObject,
                iTween.Hash("from", from, "to", to, "time", HorizontalDuration, "easetype", iTween.EaseType.linear,
            "onupdate", (Action<object>) (value =>
            {
                texture.uvRect = new Rect((float) value, texture.uvRect.y, texture.uvRect.width,
                    texture.uvRect.height);
            })));
        }
        else if (LoopStyle == Style.Vertical)
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", from, "to", to, "time", VerticalDuration, "easetype", iTween.EaseType.linear,
                "onupdate", (Action<object>)(value =>
                {
                    texture.uvRect = new Rect(texture.uvRect.x, (float)value, texture.uvRect.width,
                        texture.uvRect.height);
                })));
        }
        else if (LoopStyle == Style.Both)
        {
            LoopStyle = Style.Horizontal;
            GoTo(from, to);
            LoopStyle = Style.Vertical;
            GoTo(from, to);
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
    }
}
