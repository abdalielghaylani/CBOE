using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Diagnostics;

using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

using FormDBLib;
using ChemControls;
using CBVControls;
using CBVUtilities;
using ChemBioViz.NET;

/// <summary>
/// Print Layout: handles box arrangement for printing forms
/// </summary>
public class FormPrintLayout
{
    private FormViewControl m_formview;
    private List<PrintBox> m_boxes;

    public FormPrintLayout(FormViewControl formview)
	{
        m_formview = formview;
        m_boxes = new List<PrintBox>();
        FormToBoxes();
	}
    //---------------------------------------------------------------------
    private void FormToBoxes()
    {
        foreach (Control c in m_formview.Controls)
        {
            // CSBR-130311: do not print hidden boxes or buttons
            if (!c.Visible)
                continue;

            PrintBox box = MakePrintBox(c);
            if (box != null)
                m_boxes.Add(box);
        }
    }
    //---------------------------------------------------------------------
    static PrintBox MakePrintBox(Control c)
    {
        PrintBox box = new PrintBox(c);
        return box;
    }
    //---------------------------------------------------------------------
    static bool IsBelow(PrintBox bSub, PrintBox box)
    {
        if (bSub.Rect.Top > box.Rect.Bottom)    // TO DO: improve
            return true;
        return false;
    }
    //---------------------------------------------------------------------
    public void RelayoutBoxes()
    {
        // if any box is expanding, move boxes below it downwards
        foreach (PrintBox box in m_boxes)
        {
            if (box.m_expandBy == 0) continue;
            foreach (PrintBox sbox in m_boxes)
            {
                if (sbox != box && IsBelow(sbox, box))
                    sbox.m_offsetBy = box.m_expandBy;
            }
        }
    }
    //---------------------------------------------------------------------
    static bool bPrintByBitmap = false;
    public void Render(Graphics g, PrintPageEventArgs e)
    {
        if (bPrintByBitmap)
        {
            Rectangle r = GetFormSpace();
            Bitmap bitmap = new Bitmap(r.Width, r.Height);
            m_formview.DrawToBitmap(bitmap, new Rectangle(0, 0, r.Width, r.Height));
            g.DrawImage(bitmap, r);
        }
        else
        {
            bool bNeedsRelayout = false;
            foreach (PrintBox box in m_boxes)
            {
                if (box.Measure(g))
                    bNeedsRelayout = true;
            }
            if (bNeedsRelayout)
                RelayoutBoxes();

            foreach (PrintBox box in m_boxes)
                box.Render(g, e);
        }
    }
    //---------------------------------------------------------------------
    public Rectangle GetFormSpace()
    {
        Rectangle r = Rectangle.Empty;
        foreach (PrintBox box in m_boxes)
        {
            //r = Rectangle.Union(r, box.Rect);
            if (r.IsEmpty)
                r = box.GetSpace();
            else
                r = Rectangle.Union(r, box.GetSpace());
        }
        return r;
   }
}
//---------------------------------------------------------------------
public class PrintBox
{
    protected Rectangle m_rect;
    protected Control m_formbox;
    public int m_expandBy;      // move bottom down by this amount
    public int m_offsetBy;      // move entire box down by this amount

