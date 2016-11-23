using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    //this class is for convenience object that allows the client configuraitn to be passed through the dataportal to the applicaton server
    //the calling object uses the enterpriselibrary configuraiton whihc is not serializable and thus can not be passed in 3 tier mode.
    [Serializable]
    internal class COECallingClientLoggingConfig
    {
        private bool _enabled = true;
        private int _priority = 0;
        private string _categories = string.Empty;
        private string _severity = string.Empty;
        private string _logEntryIdentifier = string.Empty;
        internal COECallingClientLoggingConfig()
        {
          
        }

        public string LogEntryIdentifier
        {
            get
            {
                return _logEntryIdentifier;
            }
            set
            {

                _logEntryIdentifier = value;
            }


        }


        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {

                _priority = value;
            }
           

        }


        public  string Categories
        {
            get
            {

                return _categories;
            }
            set
            {

                _categories= value;
            }

        }

        public string Severity
        {
            get
            {

                return _severity;
            }
            set
            {

                _severity = value;
            }

        }

        public bool Enabled
        {
            get
            {

                return _enabled; 
            }
            set
            {

                _enabled = value;
            }

        }
    }
}
