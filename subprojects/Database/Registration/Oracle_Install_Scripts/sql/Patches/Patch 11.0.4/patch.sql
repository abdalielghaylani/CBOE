--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#########################################################
--TABLES
--######################################################### 

/* Create Drawing_Type table (with metadata) if it does not exist */
DECLARE
  LTableExists INTEGER := 0;
  LChemicalStructure clob := null;
BEGIN
  select COUNT(*) INTO LTableExists from all_tables t where t.table_name = 'DRAWING_TYPE' and t.owner = upper('&&schemaName');
  if ( LTableExists = 0 ) then
    execute immediate '
    CREATE TABLE &&schemaName..DRAWING_TYPE
    (
      ID NUMBER(1,0),
      DESCRIPTION VARCHAR2(50) unique not null,
      DEFAULT_DRAWING CLOB,
      STRUCTUREFORMAT VARCHAR2(150),
      CONSTRAINT DRAWING_TYPE_PK
      PRIMARY KEY (ID)
    )';
  end if;
END;
/
prompt Created or bypassed "DRAWING_TYPE" table creation

/* Provide the default metadata for this new table */
DECLARE 
	LTableExists INTEGER := 0;
	LChemicalStructure clob := null;
BEGIN
  select COUNT(*) INTO LTableExists from all_tables t where t.table_name = 'DRAWING_TYPE' and t.owner = upper('&&schemaName');
	
  if ( LTableExists <> 0 ) then
    merge into &&schemaName..DRAWING_TYPE T using
    (
      select 0 as ID,'Chemical' as DESCRIPTION,LChemicalStructure as DEFAULT_DRAWING,null as STRUCTUREFORMAT from dual union all
      select 1,'Unknown',base64_cdx,StructureFormat from &&schemaName..Structures where cpd_internal_id=-1 union all
      select 2,'No Structure',base64_cdx,StructureFormat from &&schemaName..Structures where cpd_internal_id=-2 union all
      select 3,'Non Chemical Content',base64_cdx,StructureFormat from &&schemaName..Structures where cpd_internal_id=-3
    ) NT on (T.ID=NT.ID)
    when not matched then
      insert values(NT.ID, NT.Description, NT.DEFAULT_DRAWING, NT.STRUCTUREFORMAT);
  end if;
END;
/
commit;
prompt Inserted or updated metadata for the "DRAWING_TYPE" table

/* Add a new column DRAWING_TYPE to STRUCTURES as a foreign key */
DECLARE
  LColumnExists integer :=0;
BEGIN
  select count(*) into LColumnExists
  from all_tab_cols t
  where t.owner = upper('&&schemaName') and t.table_name = 'STRUCTURES' and t.column_name = 'DRAWING_TYPE';

  if ( LColumnExists = 0 ) then
    execute immediate
      'ALTER TABLE &&schemaName..STRUCTURES ADD ( 
		   DRAWING_TYPE NUMBER(1,0)
		   CONSTRAINT STRUCTURES_DRAWING_TYPE_FK REFERENCES &&schemaName..DRAWING_TYPE (ID) )';
  end if;
END;
/
prompt Created or bypassed generation of "drawing_type" FK column in "STRUCTURES" table


/* Create STRUCTURE_IDENTIFIER table if it does not exist */
DECLARE
  LTableExists INTEGER := 0;
  LChemicalStructure clob := null;
BEGIN
  select COUNT(*) INTO LTableExists from all_tables t where t.table_name = 'STRUCTURE_IDENTIFIER' and t.owner = upper('&&schemaName');
  if ( LTableExists = 0 ) then
    execute immediate '
    CREATE TABLE &&schemaName..STRUCTURE_IDENTIFIER
    (
      ID           NUMBER(8,0),
      TYPE         NUMBER(8,0), 
      STRUCTUREID      NUMBER(8,0),
      VALUE        VARCHAR2(2000 Byte),
      ORDERINDEX   NUMBER(8,0),
      CONSTRAINT STRUCTURE_IDENTIFIER_PK 
        PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName
    )';

    /* Add StructureIdentifiersXml column to Temporary_Compound table */
    execute immediate
      'ALTER TABLE &&schemaName..TEMPORARY_COMPOUND ADD STRUCTIDENTIFIERXML CLOB';
  end if;
