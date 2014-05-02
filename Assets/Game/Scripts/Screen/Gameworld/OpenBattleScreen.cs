using KXSGCodec;
using UnityEngine;

public class OpenBattleScreen : MonoBehaviour
{
    void OnClick()
    {
        //ScreenManager.CurrentScreen = ScreenType.Battle;

        var csMsg = new CSRaidBattleStartMsg();
        csMsg.RaidId = 1;
        csMsg.FriendId = 1;
        NetManager.SendMessage(csMsg);
        
        
    }
}
