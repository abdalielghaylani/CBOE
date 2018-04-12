-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
Connect &&schemaName/&&schemaPass@&&serverName
-- Create Views

CREATE OR REPLACE VIEW INV_VW_PHYSICAL_PLATE (
   PHYS_PLATE_ID,
   PHYS_PLATE_NAME,
   ROW_COUNT,
   COL_COUNT,
   ROW_PREFIX,
   COL_PREFIX,
   ROW_USE_LETTERS,
   COL_USE_LETTERS,
   NAME_SEPARATOR,
   NUMBER_START_CORNER,
   NUMBER_DIRECTION,
   SUPPLIER_ID_FK,
   GRID_FORMAT_ID_FK,
   IS_PRE_BARCODED,
   WELL_CAPACITY,
   CAPACITY_UNIT_ID_FK
)
AS
   SELECT PHYS_PLATE_ID,
	  PHYS_PLATE_NAME,
	  ROW_COUNT,
	   COL_COUNT,
	   ROW_PREFIX,
	   COL_PREFIX,
	   ROW_USE_LETTERS,
	   COL_USE_LETTERS,
	   NAME_SEPARATOR,
	   NUMBER_START_CORNER,
	   NUMBER_DIRECTION,
	   SUPPLIER_ID_FK,
	   GRID_FORMAT_ID_FK,
	   IS_PRE_BARCODED,
	   WELL_CAPACITY,
	   CAPACITY_UNIT_ID_FK
     FROM INV_PHYSICAL_PLATE, INV_GRID_FORMAT
    WHERE GRID_FORMAT_ID_FK = GRID_FORMAT_ID;

CREATE OR REPLACE VIEW INV_VW_PLATE_FORMAT (
   PLATE_FORMAT_ID,
   PLATE_FORMAT_NAME,
   PHYS_PLATE_ID,
   PHYS_PLATE_NAME,
   ROW_COUNT,
   COL_COUNT,
   SUPPLIER_ID_FK,
   GRID_FORMAT_ID_FK,
   IS_PRE_BARCODED
)
AS
   SELECT
	PLATE_FORMAT_ID,
	PLATE_FORMAT_NAME,
	   PHYS_PLATE_ID,
	  PHYS_PLATE_NAME,
	  ROW_COUNT,
	   COL_COUNT,
	   SUPPLIER_ID_FK,
	   GRID_FORMAT_ID_FK,
	   IS_PRE_BARCODED
     FROM INV_PLATE_FORMAT, INV_VW_PHYSICAL_PLATE
    WHERE PHYS_PLATE_ID_FK = PHYS_PLATE_ID;

CREATE OR REPLACE VIEW INV_VW_PLATE ( PLATE_ID,
	PLATE_BARCODE,
	DATE_CREATED,
	LOCATION_ID_FK,
	GROUP_NAME,
	LIBRARY_ID_FK,
	LIBRARY_NAME,
	CONTAINER_ID_FK,
	PLATE_TYPE_ID_FK,
	STATUS_ID_FK,
	PLATE_FORMAT_ID_FK,
	FT_CYCLES,
	PLATE_FORMAT_NAME,
	PHYS_PLATE_ID,
	PHYS_PLATE_NAME,
	ROW_COUNT,
	COL_COUNT,
	SUPPLIER_ID_FK,
	GRID_FORMAT_ID_FK,
	IS_PRE_BARCODED,
	WEIGHT,
	WEIGHT_UNIT_FK,
	QTY_INITIAL,
	QTY_REMAINING,
	QTY_UNIT_FK,
	SOLVENT,
	CONCENTRATION,
	CONC_UNIT_FK
) AS Select
inv_plates.plate_id,
inv_plates.plate_barcode,
inv_plates.date_created,
inv_plates.location_id_fk,
inv_plates.group_name,
inv_plates.library_id_fk,
inv_enumeration.enum_value,
inv_plates.container_id_fk,
inv_plates.plate_type_id_fk,
inv_plates.status_id_fk,
inv_plates.plate_format_id_fk,
inv_plates.ft_cycles,
inv_vw_plate_format.PLATE_FORMAT_NAME,
inv_vw_plate_format.PHYS_PLATE_ID,
inv_vw_plate_format.PHYS_PLATE_NAME,
inv_vw_plate_format.ROW_COUNT,
inv_vw_plate_format.COL_COUNT,
inv_vw_plate_format.SUPPLIER_ID_FK,
inv_vw_plate_format.GRID_FORMAT_ID_FK,
inv_vw_plate_format.IS_PRE_BARCODED,
inv_plates.WEIGHT,
inv_plates.WEIGHT_UNIT_FK,
inv_plates.QTY_INITIAL,
inv_plates.QTY_REMAINING,
inv_plates.QTY_UNIT_FK,
inv_solvents.SOLVENT_NAME,
inv_plates.CONCENTRATION,
inv_plates.CONC_UNIT_FK
from inv_plates, inv_enumeration, inv_vw_plate_format,inv_solvents
where inv_plates.library_id_fk = inv_enumeration.ENUM_ID(+) and inv_plates.solvent_id_fk = inv_solvents.solvent_id (+)
and inv_plates.plate_format_id_fk = inv_vw_plate_format.plate_format_id;

