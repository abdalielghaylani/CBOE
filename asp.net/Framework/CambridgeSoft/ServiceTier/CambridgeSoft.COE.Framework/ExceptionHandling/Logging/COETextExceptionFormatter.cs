using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using CambridgeSoft.COE.Framework.Properties;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    public class COETextExceptionFormatter : TextExceptionFormatter
    {
        private int innerDepth = 0;
        private int separatorLength = 0;
        private static readonly List<string> IgnoredProperties = new List<string>(
            new String[] { "Data", "HelpLink", "InnerException", "Message", "Source", "StackTrace", "TargetSite" });

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="COETextExceptionFormatter"/> using the specified
        /// <see cref="TextWriter"/> and <see cref="Exception"/>
        /// objects.
        /// </summary>
        /// <param name="writer">The stream to write formatting information to.</param>
        /// <param name="exception">The exception to format.</param>
        public COETextExceptionFormatter(TextWriter writer, Exception exception)
            : base(writer, exception)
        { }

        /// <summary>
        /// Writes a description of the caught exception, it's the first part in the log file.
        /// </summary>
        protected override void WriteDescription()
        {
            Writer.WriteLine(Resources.ExceptionLogging_DescriptionToken);
            Writer.WriteLine();

            innerDepth++;

            IndentAndWriteLine(string.Format(Resources.ExceptionLogging_TimestampFormat, DateTime.Now.ToString()));

            string line = string.Format(Resources.Culture, Resources.ExceptionLogging_ExceptionWasCaught, base.Exception.GetType().FullName);
            separatorLength = line.Length;
            IndentAndWriteLine(line);

            innerDepth--;
            WriteSeparator();
        }

        /// <summary>
        /// Writes the current time, and it's the second part in the log file
        /// </summary>
        /// <param name="utcNow">The current time.</param>
        protected override void WriteDateTime(DateTime utcNow)
        {
            // Do nothing here to not add date/time at this place
        }

        /// <summary>
        /// Formats the exception and all nested inner exceptions. And it's the third part in the log file.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format</param>
        /// <param name="outerException">The outer exception. This 
        /// value will be null when writing the outer-most exception.</param>
        protected override void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            IndentAndWriteLine(Resources.ExceptionLogging_ExceptionInfoToken);
            Writer.WriteLine();

            innerDepth++;

            WriteExceptionInternal(exceptionToFormat, outerException, true);

            WriteSeparator();
        }

        protected override void WriteMessage(string message)
        {
            // If the exception is DataPortalException, we want to shorten its Message value since
            // it contains the stack trace data by default.
            if (Exception.InnerException is Csla.DataPortalException &&
                message.IndexOf("--->") != -1)
                message = message.Substring(0, message.IndexOf("--->"));

            base.WriteMessage(message);
        }

        protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            // If the exception is DataPortalException, we want to shorten its Message value since
            // it contains the stack trace data by default.
            if (Exception.InnerException is Csla.DataPortalException &&
                value != null &&
                value.ToString().IndexOf("--->") != -1)
                value = value.ToString().Substring(0, value.ToString().IndexOf("--->"));

            base.WritePropertyInfo(propertyInfo, value);
        }

        /// <summary>
        /// Indent line accroding to the innerDepth
        /// </summary>
        protected override void Indent()
        {
            for (int i = 0; i < innerDepth; i++)
            {
                this.Writer.Write("\t");
            }
        }

        /// <summary>
        /// Indent and write the format string to the log file.
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="arg">The params to format</param>
        private void IndentAndWriteLine(string format, params object[] arg)
        {
            this.Indent();
            this.Writer.WriteLine(format, arg);
        }

        /// <summary>
        /// A recursive method, called by WriteException.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format</param>
        /// <param name="outerException">The outer exception. This 
        /// value will be null when writing the outer-most exception.</param>
        /// <param name="writeStackTrace">Whether or not to write call stack information.
        /// True for outermost exception only.</param>
        private void WriteExceptionInternal(Exception exceptionToFormat, Exception outerException, bool writeStackTrace)
        {
            if (exceptionToFormat == null) throw new ArgumentNullException("exceptionToFormat");

            this.WriteExceptionType(exceptionToFormat.GetType());
            this.WriteMessage(exceptionToFormat.Message);
            this.WriteSource(exceptionToFormat.Source);

            if (writeStackTrace)
                this.WriteStackTrace(exceptionToFormat.StackTrace);

            this.WriteCustomProperty(exceptionToFormat);

            Exception inner = exceptionToFormat.InnerException;

            if (inner != null)
            {
                IndentAndWriteLine(Resources.ExceptionLogging_InnerExceptionToken);
                innerDepth++;
                // recursive call
                this.WriteExceptionInternal(inner, exceptionToFormat, false);
            }
        }

        /// <summary>
        /// Writes the extra info to the log file, and it's part of the exception format
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format</param>
        private void WriteCustomProperty(Exception exceptionToFormat)
        {
            IndentAndWriteLine(Resources.ExceptionLogging_CustomPropertyToken);

            innerDepth++;

            WriteCustomPropertyInternal(exceptionToFormat);

            innerDepth--;
        }

        /// <summary>
        /// The concretely implementation method to write extra information.
        /// </summary>
        /// <param name="exceptionToFormat">The exception to format</param>
        private void WriteCustomPropertyInternal(Exception exceptionToFormat)
        {
            if (exceptionToFormat == null) throw new ArgumentNullException("exceptionToFormat");

            Type type = exceptionToFormat.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            object value;

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && IgnoredProperties.IndexOf(property.Name) == -1)
                {
                    value = property.GetValue(exceptionToFormat, null);
                    WritePropertyInfo(property, value);
                }
            }
        }

        /// <summary>
        /// Writes the sepatator between different parts.
        /// </summary>
        private void WriteSeparator()
        {
            this.Writer.WriteLine(new string('-', separatorLength));
        }
    }
}