END;
/
prompt Created or bypassed "STRUCTURE_IDENTIFIER" table creation and Temporary_Compound.StructureIdentifiersXml column creation

/* Create the 'BATCHNUMBER_LENGTH' column in the SEQUENCES table  */
declare
  v_column_exists number := 0;
  v_column_name varchar2(30) := 'BATCHNUMBER_LENGTH';
  v_table_name varchar2(30) := 'SEQUENCE';
begin

  select count(*) into v_column_exists
  from all_tab_cols  t
  where t.table_name = v_table_name
    and t.column_name = v_column_name
    and t.owner = upper('&&schemaName');

  if (v_column_exists = 0) then
    execute immediate
      'alter table &&schemaName..SEQUENCE add ' || v_column_name || ' number(1)';

    /* Add this column to the corresponding view */
    execute immediate
      'CREATE OR REPLACE VIEW &&schemaName..VW_SEQUENCE (
         sequenceid, regnumberlength, prefix, prefixdelimiter
         , suffix, suffixdelimiter, saltsuffixtype, batchdelimiter
         , nextinsequence, example, active, type
         , siteid, batchnumlength
       ) AS
       select
         sequence_id,regnumber_length,prefix,prefix_delimiter
         ,suffix,suffixdelimiter,saltsuffixtype,batchdelimeter
         ,next_in_sequence,example,active,type
         ,siteid, batchnumber_length
       from &&schemaName..SEQUENCE';
  end if;
end;
/
prompt Created or bypassed generation of "batchnumber_length" column in "SEQUENCE" table and "VW_SEQUENCE" view

/* Add a new column PERCENTAGE to BatchComponent table */
DECLARE
  LColumnExists integer := 0;
BEGIN
  select count(*) into LColumnExists
  from all_tab_cols t
  where t.owner = upper('&&schemaName')
    and t.table_name = 'BATCHCOMPONENT'
    and t.column_name = 'PERCENTAGE';

  if ( LColumnExists = 0 ) then
	  execute immediate
      'ALTER TABLE &&schemaName..BATCHCOMPONENT ADD PERCENTAGE NUMBER(5,2)';
  end if;
END;
/
prompt Created or bypassed generation of "percentage" column in "BatchComponent" table

/* Allow the Sequence.Prefix to be nullable */
ALTER TABLE &&schemaName..SEQUENCE MODIFY PREFIX VARCHAR2(150) NULL;

/* Add default structure properties to both perm and temp tables */
DECLARE
  LColumnExists integer := 0;
BEGIN
  /* 1 - STRUCT_COMMENTS in STRUCTURES table */
  select count(*) into LColumnExists from all_tab_cols t
  where t.owner = upper('&&schemaName') and t.table_name = 'STRUCTURES' and t.column_name = 'STRUCT_COMMENTS';
  if ( LColumnExists = 0 ) then
    execute immediate 'ALTER TABLE &&schemaName..STRUCTURES ADD STRUCT_COMMENTS VARCHAR2(200)';
  end if;

  /* 2 - STRUCT_NAME in STRUCTURES table */
  select count(*) into LColumnExists from all_tab_cols t
  where t.owner = upper('&&schemaName') and t.table_name = 'STRUCTURES' and t.column_name = 'STRUCT_NAME';
  if ( LColumnExists = 0 ) then
    execute immediate 'ALTER TABLE &&schemaName..STRUCTURES ADD STRUCT_NAME varchar2(2000)';
  end if;
  
  /* 3 - STRUCT_COMMENTS in TEMPORARY_COMPOUND table */
  select count(*) into LColumnExists from all_tab_cols t
  where t.owner = upper('&&schemaName') and t.table_name = 'TEMPORARY_COMPOUND' and t.column_name = 'STRUCT_COMMENTS';
  if ( LColumnExists = 0 ) then
    execute immediate 'ALTER TABLE &&schemaName..TEMPORARY_COMPOUND ADD STRUCT_COMMENTS varchar2(200)';
  end if;

  /* 4 - STRUCT_NAME in TEMPORARY_COMPOUND table */
  select count(*) into LColumnExists from all_tab_cols t
  where t.owner = upper('&&schemaName') and t.table_name = 'TEMPORARY_COMPOUND' and t.column_name = 'STRUCT_NAME';
  if ( LColumnExists = 0 ) then
    execute immediate 'ALTER TABLE &&schemaName..TEMPORARY_COMPOUND ADD STRUCT_NAME varchar2(2000)';
  end if;
  
