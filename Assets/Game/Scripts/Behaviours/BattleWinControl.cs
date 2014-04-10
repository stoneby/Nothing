using System.Collections;
using UnityEngine;

public class BattleWinControl : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The initial position of the tween.
    /// </summary>
    public Transform From;

    /// <summary>
    /// The target position of the tween.
    /// </summary>
    public Transform To;

    /// <summary>
    /// The slider of the experience bar.
    /// </summary>
    public UISlider ExpSlider;

    /// <summary>
    /// The tranfrom of template game object.
    /// </summary>
    public UISprite Template;

    /// <summary>
    /// The transform of coin sprites game object's the parent.
    /// </summary>
    public Transform CoinValues;

    /// <summary>
    /// The transform of soul sprites game object's the parent.
    /// </summary>
    public Transform SoulValues;

    /// <summary>
    ///The transform of reputation sprites game object's the parent.
    /// </summary>
    public Transform RepValues;

    /// <summary>
    /// The transform of experience sprites game object's the parent.
    /// </summary>
    public Transform ExpValues;

    /// <summary>
    /// The coin value.
    /// </summary>
    public int CoinValue = 25000;

    /// <summary>
    /// The soul value.
    /// </summary>
    public int SoulValue = 25000;

    /// <summary>
    ///  The reputation value.
    /// </summary>
    public int RepValue = 25000;

    /// <summary>
    ///  The experience value.
    /// </summary>
    public int ExpValue = 25000;

    /// <summary>
    /// The time interval between different labels showing.
    /// </summary>
    public float Delay = 0.2f;

    #endregion

    #region Private Methods

    /// <summary>
    /// This is used to test the battle win when we enable the prefab game object.
    /// </summary>
    private void OnEnable()
    {
        Show();
    }

    /// <summary>
    /// Show coins, soul, reputation and experience text in the battle win window.
    /// </summary>
    private IEnumerator ShowBattleWin()
    {
        var tweenPosition = GetComponent<TweenPosition>();
        if(tweenPosition == null)
        {
            tweenPosition = gameObject.AddComponent<TweenPosition>();
        }
        tweenPosition.from = From.position;
        tweenPosition.to = To.position;
        tweenPosition.PlayForward();
        yield return new WaitForSeconds(tweenPosition.duration);
        Int2Sprite.Show(CoinValues, Template, 25000);
        yield return new WaitForSeconds(Delay);
        Int2Sprite.Show(SoulValues, Template, 3600);
        yield return new WaitForSeconds(Delay);
        Int2Sprite.Show(RepValues, Template, 60000);
        yield return new WaitForSeconds(Delay);
        Int2Sprite.Show(ExpValues, Template, 125586);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show battle win screen.
    /// </summary>
    public void Show()
    {
        StartCoroutine("ShowBattleWin");
    }

    #endregion
}
