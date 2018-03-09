using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using Csla;

namespace Csla.Windows
{
    public class DateTextExtender
    {
        public DateTextExtender(Control textControl)
        {
            txt = textControl;
            txt.KeyPress += new KeyPressEventHandler(TextControl_KeyPress);
            txt.Validating += new System.ComponentModel.CancelEventHandler(TextControl_Validating);
        }

        public DateTextExtender(Control textControl, string dateFormat)
        {
            txt = textControl;
            txt.KeyPress += new KeyPressEventHandler(TextControl_KeyPress);
            txt.Validating += new System.ComponentModel.CancelEventHandler(TextControl_Validating);

            date.FormatString = dateFormat;
        }

        protected Control txt;
        protected SmartDate date;

        void TextControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool redisplay = true;

            switch (e.KeyChar)
            {
            	case '+' :
                    SetToday(true);
                    date = new SmartDate(date.Date.AddDays(1));
            		break;
                case '-' :
                    SetToday(true);
                    date = new SmartDate(date.Date.AddDays(-1));
                    break;
                case 't' :
                    SetToday();
                    break;
                case 'T' :
                    SetToday();
                    break;
                default :
                    redisplay = false;
                    break;
            }

            if (redisplay)
            {
                txt.Text = date.Text;
                e.Handled = true;
            }
        }

        void TextControl_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                date = new SmartDate(txt.Text);
                txt.Text = date.Text;
            }
            catch
            {
                date = new SmartDate();
                txt.Text = "";
            }
        }

        void SetToday()
        {
            SetToday(false);
        }

        void SetToday(bool ifEmpty)
        {
            if((ifEmpty && date.IsEmpty) || !ifEmpty)
                date = new SmartDate(DateTime.Now);
        }

    }
}
