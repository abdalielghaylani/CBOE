-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Drop all public synonyms
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'dropping public synonyms...' 
prompt '#########################################################'

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
	dropSynonym('INV_DATA_MAPS');
	dropSynonym('INV_DATA_MAPPINGS');
	dropSynonym('INV_MAP_FIELDS');
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
show errors;
