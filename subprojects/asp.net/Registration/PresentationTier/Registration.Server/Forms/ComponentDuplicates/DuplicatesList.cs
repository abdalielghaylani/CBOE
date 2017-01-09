using System;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PerkinElmer.COE.Registration.Server.Forms.ComponentDuplicates
{
    public class DuplicatesList : IEnumerable
    {
        private string _duplicatesXml;
        private bool _removeNonDuplicates;
        private bool _isMixture;

        public RegistryRecord this[int index]
        {
            get {
                return DuplicatedRecords[index];
            }
            set {
                DuplicatedRecords[index] = value;
            }
        }
        public int Count
        {
            get {
                return this.DuplicatedRecords.Count;
            }
        }

        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get {
                return _currentIndex;
            }
            set
            {
                _currentIndex = Math.Min(Math.Max(0, value), DuplicatedRecords.Count - 1);
            }
        }

        private RegistryRecordList _duplicatedRecords = null;
        private RegistryRecordList DuplicatedRecords
        {
            get
            {
                if (_duplicatedRecords == null)
                {
                    string registryList = string.Empty;

                    //if (CurrentSolvingCompound != null)
                    //{
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_duplicatesXml);

                    //XmlNode compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + CurrentSolvingCompound.ID + "']").ParentNode;

                    registryList = doc.SelectSingleNode("REGISTRYLIST").OuterXml;
                    //}

                    _duplicatedRecords = this.GetDuplicatedRecords(registryList);
                                        
                    /*if (_duplicatedRecords.Count == 0)
                        throw new Exception("No duplicates found");*/

                    if(_removeNonDuplicates)
                        RemoveNonActualDuplicates();

                    _currentIndex = 0;
                }

                return _duplicatedRecords;
            }
        }

        /// <summary>
        /// Check the Regnum valid or not 
        /// </summary>
        /// <returns>Valid to use RegNum.</returns>
        private bool IsAValidRegNumber(RegistryRecord registryRecord,XmlNode xRegNode )
        {
            // Can add more conditions to validate duplicatelist 
            if (xRegNode != null && registryRecord != null) //Coverity fix - CID 19032 
            {
                if (!string.IsNullOrEmpty(xRegNode.InnerText) || !string.IsNullOrEmpty(registryRecord.RegNum))
                {
                    if (xRegNode.InnerText.ToString() == registryRecord.RegNum.ToString())
                    {
                        return true; 
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check the GlobalContext object matches the duplicatesXml or not 
        /// </summary>
        /// <returns>RegistryRecordList from GlobalContext or from DB.</returns>
        private RegistryRecordList GetDuplicatedRecords(string registryList)
        {
            try
            {
                bool isAValidDuplicateList = true; 
                if (Csla.ApplicationContext.GlobalContext["DuplicatedRecords"] == null)
                     isAValidDuplicateList = false;
                else 
                {
                    RegistryRecordList duplicateList = (RegistryRecordList)Csla.ApplicationContext.GlobalContext["DuplicatedRecords"];
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(registryList);
                    XmlNodeList xRegNodeList = doc.SelectNodes("REGISTRYLIST/REGNUMBER");
                    if (xRegNodeList!= null && duplicateList.Count == xRegNodeList.Count)
                    {
                        for (int currentIndex = 0; currentIndex < duplicateList.Count - 1; currentIndex++)
                        {
                            XmlNode xRegNode = xRegNodeList[currentIndex];
                            RegistryRecord registryRecord = duplicateList[currentIndex];
                            if (!IsAValidRegNumber(registryRecord,xRegNode))
                            {
                                isAValidDuplicateList=false;
                                break;
                            }
                        }
                    }
                    else
                    { isAValidDuplicateList = false; }
                }
                if (isAValidDuplicateList)
                {
                    return (RegistryRecordList)Csla.ApplicationContext.GlobalContext["DuplicatedRecords"];
                }
                Csla.ApplicationContext.GlobalContext["DuplicatedRecords"] = RegistryRecordList.GetList(registryList);
                return (RegistryRecordList)Csla.ApplicationContext.GlobalContext["DuplicatedRecords"];
            }
            catch
            {
                Csla.ApplicationContext.GlobalContext["DuplicatedRecords"] = RegistryRecordList.GetList(registryList);
                return (RegistryRecordList)Csla.ApplicationContext.GlobalContext["DuplicatedRecords"];
            }
        }
        private void RemoveNonActualDuplicates()
        {
            List<RegistryRecord> notActualDuplicate = new List<RegistryRecord>();

            this.CurrentIndex = 0;
            for(int currentIndex = 0; currentIndex < this.Count; currentIndex++, this.CurrentIndex++)
                if(!this.CurrentHasSameFragments)
                    notActualDuplicate.Add(this.Current);

            foreach(RegistryRecord currentNotActualDuplicate in notActualDuplicate)
                this.Remove(currentNotActualDuplicate);
        }

        public RegistryRecord Current
        {
            get
            {
                if(_currentIndex < this.DuplicatedRecords.Count)
                    return this.DuplicatedRecords[_currentIndex];

                return null;
            }
        }

        public bool CurrentHasSameFragments
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_duplicatesXml);

                //XmlNode compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + CurrentSolvingCompound.ID + "']").ParentNode;

                //Ulises confirmed with David that "same fragments" now means having the very same fragments and equivalents in each fragment.
                //return bool.Parse(doc.SelectSingleNode("REGISTRYLIST").ChildNodes[_currentIndex].Attributes["SAMEFRAGMENT"].Value) && CurrentHasSameEquivalents;
                //LJB in the new mixture duplicate checking we should NOT be considering fragments
                if (_isMixture)
                {
                    return true;
                }
                else
                {
                    return bool.Parse(doc.SelectSingleNode(string.Format("REGISTRYLIST/REGNUMBER[. ='{0}']", Current.RegNumber.RegNum)).Attributes["SAMEFRAGMENT"].Value) && CurrentHasSameEquivalents;

                }
            }

        }

        public bool CurrentHasSameEquivalents
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_duplicatesXml);

                //XmlNode compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + CurrentSolvingCompound.ID + "']").ParentNode;
                //return bool.Parse(doc.SelectSingleNode("REGISTRYLIST").ChildNodes[_currentIndex].Attributes["SAMEEQUIVALENT"].Value);

                //LJB in the new mixture duplicate checking we should NOT be considering fragments
                if (_isMixture)
                {
                    return true;
                }
                else
                {
                    return bool.Parse(doc.SelectSingleNode(string.Format("REGISTRYLIST/REGNUMBER[. ='{0}']", Current.RegNumber.RegNum)).Attributes["SAMEEQUIVALENT"].Value);

                }
            }
        }

        public int CurrentCompoundId
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_duplicatesXml);

                //XmlNode compound = doc.SelectSingleNode("//COMPOUNDLIST/COMPOUND/TEMPCOMPOUNDID[. ='" + CurrentSolvingCompound.ID + "']").ParentNode;
                return int.Parse(doc.SelectSingleNode("REGISTRYLIST").ChildNodes[_currentIndex].Attributes["CompoundID"].Value);
            }
        }

        public int CurrentCompoundIndex
        {
            get
            {
                int id = CurrentCompoundId;

                for (int index = 0; index < this.Current.ComponentList.Count; index++)
                    if (this.Current.ComponentList[index].Compound.ID == id)
                        return index;

                return 0;
            }
        }

        public DuplicatesList(string duplicatesXml, bool removeNonDuplicates, bool isMixture)
        {
            this._duplicatesXml = duplicatesXml;
            this._duplicatedRecords = null;
            this._currentIndex = 0;
            this._removeNonDuplicates = removeNonDuplicates;
            this._isMixture = isMixture;
        }

        public DuplicatesList(string duplicatesXml,  bool isMixture)
            : this(duplicatesXml, false, isMixture)
        {
        }

        public bool Remove(RegistryRecord duplicate)
        {
            if(_duplicatedRecords.Remove(duplicate)) 
            {
                _currentIndex = Math.Min(_currentIndex, this.Count - 1);
                return true;
            }
            
            return false;
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new DuplicatesEnumerator(this.DuplicatedRecords);
        }

        #endregion
    }

    public class DuplicatesEnumerator : IEnumerator
    {
        private int currentIndex;

        public RegistryRecordList _registryRecordList;

        public DuplicatesEnumerator(RegistryRecordList registryRecordList)
        {
            Reset();

            _registryRecordList = registryRecordList;
        }

        public object Current
        {
            get {
                if (0 <= currentIndex && currentIndex < _registryRecordList.Count)
                    return _registryRecordList[currentIndex];

                throw new InvalidOperationException();
            }
        }

        public bool MoveNext()
        {
            if (currentIndex+1 < _registryRecordList.Count)
            {
                currentIndex++;
                return true;
            }
            return false;

        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }
}
