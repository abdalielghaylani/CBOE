using System;
using System.Collections.Generic;
using System.Text;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Interop.ChemDrawControl11;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace ChemBioVizExcelAddIn
{
    class GraphicUtility1 :IDisposable
    {
        public GraphicUtility1()
        {
        }
        #region Member Varaibles
        private bool _Disposed;
        #endregion

        # region Public Methods
        # region Image Method

        /// <summary>
        /// Create image in temporary directory and add on excel sheet
        /// </summary>
        
        public void AddImage(Excel.Worksheet nWorksheet, object graphics, Excel.Range nRange)
        {
            byte[] bData = new byte[1023];
            bData = (byte[])graphics;

            //convert byteArray to Image
            Image img = Global.byteArrayToImage(bData);

            //string tempFileName = Path.GetTempFileName();
            //string tempImageFile = Path.GetFileNameWithoutExtension(tempFileName) + ".jpg";
            //File.Move(tempFileName, tempImageFile);

            string tempImageFile = Path.GetTempPath() + "\\" + "tempImage.jpeg";
            using (FileStream fsImage = new FileStream(tempImageFile, FileMode.OpenOrCreate))
            {
                fsImage.Write(bData, 0, bData.Length);
                fsImage.Close();
            }


            Excel::Shape shape = nWorksheet.Shapes.AddPicture(tempImageFile, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue,
(float)(double)nRange.Left, (float)(double)nRange.Top, img.Width, img.Height);
            // delete the temporary file
            if (System.IO.File.Exists(tempImageFile)) System.IO.File.Delete(tempImageFile);


            shape.Line.Visible = Office.MsoTriState.msoFalse;
            shape.Fill.Visible = Office.MsoTriState.msoFalse;

            // make original size image
            shape.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
            shape.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);

            AdjustCellAndShape(nRange, shape);
        }
#endregion
       
        
        # region Structure Method

        /// <summary>
        /// Create chem draw structrue in temporary directory and add on excel sheet
        /// </summary>
        
        public void AddChemDrawStructure(Excel.Worksheet nWorksheet, Excel.Range nRange, string base64)
        {
            string tempFile = Path.GetTempPath() + "\\" + "tempbase64.gif";
            // delete the temporary file
            if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);

            object outResoulution = 200;  //Fixed              
            object outPath = tempFile;
            object outMymeType = "gif";
            object outWidth = null; //Fixed
            object outHeight = null; //Fixed

            ChemDrawCtl ctrl = new ChemDrawCtl();
            if (!File.Exists(tempFile))
            {
                ctrl.DataEncoded = true;
                ctrl.set_Data("chemical/x-cdx", base64);
                outWidth = ctrl.Objects.Width;
                outHeight = ctrl.Objects.Height;
                ctrl.SaveAs(ref outPath, ref outMymeType, ref outResoulution, ref outWidth, ref outHeight);
            }
            Excel::Shape shape = nWorksheet.Shapes.AddPicture(tempFile, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue,
                (float)(double)nRange.Left, (float)(double)nRange.Top, Convert.ToSingle(ctrl.Objects.Width), Convert.ToSingle(ctrl.Objects.Height));

            if (ctrl != null)
                ctrl.Objects.Clear();
            
            shape.Line.Visible = Office.MsoTriState.msoFalse;
            shape.Fill.Visible = Office.MsoTriState.msoFalse;

            // make original size image
            shape.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
            shape.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
            AdjustCellAndShape(nRange, shape);

            //Image oImage = Image.FromFile(tempImageFile);
            //System.Windows.Forms.Clipboard.SetDataObject(oImage, true);
            //nWorksheet.Paste(nRange, tempImageFile);
        }
        #endregion
        #endregion



        #region Private Methods
        /// <summary>
        /// Adjust cell width and height according to the contained shape object. Shape's Left and 
        /// Top properties are set to align it with cell.
        /// </summary>
        
        private void AdjustCellAndShape(Excel.Range nRange, Excel.Shape shape)
        {
            //Excel.Font font = nRange.Font;
            //double fontsize = (double)font.Size;

            // set cell height
            float requiredHeight = shape.Height + 20; // space at the bottom for formula

            if (requiredHeight < Convert.ToSingle(nRange.RowHeight))
            {
                if (requiredHeight > 100) // we can't have cell height greater than this
                    nRange.RowHeight = shape.Height = 100;
            }
            else
            {
                if (requiredHeight > 100)
                    nRange.RowHeight = shape.Height = 100;
                else
                    nRange.RowHeight = requiredHeight;
            }

            Double requiredColumnWidth = (((6 + shape.Width + ((shape.Width - 48) / 12)) / 48) * 8.43);

            if (requiredColumnWidth > Global.MaxColumnWidth)
            {
                Global.MaxColumnWidth = requiredColumnWidth;
            }

            if (Global.MaxColumnWidth > Convert.ToSingle(nRange.ColumnWidth))
            {
                if (Global.MaxColumnWidth > 100)
                {
                    shape.Width = 100;
                    Global.MaxColumnWidth = 100;
                }
                nRange.ColumnWidth = Global.MaxColumnWidth;
            }
            else
            {
                nRange.ColumnWidth = Global.MaxColumnWidth; // reducing column width
            }

            shape.Left = Convert.ToSingle(shape.Left) + 2;
            shape.Top = Convert.ToSingle(shape.Top) + 2;
            //Marshal.ReleaseComObject(font);

        }
        #endregion

        public virtual void Dispose()
        {
            if (!_Disposed)
            {  

                GC.SuppressFinalize(this);
                _Disposed = true;
            }
        }
    }
}
