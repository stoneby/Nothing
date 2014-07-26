using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class SDKLoginManager : MonoBehaviour 
{
    #region Private Fields
    
    private UIEventListener loginLis;

#if UNITY_ANDROID
    private AndroidJavaClass jc;
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

    private void OnLogin(GameObject go)
    {
        
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

	#region Public Fields

	#endregion 

    #region Public Methods

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () 
    {
        this.gameObject.SetActive(true);
        InstallHandlers();
        SDKResponse.IsInitialized = false;
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
#if UNITY_ANDROID      
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }
	
	/// <summary>
    /// Update is called once per frame
	/// </summary>
	void Update () 
    {
        
    }

    void Awake()
    {
        loginLis = UIEventListener.Get(transform.gameObject);
    }

    #endregion
}
