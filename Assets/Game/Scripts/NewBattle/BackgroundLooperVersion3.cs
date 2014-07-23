using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loop background forever version 3. (final version)
/// </summary>
/// <remarks>
/// Head and tail are only components on faces, 
/// body (UV moving) + (GameObject moving) is designed to repeat forever.
/// </remarks>
public class BackgroundLooperVersion3 : AbstractBattlegroundLooper
{
    public UIWidget Head;
    public UIWidget Tail;
    public List<TextureLooper> TextureLooperList;
    public List<WidgetLooper> WidgetLooperList;

    private TweenPosition headPositionTween;
    private TweenPosition tailPositionTween;
    private Vector3 headDefaultPosition;
    private Vector3 tailDefaultPosition;

    public override void PlayBegin()
    {
        PlayTween(headPositionTween, headDefaultPosition,
            new Vector3(headDefaultPosition.x - Head.width, headDefaultPosition.y, headDefaultPosition.z));
        TextureLooperList.ForEach(looper => looper.CurrentPosition = 0f);
        PlayOnce();
    }

    public override void PlayOnce()
    {
        TextureLooperList.ForEach(looper => looper.GoByTime(Duration));
    }

    public override void PlayEnd()
    {
        TextureLooperList.ForEach(looper => looper.CurrentPosition = 0.5f);
        PlayOnce();

        PlayTween(tailPositionTween, tailDefaultPosition,
            new Vector3(tailDefaultPosition.x - Tail.width, tailDefaultPosition.y, tailDefaultPosition.z));
    }

    public override void Reset()
    {
        Head.transform.localPosition = headDefaultPosition;
        Tail.transform.localPosition = tailDefaultPosition;
        TextureLooperList.ForEach(looper => looper.Reset());
    }

    protected override void Awake()
    {
        headPositionTween = GetComponent<TweenPosition>() ?? Head.gameObject.AddComponent<TweenPosition>();
        headPositionTween.enabled = false;
        headDefaultPosition = Head.transform.localPosition;

        tailPositionTween = GetComponent<TweenPosition>() ?? Tail.gameObject.AddComponent<TweenPosition>();
        tailPositionTween.enabled = false;
        tailDefaultPosition = Tail.transform.localPosition;
    }
}
