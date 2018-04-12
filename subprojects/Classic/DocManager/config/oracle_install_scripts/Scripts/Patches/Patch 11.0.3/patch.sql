--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 


--#########################################################
--TABLES
--######################################################### 

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################
set escape \ ;


create or replace view DOCMGR_VW_REG_DOCUMENT_LINKS As
Select del.rid, del.linkid, dd.docid, dd.docname, dd.date_submitted, dd.title,dd.docsize, dd.doctype, dd.author, dd.submitter, CONCAT('/docmanager/default.asp?formgroup=base_form_group\&dbname=docmanager\&formmode=list\&dataaction=query_string\&field_type=integer\&full_field_name=docmgr_documents.docid\&field_value=', del.docid) url, CONCAT('/docmanager/default.asp?formgroup=base_form_group\&dbname=docmanager\&formmode_override=edit\&dataaction=query_string\&field_type=integer\&full_field_name=docmgr_documents.docid\&field_value=', del.docid) urldetail, dd.REPORT_NUMBER, dd.MAIN_AUTHOR,dd.STATUS,dd.WRITER,dd.ABSTRACT,dd.DOCUMENT_DATE
From DOCMGR.DOCMGR_EXTERNAL_LINKS del
	INNER JOIN DOCMGR.DOCMGR_DOCUMENTS dd on dd.docid = del.docid
Where del.linktype = 'CHEMREGREGNUMBER';
select * from  DOCMGR_VW_REG_DOCUMENT_LINKS;
commit;

set escape off;

--#########################################################
--PACKAGES
--#########################################################

set define off

set define on

UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
COMMIT;

prompt **** Patch &&CurrentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&CurrentPatch'
		THEN  'Patches\stop.sql'
		ELSE  '"Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;
 
prompt ****&&setNextPatch ***
@&&setNextPatch 






