using KXSGCodec;
using UnityEngine;
using System.Collections;

public class TokenLoginResponse : MonoBehaviour 
{
    //Process the string from SDK.
    //':'is the key char.
    public void message(string str)
    {
        Logger.Log("message is:"+str);
        string[] split=str.Split(new char[]{':'});
        if (split[0] == "startsResult is")
        {
            
        }
        if (split[0] == "loginResult is")
        {
            Logger.Log("Response succeed!Sending message to server.");
            var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", Token = split[1] };
            NetManager.SendMessage(msg);
        }
        if (split[0] == "logoutResult is")
        {

        }
        if (split[0] == "addroleResult is")
        {

        }
        if (split[0] == "iospayResult is")
        {

        }
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
