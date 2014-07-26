using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class FragmentConfirmWindow : Window
{
    #region Private Fields

    private UIEventListener confirmLis;
    private UIEventListener closeLis;
    
    // Combine info get from sever.
    private int templateId;
    private int materialCount;

    //Interface info.
    private UISprite cornorSprite;
    private UISprite heroImage;
    private int star;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel accessibleWay;
    private UILabel combineCount;
    private UILabel cost;
    private UILabel costLabel1;
    private UILabel costLabel2;
    private UISprite confirmBTN;
    private UIButton confirmHoverEffect;
     
    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        closeLis.onClick += OnClose;
    }

    private void UnInstallHandlers()
    {
        closeLis.onClick -= OnClose;
    }

    /// <summary>
    /// Response event of click confirmBTN.
    /// </summary>
    /// <param name="go"></param>
    private void OnConfirm(GameObject go)
    {
        var msg = new CSLotteryCompose {HeroTemplateId = templateId};
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

    #region Public Fields

    /// <summary>
    /// Initialize or refresh private fields.
    /// </summary>
    /// <param name="isWhite"></param>
    public void Refresh(bool isWhite)
    {
        //initialze or refresh the private fields.
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[templateId];
        cornorSprite.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        star = heroTemplate.Star;
        for (int i = 0; i < star; i++)
        {
            transform.Find("Hero/Star" + (i + 1)).gameObject.SetActive(true);
        }

        attack.text = heroTemplate.Attack.ToString();
        hp.text = heroTemplate.HP.ToString();
        recover.text = heroTemplate.Recover.ToString();
        mp.text = heroTemplate.MP.ToString();
        accessibleWay.text = heroTemplate.Desc;
        combineCount.text = materialCount + "/" + heroTemplate.ComposeCount;
        if (isWhite)
        {
            confirmBTN.color = Color.white;
            confirmLis.onClick = OnConfirm;
            confirmHoverEffect.enabled = true;
            if (materialCount < heroTemplate.ComposeCount)
            {
                cost.text = (heroTemplate.ComposeCount - materialCount).ToString();
                costLabel1.enabled = true;
                costLabel2.enabled = true;
            }
            else
            {
                cost.text = "";
                costLabel1.enabled = false;
                costLabel2.enabled = false;
            }
        }
        else
        {
            confirmBTN.color = Color.grey;
            confirmLis.onClick = null;
            confirmHoverEffect.enabled = false;
            cost.text = "";
            costLabel1.enabled = false;
            costLabel2.enabled = false;
        }
    }

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
        confirmLis = UIEventListener.Get(transform.Find("ConfirmBTN").gameObject);
        closeLis = UIEventListener.Get(transform.Find("CloseBTN").gameObject);
        cornorSprite = transform.Find("Hero/Cornor/Sprite").gameObject.GetComponent<UISprite>();
        heroImage = transform.Find("Hero/HeroImage").gameObject.GetComponent<UISprite>();
        attack = transform.Find("HeroInfo/Attack").gameObject.GetComponent<UILabel>();
        hp = transform.Find("HeroInfo/Hp").gameObject.GetComponent<UILabel>();
        recover = transform.Find("HeroInfo/Recover").gameObject.GetComponent<UILabel>();
        mp = transform.Find("HeroInfo/Mp").gameObject.GetComponent<UILabel>();
        accessibleWay = transform.Find("CombineInfo/AccessibleWay").gameObject.GetComponent<UILabel>();
        combineCount = transform.Find("CombineInfo/CombineCount").gameObject.GetComponent<UILabel>();
        cost = transform.Find("CombineInfo/Cost").gameObject.GetComponent<UILabel>();
        costLabel1 = transform.Find("CombineInfo/CostLabel1").gameObject.GetComponent<UILabel>();
        costLabel2 = transform.Find("CombineInfo/CostLabel2").gameObject.GetComponent<UILabel>();
        confirmBTN = transform.Find("ConfirmBTN").gameObject.GetComponent<UISprite>();
        confirmHoverEffect = transform.Find("ConfirmBTN").gameObject.GetComponent<UIButton>();
    }

    #endregion
}
