using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System;

namespace CambridgeSoft.COE.Patcher.Test
{
    public static class TestUtility
    {
        private static string _testFileFolderPath = string.Empty;

        /// <summary>
        /// Generates a folder path to the local TestFiles folder to simplify test initializations.
        /// </summary>
        /// <returns>a string representing the parent folder for the desired test file(s)</returns>
        public static string TestFileFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_testFileFolderPath))
                {
                    string assemblyExecutingPath = Path.GetDirectoryName(
                          Assembly.GetExecutingAssembly().GetName().CodeBase
                    );

                    if (assemblyExecutingPath.StartsWith("file:\\"))
                        assemblyExecutingPath = assemblyExecutingPath.Substring(6);

                    _testFileFolderPath = Path.Combine(assemblyExecutingPath, "TestFiles");

                    if (!Directory.Exists(_testFileFolderPath))
                        Directory.CreateDirectory(_testFileFolderPath);
                }

                return _testFileFolderPath;
            }
        }

        /// <summary>
        /// For testing purpose, we probably want to output the xml after processing into a file for convenient check.
        /// </summary>
        /// <param name="coeFormGroupId">The COEFormGroupId to be included in the output file name</param>
        /// <returns>An output file full name</returns>
        public static string BuildCOEFormGroupOutputFileName(string coeFormGroupId)
        {
            return Path.Combine(TestUtility.TestFileFolderPath, string.Format("CoeForms\\{0}_Upgraded.xml", coeFormGroupId));
        }

        /// <summary>
        /// For testing purpose, we probably want to output the xml after processing into a file for convenient check.
        /// </summary>
        /// <param name="coeFormGroupId">The COEDataViewId to be included in the output file name</param>
        /// <returns>An output file full name</returns>
        public static string BuildCOEDataViewOutputFileName(string coeDataViewId)
        {
            return Path.Combine(TestUtility.TestFileFolderPath, string.Format("CoeDataViews\\{0}_Upgraded.xml", coeDataViewId));
        }

        /// <summary>
        /// Generates the name of the file to store the content of COEObjectConfig after patchers
        /// are run.
        /// </summary>
        /// <returns>An output file full name</returns>
        public static string BuildCOEObjectConfigFileName()
        {
            return Path.Combine(TestUtility.TestFileFolderPath, "COEObjectConfig_Upgrade.xml");
        }

        /// <summary>
        /// Generates the name of the file to store the content of COEObjectConfig after patchers
        /// are run.
        /// </summary>
        /// <returns>An output file full name</returns>
        public static string BuildCOEFrameworkConfigFileName()
        {
            return Path.Combine(TestUtility.TestFileFolderPath, "COEFrameworkConfig_Upgrade.xml");
        }

        /// <summary>
        /// Runs the single patcher specified by the generic type parameter
        /// </summary>
        /// <typeparam name="T">The type of the Patcher class</typeparam>
        /// <param name="coeFormGroup">The COEFormGroup document to run patcher on</param>
        public static void PatchSingleCOEFormGroup<T>(XmlDocument coeFormGroup)
            where T : BugFixBaseCommand
        {
            List<XmlDocument> allCoeFormGroups = new List<XmlDocument>();
            allCoeFormGroups.Add(coeFormGroup);

            BugFixBaseCommand command = Activator.CreateInstance<T>();

            if (command == null)
                throw new Exception(string.Format("Failed to create the patcher", typeof(T).FullName));

            command.Fix(allCoeFormGroups, null, null, null, null);
        }

        /// <summary>
        /// Runs the single patcher specified by the generic type parameter
        /// </summary>
        /// <typeparam name="T">The type of the Patcher class</typeparam>
        /// <param name="coeFormGroup">The COEDataView document to run patcher on</param>
        public static void PatchSingleCOEDataView<T>(XmlDocument coeDataView)
            where T : BugFixBaseCommand
        {
            List<XmlDocument> allCoeDataViews = new List<XmlDocument>();
            allCoeDataViews.Add(coeDataView);

            BugFixBaseCommand command = Activator.CreateInstance<T>();

            if (command == null)
                throw new Exception(string.Format("Failed to create the patcher", typeof(T).FullName));

            command.Fix(null, allCoeDataViews, null, null, null);
        }

        /// <summary>
        /// Runs a single patcher.
        /// </summary>
        public static void RunSinglePatcher<T>(
            List<XmlDocument> coeFormGroups,
            List<XmlDocument> coeDataViews,
            List<XmlDocument> coeConfigurations,
            XmlDocument coeObjectConfig,
            XmlDocument coeFrameworkConfig)
            where T : BugFixBaseCommand
        {
            BugFixBaseCommand command = Activator.CreateInstance<T>();

            if (command == null)
                throw new Exception(string.Format("Failed to create the patcher", typeof(T).FullName));

            command.Fix(coeFormGroups, coeDataViews, coeConfigurations, coeObjectConfig, coeFrameworkConfig);
        }

    }
}
