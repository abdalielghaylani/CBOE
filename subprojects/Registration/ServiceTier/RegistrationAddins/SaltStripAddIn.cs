using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using System.Collections;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using ChemDrawControl18;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    /// <summary>
    /// AddIn used by the bulk loader (and potentially others) to break up Compound.BaseFragment.Structure 
    /// and populate the fragments. See design doc under Registration\Documents\Design in 11.0.x branch
    /// </summary>
    [Serializable]
    class SaltStripAddIn : IAddIn
    {

        /// <summary>
        /// Cache key for the salkt-stripping ChemDraw control
        /// </summary>
        private string _cacheKey;

        /// <summary>
        /// By default, this add-in will NOT eliminate fragments from the original drawing unless
        /// the configuration overrides this setting.
        /// </summary>
        private bool _editChemDrawing = false;

        private StructureTargets _source = StructureTargets.Normalized;  //default is to look at normalized
        private StructureTargets _target = StructureTargets.Normalized;  //default is to write back to normalized
        //configuration values that allow turning off addin based on a value
        private RegAddInsUtilities.PropertyListType _propertyListType = RegAddInsUtilities.PropertyListType.NotSet;
        private string[] _disableAddinValueArray = null;
        private string _propertyName = string.Empty;
        
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("SaltStripAddIn");

        private enum StructureTargets
        {
            BaseFragment,
            Normalized,
            Both,
        }
        #region Events Handlers

        /// <summary>
        /// This is the publically-available event our RegistryRecord instances will delegate to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnEventHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            //LJB  don't allow addins that work on components to run if AllowUnRegisteredComponents if false and componentlistcount>1)
            if ((_registryRecord.AllowUnregisteredComponents == false && _registryRecord.ComponentList.Count == 1) || (_registryRecord.AllowUnregisteredComponents == true))
            {

                if (!RegAddInsUtilities.DisableAddIn(_registryRecord, _propertyListType, _propertyName, _disableAddinValueArray))
                {
                    //create a batch
                    Batch batch = _registryRecord.BatchList[0];

                    //JED: Hmmm...is it safe to be only concerned with the zeroth batch?

                    foreach (Component component in _registryRecord.ComponentList)
                    {
                        //BatchComponent batchComp = batch.BatchComponentList[0];

                        BatchComponentList batchComps = batch.BatchComponentList;
                        foreach (BatchComponent batchComp in batchComps)
                        {
                            if (batchComp.ComponentIndex == component.ComponentIndex)
                            {
                                this.StripSalts(component.Compound, batchComp);
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// This method separates all the sub-structures from a 'compound' drawing and attempts to
        /// separate salt and solvate fragments from the base fragment. This is done by matching against
        /// the fragments repository containing the standard CambridgeSoft fragment-set as well as any
        /// customer-added fragment definitions.
        /// </summary>
        /// <param name="compound">the compound of interest (having zero or more sub-structures)</param>
        /// <param name="batchComp">the batch-component instance for this compound</param>
        private void StripSalts(Compound compound, BatchComponent batchComp)
        {
            //list of stripped fragments. If same fragment is drawn twice, it will be in the list twice.
            ArrayList strippedFragments = new ArrayList();

            //loop through compound structures in _registryRecord
            ChemDrawCtl ctrl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            ctrl.AllowInvisibleView = true;
            string mimeType = "chemical/x-cdx";  //"chemical/smiles"; //
            try
            {
                bool bNormalized = compound.BaseFragment.Structure.UseNormalizedStructure && (RegistryRecord.IsTemporal || compound.BaseFragment.Structure.IsBeingRegistered);
                if (_source == StructureTargets.Normalized && bNormalized)
                {
                    RegAddInsUtilities.SetDataStructure(ref ctrl, compound.BaseFragment.Structure.NormalizedStructure);
                }
                else
                {
                    RegAddInsUtilities.SetDataStructure(ref ctrl, compound.BaseFragment.Structure.Value);
                }
                if (ctrl.Groups.Count > 1)
                {
                    //create list for holding groups to be deleted
                    List<Group> groupsToDelete = new List<Group>();

                    foreach (ChemDrawControl18.Group group in ctrl.Groups)
                    {
                        if (group.GroupType == CDGroupType.kCDGroupTypeFragment)
                        {
                            //get structure from group
                            string structure = Convert.ToString(group.Objects.get_Data(mimeType, null, null, null));

                            //check if the fragment is a known salt/solvate
                            Fragment matchedFrag = this.GetMatchedFragment(structure);

                            //transfer salt/solvate information to the fragments list and eliminate the group
                            if (matchedFrag != null)
                            {
                                //determine if this fragment is already in this compound's fragment list prior to salt stripping 
                                //e.g. added in ELN by the salts listener but not present in the drawing (see design doc)
                                //Fragment existingFrag = compound.FragmentList.GetByCode(matchedFrag.Code);  --this was failing
                                Fragment existingFrag = compound.FragmentList.GetByID(matchedFrag.FragmentID);   //suggested by Jeff Dugas 22-Oct-2010

                                if (existingFrag == null)
                                    strippedFragments.Add(matchedFrag);

                                //strip out even if fragment was already present
                                groupsToDelete.Add(group);
                            }
                        }
                    }

                    //Now go through strippedFragmentList and add these stripped fragments to the fragment list on the compound
                    foreach (Fragment frag in strippedFragments)
                    {
                        AddFragmentDetails(frag, compound, batchComp);
                    }

                    //now delete any groups that were matched
                    foreach (Group group in groupsToDelete)
                        group.Objects.Clear();

                    //get rid of the group itself if only one fragment is left
                    if (ctrl.Groups.Count == 2)
                    {
                        foreach (ChemDrawControl18.Group group in ctrl.Groups)
                        {
                            if (group.GroupType == CDGroupType.kCDGroupTypeGroup)
                            {
                                group.Delete();
                            }
                        }
                    }

                    //JED: We just want to re-create the structure. Clearly we now have a normalized
                    // structure left over, but 'Normalization' itself might be turned off!
                    //string newCdx = Convert.ToString(ctrl.Objects.get_Data(mimeType, null, null, null));
                    //compound.BaseFragment.Structure.Value = newCdx;
                    //compound.BaseFragment.Structure.NormalizedStructure = newCdx;

                    //SJ: I was told that normalisation addin works off the normalised structure
                    //copy to BaseFragment.structure.normalisedStructure
                    //compound.BaseFragment.Structure.NormalizedStructure = Convert.ToString(ctrl.Objects.get_Data(mimeType, null, null, null));

                    //JED: That is incorrect...normalization *uses* the value of the drawn structure
                    //     and *sets* the value of the normalized structure:
                    //
                    // Also, we don't carefully control the order the add-ins fire in. Therefore, if this
                    // addin has fired AFTER the normalizer, we have to replace THAT structure too, since
                    // it will still have salts in it.

                    if (_editChemDrawing)
                    {
                        string newCdx = Convert.ToString(ctrl.Objects.get_Data(mimeType, null, null, null));

                        switch (_target)
                        {
                            case StructureTargets.BaseFragment:
                                compound.BaseFragment.Structure.Value = newCdx;
                                break;
                            case StructureTargets.Normalized:
                                if (bNormalized)
                                    compound.BaseFragment.Structure.NormalizedStructure = newCdx;
                                else
                                    compound.BaseFragment.Structure.Value = newCdx;
                                break;
                            case StructureTargets.Both:
                                compound.BaseFragment.Structure.Value = newCdx;
                                compound.BaseFragment.Structure.NormalizedStructure = newCdx;
                                break;
                        }

                    }
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
        }

        /// <summary>
        /// Retrieves a list of fragments whose structure matched the filter-structure value.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="structureToMatch">the fragment structure used to filter matches</param>
        /// <returns></returns>
        private Fragment GetMatchedFragment(string structureToMatch)
        {
            FragmentList matchedFrags = null;
            Fragment matchedFrag = null;

            //do a structure search on fragments in repository
            matchedFrags = FragmentList.GetFragmentList(structureToMatch);
            if (matchedFrags != null && matchedFrags.Count > 0)
            {
                matchedFrag = matchedFrags[0].Clone();
            }

            return matchedFrag;
        }

        /// <summary>
        /// Adds a fragment to the compound in question, or if it already exists, increments its equivalence.
        /// </summary>
        /// <param name="matchedFragment">the repository-derived fragment which matched the query structure</param>
        /// <param name="containingCompound">the compound for which we want to update the fragment list</param>
        /// <param name="batchComponent">the batch-component containing the fragment-equivalence information</param>
        private void AddFragmentDetails(Fragment matchedFragment, Compound containingCompound, BatchComponent batchComponent)
        {
            //determine if this fragment is already in this compound's fragment list
            Fragment existingFrag = containingCompound.FragmentList.GetByCode(matchedFragment.Code);

            if (existingFrag != null)
            {
                //This fragment is already listed: increase the number of equivalents
                foreach (BatchComponentFragment batchCompFrag in batchComponent.BatchComponentFragmentList)
                {
                    if (batchCompFrag.FragmentID == existingFrag.FragmentID)
                    {
                        batchCompFrag.Equivalents++;
                        break;
                    }
                }
            }
            else
            {
                //No matching compound fragment was listed, so add it ...
                containingCompound.FragmentList.Add(matchedFragment);

                //...and add the batch-component fragment with 1 equivalent
                BatchComponentFragment newBatchComponentFrag =
                    BatchComponentFragment.NewBatchComponentFragment(matchedFragment.FragmentID, 1);
                newBatchComponentFrag.Formula = matchedFragment.Formula;
                newBatchComponentFrag.MW = matchedFragment.MW;
                batchComponent.BatchComponentFragmentList.Add(newBatchComponentFrag);
            }
        }

        #region IAddIn Members

        private IRegistryRecord _registryRecord;

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
            //TODO: Determine any remaining configuration details
            if (!string.IsNullOrEmpty(xmlConfiguration))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlConfiguration);

                XmlNode xmlNode = document.SelectSingleNode("AddInConfiguration/EditChemDrawing");
                if (xmlNode != null)
                {
                    string boolValue = xmlNode.InnerText;
                    bool.TryParse(boolValue, out _editChemDrawing);
                }


                XmlNode xmlNode2 = document.SelectSingleNode("AddInConfiguration/Source");
                if (xmlNode2 != null)
                {
                    if (Enum.IsDefined(typeof(StructureTargets), xmlNode2.InnerText))
                        _source = (StructureTargets)Enum.Parse(typeof(StructureTargets), xmlNode2.InnerText);
                }

                XmlNode xmlNode3 = document.SelectSingleNode("AddInConfiguration/Target");
                if (xmlNode3 != null)
                {
                    if (Enum.IsDefined(typeof(StructureTargets), xmlNode3.InnerText))
                        _target = (StructureTargets)Enum.Parse(typeof(StructureTargets), xmlNode3.InnerText);
                }
               

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
            }
        }

        #endregion
    }
}
