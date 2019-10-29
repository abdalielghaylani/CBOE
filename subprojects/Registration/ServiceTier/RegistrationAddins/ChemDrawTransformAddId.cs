using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Data;
using ChemDrawControl19;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    class ChemDrawTransformAddIn : IAddIn
    {
        #region IAddIn Members

        public void ResolveChemDrawTransformation(object sender, EventArgs args)
        {
            if (sender is IRegistryRecord)
                this.ApplyOperation((IRegistryRecord)sender);
        }

        #endregion

        #region Variables

        private IRegistryRecord _registryRecord;
        private Behavior _behavior = Behavior.NotSet;
        private Transformation _transformation = Transformation.NotSet;
        private string _fieldName = string.Empty;
        private string _defaultText = string.Empty;
        private string _saltFormat = "+ {0}"; //by default
        private string _solvateFormat = "- {0}"; //by default
        private Batch _firstBatch;
        private string _firstBatchPercentActive;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("ChemDrawTransformAddIn");

        /// <summary>
        /// Behavior of the addin (object to apply transformation)
        /// </summary>
        public enum Behavior
        {
            BatchProperty,
            CompoundProperty,
            NotSet,
        }

        /// <summary>
        /// List of transformation available
        /// </summary>
        public enum Transformation
        {
            FindNameByStructure,    //Finds a structure name given the structure value
            FormulaWeight,          //Finds the MW of the components (including salts + solvates)
            MolecularFormula,       //Finds the Mol Formulat (including salts + solvates)
            PercentActive,          //Finds the percentage of active sol
            NotSet,                 //Default value
        }

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

                //Default behavior of this AddIn
                XmlNode currentNode = document.SelectSingleNode("AddInConfiguration/Behavior");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                {
                    if (Enum.IsDefined(typeof(Behavior), currentNode.InnerText))
                        _behavior = (Behavior)Enum.Parse(typeof(Behavior), currentNode.InnerText);
                }

                currentNode = document.SelectSingleNode("AddInConfiguration/Transformation");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                {
                    if (Enum.IsDefined(typeof(Transformation), currentNode.InnerText))
                        _transformation = (Transformation)Enum.Parse(typeof(Transformation), currentNode.InnerText);
                }

                //Required for the search (except CompoundStructure which is done in the DB by default.
                currentNode = document.SelectSingleNode("AddInConfiguration/PropertyName");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                    _fieldName = currentNode.InnerText;

                currentNode = document.SelectSingleNode("AddInConfiguration/DefaultText");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                    _defaultText = currentNode.InnerText;

                currentNode = document.SelectSingleNode("AddInConfiguration/SaltFormat");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                    _saltFormat = currentNode.InnerText;

                currentNode = document.SelectSingleNode("AddInConfiguration/SolvateFormat");
                if (currentNode != null && currentNode.InnerText.Length > 0)
                    _solvateFormat = currentNode.InnerText;

            }
            catch
            {
                //Default Settings.
                _behavior = Behavior.CompoundProperty;
                _transformation = Transformation.FindNameByStructure;
                _fieldName = "COMPOUNDNAME";
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
            switch (_behavior)
            {
                case Behavior.CompoundProperty:
                    //LJB  don't allow addins that work on components to run if AllowUnRegisteredComponents if false and componentlistcount>1)
                    if ((registry.AllowUnregisteredComponents == false && registry.ComponentList.Count == 1) || (registry.AllowUnregisteredComponents == true))
                    {
                        this.SetCompoundProperty();
                    }
                    break;
                case Behavior.BatchProperty:
                    this.SetBatchProperty();
                    break;
            }
        }

        /// <summary>
        /// Sets a value of the PropertyList of the Compound
        /// </summary>
        private void SetCompoundProperty()
        {
            //Loop through the compounds
            foreach (Component component in _registryRecord.ComponentList)
            {
                if (ExistsProperty(_fieldName, component.Compound))
                {
                    string retVal = string.Empty;
                    switch (_transformation)
                    {
                        case Transformation.FindNameByStructure:
                            retVal = GetNameByStructure(component.Compound.BaseFragment.Structure.Value);
                            break;
                        case Transformation.FormulaWeight:
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
                        case Transformation.MolecularFormula:
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
                        case Transformation.PercentActive:
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
                        component.Compound.PropertyList[_fieldName].Value = retVal;
                    else
                    {
                        //If defined and the result of the operation was empty, put a default text.
                        if (!string.IsNullOrEmpty(_defaultText))
                            component.Compound.PropertyList[_fieldName].Value = _defaultText;
                    }

                }
            }
        }

        /// <summary>
        /// Sets a value of the PropertyList of the Batch
        /// </summary>
        private void SetBatchProperty()
        {
            //Loop through the compounds
            _firstBatch = this.RegistryRecord.BatchList[0];
            foreach (Batch batch in _registryRecord.BatchList)
            {
                if (ExistsBatchProperty(_fieldName, batch))
                {
                    string retVal = string.Empty;
                    switch (_transformation)
                    {
                        case Transformation.FormulaWeight:
                            double formulaWeight = 0;
                            if (batch.BatchComponentList.Count == 1)
                            {
                                if (_registryRecord.ComponentList[0].ComponentIndex < 0)
                                {
                                    retVal = this.GetFormulaWeight(_registryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value, _registryRecord.ComponentList[0].Compound.FragmentList,
                                                                   batch.BatchComponentList[0].BatchComponentFragmentList, true).ToString();
                                }
                                else
                                {
                                    retVal = this.GetFormulaWeight(_registryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value, _registryRecord.ComponentList[0].Compound.FragmentList,
                                                                   batch.BatchComponentList[0].BatchComponentFragmentList, false).ToString();
                                }
                            }
                            else
                            {
                                foreach (BatchComponent batchComponent in batch.BatchComponentList)
                                {

                                    if (batchComponent.ComponentIndex < 0)
                                        formulaWeight += this.GetFraction(batchComponent) * this.GetFormulaWeight(_registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).BaseFragment.Structure.Value,
                                            _registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).FragmentList, batchComponent.BatchComponentFragmentList, true);
                                    else
                                        formulaWeight += this.GetFraction(batchComponent) * this.GetFormulaWeight(_registryRecord.ComponentList[batchComponent.ComponentIndex].Compound.BaseFragment.Structure.Value,
                                            _registryRecord.ComponentList[batchComponent.ComponentIndex].Compound.FragmentList, batchComponent.BatchComponentFragmentList, false);

                                }
                                if (formulaWeight != 0)
                                    retVal = formulaWeight.ToString();
                            }
                            break;
                        case Transformation.MolecularFormula:
                            string tempMolFormula = string.Empty;
                            List<string> molFormulaList = new List<string>();
                            foreach (BatchComponent batchComponent in batch.BatchComponentList)
                            {
                                if (batchComponent.ComponentIndex < 0)
                                {
                                    string val = _registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).BaseFragment.Structure.Value;
                                    tempMolFormula = GetMolecularFormula(val,
                                        _registryRecord.ComponentList.GetCompoundByID(batchComponent.CompoundID).FragmentList, batchComponent.BatchComponentFragmentList, true);
                                }
                                else
                                {
                                    tempMolFormula = GetMolecularFormula(_registryRecord.ComponentList[batchComponent.ComponentIndex].Compound.BaseFragment.Structure.Value,
                                        _registryRecord.ComponentList[batchComponent.ComponentIndex].Compound.FragmentList, batchComponent.BatchComponentFragmentList, false);
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
                        case Transformation.PercentActive:
                            if (this.RegistryRecord.ComponentList.Count == 1)
                            {
                                FragmentList fragementList = this.RegistryRecord.ComponentList[0].Compound.FragmentList;
                                BatchComponentFragmentList batchComponentFragmentList = batch.BatchComponentList[0].BatchComponentFragmentList;
                                string structure = this.RegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value;

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
                            break;
                    }
                    if (!string.IsNullOrEmpty(retVal))
                    {
                        if (batch.PropertyList[_fieldName].Value != retVal)
                            batch.PropertyList[_fieldName].Value = retVal;
                    }
                    else
                    {
                        //If defined and the result of the operation was empty, put a default text.
                        if (!string.IsNullOrEmpty(_defaultText))
                            batch.PropertyList[_fieldName].Value = _defaultText;
                    }

                }
            }
        }

        /// <summary>
        /// Check if the property exist in the control before any kind of casting
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <param name="comp">Compound object</param>
        /// <returns>Boolean indicating the result</returns>
        private bool ExistsProperty(string name, Compound comp)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(name) && comp != null)
            {
                if (comp.PropertyList.Count > 0)
                {
                    if (comp.PropertyList[name] != null)
                        retVal = true;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Check if the property exist in the control before any kind of casting
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <param name="comp">Batch object</param>
        /// <returns>Boolean indicating the result</returns>
        private bool ExistsBatchProperty(string name, Batch batch)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(name) && batch != null)
            {
                if (batch.PropertyList.Count > 0)
                {
                    if (batch.PropertyList[name] != null)
                        retVal = true;
                }
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
            try
            {
                RegAddInsUtilities.SetDataStructure(ref ctl, base64);
                if (ctl.get_Data("chemical/x-name") != null)
                {
                    retVal = ctl.get_Data("chemical/x-name").ToString();
                }
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
                double returnMW = (double)(fragmentsMW + ctl.Objects.MolecularWeight);
                retVal = returnMW;
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
                if (fromDataBase)
                {
                    if (!bcFragment.IsNew)
                        retVal += (double)fragments.GetByID(bcFragment.FragmentID).Structure.MolWeight * bcFragment.Equivalents;
                    else
                        retVal += (double)(double.Parse(bcFragment.MW) * bcFragment.Equivalents);
                }
                else
                {
                    if (!string.IsNullOrEmpty(bcFragment.MW))
                        retVal += (double)(double.Parse(bcFragment.MW) * bcFragment.Equivalents);
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
            foreach (Fragment fragment in fragments)
            {
                if (fromDataBase)
                {
                    if (!string.IsNullOrEmpty(fragment.Structure.Formula))
                        retVal += batchComponentFragments.GetBatchComponentFragmentByFragmentID(fragment.FragmentID).Equivalents.ToString() + fragment.Structure.Formula + "\u00B7";
                }
                else
                {
                    if (!string.IsNullOrEmpty(batchComponentFragments.GetBatchComponentFragmentByFragmentID(fragment.FragmentID).Formula))
                        retVal += batchComponentFragments.GetBatchComponentFragmentByFragmentID(fragment.FragmentID).Equivalents.ToString() +
                            batchComponentFragments.GetBatchComponentFragmentByFragmentID(fragment.FragmentID).Formula + "\u00B7";
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
                if (this.GetFragmentsFormulas(fragments, batchComponentFragments, fromDataBase) != string.Empty)
                    retVal = ctl.Objects.Formula + "\u00B7" + this.GetFragmentsFormulas(fragments, batchComponentFragments, fromDataBase);
                else
                    retVal = ctl.Objects.Formula;
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

        #endregion
    }
}
