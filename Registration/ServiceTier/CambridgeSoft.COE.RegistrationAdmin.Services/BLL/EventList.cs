using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    [Serializable]
    public class EventList : BusinessListBase<EventList, Event>
    {
        #region Variables

        private int _id;

        #endregion

        #region Properties
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                    _id = value;
            }
        }
        #endregion

        #region Constructors

        private EventList(XmlNodeList xmlNodeList)
        {
            foreach (XmlNode eventNode in xmlNodeList)
            {
                string eventName = eventNode.Attributes["eventName"].InnerText;
                string eventHandler = eventNode.Attributes["eventHandler"].InnerText;
                this.Add(Event.NewEvent(eventName, eventHandler,false));
            }
        }

        private EventList() 
        {

        }

        #endregion        
                
        #region Factory Methods

        [COEUserActionDescription("CreateEventList")]
        public static EventList NewEventList(XmlNodeList xmlNodeList)
        {
            try
            {
                return new EventList(xmlNodeList);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        public static EventList NewEventList() 
        {
            return new EventList();
        }

        #endregion

        #region Business Method

        protected override void RemoveItem(int index)
        {
            this[index].Delete();
            if (!this[index].IsNew)
                DeletedList.Add(this[index]);           
            this.Items.RemoveAt(index);
        }

        [COEUserActionDescription("GetEventXml")]
        public string UpdateSelf(bool addInIsNew) 
        {
            StringBuilder builder = new StringBuilder();

            try
            {
                builder.Append("");
                foreach (Event evt in this)
                {
                    if (evt.IsNew && !addInIsNew)
                    {
                        builder.Append("<Event eventName=\"" + evt.EventName + "\" eventHandler=\"" + evt.EventHandler + "\" insert=\"yes\"/>");
                    }
                    else
                    {
                        builder.Append("<Event eventName=\"" + evt.EventName + "\" eventHandler=\"" + evt.EventHandler + "\"/>");
                    }
                }
                foreach (Event delEvt in this.DeletedList)
                {
                    if (delEvt.IsDirty && !delEvt.IsNew)
                        builder.Append("<Event eventName=\"" + delEvt.EventName + "\" eventHandler=\"" + delEvt.EventHandler + "\" delete=\"yes\"/>");
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
            
            return builder.ToString();
        }

        [COEUserActionDescription("ExistEventCheck")]
        public bool ExistEventCheck(Event evtToCheck) 
        {
            bool exist = false;

            try
            {
                foreach (Event evt in this)
                {
                    if (evtToCheck.EventName == evt.EventName && evtToCheck.EventHandler == evt.EventHandler)
                    {
                        exist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }

            return exist;
        }

        #endregion
    }
}
