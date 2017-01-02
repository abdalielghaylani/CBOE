using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Web.SessionState;
using Csla.Validation;
using System.Collections;
using System.Collections.Generic;
using ChemDrawControl15;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Framework.Common.Validation;


namespace RegistrationWebApp.Forms.ComponentDuplicates.ContentArea
{
    [Serializable]
    public class DuplicatesResolver
    {
        #region [ Properties and members ]

        private string _duplicatesXml;

        private bool _isPreReg;

        private DuplicatesList _duplicates;
        public DuplicatesList Duplicates
        {
            get
            {
                return _duplicates;
            }
        }

        private Compound _currentSolvingCompound = null;
        public Compound CurrentSolvingCompound
        {
            get
            {
                return _currentSolvingCompound;
            }
        }

        private CompoundList _compoundsToResolve;
        public CompoundList CompoundsToResolve
        {
            get
            {
                bool checkIDs = false ;
                if(_compoundsToResolve == null)
                {
                    _compoundsToResolve = CompoundList.NewCompoundList();

                    if(!string.IsNullOrEmpty(_duplicatesXml))
                    {
                        _registryRecord.EnsureComponentUniqueness();

                        foreach(Component component in _registryRecord.ComponentList)
                        {
                            
                            string ids = _duplicatesXml;
                            string compoundid = string.Empty;
                            if (_isPreReg)
                            {
                                 compoundid = "0";
                                 checkIDs =ids.ToLower().Contains("<tempcompoundid>" + compoundid + "</tempcompoundid>");
                            }
                            else
                            {
                                 compoundid = component.Compound.ID.ToString();
                                 checkIDs =ids.ToLower().Contains("<tempcompoundid>" + compoundid + "</tempcompoundid>");
                           }
                            if (checkIDs)
                            {
                                string formula = string.Empty;
                                if (component.Compound.BaseFragment.Structure.MolWeight == 0)
                                {
                                    ChemDrawCtl chemDrawCtr = new ChemDrawCtl();
                                    chemDrawCtr.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(component.Compound.BaseFragment.Structure.Value));
                                    chemDrawCtr.Objects.Select();
                                    component.Compound.BaseFragment.Structure.MolWeight = chemDrawCtr.Objects.MolecularWeight;
                                    formula = chemDrawCtr.Objects.Formula;

                                }
                                if (string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.Formula)) 
                                {
                                    if (formula != string.Empty)
                                        component.Compound.BaseFragment.Structure.Formula = formula;
                                    else
                                    {
                                        ChemDrawCtl chemDrawCtr = new ChemDrawCtl();
                                        chemDrawCtr.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(component.Compound.BaseFragment.Structure.Value));
                                        chemDrawCtr.Objects.Select();
                                        component.Compound.BaseFragment.Structure.Formula = chemDrawCtr.Objects.Formula;
                                    }
                                }
                                if (string.IsNullOrEmpty(component.Compound.RegNumber.RegNum))
                                    component.Compound.RegNumber.RegNum = "N/A";
                                _compoundsToResolve.Add(component.Compound);
                            }
                        }

                        //Force checking if actual duplicates on every component due to registration current limitation: mixtures with SBI=false are not supported.
                        for(int currentIndex = 0; currentIndex < this._compoundsToResolve.Count; currentIndex++)
                            this.SelectCompoundToSolve(currentIndex);
                    }
                }