CREATE OR REPLACE VIEW INV_VW_WELL_FORMAT ( WELL_ID,
WELL_FORMAT_ID_FK, CONCENTRATION, CONC_UNIT_FK, PLATE_FORMAT_ID_FK,
GRID_POSITION_ID, GRID_FORMAT_ID_FK, ROW_INDEX, COL_INDEX,
ROW_NAME, COL_NAME, NAME, SORT_ORDER
 ) AS SELECT
	INV_WELLS.WELL_ID,
	INV_WELLS.WELL_FORMAT_ID_FK,
	INV_WELLS.CONCENTRATION,
	INV_WELLS.CONC_UNIT_FK,
	INV_WELLS.PLATE_FORMAT_ID_FK,
	INV_GRID_POSITION.GRID_POSITION_ID,
	INV_GRID_POSITION.GRID_FORMAT_ID_FK,
	INV_GRID_POSITION.ROW_INDEX,
	INV_GRID_POSITION.COL_INDEX,
	INV_GRID_POSITION.ROW_NAME,
	INV_GRID_POSITION.COL_NAME,
	INV_GRID_POSITION.NAME,
	INV_GRID_POSITION.SORT_ORDER
     FROM INV_WELLS, INV_GRID_POSITION
    WHERE GRID_POSITION_ID_FK = GRID_POSITION_ID
    AND plate_id_fk is null
    ORDER BY INV_GRID_POSITION.SORT_ORDER;

CREATE OR REPLACE VIEW INV_VW_WELL(
	WELL_ID,
	WELL_FORMAT_ID_FK,
	PLATE_ID_FK,
	COMPOUND_ID_FK,
	REG_ID_FK,
	BATCH_NUMBER_FK,
	MOL_ID,
	CAS,
	ACX_ID,
	SUBSTANCE_NAME,
	QTY_INITIAL,
	QTY_REMAINING,
	QTY_UNIT_FK,
	WEIGHT,
	WEIGHT_UNIT_FK,
	SOLVENT,
	CONCENTRATION,
	CONC_UNIT_FK,
	GRID_POSITION_ID,
	GRID_FORMAT_ID_FK,
	ROW_INDEX,
	COL_INDEX,
	ROW_NAME,
	COL_NAME,
	NAME,
	SORT_ORDER
) AS SELECT
	INV_WELLS.WELL_ID,
	INV_WELLS.WELL_FORMAT_ID_FK,
	INV_WELLS.PLATE_ID_FK,
	--INV_WELL_COMPOUNDS.COMPOUND_ID_FK,
	--INV_WELL_COMPOUNDS.REG_ID_FK,
	--INV_WELL_COMPOUNDS.BATCH_NUMBER_FK,
	--INV_COMPOUNDS.MOL_ID,
	--INV_COMPOUNDS.CAS,
	--INV_COMPOUNDS.ACX_ID,
	--INV_COMPOUNDS.SUBSTANCE_NAME,
  (SELECT compound_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id) AS compound_id_fk,
  (SELECT reg_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id) AS reg_id_fk,
  (SELECT batch_number_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id) AS batch_number_fk,
  (SELECT mol_id FROM inv_compounds WHERE compound_id in (SELECT compound_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id)) AS mol_id,
  (SELECT cas FROM inv_compounds WHERE compound_id in (SELECT compound_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id)) AS cas,
  (SELECT acx_id FROM inv_compounds WHERE compound_id in (SELECT compound_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id)) AS acx_id,
  (SELECT substance_name FROM inv_compounds WHERE compound_id in (SELECT compound_id_fk FROM inv_well_compounds WHERE well_compound_id = FIRST_COMPOUND_ROW.well_compound_id)) AS substance_name,
	INV_WELLS.QTY_INITIAL,
	INV_WELLS.QTY_REMAINING,
	INV_WELLS.QTY_UNIT_FK,
	INV_WELLS.WEIGHT,
	INV_WELLS.WEIGHT_UNIT_FK,
	INV_SOLVENTS.SOLVENT_NAME,
	INV_WELLS.CONCENTRATION,
	INV_WELLS.CONC_UNIT_FK,
	INV_GRID_POSITION.GRID_POSITION_ID,
	INV_GRID_POSITION.GRID_FORMAT_ID_FK,
	INV_GRID_POSITION.ROW_INDEX,
	INV_GRID_POSITION.COL_INDEX,
	INV_GRID_POSITION.ROW_NAME,
	INV_GRID_POSITION.COL_NAME,
	INV_GRID_POSITION.NAME,
	INV_GRID_POSITION.SORT_ORDER
  FROM INV_WELLS, INV_GRID_POSITION, INV_SOLVENTS, (SELECT min(well_compound_id) as well_compound_id, well_id_fk FROM INV_WELL_COMPOUNDS GROUP BY well_id_fk) FIRST_COMPOUND_ROW
  WHERE GRID_POSITION_ID_FK = GRID_POSITION_ID
		AND INV_WELLS.SOLVENT_ID_FK = INV_SOLVENTS.SOLVENT_ID(+)
    AND INV_WELLS.WELL_ID = FIRST_COMPOUND_ROW.well_id_fk(+)
  ORDER BY INV_GRID_POSITION.SORT_ORDER;

