using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class RaidsWindow : Window
{
    private UIEventListener BtnReturnUIEventListener;
    private UIEventListener BtnStageDetailUIEventListener;
    private UIEventListener BtnStageLevelUIEventListener;

    private GameObject ContainerMap;
    private GameObject ContainerBigMap;
    private GameObject ContainerSmallMap;

    private GameObject LabelGold;
    private GameObject LabelEnergy;

    private GameObject BtnReturn;

    private GameObject PrefabBigItem;

    private bool IsScale = false;
    private bool IsShowStage = false;
    private bool HaveInitMaps = false;

    private List<RaidInfo> Raids;

    private List<GameObject> ChapterObjs;
    private List<GameObject> RaidObjs;

    private int SelectChapterId;

    //stage
    private GameObject StageContainer;
    private GameObject StageTitleLabel;
    private GameObject StageDetailBtn;
    private GameObject StageLevelBtn;
    private GameObject StageLevelSprite;
    private GameObject StageItemsContainer;

    //private int StageLevel = 0;//难度1普通2精英3英雄
    private int StageLevelCount = 3;

    //private List<GameObject> ChapterRaidsContainers; 
    #region Window

    public override void OnEnter()
    {
        var cma = Camera.main.GetComponent<UICamera>();
        if (cma != null)
        {
            cma.eventType = UICamera.EventType.Unity2D;
        }
        MissionModelLocator.Instance.ComputeStagecount();
        if (!HaveInitMaps)
        {
            RaidObjs = new List<GameObject>();
            ChapterObjs = new List<GameObject>();
            //ChapterRaidsContainers = new List<GameObject>();
            for (int i = 0; i < MissionModelLocator.Instance.StageMaps.Count; i++)
            {
                var obj = NGUITools.AddChild(ContainerSmallMap, PrefabBigItem);
                var item = obj.GetComponent<RaidBigItemControl>();
                item.ItemIndex = i;
                item.IsChapter = false;
                item.SetData(MissionModelLocator.Instance.StageMaps[i]);
                item.OnSelected += OnSmallSelecteHandler;
                RaidObjs.Add(obj);
            }


            for (int i = 0; i < MissionModelLocator.Instance.RaidMaps.Count; i++)
            {
                var obj = NGUITools.AddChild(ContainerBigMap, PrefabBigItem);
                var item = obj.GetComponent<RaidBigItemControl>();
                item.SetData(MissionModelLocator.Instance.RaidMaps[i]);
                item.OnSelected += OnBigSelecteHandler;
                ChapterObjs.Add(obj);
            }


            HaveInitMaps = true;
        }
        var lb = LabelGold.GetComponent<UILabel>();
        lb.text = PlayerModelLocator.Instance.Diamond.ToString();
        lb = LabelEnergy.GetComponent<UILabel>();
        lb.text = PlayerModelLocator.Instance.Energy.ToString();

        SetRaids();
        if (IsShowStage)
        {
            ShowStage(CurrRaidMap);
        }
        BtnReturnUIEventListener.onClick += OnReturnButtonClick;
        BtnStageDetailUIEventListener.onClick += OnStageDetailHandler;
        BtnStageLevelUIEventListener.onClick += OnStageLevelHandler;
        ResetButton();
    }

    private void SetRaids()
    {
        Raids = MissionModelLocator.Instance.GetCurrentRaids();
        var temp = new List<RaidInfo>(Raids.OrderBy(raidInfo => raidInfo.TemplateId));
        for (int i = 0; i < temp.Count; i++)
        {
            var item = RaidObjs[i].GetComponent<RaidBigItemControl>();
            item.SetRaidData(temp[i]);
            item.SetLock(false);
            var cid = item.RaidTemp.BigMap;
            var mapitem = ChapterObjs[cid - 1].GetComponent<RaidBigItemControl>();
            mapitem.SetLock(false);
        }
    }

    private IEnumerator PlayScall()
    {
        yield return new WaitForSeconds(0.5f);
        ContainerSmallMap.SetActive(true);

        for (int i = 0; i < RaidObjs.Count; i++)
        {
            var item = RaidObjs[i].GetComponent<RaidBigItemControl>();
            if (item.RaidTemp.BigMap == SelectChapterId)
            {
                RaidObjs[i].SetActive(true);
            }
            else
            {
                RaidObjs[i].SetActive(false);
            }
        }

        ResetButton();

    }

    private float Tox;
    private float Toy;
    private void OnBigSelecteHandler(GameObject obj)
    {
        var item = obj.GetComponent<RaidBigItemControl>();
        if (item.IsLock)
        {
            PopTextManager.PopTip("The Chapter is Locked");
            return;
        }
        //

        if (!IsScale)
        {
            PlayTweenScale(ContainerMap, 0.5f, new Vector3(1, 1, 1), new Vector3(3, 3, 1));
            Tox = -item.map.x * 3;
            if (Tox - 640 * 3 > -640)
            {
                Tox = 640 * 2;
            }
            SelectChapterId = int.Parse(item.map.id);
            Toy = -item.map.y*3;
            ContainerSmallMap.transform.localPosition = new Vector3(Tox, Toy, 0);
            PlayTweenPosition(ContainerMap, 0.5f, new Vector3(0, 0, 0), new Vector3(Tox, Toy, 0));
            ContainerBigMap.SetActive(false);
            IsScale = true;
            StartCoroutine(PlayScall());
        }
    }

    private void OnSmallSelecteHandler(GameObject obj)
    {
        var item = obj.GetComponent<RaidBigItemControl>();
        if (item.IsLock)
        {
            PopTextManager.PopTip("The Map is Locked");
        }
        else
        {
//            PopTextManager.PopTip(item.map.id + " Raid Clicked");
            if (IsShowStage && CurrRaidMap.id == item.map.id) return;
            MissionModelLocator.Instance.Raid = item.RaidData;
            var lb = StageTitleLabel.GetComponent<UILabel>();
            lb.text = MissionModelLocator.Instance.SelectRaidName = item.RaidTemp.RaidName;
            ShowStage(item.map);
        }
    }

    private MapVO CurrRaidMap;
    private void ShowStage(MapVO themap)
    {
        var table = StageItemsContainer.GetComponent<KxVListRender>();
        var stages = GetStages(themap);
        List<RaidStageInfo> temp;
        if (stages != null)
        {
            if (MissionModelLocator.Instance.CurrRaidType == RaidType.RaidNormal)
            {
                temp = new List<RaidStageInfo>(stages.OrderByDescending(stageinfo => stageinfo.TemplateId));
            }
            else
            {
                temp = new List<RaidStageInfo>(stages.OrderBy(stageinfo => stageinfo.TemplateId));
            }
            
        }
        else
        {
            temp = new List<RaidStageInfo>();
        }
        table.Init(temp, "Prefabs/Component/StageItem", 540, 490, 540, 140, OnStageItemClicktHandler);

        if (IsShowStage && CurrRaidMap.id == themap.id) return;
        CurrRaidMap = themap;
        StartCoroutine(PlayShowStage());
    }

    private IEnumerator PlayShowStage()
    {
        float tox = -320 - CurrRaidMap.x;
        float toy = -CurrRaidMap.y;
        PlayTweenPosition(ContainerMap, 0.4f, ContainerMap.transform.localPosition, new Vector3(tox, toy, 0));
        PlayTweenPosition(ContainerSmallMap, 0.4f, ContainerSmallMap.transform.localPosition, new Vector3(tox, toy, 0));
        yield return new WaitForSeconds(0.4f);
        if (!IsShowStage)
        {
            PlayTweenPosition(StageContainer, 0.5f, new Vector3(950, -11, 0), new Vector3(310, -11, 0));
            yield return new WaitForSeconds(0.5f);
            StageItemsContainer.SetActive(true);
            IsShowStage = true;
        }
        ResetButton();
    }

    private void ResetButton()
    {
        var cd = BtnReturn.GetComponent<BoxCollider2D>();
        cd.isTrigger = false;
        cd.isTrigger = true;
    }

    private IEnumerator PlayHideStage()
    {
        IsShowStage = false;
        StageItemsContainer.SetActive(false);
        PlayTweenPosition(StageContainer, 0.3f, new Vector3(310, -11, 0), new Vector3(950, -11, 0));
        yield return new WaitForSeconds(0.3f);
        PlayTweenPosition(ContainerMap, 0.2f, ContainerMap.transform.localPosition, new Vector3(Tox, Toy, 0));
        PlayTweenPosition(ContainerSmallMap, 0.2f, ContainerSmallMap.transform.localPosition, new Vector3(Tox, Toy, 0));
    }

    private List<RaidStageInfo> GetStages(MapVO themap)
    {
        Raids = MissionModelLocator.Instance.GetCurrentRaids();
        int raidtemplateid = MissionModelLocator.Instance.CurrRaidType*100 + int.Parse(themap.id);
        for (int i = 0; i < Raids.Count; i++)
        {
            if (Raids[i].TemplateId == raidtemplateid)
            {
                return Raids[i].StateInfo;
            }
        }
        return null;
    }

    private void OnStageItemClicktHandler(GameObject obj)
    {
        var control = obj.GetComponent<StageItemControl>();

        if (MissionModelLocator.Instance.RaidLoadingAll != null &&
            MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes != null &&
            MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes.ContainsKey(control.StageTemp.Id) &&
            MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes[control.StageTemp.Id] >= control.StageTemp.DailyLimitTimes)
        {
            var text = LanguageManager.Instance.GetTextValue("Poptip.Limit");
            PopTextManager.PopTip(text);
        }
        else
        {
            MissionModelLocator.Instance.SelectedStageId = control.StageTemp.Id;
            MissionModelLocator.Instance.SelectStageName = control.StageTemp.StageName;
            MissionModelLocator.Instance.SelectStageNeedEnegry = control.StageTemp.CostEnergy;
            MissionModelLocator.Instance.SelectStageCountStr = control.EneryCountStr;
            NetManager.SendMessage(new CSRaidQueryFriend());
        }
    }

    public override void OnExit()
    {
        if (BtnReturnUIEventListener != null) BtnReturnUIEventListener.onClick -= OnReturnButtonClick;
        if (BtnStageDetailUIEventListener != null) BtnStageDetailUIEventListener.onClick -= OnStageDetailHandler;
        if (BtnStageLevelUIEventListener != null) BtnStageLevelUIEventListener.onClick -= OnStageLevelHandler;
        var cma = Camera.main.GetComponent<UICamera>();
        if (cma != null)
        {
            cma.eventType = UICamera.EventType.UI;
        }
    }

    private void OnReturnButtonClick(GameObject obj)
    {
        if (IsShowStage)
        {
            StartCoroutine(PlayHideStage());
        }
        else if (IsScale)
        {
            PlayTweenScale(ContainerMap, 0.5f, new Vector3(3, 3, 1), new Vector3(1, 1, 1));
            PlayTweenPosition(ContainerMap, 0.5f, ContainerMap.transform.localPosition, new Vector3(0, 0, 0));
            ContainerBigMap.SetActive(true);
            ContainerSmallMap.SetActive(false);
            IsScale = false;
        }
        else
        {
            WindowManager.Instance.Show(typeof(RaidsWindow), false);
            //WindowManager.Instance.Show<MainMenuBarWindow>(true);
            WindowManager.Instance.Show<UIMainScreenWindow>(true);
        }
        
    }

    private void OnStageDetailHandler(GameObject obj)
    {

    }

    private void OnStageLevelHandler(GameObject obj)
    {
        MissionModelLocator.Instance.CurrRaidType++;
        if (MissionModelLocator.Instance.CurrRaidType > 3) MissionModelLocator.Instance.CurrRaidType = 1;
        var sp = StageLevelSprite.GetComponent<UISprite>();
        sp.spriteName = "level_" + (MissionModelLocator.Instance.CurrRaidType - 1);
        ShowStage(CurrRaidMap);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        ContainerBigMap = transform.FindChild("Container map/Container map big").gameObject;
        ContainerMap = transform.FindChild("Container map").gameObject;
        ContainerSmallMap = transform.FindChild("Container map small").gameObject;
        ContainerSmallMap.SetActive(false);
        PrefabBigItem = Resources.Load("Prefabs/Component/RaidBigItem") as GameObject;
        BtnReturn = transform.FindChild("Image Button return").gameObject;
        BtnReturnUIEventListener = UIEventListener.Get(BtnReturn);

        LabelGold = transform.FindChild("Label gold").gameObject;
        LabelEnergy = transform.FindChild("Label energy").gameObject;

        StageContainer = transform.FindChild("Container stage").gameObject;
        StageTitleLabel = transform.FindChild("Container stage/Label title").gameObject;
        StageDetailBtn = transform.FindChild("Container stage/Image Button detail").gameObject;
        StageLevelBtn = transform.FindChild("Container stage/Image Button level").gameObject;
        StageLevelSprite = transform.FindChild("Container stage/Image Button level/Sprite level").gameObject;
        StageItemsContainer = transform.FindChild("VList").gameObject;
        StageItemsContainer.SetActive(false);

        BtnStageDetailUIEventListener = UIEventListener.Get(StageDetailBtn);
        BtnStageLevelUIEventListener = UIEventListener.Get(StageLevelBtn);
    }

    #endregion

    void PlayTweenScale(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenScale ts = obj.AddComponent<TweenScale>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }

    void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenPosition ts = obj.AddComponent<TweenPosition>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }
}
