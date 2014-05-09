using System;
using Thrift.Protocol;

namespace KXSGCodec
{
    /// <summary>
    /// message sended from client to server
    /// </summary>
    public class ThriftCSMessage : BaseMessage
    {
        // real msg content, exclude msg header
        private readonly TBase content;

        public ThriftCSMessage(TBase content)
        {
            this.content = content;
        }

        protected override void WriteImpl()
        {
            try
            {
                var contentBytes = ThriftMsgSerialize.Serialize(content);
                if (contentBytes != null)
                {
                    WriteBytes(contentBytes);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
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
