using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class FragmentConfirmWindow : Window
{
    #region Public Fields

    public PropertyUpdater PropertyUpdater;
    public HeroBaseInfoRefresher HeroInfoRefresher;

    public int TemplateId
    {
        get { return templateId; }

        set { templateId = value; }
    }

    public int MaterialCount
    {
        get { return materialCount; }

        set { materialCount = value; }
    }

    #endregion

    #region Private Fields

    private UIEventListener confirmLis;
    private UIEventListener closeLis;

    // Combine info get from sever.
    private int templateId;
    private int materialCount;

    //Interface info.
    //private UISprite cornorSprite;
    //private UISprite heroImage;
    //private int star;
    //private UILabel attack;
    //private UILabel hp;
    //private UILabel recover;
    //private UILabel mp;
    private UILabel accessibleWay;
    private UILabel combineCount;
    private UILabel cost;
    private UILabel costLabel1;
    private UILabel costLabel2;
    private UIButton confirmBTN;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize or refresh private fields.
    /// </summary>
    /// <param name="canCombine"></param>
    public void Refresh(bool canCombine)
    {
        //initialze or refresh the private fields.
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[templateId];
        //cornorSprite.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        //star = heroTemplate.Star;
        //for (int i = 0; i < 5; i++)
        //{
        //    transform.Find("Hero/Star" + (i + 1)).gameObject.SetActive(i < star);
        //}

        HeroInfoRefresher.Refresh(heroTemplate);

        PropertyUpdater.UpdateProperty(0, 0, heroTemplate.Attack, heroTemplate.HP, heroTemplate.Recover, heroTemplate.MP);
        //attack.text = heroTemplate.Attack.ToString();
        //hp.text = heroTemplate.HP.ToString();
        //recover.text = heroTemplate.Recover.ToString();
        //mp.text = heroTemplate.MP.ToString();

        accessibleWay.text = heroTemplate.Desc;
        combineCount.text = materialCount + "/" + heroTemplate.ComposeCount;
        if (canCombine)
        {
            confirmBTN.isEnabled = true;
            if (materialCount < heroTemplate.ComposeCount)
            {
                cost.text = (heroTemplate.ComposeCount - materialCount).ToString();
                costLabel1.enabled = true;
                costLabel1.text = LanguageManager.Instance.GetTextValue("FragmentConfirm.CostLabel1_1");
                costLabel2.enabled = true;
            }
            else
            {
                cost.text = "";
                costLabel1.enabled = true;
                costLabel1.text = LanguageManager.Instance.GetTextValue("FragmentConfirm.CostLabel1_2");
                costLabel2.enabled = false;
            }
        }
        else
        {
            confirmBTN.isEnabled = false;
            cost.text = "";
            costLabel1.enabled = true;
            costLabel1.text = LanguageManager.Instance.GetTextValue("FragmentConfirm.CostLabel1_3");
            costLabel2.enabled = false;
        }
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        confirmLis.onClick += OnConfirm;
        closeLis.onClick += OnClose;
    }

    private void UnInstallHandlers()
    {
        confirmLis.onClick -= OnConfirm;
        closeLis.onClick -= OnClose;
    }

    /// <summary>
    /// Response event of click confirmBTN.
    /// </summary>
    /// <param name="go"></param>
    private void OnConfirm(GameObject go)
    {
        var msg = new CSLotteryCompose { HeroTemplateId = templateId };
        NetManager.SendMessage(msg);
        WindowManager.Instance.Show<FragmentConfirmWindow>(false);
    }

    /// <summary>
    /// Response event of click CloseBTN.
    /// </summary>
    /// <param name="go"></param>
    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<FragmentConfirmWindow>(false);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        confirmLis = UIEventListener.Get(Utils.FindChild(transform, "ConfirmBTN").gameObject);
        closeLis = UIEventListener.Get(Utils.FindChild(transform, "CloseDimmer").gameObject);
        //cornorSprite = Utils.FindChild(transform,"Hero/Cornor/Sprite").gameObject.GetComponent<UISprite>();
        //heroImage = Utils.FindChild(transform,"Hero/HeroImage").gameObject.GetComponent<UISprite>();
        //attack = Utils.FindChild(transform,"HeroInfo/Attack").gameObject.GetComponent<UILabel>();
        //hp = Utils.FindChild(transform,"HeroInfo/Hp").gameObject.GetComponent<UILabel>();
        //recover = Utils.FindChild(transform,"HeroInfo/Recover").gameObject.GetComponent<UILabel>();
        //mp = Utils.FindChild(transform,"HeroInfo/Mp").gameObject.GetComponent<UILabel>();
        accessibleWay = Utils.FindChild(transform, "AccessibleWay").gameObject.GetComponent<UILabel>();
        combineCount = Utils.FindChild(transform, "CombineCount").gameObject.GetComponent<UILabel>();
        cost = Utils.FindChild(transform, "Cost").gameObject.GetComponent<UILabel>();
        costLabel1 = Utils.FindChild(transform, "CostLabel1").gameObject.GetComponent<UILabel>();
        costLabel2 = Utils.FindChild(transform, "CostLabel2").gameObject.GetComponent<UILabel>();
        confirmBTN = Utils.FindChild(transform, "ConfirmBTN").gameObject.GetComponent<UIButton>();
    }

    #endregion
}
