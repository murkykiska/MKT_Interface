namespace Logger;

internal interface ILogger
{
   void DebugLog(string toLog);
   void ErrorLog(string toLog);
   string LogFilePath { get; init; }
}
