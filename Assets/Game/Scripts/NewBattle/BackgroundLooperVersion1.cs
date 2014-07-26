using UnityEngine;

/// <summary>
/// Loop background forever.
/// </summary>
/// <remarks>Head and tail is one time showing, body is designed to loop.</remarks>
public class BackgroundLooperVersion1 : AbstractBattlegroundLooper
{
    private TextureLooper looper;
    private TweenPosition positionTween;
    private Vector3 defaultPosition;

    public override void PlayBegin()
    {
        PlayTween(positionTween, new Vector3(Head.width, 0), Vector3.zero, GetDuration(Head.width));
    }

    public override void PlayOnce()
    {
        looper.Play(Duration);
    }

    public override void PlayEnd()
    {
        PlayTween(positionTween, Vector3.zero, new Vector3(-Tail.width, 0), GetDuration(Tail.width));
    }

    public override void Reset()
    {
        transform.localPosition = defaultPosition;
        looper.Reset();
    }

    protected override void Awake()
    {
        positionTween = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
        looper = Body.GetComponent<TextureLooper>();
        defaultPosition = transform.localPosition;
    }
}
