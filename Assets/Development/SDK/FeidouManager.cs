using KXSGCodec;
using UnityEngine;

public class FeidouManager
{

#if UNITY_ANDROID
    private static AndroidJavaClass jc;
    private static AndroidJavaObject jo;
#endif

    private static bool HaveInit = false;
    private static float LastLoginTime = -10;

    private static bool CanLogin()
    {
        var t = Time.realtimeSinceStartup;
        if (t - LastLoginTime > 1)
        {
            LastLoginTime = t;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void DoLogin()
    {
#if UNITY_ANDROID||UNITY_IPHONE
        if (SDKResponse.TokenString != null)
        {
            var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", DeviceModel = SystemInfo.deviceModel, Token = SDKResponse.TokenString };
            NetManager.SendMessage(msg);
            return;
        }
#endif

#if UNITY_ANDROID
        Init();
        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                Debug.Log("Calling SDK initialize.");
                SDKResponse.WhichResponse += LoginAfterInit;
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                Debug.Log("Calling SDK login.");
                jo.Call("login", ServiceManager.ServerData.SID, "login");
                WindowManager.Instance.Show<LoadingWaitWindow>(true);
            }
        }
#endif

#if UNITY_IPHONE
        ServiceManager.IsDebugAccount = 0;
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			if(SDKResponse.IsInitialized==false)
			{
				Debug.Log("Calling ActivateInitialize.");
			    SDKResponse.WhichResponse += LoginAfterInit;
				SDK_IOS.ActivateInitialize();
			}
			else 
			{
				Debug.Log("Calling ActivateLogin.");
				SDK_IOS.ActivateLogin();
			}
        }
#endif
    }

    private static void LoginAfterInit()
    {
#if UNITY_IPHONE
        Debug.Log("Calling ActivateLogin after initialize.");
        SDK_IOS.ActivateLogin();
        SDKResponse.WhichResponse = null;
#endif
#if UNITY_ANDROID
        Debug.Log("Calling SDK login.");
        jo.Call("login", ServiceManager.ServerData.SID, "login");
        WindowManager.Instance.Show<LoadingWaitWindow>(true);
        SDKResponse.WhichResponse = null;
#endif
    }

    private static void Init()
    {
        if (HaveInit) return;
        HaveInit = true;
        SDKResponse.IsInitialized = false;
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
#if UNITY_ANDROID
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

    public static void DoSwitchAccount()
    {
        //if (!CanLogin()) return;
#if UNITY_ANDROID
        Init();
        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                Debug.Log("Calling SDK initialize.");
                SDKResponse.WhichResponse += SwitchAccountAfterInit;
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

    private static void SwitchAccountAfterInit()
    {
#if UNITY_ANDROID
        Debug.Log("Calling SDK switch account.");
        jo.Call("accountManager");
        SDKResponse.WhichResponse = null;
#endif
    }
}
