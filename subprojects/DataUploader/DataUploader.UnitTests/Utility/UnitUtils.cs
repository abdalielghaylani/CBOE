using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader
{
    /// <summary>
    /// Support methods for Unit Tests only. These are intended strictly as code-savers for unit testing,
    /// such as automatically generating folder and file paths for various kinds of test files.
    /// </summary>
    /// <remarks>
    /// General-purpose methods that extend beyond unit testing should NOT be located here.
    /// </remarks>
    class UnitUtils
    {
        public const string NO_DEPENDENCY_CATEGORY = "NoDependency";
        public const string DATABASE_DEPENDENCY = "DatabaseDependency";

        /// <summary>
        /// Generates a folder path to the local TestFiles folder to simplify test initializations.
        /// </summary>
        /// <returns>a string representing the parent folder for the desired test file(s)</returns>
        public static string GetTestFolderPath()
        {
            string assemblyExecutingPath = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase
            );

            if (assemblyExecutingPath.StartsWith("file:\\"))
                assemblyExecutingPath = assemblyExecutingPath.Substring(6);

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(assemblyExecutingPath);
            string filePath = di.Parent.Parent.FullName;
            filePath = System.IO.Path.Combine(filePath, "TestFiles");
            return filePath;
        }

        /// <summary>
        /// Finds the specified file in the root TestFiles folder
        /// </summary>
        /// <param name="fileName">the name and extension of the file</param>
        /// <returns>a path string for the sought file</returns>
        public static string GetTestFilePath(string fileName)
        {
            string filePath = System.IO.Path.Combine(GetTestFolderPath(), fileName);
            return filePath;
        }

        /// <summary>
        /// Finds the DataFiles folder
        /// </summary>
        /// <returns>a path string for the sought folder</returns>
        public static string GetDataFolderPath()
        {
            string folderPath = System.IO.Path.Combine(GetTestFolderPath(), "DataFiles");
            return folderPath;
        }

        /// <summary>
        /// Finds the specified data file
        /// </summary>
        /// <param name="fileName">the name and extension of the file</param>
        /// <returns>a path string for the sought file</returns>
        public static string GetDataFilePath(string fileName)
        {
            string filePath = System.IO.Path.Combine(GetDataFolderPath(), fileName);
            return filePath;
        }

        /// <summary>
        /// Finds the MappingFiles folder
        /// </summary>
        /// <returns>a path string for the sought folder</returns>
        public static string GetMappingFolderPath()
        {
            string folderPath = System.IO.Path.Combine(GetTestFolderPath(), "MappingFiles");
            return folderPath;
        }

        /// <summary>
        /// Finds the specified Mappings file
        /// </summary>
        /// <param name="fileName">the name and extension of the file</param>
        /// <returns>a path string for the sought file</returns>
        public static string GetMappingFilePath(string fileName)
        {
            string filePath = System.IO.Path.Combine(GetMappingFolderPath(), fileName);
            return filePath;
        }

        /// <summary>
        /// Fetch a simple application configuration setting from the AppSettings section of the
        /// context-specific configuration file.
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string appSettingKey)
        {
            string value = ConfigurationManager.AppSettings[appSettingKey];
            return value;
        }

        /// <summary>
        /// Provides authentication for database-enabled unit tests.
        /// </summary>
        /// <param name="login">the user's login</param>
        /// <param name="password">the user's password</param>
        /// <returns></returns>
        public static bool AuthenticateCoeUser(string login, string password)
        {
            return CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(login, password);
        }

        /// <summary>
        /// Finds the loader executable's path
        /// </summary>
        /// <returns>a path string for the loader executable</returns>
        public static string GetExecutableFilePath()
        {
            string assemblyExecutingPath = Path.GetDirectoryName(
                  Assembly.GetExecutingAssembly().GetName().CodeBase
            );

            if (assemblyExecutingPath.StartsWith("file:\\"))
                assemblyExecutingPath = assemblyExecutingPath.Substring(6);

            string executableFilePath = Path.Combine(assemblyExecutingPath, "COEDataLoader.exe");
            return executableFilePath;
        }
    }
}
