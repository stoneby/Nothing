using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class GreenHandGuideWindow : Window
{
    #region Public Fields

    public ObjectMoveController ObjectMove;
    public List<Vector3> MoveTrace = new List<Vector3>();
    public GameObject FingerBlinkObject;
    public GameObject FingerBlinkButton;

    #endregion

    #region Private Fields

    private GameObject dimmerObject;
    private GameObject nextObject;
    private GameObject frameObject;
    private GameObject fingerObject;
    private GameObject dimmerButtomObject;

    private Vector3 originalNextPos;
    private UILabel textLabel;
    private List<string> showStringList = new List<string>();
    private int currentIndex;
    private UIEventListener.VoidDelegate lastClickDelegate;
    private UIEventListener frameClickLis;

    private const int ToppestPanelDepth = 150;

    #endregion

    #region Public Methods

    /// <summary>
    /// Show frame and finger or not.
    /// </summary>
    /// <param name="isShowDimmer"></param>
    /// <param name="isShowFrame"></param>
    /// <param name="isShowFinger"></param>
    /// <param name="isShowFingerBlink"></param>
    public void ShowComponents(bool isShowDimmer, bool isShowFrame, bool isShowFinger, bool isShowFingerBlink)
    {
        dimmerObject.SetActive(isShowDimmer);
        frameObject.SetActive(isShowFrame);
        fingerObject.SetActive(isShowFinger);
        FingerBlinkObject.SetActive(isShowFingerBlink);

        //Shake nextObject if frame is active.
        ObjectMove.StopMove();
        if (frameObject.activeSelf)
        {
            ObjectMove.CurrentObject = nextObject;
            ObjectMove.ShakeMove(true, originalNextPos);
        }
    }

    public void ShowDimmerButtom(bool isShowDimmerButtom)
    {
        dimmerButtomObject.SetActive(isShowDimmerButtom);
    }

    /// <summary>
    /// Show a series of text, and execute a function finally.
    /// </summary>
    /// <param name="textNeedToShow">a series of text</param>
    /// <param name="onClick">function executed finally</param>
    public void ShowAPeriodInfos(List<string> textNeedToShow, UIEventListener.VoidDelegate onClick)
    {
        ClearAll();
        if (textNeedToShow != null)
        {
            showStringList = textNeedToShow;
            currentIndex = 0;
            textLabel.text = showStringList[currentIndex];
            lastClickDelegate = onClick;
        }
        else
        {
            showStringList = null;
            currentIndex = 0;
            lastClickDelegate = onClick;
            lastClickDelegate(new GameObject());
        }
    }

    /// <summary>
    /// Clear all showing, remain to show text and VoidDelegate.
    /// </summary>
    public void ClearAll()
    {
        textLabel.text = null;
        showStringList = new List<string>();
        lastClickDelegate = null;
    }

    public void OnMoveFinger(GameObject go)
    {
        if (GreenHandGuideHandler.Instance.ConfigMode == "NormalMove")
        {
            ShowDimmer(true);
        }
        else if (GreenHandGuideHandler.Instance.ConfigMode == "BattleMove")
        {
            ShowDimmer(false);
        }
        else
        {
            Logger.LogError("!!!!!!!!!!Not correct mode in MoveFinger.");
            return;
        }

        ObjectMove.StopMove();
        ObjectMove.CurrentObject = fingerObject;
        ObjectMove.DirectMove(true, MoveTrace);
    }

    public void OnBlinkFinger(GameObject go)
    {
        if (MoveTrace.Count != 1)
        {
            Logger.LogError("Can't do blink cause moveTrace.count=" + MoveTrace.Count);
            return;
        }

        if (GreenHandGuideHandler.Instance.ConfigMode == "NormalBlink")
        {
            ShowDimmer(true);
        }
        else if (GreenHandGuideHandler.Instance.ConfigMode == "BattleBlink")
        {
            ShowDimmer(false);
        }
        else
        {
            Logger.LogError("!!!!!!!!!!Not correct mode in BlinkFinger.");
            return;
        }

        ObjectMove.StopMove();
        ObjectMove.CurrentObject = FingerBlinkObject;
        ObjectMove.EffectPlay(true, MoveTrace[0]);
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        frameClickLis.onClick = OnClickFrame;
    }

    private void UnInstallHandlers()
    {
        frameClickLis.onClick = null;
    }

    /// <summary>
    /// Always go to this function after click frame.
    /// If you want to go to another function, bind it to lastClickDelegate.
    /// </summary>
    /// <param name="go"></param>
    private void OnClickFrame(GameObject go)
    {
        currentIndex++;
        if (currentIndex < showStringList.Count)
        {
            textLabel.text = showStringList[currentIndex];
            return;
        }

        if (lastClickDelegate != null)
        {
            ShowComponents(false, false, lastClickDelegate == OnMoveFinger, lastClickDelegate == OnBlinkFinger);
            lastClickDelegate(frameObject);
        }
        else
        {
            Logger.LogWarning("Deactive 4 components in GreenHandGuide window.");
            ShowComponents(false, false, false, false);
        }
    }

    private void ShowDimmer(bool isShow)
    {
        if (isShow)
        {
            //Deactive all components.
            ShowComponents(false, false, false, false);

            GlobalDimmerController.Instance.Transparent = true;
            GlobalDimmerController.Instance.DetectObject = FingerBlinkButton;
        }

        Logger.Log("!!!!!!!!!!Show dimmer:" + isShow);
        GlobalDimmerController.Instance.Show(isShow);
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
        dimmerObject = Utils.FindChild(transform, "Dimmer").gameObject;
        nextObject = Utils.FindChild(transform, "Next").gameObject;
        originalNextPos = nextObject.transform.position;
        frameObject = Utils.FindChild(transform, "Frame").gameObject;
        fingerObject = Utils.FindChild(transform, "Finger").gameObject;
        FingerBlinkObject = Utils.FindChild(transform, "FingerBlink").gameObject;
        dimmerButtomObject = Utils.FindChild(transform, "DimmerButtom").gameObject;

        textLabel = frameObject.GetComponentInChildren<UILabel>();
        frameClickLis = UIEventListener.Get(frameObject);
    }

    #endregion
}
