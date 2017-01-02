using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Data;
using ChemDrawControl15;
using System.Globalization;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    public class ChemDrawPropertyCalculationAddIn : IAddIn
    {
        #region IAddIn Members -- JED: "Calculate" isn't part of the interface

        public void Calculate(object sender, EventArgs args)
        {
            if (sender is IRegistryRecord)
                this.ApplyOperation((IRegistryRecord)sender);
        }

        #endregion

        #region Variables

        private IRegistryRecord _registryRecord;

        private Batch _firstBatch;
        private string _firstBatchPercentActive;
        protected double formulaWeight;
        //this allows you to build a list of caluclations to lop through
        private List<Calculation> _calcList = new List<Calculation>();

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("ChemDrawPropertyCalculationAddIn");
        #endregion

        #region IAddIn Members

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

        public void Initialize(string xmlConfiguration)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlConfiguration);

                //Get the Calculations node which has child Calculations
                XmlNode currentNode = document.SelectSingleNode("AddInConfiguration/Calculations");
                if (currentNode != null && currentNode.ChildNodes.Count > 0)
                {
                    //If the caluclations node exists get the Calculation nodes
                    foreach (XmlElement calcNode in currentNode.ChildNodes)
                    {
                        //I think I should be checking here 
                        //that the child Node Name is actually Calculation.

                        //Create a calculation object to be configured and added to the calcList
                        Calculation calc = new Calculation();

                        //Allow for an override if the 'enabled' attribute is false
                        if (calcNode.HasAttribute("enabled"))
                            calc.Enabled = Convert.ToBoolean(calcNode.Attributes["enabled"].Value);
                        else
                            calc.Enabled = true;

                        //By-pass disabled calculations
                        if (calc.Enabled == true)
                        {
                            //Grab the Calculation Node
                            currentNode = calcNode;

                            //set the Calculation object variables based on the values passed
                            XmlNode calcNodeConfig = currentNode.SelectSingleNode("Behavior");
                            if (calcNodeConfig != null && calcNodeConfig.InnerText.Length > 0)
                            {
                                if (Enum.IsDefined(typeof(Calculation.Behavior), calcNodeConfig.InnerText))
                                    calc.Target = (Calculation.Behavior)Enum.Parse(typeof(Calculation.Behavior), calcNodeConfig.InnerText);
                            }

                            calcNodeConfig = currentNode.SelectSingleNode("Transformation");
                            if (calcNodeConfig != null && calcNodeConfig.InnerText.Length > 0)
                            {
                                if (Enum.IsDefined(typeof(Calculation.Transformation), calcNodeConfig.InnerText))
                                    calc.Action = (Calculation.Transformation)Enum.Parse(typeof(Calculation.Transformation), calcNodeConfig.InnerText);
                            }

                            calcNodeConfig = currentNode.SelectSingleNode("PropertyName");
                            if (calcNodeConfig != null && calcNodeConfig.InnerText.Length > 0)
                                calc.PropertyName = calcNodeConfig.InnerText;

                            calcNodeConfig = currentNode.SelectSingleNode("DefaultText");
                            if (calcNodeConfig != null && calcNodeConfig.InnerText.Length > 0)
                                calc.DefaultText = calcNodeConfig.InnerText;

                            //now add to the calculation to the calcList
                            _calcList.Add(calc);

                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("Your addIn is not correctly configured.", e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply a transformation to any object of the registry
        /// </summary>
        /// <param name="registry"></param>
        private void ApplyOperation(IRegistryRecord registry)
        {
            _registryRecord = registry;

            //we now need all of the calculations in the list
            foreach (Calculation c in _calcList)
            {

                //we need to set the add in properties each time
                switch (c.Target)
                {
                    case Calculation.Behavior.CompoundProperty:
                        this.SetCompoundProperty(c.Action, c.PropertyName, c.DefaultText);
                        break;
                    case Calculation.Behavior.StructureProperty:
                        this.SetStructureProperty(c.Action, c.PropertyName, c.DefaultText);
                        break;
                    case Calculation.Behavior.BatchProperty:
                        this.SetBatchProperty(c.Action, c.PropertyName, c.DefaultText);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets a value of the PropertyList of the Compound
        /// </summary>
        private void SetCompoundProperty(
            Calculation.Transformation transformation, string fieldName, string defaultText)
        {
            //Loop through the compounds
            foreach (Component component in _registryRecord.ComponentList)
            {
                if (PropertyExists(fieldName, component.Compound.PropertyList))
                {
                    string retVal = string.Empty;
                    switch (transformation)
                    {
                        case Calculation.Transformation.FindNameByStructure:
                            retVal = GetNameByStructure(component.Compound.BaseFragment.Structure.Value);
                            break;
                        case Calculation.Transformation.FormulaWeight:
                            if (component.ComponentIndex < 0)
                            {
                                retVal = GetFormulaWeight(component.Compound.BaseFragment.Structure.Value, component.Compound.FragmentList,
                                                            _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, true).ToString();
                            }
                            else
                            {
                                retVal = GetFormulaWeight(component.Compound.BaseFragment.Structure.Value, component.Compound.FragmentList,
                                                            _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, false).ToString();
                            }
                            break;
                        case Calculation.Transformation.MolecularFormula:
                            if (component.ComponentIndex < 0)
                            {
                                retVal = GetMolecularFormula(component.Compound.BaseFragment.Structure.Value, component.Compound.FragmentList,
                                                                _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, true).ToString();
                            }
                            else
                            {

                                retVal = GetMolecularFormula(component.Compound.BaseFragment.Structure.Value, component.Compound.FragmentList,
                                                                _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, false).ToString();
                            }
                            break;
                        case Calculation.Transformation.PercentActive:
                            if (component.ComponentIndex < 0)
                            {
                                retVal = GetPercentActive(component.Compound.BaseFragment.Structure.Value, _registryRecord.ComponentList[0].Compound.FragmentList,
                                                            _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, true);
                            }
                            else
                            {
                                retVal = GetPercentActive(component.Compound.BaseFragment.Structure.Value, _registryRecord.ComponentList[0].Compound.FragmentList,
                                                            _registryRecord.BatchList[0].BatchComponentList[_registryRecord.ComponentList.IndexOf(component)].BatchComponentFragmentList, false);
                            }
                            break;
                    }
                    if (!string.IsNullOrEmpty(retVal))
                        component.Compound.PropertyList[fieldName].Value = retVal;
                    else
                    {
                        //If defined and the result of the operation was empty, put a default text.
                        if (!string.IsNullOrEmpty(defaultText))
                            component.Compound.PropertyList[fieldName].Value = defaultText;
                    }

                }
            }
        }

        /// <summary>
        /// Sets a value in a Structure's PropertyList
        /// </summary>
        private void SetStructureProperty(
            Calculation.Transformation transformation, string fieldName, string defaultText)
        {
            //Loop through the compounds
            foreach (Component component in _registryRecord.ComponentList)
            {
                if (PropertyExists(fieldName, component.Compound.BaseFragment.Structure.PropertyList))
                {
                    string retVal = string.Empty;
                    switch (transformation)
                    {
                        case Calculation.Transformation.FindNameByStructure:
                            retVal = GetNameByStructure(component.Compound.BaseFragment.Structure.Value);
                            break;
                    }

                    if (!string.IsNullOrEmpty(retVal))
                        component.Compound.BaseFragment.Structure.PropertyList[fieldName].Value = retVal;
                    else
                    {
                        //If defined and the result of the operation was empty, put a default text.
                        if (!string.IsNullOrEmpty(defaultText))
                            component.Compound.BaseFragment.Structure.PropertyList[fieldName].Value = defaultText;
                    }
                }
            }
        }

        /// <summary>
        /// Sets a value of the PropertyList of the Batch
        /// </summary>
        private void SetBatchProperty(
            Calculation.Transformation transformation, string fieldName, string defaultText)
        {
            //Loop through the compounds
            _firstBatch = this.RegistryRecord.BatchList[0];
            foreach (Batch batch in _registryRecord.BatchList)
            {
                if (PropertyExists(fieldName, batch.PropertyList))
                {
                    string retVal = string.Empty;
                    switch (transformation)
                    {
                        case Calculation.Transformation.FormulaWeight:
                            formulaWeight = 0;
                            if (batch.BatchComponentList.Count == 1)
                            {
                                if (_registryRecord.ComponentList[0].ComponentIndex < 0)
                                {
                                    retVal = this.GetFormulaWeight(GetStructureValue(_registryRecord.ComponentList[0].Compound.BaseFragment.Structure), _registryRecord.ComponentList[0].Compound.FragmentList,
                                                                   batch.BatchComponentList[0].BatchComponentFragmentList, true).ToString();
                                }
                                else
                                {
                                    retVal = this.GetFormulaWeight(GetStructureValue(_registryRecord.ComponentList[0].Compound.BaseFragment.Structure), _registryRecord.ComponentList[0].Compound.FragmentList,
                                                                   batch.BatchComponentList[0].BatchComponentFragmentList, false).ToString();
                                }
                            }
                            else
                            {
                                foreach (BatchComponent batchComponent in batch.BatchComponentList)
                                {

                                    if (batchComponent.ComponentIndex < 0)
                                        formulaWeight += this.GetFraction(batchComponent) * this.GetFormulaWeight(GetStructureValue(_registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).BaseFragment.Structure),
                                            _registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).FragmentList, batchComponent.BatchComponentFragmentList, true);
                                    else
                                        formulaWeight += this.GetFraction(batchComponent) * this.GetFormulaWeight(GetStructureValue(_registryRecord.ComponentList[batchComponent.OrderIndex - 1].Compound.BaseFragment.Structure),
                                            _registryRecord.ComponentList[batchComponent.OrderIndex - 1].Compound.FragmentList, batchComponent.BatchComponentFragmentList, false);

                                }
                                if (formulaWeight != 0)
                                    retVal = formulaWeight.ToString();
                            }
                            break;
                        case Calculation.Transformation.MolecularFormula:
                            string tempMolFormula = string.Empty;
                            List<string> molFormulaList = new List<string>();
                            foreach (BatchComponent batchComponent in batch.BatchComponentList)
                            {
                                if (batchComponent.ComponentIndex < 0)
                                {
                                    tempMolFormula = GetMolecularFormula(GetStructureValue(_registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).BaseFragment.Structure),
                                        _registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).FragmentList, batchComponent.BatchComponentFragmentList, true);
                                }
                                else
                                {
                                    tempMolFormula = GetMolecularFormula(GetStructureValue(_registryRecord.ComponentList[batchComponent.OrderIndex - 1].Compound.BaseFragment.Structure),
                                        _registryRecord.ComponentList[batchComponent.OrderIndex - 1].Compound.FragmentList, batchComponent.BatchComponentFragmentList, false);
                                }

                                if (!molFormulaList.Contains(tempMolFormula))
                                    molFormulaList.Add(tempMolFormula);

                            }
                            if (molFormulaList.Count > 1)
                            {
                                retVal = molFormulaList[0] + "  ";
                                for (int i = 1; i < molFormulaList.Count; i++)
                                {
                                    retVal += molFormulaList[i] + "  ";
                                }
                                retVal = retVal.Remove(retVal.Length - 2, 2);
                            }
                            else
                                retVal = molFormulaList[0];
                            break;
                        case Calculation.Transformation.PercentActive:
                            if (this.RegistryRecord.ComponentList.Count == 1)
                            {
                                FragmentList fragementList = this.RegistryRecord.ComponentList[0].Compound.FragmentList;
                                BatchComponentFragmentList batchComponentFragmentList = batch.BatchComponentList[0].BatchComponentFragmentList;
                                string structure = GetStructureValue(this.RegistryRecord.ComponentList[0].Compound.BaseFragment.Structure);

                                if (RegistryRecord.SameBatchesIdentity)
                                {
                                    if (batch.OrderIndex == _firstBatch.OrderIndex)
                                    {
                                        retVal = _firstBatchPercentActive = this.GetPercentActive(structure, fragementList, batchComponentFragmentList, !batch.IsNew);
                                    }
                                    else
                                    {
                                        retVal = _firstBatchPercentActive;
                                    }
                                }
                                else
                                {
                                    retVal += this.GetPercentActive(structure, fragementList, batchComponentFragmentList, !batch.IsNew);
                                }
                            }
                            else
                            {
                                // CSBR-133267
                                // Note #3   When EnableMixtures are true EnableFragments should be false. Therefore the number would always be 100%.
                                retVal = "100";
                            }
                            break;
                    }
                    if (!string.IsNullOrEmpty(retVal))
                    {
                        if (batch.PropertyList[fieldName].Value != retVal)
                        {
                           
                            int valuelength = 0;
                            int precisionCount = 0;
                            string str = "";
                            string precisionValue = null;
                            string[] PrecisionSegments = null;
                            string[] valueSegments = null;

                            if (batch.PropertyList[fieldName].Type.ToString() == "NUMBER")
                            {
                                double dretVal = double.Parse(retVal);
                                if (batch.PropertyList[fieldName].Precision != null)
                                    precisionValue = batch.PropertyList[fieldName].Precision.ToString();
                                else
                                    precisionValue = "0";
                                PrecisionSegments = precisionValue.Split('.');
                                valueSegments = retVal.Split('.');

                                if (PrecisionSegments != null && PrecisionSegments.Length > 1)
                                {
                                    precisionCount = Convert.ToInt32(PrecisionSegments[1]);
                                    if (valueSegments != null && valueSegments.Length > 1)
                                    {
                                        valuelength = Convert.ToInt32(valueSegments[1].Length);
                                        int lessIndexnumber = (valuelength > precisionCount) ? precisionCount : valuelength;
                                        
                                        retVal = String.Format("{0:0." + str.PadRight(lessIndexnumber, '0') + "}", dretVal);
                                        batch.PropertyList[fieldName].Value = retVal;
                                    }
                                    else
                                        batch.PropertyList[fieldName].Value = retVal;
                                }
                                else
                                    batch.PropertyList[fieldName].Value = retVal;
                            }
                            else
                                batch.PropertyList[fieldName].Value = retVal;
                        }
                    }
                    else
                    {
                        //If defined and the result of the operation was empty, put a default text.
                        if (!string.IsNullOrEmpty(defaultText))
                            batch.PropertyList[fieldName].Value = defaultText;
                    }

                }
            }
        }

        /// <summary>
        /// Determines if the specified PropertyList contains the property by its key.
        /// </summary>
        /// <param name="name">the key of the Property</param>
        /// <param name="list">the actual PropertyList being queried</param>
        /// <returns>true if the property key was found in the specified list</returns>
        private bool PropertyExists(string name, PropertyList list)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(name) && list != null)
            {
                if (list.Count > 0)
                    if (list[name] != null)
                        retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Finds the structure name given the 64 value.
        /// </summary>
        /// <param name="base64">Structure value</param>
        /// <returns>Name of tyhe structure</returns>
        private string GetNameByStructure(string base64)
        {
            string retVal = string.Empty;
            ChemDrawCtl ctl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            RegAddInsUtilities.SetDataStructure(ref ctl, base64);
            //Added a try catch block to capture the error thrown by chemdraw control 
            //while generatinng structure name for bigger compounds and returning empty string in that scenario.
            try
            {
                if (ctl.get_Data("chemical/x-name") != null)
                    retVal = ctl.get_Data("chemical/x-name").ToString();
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                ctl = null;
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }

            return retVal;

        }

        /// <summary>
        /// Finds the Formula Weight (including salts and solvates)
        /// </summary>
        /// <param name="base64">Structure value</param>
        /// <param name="fragments"></param>
        /// <returns></returns>
        private double GetFormulaWeight(string base64, FragmentList fragments, BatchComponentFragmentList batchComponentFragments, bool fromDataBase)
        {
            double retVal = 0.0;
            ChemDrawCtl ctl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                RegAddInsUtilities.SetDataStructure(ref ctl, base64);
                double fragmentsMW = GetFragmentsMW(fragments, batchComponentFragments, fromDataBase);
                double objectMW = ctl.Objects.MolecularWeight;
                retVal = (double)(fragmentsMW + objectMW);
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
            return retVal;
        }

        /// <summary>
        /// Gets the (Salt * equivalent + Solvates * equivalent) value for all the salts/solvates in the current registry
        /// </summary>
        /// <param name="fragments">List of fragments</param>
        /// <returns>Salt/Solvates MW</returns>
        private double GetFragmentsMW(FragmentList fragments, BatchComponentFragmentList batchComponentFragments, bool fromDataBase)
        {
            double retVal = 0.0;
            foreach (BatchComponentFragment bcFragment in batchComponentFragments)
            {
                if (bcFragment.FragmentID > 0)
                {
                    if (fromDataBase)
                    {
                        if (string.IsNullOrEmpty(bcFragment.MW) || (!bcFragment.IsNew && !bcFragment.IsDirty))
                        {
                            if (fragments.GetByID(bcFragment.FragmentID).Structure != null)
                                retVal += (double)fragments.GetByID(bcFragment.FragmentID).Structure.MolWeight * bcFragment.Equivalents;
                        }
                        else
                            retVal += (double)(double.Parse(bcFragment.MW, CultureInfo.InvariantCulture) * bcFragment.Equivalents);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(bcFragment.MW))
                            retVal += (double)(double.Parse(bcFragment.MW, CultureInfo.InvariantCulture) * bcFragment.Equivalents);
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Concats all the fragments of the current registry
        /// </summary>
        /// <param name="fragments">List of Salts/Solvates</param>
        /// <returns>Concatenated formulas</returns>
        private string GetFragmentsFormulas(FragmentList fragments, BatchComponentFragmentList batchComponentFragments, bool fromDataBase)
        {
            string retVal = string.Empty;
            foreach(BatchComponentFragment bcFragment in batchComponentFragments)
            {
                if(bcFragment.FragmentID > 0)
                {
                    if(fromDataBase)
                    {
                        if(string.IsNullOrEmpty(bcFragment.Formula) || (!bcFragment.IsNew && !bcFragment.IsDirty))
                        {
                            if(fragments.GetByID(bcFragment.FragmentID).Structure != null)
                                retVal += bcFragment.Equivalents.ToString() + fragments.GetByID(bcFragment.FragmentID).Structure.Formula + "\u00B7";
                        }
                        else
                            retVal += bcFragment.Equivalents.ToString() + bcFragment.Formula + "\u00B7";
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(bcFragment.Formula))
                            retVal += bcFragment.Equivalents.ToString() + bcFragment.Formula + "\u00B7";
                    }
                }
            }
            if (retVal != string.Empty)
                retVal = retVal.Remove(retVal.Length - 1, 1);
            return retVal;
        }

        /// <summary>
        /// Gets the Molecular formula (including salts & solvates)
        /// </summary>
        /// <param name="base64">Structure value</param>
        /// <returns>Formula concatenation</returns>
        private string GetMolecularFormula(string base64, FragmentList fragments, BatchComponentFragmentList batchComponentFragments, bool fromDataBase)
        {
            string retVal = string.Empty;
            ChemDrawCtl ctl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                RegAddInsUtilities.SetDataStructure(ref ctl, base64);
                string fragmentsFormula = this.GetFragmentsFormulas(fragments, batchComponentFragments, fromDataBase);
                string objectFormula = ctl.Objects.Formula;
                if (!string.IsNullOrEmpty(fragmentsFormula))
                    retVal = objectFormula + "\u00B7" + fragmentsFormula;
                else
                    retVal = objectFormula;
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Percentage of Active
        /// </summary>
        /// <param name="base64">Structure value</param>
        /// <param name="fragments">list of fragments</param>
        /// <returns>% value</returns>
        private string GetPercentActive(string base64, FragmentList fragments, BatchComponentFragmentList batchComponentFragments, bool fromDataBase)
        {
            string retVal = string.Empty;
            ChemDrawCtl ctl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                RegAddInsUtilities.SetDataStructure(ref ctl, base64);
                double fragmentsMW = this.GetFragmentsMW(fragments, batchComponentFragments, fromDataBase);
                double percentage;
                if (fragmentsMW + ctl.Objects.MolecularWeight != 0)
                    percentage = (ctl.Objects.MolecularWeight / ((double)(fragmentsMW + ctl.Objects.MolecularWeight))) * 100;
                else
                    percentage = 0;
                percentage = Math.Round(percentage, 2);
                retVal = percentage.ToString();
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
            return retVal;
        }
    

        private int GetFraction(BatchComponent batchCompoenent)
        {
            int fraction = 1;
            foreach (Property prop in batchCompoenent.PropertyList)
            {
                if (prop.Name.ToUpper() == "FRACTION" && prop.Value != string.Empty)
                    fraction = int.Parse(prop.Value);
            }
            return fraction;
        }

        /// <summary>
        /// Added a new method to extract the proper structure base64 value to calculate MW,Formula and percentage active
        /// </summary>
        /// <param name="basefragmentstructure"></param>
        /// <returns></returns>
        private string GetStructureValue(Structure basefragmentstructure)
        {
            string base64value = string.Empty;
            if (basefragmentstructure != null)
            {
                if (!string.IsNullOrEmpty(basefragmentstructure.NormalizedStructure) && basefragmentstructure.UseNormalizedStructure)
                    base64value = basefragmentstructure.NormalizedStructure;
                else
                    base64value = basefragmentstructure.Value;
            }
            return base64value;
        }

        #endregion
    }
}

