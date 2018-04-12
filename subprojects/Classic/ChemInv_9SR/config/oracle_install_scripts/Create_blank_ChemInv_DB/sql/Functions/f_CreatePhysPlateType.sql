-- CreatePhysicalPlateType
CREATE OR REPLACE FUNCTION             
"CREATEPHYSPLATETYPE"
(
 pPhysPlateName IN inv_physical_plate.phys_plate_name%Type,
 pRowCount IN inv_grid_format.row_count%Type,
 pColCount IN inv_grid_format.col_count%Type,
 pRowPrefix IN inv_grid_format.row_prefix%type,
 pColPrefix in inv_grid_format.col_prefix%type,
 pRowUseLetters in inv_grid_format.row_use_letters%type,
 pColUseLetters in inv_grid_format.col_use_letters%type,
 pNameSeparator in inv_grid_format.name_separator%type,
 pNumberStartCorner in inv_grid_format.number_start_corner%type,
 pNumberDirection in inv_grid_format.number_direction%type,
 pSupplierIdFK in inv_physical_plate.supplier_id_fk%Type,
 pIsPreBarcoded IN inv_physical_plate.is_pre_barcoded%Type,
 pWellCapacity IN inv_physical_plate.well_capacity%Type,
 pCapacityUnitId in inv_physical_plate.capacity_unit_id_fk%type)
return inv_physical_plate.phys_plate_id%Type

IS

NewPhysicalPlateId inv_physical_plate.phys_plate_id%Type;
NewGridFormatId inv_grid_format.grid_format_id%Type;
row_counter inv_grid_position.row_index%Type;
col_counter inv_grid_position.col_index%Type;

BEGIN

-- create new grid format
-- 9 is the constant in INV_ENUMERATION for a Plate grid type

NewGridFormatId := CreateGridFormat(9, pRowCount, pColCount, pRowPrefix, pColPrefix, pRowUseLetters, pColUseLetters, 
				    pNameSeparator, pNumberStartCorner, pNumberDirection, pPhysPlateName, pPhysPlateName);

-- insert into phys plate 

insert into inv_physical_plate
(phys_plate_name, supplier_id_fk, is_pre_barcoded, grid_format_id_fk, well_capacity, capacity_unit_id_fk)
values
(pPhysPlateName, pSupplierIDFk, pIsPreBarCoded, NewGridFormatId, pWellCapacity, pCapacityUnitID)
returning phys_plate_id into NewPhysicalPlateId;


RETURN NewPhysicalPlateId;

END CreatePhysPlateType;
/
show errors;