CREATE OR REPLACE VIEW INV_VW_WELL_FLAT(
	WELL_ID,
	WELL_FORMAT_ID_FK,
	PLATE_ID_FK,
	COMPOUND_ID_FK,
	REG_ID_FK,
	BATCH_NUMBER_FK,
	MOL_ID,
	CAS,
	ACX_ID,
	SUBSTANCE_NAME,
	QTY_INITIAL,
	QTY_REMAINING,
	QTY_UNIT_FK,
	WEIGHT,
	WEIGHT_UNIT_FK,
	SOLVENT,
	CONCENTRATION,
	CONC_UNIT_FK,
	GRID_POSITION_ID,
	GRID_FORMAT_ID_FK,
	ROW_INDEX,
	COL_INDEX,
	ROW_NAME,
	COL_NAME,
	NAME,
	SORT_ORDER
) AS SELECT
	INV_WELLS.WELL_ID,
	INV_WELLS.WELL_FORMAT_ID_FK,
	INV_WELLS.PLATE_ID_FK,
	INV_WELL_COMPOUNDS.COMPOUND_ID_FK,
	INV_WELL_COMPOUNDS.REG_ID_FK,
	INV_WELL_COMPOUNDS.BATCH_NUMBER_FK,
	--INV_COMPOUNDS.MOL_ID,
	--INV_COMPOUNDS.CAS,
	--INV_COMPOUNDS.ACX_ID,
	--INV_COMPOUNDS.SUBSTANCE_NAME,
  (SELECT mol_id FROM inv_compounds WHERE compound_id = INV_WELL_COMPOUNDS.well_compound_id) AS mol_id,
  (SELECT cas FROM inv_compounds WHERE compound_id = INV_WELL_COMPOUNDS.well_compound_id) AS cas,
  (SELECT acx_id FROM inv_compounds WHERE compound_id = INV_WELL_COMPOUNDS.well_compound_id) AS acx_id,
  (SELECT substance_name FROM inv_compounds WHERE compound_id = INV_WELL_COMPOUNDS.well_compound_id) AS substance_name,
  INV_WELLS.QTY_INITIAL,
	INV_WELLS.QTY_REMAINING,
	INV_WELLS.QTY_UNIT_FK,
	INV_WELLS.WEIGHT,
	INV_WELLS.WEIGHT_UNIT_FK,
	INV_SOLVENTS.SOLVENT_NAME,
	INV_WELLS.CONCENTRATION,
	INV_WELLS.CONC_UNIT_FK,
	INV_GRID_POSITION.GRID_POSITION_ID,
	INV_GRID_POSITION.GRID_FORMAT_ID_FK,
	INV_GRID_POSITION.ROW_INDEX,
	INV_GRID_POSITION.COL_INDEX,
	INV_GRID_POSITION.ROW_NAME,
	INV_GRID_POSITION.COL_NAME,
	INV_GRID_POSITION.NAME,
	INV_GRID_POSITION.SORT_ORDER
  FROM INV_WELLS, INV_GRID_POSITION, INV_SOLVENTS, INV_WELL_COMPOUNDS
  WHERE GRID_POSITION_ID_FK = GRID_POSITION_ID
  	AND INV_WELLS.WELL_ID = INV_WELL_COMPOUNDS.WELL_ID_FK(+)
		AND INV_WELLS.SOLVENT_ID_FK = INV_SOLVENTS.SOLVENT_ID(+)
  ORDER BY INV_GRID_POSITION.SORT_ORDER;
  
  
