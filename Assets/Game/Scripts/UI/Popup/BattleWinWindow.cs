using UnityEngine;
using KXSGCodec;
using Property;
using System.Collections;
using System.Collections.Generic;

public class BattleWinWindow : Window
{
    #region Window



    #endregion

    #region Battle Result Window

    #endregion
    private const int StepTypeBegin = 0;
    private const int StepTypeLevelup = 1;
    private const int StepTypeItems = 2;
    private const int StepTypeHero = 3;
    private const int StepTypeFriend = 4;
    private const int StepTypeEnd = 5;
    #region Public Fields

    /// <summary>
    /// The time interval between different labels showing.
    /// </summary>
    public float Delay = 0.2f;

    #endregion

    private GameObject ItemPrefab;

    private GameObject InfoContainer;
    private GameObject GetContainer;
    private GameObject LevelupContainer;

    private GameObject ItemsContainer;

    private GameObject PopupAddFriend;

    private GameObject SpriteStar0;
    private GameObject SpriteStar1;
    private GameObject SpriteStar2;

//    private GameObject LabelExp;
//    private GameObject LabelGold;
//    private GameObject LabelCoin;
//    private GameObject LabelEnergy;
//    private GameObject LabelMingqi;
    private GameObject LabelOldMaxEnergy;
    private GameObject LabelNewMaxEnergy;
    private GameObject LabelOldMaxFriend;
    private GameObject LabelNewMaxFriend;

    private List<GameObject> SpriteIcons;
    private List<GameObject> LabelValues; 

    private GameObject LabelLevel;

    private GameObject BtnBattleAgain;
    private GameObject BtnNextRaid;
    private GameObject BtnBackToRaid;
    private GameObject BtnAddFriend;
    private GameObject BtnCancelFriend;

    private GameObject LabelAgainEnergy;
    private GameObject LabelNextEnergy;

    private GameObject FriendNameLanel;
    private GameObject FriendHeadSprite;

    //private GameObject ProgressBarExp;

    private GameObject SpriteClick;

    private GameObject ExpBar;

    private int CurrentStep;
    private bool CanClick = false;
    private int CurrItemIndex;

    private List<GameObject> Items;

    private UIEventListener BattleAgainUIEventListener;
    private UIEventListener NextRaidUIEventListener;
    private UIEventListener BackToRaidUIEventListener;
    private UIEventListener AddFriendUIEventListener;
    private UIEventListener CancelFriendUIEventListener;

