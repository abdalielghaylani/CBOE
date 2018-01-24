//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="JavaProcessHost.cs" company="PerkinElmer Inc.">
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
//    using System.Globalization;

//    /// <summary>
//    /// A <see cref="ProcessHost"/> for java processes.
//    /// </summary>
//    ////[FriendVisible(false)]
//    internal class JavaProcessHost : ProcessHost
//    {
//        /// <summary>
//        /// The full name of the jar file to start, including the directory.
//        /// </summary>
//        private readonly string _pathToJarFile;

//        /// <summary>
//        /// The name of the java class to start in the application.
//        /// </summary>
//        private readonly string _classNameToStart;

//        /// <summary>
//        /// The list of classPaths to use. This is a semi-colon separated list.
//        /// </summary>
//        private readonly string _classPaths;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="JavaProcessHost"/> class.
//        /// </summary>
//        /// <param name="pathToJarFile">The path to the main jar file.</param>
//        /// <param name="classNameToStart">The class name to start.</param>
//        /// <param name="classPaths">The class paths to include.</param>
//        public JavaProcessHost(string pathToJarFile, string classNameToStart, params string[] classPaths)
//        {
//            _pathToJarFile = pathToJarFile;
//            _classNameToStart = classNameToStart;
//            _classPaths = string.Join(";", classPaths);
//        }

//        /// <summary>
//        /// Gets ExecutableName.
//        /// </summary>
//        protected override string ExecutableName
//        {
//            get
//            {
//                return "java";
//            }
//        }

//        /// <summary>
//        /// Gets Arguments.
//        /// </summary>
//        protected override string Arguments
//        {
//            get
//            {
//                var arguments = string.Format(CultureInfo.InvariantCulture, "-classpath \"{0};{1}\" {2}", _pathToJarFile, _classPaths, _classNameToStart);
//                return arguments;
//            }
//        }
//    }
//}