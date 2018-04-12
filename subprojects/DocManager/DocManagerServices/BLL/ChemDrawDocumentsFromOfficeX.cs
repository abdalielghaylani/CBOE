using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    public class ChemDrawDocumentsFromOfficeX
    {
        #region Win32Interfaces

        [ComImport(), Guid("0000010A-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistStorage
        {
            void GetClassID([Out] out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void InitNew(IntPtr pstg);
            [PreserveSig]
            int Load(IStorage pstg);
            void Save(IntPtr pStgSave, bool fSameAsLoad);
            void SaveCompleted(IntPtr pStgNew);
            void HandsOffStorage();
        }

        [ComImport]
        [Guid("0000000b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IStorage
        {
            void CreateStream(
                /* [string][in] */ string pwcsName,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved1,
                /* [in] */ uint reserved2,
                /* [out] */ out IStream ppstm);

            void OpenStream(
                /* [string][in] */ string pwcsName,
                /* [unique][in] */ IntPtr reserved1,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved2,
                /* [out] */ out IStream ppstm);

            void CreateStorage(
                /* [string][in] */ string pwcsName,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved1,
                /* [in] */ uint reserved2,
                /* [out] */ out IStorage ppstg);

            void OpenStorage(
                /* [string][unique][in] */ string pwcsName,
                /* [unique][in] */ IStorage pstgPriority,
                /* [in] */ uint grfMode,
                /* [unique][in] */ IntPtr snbExclude,
                /* [in] */ uint reserved,
                /* [out] */ out IStorage ppstg);

            void CopyTo(
                /* [in] */ uint ciidExclude,
                /* [size_is][unique][in] */ Guid rgiidExclude, // should this be an array?
                /* [unique][in] */ IntPtr snbExclude,
                /* [unique][in] */ IStorage pstgDest);

            void MoveElementTo(
                /* [string][in] */ string pwcsName,
                /* [unique][in] */ IStorage pstgDest,
                /* [string][in] */ string pwcsNewName,
                /* [in] */ uint grfFlags);

            void Commit(
                /* [in] */ uint grfCommitFlags);

            void Revert();

            void EnumElements(
                /* [in] */ uint reserved1,
                /* [size_is][unique][in] */ IntPtr reserved2,
                /* [in] */ uint reserved3,
                /* [out] */ out IEnumSTATSTG ppenum);

            void DestroyElement(
                /* [string][in] */ string pwcsName);

            void RenameElement(
                /* [string][in] */ string pwcsOldName,
                /* [string][in] */ string pwcsNewName);

            void SetElementTimes(
                /* [string][unique][in] */ string pwcsName,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pctime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME patime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

            void SetClass(
                /* [in] */ Guid clsid);

            void SetStateBits(
                /* [in] */ uint grfStateBits,
                /* [in] */ uint grfMask);

            void Stat(
                /* [out] */ out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg,
                /* [in] */ uint grfStatFlag);

        }

        [ComImport]
        [Guid("0000000d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IEnumSTATSTG
        {
            // The user needs to allocate an STATSTG array whose size is celt.
            [PreserveSig]
            uint
                Next(
                uint celt,
                [MarshalAs(UnmanagedType.LPArray), Out]
			System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt,
                out uint pceltFetched
                );

            void Skip(uint celt);

            void Reset();

            [return: MarshalAs(UnmanagedType.Interface)]
            IEnumSTATSTG Clone();
        }

        [ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IStream
        {
            void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, uint cb, out uint pcbRead);
            void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, uint cb, out uint pcbWritten);
            void Seek(long dlibMove, uint dwOrigin, out long plibNewPosition);
            void SetSize(long libNewSize);
            void CopyTo(IStream pstm, long cb, out long pcbRead, out long pcbWritten);
            void Commit(uint grfCommitFlags);
            void Revert();
            void LockRegion(long libOffset, long cb, uint dwLockType);
            void UnlockRegion(long libOffset, long cb, uint dwLockType);
            void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, uint grfStatFlag);
            void Clone(out IStream ppstm);
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000A-0000-0000-C000-000000000046")]
        private interface ILockBytes
        {
            void ReadAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbRead);
            void WriteAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbWritten);
            void Flush();
            void SetSize(long cb);
            void LockRegion(long libOffset, long cb, int dwLockType);
            void UnlockRegion(long libOffset, long cb, int dwLockType);
            void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        }

        [Flags]
        private enum STGM : int
        {
            DIRECT = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            READ = 0x00000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            FAILIFTHERE = 0x00000000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000,
        }

        private enum STATFLAG : uint
        {
            STATFLAG_DEFAULT = 0,
            STATFLAG_NONAME = 1,
            STATFLAG_NOOPEN = 2
        }

        private enum STGTY : int
        {
            STGTY_STORAGE = 1,
            STGTY_STREAM = 2,
            STGTY_LOCKBYTES = 3,
            STGTY_PROPERTY = 4
        }

        [DllImport("ole32.dll")]
        static extern int StgOpenStorageOnILockBytes(ILockBytes plkbyt,
            IStorage pStgPriority, uint grfMode, IntPtr snbEnclude, uint reserved,
            out IStorage ppstgOpen);

        [DllImport("ole32.dll",
                EntryPoint = "CreateILockBytesOnHGlobal",
                ExactSpelling = true, PreserveSig = true, CharSet = CharSet.Ansi,
                CallingConvention = CallingConvention.StdCall)]
        static extern int CreateILockBytesOnHGlobal(IntPtr /* HGLOBAL */ hGlobal, bool fDeleteOnRelease, [MarshalAs(UnmanagedType.Interface)]out ILockBytes ppLkbyt);

        [DllImport("ole32.dll")]
        private static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

        [DllImport("ole32.dll")]
        private static extern int StgIsStorageILockBytes(ILockBytes plkbyt);

        [DllImport("ole32.dll")]
        static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            IStorage pstgPriority,
            STGM grfMode,
            IntPtr snbExclude,
            uint reserved,
            out IStorage ppstgOpen);

        #endregion Win32Interfaces

        #region Public Members

        /// <summary>
        /// Extract chemical objects embedded in a .docx, .xlsx, or .pptx file.
        /// </summary>
        /// <param name="filename">Full path to original file.</param>
        /// <returns>The extracted chemical objects, as a List of byte arrays.</returns>
        public static List<byte[]> GetChemicalObjects(string filename)
        {
            try
            {
                List<byte[]> ret = null;

                if (StgIsStorageFile(filename) == 0)
                {
                    IStorage storage = null;
                    int hr = 0;
                    hr = StgOpenStorage(filename, null, STGM.READ | STGM.SHARE_DENY_NONE | STGM.TRANSACTED, IntPtr.Zero, 0, out storage);
                    if (hr != 0)
                        return null;

                    ret = GetChemicalObjects(storage);

                    storage = null;
                }
                else
                {
                    using (Package package = Package.Open(filename))
                    {
                        ret = GetChemicalObjects(package);
                    }
                }

                return ret;
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }

        /// <summary>
        /// Extract chemical objects embedded in a .docx, .xlsx, or .pptx file.
        /// </summary>
        /// <param name="stream">Original file as a Stream.</param>
        /// <returns>The extracted chemical objects, as a List of byte arrays.</returns>
        public static List<byte[]> GetChemicalObjects(Stream stream)
        {
            List<byte[]> result = null;

            try
            {
                try
                {
                    Package package = Package.Open(stream);

                    result = GetChemicalObjects(package);
                }
                catch (Exception /*ex*/)
                {
                    // fall through
                }

                if (result == null || result.Count == 0)
                {
                    BinaryReader b = new BinaryReader(stream);
                    byte[] bytes = b.ReadBytes((int)stream.Length);
                    result = ExtractAllChemistryFromDocument(bytes);
                }
            }
            catch (Exception /*ex*/)
            {
            }

            return result;
        }

        /// <summary>
        /// Extract chemical objects embedded in a .docx, .xlsx, or .pptx file.
        /// </summary>
        /// <param name="bytes">Original file as a byte array.</param>
        /// <returns>The extracted chemical objects, as a List of byte arrays.</returns>
        public static List<byte[]> GetChemicalObjects(byte[] bytes)
        {
            try
            {
                List<byte[]> result = null;
                try
                {
                    MemoryStream memoryStream = new MemoryStream(bytes);
                    Package package = Package.Open(memoryStream);

                    result = GetChemicalObjects(package);
                }
                catch (Exception /*ex*/)
                {
                    // fall through
                }

                if (result == null || result.Count == 0)
                    result = ExtractAllChemistryFromDocument(bytes);

                return result;
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }

        #endregion Public Members

        #region Private Members

        /// <summary>
        /// Extract chemical objects embedded in a .docx, .xlsx, or .pptx file.
        /// Additionally extract chemical objects from the comment field in .xlsx worksheet cells, as stored by ChemDraw for Excel.
        /// </summary>
        /// <param name="bytes">Original file as a Package.</param>
        /// <returns>The extracted chemical objects, as a List of byte arrays.</returns>
        private static List<byte[]> GetChemicalObjects(Package package)
        {
            List<byte[]> documents = new List<byte[]>();
            foreach (PackagePart part in package.GetParts())
            {
                if (part.Uri.OriginalString.Contains("/embeddings/"))
                {
                    // Embedded OLE objects, as created with normal copy-and-paste operations.
                    try
                    {
                        Stream s = part.GetStream(FileMode.Open, FileAccess.Read);
                        BinaryReader b = new BinaryReader(s);
                        byte[] embeddedBytes = b.ReadBytes((int)s.Length);
                        byte[] chemistryResult = ExtractOneChemistryFromEmbedding(embeddedBytes);
                        if (chemistryResult != null)
                            documents.Add(chemistryResult);

                        // Debugging code to write out the extracted chemistry object that was just found
                        //using (FileStream fileStream = new FileStream("C:\\Documents and Settings\\jsb\\Desktop\\New Folder\\temp.cdx", FileMode.Create, FileAccess.Write))
                        //{
                        //    BinaryWriter fileWriter = new BinaryWriter(fileStream);
                        //    fileWriter.Write(chemistryResult, 0, chemistryResult.Length);
                        //}
                    }
                    catch (Exception /*ex*/)
                    {
                    }
                }
                else if (part.Uri.OriginalString.Contains("/comments"))
                {
                    // Comment fields in .xlsx worksheet cells, as stored by ChemDraw for Excel.
                    try
                    {
                        // Read the part into a char array instead of a byte array, for ease of decoding the Base64.
                        Stream s = part.GetStream(FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(s, System.Text.Encoding.ASCII);
                        BinaryReader b = new BinaryReader(sr.BaseStream);
                        char[] embeddedChars = b.ReadChars((int)s.Length);

                        // The maximum length excludes the opening and closing tags.
                        int maxLen = embeddedChars.Length - 11;

                        int i = 0;
                        while (i < maxLen)
                        {
                            // Opening tag may be <t> or <x:t>
                            if ((embeddedChars[i] == '<' || embeddedChars[i] == ':') && embeddedChars[i + 1] == 't' && embeddedChars[i + 2] == '>')
                            {
                                int j = i + 3;
                                while (j < maxLen)
                                {
                                    // Closing tag may be </t> or </x:t>
                                    if ((embeddedChars[i] == '<' && embeddedChars[j + 1] == '/' && embeddedChars[j + 2] == 't' && embeddedChars[j + 3] == '>')
                                        || (embeddedChars[i] == '<' && embeddedChars[j + 1] == '/' && embeddedChars[j + 2] == 'X' && embeddedChars[j + 3] == ':' && embeddedChars[j + 4] == 't' && embeddedChars[j + 5] == '>'))
                                    {
                                        try
                                        {
                                            byte[] commentBytes = Convert.FromBase64CharArray(embeddedChars, i + 3, j - i - 3);
                                            byte[] cdxBytes = ExtractCDXFromExcelComment(commentBytes);
                                            if (cdxBytes != null && cdxBytes.Length > 0)
                                                documents.Add(cdxBytes);

                                            // Debugging code to write out the extracted chemistry object that was just found
                                            //using (FileStream fileStream = new FileStream("C:\\Documents and Settings\\jsb\\Desktop\\New Folder\\temp.cdx", FileMode.Create, FileAccess.Write))
                                            //{
                                            //    BinaryWriter fileWriter = new BinaryWriter(fileStream);
                                            //    fileWriter.Write(cdxBytes, 0, cdxBytes.Length);
                                            //}
                                        }
                                        catch { }

                                        // Start looking for the next tag at the end of this tag.
                                        i = j + 3;

                                        break;
                                    }
                                    ++j;
                                }
                            }
                            ++i;
                        };
                    }
                    catch (Exception /*ex*/)
                    {
                    }
                }
            }
            return documents;
        }

        /// <summary>
        /// Extract chemical objects embedded in a .doc, .xls, or .ppt file.
        /// Additionally extract chemical objects from the comment field in .xlsx worksheet cells, as stored by ChemDraw for Excel.
        /// </summary>
        /// <param name="bytes">Original file as an IStorage.</param>
        /// <returns>The extracted chemical objects, as a List of byte arrays.</returns>
        private static List<byte[]> GetChemicalObjects(IStorage storage)
        {
            List<byte[]> documents = new List<byte[]>();

            try
            {
                // Embedded OLE objects, as created with normal copy-and-paste operations.

                IStorage objectPoolStorage = null;
                storage.OpenStorage("ObjectPool", null, (uint)STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0, out objectPoolStorage);
                if (objectPoolStorage == null)
                    return null;

                System.Runtime.InteropServices.ComTypes.STATSTG statstg;
                objectPoolStorage.Stat(out statstg, (uint)STATFLAG.STATFLAG_DEFAULT);

                IEnumSTATSTG enumOPStatStg = null;
                objectPoolStorage.EnumElements(0, IntPtr.Zero, 0, out enumOPStatStg);
                if (enumOPStatStg == null)
                    return null;

                System.Runtime.InteropServices.ComTypes.STATSTG[] regelt = { statstg };
                int hr = 0;
                while (hr == 0)
                {
                    uint fetched = 0;
                    hr = (int)enumOPStatStg.Next(1, regelt, out fetched);
                    if (hr == 0)
                    {
                        if (regelt[0].type == (int)STGTY.STGTY_STORAGE)
                        {
                            try
                            {
                                IStorage elementStorage = null;
                                objectPoolStorage.OpenStorage(regelt[0].pwcsName, null, (uint)(STGM.READ | STGM.SHARE_EXCLUSIVE), IntPtr.Zero, 0, out elementStorage);
                                if (elementStorage == null)
                                    continue;

                                byte[] chemistryResult = ExtractOneChemistryFromStorage(elementStorage);
                                if (chemistryResult != null)
                                    documents.Add(chemistryResult);
                            }
                            catch (Exception /*ex*/)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch { }


            // Comment fields in .xls worksheet cells, as stored by ChemDraw for Excel.
            ExtractChemistryFromExcelComments(storage, ref documents);

            return documents;
        }

        /// <summary>
        /// Extract the raw ChemDraw or ISIS data from an OLE embedded object.
        /// </summary>
        /// <param name="embeddedBytes">Raw bytes of the OLE embedded object.</param>
        /// <returns>The extracted ChemDraw or ISIS data, or null if none is found.</returns>
        private static byte[] ExtractOneChemistryFromEmbedding(byte[] embeddedBytes)
        {
            byte[] result = null;

            // We're going to call Win32 APIs, so we need our embedded bytes in an unmanaged pointer.
            // Make sure to dealloc this pointer!
            IntPtr unmData = Marshal.AllocHGlobal(embeddedBytes.Length);

            try
            {
                // Copy the embedded bytes to an unmanaged pointer, then use Win32 calls
                // to open an IStorage from that pointer.
                Marshal.Copy(embeddedBytes, 0, unmData, embeddedBytes.Length);

                ILockBytes iLockBytes = null;
                int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out iLockBytes);
                UIntPtr written;
                iLockBytes.WriteAt(0, unmData, embeddedBytes.Length, out written);

                System.Runtime.InteropServices.ComTypes.STATSTG st;
                iLockBytes.Stat(out st, 0);

                uint grfMode = (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE);
                IStorage storage = null;
                hr = StgOpenStorageOnILockBytes(iLockBytes, null, grfMode, IntPtr.Zero, 0, out storage);

                result = ExtractOneChemistryFromStorage(storage);
            }
            finally
            {
                // We MUST free the unmanaged data pointer that we allocated.
                Marshal.FreeHGlobal(unmData);
            }

            return result;
        }

        /// <summary>
        /// Extract the raw ChemDraw or ISIS data from a compound document (.doc, etc).
        /// </summary>
        /// <param name="embeddedBytes">Raw bytes of the compound document.</param>
        /// <returns>The extracted ChemDraw or ISIS data, or null if none is found.</returns>
        private static List<byte[]> ExtractAllChemistryFromDocument(byte[] embeddedBytes)
        {
            List<byte[]> result = null;

            // We're going to call Win32 APIs, so we need our embedded bytes in an unmanaged pointer.
            // Make sure to dealloc this pointer!
            IntPtr unmData = Marshal.AllocHGlobal(embeddedBytes.Length);

            try
            {
                // Copy the embedded bytes to an unmanaged pointer, then use Win32 calls
                // to open an IStorage from that pointer.
                Marshal.Copy(embeddedBytes, 0, unmData, embeddedBytes.Length);

                ILockBytes iLockBytes = null;
                int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out iLockBytes);
                UIntPtr written;
                iLockBytes.WriteAt(0, unmData, embeddedBytes.Length, out written);

                System.Runtime.InteropServices.ComTypes.STATSTG st;
                iLockBytes.Stat(out st, 0);

                uint grfMode = (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE);
                IStorage storage = null;
                hr = StgOpenStorageOnILockBytes(iLockBytes, null, grfMode, IntPtr.Zero, 0, out storage);

                result = GetChemicalObjects(storage);
            }
            finally
            {
                // We MUST free the unmanaged data pointer that we allocated.
                Marshal.FreeHGlobal(unmData);
            }

            return result;
        }

        /// <summary>
        /// Extract the raw ChemDraw or ISIS data from an IStream.
        /// </summary>
        /// <param name="stream">IStream containing the embedded.</param>
        /// <returns>The extracted ChemDraw or ISIS data, or null if none is found.</returns>
        private static byte[] ExtractOneChemistryFromStorage(IStorage storage)
        {
            // Identify a stream that potentially contains ChemDraw or ISIS data, based on the 
            // hardcoded names used by those applications for their streams.
            IStream stream = null;
            bool isCDX = false, isSKC = false;
            try
            {
                // First try ChemDraw native format
                storage.OpenStream("CONTENTS", IntPtr.Zero, (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, out stream);
                isCDX = true;
            }
            catch
            {
                // If that fails, try ISIS native format
                storage.OpenStream("\u0001Ole10Native", IntPtr.Zero, (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, out stream);
                isSKC = true;
            }
            if (!isCDX && !isSKC)
                return null;

            return ExtractOneChemistryFromStream(stream, isSKC);
        }

        /// <summary>
        /// Extract the raw ChemDraw or ISIS data from an IStream.
        /// </summary>
        /// <param name="stream">IStream containing the embedded.</param>
        /// <returns>The extracted ChemDraw or ISIS data, or null if none is found.</returns>
        private static byte[] ExtractOneChemistryFromStream(IStream stream, bool isSKC)
        {
            byte[] result = null;

            // Read the ChemDraw or ISIS data out of the stream, using (arbitrarily) a 10,000-byte buffer to do so.
            byte[] buffer = new byte[10000];
            while (true)
            {
                uint nread = 0;
                stream.Read(buffer, (uint)buffer.Length, out nread);
                if (nread > 0)
                {
                    int earlierLen = (result == null ? 0 : result.Length);

                    int newStart = 0;
                    uint newLen = nread;
                    if (isSKC && earlierLen == 0)
                    {
                        // ISIS Data always has a 4-byte header at the very beginning, and the three bytes after the header are identifiable.
                        if (newLen > 8 && buffer[4] == '\u0001' && buffer[5] == '\u0003' && buffer[6] == '\u0000')
                        {
                            newStart += 4;
                            newLen -= 4;
                        }
                        else
                        {
                            break;
                        }
                    }

                    byte[] b2 = new byte[earlierLen + nread];
                    if (earlierLen > 0)
                        Buffer.BlockCopy(result, 0, b2, 0, earlierLen);
                    Buffer.BlockCopy(buffer, newStart, b2, earlierLen, (int)newLen);
                    result = b2;
                }

                if (nread < 10000)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Extract the raw ChemDraw data from comment fields in .xls worksheet cells, as stored by ChemDraw for Excel.
        /// 
        /// This approach is really gross.  Rather than trying to decode the (undocumented?) Microsoft Excel storage format,
        /// this relies on the fact that ChemDraw for Excel stores its data as Base64-formatted comments, and that the comments
        /// are always fairly large if they store CDX data.  The entire stream is searched for character blocks that 
        /// might possibly correspond to valid Base64 strings, and each of those is Base64-decoded in turn.  If the Base64
        /// decoding is successful, and the decoded string contains two pipe characters ('|') as required by ChemDraw for Excel,
        /// then it is assumed that the bytes following the second pipe character must correspond to a valid CDX document.
        /// </summary>
        /// <param name="storage">IStorage containing the embedded.</param>
        /// <param name="documents">A list of ChemDraw documents, to which any new comments are appended.</param>
        private static void ExtractChemistryFromExcelComments(IStorage storage, ref List<byte[]> documents)
        {
            // Try to identify a workbook stream.  This is stored under two possible names, depending on the version of Excel.
            IStream workbookStream = null;
            try
            {
                storage.OpenStream("Workbook", IntPtr.Zero, (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, out workbookStream);
            }
            catch { }
            try
            {
                if (workbookStream == null)
                    storage.OpenStream("Book", IntPtr.Zero, (uint)(STGM.READ | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, out workbookStream);
            }
            catch { }
            if (workbookStream == null)
                return;

            byte[] streamBytes = null;

            // Read the whole stream, using (arbitrarily) a 100,000-byte buffer to do so.
            const int BUFFERSIZE = 100000;
            byte[] buffer = new byte[BUFFERSIZE];
            while (true)
            {
                uint nread = 0;
                workbookStream.Read(buffer, (uint)buffer.Length, out nread);
                if (nread > 0)
                {
                    int earlierLen = (streamBytes == null ? 0 : streamBytes.Length);
                    byte[] b2 = new byte[earlierLen + nread];
                    if (earlierLen > 0)
                        Buffer.BlockCopy(streamBytes, 0, b2, 0, earlierLen);
                    Buffer.BlockCopy(buffer, 0, b2, earlierLen, (int)nread);
                    streamBytes = b2;
                }

                if (nread < BUFFERSIZE)
                    break;
            }

            // Use an arbitrary cutoff -- discard very small blocks that couldn't possibly encode a CDX blob.
            const int SMALLESTPOSSIBLECDXOFINTEREST = 30;

            // Loop through the entire contents of the file, looking for chunks of data above the minimim size
            // that might encode an interesting CDX.
            for (int i = 0; i < streamBytes.Length; )
            {
                if (IsValidBase64Byte(streamBytes[i]))
                {
                    int j = i + 1;
                    while (j < streamBytes.Length && IsValidBase64Byte(streamBytes[j]))
                        ++j;

                    if (j - i > SMALLESTPOSSIBLECDXOFINTEREST)
                    {
                        if (j < streamBytes.Length - 1 && streamBytes[j] == '=' && streamBytes[j + 1] == '=')
                            j += 2;
                        else if (j < streamBytes.Length && streamBytes[j] == '=')
                            j += 1;

                        try
                        {
                            string commentStr = System.Text.Encoding.ASCII.GetString(streamBytes, i, j - i);
                            byte[] commentBytes = Convert.FromBase64String(commentStr);
                            byte[] cdxBytes = ExtractCDXFromExcelComment(commentBytes);
                            if (cdxBytes != null)
                                documents.Add(cdxBytes);
                        }
                        catch { }
                    }

                    // Start looking for the next block after the end of this block.
                    i = j;
                }
                ++i;
            }
        }

        /// <summary>
        /// Identify whether a given byte is valid within a Base64 string.  
        /// Does not recognize the equals sign that may appear at the very end of a Base64 string.
        /// </summary>
        /// <param name="b">A byte.</param>
        /// <returns>Whether a given byte is valid within a Base64 string.</returns>
        private static bool IsValidBase64Byte(byte b)
        {
            return ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') || (b >= '0' && b <= '9') || b == '+' || b == '/');
        }

        /// <summary>
        /// Extract CDX data from a byte array potentially representing a cell comment created by ChemDraw for Excel.
        /// ChemDraw for Excel cell comments consist of three fields separated by pipe characters ('|'), where the first
        /// two fields may be empty and where the third field contains Base64-encoded CDX data.  
        /// If two pipe characters are found, and if the third field can be successfully Base64-decoded, then it is assumed
        /// that this really was a ChemDraw for Excel cell comment.  No further validation is performed on the decoded third field.
        /// </summary>
        /// <param name="commentBytes">A cell comment potentially created by ChemDraw for Excel.</param>
        /// <returns>A byte array containing raw CDX data, if such could be identified, or null.</returns>
        private static byte[] ExtractCDXFromExcelComment(byte[] commentBytes)
        {
            try
            {
                int numPipes = 0;
                for (int i = 0; i < commentBytes.Length; ++i)
                {
                    if (commentBytes[i] == '|' && ++numPipes == 2)
                    {
                        // If two pipe characters are found, see if the rest of the input array can be Base64 decoded.
                        string cdxString = System.Text.Encoding.ASCII.GetString(commentBytes, i + 1, commentBytes.Length - i - 1);
                        byte[] cdxBytes = Convert.FromBase64String(cdxString);
                        if (cdxBytes != null && cdxBytes.Length > 0)
                            return cdxBytes;
                    }
                }
            }
            catch { }

            return null;
        }

        #endregion Private Members

    }
}
