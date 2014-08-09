/// <summary>
/// Specific window controller.
/// </summary>
public class SimpleConfirmWindow : Window
{
    #region Private Fields

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

    void Awake()
    {
        okBTN = UIEventListener.Get(transform.Find("Buttons/Button-Ok").gameObject);
        cancelBTN = UIEventListener.Get(transform.Find("Buttons/Button-Cancel").gameObject);
        confirmLabel = transform.Find("ConfirmLabel").gameObject.GetComponent<UILabel>();
    }

    #endregion
}
