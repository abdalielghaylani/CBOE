CREATE OR REPLACE PACKAGE BODY PLATESETTINGS
    IS
	FUNCTION CREATEGRIDFORMAT
		(p_grid_format_type in inv_enumeration.enum_id%type,
		 p_row_count in inv_grid_format.row_count%type,
		 p_col_count in inv_grid_format.col_count%type,
		 p_Row_Prefix in inv_grid_format.row_prefix%type,
		 p_Col_Prefix in inv_grid_format.col_prefix%type,
		 p_Row_Use_Letters in inv_grid_format.row_use_letters%type,
		 p_Col_Use_Letters in inv_grid_format.col_use_letters%type,
		 p_Name_Separator in inv_grid_format.name_separator%type,
		 p_Number_Start_Corner in inv_grid_format.number_start_corner%type,
		 p_Number_Direction in inv_grid_format.number_direction%type,
		 p_name in inv_grid_format.name%type:=Null,
		 p_description in inv_grid_format.description%type:=Null,
     p_Zero_Padding_Count in inv_grid_format.zero_padding_count%TYPE,
		 pGridFormatID in inv_grid_format.grid_format_id%type:=Null) return inv_grid_format.grid_format_id%type
	IS
		new_grid_format_id inv_grid_format.grid_format_id%type;
		row_counter inv_grid_position.row_index%Type;
		col_counter inv_grid_position.col_index%Type;
		new_sort_order inv_grid_position.sort_order%type;
		new_name inv_grid_position.name%type;
		new_row_name inv_grid_position.row_name%type;
		new_col_name inv_grid_position.col_name%type;
		colLetterNum number:=1;
		rowLetterNum number:=1;
		colLetterCount number:=1;
		rowLetterCount number:=1;
		colLoopCount number:=1;
		rowLoopCount number:=1;
		colLetter inv_grid_position.col_name%type:='';
		rowLetter inv_grid_position.row_name%type:='';
	begin
		-- insert into grid format
		insert into inv_grid_format
			(grid_format_id, name, description, row_count, col_count,
			 row_prefix, col_prefix, row_use_letters,
			 col_use_letters, name_separator, number_start_corner,
			 number_direction, grid_format_type_fk, zero_padding_count)
		values
			(pGridFormatID, p_name, p_description, p_row_count, p_col_count,
			p_row_prefix, p_col_prefix, p_row_use_letters,
			 p_col_use_letters, p_name_separator, p_number_start_corner,
			 p_number_direction, p_grid_format_type, p_zero_padding_count) returning grid_format_id into new_grid_format_id;

		-- insert new rows into inv_grid_position
		-- not supported - starting at any other corner
		for row_counter in 1..p_row_count loop
			--determine row name
				if p_row_use_letters = 1 then
					rowLetter := '';
					for rowLoopCount in 1..rowLetterCount loop
						rowLetter := rowLetter || chr(65 + rowLetterNum - 1);
					end loop;
					new_row_name := p_row_prefix || rowLetter;
					if rowLetterNum > 25 then
						rowLetterNum := 1;
						rowLetterCount := rowLetterCount + 1;
					else
						rowLetterNum := rowLetterNum + 1;
					end if;
				else
					new_row_name := p_row_prefix || lpad(row_counter,p_zero_padding_count,'0');
				end if;
			--reset the col letter
			colLetterNum := 1;
			colLetterCount := 1;
			for col_counter in 1..p_col_count loop
				-- add new record
				-- determine sort order
				if p_number_direction = 17 then
					-- if rows first
					new_sort_order := col_counter + ((row_counter-1) * p_col_count);
				else
					-- if columns first
					new_sort_order := row_counter + ((col_counter-1) * p_row_count);
				end if;
				-- determine column names
				if p_col_use_letters = 1 then
					colLetter := '';
					for colLoopCount in 1..colLetterCount loop
						colLetter := colLetter || chr(65 + colLetterNum - 1);
					end loop;
					new_col_name := p_col_prefix || colLetter;
					--insert into inv_debug values (colLetter||':'||col_counter,colLetterNum,null);
					--new_col_name := 'a';
					if colLetterNum > 25 then
						colLetterNum := 1;
						colLetterCount := colLetterCount + 1;
					else
						colLetterNum := colLetterNum + 1;
					end if;
				else
					new_col_name := p_col_prefix || lpad(col_counter, p_zero_padding_count, '0');
				end if;
				-- determine cell name
				if p_number_direction = 17 then
					new_name := new_row_name || p_name_separator || new_col_name;
				else
					new_name := new_col_name || p_name_separator || new_row_name;
				end if;
				-- add new record
				insert into inv_grid_position
				(grid_format_id_fk, row_index, col_index, row_name, col_name, sort_order, name)
				values
				(new_grid_format_id, row_counter, col_counter, new_row_name, new_col_name, new_sort_order, new_name);
			end loop;
		end loop;
		return new_grid_format_id;
	end CREATEGRIDFORMAT;

	FUNCTION CREATEPHYSPLATETYPE
		(
		 pPhysPlateName in inv_physical_plate.phys_plate_name%Type,
		 pRowCount in inv_grid_format.row_count%Type,
		 pColCount in inv_grid_format.col_count%Type,
		 pRowPrefix in inv_grid_format.row_prefix%type,
		 pColPrefix in inv_grid_format.col_prefix%type,
		 pRowUseLetters in inv_grid_format.row_use_letters%type,
		 pColUseLetters in inv_grid_format.col_use_letters%type,
		 pNameSeparator in inv_grid_format.name_separator%type,
		 pNumberStartCorner in inv_grid_format.number_start_corner%type,
		 pNumberDirection in inv_grid_format.number_direction%type,
     pZeroPaddingCount in inv_grid_format.zero_padding_count%TYPE,
		 pSupplierIdFK in inv_physical_plate.supplier_id_fk%Type,
		 pIsPreBarcoded in inv_physical_plate.is_pre_barcoded%Type,
		 pWellCapacity in inv_physical_plate.well_capacity%Type,
		 pCapacityUnitId in inv_physical_plate.capacity_unit_id_fk%type,
		 pPhysPlateID in inv_physical_plate.phys_plate_id%Type:=Null) return inv_physical_plate.phys_plate_id%Type
	is
		NewPhysicalPlateId inv_physical_plate.phys_plate_id%Type;
		NewGridFormatId inv_grid_format.grid_format_id%Type;
		row_counter inv_grid_position.row_index%Type;
		col_counter inv_grid_position.col_index%Type;
	begin
		-- create new grid format
		-- 9 is the constant in inV_ENUMERATION for a Plate grid type
		NewGridFormatId := CreateGridFormat(9,
											pRowCount,
											pColCount,
											pRowPrefix,
											pColPrefix,
											pRowUseLetters,
											pColUseLetters,
											pNameSeparator,
											pNumberStartCorner,
											pNumberDirection,
											pPhysPlateName,
											pPhysPlateName,
                      pZeroPaddingCount);

		-- insert into phys plate
		insert into inv_physical_plate
					(phys_plate_id, phys_plate_name, supplier_id_fk, is_pre_barcoded, grid_format_id_fk, well_capacity, capacity_unit_id_fk)
		values		(pPhysPlateID, pPhysPlateName, pSupplierIDFk, pIsPreBarCoded, NewGridFormatId, pWellCapacity, pCapacityUnitID) returning phys_plate_id into NewPhysicalPlateId;

		return NewPhysicalPlateId;
	end CREATEPHYSPLATETYPE;