    public PrintBox(Control c)
    {
        m_formbox = c;
        m_rect = new Rectangle(c.Location, c.Size);
        m_expandBy = m_offsetBy = 0;
    }
    //---------------------------------------------------------------------
    public Rectangle Rect
    {
        get { return m_rect; }
        set { m_rect = value; }
    }
    //---------------------------------------------------------------------
    public Rectangle GetSpace()
    {
        Rectangle r = m_rect;
        r.Size = new Size(r.Width, r.Height + m_expandBy);
        r.Offset(0, m_offsetBy);
        return r;
    }
    //---------------------------------------------------------------------
    public Control Control
    {
        get { return m_formbox; }
        set { m_formbox = value; }
    }
    //---------------------------------------------------------------------
    public bool Measure(Graphics g)
    {
        // return true if any box has to be enlarged
        bool ret = false;
        m_expandBy = m_offsetBy = 0;
        if (m_formbox is TextBox && (m_formbox as TextBox).Multiline == true)
        {
            Font font = m_formbox.Font;
            SizeF strSize = g.MeasureString(m_formbox.Text, font);
            if (strSize.Height > m_rect.Height)
            {
                m_expandBy = (int)strSize.Height - m_rect.Height + 1;
                ret = true;
            }
        }
        else if (m_formbox is ChemDataGrid)
        {
            ChemDataGrid cdg = m_formbox as ChemDataGrid;
            int rowHeight = cdg.DisplayLayout.Bands[0].Override.DefaultRowHeight;
            int headerHeight = rowHeight;   // approximate
            int totalRowHeight = headerHeight + cdg.RowCount * rowHeight;
            if (totalRowHeight > m_formbox.Height)
            {
                m_expandBy = totalRowHeight - m_formbox.Height;
                ret = true;
            }
        }
        return ret;
    }
    //---------------------------------------------------------------------
    public void Render(Graphics g, PrintPageEventArgs e)
    {
        // expand or move box if necessary
        Rectangle rBox = m_rect;
        if (m_expandBy > 0)
            rBox.Size = new Size(rBox.Width, rBox.Height + m_expandBy);
        else if (m_offsetBy > 0)
            rBox.Location = new Point(rBox.Location.X, rBox.Location.Y + m_offsetBy);

        // draw box outline if not label
        if (!(m_formbox is Label))
            g.DrawRectangle(new Pen(Color.LightGray), rBox);

        // draw box
        if (m_formbox is CBVChartControl || m_formbox is ChemDataGrid || m_formbox is PictureBox)
        {
            Bitmap bitmap = new Bitmap(rBox.Width, rBox.Height);
            Size origSize = m_formbox.Size;
            if (m_expandBy > 0)
                m_formbox.Size = new Size(m_formbox.Size.Width, m_formbox.Size.Height + m_expandBy);

            m_formbox.DrawToBitmap(bitmap, new Rectangle(0, 0, rBox.Width, rBox.Height));
            g.DrawImage(bitmap, rBox);

            if (m_expandBy > 0)
                m_formbox.Size = origSize;
       }
        else if (m_formbox is ChemDraw)
        {
            Metafile metafile = (m_formbox as ChemDraw).GetMetafile((m_formbox as ChemDraw).Base64);
            //Coverity Bug Fix : CID 13095 
            if (metafile != null)
            {
                Rectangle rMeta = new Rectangle(new Point(0, 0), metafile.Size), rStruct = rBox;
                int xmargin = rStruct.Width / 10, ymargin = rStruct.Height / 10;
                rStruct.Inflate(-xmargin, -ymargin);
                Rectangle rTarget = CBVUtil.ScaleRect(rMeta, rStruct);
                g.DrawImage(metafile, rTarget);
            }
        }
        else if (m_formbox is ChemFormulaBox)
        {
            ChemFormulaBox cfbox = m_formbox as ChemFormulaBox;
            String text = cfbox.Rtf;
            if (!String.IsNullOrEmpty(text))
            {
                RichTextBoxPrintCtrl rtPrintCtl = new RichTextBoxPrintCtrl();
                ControlFactory.CopyProperties(cfbox, rtPrintCtl);
                rtPrintCtl.Rtf = text;

                Rectangle rScreen = rBox;
                int smallMargin = 4;
                rScreen.Offset((int)g.Transform.OffsetX + smallMargin, (int)g.Transform.OffsetY + smallMargin);
                rtPrintCtl.PrintToRectEx(0, text.Length, e, rScreen, e.PageBounds);
            }
        }
        else
        {
            String text = m_formbox.Text;
            if (!String.IsNullOrEmpty(text))
            {
                Font font = m_formbox.Font;
                using (Brush brush = new SolidBrush(Color.Navy))
                {
                    using (StringFormat sf = (m_formbox is Label) ? new StringFormat(StringFormatFlags.NoWrap) : new StringFormat())
                    {
                        g.DrawString(text, font, brush, rBox, sf);
                    }
                }

            }
        }
    }
}
//---------------------------------------------------------------------
