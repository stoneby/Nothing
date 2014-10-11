using KXSGCodec;
using UnityEngine;
using System.Collections;

public class SignBtnControl : MonoBehaviour
{
    private GameObject AlertIcon;

    private UIEventListener BtnUIEventListener;
	// Use this for initialization
	void Start ()
	{
	    AlertIcon = transform.FindChild("Sprite alert").gameObject;
        AlertIcon.SetActive(PlayerModelLocator.Instance.CanSign);
        BtnUIEventListener = UIEventListener.Get(gameObject);
        BtnUIEventListener.onClick += ClickHandler;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ClickHandler(GameObject obj)
    {
        AlertIcon.SetActive(false);
        var msg = new CSSignLoad();
        NetManager.SendMessage(msg);

        //WindowManager.Instance.Show<SignWindow>(true);
    }
}
