using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class FriendEnergyHandler : FriendHandlerBase
{
    private UILabel receiveLabel;
    private long cachedUuid = -1;
    private GameObject cachedReceiveObject;
    private sbyte receiveTimesCached;
    private sbyte timesLimitCached;

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.ReceiveEnergyWindow);
        receiveLabel.text = "";
        var msg = new CSFriendRecieveEnergyList();
        NetManager.SendMessage(msg);
    }
    
    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.ReceiveEnergyWindow);
    }

    private void Awake()
    {
        receiveLabel = transform.Find("GetCount/GetCountValue").GetComponent<UILabel>();
    }

    public void Refresh(List<RecieveEnergyInfo> energyInfos, sbyte receiveTimes, sbyte timesLimit)
    {
        receiveTimesCached = receiveTimes;
        timesLimitCached = timesLimit;
        receiveLabel.text = string.Format("{0}/{1}", receiveTimesCached, timesLimitCached);
        var count = energyInfos.Count;
        UpdateItemList(count);
        for (var i = 0; i < count; i++)
        {
            var energyInfo = energyInfos[i];
            var child = Items.transform.GetChild(i);
            child.GetComponent<ReceiveItem>().Init(energyInfo, OnReceiveClicked);
        }
    }

    public void RefreshReceiveSucc(long uuid, sbyte receiveTimes)
    {
        if (cachedUuid == uuid)
        {
            cachedReceiveObject.GetComponent<UIButton>().isEnabled = false;
            receiveTimesCached = receiveTimes;
            receiveLabel.text = string.Format("{0}/{1}", receiveTimesCached, timesLimitCached);
        }
    }

     private void OnReceiveClicked(GameObject go)
     {
         cachedReceiveObject = go;
         var receiveItem = NGUITools.FindInParents<ReceiveItem>(go);
         cachedUuid = receiveItem.FriendId;
         var msg = new CSFriendRecieveEnergy { FriendUuid = cachedUuid };
         NetManager.SendMessage(msg);
     }
}
