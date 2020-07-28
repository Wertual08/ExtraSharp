using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtraSharp
{
    public static class Logger
    {
        public static string LogFile = "LogFile.txt";
        public static void Message(string message)
        {
            StreamWriter Writer = File.AppendText(LogFile);
            Writer.WriteLine(message);
            Writer.Close();
        }
        public static void TimedMessage(string message)
        {
            StreamWriter Writer = File.AppendText(LogFile);
            Writer.WriteLine("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + message);
            Writer.Close();
        }
    }
}
