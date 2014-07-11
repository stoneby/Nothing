using System.Runtime.Serialization.Formatters;
using KXSGCodec;
using UnityEngine;

public class SDKResponse : MonoBehaviour 
{
    //Process the string from SDK.
    //':'is the key char.
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

#if UNITY_IPHONE
	public void Respond(string str)
	{

		Debug.Log ("message is:" + str);
		string[] split=str.Split(new char[]{':'});
		if (split [0] == "initGameStart") 
		{
			if(split[1]=="ok")
			{
				Debug.Log("Calling ActivateLogin, setting isInitialized true.");
				SDK_IOS.ActivateLogin();
				SDKLoginManager.isInitialized=true;
			}
		}
		else if(split[0]=="login")
		{
			Debug.Log("Response login succeed, Sending message to server.");
			var msg=new CSTokenLoginMsg(){DeviceType=0,DeviceId="",Token=split[1]};
			NetManager.SendMessage(msg);
		}
        else if (split[0] == "Pay")
        {
            Debug.Log("Response Pay succeed, sending message to server.");           
        }
	}
#endif

    // Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
