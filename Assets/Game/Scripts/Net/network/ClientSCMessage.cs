using KXSGCodec;

namespace Assets.Game.Scripts.Net.network
{
    class ClientSCMessage : ThriftSCMessage
    {
        public string Info;

        public ClientSCMessage(short type, string info)
            : base(type)
        {
            Info = info;
        }
    }
}
