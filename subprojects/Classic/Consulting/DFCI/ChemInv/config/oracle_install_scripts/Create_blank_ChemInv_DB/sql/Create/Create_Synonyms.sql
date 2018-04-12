-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create schema views
--######################################################### 

connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Creating synonyms ...'
prompt '#########################################################'

DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN
	createSynonym('INV_UNITS'); 
	createSynonym('INV_UNIT_TYPES');
	createSynonym('INV_UNIT_CONVERSION');
	createSynonym('INV_COMPOUNDS'); 
	createSynonym('INV_SYNONYMS'); 
	createSynonym('INV_CONTAINERS'); 
	createSynonym('INV_LOCATIONS');
	createSynonym('INV_LOCATION_TYPES');
	createSynonym('INV_CONTAINER_TYPES'); 
	createSynonym('INV_SUPPLIERS');
	createSynonym('INV_PHYSICAL_STATE');
	createSynonym('INV_CONTAINER_STATUS');
	createSynonym('INV_RESERVATIONS');
	createSynonym('INV_RESERVATION_TYPES');
	createSynonym('INV_API_ERRORS');
	createSynonym('INV_GRID_FORMAT');
	createSynonym('INV_GRID_FILL_TEMPLATE');
	createSynonym('INV_GRID_POSITION');
	createSynonym('INV_GRID_STORAGE');
	createSynonym('INV_GRID_ELEMENT');
	createSynonym('INV_ENUMERATION');
	createSynonym('INV_ENUMERATION_SET');
	createSynonym('INV_ESET_TYPE');
	createSynonym('INV_ENUM_VALUES');
	createSynonym('INV_PHYSICAL_PLATE');
	createSynonym('INV_PLATE_FORMAT');
	createSynonym('INV_PLATE_TYPES');
	createSynonym('INV_PLATES');
	createSynonym('INV_PLATE_HISTORY');
	createSynonym('INV_PLATE_ACTIONS');
	createSynonym('INV_WELLS');
	createSynonym('INV_WELL_COMPOUNDS');
	createSynonym('INV_ALLOWED_CTYPES');
	createSynonym('INV_ALLOWED_LTYPES');
	createSynonym('INV_ALLOWED_PTYPES');
	createSynonym('INV_BARCODE_DESC');
	createSynonym('INV_REQUESTS');
	createSynonym('INV_SOLVENTS');
	createSynonym('INV_REPORTTYPES');
	createSynonym('INV_CONTAINER_ORDER');
	createSynonym('CUSTOM_CHEM_ORDER');
	createSynonym('CUSTOM_ACX_ST_VENDORS');
	createSynonym('INV_CONTAINER_ORDER_REASON');
	createSynonym('INV_EHS_CAS_SUBSTANCE');
	createSynonym('INV_EHS_CATNUM_SUBSTANCE');
	createSynonym('INV_EHS_SUBSTANCES');
	createSynonym('INV_EXCLUDE_CONTAINER_TYPES');
	createSynonym('INV_OWNERS');
	createSynonym('INV_PICKLISTS');
	createSynonym('INV_PROJECT_JOB_INFO');
	createSynonym('INV_URL');
	createSynonym('INV_XMLDOCS');
	createSynonym('INV_XMLDOC_TYPES');
	createSynonym('INV_XSLTS');
	createSynonym('INV_WELL_PARENT');
	createSynonym('INV_PLATE_PARENT');
	createSynonym('INV_COUNTRY');
	createSynonym('INV_STATES');
	createSynonym('INV_ADDRESS');
	createSynonym('INV_REQUEST_TYPES');
	createSynonym('INV_REQUEST_STATUS');
	createSynonym('INV_REQUEST_SAMPLES');
	createSynonym('INV_ORDER_STATUS');
	createSynonym('INV_ORDERS');
	createSynonym('INV_ORDER_CONTAINERS');    
  	createSynonym('INV_PLATE_PARENT');
  	createSynonym('INV_WELL_PARENT');
  	createSynonym('INV_WELL_COMPOUNDS');
 	createSynonym('INV_DATA_MAPS');
	createSynonym('INV_DATA_MAPPINGS');
	createSynonym('INV_MAP_FIELDS');
	createSynonym('INV_GRAPHICS');
	createSynonym('INV_GRAPHIC_TYPES');
	createSynonym('INV_CONTAINER_BATCHES');
	createSynonym('INV_UNIT_CONVERSION_FORMULA');
	createSynonym('INV_DOCS');
	createSynonym('INV_DOC_TYPES');
	createSynonym('INV_ORG_UNIT');
	createSynonym('INV_ORG_ROLES');
	createSynonym('INV_ORG_USERS');
	createSynonym('INV_BATCH_STATUS');
	
	-- CREATE VIEW SYNONYMS
	createSynonym('INV_VW_PHYSICAL_PLATE');
	createSynonym('INV_VW_PLATE_FORMAT');
	createSynonym('INV_VW_WELL_FORMAT');
  	createSynonym('INV_VW_WELL_FLAT');
	createSynonym('INV_VW_PLATE');
	createSynonym('INV_VW_WELL');
	createSynonym('INV_VW_GRID_LOCATION');
	createSynonym('INV_VW_GRID_LOCATION_PARENT');
	createSynonym('INV_VW_ENUMERATED_VALUES');
	createSynonym('INV_VW_PLATE_LOCATIONS');
	createSynonym('INV_VW_NONGRID_LOCATIONS');
	createSynonym('INV_VW_PLATE_GRID_LOCATIONS');
	createSynonym('INV_VW_PLATE_HISTORY');
	createSynonym('INV_VW_PLATE_LOCATIONS_ALL');     
	createSynonym('INV_VW_GRID_LOCATION_LITE');
	createSynonym('INV_VW_REG_BATCHES');
	createSynonym('INV_VW_REG_STRUCTURES');
	createSynonym('INV_VW_REG_ALTIDS');
	
	
END;
/                                           


