using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using KXSGLog;

namespace KXSGCodec
{
    /// <summary>
    /// Server to client message recognizor
    /// </summary>
    class SCMessageRecognizor
    {
        private const int MSG_LEN_HIGH = 0;
        private const int MSG_LEN_LOW = 1;
        private const int MSG_TYPE_HIGH = 2;
        private const int MSG_TYPE_LOW = 3;

        /// <summary>
        /// analyze msg length from msg data
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
	    public short RecognizeMsgLen(byte[] bytes) {
		    if (bytes.Length < BaseMessage.MIN_MSG_LEN) {
			    return -1;
		    }

            byte _high = bytes[MSG_LEN_HIGH];
            byte _low = bytes[MSG_LEN_LOW];
            return (short)(_high << 8 | _low);
	    }

        /// <summary>
        /// analyze msg type from msg data
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
         public short RecognizeMsgType(byte[] bytes) {
		    // 
		    if (bytes.Length < BaseMessage.MIN_MSG_LEN) {
			    return -1;
		    }

            byte _high = bytes[MSG_TYPE_HIGH];
            byte _low = bytes[MSG_TYPE_LOW];
            return (short)(_high << 8 | _low);
	    }

        /// <summary>
        /// analyze original message object without real content
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public ThriftSCMessage RecognizeMsg(byte[] bytes)
        {
		    if (bytes.Length < BaseMessage.MIN_MSG_LEN) {
			    return null;
		    }
		    //
		    try {
			    short _len = RecognizeMsgLen(bytes);
                if (_len > bytes.Length || _len < 0)
                {
                    return null;
                }

			    short _type = RecognizeMsgType(bytes);
                ClientLog.Instance.LogWarn("msg type : " + _type.ToString());
			    return CreateMessage(_type);
		    } catch (Exception le) {
                ClientLog.Instance.LogError(le.ToString());
                return null;
		    }
	    }

        /// <summary>
        /// create thrift message by msg type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ThriftSCMessage CreateMessage(short type) 
		{
            KXSGCodec.ThriftSCMessage msg = new KXSGCodec.ThriftSCMessage(type);
            TBase msgContent = msg.getContent();
            if (msgContent == null)
            {
                return null;
            }

            return msg;
	    }
    }
}
