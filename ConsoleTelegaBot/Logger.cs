using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSearchExample
{
    public static class Logger
    {
        static List<string> LogMsg = new List<string>();

        public static void Log(string _msg)
        {
            LogMsg.Add(_msg);
        }

        public static void IOLog(string _msg)
        {
            Console.WriteLine(_msg);
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        public static void SaveLog()
        {
            StringBuilder sb = new StringBuilder();
            if (LogMsg != null)
            {
                foreach (string anyMsg in LogMsg)
                {
                    sb.Append(anyMsg);
                    sb.Append(Environment.NewLine);
                }
                FileWorker.WriteToFile(sb.ToString(), "log.txt");
                LogMsg.Clear();
            }
        }

    }
}
