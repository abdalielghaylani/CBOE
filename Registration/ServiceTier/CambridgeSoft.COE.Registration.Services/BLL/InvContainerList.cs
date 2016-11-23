using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using System.Xml.XPath;
using System.Data;
using System.Collections;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// List of InvContainers that can be associated with a Registry.
    /// </summary>
    /// <remarks>Mostly used for the interaction between Registration and DocManager</remarks>
    [Serializable()]
    public class InvContainerList : BusinessListBase<InvContainerList, InvContainer>
    {

        #region Variables, Constants

        private string _regNum = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Registry number.
        /// </summary>
        /// <value>The reg number.</value>
        public string RegNumber
        { get { return _regNum; } }

        /// <summary>
        /// Sum of the quantity avalable of each container.
        /// </summary>
        public string TotalQtyAvailable
        {
            get
            {
                if (this.Items.Count > 0)
                    return this.Items[0].TotalQtyAvailable;

                return "0";
            }
        }

        /// <summary>
        /// Last ContainerID added.
        /// </summary>
        public int LastContainerID
        {
            get
            {
                if (this.Items.Count > 0)
                    return this.Items[this.Items.Count - 1].ContainerID;
                
                return 0;
            }
        }

        /// <summary>
        /// URL for requesting material from the last container.
        /// </summary>
        public string LastContainerRequestURL
        {
            get
            {
                if(this.Items.Count > 0)
                    return this.Items[this.Items.Count - 1].RequestURL;

                return string.Empty;
            }
        }
        #endregion

        #region Factory Methods

        /// <summary>
        ///Creates a new list of InvContainers
        /// </summary>
        /// <param name="table">The table that contains the infomation abour the doc</param>
        /// <param name="regNum">The reg num associated to the doc</param>
        /// <returns>A list of InvContainers associated with the given registry number</returns>
        [COEUserActionDescription("CreateInvContainerList")]
        public static InvContainerList NewInvContainerList(DataTable table, string regNum)
        {
            try
            {
                return new InvContainerList(table, regNum);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Creates a new list of InvContainers
        /// </summary>
        /// <returns>A list of newly created InvContainers(empty)</returns>
        [COEUserActionDescription("CreateInvContainerList")]
        public static InvContainerList NewInvContainerList()
        {
            try
            {
                return new InvContainerList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        ///Creates a new list of InvContainers
        /// </summary>
        //// <param name="regNum">The reg num associated to the doc</param>
        /// <returns>A list of InvContainers associated with the given registry number</returns>
        /// 
        [COEUserActionDescription("CreateInvContainerList")]
        public static InvContainerList NewInvContainerList(string regNum)
        {
            try
            {
                return new InvContainerList(regNum);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvContainerList"/> class.
        /// </summary>
        /// <param name="table">The table that contains the infomation abour the doc</param>
        /// <param name="regNum">The registry number of the list of InvContainers</param>
        private InvContainerList(DataTable table, string regNum)
        {
            _regNum = regNum;
            foreach (DataRow row in table.Rows)
                this.Add(InvContainer.NewInvContainer(row));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvContainerList"/> class.
        /// </summary>
        private InvContainerList()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvContainerList"/> class.
        /// </summary>
        /// <param name="regNum">The registry number of the list of InvContainers</param>
        private InvContainerList(string regNum)
        {
            _regNum = regNum;
        }

        #endregion
    }
}
