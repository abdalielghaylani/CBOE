ALTER TABLE DOCMGR.DOCMGR_DOCUMENTS
ADD (  REPORT_NUMBER varchar2(12),
 		MAIN_AUTHOR varchar2(60),
 		STATUS varchar2(12),
        WRITER varchar2(60),
        DOCUMENT_DATE  DATE,
        DOCUMENT_CLASS varchar2(60),
        SEC_DOC_CAT varchar2(60)
 );

ALTER TABLE DOCMGR.DOCMGR_DOCUMENTS
 MODIFY AUTHOR varchar2(500);

ALTER TABLE DOCMGR.DOCMGR_DOCUMENTS
 MODIFY SUBMITTER_COMMENTS varchar2(2000);



ALTER TABLE DOCMGR.DOCMGR_DOCUMENTS
ADD ABSTRACT CLOB;

Create index ix_dd_rn on DOCMGR.DOCMGR_DOCUMENTS(REPORT_NUMBER);
Create index ix_dd_ma on DOCMGR.DOCMGR_DOCUMENTS(MAIN_AUTHOR);
Create index ix_dd_status on DOCMGR.DOCMGR_DOCUMENTS(STATUS);
Create index ix_dd_writer on DOCMGR.DOCMGR_DOCUMENTS(WRITER);
Create index ix_dd_docdate on DOCMGR.DOCMGR_DOCUMENTS(DOCUMENT_DATE);
Create index ix_dd_docclass on DOCMGR.DOCMGR_DOCUMENTS(DOCUMENT_CLASS);
Create index ix_dd_docsec on DOCMGR.DOCMGR_DOCUMENTS(SEC_DOC_CAT);

set escape \ ;

create or replace view DOCMGR_VW_REG_DOCUMENT_LINKS As
Select del.rid, del.linkid, dd.docname, dd.docsize, dd.doctype, dd.author, dd.submitter, CONCAT('/docmanager/default.asp?formgroup=base_form_group\&dbname=docmanager\&formmode=list\&dataaction=query_string\&field_type=integer\&full_field_name=docmgr_documents.docid\&field_value=', del.docid) url, CONCAT('/docmanager/default.asp?formgroup=base_form_group\&dbname=docmanager\&formmode_override=edit\&dataaction=query_string\&field_type=integer\&full_field_name=docmgr_documents.docid\&field_value=', del.docid) urldetail, dd.REPORT_NUMBER, dd.MAIN_AUTHOR,dd.STATUS,dd.WRITER,dd.ABSTRACT,dd.DOCUMENT_DATE
From DOCMGR.DOCMGR_EXTERNAL_LINKS del
	INNER JOIN DOCMGR.DOCMGR_DOCUMENTS dd on dd.docid = del.docid
Where del.linktype = 'CHEMREGREGNUMBER';

set escape off;



CREATE OR REPLACE PROCEDURE DOCMGR.INSERTDOC(u_id OUT INTEGER, fileLocation IN VARCHAR2, fileName IN VARCHAR2, fileSize IN INTEGER, fileType IN VARCHAR2, title IN VARCHAR2, author IN VARCHAR2, submitter IN VARCHAR2, submitter_comments IN VARCHAR2, projectID IN INTEGER default null, REPORT_NUMBER IN VARCHAR2 default null,MAIN_AUTHOR  IN VARCHAR2 default null,STATUS  IN VARCHAR2 default null,WRITER  IN VARCHAR2 default null, ABSTRACT IN CLOB default null, DOCUMENT_DATE IN DATE default null,DOCUMENT_CLASS IN VARCHAR2 default null,SEC_DOC_CAT IN VARCHAR2 default null)
AS
BEGIN
	SELECT DOCMGR.SEQ_DOCMGR_DOCUMENTS.NEXTVAL INTO u_id FROM DUAL;
	INSERT INTO DOCMGR.DOCMGR_DOCUMENTS (DOCID, DOC, DOCLOCATION, DOCNAME, DOCSIZE, DOCTYPE, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED, REG_RLS_PROJECT_ID, REPORT_NUMBER,MAIN_AUTHOR,STATUS,WRITER,ABSTRACT,DOCUMENT_DATE,DOCUMENT_CLASS,SEC_DOC_CAT)
	VALUES (u_id, EMPTY_BLOB(), fileLocation, fileName, fileSize, fileType, title, author, submitter, submitter_comments, SYSDATE, projectID, REPORT_NUMBER,MAIN_AUTHOR,STATUS,WRITER,ABSTRACT,DOCUMENT_DATE,DOCUMENT_CLASS,SEC_DOC_CAT);
	COMMIT;
END;
/





CREATE OR REPLACE PROCEDURE CDX_BLOB_IN (uid IN INTEGER,
						amount IN INTEGER,
						cdx_buffer IN LONG RAW)
IS
	cdx_blob BLOB; 
	cartridge_failed_to_index EXCEPTION;
	PRAGMA EXCEPTION_INIT(cartridge_failed_to_index, -29877);
BEGIN
	--FETCH THE LOB LOCATOR
	SELECT BASE64_CDX INTO cdx_blob
	FROM DOCMGR.DOCMGR_STRUCTURES
	WHERE U_ID = uid
	FOR UPDATE;

	DBMS_LOB.WRITEAPPEND(cdx_blob, amount, cdx_buffer);     
	EXCEPTION
		when  cartridge_failed_to_index then
			null;
END;
/




@@globals.sql


--xxReport Number: alphanumerical of 12 characters
--xxMain Author: alphanumerical of 60 characters
--?Status: alphanumerical of 12 characters with a picklist functionality
--xx(Document Date)Data: date format with the capability to choose from a date picker
--xxWriter: alphanumerical of 60 characters
--xxAuthors: alphanumerical of 500 characters
--xx(alter)Title: alphanumerical of 1000 characters
--xxAbstract: alphanumerical that can store 32K characters
--xxComments: alphanumerical with 2000 characters
