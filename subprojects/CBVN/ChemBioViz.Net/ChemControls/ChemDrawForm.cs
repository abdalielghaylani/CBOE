using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ChemControls
{
    public partial class ChemDrawForm : Form
    {
        public ChemDrawForm()
        {
            InitializeComponent();
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
        }

        public void Reposition(int xCenter, int yCenter, int preferredWidth, int preferredHeight)
        {
            Left = xCenter - preferredWidth / 2;
            Top = yCenter - preferredHeight / 2;
            Width = preferredWidth;
            Height = preferredHeight;

            Left = System.Math.Max(0, Left);
            Top = System.Math.Max(0, Top);

            if (Right > Parent.Width)
                Width = Parent.Width - Left;
            if (Bottom > Parent.Height)
                Height = Parent.Height - Top;
        }

#if ANDRAS
        void ChemDrawEditor_DataChanged(object sender, EventArgs e)
        {
            if (!chemDrawEditor.Visible)
                return;

            int dw = (int)chemDrawEditor.Objects.Width + 20 - chemDrawEditor.Width;
            int dh = (int)chemDrawEditor.Objects.Height + 20 - chemDrawEditor.Height;

            if (dw < 0 && dh < 0)
                return;

            dw = System.Math.Max(0, dw);
            dh = System.Math.Max(0, dh);

            Reposition(Left + Width / 2, Top + Height / 2, Width + dw, Height + dh);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            chemDrawEditor.Focus();
        }
        public object Value
        {
            get
            {
                chemDrawEditor.DataEncoded = dataType == "cdxb64";
                object o = chemDrawEditor.get_Data(dataType == "cdxb64" ? "cdx" : dataType);
                chemDrawEditor.DataEncoded = false;
                return o;
            }
            set
            {
                SetChemDrawData(chemDrawEditor, value);
            }
        }
#endif

        static string DataType(object o)
        {
            if (o == null)
                return "cdx";

            string t = "";

            if (o.GetType() == typeof(byte[]))
            {
                byte[] p = o as byte[];
                ASCIIEncoding AE = new ASCIIEncoding();
                t = AE.GetString(p);
            }
            else if (o.GetType() == typeof(string))
                t = System.Convert.ToString(o);

            if (t.Length < 4)
                return "cdx";

            t = t.Substring(0, 4);

            if (t == "VjCD")
                return "cdx";
            if (t == "<?xm")
                return "cdxml";
            else if (t == "VmpD")
                return "cdxb64";
            else
                return "cdx";
        }

        public static void SetChemDrawData(DualCdaxControl ctl, object o)
        {
            if (ctl == null)
                return;

            if (o == null)
                return;

            if (o is System.DBNull)
                return;

            if (o is string)
            {
                string s = o as string;
                if (s == "")
                    return;
            }

            string dataType = DataType(o);

            ctl.DataEncoded = dataType == "cdxb64";
            // try cdx, followed by mol - ideally we should be able to find out 
            // what the format is from the dataview, or by investigation, but the DataType
            // method isn't finished yet.
            try
            {
                ctl.set_Data(dataType == "cdxb64" ? "cdx" : dataType, o);
            }
            catch (Exception)
            {
                try
                {
                    ctl.set_Data("mol", o);
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
            ctl.DataEncoded = false;
        }

        public String GetCaption(object o)
        {
            return GetCaption(chemDrawCtl, o);
        }

        public String GetCaption(DualCdaxControl ctl, object o)
        {
            return ctl.GetCaption();
        }

        public Metafile GetMetafile(object o)
        {
            return GetMetafile(chemDrawCtl, o);
        }

        public static Metafile GetMetafile(DualCdaxControl ctl, object o)
        {
            if (ctl == null)
                return null;

            if (o == null)
                return null;

            if (o is DBNull)
                return null;

            SetChemDrawData(ctl, o);

            ctl.MoveObjects();

            object oo = ctl.get_Data("wmf");    // TO DO: tell it we want a white background in the metafile
            Byte[] b = oo as Byte[];
            System.IO.MemoryStream mem = new System.IO.MemoryStream(b);
            Metafile m = new Metafile(mem);
            return m;
        }

        public Rectangle GetBounds(object o)
        {
            return GetBounds(chemDrawCtl, o);
        }

        public Rectangle GetBounds(DualCdaxControl ctl, object o)
        {
            if (ctl == null)
                return new Rectangle();

            SetChemDrawData(ctl, o);

            ctl.MoveObjects();

            int w = 0, h = 0, l = 0, t = 0;
            ctl.GetObjectsDimensions(ref w, ref h, ref l, ref t);

            Rectangle r = new Rectangle(l, t, w, h);
            return r;
        }

        public object Convert(object mimetype, object o)
        {
            SetChemDrawData(chemDrawCtl, o);
            return chemDrawCtl.get_Data(mimetype);
        }

        public DualCdaxControl ChemDrawCtl { get { return chemDrawCtl; } }
    }
}
