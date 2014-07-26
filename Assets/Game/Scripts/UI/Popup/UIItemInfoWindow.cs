using System.Globalization;
using System.Linq;
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

    private UIEventListener skillTabLis;
    private UIEventListener explainTabLis;
    private Transform skillContent;
    private Transform explainContent;

    /// <summary>
    /// The sprite name when the button is clicked.
    /// </summary>
    public static string DownTabSpriteName = "TabD";

    /// <summary>
    /// The sprite name when the button is normal.
    /// </summary>
    public static string NormalabSpriteName = "TabN";

    private bool canLvlUp;
    private bool CanLvlUp
    {
        get
        {
            return canLvlUp;
        }
        set
        {
            canLvlUp = value;
            lvUpBtnLis.GetComponent<UIButton>().isEnabled = canLvlUp;
        }
    }
    
    private bool canEvolve;
    private bool CanEvolve
    {
        get
        {
            return canEvolve;
        }
        set
        {

            canEvolve = value;
            lvEvolveBtnLis.GetComponent<UIButton>().isEnabled = canEvolve;
        }
    }

    #region Window

    public override void OnEnter()
    {
        itemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        Toggle(1);
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
        skillTabLis = UIEventListener.Get(skillContent.parent.gameObject);
        skillInitTitle = Utils.FindChild(skillContent, "SkillInitTitle").GetComponent<UILabel>();
        skillInitDesc = Utils.FindChild(skillContent, "SkillInitDesc").GetComponent<UILabel>();
        skillRandTitle = Utils.FindChild(skillContent, "SkillInitTitle").GetComponent<UILabel>();
        skillRandDesc = Utils.FindChild(skillContent, "SkillRandDesc").GetComponent<UILabel>();
        matchInfoDesc = Utils.FindChild(skillContent, "MatchInfoDesc").GetComponent<UILabel>();

        explainContent = Utils.FindChild(transform, "ExplainContent");
        explainTabLis = UIEventListener.Get(explainContent.parent.gameObject);
        explainName = Utils.FindChild(explainContent, "Name").GetComponent<UILabel>();
        explainDesc = Utils.FindChild(explainContent, "Desc").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick = OnBackBtnClicked;
        lvUpBtnLis.onClick = OnLvUpBtnClicked;
        lvEvolveBtnLis.onClick = OnEvolveBtnLis;
        skillTabLis.onClick = OnSkillTab;
        explainTabLis.onClick = OnExplainTab;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick = null;
        lvUpBtnLis.onClick = null;
        lvEvolveBtnLis.onClick = null;
        skillTabLis.onClick = null;
        explainTabLis.onClick = null;
    }

    private void OnSkillTab(GameObject go)
    {
        Toggle(1);
    }

    private void OnExplainTab(GameObject go)
    {
        Toggle(2);
    }

    private void OnLvUpBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show<UIItemLevelUpWindow>(true);
    }

    private void OnEvolveBtnLis(GameObject go)
    {
        WindowManager.Instance.Show<UIItemEvolveWindow>(true);
    }

    private void Refresh()
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        var initSkillId = ItemBaseInfoWindow.ItemDetail.InitSkillId;
        if (skillTmp.ContainsKey(initSkillId))
        {
            var initTmp = skillTmp[initSkillId];
            skillInitTitle.text = initTmp.BaseTmpl.Name;
            skillInitDesc.text = initTmp.BaseTmpl.Desc;
        }
        var randSkillId = ItemBaseInfoWindow.ItemDetail.RandSkillId;
        if (skillTmp.ContainsKey(randSkillId))
        {
            var randTmp = skillTmp[randSkillId];
            skillRandTitle.text = randTmp.BaseTmpl.Name;
            skillRandDesc.text = randTmp.BaseTmpl.Desc;
        }
        matchInfoDesc.text = ItemBaseInfoWindow.ItemDetail.MatchInfo;
        NGUITools.SetActive(explainContent.gameObject, true);
        explainName.text = ItemModeLocator.Instance.GetName(itemInfo.TmplId);
        explainDesc.text = ItemModeLocator.Instance.GetDesc(itemInfo.TmplId);
        NGUITools.SetActive(explainContent.gameObject, false);
        CanLvlUp = ItemModeLocator.Instance.GetCanLvlUp(itemInfo.TmplId);
        var isInEvolve = IsInEvolveTemplate();
        CanEvolve = ItemModeLocator.Instance.GetCanEvolve(itemInfo.TmplId) && isInEvolve;
    }

    private bool IsInEvolveTemplate()
    {
        var temp = ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls;
        return temp.Any(item => item.Value.Id == itemInfo.TmplId);
    }

    private void OnBackBtnClicked(GameObject go)
    {
        if(ItemModeLocator.Instance.GetItemDetailPos == ItemType.GetItemDetailInPanel)
        {
            WindowManager.Instance.Show(typeof(UIEquipDispTabWindow), true);
            WindowManager.Instance.Show(typeof(UIItemInfoWindow), false);
        }
        else if (ItemModeLocator.Instance.GetItemDetailPos == ItemType.GetItemDetailInHeroInfo)
        {
            WindowManager.Instance.Show<UIHeroDetailWindow>(true);
            WindowManager.Instance.Show<ItemBaseInfoWindow>(false);
        }
    }
    /// <summary>
    /// Toggle the button's color.
    /// </summary>
    /// <param name="index">Indicates which button will have the highlight color.</param>
    public void Toggle(int index)
    {
        switch (index)
        {
            case 1:
                skillContent.parent.GetComponent<UISprite>().spriteName = DownTabSpriteName;
                explainContent.parent.GetComponent<UISprite>().spriteName = NormalabSpriteName;
                NGUITools.SetActive(skillContent.gameObject ,true);
                NGUITools.SetActive(explainContent.gameObject, false);
                break;
            case 2:
                explainContent.parent.GetComponent<UISprite>().spriteName = DownTabSpriteName;
                skillContent.parent.GetComponent<UISprite>().spriteName = NormalabSpriteName;
                NGUITools.SetActive(explainContent.gameObject, true);
                NGUITools.SetActive(skillContent.gameObject, false);
                break;
        }
    }

    #endregion
}
