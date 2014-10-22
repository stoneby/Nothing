using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ReChargeWindow : Window
{
    #region Public Fields

    [HideInInspector]
    public string ReChargeID;

    public UIButton[] ReChargeButtons;

    #endregion

    #region Private Fields

    private UIGrid reChargeItemGrid;
    private UIEventListener closeBTN;

    #endregion

    #region

    public void Refresh()
    {
        foreach (var item in reChargeItemGrid.GetComponentsInChildren<ReChargeItem>())
        {
            //Add IsAdditionalMoney info here.

            item.IsAdditionalMoney = false;
        }
    }

    #endregion

    #region Private Methods

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<ReChargeWindow>(false);
    }

    private void InstallHandlers()
    {
        closeBTN.onClick = OnClose;
    }

    private void UnInstallHandlers()
    {
        closeBTN.onClick = null;
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        foreach (var item in ReChargeButtons)
        {
            item.isEnabled = true;
        }
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    void Awake()
    {
        reChargeItemGrid = Utils.FindChild(transform, "Grid").GetComponent<UIGrid>();
        closeBTN = UIEventListener.Get(Utils.FindChild(transform, "CloseBTN").gameObject);
        ReChargeButtons = reChargeItemGrid.GetComponentsInChildren<UIButton>();
    }

    #endregion
}
