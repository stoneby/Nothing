using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendEntryWindow : Window
{
    #region Public Fields

    public List<UIToggle> Toggles;
    public List<FriendHandlerBase> FriendHandlers;
    public GameObject ExtendBagConfirm;

    #endregion

    #region Private Fields

    private ExtendBag itemExtendConfirm;
    private UIEventListener closeLis;
    private UIEventListener extendLis;
    private UILabel friendCount;

    #endregion

    #region Public Methods

    public void RefreshFriendCount()
    {
        var scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        friendCount.text = string.Format("{0}/{1}", scFriendLoadingAll.FriendList.Count, scFriendLoadingAll.FriendLimit);
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        closeLis.onClick = OnClose;
        extendLis.onClick = OnExtend;
    }

    private void UnInstallHandlers()
    {
        closeLis.onClick = null;
        extendLis.onClick = null;
    }

    private void OnExtend(GameObject go)
    {
        if (ItemModeLocator.Instance.Bag.FriendExtTmpls.Count - FriendModelLocator.Instance.ExtendFriendTimes != 0)
        {
            itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ExtendBagConfirm).GetComponent<ExtendBag>();
            itemExtendConfirm.ExtendContentKey = FriendConstant.ExtendContentKey;
            itemExtendConfirm.ExtendLimitKey = FriendConstant.ExtendLimitKey;
            var bases = ItemModeLocator.Instance.Bag;
            var costDict = bases.FriendExtTmpls.ToDictionary(item => item.Value.Id, item => item.Value.ExtendCost);
            itemExtendConfirm.Init(FriendModelLocator.Instance.ExtendFriendTimes, bases.BagBaseTmpls[1].ExtendfriendCount,
                                   costDict);
            itemExtendConfirm.OkClicked += OnExendBagOk;
        }
        else
        {
            PopTextManager.PopTip("当前等级可拥有好友数量已达上限，不可继续扩展！");
        }
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendEntryWindow>(false);
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSFriendExtend
        {
            ExtendTimes = (sbyte)itemExtendConfirm.ExtendSize
        };
        NetManager.SendMessage(msg);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    private void Awake()
    {
        closeLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Close").gameObject);
        var title = transform.Find("Title");
        friendCount = title.Find("FriendCount/FriendCountValue").GetComponent<UILabel>();
        extendLis = UIEventListener.Get(title.Find("Button-Extend").gameObject);
    }

    #endregion
}
