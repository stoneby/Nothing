using KXSGCodec;
using UnityEngine;

public class SDKLoginManager
{
    #region Private Fields

    //#if UNITY_ANDROID
    private static AndroidJavaClass jc;
    private static AndroidJavaObject jo;
    //#endif

    private static bool IsInit = false;
    private static float LastLoginTime = -10;

    private static SDKResponseManager sdkResponse
    {
        get { return GameObject.FindWithTag("SDKResponse").GetComponent<SDKResponseManager>(); }
    }

    #endregion

    #region Public Methods

    public static void DoLogin()
    {
#if UNITY_ANDROID||UNITY_IPHONE
        if (SDKResponseManager.TokenString != null)
        {
            var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", DeviceModel = SystemInfo.deviceModel, Token = SDKResponseManager.TokenString };
            NetManager.SendMessage(msg);
            return;
        }
#endif

#if UNITY_ANDROID
        Init();
        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (!SDKResponseManager.IsInitialized)
            {
                Debug.Log("Calling SDK initialize.");
                SDKResponseManager.WhichResponse = DoLogin;
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else if (SDKResponseManager.IsLogin)
            {
                if (GameConfig.BundleID == "cn.kx.sglm.jinshan")
                {
                    Debug.Log("Trying to logout in JinShan SDK, goto switch account.");
                    DoSwitchAccount();
                    return;
                }
                Debug.Log("Calling SDK logout.");
                SDKResponseManager.WhichResponse = DoLogin;
                jo.Call("logout", ServiceManager.UserID.ToString(), ServiceManager.ServerData.SID, "logout");
                WindowManager.Instance.Show<LoadingWaitWindow>(true);
            }
            else
            {
                Debug.Log("Calling SDK login.");
                SDKResponseManager.WhichResponse = null;
                jo.Call("login", ServiceManager.ServerData.SID, "login");
                WindowManager.Instance.Show<LoadingWaitWindow>(true);
            }
        }
#endif

#if UNITY_IPHONE
        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            if (SDKResponseManager.IsInitialized == false)
            {
                Debug.Log("Calling ActivateInitialize.");
                SDKResponseManager.WhichResponse = DoLogin;
                SDK_IOS.ActivateInitialize();
            }
            else
            {
                Debug.Log("Calling ActivateLogin.");
                SDKResponseManager.WhichResponse = null;
                sdkResponse.ShieldUI(false);
                SDK_IOS.ActivateLogin();
            }
        }
#endif
    }

    public static void DoSwitchAccount()
    {
#if UNITY_ANDROID
        Init();
        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponseManager.IsInitialized == false)
            {
                Debug.Log("Calling SDK initialize.");
                SDKResponseManager.WhichResponse += SwitchAccountAfterInit;
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                Debug.Log("Calling SDK switch account.");
                jo.Call("accountManager");
            }
        }
#endif
    }

    #endregion

    #region Private Methods

    private static void Init()
    {
        if (IsInit)
        {
            return;
        }
        IsInit = true;
        SDKResponseManager.IsInitialized = false;
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            return;
        }

#if UNITY_ANDROID
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

    private static void SwitchAccountAfterInit()
    {
#if UNITY_ANDROID
        Debug.Log("Calling SDK switch account.");
        jo.Call("accountManager");
        SDKResponseManager.WhichResponse = null;
#endif
    }

    #endregion
}
