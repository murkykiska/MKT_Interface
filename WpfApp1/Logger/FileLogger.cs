using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Logger;

public class FileLogger : ILogger
{
   public string LogFilePath { get; init; } = @"./time.log";

   private static FileLogger logger = null;
   public static FileLogger Logger => logger ??= new FileLogger();
   private FileLogger() 
   {
      File.Open(LogFilePath, FileMode.Create).Close();
   }
   public void DebugLog(string toLog)
   {
      string time = DateTime.Now.ToString("hh:mm:ss:fff");
      string input = "\n" + time + ": " + toLog;
      using var stream = File.AppendText(path: LogFilePath);
      
      stream.Write(input);
   }

   public void ErrorLog(string toLog)
   {
      string time = DateTime.Now.ToString("hh:mm:ss:fff");
      string input = "\n<ERROR>:" + time + ": " + toLog;
      using var stream = File.AppendText(path: LogFilePath);

      stream.Write(input);
   }
}
