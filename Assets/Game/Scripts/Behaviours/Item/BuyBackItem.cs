using System;
using KXSGCodec;
using UnityEngine;

public class BuyBackItem : ItemBase
{
    #region Private Fields

    private Transform selMaskTran;
    private UIEventListener itemClickedLis;
    private UILabel timeRemainLabel;
    private TimeSpan timeRemain;
    private bool selected;

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

    public event UIEventListener.VoidDelegate ItemClicked;
   
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            selMaskTran.gameObject.SetActive(selected);
        }
    }

    #endregion

    #region Private Methods

    private void OnItemClicked(GameObject go)
    {
        Selected = !Selected;
        if(ItemClicked != null)
        {
            ItemClicked(go);
        }
    }

    #endregion

    #region Public Methods

    public void Init(ItemInfo itemInfo)
    {
        BagIndex = itemInfo.BagIndex;
        Quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
        DateTime expireTime = Utils.ConvertFromJavaTimestamp(itemInfo.ExpireTime);
        TimeRemain = expireTime.Subtract(DateTime.Now);
    }

    #endregion

    #region Mono

    protected override void Awake()
    {
        base.Awake();
        selMaskTran = Utils.FindChild(transform, "Mask");
        itemClickedLis = UIEventListener.Get(gameObject);
        itemClickedLis.onClick += OnItemClicked;
        timeRemainLabel = cachedTran.FindChild("TimeRemain").GetComponent<UILabel>();
    }

    private void OnDestory()
    {
        itemClickedLis.onClick -= OnItemClicked;
    }

    private void OnEnable()
    {
        Selected = false;
    }

    #endregion
}
