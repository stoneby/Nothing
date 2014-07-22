using System.Runtime.Serialization.Formatters;
using KXSGCodec;
using UnityEngine;

public class SDKResponse : MonoBehaviour
{
    #region Private Fields

    #endregion

    #region Public Fields

    public static bool IsInitialized=false;

    public delegate void Responses();
    public static Responses WhichResponse;

    #endregion

    #region Public Methods

    /// <summary>
    /// Process the string from Android SDK.
    /// ':'is the key char.
    /// </summary>
    /// <param name="str"></param>
#if UNITY_ANDROID
    public void message(string str)
    {
        Debug.Log("message is:"+str);
        string[] split=str.Split(new char[]{':'});
        if (split[0] == "startsResult is")
        {
            
        }
        else if (split[0] == "loginResult is")
        {
            Debug.Log("Response succeed!Sending message to server.");
            var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", Token = split[1] };
            NetManager.SendMessage(msg);
        }
        else if (split[0] == "logoutResult is")
        {

        }
        else if (split[0] == "addroleResult is")
        {

        }
        else if (split[0] == "iospayResult is")
        {

        }
    }
#endif

    /// <summary>
    /// Process the string from IOS SDK.
    /// ':'is the key char.
    /// </summary>
    /// <param name="str"></param>
#if UNITY_IPHONE
	public void Respond(string str)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) 
		{
			Debug.Log ("message is:" + str);
			string[] split=str.Split(new char[]{':'});
		    if (split[0] == "initGameStart")
		    {
		        if (split[1] == "ok")
		        {
		            Debug.Log("Setting isInitialized true.");
		            IsInitialized = true;

		            WhichResponse();
		        }
		    }
		    else if (split[0] == "login")
		    {
		        Debug.Log("Response login succeed, Sending message to server.");
		        var msg = new CSTokenLoginMsg() {DeviceType = 0, DeviceId = "", Token = split[1]};
		        NetManager.SendMessage(msg);
		    }
            else if (split[0] == "logout")
            {
                Debug.Log("Response logout succeed.");
            }
            else if (split[0] == "addRole")
            {
                Debug.Log("Response addRole succeed.");
            }
            else if (split[0] == "iosPay")
            {
                Debug.Log("Response iosPay succeed.");
            }
		    else if (split[0] == "Pay")
		    {
		        Debug.Log("Response Pay succeed, sending message to server.");
		        var msg = new CSRefreshRechargeMsg() {OrderId = split[1]};
		        NetManager.SendMessage(msg);
		    }
		}
	}
#endif

    #endregion

    // Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
