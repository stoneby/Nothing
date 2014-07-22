using UnityEngine;

/// <summary>
/// Loop background forever.
/// </summary>
/// <remarks>Head and tail is one time showing, body is designed to loop.</remarks>
public class BackgroundLooperVersion1 : MonoBehaviour
{
    public UITexture Head;
    public UITexture Body;
    public UITexture Tail;

    private TextureLooper looper;
    private TweenPosition positionTween;
    private Vector3 defaultPosition;

    /// <summary>
    /// Duration to loop body once.
    /// </summary>
    public float Duration;

    public void Begin()
    {
        PlayTween(new Vector3(640, 0), Vector3.zero);
    }

    public void Loop()
    {
        looper.GoByTime(Duration);
    }

    public void End()
    {
        PlayTween(Vector3.zero, new Vector3(-640, 0));
    }

    public void Reset()
    {
        transform.localPosition = defaultPosition;
        looper.Reset();
    }

    private void PlayTween(Vector3 from, Vector3 to)
    {
        positionTween.ResetToBeginning();
        positionTween.from = from;
        positionTween.to = to;
        positionTween.PlayForward();
        positionTween.duration = Duration;
    }

    private void Awake()
    {
        positionTween = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
        looper = Body.GetComponent<TextureLooper>();
        defaultPosition = transform.localPosition;
    }
}
