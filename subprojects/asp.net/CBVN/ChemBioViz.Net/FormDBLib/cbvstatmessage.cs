using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using CBVUtilities;

namespace FormDBLib
{
    public class CBVStatMsgItem
    {
        #region Variables
        public String m_message;
        public DateTime m_startTime;
        #endregion

        #region Constructor
        public CBVStatMsgItem(String message)
        {
            m_message = message;
            m_startTime = System.DateTime.Now;
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public static class CBVStatMessage
    {
        #region Variables
        private static Stack m_stack = new Stack(); // WRONG! this should belong to a window
        #endregion

        #region Events
        // to be set by caller
        public static event EventHandler<CBVEventArgs> StatMessageEvent;
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public static void ShowReadyMsg()
        {
            m_stack.Clear();
            Show("Ready");
        }
        //---------------------------------------------------------------------
        public static void Show(String message)
        {
            // show message on status bar, push onto stack
            if (StatMessageEvent != null)
            {
                StatMessageEvent(null, new CBVEventArgs(message));
                m_stack.Push(new CBVStatMsgItem(message));

                CBVTimer.LogTimerMessage("==> " + message);
            }
        }
        //---------------------------------------------------------------------
        public static void Hide()
        {
            // take down latest shown message; go back to previous if any
            if (StatMessageEvent != null)
            {
                if (m_stack.Count <= 1)
                {
                    StatMessageEvent(null, new CBVEventArgs(string.Empty));
                    m_stack.Clear();
                }
                else
                {
                    CBVStatMsgItem topItem = m_stack.Pop() as CBVStatMsgItem;
                    CBVStatMsgItem prevItem = m_stack.Peek() as CBVStatMsgItem;
                    //Coverity Bug Fix CID 13026 
                    if (prevItem != null)
                        StatMessageEvent(null, new CBVEventArgs(prevItem.m_message));
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
