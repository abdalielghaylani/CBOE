using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.DocumentManager.Services.Types;
using Csla.Core;
using System.Xml;
using System.Reflection;

namespace CambridgeSoft.COE.DocumentManager.Services.AddIns
{
    class AddInsManager
    {
        private static AddInsManager _manager;
        public Document _Document;
        private List<IAddIn> _addInList;
        private static XmlDocument _eventsDefinition = new XmlDocument();

        public static XmlNode AddInsDefinition
        {
            get
            {
                return _eventsDefinition.FirstChild;
            }
        }
        public static AddInsManager GetManager(Document Document)
        {
            if (_manager == null)
                _manager = new AddInsManager(Document);

            return _manager;
        }

        private AddInsManager()
        {
            this._addInList = new List<IAddIn>();
       }

        ~AddInsManager()
        {
        }

        private AddInsManager(Document Document)
            : this()
        {
            this._Document = Document;
        }

        public void Add(object businessObject, string xmlSnippet)
        {
            _eventsDefinition = new XmlDocument();
            _eventsDefinition.LoadXml(xmlSnippet);
            foreach (XmlNode addInNode in _eventsDefinition.SelectNodes("/AddIns/AddIn"))
            {
                string assembly = addInNode.Attributes["assembly"] != null ? addInNode.Attributes["assembly"].Value : string.Empty;
                string className = addInNode.Attributes["class"] != null ? addInNode.Attributes["class"].Value : string.Empty;
                IAddIn addIn = InstantiateAddIn(assembly, className);

                XmlNode configurationNode = addInNode.SelectSingleNode("AddInConfiguration");
                if (configurationNode != null)
                    addIn.Initialize(configurationNode.OuterXml);

                foreach (XmlNode eventToListen in addInNode.SelectNodes("./Event"))
                {
                    string eventName = eventToListen.Attributes["eventName"] != null ? eventToListen.Attributes["eventName"].Value : string.Empty;
                    string eventHandler = eventToListen.Attributes["eventHandler"] != null ? eventToListen.Attributes["eventHandler"].Value : string.Empty;
                    SubscribeAddInToEvent(businessObject, addIn, eventName, eventHandler);
                }
                addIn.Document = (IDocument)_Document;
                _addInList.Add(addIn);
            }
        }

        private IAddIn InstantiateAddIn(string assemblyName, string className)
        {
            IAddIn addIn;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);

                addIn = (IAddIn)assembly.CreateInstance(className.Trim());
            }
            else
            {
                Type addInClass = Type.GetType(className.Trim());
                if (addInClass == null)
                    throw new Exception("The class does not exist: " + className);

                System.Reflection.ConstructorInfo addInDefaultConstructor = addInClass.GetConstructor(System.Type.EmptyTypes);
                if (addInDefaultConstructor == null)
                    throw new Exception("Missing default constructor");

                addIn = (IAddIn)addInDefaultConstructor.Invoke(null);
            }

            return addIn;
        }

        private void SubscribeAddInToEvent(object businessObject, IAddIn addIn, string eventName, string eventHandler)
        {
            System.Reflection.EventInfo eventDelegate = businessObject.GetType().GetEvent(eventName);
            if (eventDelegate != null)
            {
                Type eventHandlerType = eventDelegate.EventHandlerType;

                System.Reflection.MethodInfo handlerMethod = addIn.GetType().GetMethod(eventHandler);
                if (handlerMethod != null)
                {

                    Delegate d = Delegate.CreateDelegate(eventHandlerType, addIn, handlerMethod);
                    eventDelegate.AddEventHandler(businessObject, d);
                }
                else
                    throw new Exception(string.Format("Couldn't find Event Handler '{0}' - It HAS to be public", eventHandler));
            }
            else
                throw new Exception(string.Format("Invalid Event '{0}' for object of type {1}", eventName, businessObject.GetType().ToString()));
        }

    }
}
