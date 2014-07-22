using System.Runtime.InteropServices;
using UnityEngine;

public class SDK_IOS : MonoBehaviour
{
	#region Mono
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	#endregion

//#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void PressInitialize(int gameid,string appversion,string f,string extradata);
	[DllImport("__Internal")]
	private static extern void PressLogin(int serverid,string extradata);
	[DllImport("__Internal")]
	private static extern void PressLogout(int userid,int serverid,string extradata);
	[DllImport("__Internal")]
	private static extern void PressAddrole(int userid,string roleid,string rolename,int serverid,string extradata);
	[DllImport("__Internal")]
	private static extern void PressIospay(
		int userid,string roleid,string currency,string amount,string itemid,
		string itemname,int itemprice,string coin,string orderid,string channel,
		int serverid,string extradata);

    [DllImport("__Internal")]
    private static extern void PressPay(string remark, int userid, int serverid, string roleid, string extradata);
    [DllImport("__Internal")]
	private static extern void PressWeburl(int userid,int urltype,string url,int serverid);
    
	
	public static void ActivateInitialize()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			Debug.Log("Calling Initialize in SDK");
            PressInitialize(int.Parse(ServiceManager.GameID), GameConfig.Version, ServiceManager.FValue, "111111111");
		}
	}
	
	public static void ActivateLogin()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			Debug.Log("Calling Login in SDK");
            PressLogin(int.Parse(ServiceManager.ServerData.SID), "111111111");
		}
	}
	
	public static void ActivateLogout()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
            PressLogout(int.Parse(ServiceManager.UserID.ToString()), int.Parse(ServiceManager.ServerData.SID), "111111111");
		}
	}
	
	public static void ActivateAddrole()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
			//PressAddrole(159630813,"123456","123456",15,"111111111");
		}
	}
	
	public static void ActivateIospay()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
            PressIospay(int.Parse(ServiceManager.UserID.ToString()), "1", "1", "1", "1", "1", 1, "1", "1", "1", 15, "111111111");
		}
	}
	
	public static void ActivateWeburl()
	{
		if (Application.platform != RuntimePlatform.OSXEditor)
		{
            PressWeburl(int.Parse(ServiceManager.UserID.ToString()), 2, "1", int.Parse(ServiceManager.ServerData.SID));
		}
	}

    public static void ActivatePay(string gameOrderId)
    {
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            PressPay(gameOrderId, int.Parse(ServiceManager.UserID.ToString()), int.Parse(ServiceManager.ServerData.SID), "123456", "11");
        }
    }
//#endif
}
