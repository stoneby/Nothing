using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendEntryWindow : Window
{
    public List<UIToggle> Toggles;
    public List<FriendHandlerBase> FriendHandlers;
    public GameObject ExtendBagConfirm;

    private ExtendBag itemExtendConfirm;
    private UIEventListener backLis;
    private UIEventListener extendLis;
    private UILabel friendCount;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    private void Awake()
    {
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        var title = transform.Find("Title");
        friendCount = title.Find("FriendCount/FriendCountValue").GetComponent<UILabel>();
        extendLis = UIEventListener.Get(title.Find("Button-Extend").gameObject);
    }

    private void InstallHandlers()
    {
        backLis.onClick = OnBack;
        extendLis.onClick = OnExtend;
    }

    private void UnInstallHandlers()
    {
        backLis.onClick = null;
        extendLis.onClick = null;
    }

    private void OnExtend(GameObject go)
    {
        itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ExtendBagConfirm).GetComponent<ExtendBag>();
        itemExtendConfirm.ExtendContentKey = FriendConstant.ExtendContentKey;
        itemExtendConfirm.ExtendLimitKey = FriendConstant.ExtendLimitKey;
        var bases = ItemModeLocator.Instance.Bag;
        var costDict = bases.FriendExtTmpls.ToDictionary(item => item.Value.Id, item => item.Value.ExtendCost);
        itemExtendConfirm.Init(FriendModelLocator.Instance.ExtendFriendTimes + 1, bases.BagBaseTmpls[1].ExtendfriendCount,
                               costDict);
        itemExtendConfirm.OkClicked += OnExendBagOk;
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendEntryWindow>(false);
    }

    public void RefreshFriendCount()
    {
        var scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        friendCount.text = string.Format("{0}/{1}", scFriendLoadingAll.FriendList.Count, scFriendLoadingAll.FriendLimit);
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSFriendExtend
        {
            ExtendTimes = (sbyte)itemExtendConfirm.ExtendSize
        };
        NetManager.SendMessage(msg);
    }
}
