using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core
{
    public abstract class SourceRecordBase: ISourceRecord
    {
        protected SourceRecordBase(int recordIndex)
        {
            _sourceIndex = recordIndex;
        }

        protected int _sourceIndex;
        protected FieldValues _fieldSet = new FieldValues();

        #region ISourceRecord Members

        public int SourceIndex
        {
            get { return _sourceIndex; }
        }

        public FieldValues FieldSet
        {
            get { return _fieldSet; }
        }

        #endregion
    }
}
