using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using System.Data.Common;
using System.Data;
using Csla.Data;
using Oracle.DataAccess.Client;
using CambridgeSoft.COE.DocumentManager.Services.Types;
using System.IO;

namespace CambridgeSoft.COE.DocumentManager.Services.COEDocumentManagerService
{
    class OracleDataAccessClientDAL : DAL
    {

        #region DocumentList Object

        /// <summary>
        /// Get a xml from DB by given application name and type.
        /// </summary>
        /// <param name="appName">application name</param>
        /// <param name="type">Custom/Master/Privileges</param>
        /// <returns></returns>
        internal override string GetDocumentList()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Document Object

        internal override string GetDocumentByID(Int32 ID)
        {
            //string xml = string.Empty;
            string sql = string.Format(@" SELECT * FROM DOCMGR_DOCUMENTS WHERE DOCID = ({0})", ID);
            string sqle = string.Format(@" SELECT * FROM DOCMGR_EXTERNAL_LINKS WHERE DOCID = ({0})", ID);
            string sqls = string.Format(@" SELECT * FROM DOCMGR_STRUCTURES WHERE DOCID = ({0})", ID);

            StringBuilder builder = new StringBuilder();
            DataTable dt = new DataTable("Documents");
            DataTable dte = new DataTable("ExternalLinks");
            DataTable dts = new DataTable("Structures");

            DataSet ds = new DataSet();

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DbCommand dbCommandExt = DALManager.Database.GetSqlStringCommand(sqle);
            DbCommand dbCommandStr = DALManager.Database.GetSqlStringCommand(sqls);

            //instead use the empty object (maybe there is a better way to get the empty object from here)
            Document empD = Document.GetNewDocument();

            try
            {
                //if it is decided to return a full xml them this is ok, but it 
                //would mean that the db will have to build the xml

                //so for now let's build get a dataset and then build an sml
                SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
                dt.Load(safeReader);

                SafeDataReader safeReaderExt = new SafeDataReader(DALManager.ExecuteReader(dbCommandExt));
                dte.Load(safeReaderExt);

                SafeDataReader safeReaderStr = new SafeDataReader(DALManager.ExecuteReader(dbCommandStr));
                dts.Load(safeReaderStr);

                ds.Tables.Add(dt);
                ds.Tables.Add(dte);
                ds.Tables.Add(dts);

                //ds = DALManager.ExecuteDataSet(dbCommand);
                ds.Relations.Add("rExternalLinks", ds.Tables["Documents"].Columns["DOCID"], ds.Tables["ExternalLinks"].Columns["DOCID"]);
                ds.Relations.Add("rStructures", ds.Tables["Documents"].Columns["DOCID"], ds.Tables["Structures"].Columns["DOCID"]);


                //change this to an xsl transform but here i brute force

                if (ds.Tables[0].Rows.Count > -1)
                {
                    builder.Append("<Document>");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        builder.Append("<ID>" + ds.Tables[0].Rows[0]["DOCID"] + "</ID>");

                        //content will have to be base64 encoded for the xml
                        builder.Append("<Content>" + Convert.ToBase64String((byte[])ds.Tables[0].Rows[0]["DOC"]) + "</Content>");

                        builder.Append("<Name>" + ds.Tables[0].Rows[0]["DOCNAME"] + "</Name>");
                        builder.Append("<Type>" + ds.Tables[0].Rows[0]["DOCTYPE"] + "</Type>");
                        builder.Append("<Size>" + ds.Tables[0].Rows[0]["DOCSIZE"] + "</Size>");
                        builder.Append("<Location>" + ds.Tables[0].Rows[0]["DOCLOCATION"] + "</Location>");
                        builder.Append("<Title>" + ds.Tables[0].Rows[0]["TITLE"] + "</Title>");
                        builder.Append("<Author>" + ds.Tables[0].Rows[0]["AUTHOR"] + "</Author>");
                        builder.Append("<Submitter>" + ds.Tables[0].Rows[0]["SUBMITTER"] + "</Submitter>");
                        builder.Append("<Comments>" + ds.Tables[0].Rows[0]["SUBMITTER_COMMENTS"] + "</Comments>");
                        builder.Append("<DateSubmitted>" + ds.Tables[0].Rows[0]["DATE_SUBMITTED"] + "</DateSubmitted>");

                        //anything that is not in the above list (use table names) but is in the base table goes into the property list
                        builder.Append("<Properties>");

                        //loop through empty object property list
                        foreach (Property p in empD.PropertyList)
                        {
                            //maybe this can be done better by setting actual properties then outputing to xml
                            builder.Append("<Property name=\"" + p.Name + "\" type=\"" + p.Type + "\">");
                            builder.Append(ds.Tables[0].Rows[0][p.Name]);
                            builder.Append("</Property>");
                        }

                        builder.Append("</Properties>");
                    }

                    builder.Append("<ExternalLinks>");


                    foreach (DataRow dar in ds.Tables[1].Rows)
                    {
                        builder.Append("<ExternalLink>");
                        builder.Append("<ID>" + dar["RID"] + "</ID>");
                        builder.Append("<DocumentID>" + dar["DOCID"] + "</DocumentID>");
                        builder.Append("<ExternalApplication>" + dar["APPNAME"] + "</ExternalApplication>");
                        builder.Append("<ExternalLinkType>" + dar["LINKTYPE"] + "</ExternalLinkType>");
                        builder.Append("<ExternalID>" + dar["LINKID"] + "</ExternalID>");
                        builder.Append("<ExternalFieldName>" + dar["LINKFIELDNAME"] + "</ExternalFieldName>");
                        builder.Append("<LinkSubmitter>" + dar["SUBMITTER"] + "</LinkSubmitter>");
                        builder.Append("<LinkSubmittedDate>" + dar["DATE_SUBMITTED"] + "</LinkSubmittedDate>");
                        builder.Append("</ExternalLink>");
                    }
                    builder.Append("</ExternalLinks>");

                    builder.Append("<Structures>");
                    foreach (DataRow dar in ds.Tables[2].Rows)
                    {
                        builder.Append("<Structure>");
                        builder.Append("<ID>" + dar["U_ID"] + "</ID>");
                        builder.Append("<MOLID>" + dar["MOL_ID"] + "</MOLID>");
                        builder.Append("<DocumentID>" + dar["DOCID"] + "</DocumentID>");
                        builder.Append("<BASE64_CDX>" + dar["BASE64_CDX"] + "</BASE64_CDX>");
                        builder.Append("</Structure>");
                    }
                    builder.Append("</Structures>");

                    builder.Append("</Document>");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return builder.ToString();

        }

        internal override Int32 AddNewDocument(DocumentManager.Services.Types.Document doc)
        {
            Int32 newid = 0;

            //I think it is best to do the base object properties as an insert
            //then update the custom properties
            //these may belong in a transaction block
            BeginTransaction();
            string sql = string.Format(@"Insert Into DOCMGR_DOCUMENTS (DOCID, DOC, DOCLOCATION, DOCNAME, DOCSIZE, DOCTYPE, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS,DATE_SUBMITTED) 
						Values(SEQ_DOCMGR_DOCUMENTS.NEXTVAL, :pDoc, :pLocation, :pName, :pSize, :pType, :pTitle, :pAuthor, :pSubmitter, :pComments, :pDateSubmitted) RETURNING DOCID INTO :pNewId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            //this first parameter should actually be the Document binary
            dbCommand.Parameters.Add(new OracleParameter("pDoc", OracleDbType.Blob, doc.BinaryContent, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pLocation", OracleDbType.Varchar2, doc.Location.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pName", OracleDbType.Varchar2, doc.Name.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pSize", OracleDbType.Int32, doc.Size, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pType", OracleDbType.Varchar2, doc.Type.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pTitle", OracleDbType.Varchar2, doc.Title.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pAuthor", OracleDbType.Varchar2, doc.Author.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pSubmitter", OracleDbType.Varchar2, doc.Submitter.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, doc.Comments.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pDateSubmitted", OracleDbType.Date, DateTime.Now, ParameterDirection.Input));

            DALManager.Database.AddOutParameter(dbCommand, "pNewID", DbType.Int32, newid);

            DALManager.ExecuteNonQuery(dbCommand);

            newid = (Int32)dbCommand.Parameters[":pNewID"].Value;

            dbCommand.Parameters.Clear();
            sql = String.Empty;
            StringBuilder sbuilder = new StringBuilder();
            string spname = String.Empty;

            foreach (DocumentManager.Services.Types.Property prop in doc.PropertyList)
            {
                if ((doc.PropertyList[prop.Name].Value != null) && (doc.PropertyList[prop.Name].Value != string.Empty))
                {

                    sbuilder.Append(prop.Name + " = :p" + prop.Name + ",");

                    //create parameters for above
                    if (prop.Type == "DateTime")
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Date, DateTime.Parse(prop.Value), ParameterDirection.Input));
                    }
                    else if (prop.Type == "Int32")
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Int32, Int32.Parse(prop.Value), ParameterDirection.Input));
                    }
                    else
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Varchar2, prop.Value, ParameterDirection.Input));
                    }
                }
            }
            ////remove extra comma
            if (sbuilder.ToString() != String.Empty)
            {
                sql = string.Format(@"Update DOCMGR.DOCMGR_DOCUMENTS Set ");
                sql += sbuilder.ToString().Substring(0, sbuilder.ToString().Length - 1);
                sql += " WHERE DOCID = :pDocID";

                dbCommand.Parameters.Add(new OracleParameter("pDocID", OracleDbType.Int32, newid, ParameterDirection.Input));
                dbCommand.CommandText = sql;
                DALManager.ExecuteNonQuery(dbCommand);
            }

            //try inserting structures
            if (doc.StructureList.Count > 0)
            {
                InsertStructureList(newid, doc.StructureList);
            }

            CommitTransaction();
            return newid;
        }

        internal void InsertStructureList(int docID, StructureList structureList)
        {

            //I think this can be cleaned up but use the classic way for now to ensure compatability
            //classic has a blob (not a clob) field which stinks
            //this creates a new record with no structure and returns the unique id
            DbCommand dbCommand = DALManager.Database.GetStoredProcCommand("INSERT_STRUCTURE");

            //these paramters are always the same what is returned will be different inside the loop
            DALManager.Database.AddOutParameter(dbCommand, "u_id", DbType.Int32, 100);
            dbCommand.Parameters.Add(new OracleParameter("docID", OracleDbType.Int32, docID, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("molid", OracleDbType.Int32, null, ParameterDirection.Input));
            int strucID = 0;


            string sqlCreateTemp = "create global temporary table tempstruc (id number(10),struc clob) on commit preserve rows";
            DbCommand dbCommandI = DALManager.Database.GetSqlStringCommand(sqlCreateTemp);
            try
            {
                DALManager.ExecuteNonQuery(dbCommandI);
            }
            catch
            {

            }
            string sqlInsertLob = "Insert Into tempstruc(id,struc) Values(:sid,:lob)";

            //string sqlAddLob = "Update DOCMGR_STRUCTURES SET BASE64_CDX=c2b(:lob) where u_id=:sid";
            string sqlAddLob = "Update DOCMGR_STRUCTURES SET BASE64_CDX=c2b((Select struc from tempstruc where id=:sid)) where u_id=:sid2";

            foreach (Structure s in structureList)
            {
                DALManager.ExecuteNonQuery(dbCommand);
                strucID = (Int32)dbCommand.Parameters[":u_id"].Value;

                DbCommand dbCommand2 = DALManager.Database.GetSqlStringCommand(sqlInsertLob);
                dbCommand2.Parameters.Add(new OracleParameter("sid", OracleDbType.Int32, strucID, ParameterDirection.Input));
                dbCommand2.Parameters.Add(new OracleParameter("lob", OracleDbType.Varchar2, s.Value, ParameterDirection.Input));
                DALManager.ExecuteNonQuery(dbCommand2);

                DbCommand dbCommand3 = DALManager.Database.GetSqlStringCommand(sqlAddLob);
                dbCommand3.Parameters.Add(new OracleParameter("sid", OracleDbType.Int32, strucID, ParameterDirection.Input));
                dbCommand3.Parameters.Add(new OracleParameter("sid2", OracleDbType.Int32, strucID, ParameterDirection.Input));
                try
                {
                    DALManager.ExecuteNonQuery(dbCommand3);
                }
                catch (Exception ex)
                {

                }
            }

        }

        internal override void DeleteDocumentByID(int docId)
        {
            BeginTransaction();
            string sql = string.Format(@"Delete from DOCMGR_DOCUMENTS where DocId=" + docId);
            string sqls = string.Format(@"Delete from DOCMGR_STRUCTURES where DocId=" + docId);
            string sqle = string.Format(@"Delete from DOCMGR_EXTERNAL_LINKS where DocId=" + docId);

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sqle);
                DALManager.ExecuteNonQuery(dbCommand);

                dbCommand = DALManager.Database.GetSqlStringCommand(sqls);
                DALManager.ExecuteNonQuery(dbCommand);

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);

                DALManager.CommitTransaction();
            }
            catch (Exception ex)
            {
                DALManager.RollbackTransaction();
                string message = "Deleting failed.";
                if (ex.Message.Contains("ORA-01031"))
                {
                    message = "Deleting failed - insufficient privileges.";
                }
                if (ex.Message.Contains("ORA-02292"))
                {
                    message = "The entry is being used and therefore should not be deleted.";
                }
                throw new Exception(message);
            }
        }

        internal override void DeleteDocument(DocumentManager.Services.Types.Document doc)
        {
            DeleteDocumentByID(doc.ID);
        }

        internal override string GetEmptyDocObject()
        {
            string xml = String.Empty;
            string sql = string.Format(@"Select XML from DOCMGR.COEOBJECTCONFIG where ID=1");
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            try
            {
                xml = Convert.ToString(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception e)
            {

            }
            return xml;
        }

        internal override Int32 UpdateDocument(DocumentManager.Services.Types.Document doc)
        {
            Int32 id = doc.ID;

            //I think it is best to do the base object properties as an insert
            //then update the custom properties
            //these may belong in a transaction block
            BeginTransaction();
            string sql = string.Format(@"Update DOCMGR.DOCMGR_DOCUMENTS set DOC=:pDoc, DOCLOCATION=:pLocation, DOCNAME=:pName, DOCTYPE=:pType, TITLE=:pTitle, AUTHOR=:pAuthor, SUBMITTER=:pSubmitter, SUBMITTER_COMMENTS=:pComments, DATE_SUBMITTED=:pDateSubmitted where DOCID=:pDocId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            //this first parameter should actually be the Document binary
            dbCommand.Parameters.Add(new OracleParameter("pDoc", OracleDbType.Blob, doc.BinaryContent, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pLocation", OracleDbType.Varchar2, doc.Location.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pName", OracleDbType.Varchar2, doc.Name.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pType", OracleDbType.Varchar2, doc.Type.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pTitle", OracleDbType.Varchar2, doc.Title.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pAuthor", OracleDbType.Varchar2, doc.Author.ToString(), ParameterDirection.Input));            
            dbCommand.Parameters.Add(new OracleParameter("pSubmitter", OracleDbType.Varchar2, doc.Submitter.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pComments", OracleDbType.Varchar2, doc.Comments.ToString(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pDateSubmitted", OracleDbType.Date, DateTime.Now, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter("pDocId", OracleDbType.Int32, id, ParameterDirection.Input));

            DALManager.ExecuteNonQuery(dbCommand);

            dbCommand.Parameters.Clear();
            sql = String.Empty;
            StringBuilder sbuilder = new StringBuilder();
            string spname = String.Empty;

            foreach (DocumentManager.Services.Types.Property prop in doc.PropertyList)
            {
                if ((doc.PropertyList[prop.Name].Value != null) && (doc.PropertyList[prop.Name].Value != string.Empty))
                {

                    sbuilder.Append(prop.Name + " = :P" + prop.Name + ",");

                    //create parameters for above
                    if (prop.Type == "DateTime")
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Date, DateTime.Parse(prop.Value), ParameterDirection.Input));
                    }
                    else if (prop.Type == "Int32")
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Int32, Int32.Parse(prop.Value), ParameterDirection.Input));
                    }
                    else
                    {
                        dbCommand.Parameters.Add(new OracleParameter("p" + prop.Name, OracleDbType.Varchar2, prop.Value, ParameterDirection.Input));
                    }
                }
            }
            ////remove extra comma
            if (sbuilder.ToString() != String.Empty)
            {
                sql = string.Format(@"Update DOCMGR.DOCMGR_DOCUMENTS Set ");
                sql += sbuilder.ToString().Substring(0, sbuilder.ToString().Length - 1);
                sql += " WHERE DOCID = :pDocID";

                dbCommand.Parameters.Add(new OracleParameter("pDocID", OracleDbType.Int32, id, ParameterDirection.Input));
                dbCommand.CommandText = sql;
                DALManager.ExecuteNonQuery(dbCommand);
            }

            CommitTransaction();
            return id;
        }

        #endregion

        #region Transaction Object

        internal void BeginTransaction()
        {
            if (DALManager.DbTransaction == null)
            {
                DALManager.DbConnection = DALManager.Database.CreateConnection();
                DALManager.DbConnection.Open();
                DALManager.DbTransaction = DALManager.DbConnection.BeginTransaction();
            }
        }
        internal void CommitTransaction()
        {
            if (DALManager.DbTransaction != null)
            {
                DALManager.DbTransaction.Commit();
                DALManager.DbTransaction = null;
                DALManager.DbConnection.Close();
                DALManager.DbConnection = null;
            }
        }
        internal void RollbackTransaction()
        {
            if (DALManager.DbTransaction != null)
            {
                DALManager.DbTransaction.Rollback();
                DALManager.DbTransaction = null;
                DALManager.DbConnection.Close();
                DALManager.DbConnection = null;
            }
        }


        #endregion

    }
}
