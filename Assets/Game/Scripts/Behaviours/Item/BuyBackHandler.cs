using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class BuyBackHandler : MonoBehaviour 
{
    public event UIEventListener.VoidDelegate OkClicked;
    public event UIEventListener.VoidDelegate CancelClicked;

    private UIEventListener okLis;
    private UIEventListener cancelLis;
    private UILabel costCoins;

    private void Awake()
    {
        okLis = UIEventListener.Get(transform.Find("OK").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Cancel").gameObject);
        costCoins = Utils.FindChild(transform, "CostValue").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        okLis.onClick += OnOK;
        cancelLis.onClick += OnCancel;
    }

    private void UnInstallHandlers()
    {
        okLis.onClick -= OnOK;
        cancelLis.onClick -= OnCancel;
    }

    private void OnEnable()
    {
        InstallHandlers();
    }

    private void OnDestory()
    {
        UnInstallHandlers();
    }

    private void OnOK(GameObject go)
    {
        if (OkClicked != null)
        {
            OkClicked(go);
        }
        Destroy(gameObject);
    }

    private void OnCancel(GameObject go)
    {
        if (CancelClicked != null)
        {
            CancelClicked(go);
        }
        Destroy(gameObject);
    }

    public void Show(List<ItemInfo> infos)
    {
        
    }
}
