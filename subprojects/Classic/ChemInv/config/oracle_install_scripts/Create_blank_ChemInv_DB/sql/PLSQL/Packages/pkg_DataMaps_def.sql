CREATE OR REPLACE PACKAGE "DATAMAPS"
AS
	TYPE  CURSOR_TYPE IS REF CURSOR;

	FUNCTION CreateDataMap (
  	pName inv_data_maps.data_map_name%TYPE,
    pTypeID inv_data_maps.data_map_type_id_fk%TYPE,
    pComments inv_data_maps.data_map_comments%TYPE,
    pNumHeaderRows inv_data_maps.num_header_rows%TYPE,
    pNumColumns inv_data_maps.num_columns%TYPE,
    pColumnDelimiter inv_data_maps.column_delimiter%TYPE,
    pUseWellCoordinates inv_data_maps.use_well_coordinates%TYPE,    
    pDataMapFieldList VARCHAR2,
    pDataMapColumnList VARCHAR2
  ) RETURN inv_data_maps.data_map_id%TYPE;

  FUNCTION EditDataMap (
		pDataMapID inv_data_maps.data_map_id%TYPE,
  	pName inv_data_maps.data_map_name%TYPE,
    pTypeID inv_data_maps.data_map_type_id_fk%TYPE,
    pComments inv_data_maps.data_map_comments%TYPE,
    pNumHeaderRows inv_data_maps.num_header_rows%TYPE,
    pNumColumns inv_data_maps.num_columns%TYPE,
    pColumnDelimiter inv_data_maps.column_delimiter%TYPE,
    pUseWellCoordinates inv_data_maps.use_well_coordinates%TYPE,    
    pDataMapFieldList VARCHAR2,
    pDataMapColumnList VARCHAR2
  ) RETURN inv_data_maps.data_map_id%TYPE;

  FUNCTION CopyDataMap (
  	pDataMapID inv_data_maps.data_map_id%TYPE,
  	pName inv_data_maps.data_map_name%TYPE
  ) RETURN inv_data_maps.data_map_id%TYPE;

  FUNCTION DeleteDataMap (
  	pDataMapID inv_data_maps.data_map_id%TYPE
  ) RETURN inv_data_maps.data_map_id%TYPE;

	PROCEDURE MapToDefault(
		pDataMapID IN inv_data_maps.data_map_id%TYPE,
		O_RS OUT CURSOR_TYPE) ;

	PROCEDURE GetDataMap(
		pDataMapID IN inv_data_maps.data_map_id%TYPE,
		O_RS OUT CURSOR_TYPE);  
    
  PROCEDURE GetDataMappings(
  	pDataMapID IN inv_data_maps.data_map_id%TYPE,
    O_RS OUT CURSOR_TYPE);
    
  PROCEDURE GetDataMappingsByTable(
  	pDataMapID IN inv_data_maps.data_map_id%TYPE,
    pTableName IN inv_map_fields.table_name%TYPE,
    O_RS OUT CURSOR_TYPE);
    
  --' Purpose: returns maps that are valid for the import from text file function
  PROCEDURE GetValidDataMaps(
  	p_mapType VARCHAR2,
    O_RS OUT CURSOR_TYPE);
  
    
END "DATAMAPS";
/
show errors;