using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ALF_Scheduler.Utilities
{
    public static class ErrorLogger
    {
        private static bool _isInstantiated;

        private static void InstantiateErrorLogger()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("/Debug/ErrorLog.log"));
            Trace.AutoFlush = true;
            _isInstantiated = true;
        }

        /// <summary>
        ///     This is a wrapper method for logging to the error file ErrorLog.log
        /// </summary>
        /// <param name="message">A string message to be added into the log.</param>
        /// <param name="ex">Optional exception parameter.</param>
        public static void LogInfo(string message, Exception ex = null)
        {
            if (!_isInstantiated) InstantiateErrorLogger();
            var fileLocation = new CallerFilePathAttribute();
            var lineNumber = new CallerLineNumberAttribute();
            var methodName = new CallerMemberNameAttribute();
            var output = $"File Path : {fileLocation}" + Environment.NewLine;
            output += $"Method Name : {methodName}" + Environment.NewLine;
            output += $"Line: {lineNumber} + {Environment.NewLine}";
            Trace.WriteLine(output + Environment.NewLine +
                            ex?.Message + ex?.StackTrace);
        }
    }
}