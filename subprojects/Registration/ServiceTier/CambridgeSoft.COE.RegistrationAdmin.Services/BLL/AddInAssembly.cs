using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class AddInAssembly
    {

        #region Variables

        private string _assemblyName;

        private string _publicToken;

        private string _fullName;

        private string _version;

        private List<AddInClass> _classList;

        private string _path;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return _assemblyName;
            }
        }

        public string PublicToken
        {
            get
            {
                return _publicToken;
            }
        }

        public List<AddInClass> ClassList
        {
            get
            {
                return _classList;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        #endregion

        #region Constructors

        [COEUserActionDescription("CreateAddInAssembly")]
        public AddInAssembly(Assembly assembly)
            : this()
        {
            try
            {
                this._fullName = assembly.GetName().FullName;
                this._assemblyName = assembly.GetName().Name;
                this._version = assembly.GetName().Version.ToString();
                this._classList = this.FillClasses(assembly.GetTypes());
                this._publicToken = this.MakePublicToken(assembly.GetName().GetPublicKey());
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public AddInAssembly()
        {
            this._fullName = string.Empty;
            this._assemblyName = string.Empty;
            this._version = string.Empty;
            this._classList = null;
            this._publicToken = string.Empty;
            this._path = string.Empty;
        }


        #endregion

        #region Methods

        private List<AddInClass> FillClasses(Type[] types)
        {
            List<AddInClass> clasList = new List<AddInClass>();
            foreach (Type type in types)
            {
                if (type.GetInterface(typeof(IAddIn).FullName) != null)
                {
                    AddInClass addinClass = new AddInClass(type);

                    clasList.Add(addinClass);
                }
            }
            return clasList;
        }
        private string MakePublicToken(byte[] publicToken)
        {
            StringBuilder sb = new StringBuilder(publicToken.Length * 2);
            foreach (byte b in publicToken)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }


        public AddInClass GetClassByName(string className)
        {
            foreach (AddInClass addInClass in this._classList)
            {
                if ((addInClass.NameSpace + "." + addInClass.Name) == className)
                    return addInClass;
            }
            return null;
        }

        #endregion
    }
}
