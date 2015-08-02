using System;
using System.IO;

namespace LogicSpawn.GTARPG.Core
{
    class RPGLog
    {
        public static void Log(string log)
        {
            if(string.IsNullOrEmpty(log))
                L("");
            else
                L("[INFO] " + log);
        }

        public static void LogError(Exception ex)
        {
            LogError(ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace);
        }
        public static void Log(Exception ex)
        {
            LogError(ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace);
        }

        public static void LogError(string error)
        {
            L("[Error] " + error);
        }

        private static void L(string s, bool raw = false)
        {
            var newDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(newDir, @"Rockstar Games\GTA V\RPGMod\");
            var logFile = "log";

            var logPath = Path.Combine(dir, logFile);

            Directory.CreateDirectory(dir);

            using (var stringwriter = new StreamWriter(logPath, true))
            {
                if(!raw)
                    stringwriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]" + s);
                else
                    stringwriter.Write(s);
            }
        }

        public static void LogRaw(string text)
        {
           L(text,true);
        }

        public static void Clear()
        {
            var newDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(newDir, @"Rockstar Games\GTA V\RPGMod\");
            var logFile = "log";

            var logPath = Path.Combine(dir, logFile);

            Directory.CreateDirectory(dir);

            File.WriteAllText(logPath,"");
        }
    }
}