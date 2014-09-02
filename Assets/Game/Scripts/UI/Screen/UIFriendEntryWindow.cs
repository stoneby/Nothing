using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendEntryWindow : Window
{
    #region Private Fields

    private ExtendBag itemExtendConfirm;
    private UIEventListener closeLis;
    private UIEventListener extendLis;
    private UILabel friendCount;

    private UIEventListener toggleListLis;
    private UIEventListener toggleAddLis;
    private UIEventListener toggleSortLis;
    private UIEventListener toggleGetLis;

    //private UILabel toggleListLabel;
    //private UILabel toggleAddLabel;
    //private UILabel toggleSortLabel;
    //private UILabel toggleGetLabel;
    //private readonly Color activedColor = new Color(255, 237, 0, 255);
    //private readonly Color deactivedColor = new Color(0, 0, 0, 255);

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        closeLis.onClick = OnClose;
        extendLis.onClick = OnExtend;
        //toggleListLis.onClick = OnListClick;
        //toggleAddLis.onClick = OnAddClick;
        //toggleSortLis.onClick = OnSortClick;
        //toggleGetLis.onClick = OnGetClick;
    }

    private void UnInstallHandlers()
    {
        closeLis.onClick = null;
        extendLis.onClick = null;
        toggleListLis.onClick = null;
        toggleAddLis.onClick = null;
        toggleSortLis.onClick = null;
        toggleGetLis.onClick = null;
    }

    //private void DeactiveAll()
    //{
    //    toggleListLabel.color = deactivedColor;
    //    toggleAddLabel.color = deactivedColor;
    //    toggleSortLabel.color = deactivedColor;
    //    toggleGetLabel.color = deactivedColor;
    //}

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
            PopTextManager.PopTip("可加好友数已达上限");
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

    #region Public Fields

    public List<UIToggle> Toggles;
    public List<FriendHandlerBase> FriendHandlers;
    public GameObject ExtendBagConfirm;

    #endregion

    #region Public Methods

    //public void OnListClick(GameObject go)
    //{
    //    DeactiveAll();
    //    toggleListLabel.color = activedColor;
    //}

    //public void OnAddClick(GameObject go)
    //{
    //    DeactiveAll();
    //    toggleAddLabel.color = activedColor;
    //}

    //public void OnSortClick(GameObject go)
    //{
    //    DeactiveAll();
    //    toggleSortLabel.color = activedColor;
    //}

    //public void OnGetClick(GameObject go)
    //{
    //    DeactiveAll();
    //    toggleGetLabel.color = activedColor;
    //}

    public void RefreshFriendCount()
    {
        var scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        friendCount.text = string.Format("{0}/{1}", scFriendLoadingAll.FriendList.Count, scFriendLoadingAll.FriendLimit);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        //OnListClick(new GameObject());
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

        toggleListLis = UIEventListener.Get(Utils.FindChild(transform, "Toggle-List").gameObject);
        toggleAddLis = UIEventListener.Get(Utils.FindChild(transform, "Toggle-Add").gameObject);
        toggleSortLis = UIEventListener.Get(Utils.FindChild(transform, "Toggle-Sort").gameObject);
        toggleGetLis = UIEventListener.Get(Utils.FindChild(transform, "Toggle-Get").gameObject);

        //toggleListLabel = Utils.FindChild(toggleListLis.transform, "Content").GetComponent<UILabel>();
        //toggleAddLabel = Utils.FindChild(toggleAddLis.transform, "Content").GetComponent<UILabel>();
        //toggleSortLabel = Utils.FindChild(toggleSortLis.transform, "Content").GetComponent<UILabel>();
        //toggleGetLabel = Utils.FindChild(toggleGetLis.transform, "Content").GetComponent<UILabel>();
    }

    #endregion

}
