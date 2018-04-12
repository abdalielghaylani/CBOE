CREATE OR REPLACE PACKAGE PLATESETTINGS
    is
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
		 pGridFormatID in inv_grid_format.grid_format_id%type:=Null) return inv_grid_format.grid_format_id%type;

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
		 pPhysPlateID in inv_physical_plate.phys_plate_id%Type:=Null) return inv_physical_plate.phys_plate_id%Type;

FUNCTION CopyPlateFormat
(pPlateFormatName IN inv_plate_format.plate_format_name%TYPE,
 pPlateFormatID in inv_plate_format.plate_format_id%type)
	RETURN inv_plate_format.plate_format_id%TYPE;


	FUNCTION CREATEPLATEFORMAT
		(pPlateFormatName in inv_plate_format.plate_format_name%type,
		 pPhysPlateIdFk in inv_plate_format.phys_plate_id_fk%type,
		 pPlateFormatID in inv_plate_format.plate_format_id%type:=Null) return inv_plate_format.plate_format_id%type;

	FUNCTION CREATEPLATETYPE
		(pPlateTypeName in inv_plate_types.plate_type_name%type,
		 pMaxFreezeThaw in inv_plate_types.max_freeze_thaw%type) return inv_plate_types.plate_type_id%type;

	FUNCTION CREATEWELLCONTENTTYPE
		(pWellFormatName in inv_enumeration.enum_value%type) return inv_enumeration.enum_value%type;

	FUNCTION DELETEGRIDFORMAT
		(p_grid_format_id in inv_grid_format.grid_format_id%type) return integer;

	FUNCTION DELETEPHYSPLATETYPE
		(pPhysPlateId in inv_physical_plate.phys_plate_id%Type) return inv_physical_plate.phys_plate_id%Type;

	FUNCTION DELETEPLATEFORMAT
		(pPlateFormatId in inv_plate_format.plate_format_id%type) return inv_plate_format.plate_format_id%type;

	FUNCTION DELETEPLATETYPE
		(pPlateTypeID in inv_plate_types.plate_type_id%Type) return inv_plate_types.plate_type_id%type;

	FUNCTION DELETEWELLCONTENTTYPE
		(pWellFormatId in inv_enumeration.enum_id%type) return inv_enumeration.enum_value%type;

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
     ) return inv_grid_format.grid_format_id%TYPE;

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
		 pCapacityUnitID in inv_physical_plate.capacity_unit_id_fk%type) return inv_physical_plate.phys_plate_id%Type;

	FUNCTION UPDATEPLATEFORMAT
		(pPlateFormatId in inv_Plate_format.plate_format_id%type,
		 pPlateFormatName in inv_plate_format.plate_format_name%type,
		 pPhysPlateIdFk in inv_plate_format.phys_plate_id_fk%type) return integer;

	FUNCTION UPDATEPLATETYPE
		(pPlateTypeID in inv_plate_types.plate_type_id%Type,
		 pPlateTypeName in inv_plate_types.plate_type_name%Type,
		 pMaxFreezeThaw in inv_plate_types.max_freeze_thaw%Type) return inv_plate_types.plate_type_id%type;

	FUNCTION UPDATEWELLCONTENTTYPE
		(pWellFormatId in inv_enumeration.enum_id%type,
		 pWellFormatName in inv_enumeration.enum_value%type) return inv_enumeration.enum_value%type;

	FUNCTION UPDATEWELLFORMAT
		(pWellID in inv_Wells.Well_id%type,
		 pWellFormatIdFK in inv_wells.well_format_id_fk%type) return inv_wells.well_format_id_fk%type;
end PLATESETTINGS;
/
show errors;
