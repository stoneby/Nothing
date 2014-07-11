using UnityEngine;

public class SDKPayManager : MonoBehaviour
{
    #region Private Fields

    private UIEventListener payLis;

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

    #endregion

    private void OnPay(GameObject go)
    {
        //Send message to server.
        Debug.Log("Sending message to server.");
        PayInSDK("aaaaa");
    }

    #region Public Methods

    public void PayInSDK(string remark)
    {
        if (SDKLoginManager.isInitialized == true)
        {
#if UNITY_IPHONE
            Debug.Log("Calling pay in SDK");
            SDK_IOS.ActivatePay(remark);
#endif
        }
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