END;
/
prompt Created or bypassed generation of "STRUCT_COMMENTS" and "STRUCT_NAME" columns in "STRCUTURES" and "TEMPORARY_COMPOUND" tables


--#########################################################
--SEQUENCES
--#########################################################

--TODO: make this conditional
CREATE SEQUENCE &&schemaName..SEQ_STRUCTURE_IDENTIFIER INCREMENT BY 1 START WITH 1;

--#########################################################
--TRIGGERS
--#########################################################

/* PK generation for new Structure Identifiers table */
CREATE OR REPLACE TRIGGER &&schemaName..TRG_STRUCTURE_IDENTIFIER
BEFORE INSERT ON &&schemaName..STRUCTURE_IDENTIFIER FOR EACH ROW
BEGIN
  SELECT &&schemaName..SEQ_STRUCTURE_IDENTIFIER.NEXTVAL INTO :NEW.ID  FROM DUAL;
END;
/

--#########################################################
--INDEXES
--#########################################################

--TODO: make these conditional

/* 'Order' index for Structure Identifiers */
CREATE INDEX &&schemaName..STR_IDN_OrderIndex_IX ON &&schemaName..STRUCTURE_IDENTIFIER(Orderindex ASC) TABLESPACE &&indexTableSpaceName;

/* Unique constraints are actually unique indices */
create unique index &&schemaName..UNQ_REGNUM_REG_NUMBER on &&schemaName..REG_NUMBERS (reg_number) TABLESPACE &&indexTableSpaceName;
create unique index &&schemaName..UNQ_BATCHES_FULLREGNUMBER on &&schemaName..BATCHES (fullregnumber) TABLESPACE &&indexTableSpaceName;

