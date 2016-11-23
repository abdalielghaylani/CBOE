using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// This class serve to define any configuration data needed by the search service. 
    /// Properties can be added as needed. Each peopery will be added to the add element in the searchserviceData parent element
    /// <code lang="Xml">
    ///   &lt;chemDrawEmbed&gt;
    ///     &lt;add showPluginDownload="YES" pluginDownloadURL="http://www.cambridgesoft.com/login/?service=ChemDrawPlugin" /&gt;
    ///   &lt;/chemDrawEmbed&gt;
    /// </code>
    /// </summary>
    public class ChemDrawEmbedData : COENamedElementCollection<NameValueConfigurationElement>
    {
        private const string showPluginDownloadProperty = "showPluginDownload";
        private const string pluginDownloadURLProperty = "pluginDownloadURL";
        private const string downloadChemDrawImageSrcProperty = "downloadChemDrawImageSrc";

        /// <summary>
        /// Initialize a new instance of the <see cref="ChemDrawEmbedData"/> class.
        /// </summary>
        public ChemDrawEmbedData() {
        }

        /// <summary>
        /// Indicates if the chemdraw embed should display a download image (in search modes).
        /// </summary>
        public string ShowPluginDownload {
            get {
                if(this.Get(showPluginDownloadProperty) != null)
                    return this.Get(showPluginDownloadProperty).Value;
                else
                    return string.Empty;
            }
            set { this.Get(showPluginDownloadProperty).Value = value; }
        }

        /// <summary>
        /// Indicates which is the url to get the plugin.
        /// </summary>
        public string PluginDownloadURL {
            get {
                if(this.Get(pluginDownloadURLProperty) != null)
                    return this.Get(pluginDownloadURLProperty).Value;
                else
                    return string.Empty;
            }
            set { this.Get(pluginDownloadURLProperty).Value = value; }
        }

        /// <summary>
        /// Indicates where is located the image to be displayed.
        /// </summary>
        public string DownloadChemDrawImageSrc {
            get {
                if(this.Get(downloadChemDrawImageSrcProperty) != null)
                    return this.Get(downloadChemDrawImageSrcProperty).Value;
                else
                    return string.Empty;
            }
            set { this.Get(downloadChemDrawImageSrcProperty).Value = value; }
        }
    }
}
