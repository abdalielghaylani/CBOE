// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHost.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StructureSearchSupport
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
    public abstract class ProcessHost : IDisposable
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
        private EventWaitHandle _errorReceivedHandle;

        /// <summary>
        /// The wait handle for output.
        /// </summary>
        private EventWaitHandle _outputReceivedHandle;

        /// <summary>
        /// The external process.
        /// </summary>
        private Process _process;

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
            DisposeProcess();
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
            var call = GetEncodedCommandCall(command, parameters);
            string errorText;
            string outputText;

            using (
                var asyncProcessReader = new AsyncProcessReader(
                    _process, _errorReceivedHandle, _outputReceivedHandle))
            {
                _process.StandardInput.WriteLine(call);

                errorText = string.Empty;
                outputText = string.Empty;

                // Wait until we have data on both the Error and the Output lines...
                // To make sure that this works on both STA and MTA we will wait for each one in turn...
                // And pump messages while we wait so Spotfire remains responsive and keeps painting
                while (true)
                {
                    Application.DoEvents();

                    // Wait for the data
                    if (_outputReceivedHandle.WaitOne(100))
                    {
                        outputText = asyncProcessReader.OutputText;
                    }

                    // Wait for the error data
                    if (_errorReceivedHandle.WaitOne(100))
                    {
                        errorText = asyncProcessReader.ErrorText;
                    }

                    //LD-794,LD-911,LD-912
                    //LD-1234
                    if (ExistProcess() && string.IsNullOrEmpty(outputText) && string.IsNullOrEmpty(errorText))
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

            var encodedValues = outputText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var values = new List<string>();
            foreach (var encodedValue in encodedValues)
            {
                var value = FromBase64(encodedValue);
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
            DisposeProcess();

            var externalProcess = ExecutableName;
            var arguments = Arguments;
            var processStartInfo = new ProcessStartInfo(externalProcess, arguments)
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            try
            {
                _process = Process.Start(processStartInfo);
                _process.EnableRaisingEvents = true;
                _errorReceivedHandle = new AutoResetEvent(false);
                _outputReceivedHandle = new AutoResetEvent(false);
                return !_process.HasExited;
            }
            catch (Exception)
            {
                return false;
            }
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
        private bool ExistProcess()
        {
            return !_process.Responding || _process.HasExited;
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
            var bytes = Convert.FromBase64String(value);
            var result = Encoding.UTF8.GetString(bytes);
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
        private static string GetEncodedCommandCall(string command, ICollection<object> parameters)
        {
            var commands = new List<string>(parameters.Count + 1) {command};

            // The actual command is not encoded.
            foreach (var parameter in parameters)
            {
                var encodedValue = ToBase64(parameter);
                commands.Add(encodedValue);
            }

            var call = string.Join(" ", commands.ToArray());
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

            if (string.IsNullOrEmpty(errorText))
            {
                return;
            }
            // The Marvin editor requires JRE 7 (64-bit) or higher, we get this error
            // if an earlier version is installed
            if (errorText.StartsWith("java.lang.UnsupportedClassVersionError"))
            {
                throw new Exception("IncorrectJREVersion");
            }

            throw new Exception(FromBase64(errorText));
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
            var valueAsString = value.ToString();
            var bytes = Encoding.UTF8.GetBytes(valueAsString);
            var base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        /// <summary>
        /// Disposes the process.
        /// </summary>
        private void DisposeProcess()
        {
            if (_process == null)
            {
                return;
            }
            _process.Dispose();
            _process = null;
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
            private readonly EventWaitHandle _errorReceivedHandle;

            /// <summary>
            /// The StringBuilder for error texts.
            /// </summary>
            private readonly StringBuilder _errorText;

            /// <summary>
            /// The handle to signal when a text on StdOut has been received.
            /// </summary>
            private readonly EventWaitHandle _outputReceivedHandle;

            /// <summary>
            /// The StringBuilder for output texts.
            /// </summary>
            private readonly StringBuilder _outputText;

            /// <summary>
            /// The external process.
            /// </summary>
            private readonly Process _process;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncProcessReader"/> class.
            /// </summary>
            /// <param name="process">
            /// The process.
            /// </param>
            /// <param name="errReceivedHandle">
            /// The handle to signal when a text on StdErr has been received.
            /// </param>
            /// <param name="outReceivedHandle">
            /// The handle to signal when a text on StdOut has been received.
            /// </param>
            public AsyncProcessReader(
                Process process, EventWaitHandle errReceivedHandle, EventWaitHandle outReceivedHandle)
            {
                _process = process;

                _outputText = new StringBuilder();
                _errorText = new StringBuilder();

                _errorReceivedHandle = errReceivedHandle;
                _outputReceivedHandle = outReceivedHandle;

                process.ErrorDataReceived += ProcessErrorDataReceived;
                process.OutputDataReceived += ProcessOutputDataReceived;

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
                    return _errorText.ToString();
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
                    return _outputText.ToString();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _process.ErrorDataReceived -= ProcessErrorDataReceived;
                _process.OutputDataReceived -= ProcessOutputDataReceived;

                _process.CancelErrorRead();
                _process.CancelOutputRead();
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
                if (e.Data == null)
                {
                    return;
                }
                var value = e.Data.TrimStart();

                // The Marvin editor requires JRE 7 (64-bit) or higher, we get this error
                // if an earlier version is installed
                if (value.StartsWith("java.lang.UnsupportedClassVersionError"))
                {
                    _errorText.AppendLine(value);
                    _errorReceivedHandle.Set();
                }
                if (!value.StartsWith(CommuncationPreamble, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                value = value.Remove(0, CommuncationPreamble.Length);
                _errorText.AppendLine(value);
                _errorReceivedHandle.Set();
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
                if (e.Data == null)
                {
                    return;
                }
                var value = e.Data.TrimStart();
                if (!value.StartsWith(CommuncationPreamble, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                value = value.Remove(0, CommuncationPreamble.Length);
                _outputText.AppendLine(value);
                _outputReceivedHandle.Set();
            }

            #endregion
        }
    }
}
