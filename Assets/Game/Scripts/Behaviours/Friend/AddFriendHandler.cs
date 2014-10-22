using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class AddFriendHandler : FriendHandlerBase
{
    private UIEventListener searchLis;
    private UIInput input;
    private GameObject cachedItemObject;
    private long uuidCached;
    private List<FriendInfo> cachedApplyList;
    private const int InvalidIndex = -1;
    private bool isAreadyInApply = false;

	void OnEnable ()
	{
        MtaManager.TrackBeginPage(MtaType.AddFriendWindow);
	    InstallHandlers();
        FriendUtils.SendFriendListMessage(FriendModelLocator.FriendListType.Apply);
	}
	
	void OnDisable ()
    {
        MtaManager.TrackEndPage(MtaType.AddFriendWindow);
        UnInstallHandlers();
    }

    private void Awake()
    {
        searchLis = UIEventListener.Get(transform.Find("Search/Button-Search").gameObject);
        input = GetComponentInChildren<UIInput>();
    }

    private void InstallHandlers()
    {
        searchLis.onClick = OnSearch;
    }

    private void UnInstallHandlers()
    {
        searchLis.onClick = null;
    }

    private void OnSearch(GameObject go)
    {
        var msg = new CSFriendQueryByName {FriendName = input.value};
        NetManager.SendMessage(msg);
    }

    public override void Refresh(List<FriendInfo> infos)
    {
        cachedApplyList = infos;
        UpdateItemList(infos.Count);
        for(var i = 0; i < infos.Count; i++)
        {
            var child = Items.transform.GetChild(i);
            child.GetComponent<ApplyItem>().Init(infos[i], OnReject, OnAgree);
        }
    }

    protected void OnAgree(GameObject go)
    {
        var applyItem = NGUITools.FindInParents<ApplyItem>(go);
        if(applyItem != null)
        {
            cachedItemObject = applyItem.gameObject;
            uuidCached = applyItem.FriendInfo.FriendUuid;
            var msg = new CSFriendApplyOper { OperType = FriendConstant.FriendApplyAgree, Uuid = uuidCached };
            NetManager.SendMessage(msg);
        }
    }

    private void OnReject(GameObject go)
    {
        var applyItem = NGUITools.FindInParents<ApplyItem>(go);
        if (applyItem != null)
        {
            cachedItemObject = applyItem.gameObject;
            uuidCached = applyItem.FriendInfo.FriendUuid;
            var msg = new CSFriendApplyOper { OperType = FriendConstant.FriendApplyReject, Uuid = uuidCached };
            NetManager.SendMessage(msg);
        }
    }

    public void RefreshApplyOper(long uuid, sbyte operType)
    {
        if(uuid == uuidCached)
        {
            cachedItemObject.transform.parent = PoolManager.Pools["FriendRelated"].transform;
            PoolManager.Pools["FriendRelated"].Despawn(cachedItemObject.transform);
            Items.repositionNow = true;
            WindowManager.Instance.GetWindow<UIFriendEntryWindow>().RefreshFriendCount();
        }
    }

    public void OnChange()
    {
        var index = IndexOfInfo(input.value);
        if (index != InvalidIndex)
        {
            isAreadyInApply = true;
            var item = Items.GetChild(index);
            NGUITools.SetActiveChildren(Items.gameObject, false);
            NGUITools.SetActive(item.gameObject, true);
            Items.repositionNow = true;
        }
        else if (isAreadyInApply)
        {
            NGUITools.SetActiveChildren(Items.gameObject, true);
            Items.repositionNow = true;
        }
    }

    private int IndexOfInfo(string friendName)
    {
        var result = InvalidIndex;
        FriendInfo info = null;
        if(cachedApplyList != null)
        {
            info = cachedApplyList.Find(item => item.FriendName == friendName);
        }
        if(info != null)
        {
            result = cachedApplyList.IndexOf(info);
        }
        return result;
    }
}
