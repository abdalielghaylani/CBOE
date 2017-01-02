// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JavaHostInstance.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc.
// 100 CambridgePark Drive, Cambridge, MA 02140.
// All rights reserved.
// This software is the confidential and proprietary information
// of PerkinElmer Inc. ("Confidential Information"). You shall not
// disclose such Confidential Information and may not use it in any way,
// absent an express written license agreement between you and PerkinElmer Inc.
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.Marvin
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using log4net;
    using CBVNStructureFilterSupport.ExternalProcess;

    ////[Spotfire.Dxp.LeadDiscovery.Framework.FriendVisible(false)]
    internal class JavaHostInstance
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(JavaHostInstance));

        private static readonly string MarvinBeansJarFile = Path.Combine(MarvinSketchProcessHost.MarvinLibLocation, "MarvinBeans.jar");

        private Process process;

        #endregion

        #region Properties

        public static bool IsInstalled
        {
            get
            {
                return new FileInfo(MarvinBeansJarFile).Exists;
            }
        }

        #endregion

        #region Public Methods

        public bool Start()
        {
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            string javaHostJAR = Path.Combine(assemblyDir, "StructureControlJavaHost.jar");
            string arguments = string.Format(
                CultureInfo.CurrentCulture,
                "-classpath \"{0}\";\"{1}\" StructureControlJavaHost",
                javaHostJAR,
                MarvinBeansJarFile);
            ProcessStartInfo processStartInfo = new ProcessStartInfo("java", arguments);
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            try
            {
                this.process = Process.Start(processStartInfo);
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Failed to start java host.", e);
            }

            return false;
        }

        public Image StructureToImage(string structure, int width, int height, string hydrogenDisplayMode)
        {
            string result =
                this.Call(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "StructureToImage {0} {1} {2} {3}",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(structure)),
                        width,
                        height,
                        hydrogenDisplayMode));

            if (!string.IsNullOrEmpty(result))
            {
                return new Bitmap(new MemoryStream(Convert.FromBase64String(result)));
            }

            return null;
        }

        public string StructureToMolfile(string structure)
        {
            string result =
                this.Call(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "StructureToMolfile {0}",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(structure))));

            if (!string.IsNullOrEmpty(result))
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(result));
            }

            return null;
        }

        #endregion

        #region Methods

        private string Call(string call)
        {
            if (this.process == null)
            {
                return null;
            }

            this.process.StandardInput.WriteLine(call);
            return this.process.StandardOutput.ReadLine();
        }

        #endregion
    }
}
