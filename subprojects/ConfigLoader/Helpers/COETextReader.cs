using System;
using System.IO;
using System.Text;

namespace CambridgeSoft.COE.ConfigLoader
{
    /// <summary>
    /// Utility class covering <c>TextReader</c> and providing additional functionality
    /// </summary>
    public class COETextReader
    {
        private FileStream oFileStream = null;
        private long cFileStreamLength; // length of the file
        private string strBuffer = "";  // buffer used for efficiency
        private long lngFileStreamPosition;   // character position in the file

        public bool IsOpen
        {
            get
            {
                return (oFileStream != null);
            }
        } // IsOpen
        public long Length
        {
            get
            {
                return cFileStreamLength;
            }
        } // Length
        public long Position
        {
            get
            {
                return lngFileStreamPosition;
            }
            set
            {
                if (lngFileStreamPosition != value)
                {
                    lngFileStreamPosition = value;
                    oFileStream.Seek(lngFileStreamPosition, SeekOrigin.Begin);
                    strBuffer = "";
                }
                return;
            }
        } // Position


        public void Close()
        {
            oFileStream.Close();
            oFileStream = null;
            strBuffer = "";
            return;
        } // Close()
        public void Open(string strFilename)
        {
            if (IsOpen)
            {
                Close();
            }
            oFileStream = File.OpenRead(strFilename);
            if (oFileStream != null)
            {
                cFileStreamLength = oFileStream.Length;
                lngFileStreamPosition = 0;
                strBuffer = "";
            }
            return;
        }
        protected int ReadChunk()
        {
            return ReadString(ref strBuffer, 4096);
        }
        protected int ReadString(ref string strBuffer, int cBytes)
        {
            if (Position != oFileStream.Position)
            {
                throw new Exception("Bill, you need to fix this!");
            }
            byte[] byteBuffer = new byte[cBytes];
            int retBytes = oFileStream.Read(byteBuffer, 0, byteBuffer.Length);
            strBuffer = Encoding.ASCII.GetString(byteBuffer);
            int nNul = strBuffer.IndexOf('\0');
            if (nNul >= 0) strBuffer = strBuffer.Substring(0, nNul);
            return strBuffer.Length;
        } // ReadString()
        public string ReadLine()
        {
            string strRet = "";
            while (Position < Length)
            {
                int nEOL = strBuffer.IndexOfAny(new char[] { '\r', '\n' }); // Add ConntrolZ ???
                if (nEOL >= 0)
                {
                    char chEOL;
                    {
                        strRet += strBuffer.Substring(0, nEOL);
                        chEOL = strBuffer[nEOL];
                        nEOL++;
                        lngFileStreamPosition += (nEOL);
                        strBuffer = strBuffer.Substring(nEOL);
                    }
                    // Could have a line break right at the end of a buffer
                    if (strBuffer.Length == 0)
                    {
                        if (ReadChunk() == 0)
                        {
                            break;  // EOF
                        }
                    }
                    // Treat CR/LF and LF/CR as single entity
                    char chAt = strBuffer[0];
                    if (((chEOL == '\r') && (chAt == '\n')) || ((chEOL == '\n') && (chAt == '\r')))
                    {
                        lngFileStreamPosition += 1;
                        strBuffer = strBuffer.Substring(1);
                    }
                    break;  // found EOL
                }
                {
                    strRet += strBuffer;
                    lngFileStreamPosition += strBuffer.Length;
                    strBuffer = "";
                    if (ReadChunk() == 0)
                    {
                        break;  // EOF
                    }
                }
            } // while (lngFileStreamPosition < cFileStreamLength)
            return strRet;
        } // ReadLine()
    } // class COETextReader
}
