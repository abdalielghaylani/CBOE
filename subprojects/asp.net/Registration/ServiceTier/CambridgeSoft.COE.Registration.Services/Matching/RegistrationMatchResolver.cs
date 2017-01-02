using System;
using System.Collections.Generic;

using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;

/* This class's intended use is for support of a user-interface whose responsibility it is
 * to help a user 
 * */

namespace RegistrationWebApp.Code
{
    /// <summary>
    /// When initialized with a RegistryRecord instance, will immediately fetch the
    /// list of matched registrations from the database. Additionally, contains the
    /// logic to determine the available actions when comparing the 'subject' to the
    /// 'current' matched object.
    /// JED - Okay, I haven't transferrred the 'options' logic to this code yet.
    /// </summary>
    /// <remarks>
    /// The 'subject' is the item in memory against which a list of matches is generated.
    /// The 'current match' is the object against which the subject is being compared.
    /// </remarks>
    public class RegistrationMatchResolver 
    {
        #region [Properties]

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

        private bool _canUseStructure;
        public bool CanUseStructure
        {
            get { return this._canUseStructure; }
        }

        private int _currentIndex;
        /// <summary>
        /// Read-only 
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        private RegistryRecord _subject;
        /// <summary>
        /// The record against which the matching algorithm is be applied.
        /// </summary>
        public RegistryRecord Subject
        {
            get { return _subject; }
        }

        private RegistryRecord _currentMatch;
        /// <summary>
        /// The current record against which the Subject is being compared.
        /// </summary>
        public RegistryRecord CurrentMatch
        {
            get { return _currentMatch; }
        }

