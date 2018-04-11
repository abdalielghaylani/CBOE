// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHost.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.ExternalProcess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// An external process that receives commands via stdin and sends data back via stdout/stderr.
    /// </summary>
    internal abstract class ProcessHost : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The preamble that all external processes must submit before all communcation.
        /// </summary>
        /// <remarks>The only reason for the preable is that if the other process might write to StdOut/StdErr
        /// and whatever it writes must not be mistaken for information that we want to consume.</remarks>
        public const string CommuncationPreamble = "Spotfire.ProcessCommunication ";

        /// <summary>
        /// The wait handle for errors.
        /// </summary>
        private EventWaitHandle errorReceivedHandle;

        /// <summary>
        /// The wait handle for output.
        /// </summary>
        private EventWaitHandle outputReceivedHandle;

        /// <summary>
        /// The external process.
        /// </summary>
        private Process process;

        #endregion

        #region Properties

        /// <summary>
        /// Gets Arguments.
        /// </summary>
        protected abstract string Arguments { get; }

        /// <summary>
        /// Gets ExecutableName.
        /// </summary>
        protected abstract string ExecutableName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2213:DisposableFieldsShouldBeDisposed",
            MessageId = "errorReceivedHandle",
            Justification = "[fblom, 2011-10-05]: The class does not own the handle, it just interacts with it.")]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2213:DisposableFieldsShouldBeDisposed", 
            MessageId = "outputReceivedHandle", 
            Justification = "[fblom, 2011-10-05]: The class does not own the handle, it just interacts with it.")]
        public void Dispose()
        {
            this.DisposeProcess();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The result of the command.
        /// </returns>
        public IList<string> SendCommand(string command, params object[] parameters)
        {
            string call = GetEncodedCommandCall(command, parameters);
            string errorText;
            string outputText;

            using (
                AsyncProcessReader asyncProcessReader = new AsyncProcessReader(
                    this.process, this.errorReceivedHandle, this.outputReceivedHandle))
            {
                this.process.StandardInput.WriteLine(call);

                errorText = string.Empty;
                outputText = string.Empty;

                // Wait until we have data on both the Error and the Output lines...
                // To make sure that this works on both STA and MTA we will wait for each one in turn...
                // And pump messages while we wait so Spotfire remains responsive and keeps painting
                while (true)
                {
                    Application.DoEvents();

                    // Wait for the data
                    if (this.outputReceivedHandle.WaitOne(100))
                    {
                        outputText = asyncProcessReader.OutputText;
                    }

                    // Wait for the error data
                    if (this.errorReceivedHandle.WaitOne(100))
                    {
                        errorText = asyncProcessReader.ErrorText;
                    }

                    //LD-794,LD-911,LD-912
                    //LD-1234
                    if (existProcess() && string.IsNullOrEmpty(outputText) && string.IsNullOrEmpty(errorText))
                    {
                        outputText = "Q2FuY2Vs\r\n";
                        break;
                    }
                    //END

                    // We can't exit until we have both the output and error data and we
                    // have to hope something doesn't go horribly wrong and one never comes
                    // This whole IPC mechanism is not good but we're stuck with it at least for now
                    if (!string.IsNullOrEmpty(outputText) && !string.IsNullOrEmpty(errorText))
                    {
                        break;
                    }
                }
            }

            // This method will throw if there was an error on the other side.
            ProcessError(errorText);

            string[] encodedValues = outputText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> values = new List<string>();
            foreach (string encodedValue in encodedValues)
            {
                string value = FromBase64(encodedValue);
                values.Add(value);
            }

            return values;
        }

        /// <summary>
        /// Starts the external process.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process was started, otherwise <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes", 
            Justification = "[fblom, 2011-10-05]: We want to catch all exceptions that occur as a result of starting the external process.")]
        public bool Start()
        {
            this.DisposeProcess();

            string externalProcess = this.ExecutableName;
            string arguments = this.Arguments;
            var processStartInfo = new ProcessStartInfo(externalProcess, arguments);
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            try
            {
                this.process = Process.Start(processStartInfo);
                this.process.EnableRaisingEvents = true;
                this.errorReceivedHandle = new AutoResetEvent(false);
                this.outputReceivedHandle = new AutoResetEvent(false);
                return !this.process.HasExited;
            }
            catch (Exception)
            {
            }

            return false;
        }

        #endregion

        #region Methods

        //LD-794,LD-911,LD-912
        //LD-1234
        /// <summary>
        /// Check the processor is exist or not.
        /// </summary>
        /// <returns>
        /// Ture or false
        /// </returns>
        private bool existProcess()
        {
            return !this.process.Responding || this.process.HasExited;
        }
        //END

        /// <summary>
        /// Converts a string from its base64 representation.
        /// </summary>
        /// <param name="value">
        /// The base64 encoded value.
        /// </param>
        /// <returns>
        /// The decoded value.
        /// </returns>
        private static string FromBase64(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary>
        /// Gets the encoded command call.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The fully encoded text command call.
        /// </returns>
        private static string GetEncodedCommandCall(string command, object[] parameters)
        {
            List<string> commands = new List<string>(parameters.Length + 1);

            // The actual command is not encoded.
            commands.Add(command);
            foreach (object parameter in parameters)
            {
                string encodedValue = ToBase64(parameter);
                commands.Add(encodedValue);
            }

            string call = string.Join(" ", commands.ToArray());
            return call;
        }

        /// <summary>
        /// Processes the error.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
        /// <remarks>
        /// This method will throw an error if the text is something other than <c>string.Empty</c>.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", 
            Justification = "[fblom, 2011-10-05]: The exception is carried over from the hosted process.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", 
            Justification = "[fblom, 2011-10-05]: The method will always throw an error in this case.")]
        private static void ProcessError(string errorText)
        {
            if (!string.IsNullOrEmpty(errorText))
            {
                errorText = errorText.Trim();
            }

            if (!string.IsNullOrEmpty(errorText))
            {
                // The Marvin editor requires JRE 7 (64-bit) or higher, we get this error
                // if an earlier version is installed
                if (errorText.StartsWith("java.lang.UnsupportedClassVersionError"))
                {
                    throw new Exception(Properties.Resources.IncorrectJREVersion);
                }

                string convertedText = errorText;
                try
                {
                    convertedText = FromBase64(errorText);
                }
                catch
                {
                }

                throw new Exception(convertedText);
            }
        }

        /// <summary>
        /// Converts the value to its base64 encoded string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The encoded value.
        /// </returns>
        private static string ToBase64(object value)
        {
            string valueAsString = value.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(valueAsString);
            string base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        /// <summary>
        /// Disposes the process.
        /// </summary>
        private void DisposeProcess()
        {
            if (this.process != null)
            {
                this.process.Dispose();
                this.process = null;
            }
        }

        #endregion

        /// <summary>
        /// An asynchronous reader for the external process.
        /// </summary>
        private class AsyncProcessReader : IDisposable
        {
            #region Constants and Fields

            /// <summary>
            /// The handle to signal when a text on StdErr has been received.
            /// </summary>
            private readonly EventWaitHandle errorReceivedHandle;

            /// <summary>
            /// The StringBuilder for error texts.
            /// </summary>
            private readonly StringBuilder errorText;

            /// <summary>
            /// The handle to signal when a text on StdOut has been received.
            /// </summary>
            private readonly EventWaitHandle outputReceivedHandle;

            /// <summary>
            /// The StringBuilder for output texts.
            /// </summary>
            private readonly StringBuilder outputText;

            /// <summary>
            /// The external process.
            /// </summary>
            private readonly Process process;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncProcessReader"/> class.
            /// </summary>
            /// <param name="process">
            /// The process.
            /// </param>
            /// <param name="errorReceivedHandle">
            /// The handle to signal when a text on StdErr has been received.
            /// </param>
            /// <param name="outputReceivedHandle">
            /// The handle to signal when a text on StdOut has been received.
            /// </param>
            public AsyncProcessReader(
                Process process, EventWaitHandle errorReceivedHandle, EventWaitHandle outputReceivedHandle)
            {
                this.process = process;

                this.outputText = new StringBuilder();
                this.errorText = new StringBuilder();

                this.errorReceivedHandle = errorReceivedHandle;
                this.outputReceivedHandle = outputReceivedHandle;

                process.ErrorDataReceived += this.ProcessErrorDataReceived;
                process.OutputDataReceived += this.ProcessOutputDataReceived;

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the error text.
            /// </summary>
            /// <value>The error text.</value>
            public string ErrorText
            {
                get
                {
                    return this.errorText.ToString();
                }
            }

            /// <summary>
            /// Gets the output text.
            /// </summary>
            /// <value>The output text.</value>
            public string OutputText
            {
                get
                {
                    return this.outputText.ToString();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.process.ErrorDataReceived -= this.ProcessErrorDataReceived;
                this.process.OutputDataReceived -= this.ProcessOutputDataReceived;

                this.process.CancelErrorRead();
                this.process.CancelOutputRead();
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the ErrorDataReceived event of the process control.
            /// </summary>
            /// <param name="sender">
            /// The source of the event.
            /// </param>
            /// <param name="e">
            /// The <see cref="System.Diagnostics.DataReceivedEventArgs"/> instance containing the event data.
            /// </param>
            private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    string value = e.Data.TrimStart();

                    // The Marvin editor requires JRE 7 (64-bit) or higher, we get this error
                    // if an earlier version is installed
                    if (value.StartsWith("java.lang.UnsupportedClassVersionError"))
                    {
                        this.errorText.AppendLine(value);
                        this.errorReceivedHandle.Set();
                    }
                    if (value.StartsWith(CommuncationPreamble, StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Remove(0, CommuncationPreamble.Length);
                        this.errorText.AppendLine(value);
                        this.errorReceivedHandle.Set();
                    }
                }
            }

            /// <summary>
            /// Handles the OutputDataReceived event of the process control.
            /// </summary>
            /// <param name="sender">
            /// The source of the event.
            /// </param>
            /// <param name="e">
            /// The <see cref="System.Diagnostics.DataReceivedEventArgs"/> instance containing the event data.
            /// </param>
            private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    string value = e.Data.TrimStart();
                    if (value.StartsWith(CommuncationPreamble, StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Remove(0, CommuncationPreamble.Length);
                        this.outputText.AppendLine(value);
                        this.outputReceivedHandle.Set();
                    }
                }
            }

            #endregion
        }
    }
}
