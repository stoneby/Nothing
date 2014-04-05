using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KXSGCodec
{
    /// <summary>
    /// cs message type map util
    /// </summary>
    public class CSMessageHelper
    {
        private static Dictionary<Type, short> MSG_TYPE_DICT = new Dictionary<Type,short>();
        private static CSMessageHelper INSTANCE = new CSMessageHelper();

        private  CSMessageHelper() 
        {
            MSG_TYPE_DICT[typeof(CSPasswdLoginMsg)] = (short)MessageType.CS_USER_PASSWD.GetHashCode();
            MSG_TYPE_DICT[typeof(CSQuickLoginMsg)] = (short)MessageType.CS_QUICK_LOGIN.GetHashCode();
				
        }

        public static short getMessageType(Type msg)
        {
            return MSG_TYPE_DICT[msg];
        }
    }
}
