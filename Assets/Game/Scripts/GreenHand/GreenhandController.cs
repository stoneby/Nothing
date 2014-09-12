using KXSGCodec;
using UnityEngine;

public class GreenhandController : Singleton<GreenhandController>
{
    public void SendStartMessage()
    {
        var message = new CSGreenhandBattleStartMsg();
        Debug.Log("Send greenhandBattleStart msg to server.");
        NetManager.SendMessage(message);
    }

    public void SendEndMessage()
    {
        var message = new CSGreenhandBattleEndMsg();
        Debug.Log("Send greenhandBattleEnd msg to server.");
        NetManager.SendMessage(message);
    }
}