using KXSGCodec;
using Thrift.Protocol;
using UnityEngine;
using System.Collections;

public class ShowImg : MonoBehaviour {
	public GameObject ShowTexture;
    public GameObject BgTexture;

	int currentIndex = 0;
	// Use this for initialization
	void Start () {
//        ShowTexture.SetActive(false);
//        var sp = BgTexture.GetComponent<UISprite>();
//	    sp.alpha = 0.1f;
	}
	
	// Update is called once per frame
	void Update ()
	{
        var msg = NetManager.GetMessage() as ThriftSCMessage;
	    if (msg != null)
	    {
            Debug.Log(msg);
	    }
	}

	void OnClick()
	{
        Debug.Log("on click");
        CSPasswdLoginMsg csMsg = new CSPasswdLoginMsg();
        csMsg.DeviceId = "1";
        csMsg.DeviceType = 2;
        csMsg.Passwd = "123456";
        csMsg.AccountName = "test";
        NetManager.SendMessage(csMsg);

		//StartCoroutine (DoSlidePicture());
	}

	IEnumerator DoSlidePicture()
	{
		if (currentIndex != 0)
		{
            playTweenPosition(ShowTexture, 0.3f, new Vector3(0, 0, 0), new Vector3(-960, 0, 0));
			yield return new WaitForSeconds(0.3f);
		}
		currentIndex = (currentIndex + 1) % 3;
		if (currentIndex != 0)
		{
            UITexture tt = ShowTexture.GetComponent<UITexture>();
			tt.mainTexture = (Texture2D)Resources.Load("textures/Design/eff_" + currentIndex.ToString(), typeof(Texture2D));
            ShowTexture.SetActive(true);
            ShowTexture.transform.localPosition = new Vector3(960, 0, 0);
            playTweenPosition(ShowTexture, 0.3f, new Vector3(960, 0, 0), new Vector3(0, 0, 0));
		}
		else
		{
            ShowTexture.SetActive(false);
		}
	}

	void playTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
	{
		TweenPosition ts = obj.AddComponent<TweenPosition>();
		ts.from = from;
		ts.to = to;
		ts.duration = playtime;
		ts.PlayForward ();
		Destroy (ts, playtime);
	}
}
