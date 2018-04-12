CREATE OR REPLACE PACKAGE BODY "DATAMAPS"
AS
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
  ) RETURN inv_data_maps.data_map_id%TYPE
  IS
  	lDataMapID inv_data_maps.data_map_id%TYPE;
	 	lDataMapField_t STRINGUTILS.t_char;
    lDataMapColumn_t STRINGUTILS.t_char;
  BEGIN
  	-- create the data map
 		INSERT INTO inv_data_maps VALUES (NULL, pName, pTypeID, pComments, pNumHeaderRows, pNumColumns, pColumnDelimiter, pUseWellCoordinates) RETURNING data_map_id INTO lDataMapID;

		-- create the data mappings
    lDataMapField_t := STRINGUTILS.split(pDataMapFieldList, ',');
    lDataMapColumn_t := STRINGUTILS.split(pDataMapColumnList, ',');
    IF lDataMapField_t.LAST = lDataMapColumn_t.LAST THEN
      FORALL i IN lDataMapField_t.First..lDataMapField_t.Last
  			INSERT INTO inv_data_mappings VALUES (lDataMapID, lDataMapField_t(i),lDataMapColumn_t(i));
		END IF;

  	RETURN lDataMapID;
  END CreateDataMap;

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
  ) RETURN inv_data_maps.data_map_id%TYPE
  IS
	 	lDataMapField_t STRINGUTILS.t_char;
    lDataMapColumn_t STRINGUTILS.t_char;
  BEGIN
  	-- update data map
		UPDATE inv_data_maps SET
    	data_map_name = pName,
      data_map_type_id_fk = pTypeID,
      data_map_comments = pComments,
      num_header_rows = pNumHeaderRows,
      num_columns = pNumColumns,
      column_delimiter = pColumnDelimiter,
      use_well_coordinates = pUseWellCoordinates
  	WHERE data_map_id = pDataMapID;

    -- delete data mappings
    DELETE inv_data_mappings WHERE data_map_id_fk = pDataMapID;

    -- create data mappings
    lDataMapField_t := STRINGUTILS.split(pDataMapFieldList, ',');
    lDataMapColumn_t := STRINGUTILS.split(pDataMapColumnList, ',');
    IF lDataMapField_t.LAST = lDataMapColumn_t.LAST THEN
      FORALL i IN lDataMapField_t.First..lDataMapField_t.Last
  			INSERT INTO inv_data_mappings VALUES (pDataMapID, lDataMapField_t(i),lDataMapColumn_t(i));
		END IF;

		RETURN pDataMapID;

  END EditDataMap;

  FUNCTION CopyDataMap (
  	pDataMapID inv_data_maps.data_map_id%TYPE,
  	pName inv_data_maps.data_map_name%TYPE
  ) RETURN inv_data_maps.data_map_id%TYPE
  IS
  	lDataMapID inv_data_maps.data_map_id%TYPE;
  	lName inv_data_maps.data_map_name%TYPE;
    lTypeID inv_data_maps.data_map_type_id_fk%TYPE;
    lComments inv_data_maps.data_map_comments%TYPE;
    lNumHeaderRows inv_data_maps.num_header_rows%TYPE;
    lNumColumns inv_data_maps.num_columns%TYPE;
    lColumnDelimiter inv_data_maps.column_delimiter%TYPE;
    lUseWellCoordinates inv_data_maps.use_well_coordinates%TYPE;
  BEGIN
  	-- copy data map
		SELECT
    	pName, data_map_type_id_fk, data_map_comments, num_header_rows, num_columns, column_delimiter, use_well_coordinates
      INTO lName, lTypeID, lComments, lNumHeaderRows, lNumColumns, lColumnDelimiter, lUseWellCoordinates
    FROM inv_data_maps WHERE data_map_id = pDataMapID ;

		INSERT INTO inv_data_maps VALUES (NULL, lName, lTypeID, lComments, lNumHeaderRows, lNumColumns, lColumnDelimiter, lUseWellCoordinates) RETURNING data_map_id INTO lDataMapID;

    -- copy data mappings
    FOR mapping_rec IN (SELECT map_field_id_fk, column_number FROM inv_data_mappings WHERE data_map_id_fk = pDataMapID)
    LOOP
    	INSERT INTO inv_data_mappings VALUES(lDataMapID, mapping_rec.map_field_id_fk, mapping_rec.column_number);
    END LOOP;

    RETURN lDataMapID;

  END CopyDataMap;

  FUNCTION DeleteDataMap (
  	pDataMapID inv_data_maps.data_map_id%TYPE
  ) RETURN inv_data_maps.data_map_id%TYPE
  IS
  BEGIN
 		-- delete data mappings
    DELETE inv_data_mappings WHERE data_map_id_fk = pDataMapID;

    -- delete data map
    DELETE inv_data_maps WHERE data_map_id = pDataMapID;

    RETURN pDataMapID;

  END DeleteDataMap;

  
	PROCEDURE MapToDefault(
		pDataMapID IN inv_data_maps.data_map_id%TYPE,
		O_RS OUT CURSOR_TYPE) 
  AS

	BEGIN
		OPEN O_RS FOR
			SELECT dm1.data_map_id_fk AS default_data_map_id, dm2.data_map_id_fk AS data_map_id, dm1.column_number default_col_num, dm2.column_number AS col_num 
			FROM inv_data_mappings dm1, inv_data_mappings dm2
			WHERE 
				dm1.map_field_id_fk = dm2.map_field_id_fk
			  AND dm2.data_map_id_fk = pDataMapID
				AND dm1.data_map_id_fk = 1;