    #region Private Methods

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        BattleResultHelper.Cleanup();
    }

    private void OnEnterHandler()
    {
        MtaManager.TrackBeginPage(MtaType.BattleWinWindow);

        BattleAgainUIEventListener.onClick += BattleAgainHandler;
        NextRaidUIEventListener.onClick += NextRaidHandler;
        BackToRaidUIEventListener.onClick += BackToRaidHandler;
        AddFriendUIEventListener.onClick += AddFriendHandler;
        CancelFriendUIEventListener.onClick += CancelFriendHandler;
    }

    private void OnExitHandler()
    {
        if (BattleAgainUIEventListener != null) BattleAgainUIEventListener.onClick -= BattleAgainHandler;
        if (NextRaidUIEventListener != null) NextRaidUIEventListener.onClick -= NextRaidHandler;
        if (BackToRaidUIEventListener != null) BackToRaidUIEventListener.onClick -= BackToRaidHandler;
        if (AddFriendUIEventListener != null) AddFriendUIEventListener.onClick -= AddFriendHandler;
        if (CancelFriendUIEventListener != null) CancelFriendUIEventListener.onClick -= CancelFriendHandler;

        MtaManager.TrackEndPage(MtaType.BattleWinWindow);
    }

    /// <summary>
    /// This is used to test the battle win when we enable the prefab game object.
    /// </summary>
    private void OnEnable()
    {
        Show();
    }

    //    private List<GameObject> GoldSprites;
    //    private List<GameObject> SpiritSprites;
    //    private List<GameObject> RepuSprites;
    //    private List<GameObject> ExpSprites;

    /// <summary>
    /// Show coins, soul, reputation and experience text in the battle win window.
    /// </summary>
    private IEnumerator ShowBattleWin()
    {
        OnEnterHandler();
        CanClick = false;
        ShowExp();

        InfoContainer.SetActive(false);
        GetContainer.SetActive(false);
        LevelupContainer.SetActive(false);
        PopupAddFriend.SetActive(false);

        yield return new WaitForSeconds(0.2f);
        SpriteClick.SetActive(false);
        CurrentStep = 0;
        CurrItemIndex = 0;

        GetValues();

        var lb = LabelLevel.GetComponent<UILabel>();
        lb.text = "Lv." + MissionModelLocator.Instance.OldLevel;

        if (MissionModelLocator.Instance.StarCount > 2)
        {
            SetStar(SpriteStar2, true);
            SetStar(SpriteStar1, true);
            SetStar(SpriteStar0, true);
        }
        else if (MissionModelLocator.Instance.StarCount > 1)
        {
            SetStar(SpriteStar2, false);
            SetStar(SpriteStar1, true);
            SetStar(SpriteStar0, true);
        }
        else if (MissionModelLocator.Instance.StarCount > 0)
        {
            SetStar(SpriteStar2, false);
            SetStar(SpriteStar1, false);
            SetStar(SpriteStar0, true);
        }
        else
        {
            SetStar(SpriteStar2, false);
            SetStar(SpriteStar1, false);
            SetStar(SpriteStar0, false);
        }

        yield return new WaitForSeconds(0.1f);

        lb = LabelAgainEnergy.GetComponent<UILabel>();
        lb.text = MissionModelLocator.Instance.BattleStageTemplate.CostEnergy.ToString();

        var bar = ExpBar.GetComponent<UIProgressBar>();
        var temp = LevelModelLocator.Instance.GetLevelByTemplateId(MissionModelLocator.Instance.OldLevel + 1);
        var v = (float)(MissionModelLocator.Instance.OldExp + MissionModelLocator.Instance.BattleReward.Exp) / temp.MaxExp;

        if (v >= 1)
        {
            LevelupContainer.SetActive(true);
            CurrentStep = StepTypeLevelup;
            yield return new WaitForSeconds(0.2f);
            lb = LabelLevel.GetComponent<UILabel>();
            lb.text = "Lv." + (MissionModelLocator.Instance.OldLevel + 1).ToString();

            var tempold = LevelModelLocator.Instance.GetLevelByTemplateId(MissionModelLocator.Instance.OldLevel);
            lb = LabelOldMaxEnergy.GetComponent<UILabel>();
            lb.text = tempold.MaxEnergy.ToString();
            lb = LabelNewMaxEnergy.GetComponent<UILabel>();
            lb.text = temp.MaxEnergy.ToString();

            bar.value = v;
            CanClick = true;
            SpriteClick.SetActive(true);
        }
        else
        {
            ShowItemView();
        }
    }

    private void GetValues()
    {
        int maxwidth = 700;
        int thewidth = 140;

        int[] values = new int[5];
        values[0] = MissionModelLocator.Instance.BattleReward.Exp;
        values[1] = GetValueByKey(RoleProperties.ROLEBASE_DIAMOND);
        values[2] = GetValueByKey(RoleProperties.ROLEBASE_GOLD);
        values[3] = GetValueByKey(RoleProperties.ROLEBASE_HERO_SPIRIT);
        values[4] = GetValueByKey(RoleProperties.ROLEBASE_FAMOUS);

        int m = 0;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > 0) m++;
        }

        int basex = 30 - thewidth*m/2;
        int k = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > 0)
            {
                SpriteIcons[i].SetActive(true);
                SpriteIcons[i].transform.localPosition = new Vector3(basex + k * thewidth, 147, 0);
                LabelValues[i].SetActive(true);
                LabelValues[i].transform.localPosition = new Vector3(basex + k * thewidth + 50, 147, 0);
                var lb = LabelValues[i].GetComponent<UILabel>();
                lb.text = values[i].ToString();
                k++;
            }
            else
            {
                SpriteIcons[i].SetActive(false);
                LabelValues[i].SetActive(false);
            }
        }
    }

    private void SetStar(GameObject obj, bool isyellow)
    {
        var thename = isyellow ? "star_yellow" : "star_grey";
        var sp = obj.GetComponent<UISprite>();
        sp.spriteName = thename;
    }

    private int GetValueByKey(int thekey)
    {
        int k = 0;
        if (MissionModelLocator.Instance.BattleReward.Money.ContainsKey(thekey))
        {
            k = MissionModelLocator.Instance.BattleReward.Money[thekey];
        }
        return k;
    }

    private void ShowExp()
    {
        if (ExpBar != null)
        {
            var bar = ExpBar.GetComponent<UIProgressBar>();
            var temp = LevelModelLocator.Instance.GetLevelByTemplateId(MissionModelLocator.Instance.OldLevel + 1);
            bar.value = (float)MissionModelLocator.Instance.OldExp / temp.MaxExp;
        }
    }

    private void ShowItemView()
    {
        if (MissionModelLocator.Instance.BattleReward.RewardItem != null &&
            MissionModelLocator.Instance.BattleReward.RewardItem.Count > 0)
        {
            InfoContainer.SetActive(false);
            LevelupContainer.SetActive(false);
            PopupAddFriend.SetActive(false);
            GetContainer.SetActive(true);
            CurrentStep = StepTypeItems;
            SetItems();
            StartCoroutine(ShowItemStep());
            CanClick = false;
        }
        else
        {
            ShowFriendView();
        }
    }

    private void ShowFriendView()
    {
        if (MissionModelLocator.Instance.ShowAddFriendAlert)
        {
            InfoContainer.SetActive(false);
            GetContainer.SetActive(false);
            LevelupContainer.SetActive(false);
            CurrentStep = StepTypeFriend;
            PopupAddFriend.SetActive(true);
            var lb = FriendNameLanel.GetComponent<UILabel>();
            lb.text = MissionModelLocator.Instance.FriendData.Data.FriendName;
            var sp = FriendHeadSprite.GetComponent<UISprite>();
            
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(MissionModelLocator.Instance.FriendData.Data.HeroProp[0].HeroTemplateId);
            HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);
            CanClick = false;
            //Alert.Show(AssertionWindow.Type.OkCancel, "系统提示", "你要添加“" + MissionModelLocator.Instance.FriendData.Data.FriendName + "”为好友吗？", AddFriendHandler);
        }
        else
        {
            ShowEndView();
        }
    }

    private void ShowEndView()
    {
        if (MissionModelLocator.Instance.RaidLoadingAll != null)
        {
            var tmp = MissionModelLocator.Instance.GetNextStage();
            //PopTextManager.PopTip(MissionModelLocator.Instance.NextRaidTemplate.RaidName + "---------raid");
            //PopTextManager.PopTip(MissionModelLocator.Instance.NextStageTemplate.StageName + "---------stage");
            var lb = LabelNextEnergy.GetComponent<UILabel>();
            lb.text = (tmp == null) ? "" : MissionModelLocator.Instance.NextStageTemplate.CostEnergy.ToString();
        }
        else
        {
            BtnBattleAgain.SetActive(false);
            BtnNextRaid.SetActive(false);
        }

        GetContainer.SetActive(false);
        LevelupContainer.SetActive(false);
        PopupAddFriend.SetActive(false);
        CurrentStep = StepTypeEnd;
        InfoContainer.SetActive(true);
        CanClick = true;
        SpriteClick.SetActive(true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show battle win screen.
    /// </summary>
    public void Show()
    {
        StartCoroutine(ShowBattleWin());
    }

    private IEnumerator ShowItemStep()
    {
        //IsPlaying = true;
        while (CurrItemIndex < MissionModelLocator.Instance.BattleReward.RewardItem.Count)
        {
            yield return new WaitForSeconds(0.8f);
            var control = Items[CurrItemIndex].GetComponent<GetItemControl>();
            CurrItemIndex++;
            control.Open();
            //            if (control.Data.IsNew)
            //            {
            //                IsPlaying = false;
            //                SpriteClick.SetActive(true);
            //                break;
            //            }
        }
        SpriteClick.SetActive(true);
        CanClick = true;
    }

    void OnClick()
    {
        //if (!CanClick) return;
        switch (CurrentStep)
        {
            case StepTypeLevelup:
                ShowItemView();
                break;
            case StepTypeItems:
                ShowFriendView();
                break;
            case StepTypeEnd:
                break;
            default:
                break;
        }
        //        if (CurrentStep == 0)
        //        {
        //            //SetItems();
        //
        //            SpriteClick.SetActive(false);
        //            InfoContainer.SetActive(false);
        //            if (MissionModelLocator.Instance.BattleReward.RewardItem == null ||
        //                MissionModelLocator.Instance.BattleReward.RewardItem.Count <= 0)
        //            {
        //                ShowEnd();
        //            }
        //            else
        //            {
        //                SetItems();
        //                GetContainer.SetActive(true);
        //                CurrentStep = 2;
        //                StartCoroutine(ShowItemStep());
        //            }
        //        }
        //        else if (CurrItemIndex < MissionModelLocator.Instance.BattleReward.RewardItem.Count)
        //        {
        //            StartCoroutine(ShowItemStep());
        //        }
        //        else
        //        {
        //            ShowEnd();
        //        }
    }

    private void SetItems()
    {
        if (Items == null)
        {
            Items = new List<GameObject>();
        }
        else
        {
            while (Items.Count > 0)
            {
                var obj = Items[0];
                Items.RemoveAt(0);
                Destroy(obj);
            }
        }
        int k = (MissionModelLocator.Instance.BattleReward.RewardItem.Count >= 7)
            ? 7
            : MissionModelLocator.Instance.BattleReward.RewardItem.Count;
        int basex = 70 - 140 * k / 2;
        int basey = 70;
        int offsetx = 140;
        int offsety = 140;
        for (int i = 0; i < MissionModelLocator.Instance.BattleReward.RewardItem.Count; i++)
        {
            var v = i % 7;
            var m = (i - v) / 7;
            var item = NGUITools.AddChild(ItemsContainer, ItemPrefab);
            var control = item.GetComponent<GetItemControl>();
            var d = MissionModelLocator.Instance.BattleReward.RewardItem[i];
            control.SetData(d);
            item.transform.localPosition = new Vector3(basex + offsetx * v, basey - offsety * m, 0);
            Items.Add(item);
        }
    }

    private void ShowEnd()
    {
        WindowManager.Instance.Show(WindowGroupType.Popup, false);
//        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
//        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        //WindowManager.Instance.Show(typeof(RaidsWindow), true);
        MissionModelLocator.Instance.ShowRaidWindow();
        OnExitHandler();
    }

    void Awake()
    {
        InfoContainer = transform.FindChild("Container normal").gameObject;
        GetContainer = transform.FindChild("Container get").gameObject;
        LevelupContainer = transform.FindChild("Container levelup").gameObject;

        ItemsContainer = transform.FindChild("Container get/Container items").gameObject;

        PopupAddFriend = transform.FindChild("Container add friend").gameObject;

        SpriteStar0 = transform.FindChild("Sprite star 0").gameObject;
        SpriteStar1 = transform.FindChild("Sprite star 1").gameObject;
        SpriteStar2 = transform.FindChild("Sprite star 2").gameObject;

        SpriteIcons = new List<GameObject>();
        SpriteIcons.Add(transform.FindChild("Sprite exp").gameObject);
        SpriteIcons.Add(transform.FindChild("Sprite gold").gameObject);
        SpriteIcons.Add(transform.FindChild("Sprite coin").gameObject);
        SpriteIcons.Add(transform.FindChild("Sprite energy").gameObject);
        SpriteIcons.Add(transform.FindChild("Sprite mingqi").gameObject);

        LabelValues = new List<GameObject>();
        LabelValues.Add(transform.FindChild("Label exp").gameObject);
        LabelValues.Add(transform.FindChild("Label gold").gameObject);
        LabelValues.Add(transform.FindChild("Label coin").gameObject);
        LabelValues.Add(transform.FindChild("Label energy").gameObject);
        LabelValues.Add(transform.FindChild("Label mingqi").gameObject);

        LabelOldMaxEnergy = transform.FindChild("Container levelup/Label enery value old").gameObject;
        LabelNewMaxEnergy = transform.FindChild("Container levelup/Label enery value new").gameObject;
        LabelOldMaxFriend = transform.FindChild("Container levelup/Label friend value old").gameObject;
        LabelNewMaxFriend = transform.FindChild("Container levelup/Label friend value new").gameObject;

        LabelLevel = transform.FindChild("Label level").gameObject;

        BtnBattleAgain = transform.FindChild("Container normal/Button again").gameObject;
        BtnNextRaid = transform.FindChild("Container normal/Button next").gameObject;
        BtnBackToRaid = transform.FindChild("Container normal/Button raid").gameObject;
        BtnAddFriend = transform.FindChild("Container add friend/Button add").gameObject;
        BtnCancelFriend = transform.FindChild("Container add friend/Button cancel").gameObject;

        LabelAgainEnergy = transform.FindChild("Container normal/Button again/Label energy").gameObject;
        LabelNextEnergy = transform.FindChild("Container normal/Button next/Label energy").gameObject;

        FriendNameLanel = transform.FindChild("Container add friend/Label name").gameObject;
        FriendHeadSprite = transform.FindChild("Container add friend/Sprite head").gameObject;

        BattleAgainUIEventListener = UIEventListener.Get(BtnBattleAgain);
        NextRaidUIEventListener = UIEventListener.Get(BtnNextRaid);
        BackToRaidUIEventListener = UIEventListener.Get(BtnBackToRaid);
        AddFriendUIEventListener = UIEventListener.Get(BtnAddFriend);
        CancelFriendUIEventListener = UIEventListener.Get(BtnCancelFriend);

        SpriteClick = transform.FindChild("Sprite click").gameObject;

        ExpBar = transform.FindChild("Progress Bar exp").gameObject;

        ItemPrefab = Resources.Load("Prefabs/Component/BattleGetItem") as GameObject;

        ShowExp();
    }

    private void BattleAgainHandler(GameObject obj)
    {
        ShowEnd();
        WindowManager.Instance.Show(typeof(SetBattleWindow), true);
    }

    private void NextRaidHandler(GameObject obj)
    {
        //ShowEnd();
        MissionModelLocator.Instance.BattleStageTemplate = MissionModelLocator.Instance.NextStageTemplate;
        MissionModelLocator.Instance.BattleRaidTemplate = MissionModelLocator.Instance.NextRaidTemplate;
        ShowEnd();
        NetManager.SendMessage(new CSRaidQueryFriend());
        
    }

    private void BackToRaidHandler(GameObject obj)
    {
        ShowEnd();
    }


    private void AddFriendHandler(GameObject obj)
    {
        var msg = new CSFriendApply();
        msg.FriendUuid = MissionModelLocator.Instance.FriendData.Data.FriendUuid;
        NetManager.SendMessage(msg);
        ShowEndView();
    }

    private void CancelFriendHandler(GameObject obj)
    {
        ShowEndView();
    }

    #endregion
}
