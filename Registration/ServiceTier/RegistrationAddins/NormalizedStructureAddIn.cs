using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Registration.Services.RegistrationAddins.localhost1;
using ChemDrawControl15;



namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    public class NormalizedStructureAddIn : IAddIn
    {

        #region Properties
        string _currentScript;
        string _aditionalPaths;
        string _pythonWebServiceURL;
        private string _structuresIdsToAvoid = string.Empty;
        private string _logPropertyName = string.Empty;

        //configuration values that allow turning off addin based on a value
        private RegAddInsUtilities.PropertyListType _propertyListType = RegAddInsUtilities.PropertyListType.NotSet;
        private string[] _disableAddinValueArray = null;
        private string _propertyName = string.Empty;
       

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("NormalizedStructureAddIn");

        #endregion

        #region Methods

        /// <summary>
        /// Uses the PyEngine assembly (a managed COM wrapper) to apply the script to the
        /// contents of each IRegistryRecord's structure.
        /// </summary>
        /// <param name="record">An IregistryRecord instance.</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected void NormalizeStructuresEngine(IRegistryRecord record, bool isTemp)
        {
            using (CambridgeSoft.ChemScript.PyEngineNet pythonEngine = new CambridgeSoft.ChemScript.PyEngineNet())
            {
                foreach (Component component in record.ComponentList)
                {
                    //if (component.Compound.BaseFragment.Structure.IsTemporary)
                    {
                        if (!string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.Value)
                            && !this.IsAnException(component.Compound.BaseFragment.Structure.ID.ToString())
                            && component.Compound.BaseFragment.Structure.DrawingType == DrawingType.Chemical
                        )
                        {
                            ChemDrawCtl ctrl = new ChemDrawCtl();
                            string originalStructureValue = component.Compound.BaseFragment.Structure.Value;
                            RegAddInsUtilities.ChemDrawWaitOnce();
                            ctrl.Objects.Clear();
                            ctrl.DataEncoded = true;
                            ctrl.Objects.set_Data(
                                "chemical/x-cdx", null, null, null
                                , UnicodeEncoding.ASCII.GetBytes(originalStructureValue)
                            );

                            string executionAudit = string.Empty;
                            try
                            {
                                //For normalization, the python engine accepts an XML representation
                                //  of the ChemDraw structure
                                string cdxXml = ctrl.get_Data("cdxml").ToString();
                                pythonEngine.SetVar("cdx", cdxXml);
                                if (!String.IsNullOrEmpty(_aditionalPaths))
                                {
                                    pythonEngine.SetVar("scriptsPath", _aditionalPaths);
                                }

                                //Trap all output from the python script execution
                                bool isError = false;

                                try { isError = !pythonEngine.Execute(_currentScript.Replace("\r\n", "\n")); }
                                catch(AccessViolationException) { isError = !pythonEngine.Execute(_currentScript.Replace("\r\n", "\n")); }

                                if (isError)
                                {
                                    executionAudit = pythonEngine.GetError();
                                }
                                else
                                {
                                    executionAudit = pythonEngine.GetVar("logstring");
                                    if (!string.IsNullOrEmpty(_logPropertyName) && !string.IsNullOrEmpty(executionAudit))
                                    {
                                        if (component.Compound.PropertyList[_logPropertyName.ToUpper()] != null)
                                            component.Compound.PropertyList[_logPropertyName.ToUpper()].Value = executionAudit;
                                    }

                                    //Get the normalized structure from the python engine
                                    //If it exists, pass it through the ChemDraw control again to convert it back to CDX
                                    //  otherwise put the raw structure value into the normalized structure field.
                                    string nb64 = pythonEngine.GetVar("normalizedBase64").Trim();
                                    string structureToReformat = string.Empty;
                                    if (!string.IsNullOrEmpty(nb64))
                                        structureToReformat = nb64;
                                    else
                                        structureToReformat = originalStructureValue;

                                    ctrl.Objects.Clear();
                                    ctrl.DataEncoded = true;
                                    ctrl.Objects.set_Data(
                                        "chemical/x-cdx", null, null, null
                                        , UnicodeEncoding.ASCII.GetBytes(structureToReformat)
                                    );
                                    if (ctrl.get_Data("cdx").ToString().Length > 0)
                                    {
                                        if (isTemp)
                                        {
                                            component.Compound.BaseFragment.Structure.UseNormalizedStructure = true;
                                            component.Compound.BaseFragment.Structure.NormalizedStructure = ctrl.get_Data("cdx").ToString();
                                            
                                        }
                                        else
                                        { //once a registry is permanent we always act on the structure.value, but store the last
                                            component.Compound.BaseFragment.Structure.UseNormalizedStructure = false;
                                            component.Compound.BaseFragment.Structure.Value = ctrl.get_Data("cdx").ToString();


                                        }
                                    }
                                    else
                                    {
                                        component.Compound.BaseFragment.Structure.UseNormalizedStructure = false;
                                    }
                                    
                                }
                            }
                            catch (Exception exception)
                            {
                                executionAudit = exception.Message;
                            }
                            finally
                            {
                                //TODO: Ensure the this is actually being saved with the Structure object.
                                component.Compound.BaseFragment.Structure.NormalizationLog = executionAudit;
                                RegAddInsUtilities.ChemDrawReleaseMutex();
                                _coeLog.Log(executionAudit);
                            }
                        }
                    }
                }
            }
        }

        

        /// <summary>
        /// Uses the Python web service (via the configured URL) to apply the script to the
        /// contents of each IRegistryRecord's structure.
        /// </summary>
        /// <param name="record">An IregistryRecord instance.</param>
        private void NormalizeStructuresWebService(IRegistryRecord record)
        {
            ChemDrawCtl ctrl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            Service pythonWebServiceProxy = null;
            foreach (Component component in record.ComponentList)
            {
                try
                {
                    if (component.Compound.BaseFragment.Structure.IsTemporary)
                    {
                        if (!string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.Value) && !this.IsAnException(component.Compound.BaseFragment.Structure.ID.ToString()))
                        {
                            if (pythonWebServiceProxy == null)
                            {
                                pythonWebServiceProxy = RegAddInsUtilities.GetPythonWebService(_pythonWebServiceURL);
                            }
                            ctrl.Objects.Clear();
                            ctrl.DataEncoded = true;
                            ctrl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(component.Compound.BaseFragment.Structure.Value));
                            string[] inputvars = new string[] { "cdx", "scriptsPath" };
                            string[] inputs = new string[] { ctrl.get_Data("cdxml").ToString(), _aditionalPaths };
                            string[] outputvars = new string[] { "normalizedBase64", "logstring" };

                            string outputs;
                            string errors;
                            string executionAudit = string.Empty;

                            int n = pythonWebServiceProxy.SingleExecute(_currentScript, inputvars, inputs, outputvars, out outputs, out errors);
                            if (string.IsNullOrEmpty(errors))
                            {
                                if (outputs != null)
                                {
                                    string[] outValues = outputs.Split(new String[] { "|||||" }, StringSplitOptions.None);

                                    //Store the Log output into a property for further reviews.
                                    executionAudit = outValues[1];

                                    if (!string.IsNullOrEmpty(_logPropertyName) && !string.IsNullOrEmpty((string)outValues[1]))
                                    {
                                        if (component.Compound.PropertyList[_logPropertyName.ToUpper()] != null)
                                            component.Compound.PropertyList[_logPropertyName.ToUpper()].Value = (string)outValues[1];
                                    }
                                    if (!string.IsNullOrEmpty(outValues[0].Trim())) //No need to set empty data as a normalized structure.
                                    {
                                        ctrl.set_Data("cdxml", outValues[0]);
                                        component.Compound.BaseFragment.Structure.NormalizedStructure = ctrl.get_Data("cdx").ToString();
                                    }
                                }
                            }
                            else
                            {
                                executionAudit = errors;
                            }
                            _coeLog.Log(executionAudit);

                        }
                    }
                }
                catch (Exception exception)
                {
                    component.Compound.BaseFragment.Structure.NormalizationLog = exception.Message;
                }
                finally 
                    {
                        RegAddInsUtilities.ChemDrawReleaseMutex();
                    }
            }
        }

        /// <summary>
        /// Applies the compound's structure to the ChemDraw control (to standardize its format).
        /// </summary>
        /// <param name="ctrl">Instance of a ChemDraw control</param>
        /// <param name="value">The structure, as a Base64 CDX string</param>
        /// <param name="clear">Indicator if whether to clear the ChemDraw palette</param>
        private void SetData(ref ChemDrawCtl ctrl, string value, bool clear)
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
        private void LoadPyScript(string previousDirectory, string path)
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
                _currentScript = File.ReadAllText(path);
                System.IO.Directory.SetCurrentDirectory(path.Substring(0, Math.Max(0, Math.Max(path.LastIndexOf('\\') + 1, path.LastIndexOf('/') + 1))));
                _aditionalPaths = System.IO.Directory.GetCurrentDirectory() + "\\";
                System.IO.Directory.SetCurrentDirectory(previousDirectory);
            }
            else
            {
                System.IO.Directory.SetCurrentDirectory(previousDirectory);
                //throw new Exception(string.Format("File not found: {0}", path));
                _coeLog.Log(string.Format("Script file not found: {0}", path));

            }
        }

        #endregion

        #region Events

        /// <summary>
        /// handler for determining whether to call normalization on insert
        /// this should only be done for insert to temp, direct reg and bulk loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnInsertHandler(object sender, EventArgs args){
            IRegistryRecord record = (IRegistryRecord)sender;
            if (record.IsTemporal || record.IsDirectReg || record.DataStrategy.ToString() == "BulkLoader")
            {
                PerformNormalization(record, true);
            }
        }

        /// <summary>
        /// handler for determining whether to call normalization on update
        /// this should be done for all updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnUpdateHandler(object sender, EventArgs args)
        {
            IRegistryRecord record = (IRegistryRecord)sender;
            PerformNormalization(record, record.IsTemporal);

        }

        /// <summary>
        /// The delegate against which the IRegistryRecord's events are wired to via the
        /// initialization configuration. This always performs normalization regardless of update or insert temp or perm
        /// </summary>
        /// <param name="sender">IRegistryRecord instance</param>
        /// <param name="args"></param>
        //public void ApplyNormalization(object sender, EventArgs args)
        //{
        //    IRegistryRecord record = (IRegistryRecord)sender;
        //    csbr-156849:  this fix stops normalization from occur when inserting to perm from temp which is incorrect. however, normalization will occur

        //    LJB  don't allow addins that work on components to run if AllowUnRegisteredComponents if false and componentlistcount>1)
        //    if ((record.AllowUnregisteredComponents == false && record.ComponentList.Count==1)||(record.AllowUnregisteredComponents == true))
        //    {
        //        if (!RegAddInsUtilities.DisableAddIn(record, _propertyListType, _propertyName, _disableAddinValueArray))
        //        {

        //            if (string.IsNullOrEmpty(this._pythonWebServiceURL))
        //            {
        //                IPrincipal principal = System.Threading.Thread.CurrentPrincipal;
        //                try
        //                {
        //                    System.Threading.Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

        //                    this.NormalizeStructuresEngine(record);
        //                }
        //                finally
        //                {
        //                    System.Threading.Thread.CurrentPrincipal = principal;
        //                }
        //            }
        //            else
        //            {
        //                this.NormalizeStructuresWebService(record);
        //            }
        //        }
        //    }
        //}


        private void PerformNormalization(IRegistryRecord record,bool isTemp)
        {
             if ((record.AllowUnregisteredComponents == false && record.ComponentList.Count == 1) || (record.AllowUnregisteredComponents == true))
            {
                if (!RegAddInsUtilities.DisableAddIn(record, _propertyListType, _propertyName, _disableAddinValueArray))
                {

                    if (string.IsNullOrEmpty(this._pythonWebServiceURL))
                    {
                        IPrincipal principal = System.Threading.Thread.CurrentPrincipal;
                        try
                        {
                            System.Threading.Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

                            this.NormalizeStructuresEngine(record, isTemp);
                        }
                        finally
                        {
                            System.Threading.Thread.CurrentPrincipal = principal;
                        }
                    }
                    else
                    {
                        this.NormalizeStructuresWebService(record);
                    }
                }
            }
        }
        #endregion

        #region IAddIn Members

        IRegistryRecord _registryRecord;

        public IRegistryRecord RegistryRecord
        {
            get
            {
                return _registryRecord;
            }
            set
            {
                _registryRecord = value;
            }
        }

        /// <summary>
        /// Utilizing a configuration xm string, sets local variables and loads the python script
        /// from the file system.
        /// </summary>
        /// <param name="xmlConfiguration">An xml string describing the IAddIn's constrcution.</param>
        public void Initialize(string xmlConfiguration)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlConfiguration);

            XmlNode xmlNode = document.SelectSingleNode("AddInConfiguration/ScriptFile");
            if (xmlNode != null)
            {
                string previousDirectory = Directory.GetCurrentDirectory();
                string path = xmlNode.InnerText.Replace("\r", "").Replace("\n", "");

                LoadPyScript(previousDirectory, path);
            }

            xmlNode = document.SelectSingleNode("AddInConfiguration/PythonWebServiceURL");
            if (xmlNode != null)
            {
                _pythonWebServiceURL = xmlNode.InnerText;
            }
            xmlNode = document.SelectSingleNode("AddInConfiguration/StructuresIdsToAvoid");
            if (xmlNode != null)
                _structuresIdsToAvoid = xmlNode.InnerText;

            xmlNode = document.SelectSingleNode("AddInConfiguration/LogOutputPropertyName");
            if (xmlNode != null)
                _logPropertyName = xmlNode.InnerText;

            //XmlNodeList xmlNodeList = document.SelectNodes("AddInConfiguration/AditionalScriptPaths/Path");
            //foreach (XmlNode currentNode in xmlNodeList)
            //{
            //    if (!string.IsNullOrEmpty(currentNode.InnerText))
            //    {
            //        if (!string.IsNullOrEmpty(_aditionalPaths) && _aditionalPaths[_aditionalPaths.Length - 1] != ';')
            //            _aditionalPaths += ";";

            //        this._aditionalPaths += currentNode.InnerText.Trim();
            //    }
            //}

            //Configuration that allows disabling addin based on a propertylist value
            XmlNode xmlNodeProperyListType = document.SelectSingleNode("AddInConfiguration/PropertyListType");
            if (xmlNodeProperyListType != null && xmlNodeProperyListType.InnerText.Length > 0)
            {
                if (Enum.IsDefined(typeof(RegAddInsUtilities.PropertyListType), xmlNodeProperyListType.InnerText))
                    _propertyListType = (RegAddInsUtilities.PropertyListType)Enum.Parse(typeof(RegAddInsUtilities.PropertyListType), xmlNodeProperyListType.InnerText);
            }

            XmlNode xmlNodeProperyName = document.SelectSingleNode("AddInConfiguration/PropertyName");
            if (xmlNodeProperyName != null && xmlNodeProperyName.InnerText.Length > 0)
            {
                _propertyName = xmlNodeProperyName.InnerText;
            }

            XmlNode xmlNodeDisableValueList = document.SelectSingleNode("AddInConfiguration/DisableValueList");
            if (xmlNodeDisableValueList != null && xmlNodeDisableValueList.InnerText.Length > 0)
            {
                _disableAddinValueArray = xmlNodeDisableValueList.InnerText.ToString().Split(new String[] { "," }, StringSplitOptions.None);
            }

            if(!String.IsNullOrEmpty(_aditionalPaths)){
                //System.Environment.GetEnvironmentVariable() can return null value if the environment variable specified by variable is not found
                //Coverity fix - CID 11795 - Used local variable to resolve Dereference null return value.
                String strPath = System.Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(strPath) && !strPath.Contains(_aditionalPaths)) //Coverity fix - CID 11795 
                    System.Environment.SetEnvironmentVariable("PATH", _aditionalPaths + ";" + strPath);
            }
        }

        /// <summary>
        /// Checks if the id is an exception and must avoid the normalization
        /// </summary>
        /// <param name="id">Structure id to find in configuration</param>
        /// <returns>Whether it is an exception or not</returns>
        private bool IsAnException(string id)
        {
            return _structuresIdsToAvoid.Contains(id + "|");
        }

        #endregion
    }
}
