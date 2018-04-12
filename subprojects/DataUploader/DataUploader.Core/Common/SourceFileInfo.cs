using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

//TODO: This should include business validation, which differs based on the source file type.

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Contains all the relevant information about the file-system file that is going to be used
    /// as a data-source for upload processing. It will be used as the constructor information
    /// for a specific file-parser instance returned by the FileParserFactory.
    /// </summary>
    [Serializable]
    public class SourceFileInfo
    {
        #region > Constructors <
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SourceFileInfo() { }

        /// <summary>
        /// This constructor provided for cases where these two data-points are sufficient for
        /// creating a file-reader instance. This is true for SDFile and CFX input file types.
        /// </summary>
        /// <param name="pathToInputFile"></param>
        /// <param name="typeOfInputFile"></param>
        public SourceFileInfo(string pathToInputFile, SourceFileType typeOfInputFile)
        {
            _fullFilePath = pathToInputFile;
            _fileType = typeOfInputFile;
        }

        /// <summary>
        /// This constructor provided for MSExcel worksheets and MSAccess tables or views.
        /// The <paramref name="tableHasHeaderRow"/> parameter will be ignored for MSAccess.
        /// </summary>
        /// <param name="pathToInputFile"></param>
        /// <param name="typeOfInputFile"></param>
        /// <param name="tableOrViewName"></param>
        /// <param name="tableHasHeaderRow"></param>
        public SourceFileInfo(
            string pathToInputFile
            , SourceFileType typeOfInputFile
            , string tableOrViewName
            , bool tableHasHeaderRow
            )
        {
            _fullFilePath = pathToInputFile;
            _fileType = typeOfInputFile;
            _tableName = tableOrViewName;
            _hasHeaderRow = tableHasHeaderRow;
        }

        #endregion


        #region File changing detection

        string _md5Checksum;

        /// <summary>
        /// Event fired when source file are changed during the data uploading process
        /// </summary>
        public event EventHandler SourceFileChanged;

        /// <summary>
        /// Repeatly detect whether the source file are changed every some milliseconds specified by the parameter.
        /// Use <see cref="SourceFileChanged"/> event along with this method to register event handler that
        /// needs to be invoked when file changing detected.
        /// </summary>
        /// <param name="milliseconds">Time span in milliseconds between each file changing detection</param>
        public void DetectFileChanging(int milliseconds)
        {
            if (string.IsNullOrEmpty(FullFilePath)) return;
            Thread checkMd5 = new Thread(
                delegate()
                {
                    ComputeMD5(milliseconds);
                });
            checkMd5.IsBackground = true;//set as background so that it will be terminated automatically
            checkMd5.Start();
        }

        /// <summary>
        /// Compute the md5 checksum of this file every some milliseconds specified by the parameter.
        /// This is intended for a thread created in the <see cref="DetectFileChanging"/> method.
        /// To detect whether source file has been changed, invoke <see cref="DetectFileChanging"/> instead.
        /// </summary>
        /// <param name="milliseconds">Time span between each md5 computing</param>
        private void ComputeMD5(int milliseconds)
        {
            while (true)
            {
                MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
                byte[] md5Bytes = md5Provider.ComputeHash(Bytes);
                StringBuilder md5Builder = new StringBuilder();
                foreach (Byte b in md5Bytes)
                {
                    md5Builder.Append(b.ToString("x2"));
                }
                string newMd5Checksum = md5Builder.ToString();
                if (string.IsNullOrEmpty(_md5Checksum)) _md5Checksum = newMd5Checksum;//compute for the first time 
                else if (_md5Checksum != newMd5Checksum)
                {
                    SourceFileChanged(this, new EventArgs());
                    Thread.CurrentThread.Abort();
                }
                System.Threading.Thread.Sleep(milliseconds);
            }
        }

        /// <summary>
        /// Get an array of bytes in the file
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                if (string.IsNullOrEmpty(FullFilePath)) return null;
                lock (SourceFileLock)
                {
                    using (FileStream stream = DerivedFileInfo.Open(FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[(int)Math.Pow(2, 16)];//buffer to store block of bytes from the file
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while (true)
                            {
                                read = stream.Read(buffer, 0, buffer.Length);
                                if (read == 0) break;//no more bytes
                                ms.Write(buffer, 0, read);
                            }
                            return ms.ToArray();
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// A lock to access the source file.
        /// You should get the lock when you need to access the source file.
        /// </summary>
        public readonly object SourceFileLock = new object();

        /// <summary>
        /// Convenience property wrapping the _fullFilePath member.
        /// </summary>
        public FileInfo DerivedFileInfo
        {
            get { return new FileInfo(_fullFilePath); }
        }

        private string _fullFilePath;
        /// <summary>
        /// universally applicable and required
        /// </summary>
        public string FullFilePath
        {
            get { return _fullFilePath; }
            set { _fullFilePath = value; }
        }

        private SourceFileType _fileType;
        /// <summary>
        /// universally applicable and required
        /// </summary>
        public SourceFileType FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

        private bool _hasHeaderRow;
        /// <summary>
        /// applicable only to CSV and Excel data-sources
        /// </summary>
        public bool HasHeaderRow
        {
            get { return _hasHeaderRow; }
            set { _hasHeaderRow = value; }
        }

        private string[] _fieldDelimiters;
        /// <summary>
        /// Applicable only to CSV data-sources for character-delimited file formats.
        /// </summary>
        public string[] FieldDelimiters
        {
            get { return _fieldDelimiters; }
            set { _fieldDelimiters = value; }
        }

        private int[] _fieldWidths;
        /// <summary>
        /// Applicable only to CSV data-sources for fixed-width file formats.
        /// </summary>
        public int[] FieldWidths
        {
            get { return _fieldWidths; }
            set { _fieldWidths = value; }
        }

        private string _tableName;
        /// <summary>
        /// The name of the MSExcel worksheet, or MSAccess table or view name, from which to
        /// read the upload data.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }
    }
}
