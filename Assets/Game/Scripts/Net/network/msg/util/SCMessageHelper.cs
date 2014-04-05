using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift.Protocol;
using KXSGLog;

namespace KXSGCodec
{
    /// <summary>
    /// sc message type map util
    /// </summary>
    public class SCMessageHelper
    {
        public static TBase createMessage(short type)
        {
            if (type == MessageType.SC_SYSTEM_INFO.GetHashCode())
            {
                return new SCSystemInfoMsg();
            } else  if (type == MessageType.SC_SYSTEM_INFO.GetHashCode()) {

                    return new SCErrorInfoMsg();
            } else {
                    ClientLog.Instance.LogError("Unknown sc msg type: " + type.ToString());
	                return null;
            }
        }
    }
}
