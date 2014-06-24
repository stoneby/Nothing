using KXSGCodec;
using System.Collections;
using UnityEngine;

public class FragmentHero : MonoBehaviour
{
    #region Private Fields

    private UIEventListener fragHeroLis;

    private int templateID;
    private int materialCount;
    private UISprite cornorSprite;
    private UISprite heroImage;
    private int star;

    #endregion

    #region Private Methods

    private void OnFragmentHero(GameObject go)
    {        
        WindowManager.Instance.Show<FragmentConfirmWindow>(true);
        FragmentConfirmWindow tempWindow = WindowManager.Instance.GetWindow<FragmentConfirmWindow>();
        tempWindow.TemplateID = templateID;
        tempWindow.MaterialCount = materialCount;
        tempWindow.Refresh();
    }

    #endregion

    #region Public Fields

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

    #region Public Methods

    public void Refresh()
    {
        //intialize or refresh the cornorsprite, heroimage, stars and UIEventListener state.
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[templateID];
        cornorSprite.name=HeroConstant.HeroJobPrefix+heroTemplate.Job;

        star = heroTemplate.Star;
        for (int i = 0; i < star; i++)
        {
            transform.Find("Star" + (i + 1)).gameObject.SetActive(true);
        }
    }
    public void InstallHandlers()
    {
        fragHeroLis.onClick = OnFragmentHero;
        heroImage.color = Color.white;
    }

    public void UnInstallHandlers()
    {
        fragHeroLis.onClick = null;
        heroImage.color = Color.grey;
    }

    #endregion

    #region Mono

    private void Awake()
    {
        fragHeroLis = UIEventListener.Get(transform.gameObject);
        cornorSprite = transform.Find("Cornor/CornorSprite").gameObject.GetComponent<UISprite>();
        heroImage = transform.Find("HeroImage").gameObject.GetComponent<UISprite>();

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion    	
}
