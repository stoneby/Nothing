using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemDetailWindow : Window
{
    public static bool IsLongPressEnter;

    private UIEventListener okLis;
    private ItemInfo itemInfo;

    private UILabel skillInitTitle;
    private UILabel skillInitDesc;
    private UILabel skillRandTitle;
    private UILabel skillRandDesc;
    private UILabel matchDesc;
    private UILabel matchTitle;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.ItemDetailWindow);
        okLis.onClick = OnOk;
        Refresh();
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.ItemDetailWindow);
        okLis.onClick = null;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        okLis = UIEventListener.Get(transform.Find("Button-Ok").gameObject);
        var initSkill = transform.Find("Skill/InitSkill");
        skillInitTitle = initSkill.Find("Title").GetComponent<UILabel>();
        skillInitDesc = initSkill.Find("Desc").GetComponent<UILabel>();
        
        var randSkill = transform.Find("Skill/RandSkill");
        skillRandTitle = randSkill.Find("Title").GetComponent<UILabel>();
        skillRandDesc = randSkill.Find("Desc").GetComponent<UILabel>();

        var matchInfo = transform.Find("Match/MatchInfo");
        matchTitle = matchInfo.Find("Title").GetComponent<UILabel>();
        matchDesc = matchInfo.Find("Desc").GetComponent<UILabel>();
    }


    private void OnOk(GameObject go)
    {
        WindowManager.Instance.Show<UIItemDetailWindow>(false);
    }

    public void Refresh()
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        var detail = ItemModeLocator.Instance.ItemDetail;
        var initSkillId = detail.InitSkillId;
        if (skillTmp.ContainsKey(initSkillId))
        {
            var initTmp = skillTmp[initSkillId];
            skillInitTitle.text = initTmp.Name;
            skillInitDesc.text = initTmp.Desc;
        }
        var randSkillId = detail.RandSkillId;
        if (skillTmp.ContainsKey(randSkillId))
        {
            var randTmp = skillTmp[randSkillId];
            skillRandTitle.text = randTmp.Name;
            skillRandDesc.text = randTmp.Desc;
        }
        matchDesc.text = detail.MatchInfo;
    }

    #endregion
}
