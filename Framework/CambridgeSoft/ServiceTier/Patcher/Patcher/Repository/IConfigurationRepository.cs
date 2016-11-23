using System.Xml;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Patcher.Repository
{
    /// <summary>
    /// The interface for all types of potential Repositories that deal with all kinds of configuration
    /// used in Registration system.
    /// Note:
    /// 1. We deal with only 1 domain concept, namely the form xml, so there's no need to create a
    /// generic Repository. If we find in the future such need, or have the requirement that
    /// different types of configuration should come from different source, we could split this
    /// currently central Repository into a bunch of more specific ones as well as possibly a generic
    /// Repository interface.
    /// 2. This is a very simplistic version of Repository pattern, in that, firstly we don't have a
    /// strong-typed system for different kinds of configurations, making us work with xml directly;
    /// seconly we don't introduce the Query/Criteria concept due to the simplicity of our
    /// requirement.
    /// 3. We return all 'domain objects' in a generic IEnumerable collection to preserve for future
    /// enhancement, such as applying Linq/IQueryable.
    /// </summary>
    public interface IConfigurationRepository
    {
        IEnumerable<XmlDocument> GetAllCoeFormGroups();
        IEnumerable<XmlDocument> GetAllCoeDataViews();
        IEnumerable<XmlDocument> GetAllConfigurationSettings();
        XmlDocument GetCoeObjectConfig();
        XmlDocument GetFrameworkConfig();
        XmlDocument GetSingleCoeFormGroupById(string formGroupId);
        XmlDocument GetSingleCOEDataViewById(string dataViewId);
    }
}
