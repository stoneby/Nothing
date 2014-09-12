using System.Collections.Generic;
using System.Net.Mime;
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

    #endregion

    #region Private Fields

    private GameObject dimmerObject;
    private GameObject nextObject;
    private GameObject frameObject;
    private GameObject fingerObject;
    private GameObject dimmerCloseObject;
    private GameObject dimmerSkill1Object;
    private GameObject dimmerSkill2Object;
    private GameObject dimmerSkill3Object;
    private GameObject dimmerSkill4Object;

    private Vector3 originalNextPos;
    private UILabel textLabel;
    private List<string> showStringList = new List<string>();
    private int currentIndex;
    private UIEventListener.VoidDelegate lastClickDelegate;
    private UIEventListener frameClickLis;

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
        if (dimmerObject.activeSelf)
        {
            ObjectMove.StopMove();
            ObjectMove.CurrentObject = nextObject;
            ObjectMove.ShakeMove(true, originalNextPos);
        }
        else
        {
            ObjectMove.StopMove();
        }
    }

    /// <summary>
    /// Shield buttons or not.
    /// </summary>
    /// <param name="isShield"></param>
    public void ShieldButtons(List<bool> isShield)
    {
        if (isShield.Count != 5)
        {
            Logger.LogError("isSheild count num error in ShieldButtons, num=" + isShield.Count);
            return;
        }
        dimmerCloseObject.SetActive(isShield[0]);
        dimmerSkill1Object.SetActive(isShield[1]);
        dimmerSkill2Object.SetActive(isShield[2]);
        dimmerSkill3Object.SetActive(isShield[3]);
        dimmerSkill4Object.SetActive(isShield[4]);
    }

    /// <summary>
    /// Show a series of text, and execute a function finally.
    /// </summary>
    /// <param name="textNeedToShow">a series of text</param>
    /// <param name="onClick">function executed finally</param>
    public void ShowAPeriodInfos(List<string> textNeedToShow, UIEventListener.VoidDelegate onClick)
    {
        if (textNeedToShow != null)
        {
            showStringList = textNeedToShow;
            currentIndex = 0;
            textLabel.text = showStringList[currentIndex];
        }
        lastClickDelegate = onClick;
    }

    /// <summary>
    /// Clear all showing, remain to show text and VoidDelegate.
    /// </summary>
    public void ClearAll()
    {
        textLabel.text = null;
        showStringList.Clear();
        lastClickDelegate = null;
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
            ShowComponents(false, false, false, false);
        }
    }

    public void OnMoveFinger(GameObject go)
    {
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
        ObjectMove.StopMove();
        ObjectMove.CurrentObject = FingerBlinkObject;
        ObjectMove.EffectPlay(true, MoveTrace[0]);
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
        dimmerCloseObject = Utils.FindChild(transform, "DimmerClose").gameObject;
        dimmerSkill1Object = Utils.FindChild(transform, "DimmerSkill1").gameObject;
        dimmerSkill2Object = Utils.FindChild(transform, "DimmerSkill2").gameObject;
        dimmerSkill3Object = Utils.FindChild(transform, "DimmerSkill3").gameObject;
        dimmerSkill4Object = Utils.FindChild(transform, "DimmerSkill4").gameObject;

        textLabel = frameObject.GetComponentInChildren<UILabel>();
        frameClickLis = UIEventListener.Get(frameObject);
    }

    #endregion
}
