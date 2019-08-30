--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 
Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--TABLES
--######################################################### 
create global temporary table for_determ_part_name (partition_name varchar2(30),high_value clob);

-- create partitioned COEFULLPAGE table 
declare
    l_Part number;
begin 
    --check license for partitions 
    SELECT DECODE(Value, 'TRUE', 1, 0) into l_Part
     FROM V$Option WHERE parameter = 'Partitioning';
     
    if l_Part = 1 then
        execute immediate q'[CREATE TABLE COEFULLPAGE
        (
          BASETABLEPRIMARYKEY  NUMBER(9),
          SORTORDER            FLOAT(126),
          CLIENT_ID            VARCHAR2(50 BYTE)        DEFAULT UPPER(SYS_Context('userenv', 'client_identifier'))
        )
        partition by list (CLIENT_ID)
        (partition d0 values ('CSSADMIN')
        )
        TABLESPACE T_COEDB_TABL
        nologging]';
        execute immediate q'[CREATE INDEX COEDB.I_COEFULLPAGE ON COEDB.COEFULLPAGE
        ( SORTORDER)  local  nologging]';
    else
        execute immediate q'[CREATE TABLE COEFULLPAGE
        (
          BASETABLEPRIMARYKEY  NUMBER(9),
          SORTORDER            FLOAT(126),
          CLIENT_ID            VARCHAR2(50 BYTE)        DEFAULT UPPER(SYS_Context('userenv', 'client_identifier'))
        )
        TABLESPACE T_COEDB_TABL
        nologging]';
        execute immediate q'[CREATE INDEX COEDB.I_COEFULLPAGE ON COEDB.COEFULLPAGE
        ( CLIENT_ID, SORTORDER)  nologging]';    
    end if;
end;
/

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

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################
@"sql\Patches\Patch &&currentPatch\Function\fn_get_current_part_name.sql"
@"sql\Patches\Patch &&currentPatch\Procedure\p_delete_coefullpage_table.sql"
@"sql\Patches\Patch &&currentPatch\Function\fn_get_next_part_name.sql"
@"sql\Patches\Patch &&currentPatch\Procedure\p_add_new_part_coefullpage.sql"
@"sql\Patches\Patch &&currentPatch\Function\fn_CreateUser.sql"
@"sql\Patches\Patch &&currentPatch\job_COEFULLPAGE_Defragment.sql"

--#########################################################
--GRANTS
--#########################################################
GRANT SELECT, INSERT, UPDATE ON COEFULLPAGE TO CSS_USER;

-- add partitions for existing users
declare
    l_Part number;
begin 
    --check license for partitions 
    SELECT DECODE(Value, 'TRUE', 1, 0) into l_Part
     FROM V$Option WHERE parameter = 'Partitioning';
     
    if l_Part = 1 then
		for cur in (select upper(user_id) as user_id from people where user_id is not null) loop
		begin
		 add_new_part_coefullpage(cur.user_id);
		end;
		end loop;
    end if;
end;
/


--#########################################################
--PACKAGES
--#########################################################
@"sql\Patches\Patch &&currentPatch\Packages\pkg_Roles.sql"
--#########################################################
--DATA
--#########################################################



--#####################################################################


UPDATE &&schemaName..CoeGlobals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'SCHEMAVERSION';

UPDATE &&schemaName..CoeGlobals
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











