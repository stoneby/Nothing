using UnityEngine;

public class MainScreenControl : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The container and position where show the value of lv.
    /// </summary>
    public Transform LvValues;

    /// <summary>
    /// The container and position where show the value of vip.
    /// </summary>
    public Transform VipValues;

    /// <summary>
    /// The container and position where show the value of attack.
    /// </summary>
    public Transform AttackValues;

    /// <summary>
    /// The container and position where show the value of life.
    /// </summary>
    public Transform LifeValues;

    /// <summary>
    /// The template of the number sprite.
    /// </summary>
    public UISprite Template;

    /// <summary>
    /// The name of the military.
    /// </summary>
    public UILabel Name;

    /// <summary>
    /// The label of annoucement.
    /// </summary>
    public UILabel AnnounceLabel;

    /// <summary>
    /// The label of coins.
    /// </summary>
    public UILabel CoinsLabel;

    /// <summary>
    /// The label of soul.
    /// </summary>
    public UILabel SoulLabel;

    /// <summary>
    /// The label of diamond.
    /// </summary>
    public UILabel DiamondLabel;

    /// <summary>
    /// The visible area of annoucement.
    /// </summary>
    public UIWidget AnnouceArea;

    /// <summary>
    /// The color of attack number sprites.
    /// </summary>
    public Color AttackNumColor;

    /// <summary>
    /// The color of life number sprites.
    /// </summary>
    public Color LifeNumColor;

    /// <summary>
    /// The value to test lv.
    /// </summary>
    public int LvValue = 20;

    /// <summary>
    /// The value to test vip.
    /// </summary>
    public int VipValue = 8;

    /// <summary>
    /// The value to test coins.
    /// </summary>
    public int Coins = 1187;

    /// <summary>
    /// The value to test soul.
    /// </summary>
    public int Soul = 1187;

    /// <summary>
    /// The value to test diamond.
    /// </summary>
    public int Diamond = 1187;

    /// <summary>
    /// The value to test life values.
    /// </summary>
    public int LifeValue = 20000;

    /// <summary>
    /// The value to test attack values.
    /// </summary>
    public int AttackValue = 12200;

    /// <summary>
    /// The value to test annoucement.
    /// </summary>
    public string AnnouceText = "系统公告： 兹定于今日22:00进行系统维护。请";

    /// <summary>
    /// The value to test name.
    /// </summary>
    public string NameText = "李牧.字伯秀";

    #endregion

    #region Private Fields

    /// <summary>
    /// The tweener speed of the annoucement. 
    /// </summary>
    private const float AnnouceSpeed = 100f;

    /// <summary>
    /// The suffix of coins, soul and diamond, this is temporary solution.
    /// </summary>
    private const string Suffix = "万";

    #endregion

    #region Private Methods

    /// <summary>
    /// This is used to test the main screen when we enable the prefab game object.
    /// </summary>
    private void OnEnable()
    {
        Show();
    }
    
    /// <summary>
    /// Play the tweener of annoucement label.
    /// </summary>
    /// <param name="speed">The speed of the tweener.</param>
    private void PlayAnnouceLabelTween(float speed)
    {
        int length = AnnouceArea.width + AnnounceLabel.width;
        var from = AnnounceLabel.transform.localPosition;
        var to = from + Vector3.left * length;
        var duration = length / speed;
        PlayTweenPosition(AnnounceLabel.transform, from, to, duration, UITweener.Style.Loop);
    }

    /// <summary>
    /// Play the tweener of position.
    /// </summary>
    /// <param name="tran">The tranfrom which will be tweened.</param>
    /// <param name="from">The initial position of the tween.</param>
    /// <param name="to">The target position of the tween.</param>
    /// <param name="duration">The duration of the tween.</param>
    /// <param name="style">The style of the tweener.</param>
    private void PlayTweenPosition(Transform tran, Vector3 from, Vector3 to, float duration, UITweener.Style style)
    {

        var tweenPosition = tran.GetComponent<TweenPosition>();
        if(tweenPosition == null)
        {
            tweenPosition = tran.gameObject.AddComponent<TweenPosition>();
        }
        tweenPosition.from = from;
        tweenPosition.to = to;
        tweenPosition.duration = duration;
        tweenPosition.style = style;
        tweenPosition.PlayForward();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show the main screen.
    /// </summary>
    public void Show()
    {
        Name.text = NameText = PlayerModelLocator.Instance.Name;
        CoinsLabel.text = Coins + Suffix;
        SoulLabel.text = Soul + Suffix;
        DiamondLabel.text = Diamond + Suffix;
        AnnounceLabel.text = AnnouceText;

        Int2Sprite.Show(LvValues, Template, LvValue);
        Int2Sprite.Show(VipValues, Template, VipValue);
        Int2Sprite.Show(AttackValues.transform, Template, AttackValue);
        Int2Sprite.Show(LifeValues.transform, Template, LifeValue);

        PlayAnnouceLabelTween(AnnouceSpeed);
    }

    #endregion
}
