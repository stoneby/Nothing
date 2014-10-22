using System.Collections.Generic;
using KXSGCodec;
using Template.Auto.Quest;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class TaskWindow : Window
{
    private GameObject TitleLabel;
    private GameObject BgTexture;
    private GameObject ItemContainer;
    private GameObject TotalRewardItem;
    private GameObject ProgressLabel;
    private GameObject TotalButton;
    private GameObject ItemPrefab;

    private GameObject SignConfirm;
    private GameObject SignCurrencyInfo;
    private GameObject SignItemInfo;


    private List<GameObject> Items; 
    #region Window
    private UIEventListener BgUIEventListener;
    private UIEventListener ButtonUIEventListener;

    private QuestChapterTemplate chapterTemp;
    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        if (TitleLabel == null)
        {
            TitleLabel = transform.FindChild("Label title").gameObject;
            BgTexture = transform.FindChild("Texture bg").gameObject;
            ItemContainer = transform.FindChild("Container items").gameObject;
            TotalRewardItem = transform.FindChild("Container total/SignItem").gameObject;
            ProgressLabel = transform.FindChild("Container total/Label progress").gameObject;
            TotalButton = transform.FindChild("Container total/Button").gameObject;
            BgUIEventListener = UIEventListener.Get(transform.FindChild("Container bg").gameObject);
            BgUIEventListener.onClick += ClickHandler;
            ButtonUIEventListener = UIEventListener.Get(TotalButton);
            ButtonUIEventListener.onClick += ClickBtnHandler;
            ItemPrefab = Resources.Load("Prefabs/Component/TaskItem") as GameObject;

            SignConfirm = transform.FindChild("Panel/SignConfirm").gameObject;
            SignCurrencyInfo = transform.FindChild("Panel/SignCurrencyInfo").gameObject;
            SignItemInfo = transform.FindChild("Panel/SignItemInfo").gameObject;

            Items = new List<GameObject>();
        }

        while (Items.Count < 9)
        {
            Items.Add(NGUITools.AddChild(ItemContainer, ItemPrefab));
            //var theitem = Items[Items.Count - 1].GetComponent<KxItemRender>();
        }

        chapterTemp = SystemModelLocator.Instance.QuestTemplates.QuestChapterTmpls[SystemModelLocator.Instance.QuestMsg.ChapterId];

        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = chapterTemp.Name;
        var signitem = TotalRewardItem.GetComponent<SignItemControl>();
        signitem.SetData(chapterTemp.RewardId, false, false, OnSelectTotalHandler);
        var tt = BgTexture.GetComponent<UITexture>();
        var k = (chapterTemp.Id % 3) + 1;
        tt.mainTexture = (Texture2D)Resources.Load("AssetBundles/Textures/Task/task" + k, typeof(Texture2D));

        int basex = -272;
        int basey = 155;
        int offsetx = 272;
        int offsety = -155;
        int yy = 0;
        int thecount = 0;
        for (int i = 0; i < SystemModelLocator.Instance.QuestMsg.QuestList.Count; i++)
        {
            int xx = i%3;
            var itemobj = Items[i];
            var item = itemobj.GetComponent<TaskItemControl>();
            itemobj.transform.localPosition = new Vector3(basex + xx * offsetx, basey + yy * offsety, 0);
            thecount += item.SetData(SystemModelLocator.Instance.QuestMsg.QuestList[i], OnSelectHandler);
            if (xx == 2)
            {
                yy++;
            }
        }

        lb = ProgressLabel.GetComponent<UILabel>();
        lb.text = "完成进度 " + thecount + "/9";

        TotalButton.SetActive(thecount == 9);
    }
    private void ClickHandler(GameObject obj)
    {
        WindowManager.Instance.Show<TaskWindow>(false);
    }

    private void ClickBtnHandler(GameObject obj)
    {
        var msg = new CSQuestRecieveReward();
        msg.Id = chapterTemp.Id;
        msg.QuestType = 2;
        NetManager.SendMessage(msg);
    }

    private void OnSelectTotalHandler(GameObject obj)
    {
        var item = obj.GetComponent<SignItemControl>();
        if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_HERO)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterHero(item.RewardTemplate);
        }
        else if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_ITEM)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterEquip(item.RewardTemplate);
        }
        else
        {
            var info = SignCurrencyInfo.GetComponent<SignCourencyInfo>();
            SignCurrencyInfo.SetActive(true);
            info.OnEnter(item.RewardId);
        }
    }

    private void OnSelectHandler(GameObject obj)
    {
        var item = obj.GetComponent<TaskItemControl>();
        //任务状态 0未完成 1已完成未领取 2 已领取
        if (item.questInfo.Status == 1)
        {
            SystemModelLocator.Instance.RewardId = item.questTemp.RewardId;
            SystemModelLocator.Instance.QuestId = item.questTemp.Id;
            SignConfirm.SetActive(true);
            var com = SignConfirm.GetComponent<SignConfirmWindow>();
            com.ConfirmType = "quest";
            com.OnEnter();

        }
        else if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_HERO)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterHero(item.RewardTemplate);
        }
        else if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_ITEM)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterEquip(item.RewardTemplate);
        }
        else
        {
            var info = SignCurrencyInfo.GetComponent<SignCourencyInfo>();
            SignCurrencyInfo.SetActive(true);
            info.OnEnter(item.questTemp.RewardId);
        }

    }
    public override void OnExit()
    {
        GlobalWindowSoundController.Instance.PlayCloseSound();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}
