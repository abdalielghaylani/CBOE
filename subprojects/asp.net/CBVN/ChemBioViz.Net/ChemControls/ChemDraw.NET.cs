using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AxChemDrawControl14;
using AxChemDrawControlConst11;

namespace ChemControls
{
    public partial class ChemDraw : UserControl
    {
        #region Variables
        string datatype;
        #endregion

        #region Properties
        public bool ReadOnly
        {
            get { return axChemDrawCtl1.ViewOnly; }
            set { axChemDrawCtl1.ViewOnly = value; }
        }
        //---------------------------------------------------------------------
        public object Base64
        {
            get
            {
                axChemDrawCtl1.DataEncoded = true;
                string s = (string)axChemDrawCtl1.get_Data("cdx");
                axChemDrawCtl1.DataEncoded = false;
                return s;
            }
            set
            {
                string dtype = DataType(value);
                if (dtype == "cdxml")
                {
                    axChemDrawCtl1.set_Data("cdxml", value);
                    axChemDrawCtl1.DataEncoded = false;
                }
                else
                {
                    axChemDrawCtl1.DataEncoded = true;
                    axChemDrawCtl1.set_Data("cdx", value);
                    axChemDrawCtl1.DataEncoded = false;
                }
            }
        }
        //---------------------------------------------------------------------
        public DualCdaxControl AxChemDrawControl { get { return axChemDrawCtl1; } }
        //---------------------------------------------------------------------
        public object ChemDrawDocument
        {
            get
            {
                return axChemDrawCtl1.get_Data(datatype);
            }

            set
            {
                datatype = DataType(value);
                if (datatype == "cdxb64")
                {
                    axChemDrawCtl1.DataEncoded = true;
                    datatype = "cdx";
                }
                axChemDrawCtl1.set_Data(datatype, value);
            }
        }
        #endregion

        #region Constructors
        public ChemDraw()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.FixedSingle;	// JD
            datatype = "cdx";
        }
        #endregion