                return _compoundsToResolve;
            }
        }

        public bool HasUnsolvedComponents
        {
            get
            {
                return this.CompoundsToResolve.Count > 0;
            }
        }

        private bool _canAddBatch;
        public bool CanAddBatch
        {
            get { return _canAddBatch; }
        }

        private bool _canDuplicate;
        public bool CanDuplicate
        {
            get { return _canDuplicate; }
        }

        private bool _canCreateCompoundForm;
        public bool CanCreateCompoundForm
        {
            get { return _canCreateCompoundForm; }
        }

        private bool _canUseStructure;
        public bool CanUseStructure
        {
            get { return this._canUseStructure; }
        }

        [NonSerialized]
        RegistryRecord _registryRecord;
        public RegistryRecord RegistryRecord 
        {
            get { return this._registryRecord; }
        }

       

        private ResolvingBetweenEnum ResolvingBetween
        {
            get
            {
                try
                {
                    if (this._registryRecord.IsSingleCompound && this.Duplicates.Current.IsSingleCompound)
                        return ResolvingBetweenEnum.Singles;
                    else if (this._registryRecord.IsSingleCompound && !this.Duplicates.Current.IsSingleCompound)
                        return ResolvingBetweenEnum.SingleMixture;
                    else if (!this._registryRecord.IsSingleCompound && this.Duplicates.Current.IsSingleCompound)
                        return ResolvingBetweenEnum.MixtureSingle;
                    else if (!this._registryRecord.IsSingleCompound && !this.Duplicates.Current.IsSingleCompound)
                        return ResolvingBetweenEnum.Mixtures;
                    else
                        return ResolvingBetweenEnum.Undefined;
                }
                catch
                {
                    return ResolvingBetweenEnum.Undefined;
                }
            }
        }

        #endregion

        /// <summary>
        /// First of all, there are 4 first-class system settings that control the availability of the 4
        /// possible duplicate actions:
        ///     EnableAddBatchButton
        ///     EnableDuplicateButton
        ///     EnableUseComponentButton
        ///     EnableUseStructureButton
        /// Only when all these settings have 'True' value, the following specific logic applies:
        /// 1. Duplicate should always be available
        /// 2. Only when the record to register is a Mixture, Use Component is available.
        /// 3. Use Structure should always be available.
        /// 4. Add Batch should be available when:
        /// 	a. (SBI=true AND Duplicates.CurrentHasSameFragments=true) OR (SBI=false)
        /// Supplement:
        /// 1. In case of Mixture duplicate scenario, disable Add Batch and Use Structure.
        /// </summary>
        public void DetermineAvailableActions()
        {
            _canAddBatch = _canCreateCompoundForm = _canDuplicate = _canUseStructure = false;

            string addBatchSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableAddBatchButton");
            string useComponentSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableUseComponentButton");
            string duplicateSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableDuplicateButton");
            string useStructureSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableUseStructureButton");

            bool.TryParse(addBatchSetting, out _canAddBatch);
            bool.TryParse(useComponentSetting, out _canCreateCompoundForm);
            bool.TryParse(duplicateSetting, out _canDuplicate);
            bool.TryParse(useStructureSetting, out _canUseStructure);

      
            //even if the buttons are enabled in configuration, they must be disabled in certain conditions
            //although there are very few changes needed, the code has been made very verbose 
            //so the cases are clear
            if (RegUtilities.GetMixturesEnabled())//covers how to handle a duplicate components when mixtures are enabled
            {
                if(RegUtilities.GetFragmentsEnabled()){
                    if (Duplicates.Current.IsSingleCompound)//conflicting component being looked at is from a  single component registry
                    { 
                        if (this.RegistryRecord.IsSingleCompound)//submission is from single component
                        {
                            _canCreateCompoundForm = _canCreateCompoundForm && ((_registryRecord.SameBatchesIdentity && Duplicates.CurrentHasSameFragments));
                            //fix for csbr-164592: Add Batch button missing from Duplicate Resolution screen simplified mixture workflow settings 
                            _canAddBatch = _canAddBatch && ((_registryRecord.SameBatchesIdentity && Duplicates.CurrentHasSameFragments) || !_registryRecord.SameBatchesIdentity) ;
                        }
                        else //this means submission is a mixture
                        {
                            _canCreateCompoundForm = _canCreateCompoundForm && ((_registryRecord.SameBatchesIdentity && Duplicates.CurrentHasSameFragments) || !_registryRecord.SameBatchesIdentity);
                           _canAddBatch = false;
                        }
                    }
                    else //means that the component being resolved is from a mixture
                    {
                        if (this.RegistryRecord.IsSingleCompound)//submission is single component
                        {
                            _canAddBatch = false;
                        }
                        else //submission is a mixture
                        {
                            _canAddBatch = false;
                        }
                    }
                }else{//fragments are not considered
                    if (Duplicates.Current.IsSingleCompound)//conflicting component being looked at is from a  single component registry
                    {
                        if (this.RegistryRecord.IsSingleCompound)//submission is from single component
                        {
                            //no changes needed
                        }
                        else //this means submission is a mixture
                        {
                            _canAddBatch = false; 
                        }
                    }
                    else //means that the component being resolved is from a mixture
                    {
                        if (this.RegistryRecord.IsSingleCompound)//submission is single component
                        {
                            _canAddBatch = false; 
                        }
                        else //submission is a mixture
                        {
                            _canAddBatch = false; 
                        }
                    }
                    
                }

              

            }
            else //this means it is standard single component againt single component. Fragments should be consider
            {
                if (RegUtilities.GetFragmentsEnabled())
                {                
                    _canCreateCompoundForm = _canCreateCompoundForm &&  ((_registryRecord.SameBatchesIdentity && Duplicates.CurrentHasSameFragments));
                    _canAddBatch = _canAddBatch && ((_registryRecord.SameBatchesIdentity && Duplicates.CurrentHasSameFragments)|| !_registryRecord.SameBatchesIdentity); 
                }
                else
                {
                    //no changes needed

                }
            }

            //override duplicate button
            OverrideDuplicateButtonVisibility();
        }

        public void OverrideDuplicateButtonVisibility()
        { //this is not quite right.  need to figure out if the current duplicate is a component or a full record
            if (Duplicates.Current.IsSingleCompound)
            {
                if (!this.RegistryRecord.CanCreateDuplicateComponent())
                {
                    _canDuplicate = false;
                }
            }
            else
            {
                if (!this.RegistryRecord.CanCreateDuplicateRegistry())
                {

                    _canDuplicate = false;

                }
            }
        }

        private string GetRegistryDuplicateListForSelectedCompound(int index)
        {
            if(!string.IsNullOrEmpty(_duplicatesXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_duplicatesXml);

                XmlNode compound = null;

                if(CurrentSolvingCompound != null)
                    if(_isPreReg){
                        compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + "0" + "']").ParentNode;
                    }
                    else
                    {
                        string compoundId = CurrentSolvingCompound.ID.ToString() ;
                        compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + compoundId + "']").ParentNode;
                    }
                else
                    compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND");

                return compound.SelectSingleNode("REGISTRYLIST").OuterXml;
            }

            return string.Empty;
        }

        public DuplicatesResolver(RegistryRecord registryRecord, string duplicatesXml, bool isPreReg)
        {
            this._registryRecord = registryRecord;
            _duplicatesXml = duplicatesXml;
            _compoundsToResolve = null;
            _isPreReg = isPreReg;
           
        }

        public void SelectCompoundToSolve(int index)
        {
            this._currentSolvingCompound = this.CompoundsToResolve[index];

            if(!string.IsNullOrEmpty(_duplicatesXml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_duplicatesXml);

                XmlNode compound = null;

                if(CurrentSolvingCompound != null)
                    if(_isPreReg){
                        compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + "0" + "']").ParentNode;
                    }
                    else
                    {
                        string compoundId = CurrentSolvingCompound.ID.ToString();
                      
                        compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + compoundId + "']").ParentNode;
                    }
                else
                    compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND");

                _duplicates = new DuplicatesList(compound.SelectSingleNode("REGISTRYLIST").OuterXml, RegistryRecord.ComponentList.Count>1);

                this.DetermineAvailableActions();
                //RemoveNonActualDuplicates();
            }
        }

        /// <summary>
        /// Remove compounds that are not actual duplicates on current component due to registration current limitation: mixtures with SBI=false are not supported.
        /// </summary>
        private void RemoveNonActualDuplicates()
        {
            //Remove compounds that are not actual duplicates on current component due to registration current limitation: mixtures with SBI=false are not supported.
            if(this._registryRecord.SameBatchesIdentity /*|| !this._registryRecord.IsSingleCompound*/)
            {
                List<RegistryRecord> notActualDuplicate = new List<RegistryRecord>();

                _duplicates.CurrentIndex = 0;
                for(int currentIndex = 0; currentIndex < _duplicates.Count; currentIndex++, _duplicates.CurrentIndex++)
                    if(!_duplicates.CurrentHasSameFragments)
                        notActualDuplicate.Add(_duplicates.Current);

                foreach(RegistryRecord currentNotActualDuplicate in notActualDuplicate)
                    _duplicates.Remove(currentNotActualDuplicate);

                if(_duplicates.Count == 0)
                    this.CreateDuplicate();
            }
        }

        public void SelectCompoundToSolveById(int tempId)
        {
            for(int index = 0; index < this.CompoundsToResolve.Count; index++)
            {
                Compound currentCompound = this.CompoundsToResolve[index];

                if(currentCompound.ID == tempId)
                {
                    SelectCompoundToSolve(index);
                    break;
                }
            }
        }

        private Compound GetNextCompoundToResolve()
        {
            if(this.CompoundsToResolve.Count > 0)
                return _currentSolvingCompound = this.CompoundsToResolve[0];

            return null;
        }

         /// <summary>
        /// Basically, do nothing. Ignores the duplicate and continues with the registration process.
        /// </summary>
        /// <returns>Next compound to solve or null if there are no more.</returns>
        public RegistryRecord CreateDuplicate()
        {
            return CreateDuplicate(false, string.Empty);
        }
        /// <summary>
        /// Basically, do nothing. Ignores the duplicate and continues with the registration process.
        /// </summary>
        /// <returns>Next compound to solve or null if there are no more.</returns>
        public RegistryRecord CreateDuplicate(bool isPreRegistration, string submitterComments)
        {
            if(CanDuplicate)
            {
                
                
                    string message = string.Empty;
                    this.CompoundsToResolve.Remove(CurrentSolvingCompound);

                    if (!HasUnsolvedComponents)
                    {
                        if (isPreRegistration)
                        {
                            this._registryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_DuplicateComponent, Csla.ApplicationContext.User.Identity.Name,  submitterComments);
                            this._registryRecord.Save();
                            return this._registryRecord;
                        }
                        else
                        {
                            if (this._registryRecord.IsTemporal)
                                this._registryRecord.Register(DuplicateCheck.None);
                            else
                            {
                                this._registryRecord.CheckOtherMixtures = false;
                                this._registryRecord.Save(DuplicateCheck.None);
                            }
                        }
                    }
                    else
                    {
                        this.SelectCompoundToSolve(0);
                    }
               
               

                return _registryRecord;
            }
            else
                throw new InvalidOperationException( Resources.Resource.DuplicateNotAllowed_Exception );
        }

        /// <summary>
        /// CreateCompoundForm (alias UseThisCompound), reuses the currently selected compound (that belongs to registries on database) on the user registry. It also copies the fragments.
        /// </summary>
        /// <returns></returns>
        public RegistryRecord CreateCompoundForm()
        {
            return CreateCompoundForm(false, string.Empty);
        }
        /// <summary>
        /// CreateCompoundForm (alias UseThisCompound), reuses the currently selected compound (that belongs to registries on database) on the user registry. It also copies the fragments.
        /// </summary>
        /// <returns></returns>
        public RegistryRecord CreateCompoundForm(bool isPreRegistration,string submitterComments)
        {
            if(CanCreateCompoundForm)
            {
               
                string originalXml = string.Empty;
                try
                {

                    Compound compoundToReplaceWith = this.Duplicates.Current.ComponentList[this.Duplicates.CurrentCompoundIndex].Compound;
                    

                    originalXml = this._registryRecord.XmlWithAddIns;

                    for(int componentIndex = 0; componentIndex < this._registryRecord.ComponentList.Count; componentIndex++)
                    {
                        Component currentComponent = this._registryRecord.ComponentList[componentIndex];

                        if(this.CurrentSolvingCompound.ID == currentComponent.Compound.ID)
                        {
                            if(RegistryRecord.SameBatchesIdentity) // copy fragments when SBI = true
                                _registryRecord.ReplaceCompound(componentIndex, compoundToReplaceWith, this.Duplicates.Current.BatchList[0].BatchComponentList[this.Duplicates.CurrentCompoundIndex].BatchComponentFragmentList);
                            else // disregard fragments when SBI = false.
                                _registryRecord.ReplaceCompound(componentIndex, compoundToReplaceWith);
                            break;
                        }
                    }

                    if(this._registryRecord.IsValid)
                        this.CompoundsToResolve.Remove(CurrentSolvingCompound);

                    if(!HasUnsolvedComponents)
                    {
                        if (isPreRegistration)
                        {
                            this._registryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_CreateCompountFormComponent, Csla.ApplicationContext.User.Identity.Name, Duplicates.Current.RegNum, submitterComments);
                            this._registryRecord.Save();
                            while (CompoundsToResolve.Count > 0)
                                this.CompoundsToResolve.Remove(CompoundsToResolve[CompoundsToResolve.Count - 1]);
                            return this._registryRecord;
                        }
                        else
                        {
                            if(this._registryRecord.IsTemporal)
                            {
                                if(this._registryRecord.ComponentList.Count > 1)
                                    this._registryRecord.Register(DuplicateCheck.MixCheck);
                                else // avoid mixture duplicates check for single-compound.
                                    this._registryRecord.Register(DuplicateCheck.None);
                            }
                            else
                            {
                                this._registryRecord.CheckOtherMixtures = false;

                                if (this._registryRecord.ComponentList.Count > 1)
                                    this._registryRecord.Save(DuplicateCheck.MixCheck);
                                else // avoid mixture duplicates check for single-compound.
                                {
                                    _registryRecord.UpdateFragments();
                                    this._registryRecord.Save(DuplicateCheck.None);
                                    //this._registryRecord.Register();
                                }
                            }
                        }
                    }
                        else{
                            this.SelectCompoundToSolve(0);
                        }
                

                    return _registryRecord;
                }
                catch(ValidationException validationException)
                {
                    _registryRecord.InitializeFromXml(originalXml, false, true);
                    throw validationException;
                }
            
            }
            else
                throw new InvalidOperationException( Resources.Resource.CannotCreateCompoundForm_Exception );
        
        }


        /// <summary>
        /// CreateCompoundForm (alias UseThisCompound), reuses the currently selected compound (that belongs to registries on database) on the user registry. It also copies the fragments.
        /// </summary>
        /// <returns></returns>
        public RegistryRecord AutoCreateCompoundForm()
        {
            if (CanCreateCompoundForm)
            {

                string originalXml = string.Empty;
                try
                {

                    Compound compoundToReplaceWith = this.Duplicates.Current.ComponentList[this.Duplicates.CurrentCompoundIndex].Compound;


                    originalXml = this._registryRecord.XmlWithAddIns;

                    for (int componentIndex = 0; componentIndex < this._registryRecord.ComponentList.Count; componentIndex++)
                    {
                        Component currentComponent = this._registryRecord.ComponentList[componentIndex];

                        if (this.CurrentSolvingCompound.ID == currentComponent.Compound.ID)
                        {
                            if (RegistryRecord.SameBatchesIdentity) // copy fragments when SBI = true
                                _registryRecord.ReplaceCompound(componentIndex, compoundToReplaceWith, this.Duplicates.Current.BatchList[0].BatchComponentList[this.Duplicates.CurrentCompoundIndex].BatchComponentFragmentList);
                            else // disregard fragments when SBI = false.
                                _registryRecord.ReplaceCompound(componentIndex, compoundToReplaceWith);
                            break;
                        }
                    }

                    if (this._registryRecord.IsValid)
                        this.CompoundsToResolve.Remove(CurrentSolvingCompound);

                    if (!HasUnsolvedComponents)
                    {
                        
                            if (this._registryRecord.IsTemporal)
                            {
                                
                                    this._registryRecord.Register(DuplicateCheck.MixCheck);
                               
                            }
                            else
                            {
                                this._registryRecord.CheckOtherMixtures = false;

                                if (this._registryRecord.ComponentList.Count > 1)
                                {
                                    this._registryRecord.Save(DuplicateCheck.MixCheck);
                                }
                               
                            }
                        }
                    
                    else
                    {
                        this.SelectCompoundToSolve(0);
                    }


                    return _registryRecord;
                }
                catch (ValidationException validationException)
                {
                    _registryRecord.InitializeFromXml(originalXml, false, true);
                    throw validationException;
                }

            }
            else
                throw new InvalidOperationException(Resources.Resource.CannotCreateCompoundForm_Exception);
        }


        public RegistryRecord AddBatch()
        {
            return AddBatch(false, string.Empty);
        }

        public RegistryRecord AddBatch(bool isPreRegistration, string submitterComments)
        {
            if(CanAddBatch)
            {
                    //CSBR 155767: validate that the batch being added has not broken any rules
                    //create a clone rather than using directly from the list.  If there is a validation error
                    //you need to start over with the same dupliate
                    //without the clone, you are directly editing the record in the duplicatelist.
                    RegistryRecord selectedDuplicate = Duplicates.Current.Clone();
               
                    if (isPreRegistration)
                    {
                        this._registryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_AddBatchComponent, Csla.ApplicationContext.User.Identity.Name, Duplicates.Current.RegNum, submitterComments);
                        this._registryRecord.Save();
                        while (CompoundsToResolve.Count > 0)
                            this.CompoundsToResolve.Remove(CompoundsToResolve[CompoundsToResolve.Count - 1]);
                        return this._registryRecord;

                    }
                    else
                    {
                      
                        string message = string.Empty;
                        this._registryRecord.CheckOtherMixtures = false;
                        bool isTemporal = this._registryRecord.IsTemporal;
                        foreach (Batch currentBatch in this._registryRecord.BatchList)
                        {
                            currentBatch.BatchComponentList[0].ComponentIndex = selectedDuplicate.ComponentList[Duplicates.CurrentCompoundIndex].ComponentIndex;
                            currentBatch.BatchComponentList[0].CompoundID = selectedDuplicate.ComponentList[Duplicates.CurrentCompoundIndex].Compound.ID;
                        }
                        if (isTemporal)
                        {
                            this._registryRecord.BatchList[0].TempBatchID = this._registryRecord.ID;
                            selectedDuplicate.AddBatch(this._registryRecord.BatchList[0]);
                            selectedDuplicate.CheckOtherMixtures = false;
                            selectedDuplicate.UpdateFragments();

                            //CSBR 155767: validate that the batch being added has not broken any rules
                            if (!selectedDuplicate.IsValid)
                            {
                                DisplayBrokenRules(selectedDuplicate.GetBrokenRulesDescription());
                                return this._registryRecord;
                            }
                            else
                            {
                                selectedDuplicate.Save(DuplicateCheck.None);

                                //Next two lines are to make the record deletable
                                this._registryRecord.Status = RegistryStatus.Submitted;
                                this._registryRecord.SetApprovalStatus();

                                this._registryRecord.Delete();
                                this._registryRecord.Save();
                            }
                        }
                        else
                        {
                            //ljbthis only comes up when you edit a structure that is already registered that has a duplicate check performed on the editted structure
                            foreach (Batch currentBatch in this._registryRecord.BatchList)
                            {
                                selectedDuplicate.InsertBatch(currentBatch);
                                selectedDuplicate.CheckOtherMixtures = false;
                                selectedDuplicate.UpdateFragments();

                                //CSBR 155767: validate that the batch being added has not broken any rules
                                if (!selectedDuplicate.IsValid)
                                {
                                    DisplayBrokenRules(selectedDuplicate.GetBrokenRulesDescription());
                                    return this._registryRecord;
                                }
                                else
                                {
                                    selectedDuplicate.Save(DuplicateCheck.None);
                                                                 
                                }
                            }

                            //Next two lines are to make the record deletable
                            this._registryRecord.Status = RegistryStatus.Submitted;
                            this._registryRecord.SetApprovalStatus(); 
                            //finally delete the registry record sinnce all the batches have been moved
                            RegistryRecord.DeleteRegistryRecord(this._registryRecord.RegNum);
                            

                        }



                       
                    }
                    while (CompoundsToResolve.Count > 0)
                        this.CompoundsToResolve.Remove(CompoundsToResolve[CompoundsToResolve.Count - 1]);
                return selectedDuplicate;
            }
            else
                throw new InvalidOperationException( Resources.Resource.AddingBatchNotAllowed_Exception );
        }

        public RegistryRecord UseStructure()
        {
            return UseStructure(false, string.Empty);
        }

        public RegistryRecord UseStructure(bool isPreRegistration, string submitterComments) 
        {
            if (CanUseStructure)
            {   
                string originalXml = string.Empty;
                try
                {
                    Structure structureToReplaceWith = this.Duplicates.Current.ComponentList[this.Duplicates.CurrentCompoundIndex].Compound.BaseFragment.Structure;


                    for (int componentIndex = 0; componentIndex < this._registryRecord.ComponentList.Count; componentIndex++)
                    {
                        Component currentComponent = this._registryRecord.ComponentList[componentIndex];

                        if (this.CurrentSolvingCompound.ID == currentComponent.Compound.ID)
                                _registryRecord.ReplaceStructure(componentIndex, structureToReplaceWith);
                    }

                    originalXml = this._registryRecord.XmlWithAddIns;            
                    this.CompoundsToResolve.Remove(CurrentSolvingCompound);

                    if (!HasUnsolvedComponents)
                    {
                        if (isPreRegistration)
                        {
                            this._registryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_UseStructureComponent, Csla.ApplicationContext.User.Identity.Name, Duplicates.Current.RegNum, submitterComments);
                            this._registryRecord.Save();
                            while (CompoundsToResolve.Count > 0)
                                this.CompoundsToResolve.Remove(CompoundsToResolve[CompoundsToResolve.Count - 1]);
                            return this._registryRecord;
                        }
                        else
                        {
                            if (this._registryRecord.IsTemporal)
                            {
                                if (this._registryRecord.ComponentList.Count > 1)
                                    this._registryRecord.Register(DuplicateCheck.MixCheck);
                                else // avoid mixture duplicates check for single-compound.
                                    this._registryRecord.Register(DuplicateCheck.None);
                            }
                            else
                            {
                                this._registryRecord.CheckOtherMixtures = false;

                                if (this._registryRecord.ComponentList.Count > 1)
                                    this._registryRecord.Save(DuplicateCheck.MixCheck);
                                else // avoid mixture duplicates check for single-compound.
                                {                                
                                    this._registryRecord.Save(DuplicateCheck.None);                               
                                }
                            }

                        }
                    }
                        else
                        {
                            this.SelectCompoundToSolve(0);
                        }
                   
                   

                    return _registryRecord;
                }
                catch (ValidationException validationException)
                {
                    _registryRecord.InitializeFromXml(originalXml, false, true);
                    throw validationException;
                }
             
            
            }
            else
                throw new InvalidOperationException("Can't create compound form on this context.");
            
        }

       

        private enum ResolvingBetweenEnum
        {
            Undefined,
            Singles,
            SingleMixture,
            MixtureSingle,
            Mixtures
        }
        private void DisplayBrokenRules(List<BrokenRuleDescription> brokenRules)
        {
            this.DisplayBrokenRules(null, brokenRules);
        }

        private void DisplayBrokenRules(Exception exception, List<BrokenRuleDescription> brokenRules)
        {
            string errorMessage = exception == null ? string.Empty : exception.Message + "<BR/>";

            foreach (BrokenRuleDescription currentBrokenRule in brokenRules)
            {
                foreach (string currentError in currentBrokenRule.BrokenRulesMessages)
                    errorMessage += string.Format("{0}<br>", currentError);
            }
            throw new InvalidOperationException(errorMessage);
        }
    }
}
