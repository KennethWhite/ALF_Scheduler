using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler.Utilities
{
    public class ErrorLogger
    {
        public ErrorLogger()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("/Debug/ErrorLog.log"));
            Trace.AutoFlush = true;
        }

        public void LogInfo(string message, Exception ex = null)
        {
            CallerFilePathAttribute fileLocation;
            CallerLineNumberAttribute lineNumber;
            CallerMemberNameAttribute methodName;
            string output = $"File Path : {fileLocation}" + Environment.NewLine;
            //TODO
            Trace.WriteLine(,ex?.Message, ex?.StackTrace);
        }


    }
}
