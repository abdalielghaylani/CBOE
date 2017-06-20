using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using log4net;

namespace PerkinElmer.COE.Registration.Server.Code
{
    /// <summary>
    /// Registration server loggomg utility class
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

        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// log error
        /// </summary>
        /// <param name="exception">error</param>
        /// <param name="callerMethodName">the name of the method of the caller</param>
        public static void Error(Exception exception, string callerMethodName = null)
        {
            if (callerMethodName == null) callerMethodName = CallerMethodName;
            LogManager.GetLogger("RegServer").Error(callerMethodName + " - " + FlattenException(exception));
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