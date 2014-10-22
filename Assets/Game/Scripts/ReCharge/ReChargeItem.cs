using System;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ReChargeItem : MonoBehaviour
{
    #region Public Fields

    public bool IsMonth;

    public string ProductID;
    public int Price;
    public int Amount;

    public string Additional;

    [HideInInspector]
    public bool IsAdditionalMoney
    {
        get { return isAdditionalMoney; }
        set
        {
            additionalObject.SetActive(value);
            isAdditionalMoney = value;
        }
    }

    #endregion

    #region Private Fields

    private bool isAdditionalMoney;

    private GameObject additionalObject;
    private UIEventListener reChargeBTN;
    private UILabel priceLabel;
    private UILabel amountLabel;
    private UILabel additionalLabel;
    private UISprite moneySprite;

    private const string MonthSpriteName = "ReChargeMonth";
    private const string NormalSpriteName = "Diamond1";

    #endregion

    #region Public Methods

    public void OnReCharge(GameObject go)
    {
        if (SDKPayManager.GameId != null)
        {
            SDKPayManager.ExecuteHangUpReCharge = false;
            SDKPayManager.PayInSDK(ProductID, Price, Amount);
        }
        else
        {
            SDKPayManager.DoOnPay();
            SDKPayManager.ExecuteHangUpReCharge = true;
            SDKPayManager.HangUpReCharge = this;
        }
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        reChargeBTN.onClick = OnReCharge;
    }

    private void UnInstallHandlers()
    {
        reChargeBTN.onClick = null;
    }

    #endregion

    #region Mono

    void OnEnable()
    {
        InstallHandlers();
    }

    void OnDisable()
    {
        UnInstallHandlers();
    }

    void Awake()
    {
        additionalObject = Utils.FindChild(transform, "AdditionalDescLabel").gameObject;
        additionalLabel = additionalObject.GetComponent<UILabel>();
        reChargeBTN = UIEventListener.Get(Utils.FindChild(transform, "ReChargeBTN").gameObject);
        priceLabel = transform.Find("ReChargeBTN/Label").GetComponent<UILabel>();
        amountLabel = Utils.FindChild(transform, "DescLabel").GetComponent<UILabel>();
        moneySprite = Utils.FindChild(transform, "DescSprite").GetComponent<UISprite>();

        //Initialize UI.
        moneySprite.spriteName = IsMonth ? MonthSpriteName : NormalSpriteName;
        priceLabel.text = "￥" + Price;
        amountLabel.text = Amount + "元宝";
        if (Additional != null)
        {
            var splited = Additional.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (splited.Length == 1 && !IsMonth)
            {
                additionalLabel.text = "送" + splited + "元宝";
            }
            else if (splited.Length == 2 && IsMonth)
            {
                additionalLabel.text = "每天" + splited[0] + "元宝\n共" + splited[1] + "天";
            }
            else
            {
                Logger.LogError("Not correct additional variable in ReChargeItem.Awake()");
                return;
            }
        }
    }

    #endregion
}
