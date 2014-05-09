using System;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class AssertionWindow : Window
{
    public enum Type
    {
        Ok,
        YesNo,
        OkCancel
    }

    #region Public Fields

    public GameObject BackgroundDimmer;

    public UILabel TitleLabel;
    public UILabel MessageLabel;

    public UIButton Button1;
    public UIButton Button2;

    public delegate void VoidDelegate(GameObject sender);

    public event VoidDelegate OkButtonClicked;
    public event VoidDelegate CancelButtonClicked;

    public Type AssertType;
    public string Message;
    public string Title;

    #endregion

    #region Private Fields

    private UILabel label1;
    private UILabel label2;

    private Vector2 positionOkay;
    private Vector2 positionCancel;
    private Vector2 positionSingleOkay;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Logger.LogWarning("========================= OnEnter Assertion Window.");

        AdjustUI();
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Private Methods

    private void OnButton1Click(GameObject sender)
    {
        WindowManager.Instance.Show(typeof(AssertionWindow), false);

        if (OkButtonClicked != null)
        {
            OkButtonClicked(sender);
        }
    }

    private void OnButton2Click(GameObject sender)
    {
        WindowManager.Instance.Show(typeof(AssertionWindow), false);

        if (CancelButtonClicked != null)
        {
            CancelButtonClicked(sender);
        }
    }

    private void AdjustUI()
    {
        switch (AssertType)
        {
            case Type.Ok:
                Button1.gameObject.SetActive(true);
                Button2.gameObject.SetActive(false);
                Button1.transform.localPosition = positionSingleOkay;
                label1.text = "Ok";
                break;

            case Type.OkCancel:
                Button1.gameObject.SetActive(true);
                Button2.gameObject.SetActive(true);
                Button1.transform.localPosition = positionOkay;
                label1.text = "Ok";
                label2.text = "Cancel";
                break;

            case Type.YesNo:
                Button1.gameObject.SetActive(true);
                Button2.gameObject.SetActive(true);
                Button1.transform.localPosition = positionOkay;
                label1.text = "Yes";
                label2.text = "No";
                break;
        }
        TitleLabel.text = Title;
        MessageLabel.text = Message;
    }

    #endregion

    #region Mono

    private void Awake()
    {
        label1 = Button1.transform.Find("Label").GetComponent<UILabel>();
        label2 = Button2.transform.Find("Label").GetComponent<UILabel>();

        positionOkay = Button1.transform.localPosition;
        positionCancel = Button2.transform.localPosition;
        positionSingleOkay = (positionOkay + positionCancel)/2;

        UIEventListener.Get(Button1.gameObject).onClick += OnButton1Click;
        UIEventListener.Get(Button2.gameObject).onClick += OnButton2Click;
    }

    private void OnDestroy()
    {
        UIEventListener.Get(Button1.gameObject).onClick -= OnButton1Click;
        UIEventListener.Get(Button2.gameObject).onClick -= OnButton2Click;
    }

    #endregion
}
