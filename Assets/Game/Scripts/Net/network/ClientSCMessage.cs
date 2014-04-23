using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KXSGCodec;
using Thrift.Protocol;

namespace Assets.Game.Scripts.Net.network
{
    class ClientSCMessage : ThriftSCMessage
    {
        public string Info;

        public ClientSCMessage(short type, string info) : base(type)
        {
            Info = info;
        }
    }
}
