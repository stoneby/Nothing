using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NetworkControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(MessageHandler());
	}

    private IEnumerator MessageHandler()
    {
        while (true)
        {
            var msg = NetManager.GetMessage();
            while (msg != null)
            {
                Debug.Log(msg.GetMsgType());
                switch (msg.GetMsgType())
                {
                    case (short)MessageType.SC_CREATE_PLAYER:
                        Debug.Log("MessageType.SC_CREATE_PLAYER");
                        var csMsg = new CSCreatePlayerMsg();
                        csMsg.Name = ServiceManager.AccountData.Account + ServiceManager.AccountData.Account;
                        NetManager.SendMessage(csMsg);
                        break;
                    case (short)MessageType.SC_PLAYER_INFO:
                        EventManager.Instance.Post(new LoginEvent() { Message = "This is login event." });
                        WindowManager.Instance.Show(typeof(MainMenuWindow), true);
                        WindowManager.Instance.Show(WindowGroupType.Popup, false);
                        break;
                }
                msg = NetManager.GetMessage();
            }

            yield return new WaitForSeconds(0.5f);
        }
        

    }
}
