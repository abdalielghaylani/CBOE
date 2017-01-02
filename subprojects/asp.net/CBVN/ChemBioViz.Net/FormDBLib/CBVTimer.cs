using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections;


namespace FormDBLib
{
    public static class CBVTimer
    {
        #region Variables
        private static bool m_waitCursor = true;
        private static DateTime m_startTime;
        private static DateTime m_endTime;
        private static bool m_appendToExistingFile;
        private static bool m_isShowingMessage = false;
        //Helps to control the use of wait cursor
        private static Stack m_startTimeStack = new Stack();
        private static Stack m_messageStack = new Stack();
        private static String m_timerMessage = String.Empty;
        #endregion

        #region Properties
        public static String TimerMessage
        {
            get { return CBVTimer.m_timerMessage; }
            set { CBVTimer.m_timerMessage = value; }
        }
        #endregion

        #region Methods
        public static void StartTimerWithMessage(bool waitCursor, string startingMessage, bool appendToExistingFile)
        {
            CBVStatMessage.Show(startingMessage);
            m_isShowingMessage = true;
            StartTimer(waitCursor, startingMessage, appendToExistingFile);
        }
        public static void StartTimer(bool waitCursor, string startingMessage, bool appendToExistingFile)
        {
            m_startTime = DateTime.Now;
            m_startTimeStack.Push(m_startTime);
            m_messageStack.Push(startingMessage);

            StringBuilder startingMessageBuilder = new StringBuilder("begin ");
            startingMessageBuilder.Append(startingMessage);
            startingMessageBuilder.Append(" ... ");
            m_timerMessage = startingMessageBuilder.ToString();
            Debug.WriteLine(m_timerMessage);
            LogTimerMessage(m_timerMessage);

            if (waitCursor)
            {
                Cursor.Current = Cursors.WaitCursor;
            }
            m_waitCursor = waitCursor;
            m_appendToExistingFile = appendToExistingFile;
        }
        /// <summary>
        /// Measure elapsed time for a long-running operations.
        /// </summary>
        /// <returns></returns>
        public static void EndTimer()
        {
            string endingMessage = string.Empty;

            m_endTime = DateTime.Now;
			if (m_startTimeStack.Count > 0)
			{
				m_startTimeStack.Pop();
				endingMessage = (string)m_messageStack.Pop();
			}

            if (m_startTimeStack.Count == 0 && m_waitCursor)
            {
                Cursor.Current = Cursors.Default;
                m_waitCursor = false;
            }

            double elapsedTime = 0;
            //Parse to minutes
            System.TimeSpan timeSpan = m_endTime - m_startTime;
            elapsedTime = timeSpan.TotalSeconds;

            StringBuilder common = new StringBuilder(" ");
            common.Append(endingMessage);
            common.Append("... Elapsed time = ");
            common.Append(elapsedTime.ToString("F3"));
            common.Append(" secs");

            //Message for m_log file
            StringBuilder logTime = new StringBuilder(DateTime.Now.ToString());
            logTime.Append(common.ToString());
            logTime.AppendLine();
            
            //Message to display while debugging
            StringBuilder displayLog = new StringBuilder("end ");
            displayLog.Append(common.ToString());
            m_timerMessage = displayLog.ToString();
            Debug.WriteLine(m_timerMessage);
            LogTimerMessage(m_timerMessage);

            if (m_isShowingMessage)
                CBVStatMessage.Hide();
            m_isShowingMessage = false;
        }

        public static void LogTimerMessage(String message)
        {
            if (!CBVUtilities.CBVUtil.Log)
                return;
            String logFilePath = CBVUtilities.CBVUtil.LogPath;            
            if (String.IsNullOrEmpty(logFilePath)) 
                return;
            /* CSBR-152255 Getting error while logging to CBV when user enabled Write Log feature. 
             * Checking whether the directory exists or not. Creating the directory if not exists. */
            string getFilePath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(getFilePath))
                Directory.CreateDirectory(getFilePath);
            /* End of CSBR-152255 */
            String sDateTime = DateTime.Now.ToString();
            String sFullMessage = String.Format("{0}: {1}", sDateTime, message);
            bool bAppend = true;
            using (StreamWriter logWriter = new StreamWriter(logFilePath, bAppend))
                logWriter.WriteLine(sFullMessage);
        }
        #endregion
    }
}