/* Foreign Key indices */
CREATE INDEX &&schemaName..COMP_FRAG_FragmentID_IX ON &&schemaName..COMPOUND_FRAGMENT (FragmentID ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..COMP_FRAG_CompoundID_IX ON &&schemaName..COMPOUND_FRAGMENT (CompoundID ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..BATCHCOMPFRAG_BatchCompID_IX ON &&schemaName..BATCHCOMPONENTFRAGMENT (BatchComponentID ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..BATCHCOMPFRAG_CompFragID_IX ON &&schemaName..BATCHCOMPONENTFRAGMENT (CompoundFragmentID ASC) TABLESPACE &&indexTableSpaceName;

--#########################################################
--CONSTRAINTS
--#########################################################

ALTER TABLE &&schemaName..STRUCTURE_IDENTIFIER ADD CONSTRAINT STR_IDN_STRUCTURES_FK foreign key (STRUCTUREID) 
REFERENCES &&schemaName..STRUCTURES (CPD_INTERNAL_ID); 

ALTER TABLE &&schemaName..STRUCTURE_IDENTIFIER ADD CONSTRAINT STR_IDN_IDENTIFIER_FK FOREIGN KEY (TYPE) 
REFERENCES &&schemaName..IDENTIFIERS (IDENTIFIER_TYPE); 

--#########################################################
--VIEWS
--#########################################################

CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_Structure_Identifier
	(ID,TYPE,StructureID,Value,OrderIndex) AS 
	SELECT ID,TYPE,StructureID,VALUE,ORDERINDEX FROM &&schemaName..STRUCTURE_IDENTIFIER;
	
DELETE FROM &&schemaName..VW_PICKLIST WHERE PICKLISTDOMAIN = 1;
commit;

DElETE FROM &&schemaName..VW_PICKLISTDOMAIN WHERE ID = 1;
commit;

/* Modify the VW_STRUCTURE view to include the "DrawingType" column */
declare
  LViewName VARCHAR2(20) := 'VW_STRUCTURE'; --case sensitive
  LViewSelect VARCHAR2(4000); --view definition
  LViewFields VARCHAR2(4000); --comma seperated existing fileds of view
  LViewNewField_Alias VARCHAR2(50) := 'DrawingType'; --alias of new field in VW_STRUCTURE
  LViewNewField_ColumnName VARCHAR2(50) := 'Drawing_Type'; --new field to be added
  LFieldExist number; --whether the field to be added exists
  cursor LCViewFields(viewname varchar2) is
    select c.column_name
    from all_tab_columns c
    where c.owner = upper('&&schemaName') and c.table_name = viewname
    order by c.column_id;
begin
  FOR RViewFields IN LCViewFields(LViewName) LOOP
    LViewFields := LViewFields || RViewFields.Column_Name || ',';
  END LOOP;
  LViewFields := SUBSTR(LViewFields,1,LENGTH(LViewFields)-1); --remove last ','

  SELECT Text INTO LViewSelect
  FROM all_views v
  WHERE v.owner = upper('&&schemaName')
    and v.view_name = upper(LViewName);

  SELECT Count(1) INTO LFieldExist
  FROM all_tab_cols t
  where t.owner = upper('&&schemaName')
    and UPPER(t.Table_name) = UPPER(LViewName)
    and t.Column_Name = UPPER(LViewNewField_Alias);

  IF ( LFieldExist = 0 ) THEN
    --add new column to select clause
    LViewSelect := 
      SUBSTR(LViewSelect,1,INSTR(LViewSelect,'FROM ')-2)
      || ',' || LViewNewField_ColumnName||' '
      || SUBSTR(LViewSelect,INSTR(LViewSelect,'FROM '),LENGTH(LViewSelect));
    --add new column to columns' alias
    LViewSelect := 
      'CREATE OR REPLACE VIEW &&schemaName..' || LViewName
      || ' (' || LViewFields || ',' || LViewNewField_Alias || ') AS '
      || LViewSelect;
  ELSE
    --recompile view if the field already exists
    LViewSelect:='ALTER VIEW &&schemaName..'||LViewName|| ' COMPILE';
  END IF;

  execute immediate LViewSelect;
END;
/
prompt Created or bypassed generation of "DrawingType" column in "VW_STRUCTURE" view

/* Recompile VW_MIXTURE_STRUCTURE */
declare
  LStatement Varchar2(4000);	--view definition
  LSubQuery Varchar2(4000);	--subquery in view definition
  LViewName Varchar2(50) := 'VW_MIXTURE_STRUCTURE';	 --view to be recompiled
  LViewAlias Varchar2(800);	--field alias of the view

  cursor LCViewFields(viewname varchar2) is
    select column_name
    from all_tab_cols c
    where c.owner = upper('&&schemaName')
      and c.table_name = viewname
    order by c.column_id;
begin
  LViewAlias := '';
  FOR RViewFields IN LCViewFields(LViewName) LOOP
    LViewAlias:=LViewAlias||RViewFields.Column_Name||',';
  END LOOP;
  LViewAlias := SUBSTR(LViewAlias,1,LENGTH(LViewAlias)-1); --remove last ','

  select text into lsubquery
  from all_views v
  where v.owner = upper('&&schemaName')
    and view_name = upper('VW_MIXTURE_STRUCTURE');

  LStatement := 'CREATE OR REPLACE NOFORCE VIEW &&schemaName..'||LViewName||' ('|| LViewAlias || ') as '||LSubQuery;
  Execute Immediate LStatement;
end;
/
prompt Recompiled "VW_MIXTURE_STRUCTURE" view

/* create view VW_Structure_Drawing
VW_Structure may already have custom properties in its definition, so we need to dynamically
create the definition of VW_Structure_Drawing to include those fields for custom properties.

The resulting VW_Structure_Drawing will have the same schema as VW_Structure, but data for
STRUCTUREFORMAT and STRUCTURE may come from Drawing_Type table
*/
declare
  LStatement   Varchar2(4000);
  LViewFields Varchar2(800); --fields of VW_Structure other than 'STRUCTUREFORMAT' and 'STRUCTURE'
  LViewAlias  Varchar2(800); --alias to be used as VW_Structure_Drawing
  LViewSelectList  Varchar2(1000);
  cursor LCViewFields(viewname varchar2) is
    select c.column_name
    from all_tab_cols c
    where c.owner = upper('&&schemaName')
      and c.table_name = viewname
    order by c.column_id;

begin
  --GET VW_STRUCTURE fields (except for 'STRUCTUREFORMAT' and 'STRUCTURE')
  FOR RViewFields IN LCViewFields('VW_STRUCTURE') LOOP
    if( RViewFields.Column_Name != 'STRUCTUREFORMAT' and RViewFields.Column_Name != 'STRUCTURE') then
      LViewFields := LViewFields || RViewFields.Column_Name || ',';
    end if;
  END LOOP;

  LViewFields := SUBSTR(LViewFields,1,LENGTH(LViewFields)-1); --remove last ','
  LViewAlias := 'STRUCTUREFORMAT,STRUCTURE,' || LViewFields;
  LViewSelectList := '
    case
      when S.DrawingType = 0 then S.StructureFormat
      else T.StructureFormat
    end as StructureFormat,
    case
      when S.DrawingType = 0 then S.Structure
      else T.DEFAULT_DRAWING
    end as Structure,' || LViewFields;

  LStatement := 'CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_STRUCTURE_DRAWING (' || LViewAlias || ')' ||
    ' as select ' ||  LViewSelectList || 
    ' FROM &&schemaName..VW_Structure S left join &&schemaName..DRAWING_TYPE T ON S.DrawingType=T.ID';

  execute immediate LStatement;
end;
/
prompt Created or bypassed "VW_STRUCTURE_DRAWING" view creation

--#########################################################
--CONTEXTS
--#########################################################

--#########################################################
--TYPES
--#########################################################

set define off
@"sql\Patches\Patch 11.0.4\Packages\typ_idlist.sql"
set define on

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--#########################################################
--GRANTS
--#########################################################

--#########################################################
--PACKAGES
--#########################################################

set define off
@"sql\Patches\Patch 11.0.4\Packages\pkg_ConfigurationCompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
set define on

-- The following EXECs require the new package method (ConfigurationCompoundRegistry.AddFieldToView)

/* Add view field for Structure Identifiers for temporary Registrations */
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCTIDENTIFIERXML', 'STRUCTIDENTIFIERXML', 'VW_TEMPORARYCOMPOUND', 'TEMPORARY_COMPOUND');

/* Add view field for Batch Component for MixtureComponentId */
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('MIXTURECOMPONENTID', 'MIXTURECOMPONENTID', 'VW_BATCHCOMPONENT', 'BATCHCOMPONENT');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('PERCENTAGE', 'PERCENTAGE', 'VW_BATCHCOMPONENT', 'BATCHCOMPONENT');

/* Add default structure properties */
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_COMMENTS', 'STRUCT_COMMENTS', 'VW_STRUCTURE', 'STRUCTURES');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_NAME', 'STRUCT_NAME', 'VW_STRUCTURE', 'STRUCTURES');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_COMMENTS', 'STRUCT_COMMENTS', 'VW_TEMPORARYCOMPOUND', 'TEMPORARY_COMPOUND');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_NAME', 'STRUCT_NAME', 'VW_TEMPORARYCOMPOUND', 'TEMPORARY_COMPOUND');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_COMMENTS', 'STRUCT_COMMENTS', 'VW_STRUCTURE_DRAWING', 'STRUCTURES');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_NAME', 'STRUCT_NAME', 'VW_STRUCTURE_DRAWING', 'STRUCTURES');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_COMMENTS', 'STRUCT_COMMENTS', 'VW_MIXTURE_STRUCTURE', 'STRUCTURES');
EXEC &&schemaName..ConfigurationCompoundRegistry.AddFieldToView('STRUCT_NAME', 'STRUCT_NAME', 'VW_MIXTURE_STRUCTURE', 'STRUCTURES');

