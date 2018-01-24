//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="MarvinSketchProcessHost.cs" company="PerkinElmer Inc.">
//// Copyright © 2013 PerkinElmer Inc. 
//// 940 Winter Street, Waltham, MA 02451. 
//// All rights reserved. 
//// This software is the confidential and proprietary information 
//// of PerkinElmer Inc. ("Confidential Information"). You shall not 
//// disclose such Confidential Information and may not use it in any way, 
//// absent an express written license agreement between you and PerkinElmer Inc. 
//// that authorizes such use.
//// </copyright>
//// --------------------------------------------------------------------------------------------------------------------

//namespace StructureSearchSupport
//{
//    using System;
//    using System.IO;
//    using System.Reflection;

//    /// <summary>
//    /// The <see cref="ProcessHost"/> for the Marvin Editor.
//    /// </summary>
//    ////[FriendVisible(true)]
//    internal sealed class MarvinSketchProcessHost : JavaProcessHost
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="MarvinSketchProcessHost"/> class.
//        /// </summary>
//        public MarvinSketchProcessHost()
//            : base(MarvinEditorJarFile, "MarvinEditorHost", MarvinLibLocation + @"\*")
//        {
//        }

//        /// <summary>
//        /// Gets the Marvin lib location the location of all the Marvin jar files).
//        /// </summary>
//        /// <value>The Marvin lib location.</value>
//        internal static string MarvinLibLocation
//        {
//            get
//            {
//                var progFilesx86 = Environment.GetEnvironmentVariable("PROGRAMFILES(x86)");
//                if (string.IsNullOrEmpty(progFilesx86))
//                {
//                    // We're running in 32-bit mode on x64 or x86
//                    progFilesx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
//                }

//                var result = Path.Combine(progFilesx86, @"ChemAxon\MarvinBeans\lib\");
//                return result;
//            }
//        }

//        /// <summary>
//        /// Gets the file location of the Spotfire Marvin Editor Host jar file.
//        /// </summary>
//        /// <value>The file location of the Spotfire Marvin Editor Host jar file.</value>
//        internal static string MarvinEditorJarFile
//        {
//            get
//            {
//                var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
//                var assemblyDir = Path.GetDirectoryName(assemblyPath);
//                if (assemblyDir == null)
//                {
//                    return string.Empty;
//                }
//                var marvinEditorJarFile = Path.Combine(assemblyDir, "MarvinEditorHost.jar");
//                return marvinEditorJarFile;
//            }
//        }
//    }
//}