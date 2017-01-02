using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Base class that MUST be implemented by each fix. Also the inheriting class should follow this naming convention:
    /// Namespace: CambridgeSoft.COE.Patcher
    /// Class Name: CSBR[Number]
    /// 
    /// Where [Number] should be replaced with the actual bug number
    /// 
    /// A public parameter less constructor must be provided whether explicitly or implicitly.
    /// </summary>
    public abstract class BugFixBaseCommand
    {
        /// <summary>
        /// This method is being called by the patch controller. All your code starts here.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public abstract List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig);
    }
}
