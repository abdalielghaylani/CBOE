using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CBVUtilities;

namespace ChemBioViz.NET
{
    public partial class CBVPrintSetupDialog : Form
    {
        private CBVPrintingHelper m_helper;

        //---------------------------------------------------------------------
        public CBVPrintSetupDialog(CBVPrintingHelper helper)
        {
            m_helper = helper;
            InitializeComponent();
        }
        //---------------------------------------------------------------------
        private void CBVPrintSetupDialog_Load(object sender, EventArgs e)
        {
            HelperToDlg();
            OnAnyChange(sender, e);
        }
        //---------------------------------------------------------------------
        private String DimnToStr(int dim)
        {
            double d = (double)dim / 100.0;
            return d.ToString();
        }
        //---------------------------------------------------------------------
        private int StrToDimn(String s)
        {
            double d = CBVUtil.StrToDbl(s);
            double dimn = d * 100.0;
            return (int)dimn;
        }
        //---------------------------------------------------------------------
        private void HelperToDlg()
        {
            PrintDocument pd = m_helper.PrintSetupDocument;
            PageSettings ps = pd.DefaultPageSettings;
            PrinterSettings prs = pd.PrinterSettings;
            CBVPrintSettings cs = m_helper.CBVPrintSettings;

            this.Text = (m_helper is CBVFormPrintingHelper) ? "Form Page Setup" : "Grid Page Setup";

            this.leftTextBox.Text = DimnToStr(ps.Margins.Left);
            this.rightTextBox.Text = DimnToStr(ps.Margins.Right);
            this.topTextBox.Text = DimnToStr(ps.Margins.Top);
            this.bottomTextBox.Text = DimnToStr(ps.Margins.Bottom);

            foreach (PaperSource psrc in prs.PaperSources)
                sourceComboBox.Items.Add(psrc.SourceName);
            sourceComboBox.SelectedItem = ps.PaperSource.SourceName;

            foreach (PaperSize psize in prs.PaperSizes)
                sizeComboBox.Items.Add(psize.PaperName);
            sizeComboBox.SelectedItem = ps.PaperSize.PaperName;

            this.portraitRadioButton.Checked = !ps.Landscape;
            this.landscapeRadioButton.Checked = ps.Landscape;

            this.hdrLeftTextBox.Text = cs.Header.m_left;    // raw strings, not substituted
            this.hdrRightTextBox.Text = cs.Header.m_right;
            this.hdrCtrTextBox.Text = cs.Header.m_center;
            this.ftrLeftTextBox.Text = cs.Footer.m_left;
            this.ftrRightTextBox.Text = cs.Footer.m_right;
            this.ftrCtrTextBox.Text = cs.Footer.m_center;

            this.fontHdrTextBox.Text = CBVUtil.FontToString(cs.Header.Font, cs.Header.FontColor);
            this.fontFtrTextBox.Text = CBVUtil.FontToString(cs.Footer.Font, cs.Footer.FontColor);
        }
        //---------------------------------------------------------------------
        private PaperSize GetPaperSize()
        {
            foreach (PaperSize psize in m_helper.PrintSetupDocument.PrinterSettings.PaperSizes)
                if (psize.PaperName.Equals(sizeComboBox.SelectedItem.ToString()))
                    return psize;
            return new PaperSize();
        }
        //---------------------------------------------------------------------
        private PaperSource GetPaperSource()
        {
            foreach (PaperSource psrc in m_helper.PrintSetupDocument.PrinterSettings.PaperSources)
                if (psrc.SourceName.Equals(sourceComboBox.SelectedItem.ToString()))
                    return psrc;
            return new PaperSource();
        }
        //---------------------------------------------------------------------
        private void DlgToHelper()
        {
            PrintDocument pd = m_helper.PrintSetupDocument;
            PageSettings ps = pd.DefaultPageSettings;
            CBVPrintSettings cs = m_helper.CBVPrintSettings;

            ps.Margins.Left = StrToDimn(leftTextBox.Text);
            ps.Margins.Right = StrToDimn(rightTextBox.Text);
            ps.Margins.Top = StrToDimn(topTextBox.Text);
            ps.Margins.Bottom = StrToDimn(bottomTextBox.Text);
            ps.PaperSize = GetPaperSize();
            ps.PaperSource = GetPaperSource();
            ps.Landscape = this.landscapeRadioButton.Checked;

            SetHeaderFooterText(cs);
        }
        //---------------------------------------------------------------------
        private void SetHeaderFooterText(CBVPrintSettings cs)
        {
            cs.Header.Left = this.hdrLeftTextBox.Text;
            cs.Header.Right = this.hdrRightTextBox.Text;
            cs.Header.Center = this.hdrCtrTextBox.Text;
            cs.Footer.Left = this.ftrLeftTextBox.Text;
            cs.Footer.Right = this.ftrRightTextBox.Text;
            cs.Footer.Center = this.ftrCtrTextBox.Text;

            Color colorTmp = Color.Black;
            cs.Header.Font = CBVUtil.StringToFont(this.fontHdrTextBox.Text, ref colorTmp);
            cs.Header.FontColor = colorTmp;
            cs.Footer.Font = CBVUtil.StringToFont(this.fontFtrTextBox.Text, ref colorTmp);
            cs.Footer.FontColor = colorTmp;
        }
        //---------------------------------------------------------------------
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PrintDocument pd = m_helper.PrintSetupDocument;
            PageSettings ps = pd.DefaultPageSettings;
            PrinterSettings prs = pd.PrinterSettings;

