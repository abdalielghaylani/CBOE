using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.RegistrationAdmin;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// List class for Sequences; classes controlling the metadata determining how
    /// registration numbers are created.
    /// </summary>
    [Serializable()]
    public class SequenceList :
        RegistrationBusinessListBase<SequenceList, Sequence>, IKeyValueListHolder
    {

        /// <summary>
        /// Returns the list sequences based on the type of sequence
        /// </summary>
        /// <param name="sequenceTypeID">Means that is CompoundSeq or Registry Seq</param>
        /// <returns>List of found sequences</returns>
        /// 
        [COEUserActionDescription("GetSequenceList")]
        public static SequenceList GetSequenceList(int sequenceTypeID)
        {
            try
            {
                return DataPortal.Fetch<SequenceList>(new Criteria(sequenceTypeID));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Gets the sequence list by user.
        /// </summary>
        /// <param name="sequenceTypeID">Means that is CompoundSeq or Registry Seq</param>
        /// <param name="userID">The person ID.</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("GetSequenceList")]
        public static SequenceList GetSequenceListByPersonID(int sequenceTypeID, int personID)
        {
            try
            {
                return DataPortal.Fetch<SequenceList>(new Criteria(sequenceTypeID, personID));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [Serializable()]
        protected class Criteria
        {
            #region Variables

            private int _sequenceTypeID = 0;
            private int _userID = 0;
            
            #endregion

            #region Properties

            public int SequenceTypeID
            {
                get { return _sequenceTypeID; }
            }
            public int UserID
            {
                get { return _userID; }
            }

            #endregion

            #region Constructors

            public Criteria() { }
            public Criteria(int sequenceTypeID)
            {
                _sequenceTypeID = sequenceTypeID;
            }
            public Criteria(int sequenceTypeID, int userID)
                : this(sequenceTypeID)
            {
                _userID = userID;
            }

            #endregion
        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
            using (SafeDataReader dr = criteria.UserID > 0 ? this.RegDal.GetSequenceListByPersonID(
                criteria.SequenceTypeID, criteria.UserID) : this.RegDal.GetSequenceList(criteria.SequenceTypeID)
                )
            {
                this.Fetch(dr);
            }
        }

        private void Fetch(SafeDataReader reader)
        {
            while (reader.Read())
            {
                this.Add(Sequence.NewSequence(
                    reader.GetInt32("SEQUENCEID")
                    , reader.GetInt32("ROOTNUMBERLENGTH")
                    , reader.GetString("PREFIX")
                    )
                );
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<IdentifierList");
            //if (this.IsDirty)
            //    builder.Append(" update=\"yes\"");
            //builder.Append(">");
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            builder.Append("</IdentifierList>");
            return builder.ToString();
        }

        #region IKeyValueListHolder Members

        public System.Collections.IDictionary KeyValueList
        {
            get
            {
                System.Collections.Specialized.HybridDictionary dictionary = new System.Collections.Specialized.HybridDictionary(this.Count);
                foreach (Sequence sequence in this)
                    dictionary.Add(sequence.ID, sequence.Prefix);
                return dictionary;
            }
        }

        #endregion
    }
}

