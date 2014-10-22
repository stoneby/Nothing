using KXSGCodec;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific window controller for ChooseCard module.
/// </summary>
public class ChooseCardWindow : Window
{
    #region Public Fields

    /// <summary>
    /// Stored lottery message.
    /// </summary>
    public SCLotteryList ScLotteryList
    {
        get { return scLotteryList; }
        set
        {
            scLotteryList = value;
            isSet.Sclotterylist = true;
        }
    }

    /// <summary>
    /// Stored fragment compose message.
    /// </summary>
    public SCLotteryComposeList ScLotteryComposeList
    {
        get { return scLotteryComposeList; }
        set
        {
            scLotteryComposeList = value;
            isSet.Sclotterycomposelist = true;
        }
    }

    /// <summary>
    /// Control hero and item summit.
    /// </summary>
    public HeroAndItemSummitHandler HeroAndItemSummitHandler;

    /// <summary>
    /// control activity summit.
    /// </summary>
    public ActivityHandler ActivityHandler;

    /// <summary>
    /// Control fragment combine.
    /// </summary>
    public FragmentCombineHandler FragmentCombineHandler;

    /// <summary>
    /// Tab button perfab for creating toggle buttons.
    /// </summary>
    public GameObject ToggleButtonPerfab;

    #endregion

    #region Private Fields

    /// <summary>
    /// Shield FragCombine module in current version, may open in next version.
    /// </summary>
    private const bool IsOpenFragmentCombine = true;

    private SCLotteryList scLotteryList;
    private SCLotteryComposeList scLotteryComposeList;

    //private readonly Color activedColor = new Color(255, 237, 0, 255);
    //private readonly Color deactivedColor = new Color(0, 0, 0, 255);

    /// <summary>
    /// IsSet flag of lottery and fragment combine message.
    /// </summary>
    private struct IsSet
    {
        public bool Sclotterylist;
        public bool Sclotterycomposelist;
    }
    private IsSet isSet;

    /// <summary>
    /// Class contains necessary info for toggle button click.
    /// </summary>
    private class ToggleButton
    {
        public UILabel ActivedLabel;
        public UILabel DeactivedLabel;
        public GameObject FrontGameObject;
        public GameObject ActiveGameObject;
    }

    /// <summary>
    /// Dictionary to control toggle button click
    /// </summary>
    private readonly Dictionary<GameObject, ToggleButton> toggleButtonsMap = new Dictionary<GameObject, ToggleButton>();

    /// <summary>
    /// Listener for close button click.
    /// </summary>
    private UIEventListener closeListener;

    private GameObject heroAndItemSummit;
    private GameObject activitySummit;
    private GameObject fragCombine;
    private GameObject toggleButtons;
    private GameObject defaultToggleButton;

    #endregion

    #region Public Methods

    /// <summary>
    /// Generate toggle buttons with server's message.
    /// </summary>
    public void InitializeToggleButtons()
    {
        //Safety check message.
        if (ScLotteryList == null)
        {
            Logger.LogError("Stored ScLotteryListMsg not found! Operation aborted!");
            return;
        }

        //Clear toggle buttons.
        toggleButtonsMap.Clear();
        while (toggleButtons.transform.childCount != 0)
        {
            var toggleButton = toggleButtons.transform.GetChild(0);
            toggleButton.parent = null;
            Destroy(toggleButton.gameObject);
        }

        //Create toggle buttons with server's message.
        SetToggleButton(LanguageManager.Instance.GetTextValue("ChooseCard.hero lottery"), heroAndItemSummit, OnChooseHeroCard, true);
        SetToggleButton(LanguageManager.Instance.GetTextValue("ChooseCard.item lottery"), heroAndItemSummit, OnChooseItemCard, false);

        var lotteryInfoList = ScLotteryList.ListLotteryHeroInfo;
        for (int i = 1; i < lotteryInfoList.Count; i++)
        {
            SetToggleButton(lotteryInfoList[i].Name, activitySummit, OnActivity, false);
        }

        lotteryInfoList = ScLotteryList.ListLotteryItemInfo;
        for (int i = 1; i < lotteryInfoList.Count; i++)
        {
            SetToggleButton(lotteryInfoList[i].Name, activitySummit, OnActivity, false);
        }

        if (IsOpenFragmentCombine)
        {
            SetToggleButton(LanguageManager.Instance.GetTextValue("ChooseCard.frag combine"), fragCombine, OnFragCombine, false);
        }

        toggleButtons.GetComponent<UITable>().Reposition();

        //Go to default toggle button, change defaultToggleButton if you want another default button.
        OnChooseHeroCard(defaultToggleButton);
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        closeListener.onClick = OnClose;
    }

