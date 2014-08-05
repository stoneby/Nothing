using KXSGCodec;
using Template;
using Template.Auto.Raid;
using UnityEngine;

public class MissionItemControl : KxItemRender
{
    private GameObject titleLable;

    private GameObject eventBox;
    private GameObject energyCountLabel;
    private GameObject flagSprite;

    private GameObject eventSprite;
    private GameObject eventName;
    private GameObject eventTime;

    private GameObject stars;
    private GameObject starLabel;
    private GameObject star2;
    private GameObject star3;

    void Start()
    {
        titleLable = transform.FindChild("Titlle Label").gameObject;
        eventBox = transform.FindChild("Event Container").gameObject;
        eventSprite = transform.FindChild("Event Container/Event Sprite").gameObject;
        eventName = transform.FindChild("Event Container/Event Label").gameObject;
        eventTime = transform.FindChild("Event Container/Left Label").gameObject;
        energyCountLabel = transform.FindChild("Energy Count Label").gameObject;
        flagSprite = transform.FindChild("Flag Sprite").gameObject;
        stars = transform.FindChild("Star Container").gameObject;
        starLabel = transform.FindChild("Star Container/Process Label").gameObject;
        star2 = transform.FindChild("Star Container/Sprite2").gameObject;
        star3 = transform.FindChild("Star Container/Sprite3").gameObject;

        SetContent();
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
        if (data is RaidInfo)
        {
            IsRaid = true;
            RaidData = data as RaidInfo;
            RaidIndex = ItemIndex;
            RaidTemp = MissionModelLocator.Instance.GetRaidByTemplateId(RaidData.TemplateId);
            RaidAdditionData = MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(RaidData.TemplateId);
            SetContent();
        }
        else
        {
            IsRaid = false;
            StageData = data as RaidStageInfo;
            StageIndex = ItemIndex;
            StageTemp = MissionModelLocator.Instance.GetRaidStagrByTemplateId(StageData.TemplateId);

            SetContent();
        }
    }

    private void SetContent()
    {
        if (titleLable == null) return;
        var lb = titleLable.GetComponent<UILabel>();
        var sp = flagSprite.GetComponent<UISprite>();
        if (IsRaid)
        {
            lb.text = RaidTemp.RaidName;
            energyCountLabel.SetActive(false);
            var starcount = MissionModelLocator.Instance.GetRaidStarCount(RaidData.TemplateId);
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
            stars.SetActive(true);
            starLabel.SetActive(true);
            star2.SetActive(false);
            star3.SetActive(false);

            lb = starLabel.GetComponent<UILabel>();
            lb.text = MissionModelLocator.Instance.GetStageCountByRaidId(RaidTemp.Id);
        }
        else
        {
            lb.text = StageTemp.StageName;
            energyCountLabel.SetActive(true);
            lb = energyCountLabel.GetComponent<UILabel>();
            lb.text = "消耗体力 " + StageTemp.CostEnergy + "  次数 " + MissionModelLocator.Instance.GetStageFinishTimeByTemplateId(StageData.TemplateId)
                + "/" + StageTemp.DailyLimitTimes;
            if (StageData.Star > 0)
            {
                sp.spriteName = "passed";
                stars.SetActive(true);
                starLabel.SetActive(false);
                switch (StageData.Star)
                {
                    case 2:
                        star2.SetActive(true);
                        star3.SetActive(false);
                        break;
                    case 3:
                        star2.SetActive(true);
                        star3.SetActive(true);
                        break;
                    default:
                        star2.SetActive(false);
                        star3.SetActive(false);
                        break;
                }
            }
            else
            {
                sp.spriteName = "new";
                stars.SetActive(false);
            }
        }

        if (IsRaid && RaidAdditionData != null)
        {
            eventBox.SetActive(true);
            sp = eventSprite.GetComponent<UISprite>();
            var lbname = eventName.GetComponent<UILabel>();
            var lbtime = eventTime.GetComponent<UILabel>();

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
            eventBox.SetActive(false);
        }
    }
}
