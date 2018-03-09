// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishingSwitch.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace COEConfigurationPublishTool
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The publishing switch
    /// </summary>
    public class PublishingSwitch
    {
        /// <summary>
        /// Gets or sets the switch name.
        /// </summary>
        public string SwitchName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the switch value is required.
        /// </summary>
        public bool IsRequired
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the switch value.
        /// </summary>
        public string SwitchValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the switch value type.
        /// </summary>
        public SwitchValueType ValueType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets options switch value allowed.
        /// </summary>
        public List<string> AllowOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Validate the switch value.
        /// </summary>
        /// <returns>Validate succeed or fail</returns>
        public bool Validate()
        {
            switch (this.ValueType)
            {
                case SwitchValueType.Text:
                    return true;
                case SwitchValueType.Bool:
                    {
                        bool parseValue;
                        return bool.TryParse(this.SwitchValue, out parseValue);
                    }

                case SwitchValueType.Int:
                    {
                        int parseValue;
                        return int.TryParse(this.SwitchValue, out parseValue);
                    }

                case SwitchValueType.Options:
                    {
                        return this.AllowOptions.Where(t => t.Equals(this.SwitchValue, System.StringComparison.InvariantCultureIgnoreCase)).Count() == 1;
                    }

                default:
                    return true;
            }
        }
    }
}
