﻿using UnityEngine;

public class FeidouManager
{

#if UNITY_ANDROID
    private static AndroidJavaClass jc;
    private static AndroidJavaObject jo;
#endif

    private static bool HaveInit = false;

    public static void DoLogin()
    {
        Init();
#if UNITY_ANDROID
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
}
