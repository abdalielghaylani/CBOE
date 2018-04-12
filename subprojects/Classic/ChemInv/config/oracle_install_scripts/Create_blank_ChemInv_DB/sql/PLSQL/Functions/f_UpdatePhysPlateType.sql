-- UpdatePhysicalPlateType

CREATE OR REPLACE FUNCTION  

"UPDATEPHYSPLATETYPE" 
	(pPhysPlateId IN inv_physical_plate.phys_plate_id%Type,
	 pGridFormatId IN inv_physical_plate.grid_format_id_fk%Type,
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
	 pWellCapacity In inv_physical_plate.well_capacity%type,
	 pCapacityUnitID in inv_physical_plate.capacity_unit_id_fk%type)

RETURN inv_physical_plate.phys_plate_id%Type

IS

ExistingRowCount inv_grid_format.row_count%type;
ExistingColCount inv_grid_format.col_count%type;
row_counter inv_grid_position.row_index%Type;
col_counter inv_grid_position.col_index%Type;
new_sort_order inv_grid_position.sort_order%type;

BEGIN

	-- update (delete and refresh) grid_positions if necessary
	select row_count, col_count into ExistingRowCount, ExistingColCount
	from inv_grid_format where grid_format_id = pGridFormatId;

	if not ExistingRowCount = pRowCount Or not ExistingColCount = pColCount then
		
		-- delete from inv_grid_position
		delete from inv_grid_position 
		where
		grid_format_id_fk in (select grid_format_id_fk from inv_physical_plate where phys_plate_id = pPhysPlateId);
		
		-- insert new rows into inv_grid_element

		for row_counter in 1..pRowCount loop
			for col_counter in 1..pColCount loop
				-- add new record
				new_sort_order := col_counter + ((row_counter-1) * pColCount);
				insert into inv_grid_position
				(grid_format_id_fk, row_index, col_index, row_name, col_name, sort_order, name)
				values
				(pGridFormatId, row_counter, col_counter, row_counter, col_counter, new_sort_order, new_sort_order);
			end loop;
		end loop;

	end if;
	

	-- update grid format
	update inv_grid_format set
		row_count = pRowCount,
		col_count = pColCount,
		row_prefix = pRowPrefix,
		col_prefix = pColPrefix,
		row_use_letters = pRowUseLetters,
		col_use_letters = pColUseLetters,
		name_separator = pNameSeparator,
		number_start_corner = pNumberStartCorner,
		number_direction = pNumberDirection
	where
		grid_format_id = pGridFormatId;

	-- update physical plate
	update inv_physical_plate set
		phys_plate_name = pPhysPlateName,
		supplier_id_fk = pSupplierIDFK,
		is_pre_barcoded = pIsPreBarcoded,
		well_capacity = pWellCapacity,
		capacity_unit_id_fk = pCapacityUnitID
	where
		phys_plate_id = pPhysPlateId;

	commit;

RETURN pPhysPlateId;

END UpdatePhysPlateType;
/
show errors;