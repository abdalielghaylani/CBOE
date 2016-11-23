using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class AddInClass
    {

        #region Variables 

        private string _className;

        private string _nameSpace;

        private List<string>  _handlers;

        #endregion

        #region Properties


        public string NameSpace
        {
            get 
            {
                return _nameSpace;
            }
        }

        public string Name
        {
            get 
            {
                return _className;
            }
        }

        public List<string> EventHandlerList
        {
            get 
            {
                return _handlers;
            }
        }

        #endregion

        #region Constructor

        public AddInClass(string name, List<string> eventHandlers) 
        {
            this._className = name;
            this._handlers = eventHandlers;
        }

        [COEUserActionDescription("CreateNewAddInClass")]
        public AddInClass(Type type)
        {
            try
            {
                this._className = type.Name;

                this._nameSpace = type.Namespace;

                List<string> events = new List<string>();

                MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

                foreach (MethodInfo method in methods)
                {
                    events.Add(method.Name);
                }

                this._handlers = events;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public AddInClass() 
        {
            this._className = string.Empty;
            this._nameSpace = string.Empty;
            this._handlers = null;
        }

        #endregion


    }
}
