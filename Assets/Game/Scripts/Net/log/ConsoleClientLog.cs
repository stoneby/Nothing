using System;
using System.Collections.Generic;
using System.Text;

namespace KXSGLog
{
    /// <summary>
    ///  console output log info
    /// </summary>
    public class ConsoleClientLog : ClientLog
    {
        public override void LogDebug(String info)
        {
            Console.WriteLine(info);
        }

        public override void LogInfo(String info)
        {
            Console.WriteLine(info);
        }

        public override void LogWarn(String info)
        {
            Console.WriteLine(info);
        }

        public override void LogError(String info)
        {
            Console.WriteLine(info);
        }
    }
}
