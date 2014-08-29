using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemDetailHandler : MonoBehaviour
{
    public PropertyUpdater PropertyUpdater;
    public ItemBase ItemBase;
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
        var selected = commonWindow.MainInfo != null;
        NGUITools.SetActive(ItemBase.gameObject, selected);
        if(commonWindow.MainInfo == null)
        {
            return;
        }
        commonWindow.ShowSelMask(true);
        Refresh();
        //var csmsg = new CSQueryItemDetail { BagIndex = commonWindow.GetInfo(commonWindow.CurSelPos).BagIndex };
        //NetManager.SendMessage(csmsg);
    }

    public void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.ItemDetailWindow);
        PropertyUpdater.Reset();
    }

    private void OnDetail(GameObject go)
    {
        commonWindow.CurSelPos = UISellItemHandler.GetPosition(go);
        commonWindow.ShowSelMask(go.transform.position);
        //var bagIndex = go.GetComponent<NewEquipItem>().BagIndex;
        //var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        //NetManager.SendMessage(csmsg);
        Refresh();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        NGUITools.SetActive(ItemBase.gameObject, true);
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
    }

    public void Refresh()
    {
        //var detail = ItemModeLocator.Instance.ItemDetail;
        //if(detail == null)
        //{
        //    return;
        //}
        //var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;

        //var initSkillId = detail.InitSkillId;
        //if (skillTmp.ContainsKey(initSkillId))
        //{
        //    var initTmp = skillTmp[initSkillId];
        //    skillInitTitle.text = initTmp.Name;
        //    skillInitDesc.text = initTmp.Desc;
        //}
        //var randSkillId = detail.RandSkillId;
        //if (skillTmp.ContainsKey(randSkillId))
        //{
        //    var randTmp = skillTmp[randSkillId];
        //    skillRandTitle.text = randTmp.Name;
        //    skillRandDesc.text = randTmp.Desc;
        //}
        //matchDesc.text = detail.MatchInfo;
        //var info = ItemModeLocator.Instance.FindItem(detail.BagIndex);
        var info = commonWindow.MainInfo;
        var level = info.Level;
        var temId = info.TmplId;
        var atk = ItemModeLocator.Instance.GetAttack(temId, level);
        var hp = ItemModeLocator.Instance.GetHp(temId, level);
        var recover = ItemModeLocator.Instance.GetRecover(temId, level);
        var mp = ItemModeLocator.Instance.GetMp(temId);
        PropertyUpdater.UpdateProperty(level, info.MaxLvl, atk, hp, recover, mp);
        ItemBase.InitItem(temId);
    }

    #endregion
}