/*    
      SELECT dm1.data_map_id_fk, dm2.data_map_id_fk, dm1.column_number, dm2.column_number 
      FROM (SELECT * FROM inv_data_mappings WHERE data_map_id_fk = 1) dm1, (SELECT * FROM inv_data_mappings WHERE data_map_id_fk = pDataMapID) dm2
      WHERE 
      	dm1.map_field_id_fk = dm2.map_field_id_fk (+);
*/    
	END MapToDefault;
  
	PROCEDURE GetDataMap(
		pDataMapID IN inv_data_maps.data_map_id%TYPE,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			SELECT	* 
      FROM inv_data_maps
      WHERE	data_map_id = pDataMapID;
	END GetDataMap;
  
  PROCEDURE GetDataMappings(
  	pDataMapID IN inv_data_maps.data_map_id%TYPE,
    O_RS OUT CURSOR_TYPE)  AS
	BEGIN    
  	OPEN O_RS FOR
    	SELECT map_field_id, column_number, display_name, table_name, column_name  
      FROM inv_data_mappings, inv_map_fields 
      WHERE map_field_id = map_field_id_Fk
      AND data_map_id_fk = pDataMapID;
  END GetDataMappings;
  PROCEDURE GetDataMappingsByTable(
  	pDataMapID IN inv_data_maps.data_map_id%TYPE,
    pTableName IN inv_map_fields.table_name%TYPE,
    O_RS OUT CURSOR_TYPE)  AS
	BEGIN    
  	IF pTableName IS NULL THEN
    	OPEN O_RS FOR
      	SELECT map_field_id, column_number, display_name, table_name, column_name  
        FROM inv_data_mappings, inv_map_fields 
        WHERE map_field_id = map_field_id_Fk
        AND table_name IS NULL
        AND data_map_id_fk = pDataMapID;
  	ELSE
    	OPEN O_RS FOR
      	SELECT map_field_id, column_number, display_name, table_name, column_name  
        FROM inv_data_mappings, inv_map_fields 
        WHERE map_field_id = map_field_id_Fk
        AND upper(table_name) = upper(pTableName)
        AND data_map_id_fk = pDataMapID;
  	END IF;
  END GetDataMappingsByTable;

  PROCEDURE GetValidDataMaps(
  	p_mapType VARCHAR2,
    O_RS OUT CURSOR_TYPE) AS
  BEGIN
  	IF p_mapType = 'source1' THEN
    	--' all maps that have only source well mapped
    	OPEN O_RS FOR
      	SELECT data_map_id 
        	FROM inv_data_maps, inv_data_mappings 
          WHERE 
          	data_map_id = data_map_id_fk 
            AND map_field_id_fk = 2
            AND data_map_id NOT IN (SELECT data_map_id         
            	FROM inv_data_maps, inv_data_mappings 
		          WHERE 
    		      	data_map_id = data_map_id_fk 
        		    AND map_field_id_fk = 1);
    ELSIF p_mapType = 'source2' THEN
    	--' all maps that have source well and source plate mapped
    	OPEN O_RS FOR
      	SELECT DISTINCT data_map_id 
        	FROM inv_data_maps, inv_data_mappings 
          WHERE 
          	data_map_id = data_map_id_fk 
            AND map_field_id_fk IN (1,2);    
    ELSIF p_mapType = 'target' THEN
    	--' all maps that have target plate barcode, target well, source plate barcode, source well mapped
    	OPEN O_RS FOR
      	SELECT DISTINCT data_map_id 
        	FROM inv_data_maps, inv_data_mappings 
          WHERE 
          	data_map_id = data_map_id_fk 
            AND map_field_id_fk IN (3,4);
    
    END IF;
  END GetValidDataMaps;
  
END "DATAMAPS";
/
show errors;