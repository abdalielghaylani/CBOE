using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.IO.Compression;

/// <summary>
/// Summary description for COERegistrationUtilities
/// </summary>
public class COERegistrationUtilities
{
    private COERegistrationUtilities()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public static void CleanSession(HttpSessionState Session)
    //{
    //    Session.Remove(RegistrationWebApp.Constants.CompoundsToResolve_Session);
    //    Session.Remove(RegistrationWebApp.Constants.DuplicateCompoundIdsList_Session);
    //    Session.Remove(RegistrationWebApp.Constants.DuplicateCompoundObjects_Session);
    //    Session.Remove(RegistrationWebApp.Constants.DuplicateIdsList_Session);
    //    Session.Remove(RegistrationWebApp.Constants.DuplicateMultiCompounds_Session);
    //    Session.Remove(RegistrationWebApp.Constants.MultiCompoundObject_Session);
    //}


    //public static byte [] GZipCompress(string plainText)
    //{
    //    byte[] inputBuffer = Encoding.UTF8.GetBytes(plainText);

    //    MemoryStream outputStream = new MemoryStream();
    //    GZipStream stream = new GZipStream(outputStream, CompressionMode.Compress, true);
    //    stream.Write(inputBuffer, 0, inputBuffer.Length);
    //    stream.Close();

    //    return outputStream.ToArray();
    //}

    //public static string GZipDecompress(byte [] encodedText)
    //{
    //    MemoryStream inputStream = new MemoryStream(encodedText);

    //    GZipStream stream = new GZipStream(inputStream, CompressionMode.Decompress);
    //    byte[] decompressedChunk = new byte[1000];
    //    string plainText = string.Empty;

    //    int bytesRead = 0;

    //    do
    //    {
    //        bytesRead = stream.Read(decompressedChunk, 0, decompressedChunk.Length);
    //        string xmlChunk = Encoding.UTF8.GetString(decompressedChunk, 0, bytesRead);
    //        plainText += xmlChunk;
    //    } while (bytesRead != 0);

    //    return plainText;
    //}

}
