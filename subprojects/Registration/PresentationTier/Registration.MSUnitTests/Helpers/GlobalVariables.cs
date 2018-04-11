using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// Class for defining Global variables
    /// </summary>
    public static class GlobalVariables
    {
        /// <summary>
        /// Variable holds a Registry record xml file
        /// </summary>
        public static string RegistryRecordXml = @"Registration.MSUnitTests\SubmitComponent\Xml_Files\RegistryRecordXml.xml";

        /// <summary>
        /// Variable holds a Registry Number 'AB-310001'
        /// </summary>
        public static string REGNUM1 = "AB-310001";

        /// <summary>
        /// Variable holds a Registry Number 'AB-310002'
        /// </summary>
        public static string REGNUM2 = "AB-310002";

        /// <summary>
        /// Variable holds a Structure string
        /// </summary>
        public static string NAPROXEN_SMILES = "O=C([C@H](C1=CC2=CC=C(OC)C=C2C=C1)C)[O-]";

        /// <summary>
        /// Variable holds a Structure string
        /// </summary>
        public static string NAPROXEN_SUBSTRUCTURE_SMILES = "C1=CC2=CC=C(OC)C=C2C=C1";

        /// <summary>
        /// Variable holds a Structure string
        /// </summary>
        public static string New_Structure = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEABM4lMA+dZxALOdcwAMUo0AAQkIAAAAAAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEAATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMACwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQAAAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAAACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8AAAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBBcmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGADAAAAAQCEAAAAAAAAAAAAAAA0AIAABwCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAATOJTAPnWcQCznXMADFKNAASAAgAAAAACCAAAwFQA+VZyAAoAAgABADcEAQABAAAEgAQAAAAAAggAAMByAPlWcgAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADAYwAMUowACgACAAUANwQBAAEAAAWACAAAAAoAAgAHAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAJAAAACgACAAgABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgAoAAAAKAAIACQAEBgQABgAAAAUGBAACAAAACgYBAAEAAAAAAAAAAAAA";

        /// <summary>
        /// Enumerator for Registry Record Load type.
        /// ID -> Loading registry record data using Registry Record ID
        /// REGNUM -> Loading registry record data using Registry Record Number
        /// </summary>
        public enum RegistryLoadType
        {
            ID = 0,
            REGNUM = 1
        }
    }
}
