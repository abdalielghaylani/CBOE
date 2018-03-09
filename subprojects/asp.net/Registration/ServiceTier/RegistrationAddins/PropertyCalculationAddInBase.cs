using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Registration.Services.AddIns;
using ChemDrawControl17;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    public abstract class PropertyCalculationAddInBase
    {
        private List<Calculation> _calculations = new List<Calculation>();
        /// <summary>
        /// Lists the calculations to be performed
        /// </summary>
        public List<Calculation> Calculations
        {
            get { return _calculations; }
        }

        private string _aditionalPaths;
        /// <summary>
        /// Intended to be alternative paths for the python scripts.
        /// </summary>
        public string AditionalPaths
        {
            get { return _aditionalPaths; }
        }

        private string _cacheKey;
        /// <summary>
        /// Key for caching this add-in
        /// </summary>
        public string CacheKey
        {
            get { return _cacheKey; }
        }

        /// <summary>
        /// Applies the compound's structure to the ChemDraw control (to standardize its format).
        /// </summary>
        /// <param name="ctrl">Instance of a ChemDraw control</param>
        /// <param name="value">The structure, as a Base64 CDX string</param>
        /// <param name="clear">Indicator if whether to clear the ChemDraw palette</param>
        public void SetData(ref ChemDrawCtl ctrl, string value, bool clear)
        {
            ctrl.DataEncoded = true;
            if (clear)
                ctrl.Objects.Clear();
            ctrl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(value));
        }

        /// <summary>
        /// Loads a python script from the file system; depending on the applciation framework
        /// configuration (2-tier vs. 3-tier), this may require a directory-walking algorithm.
        /// </summary>
        /// <param name="previousDirectory">Directory in which the file might reside.</param>
        /// <param name="path">Script file name.</param>
        protected internal void LoadPyScript(string previousDirectory, string path, ref PythonCalculation pyCalc)
        {
            //an absolute path trumps everything
            if (System.Web.HttpContext.Current != null)
            {
                string currentPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                DirectoryInfo di = new DirectoryInfo(currentPath);
                Directory.SetCurrentDirectory(di.Parent.Parent.FullName);
            }
            else
            {
                if (!Path.IsPathRooted(path))
                {
                    DirectoryInfo currentPath = new DirectoryInfo(previousDirectory);
                    do
                    {
                        currentPath = currentPath.Parent;
                    } while (!currentPath.Name.StartsWith("ChemOfficeEnterprise"));

                    path = Path.Combine(currentPath.FullName, path);
                }
            }

            if (File.Exists(path))
            {
                pyCalc.PyScriptContent = File.ReadAllText(path);
                System.IO.Directory.SetCurrentDirectory(
                    path.Substring(
                        0, Math.Max(0, Math.Max(path.LastIndexOf('\\') + 1, path.LastIndexOf('/') + 1))
                    )
                );
                _aditionalPaths = System.IO.Directory.GetCurrentDirectory() + "\\";
                System.IO.Directory.SetCurrentDirectory(previousDirectory);
            }
            else
            {
                System.IO.Directory.SetCurrentDirectory(previousDirectory);
                throw new Exception(string.Format("File not found: {0}", path));
            }
        }

    }
}
