using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration;
using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    [Serializable]
    public class Event : BusinessBase<Event>
    {

        #region Variables

        private int _id;
        private string _eventName;
        private string _eventHandler;

        #endregion

        #region Properties

        public int Id
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    CanWriteProperty(true);
                    _id = value;
                    PropertyHasChanged();
                }
            }
        }

        public string EventName
        {
            get
            {
                CanReadProperty(true);
                return _eventName;
            }
            set
            {
                if (value != _eventName)
                {
                    CanWriteProperty(true);
                    _eventName = value;
                    PropertyHasChanged();
                }
            }
        }

        public string EventHandler
        {
            get
            {
                CanReadProperty(true);
                return _eventHandler;
            }
            set
            {
                if (value != _eventHandler)
                {
                    CanWriteProperty(true);
                    _eventHandler = value;
                    PropertyHasChanged();
                }
            }
        }

        #endregion

        #region Constructors

        private Event(bool IsNew)
        {
            _eventName = string.Empty;
            _eventHandler = string.Empty;
            if (IsNew)
                this.MarkDirty();
            else
            {
                this.MarkClean();
                this.MarkOld();
            }
        }

        private Event(string eventName, string eventHandler, bool IsNew)
        {
            _eventName = eventName;
            _eventHandler = eventHandler;
            if (IsNew)
                this.MarkDirty();
            else
            {
                this.MarkClean();
                this.MarkOld();
            }
        }

        #endregion

        #region Factory Methods


        public static Event NewEvent(bool IsNew)
        {
            return new Event(IsNew);
        }

        public static Event NewEvent(string eventName, string eventHandler, bool IsNew)
        {
            return new Event(eventName, eventHandler, IsNew);
        }

        #endregion

        #region Business Method

        protected override object GetIdValue()
        {
            return _id;
        }

        public void MarkAsOld() 
        {
            this.MarkOld();
        }        

        #endregion
    }
}
