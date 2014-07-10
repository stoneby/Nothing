using System.Runtime.Serialization.Formatters;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class TokenLoginManager : MonoBehaviour 
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
        jo.Call("login", "1", "login");
#endif
    }

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

#if UNITY_ANDROID      
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("initialize", "5508", "15", "1", "initialize");
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
