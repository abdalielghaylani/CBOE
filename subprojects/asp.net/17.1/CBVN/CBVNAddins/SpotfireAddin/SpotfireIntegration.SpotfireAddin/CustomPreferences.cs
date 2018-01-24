using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotfire.Dxp.Framework.Persistence;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Framework.Preferences;
using Spotfire.Dxp.Application;

namespace SpotfireIntegration.SpotfireAddin
{
    [PersistenceVersion(1, 0)]
    public class CustomPreferences : CustomPreference
    {
        #region Variables

        /// <summary>
        /// Variable holds the Maximum number of Rows value
        /// </summary>
        private PreferenceProperty<int> maxRows;

        /// <summary>
        /// Variable to hold the use remoting service preference
        /// </summary>
        private PreferenceProperty<bool> useRemoting;

        /// <summary>
        /// Variable to hold data compression choice preference
        /// </summary>
        private PreferenceProperty<bool> compressData;

        /// <summary>
        /// Variable holds the number of Rows per page value
        /// </summary>
        private PreferenceProperty<int> pagingSize;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of the class CustomPreferences
        /// </summary>
        public CustomPreferences()
        {
            this.maxRows =
               this.AddPreference(
                   new PreferenceProperty<int>(
                       "Maximum Row Limit",
                       "1.0",
                       PreferencePersistenceScope.Server,
                       PreferenceUsage.UserGroup,
                       5000
                       ));
            //set remoting usage default value to true
            this.useRemoting = this.AddPreference(
                new PreferenceProperty<bool>(
                    "Use Remoting",
                    "1.0",
                    PreferencePersistenceScope.Server,
                    PreferenceUsage.UserGroup,
                    true));

            //set data compression usage default value to true
            this.compressData = this.AddPreference(
                new PreferenceProperty<bool>(
                    "Compress Data",
                    "1.0",
                    PreferencePersistenceScope.Server,
                    PreferenceUsage.UserGroup,
                    true));

            this.pagingSize =
               this.AddPreference(
                   new PreferenceProperty<int>(
                       "Page Row Count",
                       "1.0",
                       PreferencePersistenceScope.Server,
                       PreferenceUsage.UserGroup,
                       500
                       ));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set cateogry
        /// </summary>
        public override string Category
        {
            get
            {
                return "Datalytix";
            }
        }

        /// <summary>
        /// Get/Set sub category
        /// </summary>
        public override string SubCategory
        {
            get
            {
                return "Configuration";
            }
        }

        /// <summary>
        /// Get/Set number of maximum rows
        /// </summary>
        internal int NumberOfMaxRows
        {
            get
            {
                return this.maxRows.Value;
            }
            set
            {
                this.maxRows.Value = value;
                this.maxRows.OnValueChanged();
            }
        }


        /// <summary>
        /// Gets or sets the UseRemoting preference value
        /// </summary>
        internal bool UseRemoting
        {
            get
            {
                return this.useRemoting.Value;
            }
            set
            {
                this.useRemoting.Value = value;
                this.useRemoting.OnValueChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data compression preference value
        /// </summary>
        internal bool CompressData
        {
            get
            {
                return this.compressData.Value;
            }
            set
            {
                this.compressData.Value = value;
                this.compressData.OnValueChanged();
            }
        }

        /// <summary>
        /// Get/Set number of page row count
        /// </summary>
        internal int PagingSize
        {
            get
            {
                return this.pagingSize.Value;
            }
            set
            {
                this.pagingSize.Value = value;
                this.pagingSize.OnValueChanged();
            }
        }

        #endregion
    }

    public class CustomPreferenceManager
    {
        #region Variables
        private static CustomPreferenceManager _customPreferenceManager;
        private static object syncObject = new object();
        AnalysisApplication _theAnalysisApplication;
        bool isCompressData = false;
        bool isUseRemoting = false;
        int numberOfMaxRow = 5000;
        int pagingSize = 500;
        #endregion

        #region Properties
        public int PagingSize
        {
            get { return pagingSize; }
        }

        public bool IsCompressData
        {
            get { return isCompressData; }
        }

        public bool IsUseRemoting
        {
            get { return isUseRemoting; }
        }

        public int NumberOfMaxRow
        {
            get { return numberOfMaxRow; }
        }
        #endregion

        #region Constructor
        private CustomPreferenceManager(AnalysisApplication theAnalysisApplication)
        {
            _theAnalysisApplication = theAnalysisApplication;
            SetCustomPreferenceValues();
        }
        #endregion

        #region Methods
        public static CustomPreferenceManager Instance(AnalysisApplication theAnalysisApplication)
        {
            lock (syncObject)
            {
                if (_customPreferenceManager == null)
                {
                    _customPreferenceManager = new CustomPreferenceManager(theAnalysisApplication);
                }
                return _customPreferenceManager;
            }
        }

        private void SetCustomPreferenceValues()
        {
            if (_theAnalysisApplication != null)
            {
                PreferenceManager thePreferenceManager = _theAnalysisApplication.GetService<PreferenceManager>();
                if (thePreferenceManager != null && thePreferenceManager.PreferenceExists<CustomPreferences>())
                {
                    CustomPreferences theCustomPreferences = thePreferenceManager.GetPreference<CustomPreferences>();
                    if (theCustomPreferences != null)
                    {
                        isCompressData = theCustomPreferences.CompressData;
                        numberOfMaxRow = theCustomPreferences.NumberOfMaxRows;
                        pagingSize = theCustomPreferences.PagingSize;
                        isUseRemoting = theCustomPreferences.UseRemoting;
                    }
                }
            }
        }
        #endregion
    }


}
