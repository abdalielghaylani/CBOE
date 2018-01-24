using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Xml;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinGrid;
using FormDBLib;
using ChemControls;
using CBVUtilities;

namespace ChemBioViz.NET
{
    public class CBVPrintingHelper
    {
        #region Variables
        protected PrintDocument pDocSetup = new PrintDocument();      // container for printer settings
        protected bool printCancelled = false;
        protected bool showAllRows = false;
        protected Point parentLocation;
        protected CBVPrintSettings cbvPrintSettings;
        protected String formName, userName;
        protected int pageNo;
        #endregion

        #region Constructor
        public CBVPrintingHelper()
        {
            cbvPrintSettings = new CBVPrintSettings(this);
        }
        #endregion

        #region Properties
        public CBVPrintSettings CBVPrintSettings
        {
            get { return cbvPrintSettings; }
        }
        public String FormName
        {
            get { return formName; }
            set { formName = value; }
        }
        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public int PageNo
        {
            get { return pageNo; }
            set { pageNo = value; }
        }
        public bool PrintCancelled
        {
            get { return printCancelled; }
        }
        public PrintDocument PrintSetupDocument
        {
            get { return this.pDocSetup; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        public void InitializePrintHelper(ChemBioVizForm form)
        {
            printCancelled = false;
            showAllRows = false;
            this.parentLocation = form.Location;

            this.FormName = form.FormName;
            this.UserName = form.FormDbMgr.Login.UserName;
            this.PageNo = 0;
        }
        //---------------------------------------------------------------------
        public void PageSetup(ChemBioVizForm form)
        {
            InitializePrintHelper(form);
            CBVPrintSetupDialog dlg = new CBVPrintSetupDialog(this);
            dlg.ShowDialog();
        }
        //---------------------------------------------------------------------
        public virtual void Print(bool bSilent)
        {
        }
        //---------------------------------------------------------------------
        public virtual void PrintPreview()
        {
        }
        //---------------------------------------------------------------------
        protected bool DoPrintDialog()
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.AllowSomePages = true;
            pDialog.Document = pDocSetup;
            pDialog.PrinterSettings.PrintRange = PrintRange.AllPages;
            pDialog.AllowSelection = true;
            pDialog.AllowCurrentPage = true;
            pDialog.PrinterSettings.Collate = true;
            pDialog.PrintToFile = false;
            pDialog.ShowHelp = false;
            pDialog.ShowNetwork = true;

            pDialog.PrinterSettings.FromPage = 1;
            pDialog.PrinterSettings.ToPage = 1;

            DialogResult option = pDialog.ShowDialog();
            return option == DialogResult.OK;
        }
        //---------------------------------------------------------------------
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            return this.CBVPrintSettings.CreateXmlElement(xdoc, eltname);
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            this.CBVPrintSettings.LoadXmlElement(node);
        }
        //---------------------------------------------------------------------
        public static void DrawHeaderAndFooter(Graphics g, Rectangle marginBounds, Rectangle pageBounds,
                                               CBVPrintSettings cps)
        {
            Font hdrFont = cps.Header.Font;
            Font ftrFont = cps.Footer.Font;



            int hfheight = 50;              // half inch
            for (int i = 0; i < 6; ++i)     // HDR L,C,R  FTR L,C,R
            {
                String sText = String.Empty;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = (i >= 3) ? StringAlignment.Near : StringAlignment.Far;
                switch (i)
                {
                    case 0: sText = cps.Header.Left; sf.Alignment = StringAlignment.Near; break;
                    case 1: sText = cps.Header.Center; sf.Alignment = StringAlignment.Center; break;
                    case 2: sText = cps.Header.Right; sf.Alignment = StringAlignment.Far; break;
                    case 3: sText = cps.Footer.Left; sf.Alignment = StringAlignment.Near; break;
                    case 4: sText = cps.Footer.Center; sf.Alignment = StringAlignment.Center; break;
                    case 5: sText = cps.Footer.Right; sf.Alignment = StringAlignment.Far; break;
                }
                if (String.IsNullOrEmpty(sText)) continue;

                int xpos = marginBounds.Left;
                int ypos = (i < 3) ? 0 : pageBounds.Bottom - hfheight;
                RectangleF rText = new RectangleF(xpos, ypos, marginBounds.Width, hfheight);

                if (i < 3)
                    //Coverity Bug Fix CID 13150 
                    using (Brush hdrBrush = new SolidBrush(cps.Header.FontColor))
                    {
                        g.DrawString(sText, hdrFont, hdrBrush, rText, sf);
                    }
                else
                    using (Brush ftrBrush = new SolidBrush(cps.Footer.FontColor))
                    {
                        g.DrawString(sText, ftrFont, ftrBrush, rText, sf);
                    }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }

    public class CBVGridPrintingHelper : CBVPrintingHelper
    {
        #region Variables
        private UltraPrintPreviewDialog uPrintPreviewDialog;
        private UltraGridPrintDocument uGridPrintDoc;
        private ChemDataGrid cd = new ChemDataGrid();
        #endregion

        #region Methods
        public void InitializeGridPrintHelper(ChemDataGrid cdGrid, ChemBioVizForm form)
        {
            InitializePrintHelper(form);

            cd = cdGrid;
            cd.InitializeRow += new InitializeRowEventHandler(cd_InitializeRow);
            uGridPrintDoc = new UltraGridPrintDocument();
            uGridPrintDoc.RowProperties = RowPropertyCategories.All;
            uGridPrintDoc.Grid = cd;
            uGridPrintDoc.DefaultPageSettings = pDocSetup.DefaultPageSettings;
        }

        /// <summary>
        ///  Show print preview
        /// </summary>
        public override void PrintPreview()
        {
            this.uPrintPreviewDialog = new UltraPrintPreviewDialog();
            this.uPrintPreviewDialog.PointToScreen(this.parentLocation);

            uGridPrintDoc.Header.TextLeft = this.CBVPrintSettings.Header.Left;
            uGridPrintDoc.Header.TextCenter = this.CBVPrintSettings.Header.Center;
            uGridPrintDoc.Header.TextRight = this.CBVPrintSettings.Header.Right;
            uGridPrintDoc.Header.Padding.Bottom = 8;

            uGridPrintDoc.Footer.TextLeft = this.CBVPrintSettings.Footer.Left;
            uGridPrintDoc.Footer.TextCenter = this.CBVPrintSettings.Footer.Center;
            uGridPrintDoc.Footer.TextRight = this.CBVPrintSettings.Footer.Right;
            uGridPrintDoc.Footer.Padding.Top = 6;

            uPrintPreviewDialog.Document = uGridPrintDoc;
            uPrintPreviewDialog.Show();
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Send record(s) to the selected printer
        /// </summary>
        public override void Print(bool bSilent)
        {
            if (bSilent || DoPrintDialog())
            {
                uGridPrintDoc.Grid.Print(uGridPrintDoc.Grid.DisplayLayout, pDocSetup);
            }
        }
        //---------------------------------------------------------------------
        private bool HasContiguousSel(ChemDataGrid cdg)
        {
            // true if all selected rows are together, no gaps
            int curIndex = -1;
            foreach (UltraGridRow row in cdg.Selected.Rows)
            {
                int index = cdg.Rows.IndexOf(row);
                if (index < 0)
                    return false;
                if (curIndex != -1 && (Math.Abs(curIndex - index) > 1))
                    return false;
                curIndex = index;
            }
            return true;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Ask if you want to print just the selected records or the whole dataset
        /// </summary>
        public void AskPrintingMode()
        {
            bool bOkToPrintSelection = cd.Selected.Rows.Count > 0; // && HasContiguousSel(cd);
            if (cd.CardViewMode)
                bOkToPrintSelection = false;    // CSBR-134533 -- prevent crash

            if (bOkToPrintSelection)
            {
                // display message if file exists on the server
                String sMsg = String.Concat("Preview just the selected rows?\n\n",
                    "Click Yes to preview the selection, No to preview all, or Cancel to abort the preview. This choice does not apply in Card View.");
                DialogResult userOption = MessageBox.Show(sMsg, "Information", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (userOption == DialogResult.No)
                {
                    showAllRows = true;
                }
                else if (userOption == DialogResult.Cancel)
                    printCancelled = true;
                else
                    showAllRows = false;

            }
            else
                showAllRows = true;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        ///  Initialize rows before printing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cd_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //if the layout is for printing or exporting get
            if (e.Row.Band.Layout.IsPrintLayout)
            {
                ChemDataGrid cd_grid = sender as ChemDataGrid;
                //since the ultragrid doesn't carry the selected state of a row to the print or export you have to find the real row that is
                //being displayed to the user
                Infragistics.Win.UltraWinGrid.UltraGridRow realGridRow = cd_grid.GetRowFromPrintRow(e.Row);
                //hide the row that is being used for printing/exporting so it doesn't show up
                if (!showAllRows)
                    e.Row.Hidden = !realGridRow.Selected;
            }
        }
        #endregion
    }

    public class CBVFormPrintingHelper : CBVPrintingHelper
    {
        #region Variables
        FormViewControl m_formview;
        FormPrintLayout m_printLayout;
        PrintDocument m_printDoc;
        int m_origCurrRec, m_currPrintRec, m_maxRecs;
        private int m_origY;    // top of page in form coords; nonzero if continuation
        bool m_bIsQueryForm;
        #endregion

        #region Properties
        public FormPrintLayout PrintLayout
        {
            get { return m_printLayout; }
        }
        #endregion

        #region Methods
        public void InitializeFormPrintHelper(FormViewControl formview, ChemBioVizForm form, bool bIsQueryForm)
        {
            m_formview = formview;
            m_bIsQueryForm = bIsQueryForm;
            InitializePrintHelper(form);

            m_printLayout = new FormPrintLayout(formview);
            m_printDoc = new PrintDocument();
            m_printDoc.PrintPage += new PrintPageEventHandler(m_printDoc_PrintPage);
            m_printDoc.DefaultPageSettings = pDocSetup.DefaultPageSettings;

            m_origCurrRec = 0;
            m_maxRecs = 0;
            m_currPrintRec = 0;
            m_origY = 0;
        }

        public override void PrintPreview()
        {
            PrintPreviewDialog ppDialog = new PrintPreviewDialog();
            ppDialog.Document = m_printDoc;
            m_origY = 0;
            m_origCurrRec = (m_formview.Form.Pager == null) ? 0 : m_formview.Form.Pager.CurrRow;
            m_maxRecs = (m_formview.Form.Pager == null) ? 0 : m_formview.Form.Pager.ListSize;
            // CSBR-152727: To avoid exceptions due to huge memory usage while previewing records count > 1000 so restricting the record count up to 500 only.
            if (m_maxRecs > 500)
            {
                MessageBox.Show("Records for print preview can't exceed more than 500");
                return;
            }
            ppDialog.ShowDialog();
            m_formview.Form.DoMove(Pager.MoveType.kmGoto, m_origCurrRec);
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        public override void Print(bool bSilent)
        {
            if (bSilent || DoPrintDialog())
            {
                m_printDoc.PrinterSettings = pDocSetup.PrinterSettings;
                m_printDoc.DefaultPageSettings = pDocSetup.DefaultPageSettings;
                m_origY = 0;
                m_origCurrRec = (m_formview.Form.Pager == null) ? 0 : m_formview.Form.Pager.CurrRow;
                m_maxRecs = (m_formview.Form.Pager == null) ? 0 : m_formview.Form.Pager.ListSize;
                m_printDoc.Print();
                m_formview.Form.DoMove(Pager.MoveType.kmGoto, m_origCurrRec);
            }
        }
        //---------------------------------------------------------------------
        private void PrintHeaderAndFooter(PrintPageEventArgs e)
        {
            DrawHeaderAndFooter(e.Graphics, e.MarginBounds, e.PageBounds, CBVPrintSettings);
        }
        //---------------------------------------------------------------------
        void m_printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            PrinterSettings ps = this.m_printDoc.PrinterSettings;
            bool bPrintCurrRecOnly = ps.PrintRange == PrintRange.Selection || m_bIsQueryForm;
            bool bPrintRange = ps.PrintRange == PrintRange.SomePages;
            if (bPrintCurrRecOnly)
                m_currPrintRec = m_origCurrRec;

        DoPrintPage:
            g.Transform.Reset();
            this.PageNo = this.PageNo + 1;
            bool bPrintThisPage = !bPrintRange || (pageNo >= ps.FromPage && pageNo <= ps.ToPage);

            if (bPrintThisPage)
                PrintHeaderAndFooter(e);

            // set form origin in upper left of margins
            g.TranslateTransform(e.MarginBounds.Left, -m_origY + e.MarginBounds.Top);

            // clip to margins
            Rectangle rClip = e.MarginBounds;
            rClip.Offset(-rClip.Left, -(rClip.Top - m_origY));
            g.SetClip(rClip);

            // print as many forms as fit on the page
            e.HasMorePages = true;
            while (true)
            {
                m_formview.Form.DoMove(Pager.MoveType.kmGoto, m_currPrintRec);

                if (bPrintThisPage)
                    m_printLayout.Render(g, e);
                Rectangle formSpace = m_printLayout.GetFormSpace();

                // if form doesn't fit, reset origin and print what fits
                bool bPageOverflow = (formSpace.Bottom - m_origY) > e.MarginBounds.Bottom;
                if (bPageOverflow)
                {
                    m_origY += (rClip.Height - 20);
                    break;
                }
                m_origY = 0;

                // if done with list, terminate printing
                if (++m_currPrintRec >= m_maxRecs || bPrintCurrRecOnly)
                {
                    e.HasMorePages = false;
                    //if (!bPrintCurrRecOnly)
                    //    e.Cancel = true;
                    break;
                }

                // move origin and see if we can fit another
                g.TranslateTransform(0, formSpace.Height);
                bool bHasRoomForMore = (g.Transform.OffsetY + formSpace.Height) <= e.MarginBounds.Bottom;
                if (!bHasRoomForMore)
                {
                    if (bPrintRange && !bPrintThisPage)
                    {
                        g.TranslateTransform(-g.Transform.OffsetX, -g.Transform.OffsetY);
                        goto DoPrintPage;
                    }
                    break;
                }
            }
            if (bPrintRange && this.PageNo >= ps.ToPage)
                e.HasMorePages = false;
        }
        //---------------------------------------------------------------------
        #endregion
    }

    #region PrintSettings
    public class CBVPrintSettings
    {
        private CBVPrintingHelper m_printHelper;
        private CBVHeaderFooter m_header, m_footer;

        public CBVPrintSettings(CBVPrintingHelper printHelper)
        {
            m_printHelper = printHelper;

            m_header = new CBVHeaderFooter(this);
            m_footer = new CBVHeaderFooter(this);

            m_header.Center = (printHelper is CBVGridPrintingHelper) ? "CBVN Grid Print" : "CBVN Form Print";
            m_header.Left = "$F";
            m_header.Right = "Page #";
            m_footer.Center = "$D : $T";
        }
        //---------------------------------------------------------------------
        public CBVPrintingHelper CBVPrintHelper
        {
            get { return m_printHelper; }
        }
        public CBVHeaderFooter Header
        {
            get { return m_header; }
        }
        public CBVHeaderFooter Footer
        {
            get { return m_footer; }
        }
        //---------------------------------------------------------------------
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement elt = xdoc.CreateElement(eltname);
            PageSettings page = this.m_printHelper.PrintSetupDocument.DefaultPageSettings;
            elt.SetAttribute("landscape", page.Landscape ? "1" : "0");
            elt.SetAttribute("marginL", page.Margins.Left.ToString());
            elt.SetAttribute("marginR", page.Margins.Right.ToString());
            elt.SetAttribute("marginT", page.Margins.Top.ToString());
            elt.SetAttribute("marginB", page.Margins.Bottom.ToString());

            if (!m_header.Empty)
                elt.AppendChild(m_header.CreateXmlElement(xdoc, "header"));
            if (!m_footer.Empty)
                elt.AppendChild(m_footer.CreateXmlElement(xdoc, "footer"));
            return elt;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            PageSettings page = this.m_printHelper.PrintSetupDocument.DefaultPageSettings;
            page.Landscape = CBVUtil.GetIntAttrib(node, "landscape") == 1;
            page.Margins.Left = CBVUtil.GetIntAttrib(node, "marginL");
            page.Margins.Right = CBVUtil.GetIntAttrib(node, "marginR");
            page.Margins.Top = CBVUtil.GetIntAttrib(node, "marginT");
            page.Margins.Bottom = CBVUtil.GetIntAttrib(node, "marginB");

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.Equals("header"))
                    m_header.LoadXmlElement(childNode);
                else if (childNode.Name.Equals("footer"))
                    m_footer.LoadXmlElement(childNode);
            }
        }
        //---------------------------------------------------------------------
    }
    #region HeaderFooter
    public class CBVHeaderFooter
    {
        public String m_left, m_center, m_right;
        private Font m_font;
        private Color m_fontColor;
        private CBVPrintSettings m_cbvPrintSettings;

        public CBVHeaderFooter(CBVPrintSettings cbvPrintSettings)
        {
            m_cbvPrintSettings = cbvPrintSettings;
            m_left = m_center = m_right = String.Empty;
            m_font = new Font("Arial", 9);
            m_fontColor = Color.Black;
        }
        //---------------------------------------------------------------------
        private String SubstituteTokens(String sOrig)
        {
            String s = sOrig;
            CBVPrintingHelper ph = m_cbvPrintSettings.CBVPrintHelper;
            if (ph is CBVGridPrintingHelper)
                s = s.Replace("#", "[Page #]"); // let infragistics number the pages
            else
                s = s.Replace("#", ph.PageNo.ToString());

            s = s.Replace("$T", System.DateTime.Now.ToLongTimeString());
            s = s.Replace("$D", System.DateTime.Today.ToLongDateString());
            s = s.Replace("$t", System.DateTime.Now.ToShortTimeString());
            s = s.Replace("$d", System.DateTime.Today.ToShortDateString());
            s = s.Replace("$F", ph.FormName);
            s = s.Replace("$U", ph.UserName);
            return s;
        }
        //---------------------------------------------------------------------
        /* Infragistics: 
                [Page #] Inserts the current page number 
                [Date Printed] Inserts the date when the print operation began. 
                [User Name] Inserts the user name as available from UserName 
                [Time Printed] Inserts the time when the print operation began 

         * DateTime strings:
                Long date pattern: "dddd, MMMM dd, yyyy"
                Long date string:  "Wednesday, May 16, 2001"

                Long time pattern: "h:mm:ss tt"
                Long time string:  "3:02:15 AM"

                Short date pattern: "M/d/yyyy"
                Short date string:  "5/16/2001"

                Short time pattern: "h:mm tt"
                Short time string:  "3:02 AM"
         */
        //---------------------------------------------------------------------
        public String Left
        {
            get { return SubstituteTokens(m_left); }
            set { m_left = value; }
        }
        //---------------------------------------------------------------------
        public String Center
        {
            get { return SubstituteTokens(m_center); }
            set { m_center = value; }
        }
        //---------------------------------------------------------------------
        public String Right
        {
            get { return SubstituteTokens(m_right); }
            set { m_right = value; }
        }
        //---------------------------------------------------------------------
        public Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }
        //---------------------------------------------------------------------
        public Color FontColor
        {
            get { return m_fontColor; }
            set { m_fontColor = value; }
        }
        //---------------------------------------------------------------------
        public bool Empty
        {
            get { return String.IsNullOrEmpty(m_left) && String.IsNullOrEmpty(m_center) && String.IsNullOrEmpty(m_right); }
            set { m_left = String.Empty; m_center = String.Empty; m_right = String.Empty; }
        }
        //---------------------------------------------------------------------
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement elt = xdoc.CreateElement(eltname);
            if (!String.IsNullOrEmpty(m_left)) elt.SetAttribute("left", m_left);
            if (!String.IsNullOrEmpty(m_center)) elt.SetAttribute("center", m_center);
            if (!String.IsNullOrEmpty(m_right)) elt.SetAttribute("right", m_right);
            elt.SetAttribute("font", CBVUtil.FontToString(m_font, m_fontColor));
            return elt;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            m_left = CBVUtil.GetStrAttrib(node, "left");
            m_center = CBVUtil.GetStrAttrib(node, "center");
            m_right = CBVUtil.GetStrAttrib(node, "right");
            String fontStr = CBVUtil.GetStrAttrib(node, "font");
            if (!String.IsNullOrEmpty(fontStr))
                m_font = CBVUtil.StringToFont(fontStr, ref m_fontColor);
        }
        //---------------------------------------------------------------------
    }
    #endregion
    #endregion
}
