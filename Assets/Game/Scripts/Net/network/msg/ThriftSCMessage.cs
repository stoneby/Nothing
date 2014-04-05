using System;
using System.Collections;
using System.Text;
using Thrift.Protocol;
using KXSGLog;

namespace KXSGCodec
{
    /// <summary>
    /// message received from server to client
    /// </summary>
    public class ThriftSCMessage : BaseMessage
    {
        // real msg content, exclude msg header
        private TBase content;
        private short msgType;

        public ThriftSCMessage(short msgType)
        {
            this.msgType = msgType;
            content = SCMessageHelper.createMessage(msgType);
        }

        public TBase getContent() 
        {
            return this.content;
        }

        protected override void ReadImpl()
        {
            try
            {
                int _contentLen = this.getMsgLength() - BaseMessage.MIN_MSG_LEN;
                if (_contentLen <= 0)
                {
                    return;
                }

                byte[] _contentBytes = this.ReadBytes(_contentLen);
                ThriftMsgSerialize.DeSerialize(content, _contentBytes);
            }
            catch (Exception ex)
            {
                this.content = null;
                ClientLog.Instance.LogError(ex.ToString());
            }
        }

        public override short GetMsgType()
        {
            return this.msgType;
        }

        protected override void WriteImpl() {
            throw new UnauthorizedAccessException();
        }

        protected override int GetWritePos()
        {
            throw new UnauthorizedAccessException();
        }

        public override string ToString()
        {
            return content == null ? "null" : content.ToString();
        }
    }
}