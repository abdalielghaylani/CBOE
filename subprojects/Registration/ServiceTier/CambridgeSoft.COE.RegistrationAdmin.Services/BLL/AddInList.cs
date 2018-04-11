using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using Csla;
using Csla.Validation;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.ExceptionHandling;


namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    [Serializable()]
    public class AddInList : BusinessListBase<AddInList, AddIn>
    {

        #region Constructors

        private AddInList(XmlNode addInList)
        {
            foreach (XmlNode addIn in addInList.SelectNodes("./AddIn"))
            {
                this.Add(AddIn.NewAddIn(addIn, false));
            }

        }

        private AddInList() { }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("CreateAddInList")]
        public static AddInList NewAddInList(XmlNode addInList)
        {
            try
            {
                return new AddInList(addInList);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("CreateAddInList")]
        public static AddInList NewAddInList()
        {
            try
            {
                return new AddInList();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        #endregion

        #region Business Method

        /// <summary>
        /// Creates a new configuration addin; for administrative use only.
        /// </summary>
        /// <remarks>
        /// This should be moved to an administrative version of the object specifically
        /// for use by the Administrative tools.
        /// </remarks>
        /// <param name="addin"></param>
        [COEUserActionDescription("AddAddin")]
        public void AddAddin(AddIn addin)
        {
            try
            {

                if (addin.Assembly == string.Empty || addin.ClassName == string.Empty)
                {
                    throw new ValidationException("Addin assembly or Addin classname cannot be empty");
                }
                else
                {
                    if (!this.ExistAddIndCheck(addin))
                    {
                       // For any enhancements , Add rules for events sort order in Addin
                       // Provide index for updatehandler, onsavehandler so that events are updated.   
                        addin.MarkAsNew();
                        this.Add(addin);
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        protected override void RemoveItem(int index)
        {

            this[index].Delete();
            if (!this[index].IsNew)
                DeletedList.Add(this[index]);
            this.Items.RemoveAt(index);
        }

        public void ClearDeletedList()
        {
            DeletedList.Clear();
        }

        [COEUserActionDescription("GetAddInListXml")]
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");
            {
                try
                {
                    foreach (AddIn addIn in this)
                    {
                        builder.Append(addIn.UpdateSelf());
                    }
                    foreach (AddIn delAddIn in DeletedList)
                    {
                        if (!delAddIn.IsNew)
                            builder.Append(delAddIn.UpdateSelf());
                    }
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                }
            }
            return builder.ToString();
        }

        public bool ExistAddIndCheck(AddIn addInToCheck)
        {
            bool exist = false;

            foreach (AddIn addIn in this)
            {
                if (addInToCheck.Assembly == addIn.Assembly && addInToCheck.ClassName == addIn.ClassName ||
                    addInToCheck.Assembly == addIn.Assembly && "CambridgeSoft.COE.Registration.Services.RegistrationAddins." +
                    addInToCheck.ClassName == addIn.ClassName)
                {
                    exist = true;
                    break; // You got the addin break the loop.
                }
            }

            return exist;
        }

        public bool ExistAddInFriendlyName(string freindlyName) 
        {
            foreach (AddIn addIn in this)
            {
                if (addIn.FriendlyName == freindlyName)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
