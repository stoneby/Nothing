using UnityEngine;

/// <summary>
/// Handle IOS/Android SDK login function.
/// </summary>
public class SDKLoginManager : MonoBehaviour
{
    #region Private Fields

    private UIEventListener loginLis;

#if UNITY_ANDROID

    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private AndroidJavaClass jc;
    /// <summary>
    /// Android necessary variable.
    /// </summary>
    private AndroidJavaObject jo;

#endif

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        loginLis.onClick = OnLogin;
    }

    private void UnInstallHandlers()
    {
        loginLis.onClick = null;
    }

    /// <summary>
    /// Login SDK function.
    /// </summary>
    /// <param name="go"></param>
    private void OnLogin(GameObject go)
    {

#if UNITY_ANDROID

        ServiceManager.IsDebugAccount = 0;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                //if initialized, login.
                Debug.Log("Calling SDK initialize.");
                SDKResponse.WhichResponse += LoginAfterInit;
                jo.Call("initialize", ServiceManager.GameID, GameConfig.Version, ServiceManager.FValue, "initialize");
            }
            else
            {
                //if not initialized, initialize SDK first.
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
                //if initialized, login.
				Debug.Log("Calling ActivateInitialize.");
			    SDKResponse.WhichResponse += LoginAfterInit;
				SDK_IOS.ActivateInitialize();
			}
			else 
			{
                //if not initialized, initialize SDK first.
				Debug.Log("Calling ActivateLogin.");
				SDK_IOS.ActivateLogin();
			}
        }

#endif

    }

    /// <summary>
    /// Login after initialize SDK.
    /// </summary>
    private void LoginAfterInit()
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

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        this.gameObject.SetActive(true);
        InstallHandlers();
        SDKResponse.IsInitialized = false;
        if (SystemInfo.deviceType == DeviceType.Desktop) return;

#if UNITY_ANDROID      

        //Setting Android SDK paras.
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

#endif

    }

    void Awake()
    {
        loginLis = UIEventListener.Get(transform.gameObject);
    }

    #endregion
}
