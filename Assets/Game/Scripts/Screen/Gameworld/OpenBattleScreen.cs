using KXSGCodec;
using UnityEngine;

public class OpenBattleScreen : MonoBehaviour
{
    void OnClick()
    {
        //if (GameConfiguration.Instance.SingleMode)
        //{
        //    var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
        //    WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);

        //    Debug.Log(window);
        //    var battlemanager = window.GetComponent<InitBattleField>();
        //    battlemanager.StartBattle();
        //}
        //else
        //{
        //    var csMsg = new CSRaidBattleStartMsg();
        //    csMsg.RaidId = 1;
        //    csMsg.FriendId = 1;
        //    NetManager.SendMessage(csMsg);
        //}
        var csMsg = new CSRaidBattleStartMsg();
        csMsg.RaidId = 1;
        csMsg.FriendId = 1;
        NetManager.SendMessage(csMsg);
    }
}
