using UnityEngine;

public class GameObjectLooper : MonoBehaviour
{
    public float Duration;

    private TweenPosition positionTween;
    private Vector3 defaultPosition;

    public void Play()
    {
        PlayTween(positionTween, defaultPosition,
            new Vector3(defaultPosition.x - Head.width, defaultPosition.y, defaultPosition.z));
    }

    public void Loop()
    {
        
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
        positionTween = GetComponent<TweenPosition>() ?? gameObject.AddComponent<TweenPosition>();
        positionTween.enabled = false;
        defaultPosition = transform.localPosition;
    }
}
