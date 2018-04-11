using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using Csla;
using Csla.Core;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services.AddIns
{
    class AddInsManager
    {
        private static AddInsManager _manager;
        public RegistryRecord _registryRecord;
        private static XmlDocument _eventsDefinition = new XmlDocument();

        //JED: begin
        private bool _useCache = true;
        const string _serviceName = "COEAddInsManager";
        static COELog _coeLog = COELog.GetSingleton(_serviceName);
        private PersistentAddInWrapper _persistentAddIn;
        //JED: end

        public static XmlNode AddInsDefinition
        {
            get
            {
                return _eventsDefinition.FirstChild;
            }
        }
        public static AddInsManager GetManager(RegistryRecord registryRecord)
        {
            if (_manager == null)
                _manager = new AddInsManager(registryRecord);

            return _manager;
        }

        private AddInsManager() { }

        private AddInsManager(RegistryRecord registryRecord)
            : this()
        {
            this._registryRecord = registryRecord;
        }

        public void Add(object businessObject, string xmlSnippet)
        {
            _eventsDefinition = new XmlDocument();
            _eventsDefinition.LoadXml(xmlSnippet);
            foreach (XmlNode addInNode in _eventsDefinition.SelectNodes("/AddIns/AddIn"))
            {
                if (addInNode.Attributes["enabled"] != null && addInNode.Attributes["required"] != null)
                {
                    if (addInNode.Attributes["required"].Value == "yes" || addInNode.Attributes["enabled"].Value == "yes")
                    {
                        string assembly = addInNode.Attributes["assembly"] != null ? addInNode.Attributes["assembly"].Value : string.Empty;
                        string className = addInNode.Attributes["class"] != null ? addInNode.Attributes["class"].Value : string.Empty;
                        string friendlyName = addInNode.Attributes["friendlyName"] != null ? addInNode.Attributes["friendlyName"].Value : string.Empty;

                        //JED: begin
                        IAddIn addIn;
                        XmlNode configurationNode = addInNode.SelectSingleNode("AddInConfiguration");
                        if (_useCache == true && !string.IsNullOrEmpty(friendlyName))
                            addIn = InstantiateAddIn(assembly, className, friendlyName, configurationNode);
                        else
                        {
                            addIn = InstantiateAddIn(assembly, className);
                            if (configurationNode != null)
                            {
                                string msg = string.Format("Creating {0} add-in", friendlyName);
                                _coeLog.LogStart(msg);
                                addIn.Initialize(configurationNode.OuterXml);
                                _coeLog.LogEnd(msg);
                            }
                        }
                        //JED: end
                        //Original code
                        //addIn = InstantiateAddIn(assembly, className);

                        foreach (XmlNode eventToListen in addInNode.SelectNodes("./Event"))
                        {
                            string eventName = eventToListen.Attributes["eventName"] != null ? eventToListen.Attributes["eventName"].Value : string.Empty;
                            string eventHandler = eventToListen.Attributes["eventHandler"] != null ? eventToListen.Attributes["eventHandler"].Value : string.Empty;
                            SubscribeAddInToEvent(businessObject, addIn, eventName, eventHandler);
                        }
                        addIn.RegistryRecord = (IRegistryRecord)_registryRecord;
                    }
                }
                else 
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
                    addIn.RegistryRecord = (IRegistryRecord)_registryRecord;
                }
            }
        }

        /// <summary>
        /// A cache-enabled wrapper for add-in instantiation.
        /// </summary>
        /// <see cref="InstantiateAddIn"/>
        /// <param name="assemblyName">The anme of the assembly to create an instance of</param>
        /// <param name="className">The calss to instantiate through refelection</param>
        /// <param name="friendlyName">The name of the add-in, used as part of the cache key.</param>
        /// <returns>An instance of an IAddIn object</returns>
        private IAddIn InstantiateAddIn(
            string assemblyName
            , string className
            , string friendlyName
            , XmlNode configurationNode
            )
        {
            IAddIn addIn;

            //not sure how this cache feels about keys with spaces in the name
            friendlyName = friendlyName.Replace(" ", "_");

            string cacheKeyTemplate = "{0}.{1}.AddIn.{2}";
            string user = Csla.ApplicationContext.User.Identity.Name;
            string session = CambridgeSoft.COE.Framework.Common.COEUser.SessionID.ToString();

            //build the caching key
            string cachingKey = string.Format(cacheKeyTemplate, user, session, friendlyName);

            //fetch from cache - will return null if keyed item does not exist
            object existingPersistentAddIn = AppDomain.CurrentDomain.GetData(cachingKey);
            if (existingPersistentAddIn == null)
            {
                //create, then cache the object
                string msg = string.Format("Creating & Initializing {0} add-in", friendlyName);
                addIn = InstantiateAddIn(assemblyName, className);
                if (configurationNode != null)
                    addIn.Initialize(configurationNode.OuterXml);

                int lifetimeInSeconds = 90;
                TimeSpan lifetime = new TimeSpan(0, 0, lifetimeInSeconds);
                _persistentAddIn = new PersistentAddInWrapper(addIn, cachingKey, true, lifetime);
                AppDomain.CurrentDomain.SetData(cachingKey, _persistentAddIn);
            }
            else
            {
                _persistentAddIn = (PersistentAddInWrapper)existingPersistentAddIn;
            }

            return _persistentAddIn.AddIn;
        }//JED: new

        /// <summary>
        /// Returns an instance of an IAddIn object. For a given combination of
        /// user, session, assembly and class, the instance will be cached for later
        /// use to reduce instantiation costs.
        /// </summary>
        /// <param name="assemblyName">The anme of the assembly to create an instance of</param>
        /// <param name="className">The calss to instantiate through refelection</param>
        /// <returns>An instance of an IAddIn object</returns>
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

        /// <summary>
        /// Provides add-in event delegation.
        /// </summary>
        /// <param name="businessObject">Business object instance whose event is being monitored</param>
        /// <param name="addIn">Deletgate-container instance.</param>
        /// <param name="eventName">Name of the business object event to attach to.</param>
        /// <param name="eventHandler">The actual delegate.</param>
        private void SubscribeAddInToEvent(object businessObject, IAddIn addIn, string eventName, string eventHandler)
        {
            System.Reflection.EventInfo eventDelegate = businessObject.GetType().GetEvent(eventName);
            if (eventDelegate != null)
            {
                Type eventHandlerType = eventDelegate.EventHandlerType;

                System.Reflection.MethodInfo handlerMethod = addIn.GetType().GetMethod(eventHandler);
                if (handlerMethod == null)
                {
                    MissingMemberException mmex = new MissingMemberException(addIn.GetType().Name, eventName);
                    throw new Exception(
                        string.Format("Couldn't find AddIn event handler '{0}'.", eventHandler)
                        , mmex
                    );
                }
                else if (handlerMethod.IsPrivate == true)
                {
                    MethodAccessException maex = 
                        new MethodAccessException(string.Format("AddIn event handler '{0}' HAS to be public", eventHandler));
                }
                else
                {
                    Delegate d = Delegate.CreateDelegate(eventHandlerType, addIn, handlerMethod);

                    AddInExecutionHandler traceHandler = null;
                    //Is there a way to subscribe event handlers (for trace) to this event no matter what kind it is?
                    if (eventHandlerType.IsAssignableFrom(typeof(EventHandler)))
                        traceHandler = new AddInExecutionHandler(addIn.GetType().Name, eventHandler, businessObject.GetType().Name, eventName);
                    if(traceHandler!=null)
                    {
                        Delegate before = Delegate.CreateDelegate(eventHandlerType, traceHandler , "BeforeAddInExecution");
                        eventDelegate.AddEventHandler(businessObject, before);
                    }

                    eventDelegate.AddEventHandler(businessObject, d);
                   
                    if(traceHandler!=null)
                    {
                        Delegate after = Delegate.CreateDelegate(eventHandlerType, traceHandler, "AfterAddInExecution");
                        eventDelegate.AddEventHandler(businessObject, after);
                    }
                }
            }
            else
                throw new Exception(string.Format("Invalid Event '{0}' for object of type {1}", eventName, businessObject.GetType().ToString()));
        }
    }

    /// <summary>
    /// Provides event handlers to be called before or after an AddIn handler's execution
    /// </summary>
    [Serializable]
    public class AddInExecutionHandler
    {
        private DateTime _startTime;
        private string _addInType;
        private string _eventHandler;
        private string _businessObjectType;
        private string _eventName;

        /// <summary>
        /// constructor, stores information of an AddIn
        /// </summary>
        /// <param name="addInType"></param>
        /// <param name="eventHandler"></param>
        /// <param name="businessObjectType"></param>
        /// <param name="eventName"></param>
        public AddInExecutionHandler(string addInType, string eventHandler, string businessObjectType, string eventName)
        {
            _addInType = addInType;
            _eventHandler = eventHandler;
            _businessObjectType = businessObjectType;
            _eventName = eventName;
        }

        /// <summary>
        /// Trace handler before AddIn execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BeforeAddInExecution(object sender,EventArgs e)
        {
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Trace handler after AddIn execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AfterAddInExecution(object sender,EventArgs e)
        {
            string message = string.Format("{0} seconds for running AddIn {1}.{2} on {3} event of {4}", DateTime.Now.Subtract(_startTime).TotalSeconds,
                              _addInType, _eventHandler, _eventName, _businessObjectType);
            Trace.WriteLine(message);
        }
    }

}
