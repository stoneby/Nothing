using System;
using System.Collections.Generic;
using System.Text;

namespace KXSGLog
{
    /// <summary>
    /// client log util, extend this class to implement yourself function
    /// </summary>
    public abstract class ClientLog
    {
        // modify the value to construct the log util you need
        public static byte logType = 0;
        public static ClientLog Instance;   

        static ClientLog()
        {
            switch (logType)
            {
                case 0: {
                    Instance = new ConsoleClientLog();
                    break;
                }
                case 1: {
                    //Instance = new TextboxClientLog();
                    break;
                }

            default:
                {
                    Instance = new ConsoleClientLog();
                    break;
                }
            }
           
        }

        public abstract void LogDebug(String info);

        public abstract void LogInfo(String info);

        public abstract void LogWarn(String info);

        public abstract void LogError(String info);

    }
}
