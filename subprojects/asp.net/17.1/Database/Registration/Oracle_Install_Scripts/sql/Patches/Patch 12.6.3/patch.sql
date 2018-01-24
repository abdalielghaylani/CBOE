--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

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
CREATE TABLE "REGDB"."USER_REGNUM_COUNT"
( "USER_ID"      VARCHAR2(50 BYTE), "REG_NUM_CNT"  NUMBER ) TABLESPACE T_REGDB_TABL NOLOGGING;

CREATE TABLE "REGDB"."HITSPERPAGE"
  (
    "HITS_ID"        NUMBER(8,0) NOT NULL ENABLE,
    "HITS_USER_CODE" VARCHAR2(50 BYTE),
    "HITS_USER_ID"   VARCHAR2(50 BYTE),
    "HITS"           NUMBER(3,0),
    CONSTRAINT "HITSPERPAGE_PK" PRIMARY KEY ("HITS_ID") 
  );

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--INDEXES
--#########################################################
CREATE UNIQUE INDEX REGDB.USER_REGNUM_COUNT_UK ON REGDB.USER_REGNUM_COUNT(USER_ID) NOLOGGING TABLESPACE T_REGDB_index;

ALTER TABLE REGDB.USER_REGNUM_COUNT ADD (CONSTRAINT USER_REGNUM_COUNT_UK UNIQUE (USER_ID) USING INDEX); 

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################

--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################
set define off
@"sql\Patches\Patch 12.6.3\Function\fn_refresh_User_regnum_count.sql"
set define on

--#########################################################
--GRANTS
--#########################################################
grant execute on regdb.refresh_User_regnum_count to COEDB, COEUSER;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
grant execute on coedb.delete_coefullpage_table to REGDB;

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

Connect &&schemaName/&&schemaPass@&&serverName
--#########################################################
--SYNONYM
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName;
create or replace public synonym refresh_User_regnum_count for regdb.refresh_User_regnum_count;

--#########################################################
--PACKAGES
--#########################################################
Connect &&schemaName/&&schemaPass@&&serverName
set define off
@"sql\Patches\Patch 12.6.3\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 12.6.3\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 12.6.3\Packages\pkg_CompoundRegistry_body.sql"
set define on

--#########################################################
--TRIGGERS
--#########################################################

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









