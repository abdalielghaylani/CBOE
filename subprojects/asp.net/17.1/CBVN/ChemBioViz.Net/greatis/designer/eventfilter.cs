/*  GREATIS FORM DESIGNER FOR .NET
 *  Private Designer Interface Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using System.Diagnostics;

using System.IO;
using System.Collections.Specialized;
using System.Reflection;

using System.Runtime.Remoting;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Greatis
{
   namespace FormDesigner
   {
      #region EventFilter
      class EventFilter : IMessageFilter
      {
         private DesignerHost host;
         public EventFilter(DesignerHost h)
         {
            host = h;
         }

         public event EventHandler DoubleClick;
         public event KeyEventHandler KeyDown;
         public event KeyEventHandler KeyUp;
         public event MouseEventHandler MouseUp;
         public event MouseEventHandler MouseDown;

         #region IMessageFilter Members

         private bool HaveFocus(Control ctrl)
         {
            if (ctrl.Focused == true)
               return true;

            foreach (Control childCtrl in ctrl.Controls)
            {
               if (childCtrl.Focused == true)
                  return true;
            }
            return false;
         }

         public bool PreFilterMessage(ref Message m)
         {
            const int WM_LBUTTONDBLCLK = 0x0203, WM_LBUTTONDOWN = 0x0201,
               WM_LBUTTONUP = 0x202, WM_RBUTTONDOWN = 0x204, WM_RBUTTONUP = 0x205,
               WM_MBUTTONDOWN = 0x207, WM_MBUTTONUP = 0x208;
            const int WM_KEYDOWN = 0x100;//, WM_SYSKEYDOWN = 0x104;
            const int WM_KEYUP = 0x101;//, WM_SYSKEYUP = 0x105;
            if (m.Msg != WM_KEYDOWN && m.Msg == WM_KEYUP && m.Msg != WM_LBUTTONDBLCLK)
               return false;

            if (host.DesignedForm == null)
               return false;

            bool focused = HaveFocus(host.DesignedForm);
            if (focused == false)
               return false;

            if (MouseDown != null && (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN))
            {
               Point mousePoint = Cursor.Position;
               MouseEventArgs arg = new MouseEventArgs((m.Msg == WM_LBUTTONDOWN) ? MouseButtons.Left :
                  (m.Msg == WM_RBUTTONDOWN) ? MouseButtons.Right : MouseButtons.Middle,
                  1, mousePoint.X, mousePoint.Y, 1);
               MouseDown(this, arg);
            }
            else if (MouseUp != null && (m.Msg == WM_LBUTTONUP || m.Msg == WM_RBUTTONUP || m.Msg == WM_MBUTTONUP))
            {
               Point mousePoint = Cursor.Position;
               MouseEventArgs arg = new MouseEventArgs((m.Msg == WM_LBUTTONUP) ? MouseButtons.Left :
                  (m.Msg == WM_RBUTTONUP) ? MouseButtons.Right : MouseButtons.Middle,
                  1, mousePoint.X, mousePoint.Y, 1);
               MouseUp(this, arg);
            }
            else if (m.Msg == WM_LBUTTONDBLCLK && DoubleClick != null)
            {
               EventArgs arg = new EventArgs();
               DoubleClick(this, arg);
            }
            else if ((m.Msg == WM_KEYDOWN) && KeyDown != null)
            {
               KeyEventArgs args = new KeyEventArgs((Keys)((int)m.WParam) |
                  Control.ModifierKeys);

               KeyDown(this, args);
            }
            else if ((m.Msg == WM_KEYUP) && KeyUp != null)
            {
               KeyEventArgs args = new KeyEventArgs((Keys)((int)m.WParam) |
                  Control.ModifierKeys);

               KeyUp(this, args);
            }
            return (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP);// false;
         }

         #endregion
      }
      #endregion
   }
}
