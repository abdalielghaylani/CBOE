using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PerkinElmer.COE.Registration.Server.Code
{
    /// <summary>
    /// Reg server log class
    /// </summary>
    public static class Logger
    {
        public static string CallerMethodName
        {
            get
            {
                MethodBase method = new StackTrace().GetFrame(1).GetMethod();
                string moduleName = method.Module.Name.Contains(".") ?
                    method.Module.Name.Substring(0, method.Module.Name.LastIndexOf(".")) :
                    method.Module.Name;
                string className = method.DeclaringType.Name;
                string methodName = method.Name.Contains(".") ?
                    method.Name.Substring(method.Name.LastIndexOf(".") + 1) :
                    method.Name;
                return string.Format("{0}.{1}.{2}", moduleName, className, methodName);
            }
        }

        /// <summary>
        /// log error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="callerMethodName">the name of the method of the caller</param>
        public static void Error(string message, string callerMethodName = null)
        {
            if (callerMethodName == null) callerMethodName = CallerMethodName;
            LogManager.GetLogger("RegServer").Error(callerMethodName + " - " + message);
        }

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="message">info message</param>
        /// <param name="callerMethodName">caller Method Name</param>
        public static void Info(string message, string callerMethodName = null)
        {
            if (callerMethodName == null) callerMethodName = CallerMethodName;
            LogManager.GetLogger("RegServer").Info(callerMethodName + " - " + message);
        }

        /// <summary>
        /// Logs Debugging info
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="callerMethodName">caller method name</param>
        public static void Debug(string message, string callerMethodName = null)
        {
            if (callerMethodName == null) callerMethodName = CallerMethodName;
            LogManager.GetLogger("RegServer").Debug(callerMethodName + " - " + message);
        }

        /// <summary>
        /// log Fatal
        /// </summary>
        /// <param name="source">The error source</param>
        /// <param name="message">Fatal message</param>
        public static void Fatal(string source, string message)
        {
            LogManager.GetLogger("RegServer").Fatal(source + " - " + message);
        }
    }
}