using System;
using Thrift.Protocol;

namespace KXSGCodec
{
    /// <summary>
    /// message received from server to client
    /// </summary>
    public class ThriftSCMessage : BaseMessage
    {
        // real msg content, exclude msg header
        private TBase content;
        private readonly short msgType;

        public ThriftSCMessage(short msgType)
        {
            this.msgType = msgType;
            content = SCMessageHelper.createMessage(msgType);
        }

        public TBase GetContent()
        {
            return content;
        }

        protected override void ReadImpl()
        {
            try
            {
                var contentLen = GetMsgLength() - MinMsgLen;
                if (contentLen <= 0)
                {
                    return;
                }

                var contentBytes = ReadBytes(contentLen);
                ThriftMsgSerialize.DeSerialize(content, contentBytes);
            }
            catch (Exception ex)
            {
                content = null;
                Logger.LogError(ex.ToString());
            }
        }

        public override short GetMsgType()
        {
            return msgType;
        }

        protected override void WriteImpl()
        {
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