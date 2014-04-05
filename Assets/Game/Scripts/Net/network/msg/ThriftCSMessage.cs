using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using KXSGLog;

namespace KXSGCodec
{
    /// <summary>
    /// message sended from client to server
    /// </summary>
    public class ThriftCSMessage : BaseMessage
    {
        // real msg content, exclude msg header
        private TBase content;

        public ThriftCSMessage(TBase content)
        {
            this.content = content;
        }

        protected override void WriteImpl()
        {
            try
            {
                byte[] _contentBytes = ThriftMsgSerialize.Serialize(content);
                if (_contentBytes != null)
                {
                    WriteBytes(_contentBytes);
                }
            }
            catch (Exception ex)
            {
                ClientLog.Instance.LogError(ex.ToString());
            }
        }

        public override short GetMsgType()
        {
            return CSMessageHelper.getMessageType(content.GetType());
        }

        protected override void ReadImpl()
        {
            throw new UnauthorizedAccessException();
        }

        protected override int GetReadPos()
        {
            throw new UnauthorizedAccessException();
        }
    }
}
