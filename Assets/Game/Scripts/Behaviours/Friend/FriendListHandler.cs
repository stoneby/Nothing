﻿using System;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class FriendListHandler : FriendHandlerBase
{
    private SCFriendLoadingAll scFriendLoadingAll;
    private GameObject cachedGivenObject;
    private long cachedUuid = -1;

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.FriendListWindow);
        scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        Refresh(scFriendLoadingAll.FriendList);
        WindowManager.Instance.GetWindow<UIFriendEntryWindow>().RefreshFriendCount();
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.FriendListWindow);
    }

    private void OnGiveClicked(GameObject go)
    {
        cachedGivenObject = go;
        var friendItem = NGUITools.FindInParents<FriendItem>(go);
        cachedUuid = friendItem.FriendInfo.FriendUuid;
        var msg = new CSFriendGiveEnergy { FriendUuid = cachedUuid };
        NetManager.SendMessage(msg);

    }

    public void RefreshGivenSucc(long uuid)
    {
        foreach (var item in scFriendLoadingAll.FriendList)
        {
            if (item.FriendUuid == uuid)
            {
                item.GiveEnergyTime = Utils.ConvertToJavaTimestamp(DateTime.Today);
            }
        }
        if (cachedUuid == uuid)
        {
            cachedGivenObject.GetComponent<UIButton>().isEnabled = false;
            cachedGivenObject.transform.FindChild("GivenBtnSprite").GetComponent<UIButton>().isEnabled = false;
        }
    }

    public override void Refresh(List<FriendInfo> infos)
    {
        UpdateItemList(infos.Count);
        for(int i = 0; i < infos.Count; i++)
        {
            var child = Items.transform.GetChild(i);
            child.GetComponent<GivenItem>().Init(infos[i], OnGiveClicked);
        }
       NGUITools.FindInParents<UIScrollView>(Items.gameObject).ResetPosition();
    }
}
