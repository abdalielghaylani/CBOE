-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
prompt 'dropping public synonyms...' 

DECLARE
	PROCEDURE dropSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 1 then
				execute immediate 'DROP PUBLIC SYNONYM ' || synName;
			end if;
		END dropSynonym;
BEGIN	
	dropSynonym('INV_UNITS'); 
	dropSynonym('INV_UNIT_TYPES');
	dropSynonym('INV_UNIT_CONVERSION');
	dropSynonym('INV_COMPOUNDS'); 
	dropSynonym('INV_SYNONYMS'); 
	dropSynonym('INV_CONTAINERS'); 
	dropSynonym('INV_LOCATIONS');
	dropSynonym('INV_LOCATION_TYPES');
	dropSynonym('INV_CONTAINER_TYPES'); 
	dropSynonym('INV_SUPPLIERS');
	dropSynonym('INV_PHYSICAL_STATE');
	dropSynonym('INV_CONTAINER_STATUS');
	dropSynonym('INV_RESERVATIONS');
	dropSynonym('INV_RESERVATION_TYPES');
	dropSynonym('INV_API_ERRORS');
	dropSynonym('INV_GRID_FORMAT');
	dropSynonym('INV_GRID_FILL_TEMPLATE');
	dropSynonym('INV_GRID_POSITION');
	dropSynonym('INV_GRID_STORAGE');
	dropSynonym('INV_GRID_ELEMENT');
	dropSynonym('INV_ENUMERATION');
	dropSynonym('INV_ENUMERATION_SET');
	dropSynonym('INV_ESET_TYPE');
	dropSynonym('INV_ENUM_VALUES');
	dropSynonym('INV_PHYSICAL_PLATE');
	dropSynonym('INV_PLATE_FORMAT');
	dropSynonym('INV_PLATE_TYPES');
	dropSynonym('INV_PLATES');
	dropSynonym('INV_PLATE_HISTORY');
	dropSynonym('INV_PLATE_ACTIONS');
	dropSynonym('INV_WELLS');
	dropSynonym('INV_WELL_COMPOUNDS');
	dropSynonym('INV_ALLOWED_CTYPES');
	dropSynonym('INV_ALLOWED_LTYPES');
	dropSynonym('INV_ALLOWED_PTYPES');
	dropSynonym('INV_BARCODE_DESC');
	dropSynonym('INV_REQUESTS');
	dropSynonym('INV_SOLVENTS');
	dropSynonym('INV_REPORTTYPES');
	dropSynonym('INV_CONTAINER_ORDER');
	dropSynonym('CUSTOM_CHEM_ORDER');
	dropSynonym('CUSTOM_ACX_ST_VENDORS');
	dropSynonym('INV_CONTAINER_ORDER_REASON');
	dropSynonym('INV_EHS_CAS_SUBSTANCE');
	dropSynonym('INV_EHS_CATNUM_SUBSTANCE');
	dropSynonym('INV_EHS_SUBSTANCES');
	dropSynonym('INV_EXCLUDE_CONTAINER_TYPES');
	dropSynonym('INV_OWNERS');
	dropSynonym('INV_PICKLISTS');
	dropSynonym('INV_PROJECT_JOB_INFO');
	dropSynonym('INV_URL');
	dropSynonym('INV_XMLDOCS');
	dropSynonym('INV_XMLDOC_TYPES');
	dropSynonym('INV_XSLTS');
	dropSynonym('INV_WELL_PARENT');
	dropSynonym('INV_PLATE_PARENT');
	dropSynonym('INV_COUNTRY');
	dropSynonym('INV_STATES');
	dropSynonym('INV_ADDRESS');
	dropSynonym('INV_REQUEST_TYPES');
	dropSynonym('INV_REQUEST_STATUS');
	dropSynonym('INV_REQUEST_SAMPLES');
	dropSynonym('INV_ORDER_STATUS');
	dropSynonym('INV_ORDERS');
	dropSynonym('INV_ORDER_CONTAINERS');    
  dropSynonym('INV_PLATE_PARENT');
  dropSynonym('INV_WELL_PARENT');
  dropSynonym('INV_WELL_COMPOUNDS');
	-- DROP VIEW SYNONYMS
	dropSynonym('INV_VW_PHYSICAL_PLATE');
	dropSynonym('INV_VW_PLATE_FORMAT');
	dropSynonym('INV_VW_WELL_FORMAT');
  dropSynonym('INV_VW_WELL_FLAT');
	dropSynonym('INV_VW_PLATE');
	dropSynonym('INV_VW_WELL');
	dropSynonym('INV_VW_GRID_LOCATION');
	dropSynonym('INV_VW_GRID_LOCATION_PARENT');
	dropSynonym('INV_VW_ENUMERATED_VALUES');
	dropSynonym('INV_VW_PLATE_LOCATIONS');
	dropSynonym('INV_VW_NONGRID_LOCATIONS');
	dropSynonym('INV_VW_PLATE_GRID_LOCATIONS');
	dropSynonym('INV_VW_PLATE_HISTORY');
	dropSynonym('INV_VW_PLATE_LOCATIONS_ALL');     
END;
/

prompt 'dropping user...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n = 1 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/

prompt 'dropping tablespaces...'

DECLARE
	n NUMBER;
	dataFileClause varchar2(20);
BEGIN
	if &&OraVersionNumber =  8 then 
		dataFileClause := '';
	else 
		dataFileClause := 'AND DATAFILES';	
	end if;
	select count(*) into n from dba_tablespaces where tablespace_name = '&&tableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
			
	select count(*) into n from dba_tablespaces where tablespace_name = '&&indexTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&indexTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&lobsTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&lobsTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&auditTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&auditTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
end;
/

prompt 'dropping test users...'

DECLARE
	PROCEDURE dropUser(uName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_users where Upper(username) = uName;
			if n > 0 then
				execute immediate 'DROP USER ' || uName;
			end if;
		END dropUser;
BEGIN
	dropUser('INVADMIN');
	dropUser('INVCHEMIST');
	dropUser('INVREGISTRAR');
	dropUser('INVRECEIVING');
	dropUser('INVBROWSER');
	dropUser('INVFINANCE');
end;
/

prompt 'dropping test roles...'

DECLARE
	PROCEDURE dropRole(roleName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_roles where Upper(role) = roleName;
			if n > 0 then
				execute immediate 'DROP ROLE ' || roleName;
			end if;
		END dropRole;
BEGIN
	dropRole('INV_ADMIN');
	dropRole('INV_CHEMIST');
	dropRole('INV_REGISTRAR');
	dropRole('INV_RECEIVER');
	dropRole('INV_BROWSER');
	dropRole('INV_FINANCE');
END;
/

prompt 'Finished dropping &&schemaName.'

