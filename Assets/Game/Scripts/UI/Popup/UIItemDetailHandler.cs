using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemDetailHandler : MonoBehaviour
{
    public PropertyUpdater PropertyUpdater;
    public static bool IsLongPressEnter;
    private ItemInfo itemInfo;

    private UILabel skillInitTitle;
    private UILabel skillInitDesc;
    private UILabel skillRandTitle;
    private UILabel skillRandDesc;
    private UILabel matchDesc;
    private UILabel matchTitle;

    private UIItemCommonWindow commonWindow;

    #region Window

    public void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.ItemDetailWindow);
        commonWindow.NormalClicked = OnDetail;
        commonWindow.ShowSelMask();
        Refresh();
    }

    public void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.ItemDetailWindow);
    }

    private void OnDetail(GameObject go)
    {
        commonWindow.CurSel = go;
        commonWindow.ShowSelMask(go.transform.position);
        var bagIndex = go.GetComponent<NewEquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        NetManager.SendMessage(csmsg);
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        var initSkill = transform.Find("Skill/InitSkill");
        skillInitTitle = initSkill.Find("Title").GetComponent<UILabel>();
        skillInitDesc = initSkill.Find("Desc").GetComponent<UILabel>();
        
        var randSkill = transform.Find("Skill/RandSkill");
        skillRandTitle = randSkill.Find("Title").GetComponent<UILabel>();
        skillRandDesc = randSkill.Find("Desc").GetComponent<UILabel>();

        var matchInfo = transform.Find("Match/MatchInfo");
        matchTitle = matchInfo.Find("Title").GetComponent<UILabel>();
        matchDesc = matchInfo.Find("Desc").GetComponent<UILabel>();
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        var go = commonWindow.Items.transform.GetChild(0).GetComponent<WrapItemContent>().Children[0].gameObject;
        OnDetail(go);
    }

    public void Refresh()
    {
        var detail = ItemModeLocator.Instance.ItemDetail;
        if(detail == null)
        {
            return;
        }
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;

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
        var info = ItemModeLocator.Instance.FindItem(detail.BagIndex);
        var level = info.Level;
        var atk = ItemModeLocator.Instance.GetAttack(info.TmplId, level);
        var hp = ItemModeLocator.Instance.GetHp(info.TmplId, level);
        var recover = ItemModeLocator.Instance.GetRecover(info.TmplId, level);
        var mp = ItemModeLocator.Instance.GetMp(info.TmplId);
        PropertyUpdater.UpdateProperty(level, info.MaxLvl, atk, hp, recover, mp);
    }

    #endregion
}
