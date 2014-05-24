using KXSGCodec;
using Template;
using UnityEngine;
using System.Collections;

public class MissionItemControl : MonoBehaviour
{
    private UIEventListener BtnClickUIEventListener;
    private GameObject TitleLable;

    private GameObject EventBox;
    private GameObject EnergyCountLabel;
    private GameObject FlagSprite;

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
        EnergyCountLabel = transform.FindChild("Energy Count Label").gameObject;
        FlagSprite = transform.FindChild("Flag Sprite").gameObject;
        Stars = transform.FindChild("Star Container").gameObject;
        StarLabel = transform.FindChild("Star Container/Process Label").gameObject;
        Star1 = transform.FindChild("Star Container/Sprite1").gameObject;
        Star2 = transform.FindChild("Star Container/Sprite2").gameObject;
        Star3 = transform.FindChild("Star Container/Sprite3").gameObject;

        BtnClickUIEventListener = UIEventListener.Get(gameObject);
        BtnClickUIEventListener.onClick += OnItemClick;
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

    public void InitRaid(int theindex, RaidTemplate template, RaidInfo data, RaidAddtionInfo addition)
    {
        IsRaid = true;
        RaidIndex = theindex;
        RaidTemp = template;
        RaidData = data;
        RaidAdditionData = addition;
        setContent();
    }

    public void InitStage(int theindex, RaidStageTemplate template, RaidStageInfo data)
    {
        IsRaid = false;
        StageIndex = theindex;
        StageTemp = template;
        StageData = data;

        setContent();
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
            int pastedcount;
            if (RaidData.StateInfo.Count == 1)
            {
                sp.spriteName = "new";
                pastedcount = 0;
            }
            else if (RaidData.StateInfo[RaidData.StateInfo.Count - 1].Star <= 0)
            {
                sp.spriteName = "fighting";
                pastedcount = RaidData.StateInfo.Count - 1;
            }
            else
            {
                sp.spriteName = "passed";
                pastedcount = RaidData.StateInfo.Count;
            }
            Stars.SetActive(true);
            StarLabel.SetActive(true);
            Star2.SetActive(false);
            Star3.SetActive(false);
            //RaidTemp.
            lb = StarLabel.GetComponent<UILabel>();
            lb.text = pastedcount + "/" + MissionModelLocator.Instance.GetStageCountByRaidId(RaidTemp.Id);
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
        }
        else
        {
            EventBox.SetActive(false);
        }
    }

    private void OnItemClick(GameObject game)
    {
        var e = new MissionItemEvent();
        e.RaidIndex = RaidIndex;
        e.IsRaidClicked = IsRaid;
        if (StageTemp != null) e.StageId = StageTemp.Id;

        EventManager.Instance.Post(e);

        
    }

}
