using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class SystemHandler
    {
        public static void OnSystemInfo(ThriftSCMessage msg)
        {
            var sysmsg = msg.getContent() as SCSystemInfoMsg;
            if (sysmsg != null && sysmsg.Info != null && sysmsg.Info != "")
            {
                Debug.Log("服务端系统消息1：" + sysmsg.Info);
                PopTextManager.PopTip(sysmsg.Info);
            }
            else
            {
                var clientmsg = msg as ClientSCMessage;
                Debug.Log("服务端系统消息2：" + clientmsg.Info);
                PopTextManager.PopTip(clientmsg.Info);
            }
        }

    }
}
