using KXSGCodec;
using UnityEngine;

public class SDKPayManager : MonoBehaviour
{
    #region Private Fields

    private UIEventListener payLis;

    private static string gameID;

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        payLis.onClick = OnPay;
    }

    private void UnInstallHandlers()
    {
        payLis.onClick = null;
    }

    private static void PayAfterInit()
    {
#if UNITY_IPHONE
        Debug.Log("Calling pay in IOS SDK after initialize.");
        SDK_IOS.ActivatePay(gameID);
        SDKResponse.WhichResponse = null;
#endif
    }

    #endregion

    private void OnPay(GameObject go)
    {
        //Send message to server.
        Debug.Log("Sending message to server.");
        var msg = new CSGetRechargeIdMsg();
        NetManager.SendMessage(msg);
    }

    #region Public Fields

    #endregion

    #region Public Methods
    public static void PayInSDK(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCRechargeIdMsg;
#if UNITY_IPHONE
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            if (SDKResponse.IsInitialized == false)
            {
                Debug.Log("Calling ActivateInitialize.");
                SDKResponse.WhichResponse += PayAfterInit;
                gameID = themsg.GameOrderId.ToString();
                SDK_IOS.ActivateInitialize();
            }
            else
            {
                Debug.Log("Calling pay in IOS SDK");
                SDK_IOS.ActivatePay(themsg.GameOrderId.ToString());
            }
        }
#endif
    }

    #endregion

    #region Mono

    // Use this for initialization
	void Start () 
    {
	    this.gameObject.SetActive(true);
        InstallHandlers();
	}
	
	// Update is called once per frame
	void Update () 
    {

    }

    void Awake()
    {
        payLis = UIEventListener.Get(transform.gameObject);
    }

    #endregion
}