FUNCTION CopyPlateFormat
(pPlateFormatName IN inv_plate_format.plate_format_name%TYPE,
 pPlateFormatID in inv_plate_format.plate_format_id%type)
	RETURN inv_plate_format.plate_format_id%type
IS
l_physicalPlateId inv_plate_format.phys_plate_id_fk%TYPE;
l_newPlateFormatId inv_plate_format.plate_format_id%TYPE;
BEGIN

--'  get physical plate format
SELECT phys_plate_id_fk INTO l_physicalPlateId FROM inv_plate_format WHERE plate_format_id = pPlateFormatID;

--' create new plate format
l_newPlateFormatId := createplateformat(pPlateFormatName, l_physicalPlateId, NULL);

--' update well formats and concentrations to be the same as the source plate format
UPDATE inv_wells w1 SET (well_format_id_fk, concentration) = (SELECT well_format_id_fk, concentration FROM inv_wells  w2 WHERE plate_format_id_fk = pPlateFormatID AND w2.grid_position_id_fk = w1.grid_position_id_fk AND plate_id_fk IS NULL) 
							WHERE plate_format_id_fk = l_newPlateFormatId AND plate_id_fk is NULL;

RETURN l_newPlateFormatId;

END;


	FUNCTION CREATEPLATEFORMAT
		(pPlateFormatName in inv_plate_format.plate_format_name%type,
		 pPhysPlateIdFk in inv_plate_format.phys_plate_id_fk%type,
		 pPlateFormatID in inv_plate_format.plate_format_id%type:=Null) return inv_plate_format.plate_format_id%type
	is
		NewPlateFormatId inv_plate_format.plate_format_id%type;
		GridFormatId inv_grid_format.grid_format_id%type;
    l_XmlDocID inv_xmldocs.xmldoc_id%TYPE;
		cursor grid_positions is
			select * from inv_grid_position where grid_format_id_fk = GridFormatId;
	begin
		-- insert new values into inv_plate_format
		insert into inv_plate_format
					(plate_format_name, phys_plate_id_fk)
		values		(pPlateFormatName, pPhysPlateIdFk) returning plate_format_id into NewPlateFormatId;

		-- create as many new wells as necessary
		-- get grid format id
		select grid_format_id_fk into GridFormatId
		from inv_physical_plate
		where phys_plate_id = pPhysPlateIdFK;

		-- insert into inv_wells
		for grid_positions_cur in grid_positions loop
			insert into inv_wells
						(plate_format_id_fk, grid_position_id_fk, well_format_id_fk)
			values		(NewPlateFormatID, grid_positions_cur.grid_position_id, 1);
		end loop;
		--create the daughter reformat map
    l_XmlDocID := Reformat.CreateDaughteringMap(NewPlateFormatId);
		return NewPlateFormatId;
	end CREATEPLATEFORMAT;

	FUNCTION CREATEPLATETYPE
		(pPlateTypeName in inv_plate_types.plate_type_name%type,
		 pMaxFreezeThaw in inv_plate_types.max_freeze_thaw%type) return inv_plate_types.plate_type_id%type
	is
		newplatetypeid inv_plate_types.plate_type_id%type;
	begin
		insert  into inv_plate_types
				(plate_type_name, max_freeze_thaw)
		values  (pPlateTypeName,  pMaxFreezeThaw) returning plate_type_id into newplatetypeid;

		return newplatetypeid;
	end CREATEPLATETYPE;

	FUNCTION CREATEWELLCONTENTTYPE
		(pWellFormatName in inv_enumeration.enum_value%type) return inv_enumeration.enum_value%type
	is
		newWellFormatID inv_enumeration.enum_value%type;
	begin
		--  ESET_ID for well formats is 1
		insert into inv_Enumeration
				(enum_value, eset_id_fk)
		values  (pWellFormatName, 1) returning enum_id into newWellFormatID;

		return newWellFormatID;
	end CREATEWELLCONTENTTYPE;

	FUNCTION DELETEGRIDFORMAT
		(p_grid_format_id in inv_grid_format.grid_format_id%type) return integer
	is
		children_found exception;
		pragma exception_init (children_found, -2292);
		numCellsDeleted int;
		rc integer;
	begin
		delete from inv_grid_format where grid_format_id = p_grid_format_id;

		return p_grid_format_id;
	exception
		when children_found then
		return -125;
	end DELETEGRIDFORMAT;

	FUNCTION DELETEPHYSPLATETYPE
		(pPhysPlateId in inv_physical_plate.phys_plate_id%Type) return inv_physical_plate.phys_plate_id%Type
	is
		children_found exception;
		pragma exception_init (children_found, -2292);
		CountExistingFormatID integer;
	begin
		delete from inv_physical_plate where phys_plate_id = pPhysPlateId;
		return pPhysPlateId;
	exception
		when children_found then
		return -124;
	end DELETEPHYSPLATETYPE;

	FUNCTION DELETEPLATEFORMAT
		(pPlateFormatId in inv_plate_format.plate_format_id%type) return inv_plate_format.plate_format_id%type
	is
		children_found exception;
		pragma exception_init (children_found, -2292);
		rc integer;
	begin
		delete from inv_wells where plate_format_id_fk = pPlateFormatID and plate_id_fk is null;
		delete from inv_plate_format where plate_format_id = pPlateFormatId;
		delete from inv_xmldocs where xmldoc_type_id_fk = 1 and name = 'Daughter' || pPlateFormatID  ;
		return pPlateFormatId;
	exception
		when children_found then
		rollback;
		return -126;
	end DELETEPLATEFORMAT;

	FUNCTION DELETEPLATETYPE
		(pPlateTypeID in inv_plate_types.plate_type_id%Type) return inv_plate_types.plate_type_id%type
	is
	begin
		delete from inv_plate_types where plate_type_id = pPlateTypeID;
		return pPlatetypeID;
	end DELETEPLATETYPE;

	FUNCTION DELETEWELLCONTENTTYPE
		(pWellFormatId in inv_enumeration.enum_id%type) return inv_enumeration.enum_value%type
	is
		children_exist exception;
		pragma exception_init (children_exist, -2292);
	begin
		--  ESET_ID for well formats is 1
		delete from inv_Enumeration where enum_id = pWellFormatID;

		return pWellFormatID;
	exception
		when children_exist then
		  return -127;
	end DELETEWELLCONTENTTYPE;

	FUNCTION UPDATEGRIDFORMAT
		(p_grid_format_type in inv_enumeration.enum_id%type,
		 p_grid_format_id in inv_grid_format.grid_format_id%type,
		 p_row_count in inv_grid_format.row_count%type,
		 p_col_count in inv_grid_format.col_count%type,
		 p_Row_Prefix in inv_grid_format.row_prefix%type,
		 p_Col_Prefix in inv_grid_format.col_prefix%type,
		 p_Row_Use_Letters in inv_grid_format.row_use_letters%type,
		 p_Col_Use_Letters in inv_grid_format.col_use_letters%type,
		 p_Name_Separator in inv_grid_format.name_separator%type,
		 p_Number_Start_Corner in inv_grid_format.number_start_corner%type,
		 p_Number_Direction in inv_grid_format.number_direction%type,
		 p_name in inv_grid_format.name%type:=Null,
		 p_description in inv_grid_format.description%type:=Null,
     p_Zero_Padding_Count in inv_grid_format.zero_padding_count%TYPE
     ) return inv_grid_format.grid_format_id%type
	is
		new_grid_format_id inv_grid_format.grid_format_id%type;
		rc integer;
	begin
	  rc := DeleteGridFormat(p_grid_format_id);
	  if rc < 0 then
	    return -125;
	  end if;
	  new_grid_format_id := CreateGridFormat(p_grid_format_type,
		                                       p_row_count,
		                                       p_col_count,
		                                       p_Row_Prefix,
		                                       p_Col_Prefix,
		                                       p_Row_Use_Letters,
		                                       p_Col_Use_Letters,
		                                       p_Name_Separator,
		                                       p_Number_Start_Corner,
		                                       p_Number_Direction,
		                                       p_name,
		                                       p_description,
                                           p_Zero_Padding_Count,
		                                       p_grid_format_id);
		return new_grid_format_id;
	end UPDATEGRIDFORMAT;

	FUNCTION UPDATEPHYSPLATETYPE
		(pPhysPlateId in inv_physical_plate.phys_plate_id%Type,
		 pGridFormatId in inv_physical_plate.grid_format_id_fk%Type,
		 pPhysPlateName in inv_physical_plate.phys_plate_name%Type,
		 pRowCount in inv_grid_format.row_count%Type,
		 pColCount in inv_grid_format.col_count%Type,
		 pRowPrefix in inv_grid_format.row_prefix%type,
		 pColPrefix in inv_grid_format.col_prefix%type,
		 pRowUseLetters in inv_grid_format.row_use_letters%type,
		 pColUseLetters in inv_grid_format.col_use_letters%type,
		 pNameSeparator in inv_grid_format.name_separator%type,
		 pNumberStartCorner in inv_grid_format.number_start_corner%type,
		 pNumberDirection in inv_grid_format.number_direction%type,
		 pZeroPaddingCount in inv_grid_format.zero_padding_count%TYPE,
		 pSupplierIdFK in inv_physical_plate.supplier_id_fk%Type,
		 pIsPreBarcoded in inv_physical_plate.is_pre_barcoded%Type,
		 pWellCapacity in inv_physical_plate.well_capacity%type,
		 pCapacityUnitID in inv_physical_plate.capacity_unit_id_fk%type) return inv_physical_plate.phys_plate_id%Type
	is
		rc integer;
	begin
	    rc := DeletePhysPlateType(pPhysPlateID);
	    if rc < 0 then
	      return rc;
	    end if;

	    rc := CreatePhysPlateType(pPhysPlateName,
	                          pRowCount,
	                          pColCount,
	                          pRowPrefix,
	                          pColPrefix,
	                          pRowUseLetters,
	                          pColUseLetters,
	                          pNameSeparator,
	                          pNumberStartCorner,
	                          pNumberDirection,
                            pZeroPaddingCount,
	                          pSupplierIdFK,
	                          pIsPreBarcoded,
	                          pWellCapacity,
	                          pCapacityUnitId,
	                          pPhysPlateID);
		return pPhysPlateID;
	end UPDATEPHYSPLATETYPE;

	FUNCTION UPDATEPLATEFORMAT
		(pPlateFormatId in inv_Plate_format.plate_format_id%type,
		 pPlateFormatName in inv_plate_format.plate_format_name%type,
		 pPhysPlateIdFk in inv_plate_format.phys_plate_id_fk%type) return integer
	is
		GridFormatId inv_grid_format.grid_format_id%type;
		rc integer;
		OldPhysPlateId inv_plate_format.phys_plate_id_fk%type;
	begin
		select phys_plate_id_fk into OldPhysPlateID from inv_plate_format where plate_format_id = pPlateFormatId;

		if not OldPhysPlateId = pPhysPlateIDFK then
			rc := DeletePlateFormat(pPlateFormatID);
			if rc < 0 then
				return -126;
			end if;
			rc := CreatePlateFormat(pPlateFormatName,
									pPhysPlateIDFK,
									pPlateFormatID);
		else
			update inv_plate_format set plate_format_name = pPlateFormatName where plate_format_id = pPlateFormatID;
			rc := pPlateFormatID;
		end if;
		return rc;
	end UPDATEPLATEFORMAT;

	FUNCTION UPDATEPLATETYPE
		(pPlateTypeID in inv_plate_types.plate_type_id%Type,
		 pPlateTypeName in inv_plate_types.plate_type_name%Type,
		 pMaxFreezeThaw in inv_plate_types.max_freeze_thaw%Type) return inv_plate_types.plate_type_id%type
	is
	begin
		update  inv_plate_types
		set		plate_type_name = pPlateTypeName,
				max_freeze_thaw = pMaxFreezeThaw
		where	plate_type_id = pPlateTypeID;

		return pPlatetypeID;
	end UPDATEPLATETYPE;

	FUNCTION UPDATEWELLCONTENTTYPE
		(pWellFormatId in inv_enumeration.enum_id%type,
		 pWellFormatName in inv_enumeration.enum_value%type) return inv_enumeration.enum_value%type
	is
	begin
		update	inv_Enumeration
		set		enum_value =  pWellFormatName
		where	enum_id = pWellFormatID;

		return pWellFormatID;

	end UPDATEWELLCONTENTTYPE;

	FUNCTION UPDATEWELLFORMAT
		(pWellID in inv_Wells.Well_id%type,
		 pWellFormatIdFK in inv_wells.well_format_id_fk%type) return inv_wells.well_format_id_fk%type
	is
	begin
		update	inv_wells
		set		well_format_id_fk = pWellFormatIDFK
		where	well_id = pWellID;

		return pWellId;
	end UPDATEWELLFORMAT;
end PLATESETTINGS;
/
show errors;