        private IMatchResponse _matchResponse;
        /// <summary>
        /// The list of matched registrations generated from the Subject record.
        /// </summary>
        public IMatchResponse MatchResponse
        {
            get { return _matchResponse; }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Public constructor, accepting a registration instance and a list of matched registration.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for standard Registration application workflows. The other
        /// constructors support testing and are intended solely for internal usage.
        /// </remarks>
        /// <param name="subject">
        /// An instance of a registration object.
        /// </param>
        /// <param name="matches">
        /// A match-response object that contains IDs for zero or more matched registrations.
        /// </param>
        public RegistrationMatchResolver(RegistryRecord subject, IMatchResponse matches)
        {
            try
            {
                this._subject = subject;
                this._matchResponse = matches;

                //Defend against same-record matches once initial testing is complete
                //if (subject.ID != 0)
                //    if (this.MatchResponse.MatchedItems.Contains(subject.RegNum))
                //        this.MatchResponse.MatchedItems.Remove(subject.RegNum);

            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }


        /// <summary>
        /// Used by class constructors which do not include a match-response object.
        /// </summary>
        /// <remarks>
        /// This private method helps support testing scenarios.
        /// </remarks>
        /// <param name="subject">The registration instance used for matching against</param>
        private void LoadSubject(RegistryRecord subject)
        {
            this._subject = subject;
        }

        #endregion

        /// <summary>
        /// Fetches the current record (in terms of its list index) for comparison.
        /// </summary>
        /// <param name="matchedIndex">the zero-based index of the matched Registration</param>
        public void LoadMatch(int matchedIndex)
        {
            if (_matchResponse != null)
            {
                string regNum = this._matchResponse.MatchedItems[matchedIndex];
                this._currentMatch = RegistryRecord.GetRegistryRecord(regNum);
                this._currentIndex = matchedIndex;
                DetermineAvailableActions();
            }
        }
       
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
        private void DetermineAvailableActions()
        {
            string addBatchSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableAddBatchButton");
            string useComponentSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableUseComponentButton");
            string duplicateSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableDuplicateButton");
            string useStructureSetting = RegSvcUtilities.GetSystemSetting("REGADMIN", "EnableUseStructureButton");

            bool.TryParse(addBatchSetting, out _canAddBatch);
            bool.TryParse(duplicateSetting, out _canDuplicate);
            bool.TryParse(useStructureSetting, out _canUseStructure);

            //is used a custom property to do the matching then you have to resolve it manually or you can make a duplicate
            if ((this._matchResponse.MechanismUsed != MatchMechanism.Structure) && (this._matchResponse.MechanismUsed != MatchMechanism.Mixture))
            {
                _canDuplicate = true;
                _canAddBatch = false;
                _canUseStructure = false;

                return;
            }
  
            // Set Add Batch and Use Structure availability
            if (this._matchResponse.MechanismUsed == MatchMechanism.Mixture){
                   _canAddBatch = true;
            }
            else{
                if(this._subject.SameBatchesIdentity==true){
                    _canAddBatch = CurrentHasSameFragments();
                }else{
                    _canAddBatch = true;
                }

                if (this.MatchResponse.MechanismUsed == MatchMechanism.Structure && _canUseStructure == true)
                {
                    _canUseStructure = true;
                }
            }
         }

        /// <summary>
        /// Compares the Subject and the CurrentMatch RegistryRecord instances and determines
        /// if their batches have matching fragments.
        /// </summary>
        /// <returns></returns>
        private bool CurrentHasSameFragments()
        {
            List<BatchComponentFragment> sFrags =
                ScrubFragments(this.Subject.BatchList[0].BatchComponentList[0].BatchComponentFragmentList);
            
            List<BatchComponentFragment> tFrags = 
                ScrubFragments(this.CurrentMatch.BatchList[0].BatchComponentList[0].BatchComponentFragmentList);
            //the number of fragments must be equivalent
            if ( sFrags.Count != tFrags.Count )
                return false;

            if (sFrags.Count == 0 && tFrags.Count == 0)
                return true;

            //each item in the target list must 'equal' one item in the subject list
            foreach (BatchComponentFragment sf in sFrags)
            {
                bool fragMatches = false;

                //find the fragment in the target
                foreach (BatchComponentFragment tf in tFrags)
                {
                    if (tf.FragmentID == sf.FragmentID && tf.Equivalents == sf.Equivalents)
                    {
                        fragMatches = true;
                        break;
                    }
                }
                if (!fragMatches)
                {
                    return false;
                }
            }

            //each item in the subject list must 'equal' one item in the target list
            foreach (BatchComponentFragment tf in tFrags)
            {
                bool fragMatches = false;

                //find the fragment in the target
                foreach (BatchComponentFragment sf in sFrags)
                {
                    if (sf.FragmentID == tf.FragmentID && sf.Equivalents == tf.Equivalents)
                    {
                        fragMatches = true;
                        break;
                    }
                }
                if (!fragMatches)
                {
                    return false;
                }
            }

            //if no differences could be discerned, then the fragment lists are equal
            return true;
        }

        /// <summary>
        /// Eliminates bogus fragments with a FragmentID = 0.
        /// </summary>
        /// <param name="list">the target BatchComponentFragmentList</param>
        /// <returns>a generic list of valid BatchComponentFragment objects</returns>
        private List<BatchComponentFragment> ScrubFragments(BatchComponentFragmentList list)
        {
            List<BatchComponentFragment> localList = new List<BatchComponentFragment>(list);
            foreach (BatchComponentFragment bcf in list)
            {
                if (bcf.FragmentID != null || bcf.FragmentID != 0)
                    localList.Add(bcf);
            }
            return localList;
        }

        /// <summary>
        /// Submits a valid RegistryRecord instance. Simultaneously generates an audit record (sumbitter's comments) that the
        /// pre-submission process discovered one or more matches that the submitter provided comment on.
        /// </summary>
        /// <remarks>
        /// The value of targetEntityId should logically match the suggestedAction. For example:
        /// <para>
        /// <list type="table">
        /// <listheader><term>Action</term><description>Target Entity</description></listheader>
        /// <item><term>AddCompoundBatch</term><description>a single-compound registry</description></item>
        /// <item><term>UseStructure</term><description>a structure</description></item>
        /// <item><term>DuplicateMixture</term><description>a multi-compound registration (optional)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="suggestedAction">a resolution mechanism, as defined by the ResolutionAction enum</param>
        /// <param name="submitterComments">user-entered comment text intended to help indicate a course of action</param>
        /// <param name="targetEntityId">the internal ID of the entity that has been suggested to act upon</param>
        public void ResolveSubject(ResolutionAction suggestedAction, string submitterComments, int targetEntityId)
        {
            //TODO: add a constructor-based value-object to cram into the RegistryRecord someplace.
            /*
             * This class (SubmitterSuggestion ?) will carry the submitter's comments and be inserted into the database.
             * It will have to be put in a table that is accessible by both temporary and permanent records, or it will
             * have to be 'migrated' when a record is converted from temp to perm.
             * 
             * Still not sure how we're going to do that...sounds easy until you start thinking about it, lol!
             * */
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// this method is used by the registratar
        /// </summary>
        /// <param name="action"></param>
        public void ResolveSubmission(ResolutionAction action)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
