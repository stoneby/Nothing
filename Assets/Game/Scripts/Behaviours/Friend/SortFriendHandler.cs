using System.Collections.Generic;
using KXSGCodec;

public class SortFriendHandler : FriendHandlerBase 
{
    private SCFriendLoadingAll scFriendLoadingAll;

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.SortFriendWindow);
        scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        Refresh(scFriendLoadingAll.FriendList);
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.SortFriendWindow);
    }

    public override void Refresh(List<FriendInfo> infos)
    {
        UpdateItemList(infos.Count);
        infos.Sort(FriendUtils.CompareHeroByAtk);
        for(var i = 0; i < infos.Count; i++)
        {
            var child = Items.transform.GetChild(i);
            child.GetComponent<SortedItem>().Init(infos[i], i);
        }
        NGUITools.FindInParents<UIScrollView>(Items.gameObject).ResetPosition();
    }
}