    private void UnInstallHandlers()
    {
        closeListener.onClick = null;
    }

    /// <summary>
    /// On close button click.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void OnClose(GameObject go)
    {
        //Set GreenHand info.
        if (!GreenHandGuideHandler.Instance.SummitFinishFlag)
        {
            GreenHandGuideHandler.Instance.SummitFinishFlag = true;
            GreenHandGuideHandler.Instance.SendEndMessage(4);
        }

        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    /// <summary>
    /// Call back of ChooseHeroCard toggle button click.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void OnChooseHeroCard(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].ActivedLabel.gameObject.SetActive(true);
        toggleButtonsMap[go].DeactivedLabel.gameObject.SetActive(false);
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
        HeroAndItemSummitHandler.Refresh(ScLotteryList, true);
    }

    /// <summary>
    /// Call back of ChooseItemCard toggle button click.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void OnChooseItemCard(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].ActivedLabel.gameObject.SetActive(true);
        toggleButtonsMap[go].DeactivedLabel.gameObject.SetActive(false);
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
        HeroAndItemSummitHandler.Refresh(ScLotteryList, false);
    }

    /// <summary>
    /// Call back of SummitActivity toggle button click.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void OnActivity(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].ActivedLabel.gameObject.SetActive(true);
        toggleButtonsMap[go].DeactivedLabel.gameObject.SetActive(false);
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);
    }

    /// <summary>
    /// Call back of FragmentCombine toggle button click.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void OnFragCombine(GameObject go)
    {
        DeActiveAll();
        toggleButtonsMap[go].ActivedLabel.gameObject.SetActive(true);
        toggleButtonsMap[go].DeactivedLabel.gameObject.SetActive(false);
        toggleButtonsMap[go].ActiveGameObject.SetActive(true);
        toggleButtonsMap[go].FrontGameObject.SetActive(true);

        //Request message to server if no stored message found.
        if (!isSet.Sclotterycomposelist)
        {
            var csMsg = new CSLotteryComposeList();
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
            item.ActivedLabel.gameObject.SetActive(false);
            item.DeactivedLabel.gameObject.SetActive(true);
            item.ActiveGameObject.SetActive(false);
            item.FrontGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// New and set toggle buttons.
    /// </summary>
    /// <param name="buttonName">toggle button's name</param>
    /// <param name="activeGameObject">toggle button's front object</param>
    /// <param name="lisDelegate">toggle button's listener</param>
    /// <param name="isDefault">set this button to default or not</param>
    private void SetToggleButton(string buttonName, GameObject activeGameObject, UIEventListener.VoidDelegate lisDelegate, bool isDefault)
    {
        var newToggleButton = Instantiate(ToggleButtonPerfab) as GameObject;

        //Set toggle button's name.
        Utils.FindChild(newToggleButton.transform, "ActivedLabel").GetComponent<UILabel>().text = buttonName;
        Utils.FindChild(newToggleButton.transform, "DeactivedLabel").GetComponent<UILabel>().text = buttonName;

        //Set toggle button's listener.
        newToggleButton.GetComponentInChildren<UIEventListener>().onClick = lisDelegate;

        //Add toggle button to dictionary controller.
        toggleButtonsMap.Add(newToggleButton, new ToggleButton()
        {
            ActivedLabel = Utils.FindChild(newToggleButton.transform, "ActivedLabel").GetComponent<UILabel>(),
            DeactivedLabel = Utils.FindChild(newToggleButton.transform, "DeactivedLabel").GetComponent<UILabel>(),
            ActiveGameObject = activeGameObject,
            FrontGameObject = Utils.FindChild(newToggleButton.transform, "Front").gameObject
        });

        Utils.MoveToParent(toggleButtons.transform, newToggleButton.transform);

        //Set default button or not.
        if (isDefault)
        {
            defaultToggleButton = newToggleButton;
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        InstallHandlers();

        //Request message to server for initialization.
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

    void Awake()
    {
        closeListener = UIEventListener.Get(Utils.FindChild(transform, "Button-Close").gameObject);

        heroAndItemSummit = Utils.FindChild(transform, "HeroAndItemSummit").gameObject;
        activitySummit = Utils.FindChild(transform, "ActivitySummit").gameObject;
        fragCombine = Utils.FindChild(transform, "FragmentCombine").gameObject;

        toggleButtons = Utils.FindChild(transform, "Buttons").gameObject;
    }

    #endregion
}