        #region Methods
        public bool IsEmpty()
        {
            return axChemDrawCtl1.IsEmpty();
        }
        //---------------------------------------------------------------------
        public Metafile GetMetafile(object o)
        {
            return ChemDrawForm.GetMetafile(this.axChemDrawCtl1, o);
        }
        //---------------------------------------------------------------------
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
        #endregion
    }
    //---------------------------------------------------------------------
    public class DualCdaxControl : ISupportInitialize
    {
        public static bool m_bUsingConst11 = false;
        private AxHost m_cdax_axhost;

        public DualCdaxControl()
        {
            if (m_bUsingConst11)
                m_cdax_axhost = new AxChemDrawControlConst11.AxChemDrawCtl();//AxInterop.ChemDrawControlConst11.AxChemDrawCtl();
            else
                m_cdax_axhost = new AxChemDrawControl14.AxChemDrawCtl();
        }
        //---------------------------------------------------------------------
        public AxHost CdaxControl
        {
            get { return m_cdax_axhost; }
        }
        //---------------------------------------------------------------------
        public Control Control
        {
            get
            {
                if (m_bUsingConst11)
                    return (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl);//AxInterop.ChemDrawControlConst11.AxChemDrawCtl);
                else
                    return (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl);
            }
        }
        //---------------------------------------------------------------------
        public void BeginInit()
        {
            //Coverity Bug Fix CID :11407 
            if (m_bUsingConst11)
            {
                AxChemDrawControlConst11.AxChemDrawCtl ctrl1 = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                if (ctrl1 != null)
                    ctrl1.BeginInit();
            }
            else
            {
                AxChemDrawControl14.AxChemDrawCtl ctrl2 = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                if (ctrl2 != null)
                    ctrl2.BeginInit();
            }
        }
        //---------------------------------------------------------------------
        public void EndInit()
        {
            //Coverity Bug Fix CID 11408 
            if (m_bUsingConst11)
            {
                AxChemDrawControlConst11.AxChemDrawCtl ctrl1 = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                if (ctrl1 != null)
                    ctrl1.EndInit();
            }
            else
            {
                AxChemDrawControl14.AxChemDrawCtl ctrl2 = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                if (ctrl2 != null)
                    ctrl2.EndInit();
            }
        }
        //---------------------------------------------------------------------
        public virtual bool DataEncoded
        {
            get
            {
                return m_bUsingConst11 ? (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).DataEncoded :
                                        (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).DataEncoded;
            }
            set
            {
                //Coverity Bug Fix CID 11412 
                if (m_bUsingConst11)
                {
                    AxChemDrawControlConst11.AxChemDrawCtl ctrl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                    if (ctrl != null)
                        (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).DataEncoded = value;
                }
                else
                {
                    AxChemDrawControl14.AxChemDrawCtl ctrl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                    if (ctrl != null)
                        (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).DataEncoded = value;
                }
            }
        }
        //---------------------------------------------------------------------
        public virtual bool ViewOnly
        {
            get
            {


                return m_bUsingConst11 ? (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).ViewOnly :
                                    (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).ViewOnly;
            }
            set
            {
                //Coverity Bug Fix CID 11413 
                if (m_bUsingConst11)
                {
                    AxChemDrawControlConst11.AxChemDrawCtl ctrl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                    if (ctrl != null)
                        (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).ViewOnly = value;
                }
                else
                {
                    AxChemDrawControl14.AxChemDrawCtl ctrl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                    if (ctrl != null)
                        (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).ViewOnly = value;
                }
            }
        }
        //---------------------------------------------------------------------
        public virtual IEnumerable Objects
        {
            get
            {
                if (m_bUsingConst11)
                    return (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).Objects;
                else
                    return (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).Objects;
            }
        }
        //---------------------------------------------------------------------
        public virtual object get_Data(object dataType)
        {

            if (m_bUsingConst11)
                return (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).get_Data(dataType);
            else
                return (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).get_Data(dataType);
        }
        //---------------------------------------------------------------------
        public virtual void set_Data(object dataType, object pVal)
        {
            // CSBR-167209: CBVN fails to display molfile and smiles data
            // forcing smiles as a way to let CDAX figure out what the data really is.
            // This is a workaroud for CDAX bug.
            if (m_bUsingConst11)
            {
                AxChemDrawControlConst11.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                //Coverity Bug Fix CID 11411 
                if (ctl != null)
                    (ctl).set_Data("chemical/x-daylight-smiles", pVal);
            }
            else
            {
                AxChemDrawControl14.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                if (ctl != null)
                    (ctl).set_Data("chemical/x-daylight-smiles", pVal);
            }
        }
        //---------------------------------------------------------------------
        public bool IsEmpty()
        {
            AxChemDrawControlConst11.AxChemDrawCtl ctrl1 = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
            AxChemDrawControl14.AxChemDrawCtl ctrl2 = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;

            if (m_bUsingConst11)
                return ctrl1 != null ? (m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl).Atoms.Count == 0 : false;
            else
                return ctrl2 != null ? (m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl).Atoms.Count == 0 : false;
        }
        //---------------------------------------------------------------------
        public String GetCaption()
        {
            String s = String.Empty;
            //Coverity Bug Fix CID 11406 
            if (this.Objects != null)
            {
                if (m_bUsingConst11)
                {
                    AxChemDrawControlConst11.AxChemDrawCtl ctrl1 = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                    if (ctrl1 != null)
                        s = ctrl1.Objects.Formula;
                }
                else
                {
                    AxChemDrawControl14.AxChemDrawCtl ctrl2 = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                    if (ctrl2 != null)
                        s = ctrl2.Objects.Formula;
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public void MoveObjects()
        {
            if (this.Objects != null)
            {
                if (m_bUsingConst11)
                {
                    AxChemDrawControlConst11.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                    //Coverity Bug Fix CID 11410 
                    if (ctl != null)
                        ctl.Objects.Move(-ctl.Objects.Left, -ctl.Objects.Top);
                }
                else
                {
                    AxChemDrawControl14.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                    if (ctl != null)
                        ctl.Objects.Move(-ctl.Objects.Left, -ctl.Objects.Top);
                }
            }
        }
        //---------------------------------------------------------------------
        public void GetObjectsDimensions(ref int w, ref int h, ref int l, ref int t)
        {
            if (this.Objects != null)
            {
                if (m_bUsingConst11)
                {
                    AxChemDrawControlConst11.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                    //Coverity Bug Fix CID 11409 
                    if (ctl != null)
                    {
                        w = (int)ctl.Objects.Width;
                        h = (int)ctl.Objects.Height;
                        l = (int)ctl.Objects.Left;
                        t = (int)ctl.Objects.Top;
                    }
                }
                else
                {
                    AxChemDrawControl14.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                    if (ctl != null)
                    {
                        w = (int)ctl.Objects.Width;
                        h = (int)ctl.Objects.Height;
                        l = (int)ctl.Objects.Left;
                        t = (int)ctl.Objects.Top;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public void SetOcxState(System.ComponentModel.ComponentResourceManager resources)
        {
            if (m_bUsingConst11)
            {
                AxChemDrawControlConst11.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControlConst11.AxChemDrawCtl;
                //Coverity Bug Fix CID 18696 
                if (ctl != null)
                    ctl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axChemDrawCtl1.OcxState")));
            }
            else
            {
                AxChemDrawControl14.AxChemDrawCtl ctl = m_cdax_axhost as AxChemDrawControl14.AxChemDrawCtl;
                if (ctl != null)
                    ctl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axChemDrawCtl1.OcxState")));
            }
        }
        //---------------------------------------------------------------------
    }
}
