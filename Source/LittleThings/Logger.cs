using System;
using System.IO;

namespace LittleThings
{
    public class Logger
    {
        static string filePath = $"{LittleThings.ModDirectory}/LittleThings.log";
        public static void LogError(Exception ex)
        {
            if (LittleThings.DebugLevel >= 1)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    var prefix = "[LittleThings @ " + DateTime.Now.ToString() + "]";
                    writer.WriteLine("Message: " + ex.Message + "<br/>" + Environment.NewLine + "StackTrace: " + ex.StackTrace + "" + Environment.NewLine);
                    writer.WriteLine("----------------------------------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        public static void LogLine(String line, bool showPrefix = true)
        {
            if (LittleThings.DebugLevel >= 2)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    string prefix = "";
                    if (showPrefix)
                    {
                        prefix = "[LittleThings @ " + DateTime.Now.ToString() + "]";
                    }
                    writer.WriteLine(prefix + line);
                }
            }
        }
    }
}
