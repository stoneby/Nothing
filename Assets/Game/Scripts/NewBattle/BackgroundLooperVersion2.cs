using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loop background forever version 2.
/// </summary>
/// <remarks>Head and tail are only components on faces, body is designed to repeat forever.</remarks>
public class BackgroundLooperVersion2 : MonoBehaviour
{
    public UIWidget Head;
    public UIWidget Tail;
    public List<TextureLooper> LooperList;

    private TweenPosition headPositionTween;
    private TweenPosition tailPositionTween;
    private Vector3 headDefaultPosition;
    private Vector3 tailDefaultPosition;

    /// <summary>
    /// Duration to loop body once.
    /// </summary>
    public float Duration;

    public void Begin()
    {
        PlayTween(headPositionTween, headDefaultPosition,
            new Vector3(headDefaultPosition.x - Head.width, headDefaultPosition.y, headDefaultPosition.z));

        LooperList.ForEach(looper => looper.CurrentPosition = 0f);
        Loop();
    }

    public void Loop()
    {
        LooperList.ForEach(looper => looper.GoByTime(Duration));
    }

    public void End()
    {
        LooperList.ForEach(looper => looper.CurrentPosition = 0.5f);
        Loop();

        PlayTween(tailPositionTween, tailDefaultPosition,
            new Vector3(tailDefaultPosition.x - Tail.width, tailDefaultPosition.y, tailDefaultPosition.z));
    }

    public void Reset()
    {
        Head.transform.localPosition = headDefaultPosition;
        Tail.transform.localPosition = tailDefaultPosition;
        LooperList.ForEach(looper => looper.Reset());
    }

    private void PlayTween(TweenPosition tween, Vector3 from, Vector3 to)
    {
        tween.ResetToBeginning();
        tween.from = from;
        tween.to = to;
        tween.PlayForward();
        tween.duration = Duration;
    }

    private void Awake()
    {
        headPositionTween = GetComponent<TweenPosition>() ?? Head.gameObject.AddComponent<TweenPosition>();
        headPositionTween.enabled = false;
        headDefaultPosition = Head.transform.localPosition;

        tailPositionTween = GetComponent<TweenPosition>() ?? Tail.gameObject.AddComponent<TweenPosition>();
        tailPositionTween.enabled = false;
        tailDefaultPosition = Tail.transform.localPosition;
    }
}