            // fill window with gray
            Graphics g = e.Graphics;
            Rectangle rClient = (sender as PictureBox).ClientRectangle;
            rClient.Inflate(-1, -1);
            g.FillRectangle(new SolidBrush(Color.Gray), rClient);
            g.DrawRectangle(new Pen(Color.Black), rClient);

            // find doc size in native and client units
            PaperSize paperSize = GetPaperSize();
            Rectangle rBounds = this.landscapeRadioButton.Checked ?
                new Rectangle(0, 0, paperSize.Height, paperSize.Width) :
                new Rectangle(0, 0, paperSize.Width, paperSize.Height);
            rClient.Inflate(-8, -8);
            Rectangle rPage = CBVUtil.ScaleRect(rBounds, rClient);

            // set transform
            g.TranslateTransform(rPage.Left, rPage.Top);
            g.ScaleTransform((float)rPage.Width / (float)rBounds.Width, (float)rPage.Height / (float)rBounds.Height);
            g.FillRectangle(new SolidBrush(Color.White), rBounds);;
            g.DrawRectangle(new Pen(Color.Black), rBounds);;

            // draw border around text area within margins
            int left = StrToDimn(leftTextBox.Text), right = StrToDimn(rightTextBox.Text);
            int top = StrToDimn(topTextBox.Text), bottom = StrToDimn(bottomTextBox.Text);
            Rectangle rMargin = new Rectangle(left, top, (rBounds.Width - left - right), (rBounds.Height - top - bottom));
            g.DrawRectangle(new Pen(Color.LightGray), rMargin);

            // draw header and footer
            CBVPrintSettings cps = new CBVPrintSettings(m_helper);
            SetHeaderFooterText(cps);
            CBVPrintingHelper.DrawHeaderAndFooter(g, rMargin, rBounds, cps);

            // draw form outline(s)
            if (m_helper is CBVFormPrintingHelper)
            {
                FormPrintLayout layout = (m_helper as CBVFormPrintingHelper).PrintLayout;
                Rectangle rForm = layout.GetFormSpace();
                int formWid = rForm.Width, formHgt = rForm.Height;
                if (formWid > 0 && formHgt > 0)
                {
                    while ((top + formHgt) < rMargin.Bottom)
                    {
                        Rectangle rFormOutline = new Rectangle(left, top, formWid, formHgt);
                        g.DrawRectangle(new Pen(Color.DarkGray), rFormOutline);
                        top += formHgt;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void okbutton_Click(object sender, EventArgs e)
        {
            DlgToHelper();
            this.Hide();
        }
        //---------------------------------------------------------------------
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        //---------------------------------------------------------------------
        private void OnAnyChange(object sender, EventArgs e)
        {
            this.pictureBox1.Invalidate();
            this.pictureBox1.Update();
        }
        //---------------------------------------------------------------------
        private void DoFontDlg(CBVHeaderFooter hf, TextBox tbox)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true;
            fontDialog.Font = hf.Font;
            fontDialog.Color = hf.FontColor;
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                hf.Font = fontDialog.Font;
                hf.FontColor = fontDialog.Color;
                tbox.Text = CBVUtil.FontToString(fontDialog.Font, fontDialog.Color);
                OnAnyChange(null, null);
            }
        }
        //---------------------------------------------------------------------
        private void fontButtonHdr_Click(object sender, EventArgs e)
        {
            DoFontDlg(m_helper.CBVPrintSettings.Header, this.fontHdrTextBox);
        }
        //---------------------------------------------------------------------
        private void fontButtonFtr_Click(object sender, EventArgs e)
        {
            DoFontDlg(m_helper.CBVPrintSettings.Footer, this.fontFtrTextBox);
        }
        //---------------------------------------------------------------------
    }
}
