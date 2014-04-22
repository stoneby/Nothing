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
                    case (short)MessageType.SC_SYSTEM_INFO_MSG:
                        var sysmsg = msg.getContent() as SCSystemInfoMsg;
                        if (sysmsg != null)
                        {
                            Debug.Log("服务端系统消息：" + sysmsg.Info);
                            PopTextManager.PopTip(sysmsg.Info);
                        }
                        break;
                    case (short)MessageType.SC_CREATE_PLAYER_MSG:
                        
                        Debug.Log("MessageType.SC_CREATE_PLAYER");
                        var csMsg = new CSCreatePlayerMsg();
                        var act = ServiceManager.AccountData ?? ServiceManager.GetDefaultAccount();
                        csMsg.Name = act.Account + act.Account;
                        PopTextManager.PopTip("登录成功，正在创建角色(" + csMsg.Name + ")");
                        NetManager.SendMessage(csMsg);
                        break;
                    case (short)MessageType.SC_PLAYER_INFO_MSG:
                        PopTextManager.PopTip("登录成功，返回玩家角色信息");
                        EventManager.Instance.Post(new LoginEvent() { Message = "This is login event." });
                        WindowManager.Instance.Show(typeof(MainMenuWindow), true);
                        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
                        WindowManager.Instance.Show(WindowGroupType.Popup, false);
                        break;

                }
                msg = NetManager.GetMessage();
            }

            yield return new WaitForSeconds(0.5f);
        }
        

    }
}
