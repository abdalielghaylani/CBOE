using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core
{
    /// <summary>
    /// Test SourceFileInfo class
    /// </summary>
    [TestFixture]
    public class SourceFileInfoTest
    {
        /// <summary>
        /// Check the functionality of SourceFileInfo.DetectFileChanging(int milliseconds) method.
        /// Nomally,it fire the SourceFileChanged event on a SourceFileInfo object if it detects the source file has been changed.
        /// This test fails if the event are not fired after changing the source file.
        /// </summary>
        [Test]
        public void Test_DetectFileChanging()
        {
            //Temporarily create a new source file.
            string sourceFilePath = UnitUtils.GetDataFilePath("Test_DetectFileCanging.sdf");
            using (FileStream file = new FileStream(sourceFilePath,FileMode.Create,FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("This is a source file");
                    sw.Flush();
                }
            }

            SourceFileInfo sourceFileInfo = new SourceFileInfo(sourceFilePath, SourceFileType.SDFile);
            bool eventFired = false;//A value indicating whether the event are fired after source file changed.
            sourceFileInfo.SourceFileChanged +=
                delegate(Object sender, EventArgs e)
                {
                    eventFired = true;//Set handlerCalled to true in the handler.
                };
            sourceFileInfo.DetectFileChanging(10);
            Thread.Sleep(20);//let the file changing detection thread compute the MD5 of source file for the first time.

            byte original;
            //change the source file
            lock (sourceFileInfo.SourceFileLock)
            {
                using (FileStream stream = sourceFileInfo.DerivedFileInfo.Open(FileMode.Open, FileAccess.ReadWrite))
                {
                    //backup first byte
                    original = (byte)stream.ReadByte();
                    //change first byte
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.WriteByte((byte)(original + 1));
                    stream.Flush();
                }
            }

            Thread.Sleep(20);//let the file changing detection thread run
            File.Delete(sourceFilePath);//delete the temporary file

            //fail if event has not been fired after source file changed.
            if (!eventFired) Assert.Fail("The SourceFileChanged event should be fired after changing source file.");
        }
    }
}
