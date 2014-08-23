/// <summary>
/// ConfirmWindow got 2 buttons and 1 label.
/// </summary>
public class SimpleConfirmWindow : Window
{
    #region Private Fields

    // Buttons' listener and label.
    private UIEventListener okBTN;
    private UIEventListener cancelBTN;
    private UILabel confirmLabel;

    #endregion

    #region Private Methods

    private void UnInstallHandlers()
    {
        okBTN.onClick = null;
        cancelBTN.onClick = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set label's text, assign buttons' listener.
    /// </summary>
    /// <param name="labelString"></param>
    /// <param name="lisOK"></param>
    /// <param name="lisCancel"></param>
    public void SetLabelAndLis(string labelString, UIEventListener.VoidDelegate lisOK, UIEventListener.VoidDelegate lisCancel)
    {
        confirmLabel.text = labelString;
        okBTN.onClick = lisOK;
        cancelBTN.onClick = lisCancel;
    }

    #endregion

    #region Window

    public override void OnEnter()
    {

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
        okBTN = UIEventListener.Get(transform.Find("Buttons/Button-Ok").gameObject);
        cancelBTN = UIEventListener.Get(transform.Find("Buttons/Button-Cancel").gameObject);
        confirmLabel = transform.Find("ConfirmLabel").gameObject.GetComponent<UILabel>();
    }

    #endregion
}
