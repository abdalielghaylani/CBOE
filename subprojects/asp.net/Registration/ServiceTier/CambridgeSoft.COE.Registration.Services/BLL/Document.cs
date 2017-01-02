using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Represents a 'doc manager' document associated with a RegistryRecord.
    /// </summary>
    [Serializable()]
    public class Document : BusinessBase<Document>
    {
        private string _name = string.Empty;
        /// <summary>
        /// Gets the name of the document.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        { get { return _name; } }

        private string _title = string.Empty;
        /// <summary>
        /// Gets the title of the document.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        { get { return _title; } }

        private string _author = string.Empty;
        /// <summary>
        /// Gets the author of the document.
        /// </summary>
        /// <value>The author.</value>
        public string Author
        { get { return _author; } }

        private DateTime _dateSubmitted = DateTime.MinValue;
        /// <summary>
        /// Gets the date the document was submitted.
        /// </summary>
        /// <value>The date submitted.</value>
        public DateTime DateSubmitted
        { get { return _dateSubmitted; } }

        private int _docID = 0;
        /// <summary>
        /// Gets the DOCID.
        /// </summary>
        /// <value>The DOCID.</value>
        public int DOCID
        { get { return _docID; } }

        private int _rID = 0;
        /// <summary>
        /// Gets the RID.
        /// </summary>
        /// <value>The RID.</value>
        public int RID
        { get { return _rID; } }

        private string _regNumber = string.Empty;
        /// <summary>
        /// Gets the reg number associated with the document.
        /// </summary>
        /// <value>The reg number.</value>
        public string RegNumber
        { get { return _regNumber; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        private Document(DataRow row)
        {
            if (row != null)
            {
                if (this.ColumnExists(row, "NAME"))
                    _name = row["NAME"].ToString();
                if (this.ColumnExists(row,"DOCID"))
                    _docID = Convert.ToInt32(row["DOCID"]);
                if (this.ColumnExists(row,"TITLE"))
                    _title = row["TITLE"].ToString(); 
                if (this.ColumnExists(row,"AUTHOR"))
                    _author = row["AUTHOR"].ToString(); 
                DateTime tempDate;
                if (this.ColumnExists(row, "DATE_SUBMITTED"))                    
                    if (DateTime.TryParse(row["DATE_SUBMITTED"].ToString(), out tempDate))
                        _dateSubmitted = tempDate; 
                if (this.ColumnExists(row,"RID"))
                    _rID = Convert.ToInt32(row["RID"].ToString());
                if (this.ColumnExists(row,"DOCID_LINKID"))
                    _regNumber = row["DOCID_LINKID"].ToString();
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
            if(!string.IsNullOrEmpty(colName) && row != null)
                if(row.Table.Columns.Contains(colName))
                    if(row[colName] != null)
                        retVal = true;
            return retVal;
        }

        /// <summary>
        /// Creates a new document object
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("CreateDocument")]
        public static Document NewDocument(DataRow row)
        {
            try
            {
                return new Document(row);
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
            return _docID;
        }
    }
}