create or replace view inv_vw_grid_location (
	location_id,
	parent_id,
	description,
	location_type_id_fk,
	location_name,
	location_description,
	location_barcode,
	plate_id_fk,
	plate_barcode,
	row_index,
	col_index,
	row_name,
	col_name,
	name,
	sort_order
) as select
	inv_locations.location_id,
	inv_locations.parent_id,
	inv_locations.description,
	inv_locations.location_type_id_fk,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_grid_element.plate_id_fk,
	inv_vw_plate.plate_barcode,
	inv_grid_position.row_index,
	inv_grid_position.col_index,
	inv_grid_position.row_name,
	inv_grid_position.col_name,
	inv_grid_position.name,
	inv_grid_position.sort_order
from inv_locations, inv_grid_position, inv_grid_element, inv_vw_plate
where inv_locations.location_id = inv_grid_element.location_id_fk
and inv_grid_element.grid_position_id_fk = inv_grid_position.grid_position_id
and inv_grid_element.plate_id_fk = inv_vw_plate.plate_id(+)
order by inv_locations.parent_id, inv_grid_position.sort_order;

create or replace view inv_vw_nongrid_locations (
	location_id,
	location_name,
	location_description
) as select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description
from inv_locations, inv_grid_element
where inv_locations.location_id = inv_grid_element.location_id_fk(+)
and inv_grid_element.location_id_fk is null
and inv_locations.location_id > 999;


create or replace view inv_vw_grid_location_parent (
	location_id,
	location_name,
	location_description,
	location_barcode,
	row_count,
	col_count
) as select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_grid_format.row_count,
	inv_grid_format.col_count
from inv_locations, inv_grid_format, inv_grid_storage
where inv_locations.location_id = inv_grid_storage.location_id_fk
and inv_grid_storage.grid_format_id_fk = inv_grid_format.grid_format_id;

create or replace view inv_vw_plate_grid_locations (
	location_id,
	location_name,
	location_description,
	location_barcode,
	row_count,
	col_count,
	plate_type_id,
	plate_type_name
) as select
	inv_vw_grid_location_parent.*,
	inv_allowed_ptypes.plate_type_id_fk,
	inv_plate_types.plate_type_name
from inv_vw_grid_location_parent, inv_allowed_ptypes, inv_plate_types
where inv_allowed_ptypes.location_id_fk = inv_vw_grid_location_parent.location_id
and inv_allowed_ptypes.plate_type_id_fk = inv_plate_types.plate_type_id;

CREATE OR REPLACE
VIEW INV_VW_PLATE_LOCATIONS ( LOCATION_ID, LOCATION_NAME, LOCATION_DESCRIPTION, LOCATION_BARCODE, LOCATION_TYPE_NAME ) AS
select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_location_types.location_type_name
from inv_locations, inv_location_types where
	inv_locations.location_type_id_fk = inv_location_types.location_type_id(+)
	and inv_locations.location_id in
	(select distinct location_id_fk
	 from inv_allowed_ptypes) and location_id not in (select location_id_fk from inv_grid_element);


create or replace view inv_vw_enumerated_values (
	enum_id,
	enum_value,
	eset_id_fk,
	value,
	lookupsql
) as select
	inv_enumeration.enum_id,
	inv_enumeration.enum_value,
	inv_enumeration.eset_id_fk,
	inv_enum_values.value,
	inv_enum_values.lookupsql
from inv_enumeration, inv_enum_values
where inv_enumeration.enum_id = inv_enum_values.enum_id_fk(+);

CREATE OR REPLACE VIEW inv_vw_plate_history (
	plate_history_id,
	plate_id_fk,
	plate_barcode,
	plate_history_date,
	plate_action_name,
	from_location_name,
	to_location_name,
	is_ft_incremented
) AS SELECT
	INV_PLATE_HISTORY.plate_history_id,
	INV_PLATE_HISTORY.plate_id_fk,
	inv_vw_plate.plate_barcode,
	INV_PLATE_HISTORY.plate_history_date,
	INV_PLATE_ACTIONS.plate_action_name,
	FROM_LOCATIONS.location_name AS from_location_name,
	TO_LOCATIONS.location_name AS to_location_name,
	INV_PLATE_HISTORY.is_ft_incremented