set define off
@"sql\Patches\Patch 11.0.4\Packages\pkg_RegistrationRLS_def.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_RegistryDuplicateCheck_def.sql"
@"sql\Patches\Patch 11.0.4\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 11.0.4\Packages\Pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 11.0.4\Packages\Pkg_Gui_Util_body.sql"
set define on

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--CHANGE OBJECT PRIVILEGES
--#########################################################

delete from &&securitySchemaName..OBJECT_PRIVILEGES WHERE OBJECT_NAME='CHEM_REG_PRIVILEGES';
COMMIT;

--#########################################################
--DATA
--#########################################################

@"sql\Patches\Patch 11.0.4\UpdateConfigurations.sql"
COMMIT;

@"sql\Patches\Patch 11.0.4\PageControlSettings.sql"
COMMIT;

@"sql\Patches\Patch 11.0.4\RegAdminPrivileges.sql"
COMMIT;

/* Update existing data of Compound_Molecule
	for each record in compound_molecule with structureid < 0:
		1. create a new structure in Structures table
		2. set StructureId as new generated Structures.CPD_INTERNAL_ID
*/
declare
  C_Compound SYS_REFCURSOR; 
  LCompoundID number(8,0);
  LOriginalStructureID number(8,0);
  LNewStructureID number(8,0);
  LDrawing_Type NUMBER(1,0);
  LCreated_Date  date;
