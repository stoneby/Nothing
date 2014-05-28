using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemInfoWindow : Window
{
    private UIEventListener backBtnLis;
    private UIEventListener lvUpBtnLis;
    private UIEventListener lvEvolveBtnLis;
    private ItemInfo itemInfo;

    private UILabel skillInitTitle;
    private UILabel skillInitDesc;

    private UILabel skillRandTitle;
    private UILabel skillRandDesc;
    private UILabel matchInfoDesc;
    private UILabel explainName;
    private UILabel explainDesc;


    private Transform skillContent;
    private Transform explainContent;

    #region Window

    public override void OnEnter()
    {
        itemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        InstallHandlers();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        lvUpBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LvUp").gameObject);
        lvEvolveBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Evolve").gameObject);

        skillContent = Utils.FindChild(transform, "SkillContent");
        skillInitTitle = Utils.FindChild(skillContent, "SkillInitTitle").GetComponent<UILabel>();
        skillInitDesc = Utils.FindChild(skillContent, "SkillInitDesc").GetComponent<UILabel>();
        skillRandTitle = Utils.FindChild(skillContent, "SkillInitTitle").GetComponent<UILabel>();
        skillRandDesc = Utils.FindChild(skillContent, "SkillRandDesc").GetComponent<UILabel>();
        matchInfoDesc = Utils.FindChild(skillContent, "MatchInfoDesc").GetComponent<UILabel>();

        explainContent = Utils.FindChild(transform, "ExplainContent");
        explainName = Utils.FindChild(explainContent, "Name").GetComponent<UILabel>();
        explainDesc = Utils.FindChild(explainContent, "Desc").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
        lvUpBtnLis.onClick += OnLvUpBtnClicked;
        lvEvolveBtnLis.onClick += OnEvolveBtnLis;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        lvUpBtnLis.onClick -= OnLvUpBtnClicked;
        lvEvolveBtnLis.onClick -= OnEvolveBtnLis;
    }

    private void OnLvUpBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show<UIItemLevelUpWindow>(true);
    }

    private void OnEvolveBtnLis(GameObject go)
    {
        
    }

    private void Refresh()
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
        var initTmp = skillTmp[ItemBaseInfoWindow.ItemDetail.InitSkillId];
        var randTmp = skillTmp[ItemBaseInfoWindow.ItemDetail.RandSkillId];
        skillInitTitle.text = initTmp.Name;
        skillInitDesc.text = initTmp.Desc;
        skillRandTitle.text = randTmp.Name;
        skillRandDesc.text = randTmp.Desc;
        matchInfoDesc.text = ItemBaseInfoWindow.ItemDetail.MatchInfo;
        NGUITools.SetActive(explainContent.gameObject, true);
        explainName.text = ItemModeLocator.Instance.GetName(itemInfo);
        explainDesc.text = ItemModeLocator.Instance.GetDesc(itemInfo);
        NGUITools.SetActive(explainContent.gameObject, false);
    }

    private void OnBackBtnClicked(GameObject go)
    {
        //WindowManager.Instance.Show(typeof(UIEquipsDisplayWindow), true);
        WindowManager.Instance.Show(typeof(UIEquipDispTabWindow), true);
        WindowManager.Instance.Show(typeof(UIItemInfoWindow), false);
    }

    #endregion
}
