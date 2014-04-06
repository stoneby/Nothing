using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWinControl : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Drop off effect.
    /// </summary>
    public TweenPosition TweenPosition;

    /// <summary>
    /// The slider of the experience bar.
    /// </summary>
    public UISlider ExpSlider;

    /// <summary>
    /// The tranfrom of template game object.
    /// </summary>
    public Transform Template;

    /// <summary>
    /// The transform of the coin sprite game object.
    /// </summary>
    public Transform CoinSprite;

    /// <summary>
    /// The transform of the soul sprite game object.
    /// </summary>
    public Transform SoulSprite;

    /// <summary>
    /// The transform of reputation sprite game object.
    /// </summary>
    public Transform RepSprite;

    /// <summary>
    /// The transform of experience sprite game object.
    /// </summary>
    public Transform ExpSprite;

    /// <summary>
    /// In our Game coin, soul, reputation have the same interval from their related sprite.
    /// </summary>
    public int CommonInterval;

    /// <summary>
    /// The interval from experience sprite to value.
    /// </summary>
    public int ExpInterval;

    #endregion

    /// <summary>
    /// The time interval between different labels showing.
    /// </summary>
    private const float Delay = 0.2f;

    #region Private Methods

    /// <summary>
    /// Convert integer to char list.
    /// </summary>
    /// <param name="value">The integer to be converted.</param>
    /// <returns>The char list which is converted from the integer value.</returns>
    /// <exception cref="Exception"></exception>
    private IEnumerable<char> I2CharList(int value)
    {
        if(value < 0)
        {
            throw new Exception("The integer value is not valid to convert.");
        }
        var result = new List<char>();
        while (value != 0)
        {
            var charToAdd = (char)(value % 10 + '0');
            result.Add(charToAdd);
            value /= 10;
        }
        result.Reverse(0, result.Count);
        return result;
    }

    /// <summary>
    /// Show a nubmer label right of parent transfrom with the value. 
    /// </summary>
    /// <param name="parent">The transfrom which the label position will </param>
    /// <param name="interval">The position interval of the label to the parent transfrom.</param>
    /// <param name="rect">The dimensions of the label sprite.</param>
    /// <param name="value">The number label's value.</param>
    private void Show(Transform parent, int interval, Vector2 rect, int value)
    {
        var charList = I2CharList(value);
        var spriteWidth = (int)rect.x;
        var spriteHeight = (int)rect.y;
        int index = 1;
        foreach (var charItem in charList)
        {
            var obj = Instantiate(Template.gameObject) as GameObject;
            if(obj == null)
            {
                return;
            }
            obj.SetActive(true);
            var sprite = obj.GetComponent<UISprite>();
            sprite.spriteName = "" + charItem;
            sprite.width = spriteWidth;
            sprite.height = spriteHeight;
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Vector3.right * (interval + spriteWidth*index);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            index++;
        }
    }

    /// <summary>
    /// Show coins, soul, reputation and experience text in the battle win window.
    /// </summary>
    private IEnumerator ShowLabels()
    {
        yield return new WaitForSeconds(TweenPosition.duration);
        var sprite = Template.GetComponent<UISprite>();
        var width = sprite.width;
        var height = sprite.height;
        Show(CoinSprite, CommonInterval, new Vector2(width, height), 25000);
        yield return new WaitForSeconds(Delay);
        Show(SoulSprite, CommonInterval, new Vector2(width, height), 3600);
        yield return new WaitForSeconds(Delay);
        Show(RepSprite, CommonInterval, new Vector2(width, height), 60000);
        yield return new WaitForSeconds(Delay);
        Show(ExpSprite, ExpInterval, new Vector2(0.75f * width, 0.75f * height), 125586);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show battle win screen.
    /// </summary>
    public void Show()
    {
        TweenPosition.enabled = true;
        TweenPosition.PlayForward();

        StartCoroutine("ShowLabels");
    }

    #endregion
}
