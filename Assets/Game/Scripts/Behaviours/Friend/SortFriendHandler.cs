using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using OrderType = FriendUtils.OrderType;

public class SortFriendHandler : FriendHandlerBase 
{
    private SCFriendLoadingAll scFriendLoadingAll;
    private List<FriendInfo> friendInfos;
    private FriendInfo myself;
    public Color MyselfColor = new Color(255f, 255f, 0f);
    public UIEventListener SortLis;
    public UILabel SortLabel;
    private OrderType orderType = OrderType.Atk;

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.SortFriendWindow);
        scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        friendInfos = new List<FriendInfo>(scFriendLoadingAll.FriendList);
        myself = scFriendLoadingAll.Myself;
        friendInfos.Add(scFriendLoadingAll.Myself);
        SortLabel.text = LanguageManager.Instance.GetTextValue(FriendUtils.SortNameKeys[(int)orderType]);
        Refresh(friendInfos);
        SortLis.onClick = OnSort;
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.SortFriendWindow);
        SortLis.onClick = null;
        orderType = OrderType.Atk;
        SortLabel.text = LanguageManager.Instance.GetTextValue(FriendUtils.SortNameKeys[(int)orderType]);
    }

    private void OnSort(GameObject go)
    {
        orderType = (OrderType)(((int)orderType + 1) % FriendUtils.SortNameKeys.Count);
        SortLabel.text = LanguageManager.Instance.GetTextValue(FriendUtils.SortNameKeys[(int) orderType]);
        Refresh(friendInfos);
    }

    public override void Refresh(List<FriendInfo> infos)
    {
        UpdateItemList(infos.Count);
        FriendUtils.Sort(infos, orderType);
        for(var i = 0; i < infos.Count; i++)
        {
            var child = Items.transform.GetChild(i);
            var sortedItem = child.GetComponent<SortedItem>();
            var info = infos[i];
            sortedItem.Init(info, i);
            var isMyself = info == myself;
            sortedItem.NameLabel.color = isMyself ? MyselfColor : Color.white;
        }
    }
}
