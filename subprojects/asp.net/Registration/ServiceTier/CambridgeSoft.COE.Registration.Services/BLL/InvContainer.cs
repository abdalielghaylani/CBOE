using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Represents a 'Invetory' Container associated with a batch.
    /// </summary>
    [Serializable()]
    public class InvContainer : BusinessBase<InvContainer>
    {
        private string _qtyAvailable;
        /// <summary>
        /// Gets the quantity available of the InvContainer.
        /// </summary>
        /// <value>The qty available for this single container.</value>
        public string QtyAvailable
        { get { return _qtyAvailable; } }

        private string _containerSize;
        /// <summary>
        /// Gets the Container size of the InvContainer.
        /// </summary>
        /// <value>The container size.</value>
        public string ContainerSize
        { get { return _containerSize; } }

        private string _location = string.Empty;
        /// <summary>
        /// Gets the location of the InvContainer.
        /// </summary>
        /// <value>The location html.</value>
        public string Location
        { get { return _location; } }

        private string _requestURL = string.Empty;
        /// <summary>
        /// Gets the URL for requesting material from this container.
        /// </summary>
        /// <value>The url to request material from this container.</value>
        public string RequestURL
        { get { return _requestURL; } }

        /// <summary>
        /// URL to request from batch
        /// </summary>
        public string RequestFromBatchURL
        { get { return string.Format("{0}&RequestType=B", _requestURL); } }

        /// <summary>
        /// URL to request from container
        /// </summary>
        public string RequestFromContainerURL
        { get { return string.Format("{0}&RequestType=C", _requestURL); } }

        private string _containerType = string.Empty;
        /// <summary>
        /// Gets the Container type was submitted.
        /// </summary>
        /// <value>The container type.</value>
        public string ContainerType
        { get { return _containerType; } }

        private int _containerID = 0;
        /// <summary>
        /// Gets the ContainerID.
        /// </summary>
        /// <value>The containerID.</value>
        public int ContainerID
        { get { return _containerID; } }

        private string _regBatchID = string.Empty;
        /// <summary>
        /// Gets the RegBatchID.
        /// </summary>
        /// <value>The Full Reg Number.</value>
        public string RegBatchID
        { get { return _regBatchID; } }

        private string _regNumber = string.Empty;
        /// <summary>
        /// Gets the reg number associated with the InvContainer.
        /// </summary>
        /// <value>The reg number.</value>
        public string RegNumber
        { get { return _regNumber; } }

        private int _invBatchID = 0;
        /// <summary>
        /// Gets the Inventory BatchID.
        /// </summary>
        /// <value>The Inventory BatchID.</value>
        public int InvBatchID
        { get { return _invBatchID; } }

        private int _invCompoundID = 0;
        /// <summary>
        /// Gets the Inventory CompoundID.
        /// </summary>
        /// <value>The Inventory CompoundID.</value>
        public int InvCompoundID
        { get { return _invBatchID; } }

        private string _totalQtyAvailable = string.Empty;
        /// <summary>
        /// Gets the total quatity available str. It sums convertible quantityes and concat non convertible quantities.
        /// </summary>
        /// <value>The total qty available for the current search</value>
        public string TotalQtyAvailable
        { get { return _totalQtyAvailable; } }

        private string _barcode = string.Empty;
        /// <summary>
        /// Gets the container's barcode. IE: C1000
        /// </summary>
        /// <value>The Container's barcode</value>
        public string Barcode
        { get { return _barcode; } }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvContainer"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        private InvContainer(DataRow row)
        {
            if (row != null)
            {
                if (this.ColumnExists(row, "QTYAVAILABLE"))
                    _qtyAvailable = row["QTYAVAILABLE"].ToString();
                if (this.ColumnExists(row, "CONTAINERID"))
                    _containerID = Convert.ToInt32(row["CONTAINERID"]);
                if (this.ColumnExists(row, "CONTAINERSIZE"))
                    _containerSize = row["CONTAINERSIZE"].ToString();
                if (this.ColumnExists(row, "LOCATION"))
                    _location = row["LOCATION"].ToString();
                if (this.ColumnExists(row, "CONTAINERTYPE"))
                    _containerType = row["CONTAINERTYPE"].ToString();
                if (this.ColumnExists(row, "REGBATCHID"))
                    _regBatchID = row["REGBATCHID"].ToString();
                if (this.ColumnExists(row, "INVBATCHID"))
                    _invBatchID = Convert.ToInt32(row["INVBATCHID"]);
                if (this.ColumnExists(row, "COMPOUNDID"))
                    _invCompoundID = Convert.ToInt32(row["COMPOUNDID"]);
                if (this.ColumnExists(row, "REGNUMBER"))
                    _regNumber = row["REGNUMBER"].ToString();
                if (this.ColumnExists(row, "REQUEST_URL"))
                    _requestURL = row["REQUEST_URL"].ToString();
                if (this.ColumnExists(row, "TotalQtyAvailable"))
                    _totalQtyAvailable = row["TotalQtyAvailable"].ToString();
                if (this.ColumnExists(row, "BARCODE"))
                    _barcode = row["BARCODE"].ToString();
            }
        }

        /// <summary>
        ///Checks if the column exists
        /// </summary>
        /// <param name="row">The row to check</param>
        /// <param name="colName">Name of the column to check</param>
        /// <returns>Boolean indicating if the column was found or not</returns>
        private bool ColumnExists(DataRow row, string colName)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(colName) && row != null)
                if (row.Table.Columns.Contains(colName))
                    if (row[colName] != null)
                        retVal = true;
            return retVal;
        }

        /// <summary>
        /// Creates a new InvContainer object
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("CreateInvContainer")]
        public static InvContainer NewInvContainer(DataRow row)
        {
            try
            {
                return new InvContainer(row);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Gets the id value.
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return _containerID;
        }
    }
}
