using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class AssemblyList
    {
        #region Variables

        List<AddInAssembly> _assemblyList;

        #endregion

        #region Properties

        public List<AddInAssembly> Assemblies
        {
            get 
            {
                return _assemblyList;
            } 
        }     

        #endregion

        #region Costructors

        public AssemblyList() 
        {
            _assemblyList = new List<AddInAssembly>();
        }

        #endregion

        #region Methods

        public bool ContainsAssembly(string assemblyFullName) 
        {
            bool contains = false;
            foreach (AddInAssembly ass in this._assemblyList)
            {
                if (ass.FullName == assemblyFullName)
                {
                    contains = true;
                }
            }
            return contains;
        }

        public AddInAssembly GetAseemblyByName(string assemblyName)
        {
            foreach (AddInAssembly assembly in this._assemblyList)
            {
                if (assembly.FullName == assemblyName)
                    return assembly;
            }
            return null;
        }

        #endregion


    }
}
