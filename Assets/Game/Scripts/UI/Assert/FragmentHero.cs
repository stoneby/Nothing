using UnityEngine;

public class FragmentHero : MonoBehaviour
{
    #region Public Fields

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

    [HideInInspector]
    public bool CanCombine;

    #endregion

    #region Private Fields

    private UIEventListener fragHeroLis;

    //Combine info from sever.
    private int templateId;
    private int materialCount;

    //Interface info.
    //private UISprite cornorSprite;
    private UISprite heroImage;
    private UILabel combineInfo;
    private int star;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize or refresh private fields.
    /// </summary>
    public void Refresh()
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[templateId];
        //cornorSprite.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        //star = heroTemplate.Star;
        //for (int i = 0; i < 5; i++)
        //{
        //    transform.Find("Star" + (i + 1)).gameObject.SetActive(i < star);
        //}

        HeroConstant.SetHeadByIndex(heroImage, heroTemplate.Icon - 1);
        combineInfo.text = MaterialCount + "/" + HeroModelLocator.Instance.HeroTemplates.HeroTmpls[templateId].ComposeCount;
        if (combineInfo.text == "0/0")
            Logger.Log("!!!!!!!!!!!!templateid:" + templateId + ", composecount" + combineInfo.text);
        fragHeroLis.onClick = OnFragmentHero;
    }

    public void SwitchColor(Color color)
    {
        heroImage.color = color;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Response event of click gameobject.
    /// </summary>
    /// <param name="go"></param>
    private void OnFragmentHero(GameObject go)
    {
        WindowManager.Instance.Show<FragmentConfirmWindow>(true);
        var tempWindow = WindowManager.Instance.GetWindow<FragmentConfirmWindow>();
        tempWindow.TemplateId = templateId;
        tempWindow.MaterialCount = materialCount;
        tempWindow.Refresh(CanCombine);
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Awake()
    {
        fragHeroLis = UIEventListener.Get(transform.gameObject);
        //cornorSprite = transform.Find("Cornor/CornorSprite").gameObject.GetComponent<UISprite>();
        heroImage = transform.Find("HeroImage").gameObject.GetComponent<UISprite>();
        combineInfo = transform.Find("CombineInfo").gameObject.GetComponent<UILabel>();
    }

    #endregion
}
