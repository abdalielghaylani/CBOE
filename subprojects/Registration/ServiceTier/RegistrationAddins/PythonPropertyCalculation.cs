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
using ChemDrawControl18;

/* Sample add-in XML
<AddIn assembly="CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc" class="CambridgeSoft.COE.Registration.Services.RegistrationAddins.PythonPropertyCalculation." friendlyName="ChemScript Calculations" required="no" enabled="no">
  <Event eventName="Updating" eventHandler="OnEventHandler" />
  <Event eventName="Inserting" eventHandler="OnEventHandler" />
  <AddInConfiguration>
    <Calculations>
      <Calculation name="CLogP" enabled="true">
        <ScriptFile>C:\Program Files\CambridgeSoft\ChemOfficeEnterprise12.1.0.0\Registration\PythonScripts\calculateclogp.py</ScriptFile>
        <Behavior>CompoundProperty</Behavior>
        <Transformation>CLogP</Transformation>
        <PropertyName>CMP_COMMENTS</PropertyName>
      </Calculation>
    </Calculations>
  </AddInConfiguration>
</AddIn>
*/

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    /// <summary>
    /// Adds support for ChemScript (or other python scripts) to modify PropertyList property
    /// values.
    /// </summary>
    [Serializable]
    public class PythonPropertyCalculation : PropertyCalculationAddInBase, IAddIn
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("PythonPropertyCalculation");

        /// <summary>
        /// The list of calculation definitions for this calculator.
        /// </summary>
        private List<PythonCalculation> _calcList = new List<PythonCalculation>();

        /// <summary>
        /// stores the object property into which calculation-logs should be placed
        /// </summary>
        private string _logPropertyName = string.Empty;

        /// <summary>
        /// This is the delegated method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnEventHandler(object sender, EventArgs args)
        {
            this.PerformCalculations(sender, args);
        }
        
        /// <summary>
        /// Default ctor: added for the breakpoint!
        /// </summary>
        public PythonPropertyCalculation()
            :base()
        {
        }

        #region IAddIn Members

        IRegistryRecord _registryRecord;
        /// <summary>
        /// Partially-editable instance of an IRegistryRecord object.
        /// </summary>
        public IRegistryRecord RegistryRecord
        {
            get { return _registryRecord; }
            set { _registryRecord = value; }
        }

        /// <summary>
        /// Extracts values from the configuration setting that describe and enable the add-in to function.
        /// </summary>
        /// <param name="xmlConfiguration"></param>
        public void Initialize(string xmlConfiguration)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlConfiguration);

            //Extract the independent calculations
            XmlNodeList calcNodes = document.SelectNodes("//Calculation");
            foreach (XmlElement calcNode in calcNodes)
            {
                PythonCalculation calc = new PythonCalculation();
                XmlNode configNode = null;

                if (calcNode.HasAttribute("enabled"))
                    calc.Enabled = Convert.ToBoolean(calcNode.Attributes["enabled"].Value);

                //By-pass disabled calculations
                if (calc.Enabled == true)
                {
                    //Script file, if Python calculation
                    configNode = calcNode.SelectSingleNode("ScriptFile");
                    if (configNode != null)
                    {
                        string nodeText = configNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(nodeText))
                        {
                            calc.PyScriptPath = nodeText;
                            string previousDirectory = Directory.GetCurrentDirectory();
                            string path = calc.PyScriptPath.Replace("\r", "").Replace("\n", "");
                            LoadPyScript(previousDirectory, path, ref calc);
                        }
                        //Python return parameter name is settting to fetch the value after script execution.
                        if (configNode.Attributes["ReturnParam"] != null && !string.IsNullOrEmpty(configNode.Attributes["ReturnParam"].Value))
                            calc.PyScriptReturnParam = configNode.Attributes["ReturnParam"].Value;
                    }

                    //Behavior
                    configNode = calcNode.SelectSingleNode("Behavior");
                    if (configNode != null)
                    {
                        string nodeText = configNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(nodeText))
                            if (Enum.IsDefined(typeof(Calculation.Behavior), nodeText))
                                calc.Target = (Calculation.Behavior)Enum.Parse(typeof(Calculation.Behavior), nodeText);
                    }

                    //Transformation
                    configNode = calcNode.SelectSingleNode("Transformation");
                    if (configNode != null)
                    {
                        string nodeText = configNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(nodeText))
                            if (Enum.IsDefined(typeof(Calculation.Transformation), nodeText))
                                calc.Action = (Calculation.Transformation)Enum.Parse(typeof(Calculation.Transformation), nodeText);
                    }

                    //PropertyName
                    configNode = calcNode.SelectSingleNode("PropertyName");
                    if (configNode != null)
                    {
                        string nodeText = configNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(nodeText))
                            calc.PropertyName = configNode.InnerText;
                    }

                    _calcList.Add(calc);
                }

            }
        }

        #endregion

        #region >Private methods<

        /// <summary>
        /// Access to the file system at this level requires a Principal swap.
        /// </summary>
        /// <param name="sender">the IRegistryRecord instance</param>
        /// <param name="args">empty event arguments</param>
        private void PerformCalculations(object sender, EventArgs args)
        {
            RegistryRecord = (IRegistryRecord)sender;

            IPrincipal principal = System.Threading.Thread.CurrentPrincipal;
            try
            {
                System.Threading.Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                this.ApplyOperations();
            }
            finally
            {
                System.Threading.Thread.CurrentPrincipal = principal;
            }
        }

        /// <summary>
        /// Performs the actual calculation of CLogP via the python engine.
        /// </summary>
        /// <param name="record"></param>
        private void ApplyOperations()
        {
            foreach (PythonCalculation calc in _calcList)
            {
                switch (calc.Target)
                {
                    case Calculation.Behavior.CompoundProperty:
                        //this.SetCompoundProperty(calc);
                        //this method calculates all the Properties set in the configuration
                        this.CalculateCompoundProperty(calc);
                        break;
                    case Calculation.Behavior.BatchProperty:
                        throw new NotImplementedException("Python-based BatchProperty calculations");
                }
            }
        }

        private void SetCompoundProperty(PythonCalculation calc)
        {
            switch (calc.Action)
            {
                case Calculation.Transformation.CLogP:
                    this.CalculateLogP(calc);
                    break;
            }
        }

        /// <summary>
        /// Checks for a Property, by name, in the appropriate PropertyList.
        /// </summary>
        /// <param name="name">the key (name) of a Registration Property</param>
        /// <param name="chemCompound"></param>
        /// <returns>True if the specified Property is in this Compound's PropertyList</returns>
        private bool ComponentPropertyExists(string name, Compound chemCompound)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(name) && chemCompound != null)
            {
                if (chemCompound.PropertyList.Count > 0)
                {
                    if (chemCompound.PropertyList[name] != null)
                        retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Uses the 'structure' information of a compound to allow callers to determine
        /// a course of action.
        /// </summary>
        /// <param name="chemCompound">a Compound instance</param>
        /// <returns>True if a structure exists and is not a 'reserved' structure equivalent.</returns>
        private bool ComponentStructureExists(Compound chemCompound)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(chemCompound.BaseFragment.Structure.Value))
                if (chemCompound.BaseFragment.Structure.ID >= 0)
                    retVal = true;
            return retVal;
        }

        /// <summary>
        /// Enables the calculation of all properties set in the configuration
        /// as a pluggable Add-In using ChemScript.
        /// </summary>
        /// <param name="calcParams">A configured instance of the Calculation class</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void CalculateCompoundProperty(PythonCalculation calcParams)
        {
            ChemDrawCtl ctrl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                using (CambridgeSoft.ChemScript.PyEngineNet pythonEngine = new CambridgeSoft.ChemScript.PyEngineNet())
                {
                    foreach (Component component in RegistryRecord.ComponentList)
                    {


                        if (ComponentPropertyExists(calcParams.PropertyName, component.Compound)
                            && ComponentStructureExists(component.Compound)
                       )
                        {
                            RegistryRecord oRegistryRecord = ((CambridgeSoft.COE.Registration.Services.Types.RegistryRecord)(RegistryRecord));
                            string chemicalStructure = component.Compound.BaseFragment.Structure.Value;
                            string executionAudit = string.Empty;

                            SetData(ref ctrl, chemicalStructure, true);

                            try
                            {
                                //The python engine accepts an XML representation of the ChemDraw structure
                                string cdxXml = ctrl.get_Data("cdxml").ToString();
                                pythonEngine.SetVar("cdx", cdxXml);
                                pythonEngine.SetVar("scriptsPath", AditionalPaths);
                                pythonEngine.SetVar("errFile", AditionalPaths);

                                string scriptContent = calcParams.PyScriptContent.Replace("\r\n", "\n");

                                bool isError = false;
                                try { isError = !pythonEngine.Execute(scriptContent); }
                                catch (AccessViolationException) { isError = !pythonEngine.Execute(scriptContent); }

                                //Trap all output from the python script execution
                                if (isError)
                                {
                                    executionAudit = pythonEngine.GetError();
                                    _coeLog.Log(executionAudit);
                                }
                                else
                                {
                                    executionAudit = pythonEngine.GetVar("logString");
                                    if (!string.IsNullOrEmpty(_logPropertyName) && !string.IsNullOrEmpty(executionAudit))
                                        if (component.Compound.PropertyList[_logPropertyName.ToUpper()] != null)
                                            component.Compound.PropertyList[_logPropertyName.ToUpper()].Value = executionAudit;

                                    //If it exists, get the needed property value from the python engine
                                    string strValue = string.Empty;
                                    if (!string.IsNullOrEmpty(calcParams.PyScriptReturnParam))
                                        strValue = pythonEngine.GetVar(calcParams.PyScriptReturnParam);
                                    else
                                        strValue = pythonEngine.GetVar(calcParams.PropertyName);
                                    if (!string.IsNullOrEmpty(strValue))
                                        //property name as set in COE
                                        component.Compound.PropertyList[calcParams.PropertyName].Value = strValue;

                                    //((CambridgeSoft.COE.Registration.Services.Types.RegistryRecord)(oRegistryRecord)).PropertyList[calcParams.PropertyName.ToUpper()].Value = cLogP;
                                }
                            }
                            catch (Exception exception)
                            {
                                executionAudit = exception.Message;
                                _coeLog.Log(executionAudit);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
        }

        /// <summary>
        /// Enables the calculation of CLogP ('hydrophilicity', based on octanol:water partitioning)
        /// as a pluggable Add-In using ChemScript.
        /// </summary>
        /// <param name="calcParams">A configured instance of the Calculation class</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void CalculateLogP(PythonCalculation calcParams)
        {
            ChemDrawCtl ctrl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                using (CambridgeSoft.ChemScript.PyEngineNet pythonEngine = new CambridgeSoft.ChemScript.PyEngineNet())
                {
                    foreach (Component component in RegistryRecord.ComponentList)
                    {
                        if (ComponentPropertyExists(calcParams.PropertyName, component.Compound)
                            && ComponentStructureExists(component.Compound)
                        )
                        {
                            string chemicalStructure = component.Compound.BaseFragment.Structure.Value;
                            string executionAudit = string.Empty;

                            SetData(ref ctrl, chemicalStructure, true);

                            try
                            {
                                //The python engine accepts an XML representation of the ChemDraw structure
                                string cdxXml = ctrl.get_Data("cdxml").ToString();
                                pythonEngine.SetVar("cdx", cdxXml);
                                pythonEngine.SetVar("scriptsPath", AditionalPaths);
                                pythonEngine.SetVar("errFile", AditionalPaths);

                                string scriptContent = calcParams.PyScriptContent.Replace("\r\n", "\n");

                                bool isError = false;
                                try { isError = !pythonEngine.Execute(scriptContent); }
                                catch (AccessViolationException) { isError = !pythonEngine.Execute(scriptContent); }

                                //Trap all output from the python script execution
                                if (isError)
                                {
                                    executionAudit = pythonEngine.GetError();
                                    _coeLog.Log(executionAudit);
                                }
                                else
                                {
                                    executionAudit = pythonEngine.GetVar("logString");
                                    if (!string.IsNullOrEmpty(_logPropertyName) && !string.IsNullOrEmpty(executionAudit))
                                        if (component.Compound.PropertyList[_logPropertyName.ToUpper()] != null)
                                            component.Compound.PropertyList[_logPropertyName.ToUpper()].Value = executionAudit;

                                    //If it exists, get the clogP value from the python engine
                                    string cLogP = pythonEngine.GetVar("clogpValue");
                                    if (!string.IsNullOrEmpty(cLogP))
                                        component.Compound.PropertyList[calcParams.PropertyName.ToUpper()].Value = cLogP;
                                }
                            }
                            catch (Exception exception)
                            {
                                executionAudit = exception.Message;
                                _coeLog.Log(executionAudit);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
        }

        #endregion

    }
}
