using KXSGCodec;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardWindow : Window
{
    #region Private Fields

    //Stored lottery message.
    private SCLotteryList scLotteryList;
    private SCLotteryComposeList scLotteryComposeList;

    private readonly Color activedColor=new Color(255,237,0,255);
    private readonly Color deactivedColor = new Color(0, 0, 0, 255);

    //IsSet flag of lottery message.
    private struct IsSet
    {
        public bool Sclotterylist;
        public bool Sclotterycomposelist;
    }
    private IsSet isSet;

    /// <summary>
    /// Class contains necessary info for button click
    /// </summary>
    private class ToggleButton
    {
        public UILabel FrontLabel;
        public GameObject FrontGameObject;
        public GameObject ActiveGameObject;
    }

    /// <summary>
    /// Dictionary used for toggle button click
    /// </summary>
    private Dictionary<GameObject, ToggleButton> toggleButtonsMap = new Dictionary<GameObject, ToggleButton>();

    private UIEventListener closeLis;
    private GameObject heroAndItemSummit;
    private GameObject activitySummit;
    private GameObject fragCombine;
    private GameObject toggleButtons;
    public GameObject ToggleButtonPerfab;
    private GameObject defaultToggleButton;

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        closeLis.onClick = OnClose;
    }

    private void UnInstallHandlers()
    {
        closeLis.onClick = null;
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardWindow>(false);
    }

    /// <summary>
    /// Call back of ChooseHeroCard
    /// </summary>
    /// <param name="go"></param>
    private void OnChooseHeroCard(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].FrontLabel.color=activedColor;
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
        HeroAndItemSummitHandler.Refresh(ScLotteryList, true);
    }

    /// <summary>
    /// Call back of ChooseItemCard
    /// </summary>
    /// <param name="go"></param>
    private void OnChooseItemCard(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].FrontLabel.color = activedColor;
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
        HeroAndItemSummitHandler.Refresh(ScLotteryList, false);
    }

    /// <summary>
    /// Call back of SummitActivity
    /// </summary>
    /// <param name="go"></param>
    private void OnActivity(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].FrontLabel.color = activedColor;
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
    }

    /// <summary>
    /// Call back of FragmentCombine
    /// </summary>
    /// <param name="go"></param>
    private void OnFragCombine(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].FrontLabel.color = activedColor;
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);

        if (isSet.Sclotterycomposelist == false)
        {
            CSLotteryComposeList csMsg = new CSLotteryComposeList();
            NetManager.SendMessage(csMsg);
        }
        else
        {
            FragmentCombineHandler.Refresh(ScLotteryComposeList);
        }
    }

    /// <summary>
    /// DeActive all object and make label color black in toggleButtonMap.
    /// </summary>
    private void DeActiveAll()
    {
        foreach (var item in toggleButtonsMap.Values)
        {
            item.FrontLabel.color=deactivedColor;
            item.ActiveGameObject.SetActive(false);
            item.FrontGameObject.SetActive(false);
        }
    }

    private void NewToggleButtonAndAddToParent(string name, GameObject activeGameObject, UIEventListener.VoidDelegate lis,bool isDefault)
    {
        var newToggleButton = Instantiate(ToggleButtonPerfab) as GameObject;
        Utils.FindChild(newToggleButton.transform, "Label").GetComponent<UILabel>().text = name;
        newToggleButton.GetComponentInChildren<UIEventListener>().onClick = lis;
        toggleButtonsMap.Add(newToggleButton, new ToggleButton()
        {
            FrontLabel = Utils.FindChild(newToggleButton.transform, "Label").GetComponent<UILabel>(),
            ActiveGameObject = activeGameObject,
            FrontGameObject = Utils.FindChild(newToggleButton.transform, "Front").gameObject
        });
        Utils.MoveToParent(toggleButtons.transform, newToggleButton.transform);
        if (isDefault)
        {
            defaultToggleButton = newToggleButton;
        }
    }

    #endregion

    #region Public Fields

    public SCLotteryList ScLotteryList
    {
        get { return scLotteryList; }
        set
        {
            scLotteryList = value;
            isSet.Sclotterylist = true;
        }
    }

    public SCLotteryComposeList ScLotteryComposeList
    {
        get { return scLotteryComposeList; }
        set
        {
            scLotteryComposeList = value;
            isSet.Sclotterycomposelist = true;
        }
    }

    public HeroAndItemSummitHandler HeroAndItemSummitHandler;
    public ActivityHandler ActivityHandler;
    public FragmentCombineHandler FragmentCombineHandler;

    #endregion

    #region Public Methods

    /// <summary>
    /// Use this to generate toggleButtons with server's message.
    /// </summary>
    public void InitializeToggleButtons()
    {
        if (ScLotteryList == null)
        {
            Logger.LogError("Stored ScLotteryListMsg not found! Operation aborted!");
            return;
        }

        toggleButtonsMap.Clear();
        while (toggleButtons.transform.childCount != 0)
        {
            var toggleButton = toggleButtons.transform.GetChild(0);
            toggleButton.parent = null;
            Destroy(toggleButton.gameObject);
        }
        NewToggleButtonAndAddToParent(LanguageManager.Instance.GetTextValue("ChooseCard.hero lottery"), heroAndItemSummit, OnChooseHeroCard, true);
        NewToggleButtonAndAddToParent(LanguageManager.Instance.GetTextValue("ChooseCard.item lottery"), heroAndItemSummit, OnChooseItemCard, false);
        var lotteryInfoList = ScLotteryList.ListLotteryHeroInfo;
        for (int i = 1; i < lotteryInfoList.Count; i++)
        {
            NewToggleButtonAndAddToParent(lotteryInfoList[i].Name, activitySummit, OnActivity, false);
        }
        lotteryInfoList = ScLotteryList.ListLotteryItemInfo;
        for (int i = 1; i < lotteryInfoList.Count; i++)
        {
            NewToggleButtonAndAddToParent(lotteryInfoList[i].Name, activitySummit, OnActivity, false);
        }
        //Shield FragCombine module in current version.
        //NewToggleButtonAndAddToParent(LanguageManager.Instance.GetTextValue("ChooseCard.frag combine"), fragCombine, OnFragCombine, false);
        toggleButtons.GetComponent<UITable>().Reposition();

        //Go to default toggle button, change this if you want another default button.
        OnChooseHeroCard(defaultToggleButton);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        isSet = new IsSet()
        {
            Sclotterylist = false,
            Sclotterycomposelist = false
        };
        var msg = new CSLotteryList();
        NetManager.SendMessage(msg);
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    // Use this for initialization
    void Awake()
    {
        closeLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Close").gameObject);

        heroAndItemSummit = Utils.FindChild(transform, "HeroAndItemSummit").gameObject;
        activitySummit = Utils.FindChild(transform, "ActivitySummit").gameObject;
        fragCombine = Utils.FindChild(transform, "FragmentCombine").gameObject;

        toggleButtons = Utils.FindChild(transform, "Buttons").gameObject;
    }

    #endregion
}
