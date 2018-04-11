using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.Messaging
{
    [Serializable()]
    public class ReportBuilderMeta
    {
        #region variables
        int _id;
        string _name;
        string _description;
        string _className;
        string _config;
        bool _readOnly;
        #endregion

        #region Properties
        public int Id
        {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        public string Class
        {
            get 
            {
                return _className;
            }
            set 
            {
                _className = value;
            }
        }

        public string Config
        {
            get 
            {
                return _config;
            }
            set 
            {
                _config = value;
            }
        }

        public string Name
        {
            get {
                return _name;
            }
            set 
            {
                _name = value;
            }
        }

        public string Description
        {
            get {
                return _description;
            }
            set {
                _description = value;
            }
        }

        public bool ReadOnly
        {
            get {
                return _readOnly;
            }
            set {
                _readOnly = value;
            }
        }

        #endregion

        #region Methods
        #region Constructors
        public ReportBuilderMeta()
        { 
        }

        public ReportBuilderMeta(int id, string name, string description, string className) : this()
        {
            this._id = id;
            this._name = name;
            this._description = description;
            this._className = className;
        }
        
        public ReportBuilderMeta(int id, string name, string description, string className, bool readOnly) : this(id, name, description, className)
        {
            this.ReadOnly = readOnly;
        }
        #endregion
        #endregion
    }
}
