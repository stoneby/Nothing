using UnityEngine;

public class BattleEndControl : MonoBehaviour
{
    public GameObject BgSprite;
    public GameObject ContentSprite;

    // Use this for initialization
    void Start()
    {
        var sp = BgSprite.GetComponent<UISprite>();
        sp.alpha = 0.8f;
        sp.width = Screen.width;
        sp.height = Screen.height;
    }

    public void Show()
    {
        //_contentSprite.transform.localRotation = new Quaternion (0, 90, 0, 0);
        var tp = ContentSprite.AddComponent<TweenPosition>();
        tp.duration = 0.8f;
        tp.from = new Vector3(0, 500, 0);
        tp.to = new Vector3(0, 0, 0);
        tp.PlayForward();
        Destroy(tp, 1);
    }
}
