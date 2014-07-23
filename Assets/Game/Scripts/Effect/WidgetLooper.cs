using System.Collections;
using UnityEngine;

/// <summary>
/// Looping widget (container) base on game object moving.
/// </summary>
/// <remarks>Move widget in between right to left side crossing base widget.</remarks>
[RequireComponent(typeof(UIWidget))]
public class WidgetLooper : MonoBehaviour
{
    /// <summary>
    /// Speed of moving.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Distance of moving.
    /// </summary>
    public Vector3 Distance;

    /// <summary>
    /// Base widget for current widiget to move across by.
    /// </summary>
    public UIWidget BaseWidget;

    private UIWidget widget;
    private TweenPosition positionTween;
    private Vector3 defaultPosition;
    private Vector3 currentPosition;

    private float Duration
    {
        get { return Vector3.Magnitude(Distance) / Speed; }
    }

    private float MinX
    {
        get { return -(BaseWidget.width + widget.width) / 2.0f; }
    }
    private float MaxX
    {
        get { return (BaseWidget.width + widget.width) / 2.0f; }
    }

    public void Play()
    {
        if (currentPosition.x < MinX)
        {
            currentPosition.x = MaxX;
        }

        if (currentPosition.x > MaxX)
        {
            currentPosition.x = MinX;
        }

        PlayTween(positionTween, currentPosition, currentPosition + Distance);
        currentPosition += Distance;
    }

    public void Reset()
    {
        transform.localPosition = defaultPosition;
        currentPosition = defaultPosition;
        StopAllCoroutines();
    }


    public void Loop()
    {
        StartCoroutine(DoLoop());
    }

    private IEnumerator DoLoop()
    {
        while (true)
        {
            Play();
            yield return new WaitForSeconds(Duration);
        }
    }

    private void PlayTween(TweenPosition tween, Vector3 from, Vector3 to)
    {
        tween.ResetToBeginning();
        tween.from = from;
        tween.to = to;
        tween.PlayForward();
        tween.duration = Duration;
    }

    // Use this for initialization
    private void Awake()
    {
        widget = GetComponent<UIWidget>();
        positionTween = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
        positionTween.enabled = false;

        Reset();
    }
}
