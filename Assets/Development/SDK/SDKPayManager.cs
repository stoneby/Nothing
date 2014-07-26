using KXSGCodec;
using UnityEngine;

public class SDKPayManager : MonoBehaviour
{
    #region Private Fields
#if UNITY_ANDROID
    private static AndroidJavaClass jc;
    private static AndroidJavaObject jo;
#endif

    private UIEventListener payLis;

    private static string gameID;

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        payLis.onClick = OnPay;
    }

    private void UnInstallHandlers()
    {
        payLis.onClick = null;
    }

    private static void PayAfterInit()
    {
#if UNITY_IPHONE
        Debug.Log("Calling pay in IOS SDK after initialize.");
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

    private void OnPay(GameObject go)
    {
        //Send message to server.
        Debug.Log("Sending message to server.");
        var msg = new CSGetRechargeIdMsg();
        NetManager.SendMessage(msg);
    }

    #region Public Fields

    #endregion

    #region Public Methods
    public static void PayInSDK(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCRechargeIdMsg;
#if UNITY_IPHONE
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                Debug.Log("Calling ActivateInitialize.");
                SDKResponse.WhichResponse += PayAfterInit;
                gameID = themsg.GameOrderId.ToString();
                SDK_IOS.ActivateInitialize();
            }
            else
            {
                Debug.Log("Calling pay in IOS SDK");
                SDK_IOS.ActivatePay(themsg.GameOrderId.ToString());
            }
        }
#endif
#if UNITY_ANDROID
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                Debug.Log("Calling SDK initialize.");
                SDKResponse.WhichResponse += PayAfterInit;
                gameID = themsg.GameOrderId.ToString();
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                Debug.Log("Calling SDK platformPay.");
                jo.Call("platformpay",ServiceManager.UserID.ToString(),PlayerModelLocator.Instance.RoleId.ToString(),ServiceManager.ServerData.SID,themsg.GameOrderId.ToString(),"platformpay");
            }
        }
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
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
	}
	
	// Update is called once per frame
	void Update () 
    {

    }

    void Awake()
    {
        payLis = UIEventListener.Get(transform.gameObject);
    }

    #endregion
}
