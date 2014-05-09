using Assets.Game.Scripts.Net.network;
using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    class SystemHandler
    {
        public static void OnSystemInfo(ThriftSCMessage msg)
        {
            var sysmsg = msg.GetContent() as SCSystemInfoMsg;
            if (sysmsg != null && sysmsg.Info != null && sysmsg.Info != "")
            {
                Logger.Log("服务端系统消息1：" + sysmsg.Info);
                PopTextManager.PopTip(sysmsg.Info);
            }
            else
            {
                var clientmsg = msg as ClientSCMessage;
                Logger.Log("服务端系统消息2：" + clientmsg.Info);
                PopTextManager.PopTip(clientmsg.Info);
            }
        }

    }
}
