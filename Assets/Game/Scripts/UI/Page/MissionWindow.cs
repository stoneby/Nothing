using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class MissionWindow : Window
{
    private GameObject MapContainer;
    private GameObject RaidContainer;
    private GameObject FriendContainer;

    private GameObject MapEvent;
    private GameObject ItemsTable;
    private GameObject FriendsTable;

    private GameObject ItemPrefab;
    private GameObject FriendItemPrefab;

    #region Window

    public override void OnEnter()
    {
        MapContainer = transform.FindChild("Map Container").gameObject;
        RaidContainer = transform.FindChild("Items Container").gameObject;
        FriendContainer = transform.FindChild("Friend Container").gameObject;

        MapEvent = transform.FindChild("Map Container/Event Container").gameObject;
        ItemsTable = transform.FindChild("Items Container/Scroll View/Table").gameObject;
        ItemPrefab = Resources.Load("Prefabs/Component/MissionItem") as GameObject;

        FriendsTable = transform.FindChild("Friend Container/Scroll View/Table").gameObject;
        FriendItemPrefab = Resources.Load("Prefabs/Component/FriendItem") as GameObject;
        SetMissionList();

        EventManager.Instance.AddListener<MissionItemEvent>(OnItemClicktHandler);
        EventManager.Instance.AddListener<FriendEvent>(OnFriendHandler);
        EventManager.Instance.AddListener<FriendClickEvent>(OnFriendItemClickHandler);
    }

    private void SetMissionList()
    {
        MapEvent.SetActive(false);
        MapContainer.SetActive(true);
        RaidContainer.SetActive(true);
        FriendContainer.SetActive(false);
        MissionModelLocator.Instance.ComputeStagecount();
        var raids = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoNormal;
        var table = ItemsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;
        for (int i = raids.Count - 1; i >= 0; i--)
        {
            var raid = raids[i];
            //raid.TemplateId;
            
            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(ItemsTable, ItemPrefab);
            }
           
            var col = item.GetComponent<MissionItemControl>();
            col.InitRaid(i, MissionModelLocator.Instance.GetRaidByTemplateId(raid.TemplateId), raid,
                MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(RaidType.RaidNormal, raid.TemplateId));

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex ++;
        }
    }

    private void SetStageList(RaidInfo raid)
    {
        MapContainer.SetActive(true);
        RaidContainer.SetActive(true);
        FriendContainer.SetActive(false);
        var table = ItemsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;
        var stages = raid.StateInfo;
        var addition = MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(RaidType.RaidNormal, raid.TemplateId);
        if (addition != null)
        {
            MapEvent.SetActive(true);
        }
        for (int i = stages.Count - 1; i >= 0; i--)
        {
            var stage = stages[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(ItemsTable, ItemPrefab);
            }

            var col = item.GetComponent<MissionItemControl>();
            col.InitStage(i, MissionModelLocator.Instance.GetRaidStagrByTemplateId(stage.TemplateId), stage);

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex++;
        }
    }

    private void SetFriendList(SCRaidQueryFriend friends)
    {
        MapContainer.SetActive(false);
        RaidContainer.SetActive(false);
        FriendContainer.SetActive(true);

        var table = FriendsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;

        for (int i = 0; i < friends.BattleFriend.Count; i++)
        {
            var friend = friends.BattleFriend[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(FriendsTable, FriendItemPrefab);
            }

            var col = item.GetComponent<FriendItemControl>();
            col.Init(friend, true);

            itemindex++;
        }

        for (int i = 0; i < friends.BattleGuest.Count; i++)
        {
            var friend = friends.BattleGuest[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(FriendsTable, FriendItemPrefab);
            }

            var col = item.GetComponent<FriendItemControl>();
            col.Init(friend, false);

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex++;
        }
    }

    public override void OnExit()
    {
        EventManager.Instance.RemoveListener<MissionItemEvent>(OnItemClicktHandler);
        EventManager.Instance.RemoveListener<FriendEvent>(OnFriendHandler);
        EventManager.Instance.RemoveListener<FriendClickEvent>(OnFriendItemClickHandler);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion

    private MissionItemEvent MissionEvent;
    private void OnItemClicktHandler(MissionItemEvent e)
    {
        if (e.IsRaidClicked)
        {
            var raid = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoNormal[e.RaidIndex];
            SetStageList(raid);
        }
        else
        {
            MissionEvent = e;
            MissionModelLocator.Instance.SelectedStageId = e.StageId;
            NetManager.SendMessage(new CSRaidQueryFriend());
            // var csmsg = new CSRaidQueryFriend();
            

        }
    }

    private void OnFriendHandler(FriendEvent e)
    {
        SetFriendList(e.RaidFriend);
    }

    private void OnFriendItemClickHandler(FriendClickEvent e)
    {
        MissionModelLocator.Instance.FriendData = e.FriendData;
        MissionModelLocator.Instance.IsFriend = e.IsFriend;
        if (HeroModelLocator.Instance.SCHeroList == null)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInBattle;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show(typeof(BattleConfirmTabWindow), true);
        }
    }
}
