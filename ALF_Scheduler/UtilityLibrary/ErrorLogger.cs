using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler.Utilities
{
    public static class ErrorLogger
    {

        private static bool _isInstantiated = false;
        
        private static void InstantiateErrorLogger()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("/Debug/ErrorLog.log"));
            Trace.AutoFlush = true;
            _isInstantiated = true;
        }

        /// <summary>
        /// This is a wrapper method for logging to the error file ErrorLog.log
        /// </summary>
        /// <param name="message">A string message to be added into the log.</param>
        /// <param name="ex">Optional exception parameter.</param>
        public static void LogInfo(string message, Exception ex = null)
        {
            if (!_isInstantiated)
            {
                InstantiateErrorLogger();
            }
            CallerFilePathAttribute fileLocation = new CallerFilePathAttribute();
            CallerLineNumberAttribute lineNumber = new CallerLineNumberAttribute();
            CallerMemberNameAttribute methodName = new CallerMemberNameAttribute();
            string output = $"File Path : {fileLocation}" + Environment.NewLine;
            output += $"Method Name : {methodName}" + Environment.NewLine;
            output += $"Line: {lineNumber} + {Environment.NewLine}";
            Trace.WriteLine(output + Environment.NewLine + 
                            ex?.Message + ex?.StackTrace);
        }
    }
}