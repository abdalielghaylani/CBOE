using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Framework.Services;

namespace CambridgeSoft.COE.DataLoader.UnitTests.MockObjects
{
    public class MockSourceRecord : ISourceRecord
    {
        private int _index = -1;
        private FieldValues _fieldSet = null;

        public MockSourceRecord()
        {
            _fieldSet = new FieldValues();
        }

        #region ISourceRecord Members

        public int SourceIndex
        {
            get { return _index; }
        }

        public FieldValues FieldSet
        {
            get 
            {
                return _fieldSet;
            }
        }

        #endregion
    }

    /// <summary>
    /// Mimic the current implementation of RegistryRecord. By default, create 1 Batch.
    /// </summary>
    public class MockRegistryRecord : IDestinationRecord
    {
        private string _regNum = string.Empty;
        private bool _isValid = false;
        private MockBatchList _batchList = null;
        private MockPropertyList _propertyList = null;
        private string[] _nameHistory = null;

        public MockRegistryRecord()
        {
            _batchList = new MockBatchList();
            _propertyList = new MockPropertyList();
            _nameHistory = new string[10];
        }

        public void SetComponentProperty(string propertyListKey, string propertyValue)
        {
            if (_propertyList.Contains(propertyListKey))
            {
                _propertyList[propertyListKey].Value = propertyValue;
            }
            else
            {
                MockProperty property = new MockProperty(propertyListKey);
                property.Value = propertyValue;

                _propertyList.Add(property);
            }
        }

        public string RegNum
        {
            get { return _regNum; }
            set { _regNum = value; }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }

        public MockBatchList BatchList
        {
            get { return _batchList; }
        }

        public MockPropertyList PropertyList
        {
            get { return _propertyList; }
        }

        public string[] NameHistory
        {
            get { return _nameHistory; }
            set { _nameHistory = value; }
        }
    }

    public class MockBatchList
    {
        private string _description = string.Empty;
        private IList<MockBatch> _batchList = null;

        public MockBatchList()
        {
            _batchList = new List<MockBatch>();
            _batchList.Add(new MockBatch());
        }

        public MockBatch this[int index]
        {
            get { return _batchList[index]; }
            set { _batchList[index] = value; }
        }

        public void Add(MockBatch batch)
        {
            _batchList.Add(batch);
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    public class MockBatch
    {
        private DateTime _dateCreated;
        private MockPropertyList _propertyList = null;
        private string[] _scientists = null;

        public MockBatch()
        {
            _propertyList = new MockPropertyList();
            _scientists = new string[10];
        }

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

        public MockPropertyList PropertyList
        {
            get { return _propertyList; }
        }

        public string this[int index]
        {
            get { return _scientists[index]; }
            set { _scientists[index] = value; }
        }
    }

    public class MockPropertyList
    {
        private IList<MockProperty> _propertyList = new List<MockProperty>();

        public MockProperty this[int index]
        {
            get { return _propertyList[index]; }
            set { _propertyList[index] = value; }
        }

        public MockProperty this[string propertyName]
        {
            get
            {
                foreach (MockProperty currentProperty in _propertyList)
                    if (currentProperty.Name == propertyName.Trim())
                        return currentProperty;

                return null;
            }
        }

        public void Add(MockProperty property)
        {
            _propertyList.Add(property);
        }

        public bool Contains(string propertyListKey)
        {
            foreach (MockProperty property in _propertyList)
            {
                if (string.Compare(property.Name, propertyListKey, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class MockProperty
    {
        private string _name = string.Empty;
        private string _value = string.Empty;

        public MockProperty(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
