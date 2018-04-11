--Copyright 1998-2018 PerkinElmer Informatics, Inc. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--Drop Invalid Indexes
--#########################################################

--#########################################################
--Recreate the Indexes for the schema user
--#########################################################

--#########################################################
--TABLES
--######################################################### 
--CBOE-6299. For improve Performance of Deleting temporary regnumbers
CREATE TABLE  FOR_DELETE_TEMP_REC (ID NUMBER);
 
CREATE GLOBAL TEMPORARY TABLE  TEMP_DELETE_TEMP_REC (ID NUMBER) on commit Delete rows;

--CBOE-6299. End

update &&schemaName..coeobjectconfig
set xml = replace(xml, 'Version=17.1.0.0','Version=17.1.1.0')
where id = 2;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

update &&securitySchemaName..COECONFIGURATION set
classname = replace(classname, 'Version=17.1.0.0','Version=17.1.1.0')
where description = 'Registration';


Connect &&schemaName/&&schemaPass@&&serverName


--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--INDEXES
--#########################################################
create index I_STRUCTURES_DR_TYPE on structures (Drawing_Type); 
--CBOE-6299. For improve Performance of Deleting temporary regnumbers
CREATE INDEX I_FOR_DELETE_TEMP_REC ON FOR_DELETE_TEMP_REC(ID);

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################
--CBOE-6299. For improve Performance of Deleting temporary regnumbers
@"sql\Patches\Patch 17.1.1\PROCEDURES\DEL_PROCEEDED_TEMP_RECORDS.sql"
--CBOE-6299. end
--CBOE-6684, 6685.
@"sql\Patches\Patch 17.1.1\PROCEDURES\update_temporary_compound.sql"
@"sql\Patches\Patch 17.1.1\PROCEDURES\update_compound_molecule.sql"
--CBOE-6684, 6685. end
--CBOE-6940
@"sql\Patches\Patch 17.1.1\FUNCTIONS\CHECK_ID_FOR_DEL.SQL"
--CBOE-6940. end
--#########################################################

--#########################################################
--VIEWS
--#########################################################
--ADD MISSED COLUMNS
begin 
CONFIGURATIONCOMPOUNDREGISTRY.ADDFIELDTOVIEW('STRUCT_COMMENTS', 'STRUCT_COMMENTS', 'VW_MIXTURE_STRUCTURE', 'STRUCTURES');
end;
/

begin 
CONFIGURATIONCOMPOUNDREGISTRY.ADDFIELDTOVIEW('STRUCT_NAME', 'STRUCT_NAME', 'VW_MIXTURE_STRUCTURE', 'STRUCTURES');
end;
/

--CBOE-6299. I Added condition for 4 views For improve Performance of Deleting temporary regnumbers
declare 
 v_sql1 clob;
begin
  for cur0 in (select view_name,text from user_views where view_name in ('VW_TEMPORARYREGNUMBERSPROJECT','VW_TEMPORARYBATCH','VW_TEMPORARYBATCHPROJECT','VW_TEMPORARYCOMPOUND')) loop
    v_sql1 := 'create or replace force view &&schemaName'||'.'|| cur0.view_name ||' (';
    for cur in (select column_name from user_tab_cols t
    where table_name = cur0.view_name
    order by  column_id) loop
        v_sql1:=v_sql1||cur.column_name||',';
    end loop;
     v_sql1:=rtrim(v_sql1,',')||') as '|| cur0.text ||
     case cur0.view_name
       when 'VW_TEMPORARYREGNUMBERSPROJECT' then chr(10)||' TB where CHECK_ID_FOR_DEL(tb.TEMPBATCHID)=0'
       when 'VW_TEMPORARYBATCH' then  chr(10)||' where CHECK_ID_FOR_DEL(tb.TEMPBATCHID)=0'
       when 'VW_TEMPORARYBATCHPROJECT' then  chr(10)||' TB where CHECK_ID_FOR_DEL(tb.TEMPBATCHID)=0'
       when 'VW_TEMPORARYCOMPOUND' then chr(10)||' TB where CHECK_ID_FOR_DEL(tb.TEMPBATCHID)=0'
     end;
     execute immediate v_sql1;
  end loop;
end;
/
--CBOE-6299 end
Connect &&schemaName/&&schemaPass@&&serverName
--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################
--GRANTS
--#########################################################

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

GRANT DELETE ON coesavedhitlist to &&schemaName;

GRANT DELETE ON coetemphitlist to &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--SYNONYM
--#########################################################

--#########################################################
--PACKAGES
--#########################################################
set define off
@"sql\Patches\Patch 17.1.1\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 17.1.1\Packages\pkg_RegistryDuplicateCheck_body.sql"
set define on
--#########################################################
--TRIGGERS
--#########################################################

@"sql\Patches\Patch 17.1.1\Trigger\trg_delete_hitlist_info_temp.sql"

@"sql\Patches\Patch 17.1.1\Trigger\trg_delete_hitlist_info_perm.sql"

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

@"sql\Patches\Patch 17.1.1\Trigger\trg_upd_saved_hitcount_info.sql"

@"sql\Patches\Patch 17.1.1\Trigger\trg_upd_temp_hitcount_info.sql"

Connect &&schemaName/&&schemaPass@&&serverName


--#########################################################
--JOBS
--#########################################################
--CBOE-6299. For improve Performance of Deleting temporary regnumbers
var V_JOBNO number;
EXEC DBMS_JOB.SUBMIT(:V_JOBNO, 'begin DEL_PROCEEDED_TEMP_RECORDS; end;', (SYSDATE)+1/24/60, '(SYSDATE)+1/24/12');

EXEC DBMS_JOB.RUN(:V_JOBNO);
--CBOE-6299. end

--CBOE-6684,6685
-- fill formulaweight, molecularformula  into vw_temporarycompound VIEW
--EXEC DBMS_JOB.SUBMIT(:V_JOBNO, 'begin update_temporary_compound; end;', (SYSDATE)+1/24/60, '(SYSDATE)+1/24/12');

--EXEC DBMS_JOB.RUN(:V_JOBNO);

-- fill formulaweight, molecularformula into vw_compound view
--EXEC DBMS_JOB.SUBMIT(:V_JOBNO, 'begin update_compound_molecule; end;', (SYSDATE)+1/24/60, '(SYSDATE)+1/24/6');

--EXEC DBMS_JOB.RUN(:V_JOBNO);
--CBOE-6684,6685. END


--#########################################################
--ORACLE PARAMETERS
--#########################################################

Connect &&InstallUser/&&sysPass@&&serverName;
-- to improve performance of the refresh time of the Materialized Views
alter system set "_mv_refresh_use_stats" = true scope=both;
alter system set "_mv_refresh_use_no_merge" = false scope=both;

Connect &&schemaName/&&schemaPass@&&serverName
--#########################################################
--DATA
--#########################################################

--#####################################################################


UPDATE &&schemaName..Globals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';

UPDATE &&schemaName..Globals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;

@&&setNextPatch 

