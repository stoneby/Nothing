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

    private int templateID;
    private int materialCount;

    private UISprite cornorSprite;
    private UISprite heroImage;
    private int star;
    private UILabel gong;
    private UILabel hp;
    private UILabel huiFu;
    private UILabel qiLi;
    private UILabel huoDeTuJing;
    private UILabel suiPianShuLiang;
    private UILabel cost;
    private UISprite confirmBTN;
    private UISprite closeBTN;
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

    private void OnConfirm(GameObject go)
    {
        CSLotteryCompose msg=new CSLotteryCompose();
        msg.HeroTemplateId = templateID;
        NetManager.SendMessage(msg);
        WindowManager.Instance.Show<FragmentConfirmWindow>(false);
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<FragmentConfirmWindow>(false);
    }

    #endregion

    #region Public Fields

    public void Refresh(bool isWhite)
    {
        //initialze or refresh the private fields.
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[templateID];
        cornorSprite.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        star = heroTemplate.Star;
        for (int i = 0; i < star; i++)
        {
            transform.Find("Hero/Star" + (i + 1).ToString()).gameObject.SetActive(true);
        }

        gong.text = heroTemplate.Attack.ToString();
        hp.text = heroTemplate.HP.ToString();
        huiFu.text = heroTemplate.Recover.ToString();
        qiLi.text = heroTemplate.MP.ToString();
        huoDeTuJing.text = heroTemplate.Desc;
        suiPianShuLiang.text = materialCount.ToString() + "/" + heroTemplate.ComposeCount;
        if (isWhite == true)
        {
            confirmBTN.color = Color.white;
            confirmLis.onClick = OnConfirm;
            confirmHoverEffect.enabled = true;
            if (materialCount < heroTemplate.ComposeCount)
            {
                cost.text = "需消耗万能碎片" + (heroTemplate.ComposeCount - materialCount) + "个";
            }
            else
            {
                cost.text = "";
            }
        }
        else
        {
            confirmBTN.color = Color.grey;
            confirmLis.onClick = null;
            confirmHoverEffect.enabled = false;
            cost.text = "";
        }
    }

    public int TemplateID
    {
        get { return templateID; }

        set { templateID = value; }
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

    // Use this for initialization
    void Awake()
    {
        confirmLis = UIEventListener.Get(transform.Find("ConfirmBTN").gameObject);
        closeLis = UIEventListener.Get(transform.Find("CloseBTN").gameObject);
        cornorSprite = transform.Find("Hero/Cornor/Sprite").gameObject.GetComponent<UISprite>();
        heroImage = transform.Find("Hero/HeroImage").gameObject.GetComponent<UISprite>();
        gong = transform.Find("HeroInfo/Gong").gameObject.GetComponent<UILabel>();
        hp = transform.Find("HeroInfo/Hp").gameObject.GetComponent<UILabel>();
        huiFu = transform.Find("HeroInfo/Huifu").gameObject.GetComponent<UILabel>();
        qiLi = transform.Find("HeroInfo/Qili").gameObject.GetComponent<UILabel>();
        huoDeTuJing = transform.Find("CombineInfo/Huoqutujing").gameObject.GetComponent<UILabel>();
        suiPianShuLiang = transform.Find("CombineInfo/Suipianshuliang").gameObject.GetComponent<UILabel>();
        cost = transform.Find("CombineInfo/Cost").gameObject.GetComponent<UILabel>();
        confirmBTN = transform.Find("ConfirmBTN").gameObject.GetComponent<UISprite>();
        closeBTN = transform.Find("CloseBTN").gameObject.GetComponent<UISprite>();
        confirmHoverEffect = transform.Find("ConfirmBTN").gameObject.GetComponent<UIButton>();
    }

    #endregion
}
