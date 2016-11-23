using System;
namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// This attribute is intended to be applied to web service method to indicate
    /// the authorization (privilege) requirements to access the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class COEAuthorizationAttribute : Attribute
    {
        /// <summary>
        /// Initializes the class with the given privileges
        /// </summary>
        /// <param name="privileges">The privilege list</param>
        public COEAuthorizationAttribute(string[] privileges)
        {
            _privilegeMatches = privileges;
        }

        /// <summary>
        /// Describes an area of functionality of the web applciation which is associated
        /// with one or more individual permissions.
        /// </summary>
        public string WebAuthorizationZone;

        private string[] _privilegeMatches;
        /// <summary>
        /// Describes one or more Registration privileges.
        /// </summary>
        public string[] PrivilegeMatches
        {
            get { return _privilegeMatches; }
        }

    }
}
