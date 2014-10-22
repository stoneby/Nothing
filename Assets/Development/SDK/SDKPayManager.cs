using System.Collections;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Handle IOS/Android SDK pay function.
/// </summary>
public class SDKPayManager : MonoBehaviour
{
    #region Public Fields

    public static bool ExecuteHangUpReCharge;
    public static ReChargeItem HangUpReCharge;
    public static string GameId;

    #endregion

    #region Private Fields

    private static bool IsUiInGame
    {
        get { return GameConfig.BundleID == "com.tencent.tmgp.sanguommd"; }
    }

    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private static AndroidJavaClass jc;
    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private static AndroidJavaObject jo;

    private UIEventListener payLis;
    private const int IsPayingTime = 5;

    private static SDKResponseManager sdkResponse;

    #endregion

    #region Public Methods

    public static void DoPay(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCRechargeIdMsg;
        GameId = themsg.GameOrderId.ToString();

        if (IsUiInGame)
        {
            PayInGame();
        }
        else
        {
            PayInSDK();
        }
    }

    /// <summary>
    /// Pay SDK function.
    /// </summary>
    public static void PayInSDK(string productID = "", int price = 0, int amount = 0)
    {
#if UNITY_IPHONE
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            if (SDKResponseManager.IsInitialized == false)
            {
                //if initialized, pay.
                Debug.Log("Calling ActivateInitialize.");
                SDKResponseManager.WhichResponse += PayAfterInit;
                SDK_IOS.ActivateInitialize();
            }
            else
            {
                //if not initialized, initialize SDK first.
                Debug.Log("Calling pay in IOS SDK");
                sdkResponse.ShieldUI(false);
                SDK_IOS.ActivatePay(GameId);
            }
        }
#endif

#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponseManager.IsInitialized == false)
            {
                //if initialized, pay.
                Debug.Log("Calling SDK initialize.");
                SDKResponseManager.WhichResponse += PayAfterInit;
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                //if not initialized, initialize SDK first.
                if (IsUiInGame)
                {
                    Debug.Log("Calling SDK tencentPay.");
                    if (productID == "")
                    {
                        Debug.LogError("productID is empty in SDKPayManager.PayInSDK(,,), abort calling SDK tencentPay.");
                        return;
                    }

                    if (!WindowManager.Instance.ContainWindow<ReChargeWindow>())
                    {
                        Debug.LogError("ReCharge window not exist in SDKPayManager.PayInSDK()");
                    }
                    sdkResponse.ShieldUI(true);
                    sdkResponse.ShieldButton(WindowManager.Instance.GetWindow<ReChargeWindow>().ReChargeButtons);
                    jo.Call("tencentpay", ServiceManager.UserID.ToString(), PlayerModelLocator.Instance.RoleId.ToString(), ServiceManager.ServerData.SID, GameId, price.ToString(), amount.ToString(), productID, "tencentpay");
                }
                else
                {
                    Debug.Log("Calling SDK platformPay.");
                    sdkResponse.ShieldUI(true);
                    jo.Call("platformpay", ServiceManager.UserID.ToString(), PlayerModelLocator.Instance.RoleId.ToString(), ServiceManager.ServerData.SID, GameId, "platformpay");
                }
            }
        }
#endif
    }

    public static void DoOnPay()
    {
        //Send message to server.
        Debug.Log("Sending message to server.");
        var msg = new CSGetRechargeIdMsg();
        NetManager.SendMessage(msg);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Call pay UI in game.
    /// </summary>
    private static void PayInGame()
    {
        //var window = WindowManager.Instance.ContainWindow<ReChargeWindow>() ? WindowManager.Instance.GetWindow<ReChargeWindow>() : WindowManager.Instance.Show<ReChargeWindow>(true);
        var window = Singleton<WindowManager>.Instance.Show<ReChargeWindow>(true);

        window.ReChargeID = GameId;

        //Add IsAdditionalMoney info here.

        window.Refresh();

        if (ExecuteHangUpReCharge)
        {
            if (HangUpReCharge == null)
            {
                Debug.LogError("HangUpReCharge is null in SDKPayManager.");
                return;
            }
            HangUpReCharge.OnReCharge(new GameObject());
        }
    }

    /// <summary>
    /// Pay after initialize SDK.
    /// </summary>
    private static void PayAfterInit()
    {
#if UNITY_IPHONE
        Debug.Log("Calling pay in IOS SDK after initialize.");
        sdkResponse.ShieldUI(false);
        SDK_IOS.ActivatePay(GameId);
        SDKResponseManager.WhichResponse = null;
#endif

#if UNITY_ANDROID
        Debug.Log("Calling SDK platformPay.");
        sdkResponse.ShieldUI(true);
        jo.Call("platformpay", ServiceManager.UserID.ToString(), PlayerModelLocator.Instance.RoleId.ToString(),
            ServiceManager.ServerData.SID, GameId, "platformpay");
        SDKResponseManager.WhichResponse = null;
#endif
    }

    /// <summary>
    /// Send request for RechargeID message to server.
    /// </summary>
    /// <param name="go"></param>
    private void OnPay(GameObject go)
    {
        if (!go.GetComponent<UIButton>())
        {
            Debug.LogError("Can't get UIButton in OnPay object.");
        }
        sdkResponse.ShieldButton(go.GetComponent<UIButton>());

        DoOnPay();
    }

    private void InstallHandlers()
    {
        payLis.onClick = OnPay;
    }

    private void UnInstallHandlers()
    {
        payLis.onClick = null;
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);
        InstallHandlers();
#if UNITY_ANDROID
        //Setting Android SDK paras.
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

    void Awake()
    {
        payLis = UIEventListener.Get(transform.gameObject);
        sdkResponse = GameObject.FindWithTag("SDKResponse").GetComponent<SDKResponseManager>();
    }

    #endregion
}