FROM INV_PLATE_HISTORY, inv_vw_plate, INV_PLATE_ACTIONS, INV_LOCATIONS FROM_LOCATIONS, INV_LOCATIONS TO_LOCATIONS
WHERE INV_PLATE_HISTORY.plate_id_fk = inv_vw_plate.plate_id
AND INV_PLATE_HISTORY.plate_action_id_fk = INV_PLATE_ACTIONS.plate_action_id
AND INV_PLATE_HISTORY.from_location_id_fk = FROM_LOCATIONS.location_id(+)
AND INV_PLATE_HISTORY.to_location_id_fk = TO_LOCATIONS.location_id(+);


CREATE OR REPLACE VIEW INV_VW_GRID_LOCATION_LITE ( LOCATION_ID,
PARENT_ID ) AS select
	inv_locations.location_id,
	inv_locations.parent_id
from inv_locations, inv_grid_element
where inv_locations.location_id = inv_grid_element.location_id_fk;


CREATE OR REPLACE VIEW INV_VW_PLATE_LOCATIONS_ALL ( LOCATION_ID, PARENT_ID,
LOCATION_NAME, LOCATION_DESCRIPTION, LOCATION_BARCODE, LOCATION_TYPE_NAME
 ) AS select
	inv_locations.location_id,
	inv_locations.Parent_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_location_types.location_type_name
from inv_locations, inv_location_types where
	inv_locations.location_type_id_fk = inv_location_types.location_type_id(+)
	and inv_locations.location_id in
	(select distinct location_id_fk
	 from inv_allowed_ptypes);

create or replace view inv_vw_plate_locations (
	location_id,
	location_name,
	location_description,
	location_barcode,
	location_type_name
) as select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_location_types.location_type_name
from inv_locations, inv_location_types where
	inv_locations.location_type_id_fk = inv_location_types.location_type_id(+)
	and inv_locations.location_id in
	(select distinct location_id_fk
	 from inv_allowed_ptypes) and location_id not in (select location_id_fk from inv_grid_element);
   
CREATE OR REPLACE VIEW INV_VW_AUDIT_COLUMN_DISP ( RAID,
	CAID,
	COLUMN_NAME,
	OLDVALUE,
	NEWVALUE
) AS SELECT
  raid,
  caid,
  column_name,
  DECODE(column_name,
    'LOCATION_ID_FK', (select LOCATION_NAME from INV_LOCATIONS where LOCATION_ID = old_value),
    'DEF_LOCATION_ID_FK', (select LOCATION_NAME from INV_LOCATIONS where LOCATION_ID = old_value),
    'CONTAINER_TYPE_ID_FK', (select CONTAINER_TYPE_NAME from INV_CONTAINER_TYPES where CONTAINER_TYPE_ID = old_value),
    'CONTAINER_STATUS_ID_FK', (select CONTAINER_STATUS_NAME from INV_CONTAINER_STATUS where CONTAINER_STATUS_ID = old_value),
    'UNIT_OF_MEAS_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    'UNIT_OF_WGHT_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    'UNIT_OF_CONC_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    'UNIT_OF_PURITY_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    'UNIT_OF_DENSITY_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    'SUPPLIER_ID_FK',(select SUPPLIER_NAME from INV_SUPPLIERS where SUPPLIER_ID = old_value),
    'UNIT_OF_COST_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = old_value),
    old_value) AS oldvalue,
  DECODE(column_name,
    'LOCATION_ID_FK', (select LOCATION_NAME from INV_LOCATIONS where LOCATION_ID = new_value),
    'CONTAINER_TYPE_ID_FK', (select CONTAINER_TYPE_NAME from INV_CONTAINER_TYPES where CONTAINER_TYPE_ID = new_value),
    'CONTAINER_STATUS_ID_FK', (select CONTAINER_STATUS_NAME from INV_CONTAINER_STATUS where CONTAINER_STATUS_ID = new_value),
    'UNIT_OF_MEAS_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    'UNIT_OF_WGHT_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    'UNIT_OF_CONC_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    'UNIT_OF_PURITY_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    'UNIT_OF_DENSITY_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    'SUPPLIER_ID_FK',(select SUPPLIER_NAME from INV_SUPPLIERS where SUPPLIER_ID = new_value),
    'UNIT_OF_COST_ID_FK',(select UNIT_NAME from INV_UNITS where UNIT_ID = new_value),
    new_value) AS newvalue
FROM audit_column;
   
