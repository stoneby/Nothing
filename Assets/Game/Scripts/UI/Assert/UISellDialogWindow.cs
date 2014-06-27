using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UISellDialogWindow : Window
{
    #region Public Fields

    public GameObject SellHeroPic;
    public event UIEventListener.VoidDelegate SellOkClicked;
    public event UIEventListener.VoidDelegate SellCancelClicked;

    #endregion

    #region Private Fields

    private UIEventListener sellOkLis;
    private UIEventListener sellCancelLis;
    private UIGrid sellGrid;
    private readonly CSHeroSell csHeroSell = new CSHeroSell { SellList = new List<long>() };

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        sellOkLis = UIEventListener.Get(Utils.FindChild(transform, "Button-SellOk").gameObject);
        sellCancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-SellCancel").gameObject);
        sellGrid = GetComponentInChildren<UIGrid>();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        sellOkLis.onClick = OnSellOkClicked;
        sellCancelLis.onClick = OnSellCancelClicked;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        sellOkLis.onClick = null;
        sellCancelLis.onClick = null;
    }

    private void OnSellOkClicked(GameObject go)
    {
        if (csHeroSell.SellList.Count > 0)
        {
            NetManager.SendMessage(csHeroSell);
            if(SellOkClicked != null)
            {
                SellOkClicked(go);
            }
        }
        else
        {
            WindowManager.Instance.Show<UISellDialogWindow>(false);
        }
    }

    private void OnSellCancelClicked(GameObject go)
    {
        WindowManager.Instance.Show<UISellDialogWindow>(false);
        if (SellCancelClicked != null)
        {
            SellCancelClicked(go);
        }
    }

    private void CleanUp()
    {
        var list = sellGrid.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
    }

    #endregion

    #region Public Methods

    public void InitDialog(List<long> uUids)
    {
        csHeroSell.SellList = uUids;
        for (var index = 0; index < uUids.Count; index++)
        {
            NGUITools.AddChild(sellGrid.gameObject, SellHeroPic);
        }
        sellGrid.Reposition();
    }

    #endregion
}
