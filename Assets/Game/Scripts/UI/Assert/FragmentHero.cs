﻿using UnityEngine;

public class FragmentHero : MonoBehaviour
{
    #region Private Fields

    private UIEventListener fragHeroLis;

    //Combine info from sever.
    private int templateId;
    private int materialCount;

    //Interface info.
    private UISprite cornorSprite;
    private UISprite heroImage;
    private int star;

    #endregion

    #region Private Methods

    /// <summary>
    /// Response event of click gameobject.
    /// </summary>
    /// <param name="go"></param>
    private void OnFragmentHero(GameObject go)
    {        
        WindowManager.Instance.Show<FragmentConfirmWindow>(true);
        FragmentConfirmWindow tempWindow = WindowManager.Instance.GetWindow<FragmentConfirmWindow>();
        tempWindow.TemplateId = templateId;
        tempWindow.MaterialCount = materialCount;
        if (heroImage.color == Color.white)
        {
            tempWindow.Refresh(true);
        }
        else
        {
            tempWindow.Refresh(false);
        }
    }

    #endregion

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

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize or refresh private fields.
    /// </summary>
    public void Refresh()
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[templateId];
        cornorSprite.spriteName=HeroConstant.HeroJobPrefix+heroTemplate.Job;

        star = heroTemplate.Star;
        for (int i = 0; i < star; i++)
        {
            transform.Find("Star" + (i + 1)).gameObject.SetActive(true);
        }
        fragHeroLis.onClick = OnFragmentHero;
    }

    public void SwitchColor(Color color)
    {
        heroImage.color = color;
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Awake()
    {
        fragHeroLis = UIEventListener.Get(transform.gameObject);
        cornorSprite = transform.Find("Cornor/CornorSprite").gameObject.GetComponent<UISprite>();
        heroImage = transform.Find("HeroImage").gameObject.GetComponent<UISprite>();
    }

    #endregion    	
}
