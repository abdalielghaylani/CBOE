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
    /// List of documents that can be associated with a Registry.
    /// </summary>
    /// <remarks>Mostly used for the interaction between Registration and DocManager</remarks>
    [Serializable()]
    public class DocumentList : BusinessListBase<DocumentList, Document>
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

        #endregion

        #region Factory Methods

        /// <summary>
        ///Creates a new list of documents
        /// </summary>
        /// <param name="table">The table that contains the infomation abour the doc</param>
        /// <param name="regNum">The reg num associated to the doc</param>
        /// <returns>A list of documents associated with the given registry number</returns>
        /// 
        [COEUserActionDescription("CreateDocumentList")]
        public static DocumentList NewDocumentList(DataTable table, string regNum)
        {
            try
            {
                return new DocumentList(table, regNum);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Creates a new list of documents
        /// </summary>
        /// <returns>A list of newly created documents(empty)</returns>
        /// 
        [COEUserActionDescription("CreateDocumentList")]
        public static DocumentList NewDocumentList()
        {
            try
            {
                return new DocumentList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        ///Creates a new list of documents
        /// </summary>
        //// <param name="regNum">The reg num associated to the doc</param>
        /// <returns>A list of documents associated with the given registry number</returns>
        /// 
        [COEUserActionDescription("CreateDocumentList")]
        public static DocumentList NewDocumentList(string regNum)
        {
            try
            {
                return new DocumentList(regNum);
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
        /// Initializes a new instance of the <see cref="DocumentList"/> class.
        /// </summary>
        /// <param name="table">The table that contains the infomation abour the doc</param>
        /// <param name="regNum">The registry number of the list of documents</param>
        private DocumentList(DataTable table, string regNum)
        {
            _regNum = regNum;
            foreach (DataRow row in table.Rows)
                this.Add(Document.NewDocument(row));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentList"/> class.
        /// </summary>
        private DocumentList()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentList"/> class.
        /// </summary>
        /// <param name="regNum">The registry number of the list of documents</param>
        private DocumentList(string regNum)
        {
            _regNum = regNum;
        }

        #endregion
    }
}
