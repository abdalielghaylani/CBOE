using System;
using System.Collections.Generic;
using System.Text;

namespace CBVNStructureFilterSupport.Framework
{
    /// <summary>
    /// RenderSettings
    /// </summary>
    [Serializable]
    public class RenderSettings
    {
        /// <summary>
        /// the equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return true;
        }

        /// <summary>
        /// Get the default settings
        /// </summary>
        /// <returns>the default settings</returns>
        public static RenderSettings GetDefaultSettings()
        {
            return new RenderSettings();
        }
    }
}
