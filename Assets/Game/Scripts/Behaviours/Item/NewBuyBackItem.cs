using System;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NewBuyBackItem : ItemBase
{
    #region Private Fields

    private Transform selMaskTran;
    private UILabel timeRemainLabel;
    private TimeSpan timeRemain;

    #endregion

    #region Public Properties

    public TimeSpan TimeRemain
    {
        get
        {
            return timeRemain;
        }
        set
        {
            timeRemain = value;
            timeRemainLabel.text = Utils.ConvertTimeSpanToString(timeRemain);
        }
    }

    #endregion

    #region Public Methods

    public override void InitItem(ItemInfo itemInfo)
    {
        BagIndex = itemInfo.BagIndex;
        var expireTime = Utils.ConvertFromJavaTimestamp(itemInfo.ExpireTime);
        TimeRemain = expireTime.Subtract(DateTime.Now);
    }

    #endregion

    #region Protected Methods

    protected override void Awake()
    {
        base.Awake();
        timeRemainLabel = cachedTran.FindChild("TimeRemain").GetComponent<UILabel>();
    }

    #endregion
}
