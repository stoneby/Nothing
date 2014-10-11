using System.Collections;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Handle IOS/Android SDK pay function.
/// </summary>
public class SDKPayManager : MonoBehaviour
{
    #region Private Fields

#if UNITY_ANDROID

    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private static AndroidJavaClass jc;
    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private static AndroidJavaObject jo;

#endif

    private UIEventListener payLis;
    private const int isPayingTime = 5;

    private static string gameID;

    public static bool IsPaying = false;

    #endregion

    #region Public Methods

    /// <summary>
    /// Pay SDK function.
    /// </summary>
    /// <param name="msg"></param>
    public static void PayInSDK(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCRechargeIdMsg;
#if UNITY_IPHONE
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                //if initialized, pay.
                Debug.Log("Calling ActivateInitialize.");
                SDKResponse.WhichResponse += PayAfterInit;
                gameID = themsg.GameOrderId.ToString();
                SDK_IOS.ActivateInitialize();
            }
            else
            {
                //if not initialized, initialize SDK first.
                Debug.Log("Calling pay in IOS SDK");
                GlobalDimmerController.Instance.Transparent = true;
                GlobalDimmerController.Instance.DetectObject = null;
                GlobalDimmerController.Instance.Show(true);
                SDK_IOS.ActivatePay(themsg.GameOrderId.ToString());
            }
        }
#endif
#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                //if initialized, pay.
                Debug.Log("Calling SDK initialize.");
                SDKResponse.WhichResponse += PayAfterInit;
                gameID = themsg.GameOrderId.ToString();
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                //if not initialized, initialize SDK first.
                Debug.Log("Calling SDK platformPay.");
                jo.Call("platformpay",ServiceManager.UserID.ToString(),PlayerModelLocator.Instance.RoleId.ToString(),ServiceManager.ServerData.SID,themsg.GameOrderId.ToString(),"platformpay");
            }
        }
#endif
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Send request for RechargeID message to server.
    /// </summary>
    /// <param name="go"></param>
    private void OnPay(GameObject go)
    {
        //if (IsPaying)
        //{
        //    return;
        //}
        //IsPaying = true;
        gameObject.GetComponent<UIButton>().isEnabled = false;
        StartCoroutine("UpdatePayTime");

        //Send message to server.
        Debug.Log("Sending message to server.");
        var msg = new CSGetRechargeIdMsg();
        NetManager.SendMessage(msg);
    }

    private IEnumerator UpdatePayTime()
    {
        yield return new WaitForSeconds(isPayingTime);
        //IsPaying = false;
        gameObject.GetComponent<UIButton>().isEnabled = true;
    }

    private void InstallHandlers()
    {
        payLis.onClick = OnPay;
    }

    private void UnInstallHandlers()
    {
        payLis.onClick = null;
    }

    /// <summary>
    /// Pay after initialize SDK.
    /// </summary>
    private static void PayAfterInit()
    {
#if UNITY_IPHONE
        Debug.Log("Calling pay in IOS SDK after initialize.");
        GlobalDimmerController.Instance.Transparent = true;
        GlobalDimmerController.Instance.DetectObject = null;
        GlobalDimmerController.Instance.Show(true);
        SDK_IOS.ActivatePay(gameID);
        SDKResponse.WhichResponse = null;
#endif
#if UNITY_ANDROID
        Debug.Log("Calling SDK platformPay.");
        jo.Call("platformpay", ServiceManager.UserID.ToString(), PlayerModelLocator.Instance.RoleId.ToString(), ServiceManager.ServerData.SID, gameID, "platformpay");
        SDKResponse.WhichResponse = null;
#endif
    }

    #endregion

    #region Mono

    // Use this for initialization
	void Start () 
    {
	    this.gameObject.SetActive(true);
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
    }

    #endregion
}
