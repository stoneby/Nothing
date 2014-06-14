using KXSGCodec;
using Template;
using UnityEngine;
using System.Collections;

public class MissionItemControl : KxItemRender
{
//    private UIEventListener BtnClickUIEventListener;
    private GameObject TitleLable;

    private GameObject EventBox;
    private GameObject EnergyCountLabel;
    private GameObject FlagSprite;

    private GameObject EventSprite;
    private GameObject EventName;
    private GameObject EventTime;

    private GameObject Stars;
    private GameObject StarLabel;
    private GameObject Star1;
    private GameObject Star2;
    private GameObject Star3;
	// Use this for initialization
	void Start ()
	{
        TitleLable = transform.FindChild("Titlle Label").gameObject;
        EventBox = transform.FindChild("Event Container").gameObject;
        EventSprite = transform.FindChild("Event Container/Event Sprite").gameObject;
        EventName = transform.FindChild("Event Container/Event Label").gameObject;
        EventTime = transform.FindChild("Event Container/Left Label").gameObject;
        EnergyCountLabel = transform.FindChild("Energy Count Label").gameObject;
        FlagSprite = transform.FindChild("Flag Sprite").gameObject;
        Stars = transform.FindChild("Star Container").gameObject;
        StarLabel = transform.FindChild("Star Container/Process Label").gameObject;
        Star1 = transform.FindChild("Star Container/Sprite1").gameObject;
        Star2 = transform.FindChild("Star Container/Sprite2").gameObject;
        Star3 = transform.FindChild("Star Container/Sprite3").gameObject;

//        BtnClickUIEventListener = UIEventListener.Get(gameObject);
//        BtnClickUIEventListener.onClick += OnItemClick;
	    setContent();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsRaid;
    public int RaidIndex;
    public RaidTemplate RaidTemp;
    public RaidInfo RaidData;

    public RaidAddtionInfo RaidAdditionData;

    public int StageIndex;
    public RaidStageTemplate StageTemp;
    public RaidStageInfo StageData;


    public override void SetData<T>(T data)
    {
        //throw new System.NotImplementedException();
        if (data is RaidInfo)
        {
            IsRaid = true;
            RaidData = data as RaidInfo;
            RaidIndex = ItemIndex;
            RaidTemp = MissionModelLocator.Instance.GetRaidByTemplateId(RaidData.TemplateId);
            RaidAdditionData = MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(RaidData.TemplateId);
            setContent();
        }
        else
        {
            IsRaid = false;
            StageData = data as RaidStageInfo;
            StageIndex = ItemIndex;
            StageTemp = MissionModelLocator.Instance.GetRaidStagrByTemplateId(StageData.TemplateId);
           
            setContent();
        }
    }

    private void setContent()
    {
        if (TitleLable == null) return;
        var lb = TitleLable.GetComponent<UILabel>();
        var sp = FlagSprite.GetComponent<UISprite>();
        if (IsRaid)
        {
            lb.text = RaidTemp.Name;
            EnergyCountLabel.SetActive(false);
            int starcount = MissionModelLocator.Instance.GetRaidStarCount(RaidData.TemplateId);
            if (starcount <= 0)
            {
                sp.spriteName = "new";
            }
            else if (RaidData.StateInfo[RaidData.StateInfo.Count - 1].Star <= 0)
            {
                sp.spriteName = "fighting";
            }
            else
            {
                sp.spriteName = "passed";
            }
            Stars.SetActive(true);
            StarLabel.SetActive(true);
            Star2.SetActive(false);
            Star3.SetActive(false);
            //RaidTemp.
            lb = StarLabel.GetComponent<UILabel>();
            lb.text = MissionModelLocator.Instance.GetStageCountByRaidId(RaidTemp.Id);
        }
        else
        {
            lb.text = StageTemp.StageName;
            EnergyCountLabel.SetActive(true);
            lb = EnergyCountLabel.GetComponent<UILabel>();
            lb.text = "消耗体力 " + StageTemp.CostEnergy + "  次数 " + MissionModelLocator.Instance.GetStageFinishTimeByTemplateId(StageData.TemplateId)
                + "/" + StageTemp.DailyLimitTimes;
            if (StageData.Star > 0)
            {
                sp.spriteName = "passed";
                Stars.SetActive(true);
                StarLabel.SetActive(false);
                switch (StageData.Star)
                {
                    case 2:
                        Star2.SetActive(true);
                    Star3.SetActive(false);
                        break;
                    case 3:
                        Star2.SetActive(true);
                    Star3.SetActive(true);
                        break;
                    default:
                        Star2.SetActive(false);
                    Star3.SetActive(false);
                        break;
                }
            }
            else
            {
                sp.spriteName = "new";
                Stars.SetActive(false);
            }
        }

        if (IsRaid && RaidAdditionData != null)
        {
            EventBox.SetActive(true);
            sp = EventSprite.GetComponent<UISprite>();
            var lbname = EventName.GetComponent<UILabel>();
            var lbtime = EventTime.GetComponent<UILabel>();
            //金币150%、武魂150%、体力消耗50%、掉落概率150%
            switch (RaidAdditionData.AddtionType)
            {
                case RaidType.RaidAddtionTypeDrop:
                    sp.spriteName = "icon_box";
                    lbname.text = "x1.5";
                    break;
                case RaidType.RaidAddtionTypeEnergy:
                    sp.spriteName = "icon_energy";
                    lbname.text = "x0.5";
                    break;
                case RaidType.RaidAddtionTypeGold:
                    sp.spriteName = "icon_gold";
                    lbname.text = "x1.5";
                    break;
                case RaidType.RaidAddtionTypeSprit:
                    sp.spriteName = "icon_wuhun";
                    lbname.text = "x1.5";
                    break;
            }
            lbtime.text = MissionModelLocator.Instance.DestTime;
        }
        else
        {
            EventBox.SetActive(false);
        }
    }
}
