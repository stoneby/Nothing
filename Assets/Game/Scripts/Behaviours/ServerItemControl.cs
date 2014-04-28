using UnityEngine;
using System.Collections;

public class ServerItemControl : MonoBehaviour
{
    //private GameObject BackBtn;
    private GameObject StateSprite;
    private GameObject BtnLabel;

    public ServerVO Data;

	// Use this for initialization
	void Start () 
    {
        //BackBtn = transform.FindChild("Image Button").gameObject;
        StateSprite = transform.FindChild("Sprite").gameObject;
        BtnLabel = transform.FindChild("Label").gameObject;

	    if (Data != null)
	    {
            Reset();
	    }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetData(ServerVO server)
    {
        Data = server;

        Reset();
    }

    private void Reset()
    {
        if (BtnLabel != null)
        {
            var lb = BtnLabel.GetComponent<UILabel>();
            lb.text = Data.ServerName;
        }

        if (StateSprite != null)
        {
            var sp = StateSprite.GetComponent<UISprite>();
            sp.spriteName = "state-" + Data.ServerState;
        }
    }

    private void OnClick()
    {
        if (ServiceManager.AccountData != null) ServiceManager.AccountData.AddServer(Data.Url);
        var obj = ServiceManager.GetDefaultAccount();
        if (obj != null)
        {
            obj.AddServer(Data.Url);
        }
        ServiceManager.ServerData = Data;
        //WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }
}
