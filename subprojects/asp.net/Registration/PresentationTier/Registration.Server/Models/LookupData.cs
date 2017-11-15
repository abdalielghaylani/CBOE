using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class LookupData
    {
        public LookupData()
        {
        }

        [JsonConstructor]
        public LookupData(
            JArray users,
            JArray fragments,
            JArray fragmentTypes,
            JArray identifierTypes,
            JArray pickList,
            JArray pickListDomains,
            JArray projects,
            JArray sites,
            JArray formGroups,
            JArray customTables,
            List<SettingData> systemSettings,
            JArray addinAssemblies,
            JArray propertyGroups,
            JArray homeMenuPrivileges,
            JArray userPrivileges,
            JArray disabledControls,
            JObject systemInformation
        )
        {
            Users = users;
            Fragments = fragments;
            FragmentTypes = fragmentTypes;
            IdentifierTypes = identifierTypes;
            PickList = pickList;
            PickListDomains = pickListDomains;
            Projects = projects;
            Sites = sites;
            FormGroups = formGroups;
            CustomTables = customTables;
            SystemSettings = systemSettings;
            AddinAssemblies = addinAssemblies;
            PropertyGroups = propertyGroups;
            HomeMenuPrivileges = homeMenuPrivileges;
            UserPrivileges = userPrivileges;
            DisabledControls = disabledControls;
            SystemInformation = systemInformation;
        }

        /// <summary>
        /// Gets or sets the user array
        /// </summary>
        [JsonProperty(PropertyName = "users")]
        public JArray Users { get; set; }

        /// <summary>
        /// Gets or sets the fragment array
        /// </summary>
        [JsonProperty(PropertyName = "fragments")]
        public JArray Fragments { get; set; }

        /// <summary>
        /// Gets or sets the fragment type array
        /// </summary>
        [JsonProperty(PropertyName = "fragmentTypes")]
        public JArray FragmentTypes { get; set; }

        /// <summary>
        /// Gets or sets the identifier type array
        /// </summary>
        [JsonProperty(PropertyName = "identifierTypes")]
        public JArray IdentifierTypes { get; set; }

        /// <summary>
        /// Gets or sets the pick-list
        /// </summary>
        [JsonProperty(PropertyName = "pickList")]
        public JArray PickList { get; set; }

        /// <summary>
        /// Gets or sets the pick-list domain array
        /// </summary>
        [JsonProperty(PropertyName = "pickListDomains")]
        public JArray PickListDomains { get; set; }

        /// <summary>
        /// Gets or sets the project array
        /// </summary>
        [JsonProperty(PropertyName = "projects")]
        public JArray Projects { get; set; }

        /// <summary>
        /// Gets or sets the site array
        /// </summary>
        [JsonProperty(PropertyName = "sites")]
        public JArray Sites { get; set; }

        /// <summary>
        /// Gets or sets the unit array
        /// </summary>
        [JsonProperty(PropertyName = "units")]
        public JArray Units { get; set; }

        /// <summary>
        /// Gets or sets the form-group array
        /// </summary>
        [JsonProperty(PropertyName = "formGroups")]
        public JArray FormGroups { get; set; }

        /// <summary>
        /// Gets or sets the custom-table array
        /// </summary>
        [JsonProperty(PropertyName = "customTables")]
        public JArray CustomTables { get; set; }

        /// <summary>
        /// Gets or sets the system setting  array
        /// </summary>
        [JsonProperty(PropertyName = "systemSettings")]
        public List<SettingData> SystemSettings { get; set; }

        /// <summary>
        /// Gets or sets the addin assembly  array
        /// </summary>
        [JsonProperty(PropertyName = "addinAssemblies")]
        public JArray AddinAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the property group  array
        /// </summary>
        [JsonProperty(PropertyName = "propertyGroups")]
        public JArray PropertyGroups { get; set; }

        /// <summary>
        /// Gets or sets the home menu privilege  array
        /// </summary>
        [JsonProperty(PropertyName = "homeMenuPrivileges")]
        public JArray HomeMenuPrivileges { get; set; }

        /// <summary>
        /// Gets or sets the user privilege array
        /// </summary>
        [JsonProperty(PropertyName = "userPrivileges")]
        public JArray UserPrivileges { get; set; }

        /// <summary>
        /// Gets or sets the disabled control array
        /// </summary>
        [JsonProperty(PropertyName = "disabledControls")]
        public JArray DisabledControls { get; set; }

        /// <summary>
        /// Gets or sets the system information data
        /// </summary>
        [JsonProperty(PropertyName = "systemInformation")]
        public JObject SystemInformation { get; set; }
    }
}