begin
  OPEN C_Compound for
    select CompoundID,STRUCTUREID,datecreated from &&schemaName..vw_compound where structureid in (-1, -2, -3);
  loop
    fetch C_Compound into LCompoundID, LOriginalStructureID, LCreated_Date;
    LDrawing_Type:=ABS(LOriginalStructureID);
    EXIT WHEN C_Compound%NOTFOUND;
    SELECT &&schemaName..MOLID_SEQ.NEXTVAL INTO LNewStructureID FROM DUAL;
    --insert new record into Structures
    insert into &&schemaName..Structures(CPD_INTERNAL_ID, base64_cdx, datetime_stamp, DRAWING_TYPE) values(LNewStructureID,null,LCreated_Date,LDrawing_Type);
    --update StructureID of Compound_Molecule
    update &&schemaName..Compound_Molecule set STRUCTUREID=LNewStructureID where cpd_database_counter=LCompoundID;
  end loop;
  Close C_Compound;
end;
/
COMMIT;

--set Drawing_Type as 0 for all existing structures with CPD_INTERNAL_ID > 1, that is, for all Chemical structures
update &&schemaName..Structures set Drawing_Type=0 where CPD_INTERNAL_ID>0;
COMMIT;

--update records in Structures with CPD_INTERNAL_ID < 0
delete from &&schemaName..Structures where cpd_internal_id<0;
COMMIT;

/* Get the current value for the BatchNumberLength system setting and put it into the new Sequence.BatchNumber_Length column */
declare
  v_batchnum_length number := 2;
  v_configxml xmltype;
  v_exists number(1) := 0;
begin
  select count(1) into v_exists 
  from &&securitySchemaName..coeconfiguration c where upper(c.description) = 'REGISTRATION';
  
  if(v_exists > 0) then
	  select c.configurationxml into v_configxml
	  from &&securitySchemaName..coeconfiguration c where upper(c.description) = 'REGISTRATION';
	  
	  select extract(
		v_configxml,
		'Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="BatchNumberLength"]/@value'
	  ).GetNumberVal() into v_batchnum_length
	  from dual;
	  
	  update &&securitySchemaName..coeconfiguration c
	  set c.configurationxml = deletexml(
		c.configurationxml, 'Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="BatchNumberLength"]')
	  where upper(c.description) = 'REGISTRATION';
	  
	  dbms_output.put_line('Eliminated BatchNumberLength setting from "Registration/applicationSettings/groups/add[@name="MISC"]');
  end if;
  
  dbms_output.put_line('BatchNumberLength setting value is "' || v_batchnum_length || '".');
  
  update &&schemaName..Sequence s
  set s.batchnumber_length = v_batchnum_length
  where upper(to_char(s.type)) = upper('R');

  dbms_output.put_line('Updates the Registry-level sequences using the BatchNumberLength setting value "' || v_batchnum_length || '".');
  
end;
/
commit;

/* Update the use of 'GetSaltCode' with 'code' for the salt suffix type */
begin
  update &&schemaName..Sequence s
  set s.saltsuffixtype = 
    case
      when upper(s.saltsuffixtype) = 'GETSALTCODE' then 'code'
      else NULL
    end
  where upper(to_char(s.type)) = upper('R');
  dbms_output.put_line('Converted "GetSaltCode" batch suffix generator to "code" to match new Table Editor configuration');
end;
/
commit;

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
SELECT CASE
  WHEN '&&toVersion'='&&currentPatch' THEN 'sql\Patches\stop.sql'
  ELSE '"sql\Patches\Patch &&nextPatch\patch.sql"'
END AS setNextPatch 
FROM DUAL;

