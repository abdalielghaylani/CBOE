// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClipboardHelper.cs" company="PerkinElmer Inc.">
//   Copyright © 2011 - 2011 PerkinElmer Inc., 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.Framework
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Text;
    using System.IO;

    /// <summary>
    /// A clipboard helper. This class was initially lifted from DecisionSite (dsclient).
    /// </summary>
    internal class ClipboardHelper : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// Whether or not we own the clipboard or not.
        /// </summary>
        private bool ownsClipboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardHelper"/> class.
        /// </summary>
        public ClipboardHelper()
        {
            if (!NativeMethods.OpenClipboard(IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open clipboard.");
            }

            this.ownsClipboard = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the chemistry data.
        /// </summary>
        /// <param name="format">The Chemistry format.</param>
        /// <param name="data">The chemistry data to add to the clipboard.</param>
        public void AddChemData(DataFormats.Format format, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (DataFormats.GetFormat(ChemDataFormats.CDX) == format
            &&  data is byte[])
            {
                //if CDX,add directly
                AddData(format, data);
            }
            else if (DataFormats.GetFormat(ChemDataFormats.Smiles) == format
                   &&data is string)
            {
                //if SMILES,add 0 as the termination to the end of the array
                string SmilesString = data as string;        
                // Convert to bytes
                Encoding encoding = Encoding.GetEncoding(0);
                byte[] Smilebytes = encoding.GetBytes(SmilesString);
                //add 0 as the termination to the end of smilestring
                using (MemoryStream SmileTarget = new MemoryStream(Smilebytes.Length + 1))
                {
                    SmileTarget.Write(Smilebytes, 0, Smilebytes.Length);
                    SmileTarget.WriteByte(0);
                    Smilebytes = SmileTarget.ToArray();
                }
                //Add smile to clipboard
                AddData(format, Smilebytes);
            }
            else if (DataFormats.GetFormat(DataFormats.EnhancedMetafile) == format
                && data is byte[])
            {
                using (Metafile MetafileObj = new Metafile(new MemoryStream(data as byte[])))
                {
                    AddData(format, MetafileObj);
                }
            }
            else if (DataFormats.GetFormat(ChemDataFormats.MDLCT) == format
                &&  data is string)
            {
                string MolString = data as string;
                Encoding encoding = Encoding.GetEncoding(0);
                // Prepare Molfile data before copying to Clipboard
                byte[] Molbytes = MdlCTConverter.ConvertStringToCTData(MolString, encoding);
                //Add Molfile to clipboard
                AddData(format, Molbytes);
            }
            else if (DataFormats.GetFormat(ChemDataFormats.MDLSK) == format
                &&  data is byte[])
            {
                //if MDLSK,add directly
                AddData(format, data);
            }
            else
            {
                AddData((DataFormats.Format)format, data);
            }
        }


        protected void AddData(DataFormats.Format format, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data is string)
            {
                this.AddData(format, (string)data);
            }
            else if (data is byte[])
            {
                this.AddData(format, (byte[])data);
            }
            else if (data is Metafile)
            {
                this.AddData(format, (Metafile)data);
            }
            else if (data is Bitmap)
            {
                this.AddData(format, (Bitmap)data);
            }
            else
            {
                throw new NotSupportedException("Failed to add clipboard data, the specified data type is not supported.");
            }
        }


        protected void AddData(DataFormats.Format format, string data)
        {
            this.AddData(format, Marshal.StringToHGlobalUni(data));
        }

  
        protected void AddData(DataFormats.Format format, byte[] data)
        {
            IntPtr allocHGlobal = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, allocHGlobal, data.Length);
                this.AddData(format, allocHGlobal);
            }
            catch
            {
                Marshal.FreeHGlobal(allocHGlobal);
            }
        }


        protected void AddData(DataFormats.Format format, Metafile data)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            // Workaround: 2006-02-07, PNE
            //  The method "GetHenhmetafile" invalidates the metafile, ownership
            //  for the handle is transferred to the caller. 
            using (Metafile clone = (Metafile)data.Clone())
            {
                MetafileHeader metafileHeader = clone.GetMetafileHeader();

                if (format.Name == DataFormats.EnhancedMetafile)
                {
                    if (!metafileHeader.IsEmfOrEmfPlus())
                    {
                        throw new ArgumentException(@"Clipboard operation require an enhanced metafile.", "data");
                    }

                    IntPtr henhmetafile = data.GetHenhmetafile();
                    try
                    {
                        this.AddData(format, henhmetafile);
                    }
                    catch
                    {
                        NativeMethods.DeleteEnhMetaFile(henhmetafile);
                        throw;
                    }
                }
                else if (format.Name == DataFormats.MetafilePict)
                {
                    if (!metafileHeader.IsWmf())
                    {
                        throw new ArgumentException(@"Clipboard operation require a Windows metafile.", "data");
                    }

                    // Workaround: 2006-02-07, PNE
                    //  Apparently placable metafiles are NOT converted to enhanced metafiles.
                    //  Use legacy methods from OLE to integrate with clipboard, see MSDN KB and
                    //  Google-groups for more details. (Q113254, Q83023, Q323530)
                    NativeMethods.METAFILEPICT metafilepict = new NativeMethods.METAFILEPICT();
                    metafilepict.mm = 8; // MM_ANISOTROPIC
                    metafilepict.xExt = (int)data.PhysicalDimension.Width; // Logical width in MM_HIMETRIC
                    metafilepict.yExt = (int)data.PhysicalDimension.Height; // Logical height in MM_HIMETRIC
                    metafilepict.hMF = data.GetHenhmetafile(); // HMETAFILE (not HENHMETAFILE)
                    try
                    {
                        IntPtr allocHGlobal = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.METAFILEPICT)));
                        try
                        {
                            Marshal.StructureToPtr(metafilepict, allocHGlobal, false);
                            this.AddData(format, allocHGlobal);
                        }
                        catch
                        {
                            Marshal.FreeHGlobal(allocHGlobal);
                            throw;
                        }
                    }
                    catch
                    {
                        NativeMethods.DeleteMetaFile(metafilepict.hMF);
                        throw;
                    }
                }
                else
                {
                    throw new ArgumentException(@"Specified format is not supported for metafiles.", "format");
                }
            }
        }


        protected void AddData(DataFormats.Format format, Bitmap data)
        {
            IntPtr bitmapPointer = data.GetHbitmap();
            try
            {
                this.AddData(format, bitmapPointer);
            }
            catch
            {
                NativeMethods.DeleteObject(bitmapPointer);
                throw;
            }
        }


        protected void AddData(DataFormats.Format format, IntPtr intPtr)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            this.CheckDisposed();

            if (NativeMethods.SetClipboardData((uint)format.Id, intPtr) != intPtr)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to set clipboard data.");
            }
        }

        /// <summary>
        /// Clears the clipboard.
        /// </summary>
        public void Clear()
        {
            this.CheckDisposed();
            NativeMethods.EmptyClipboard();
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.ownsClipboard)
            {
                this.ownsClipboard = false;
                NativeMethods.CloseClipboard();
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this instance has been disposed.
        /// </summary>
        private void CheckDisposed()
        {
            if (!this.ownsClipboard)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        #endregion

        /// <summary>
        /// Native methods from Windows.
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool EmptyClipboard();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool CloseClipboard();

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteEnhMetaFile(IntPtr hemf);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteMetaFile(IntPtr hemf);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteObject(IntPtr hObject);

            /// <summary>
            /// Defines the metafile picture format used for exchanging metafile data through the clipboard.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct METAFILEPICT
            {
                /// <summary>
                /// The mapping mode in which the picture is drawn.
                /// </summary>
                public int mm;

                /// <summary>
                /// The size of the metafile picture for all modes except the MM_ISOTROPIC and MM_ANISOTROPIC modes.
                /// </summary>
                public int xExt;

                /// <summary>
                /// The size of the metafile picture for all modes except the MM_ISOTROPIC and MM_ANISOTROPIC modes.
                /// </summary>
                public int yExt;

                /// <summary>
                /// A handle to a memory metafile.
                /// </summary>
                public IntPtr hMF;
            }
        }
    }
}