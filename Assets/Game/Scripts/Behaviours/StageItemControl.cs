using KXSGCodec;
using Template.Auto.Raid;
using UnityEngine;
using System.Collections;

public class StageItemControl : KxItemRender 
{
    private GameObject titleLable;

    private GameObject energyCountLabel;

    private GameObject star1;
    //private GameObject star1;
    private GameObject star2;
    private GameObject star3;

    private GameObject battleBtn;

    private GameObject btnActiveContainer;
    private GameObject lockSprite;
    private GameObject energyLabel;

    public string EneryCountStr;

    private UIEventListener OnBattleUIEventListener;

	// Use this for initialization
	void Start () 
    {
        titleLable = transform.FindChild("Titlle Label").gameObject;
        energyCountLabel = transform.FindChild("Energy Count Label").gameObject;
        //star1 = transform.FindChild("Star Container/Sprite1").gameObject;
        star1 = transform.FindChild("Star Container/Sprite1").gameObject;
        star2 = transform.FindChild("Star Container/Sprite2").gameObject;
        star3 = transform.FindChild("Star Container/Sprite3").gameObject;
        battleBtn = transform.FindChild("Image Button battle").gameObject;
        btnActiveContainer = transform.FindChild("Image Button battle/Container active").gameObject;
        lockSprite = transform.FindChild("Image Button battle/Sprite lock").gameObject;
        energyLabel = transform.FindChild("Image Button battle/Container active/Label").gameObject;

	    OnBattleUIEventListener = UIEventListener.Get(gameObject);
	    OnBattleUIEventListener.onClick += OnBattleHandler;
	    SetContent();
    }

    private void OnBattleHandler(GameObject obj)
    {
        if (OnSelected != null)
        {
            OnSelected(gameObject);
        }
    }

    public int StageIndex;
    public RaidStageTemplate StageTemp;
    public RaidStageInfo StageData;
	
    public override void SetData<T>(T data)
    {
        StageData = data as RaidStageInfo;
        StageIndex = ItemIndex;
        StageTemp = MissionModelLocator.Instance.GetRaidStagrByTemplateId(StageData.TemplateId);

        SetContent();
    }

    private void SetContent()
    {
        if (titleLable == null) return;
        var lb = titleLable.GetComponent<UILabel>();
        lb.text = StageTemp.StageName;

        lb = energyLabel.GetComponent<UILabel>();
        lb.text = StageTemp.CostEnergy.ToString();
            
        lb = energyCountLabel.GetComponent<UILabel>();
        EneryCountStr = MissionModelLocator.Instance.GetStageFinishTimeByTemplateId(StageData.TemplateId)
                        + "/" + StageTemp.DailyLimitTimes;
        lb.text = "挑战次数 " + EneryCountStr;
        var star = star1.GetComponent<UISprite>();
        star.spriteName = "icon_star_b";
        star = star2.GetComponent<UISprite>();
        star.spriteName = "icon_star_g";
        star = star3.GetComponent<UISprite>();
        star.spriteName = "icon_star_g";
            if (StageData.Star > 0)
            {
                //sp.spriteName = "passed";
                //star.spriteName = "icon_star_b";
                switch (StageData.Star)
                {
                    case 2:
                        star = star2.GetComponent<UISprite>();
                        star.spriteName = "icon_star_b";
                        break;
                    case 3:
                        star = star2.GetComponent<UISprite>();
                        star.spriteName = "icon_star_b";
                        star = star3.GetComponent<UISprite>();
                        star.spriteName = "icon_star_b";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                star = star1.GetComponent<UISprite>();
                star.spriteName = "icon_star_g";
            }
    }
}
