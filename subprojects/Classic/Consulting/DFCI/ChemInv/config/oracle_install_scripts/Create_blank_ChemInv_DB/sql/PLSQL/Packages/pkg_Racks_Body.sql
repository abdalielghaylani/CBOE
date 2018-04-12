CREATE OR REPLACE PACKAGE BODY RACKS IS

	PROCEDURE DISPLAYRACKGRID(p_LocationID inv_locations.location_id%TYPE,
						 p_regServer VARCHAR2,
						 O_RS OUT CURSOR_TYPE) AS
	l_locationId inv_locations.location_id%TYPE;
  l_count INT;
	BEGIN
		IF isRackLocation(p_LocationID) = 1 THEN
    	SELECT parent_id INTO l_locationId FROM inv_locations WHERE location_id = p_LocationID;
    ELSE
    	l_locationId := p_LocationID;
    END IF;

		IF p_regServer = 'NULL' THEN
			OPEN O_RS FOR
				SELECT (SELECT 'Container::' || container_id || '::' ||
							barcode ||
							'::/cheminv/images/listview/flask_closed_icon_16.gif::' ||
							container_name || '::' ||
							DECODE(inv_Containers.Qty_Max,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Max || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Qty_Remaining,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Remaining || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Qty_Available,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Available || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Concentration,
								  NULL,
								  ' ',
								  inv_Containers.Concentration || ' ' ||
								  UOC.Unit_Abreviation) || '::' ||
							icb.batch_field_1 || '::' ||
							constants.cContainerBatchField1Display || '::' ||
							icb.batch_field_2 || '::' ||
							constants.cContainerBatchField2Display || '::' ||
							icb.batch_field_3 || '::' ||
							constants.cContainerBatchField3Display || '::' ||
							inv_Containers.Qty_Max || '::' ||
							inv_Containers.Qty_Remaining || '::' ||
							inv_Containers.Qty_Available || '::' ||
							inv_Containers.Concentration
					   FROM inv_containers,
						   inv_Units UOM,
						   inv_Units UOC,
						   inv_container_batches icb
					   WHERE location_id_fk = l.location_id
						    AND inv_Containers.UNIT_OF_MEAS_ID_FK =
						    UOM.Unit_ID
						    AND inv_Containers.Unit_of_Conc_ID_FK =
						    UOC.Unit_ID(+)
						    AND
						    inv_containers.batch_id_fk = icb.batch_id(+)
						    AND rownum < 2
					   UNION
					   SELECT 'Plate::' || plate_id || '::' ||
							plate_barcode ||
							'::/cheminv/images/listview/Plate_icon_16.gif::' ||
							plate_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_plates
					   WHERE location_id_fk = l.location_id
						    AND rownum < 2
					   UNION
					   SELECT 'Rack::' || location_id || '::' ||
							location_barcode ||
							'::/cheminv/images/treeview/rack_open.gif::' ||
							location_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_locations
					   WHERE parent_id = l.location_id
						    AND collapse_child_nodes = 1
						    AND rownum < 2
					   UNION
					   SELECT 'Rack::' || location_id || '::' ||
							location_barcode ||
							'::/cheminv/images/treeview/icon_clsdfold3.gif::' ||
							location_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_locations
					   WHERE parent_id = l.location_id
						 AND location_barcode NOT IN (SELECT location_barcode
						 FROM inv_locations WHERE parent_id = l.location_id
						    AND collapse_child_nodes = 1
						    AND rownum < 2)) AS grid_data,
					  l.NAME,
					  l.location_id,
					  l.location_barcode,
					  l.row_index,
					  l.col_index,
					  l.row_name AS rowname,
					  l.col_name AS colname,
					  l.NAME,
					  l.sort_order,
					  GUIUTILS.GETRACKLOCATIONPATH(p.location_ID) AS location_name,
					  GUIUTILS.GETFULLRACKLOCATIONPATH(p.location_ID) AS location_name_full,
					  f.cell_naming
				FROM inv_vw_grid_location l,
					inv_locations p,
					inv_grid_storage s,
					inv_grid_format f
				WHERE l.parent_id = p.location_id
					 AND l.parent_id = s.location_id_fk
					 AND s.grid_format_id_fk = f.grid_format_id
					 AND l.parent_id = (l_locationId)
					 order by l.col_index;

		ELSE
			OPEN O_RS FOR
				SELECT (SELECT 'Container::' || container_id || '::' ||
							barcode ||
							'::/cheminv/images/listview/flask_closed_icon_16.gif::' ||
							container_name || '::' ||
							DECODE(inv_Containers.Qty_Max,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Max || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Qty_Remaining,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Remaining || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Qty_Available,
								  NULL,
								  ' ',
								  inv_Containers.Qty_Available || ' ' ||
								  UOM.Unit_Abreviation) || '::' ||
							DECODE(inv_Containers.Concentration,
								  NULL,
								  ' ',
								  inv_Containers.Concentration || ' ' ||
								  UOC.Unit_Abreviation) || '::' ||
							icb.batch_field_1 || '::' ||
							constants.cContainerBatchField1Display || '::' ||
							icb.batch_field_2 || '::' ||
							constants.cContainerBatchField2Display || '::' ||
							icb.batch_field_3 || '::' ||
							constants.cContainerBatchField3Display || '::' ||
							ivrb.RegName || '::' ||
							inv_Containers.Qty_Max || '::' ||
							inv_Containers.Qty_Remaining || '::' ||
							inv_Containers.Qty_Available || '::' ||
							inv_Containers.Concentration || '::' ||
							ivrb.RegNumber || '::' ||
							ivrb.BatchNumber
					   FROM inv_containers,		
						   inv_Units UOM,
						   inv_Units UOC,
						   inv_container_batches icb,
						   inv_vw_reg_batches ivrb
					   WHERE location_id_fk = l.location_id
						    AND inv_Containers.UNIT_OF_MEAS_ID_FK =
						    UOM.Unit_ID
						    AND inv_Containers.Unit_of_Conc_ID_FK =
						    UOC.Unit_ID(+)
						    AND
						    inv_containers.batch_id_fk = icb.batch_id(+)
						    AND inv_containers.reg_id_fk = ivrb.RegID(+)
						    AND inv_containers.batch_number_fk =
						    ivrb.BatchNumber(+)
						    AND rownum < 2
					   UNION
					   SELECT 'Plate::' || plate_id || '::' ||
							plate_barcode ||
							'::/cheminv/images/listview/Plate_icon_16.gif::' ||
							plate_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_plates
					   WHERE location_id_fk = l.location_id
						    AND rownum < 2
					   UNION
					   SELECT 'Rack::' || location_id || '::' ||
							location_barcode ||
							'::/cheminv/images/treeview/rack_open.gif::' ||
							location_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_locations
					   WHERE parent_id = l.location_id
						    AND collapse_child_nodes = 1
						    AND rownum < 2
					   UNION
					   SELECT 'Rack::' || location_id || '::' ||
							location_barcode ||
							'::/cheminv/images/treeview/icon_clsdfold3.gif::' ||
							location_name || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::' || '::'
					   FROM inv_locations
					   WHERE parent_id = l.location_id
						 AND location_barcode NOT IN (SELECT location_barcode
						 FROM inv_locations WHERE parent_id = l.location_id
						    AND collapse_child_nodes = 1
						    AND rownum < 2)) AS grid_data,
					  l.NAME,
					  l.location_id,
					  l.location_barcode,
					  l.row_index,
					  l.col_index,
					  l.row_name AS rowname,
					  l.col_name AS colname,
					  l.NAME,
					  l.sort_order,
					  GUIUTILS.GETRACKLOCATIONPATH(p.location_ID) AS location_name,
					  GUIUTILS.GETFULLRACKLOCATIONPATH(p.location_ID) AS location_name_full,
					  f.cell_naming
				FROM inv_vw_grid_location l,
					inv_locations p,
					inv_grid_storage s,
					inv_grid_format f
				WHERE l.parent_id = p.location_id
					 AND l.parent_id = s.location_id_fk
					 AND s.grid_format_id_fk = f.grid_format_id
					 AND l.parent_id = (p_LocationID)
					 order by l.col_index;

		END IF;
	END DISPLAYRACKGRID;

	PROCEDURE DISPLAYRACKGRIDCONTAINERS(p_LocationID inv_locations.location_id%TYPE,
								 p_regServer VARCHAR2,
								 O_RS OUT CURSOR_TYPE) AS
	BEGIN

		IF p_regServer = 'NULL' THEN
			OPEN O_RS FOR

				SELECT c.barcode AS Barcode,
					  c.qty_max ||
					  (SELECT uom.unit_abreviation
					   FROM inv_units uom
					   WHERE uom.unit_id = c.unit_of_meas_id_fk) AS ContainerSize,
					  c.concentration ||
					  (SELECT uom.unit_abreviation
					   FROM inv_units uom
					   WHERE uom.unit_id = c.unit_of_conc_id_fk) AS Concentration,
					  CASE
						  WHEN ((SELECT uom.unit_abreviation
							    FROM inv_units uom
							    WHERE uom.unit_id = c.unit_of_conc_id_fk) =
							  'mg/ml') AND
							  ((SELECT uom.unit_abreviation
							    FROM inv_units uom
							    WHERE uom.unit_id = c.unit_of_meas_id_fk) = 'ml') THEN
						   c.qty_max * c.concentration || 'mg'
						  ELSE
						   c.qty_max ||
						   (SELECT uom.unit_abreviation
						    FROM inv_units uom
						    WHERE uom.unit_id = c.unit_of_meas_id_fk)
					  END AS Amount,
					  c.container_name AS ContainerName,
					  igl.NAME AS GridName,
					  icb.batch_field_1 AS Batch_Field_1,
					  icb.batch_field_1 AS Batch_Field_2,
					  icb.batch_field_3 AS Batch_Field_3
				FROM inv_containers c,
					inv_vw_grid_location igl,
					inv_container_batches icb
				WHERE igl.parent_id = p_LocationID
					 AND igl.location_id = c.location_id_fk(+)
					 AND c.batch_id_fk = icb.batch_id(+)
				ORDER BY igl.sort_order;
		ELSE
			OPEN O_RS FOR
				SELECT c.barcode AS Barcode,
					  c.qty_max ||
					  (SELECT uom.unit_abreviation
					   FROM inv_units uom
					   WHERE uom.unit_id = c.unit_of_meas_id_fk) AS ContainerSize,
					  c.concentration ||
					  (SELECT uom.unit_abreviation
					   FROM inv_units uom
					   WHERE uom.unit_id = c.unit_of_conc_id_fk) AS Concentration,
					  CASE
						  WHEN ((SELECT uom.unit_abreviation
							    FROM inv_units uom
							    WHERE uom.unit_id = c.unit_of_conc_id_fk) =
							  'mg/ml') AND
							  ((SELECT uom.unit_abreviation
							    FROM inv_units uom
							    WHERE uom.unit_id = c.unit_of_meas_id_fk) = 'ml') THEN
						   c.qty_max * c.concentration || 'mg'
						  ELSE
						   c.qty_max ||
						   (SELECT uom.unit_abreviation
						    FROM inv_units uom
						    WHERE uom.unit_id = c.unit_of_meas_id_fk)
					  END AS Amount,
					  c.container_name AS ContainerName,
					  igl.NAME AS GridName,
					  icb.batch_field_1 AS Batch_Field_1,
					  icb.batch_field_1 AS Batch_Field_2,
					  icb.batch_field_3 AS Batch_Field_3,
					  ivrb.*
				FROM inv_containers c,
					inv_vw_grid_location igl,
					inv_vw_reg_batches ivrb,
					inv_container_batches icb
				WHERE igl.parent_id = p_LocationID
					 AND igl.location_id = c.location_id_fk(+)
					 AND c.batch_id_fk = icb.batch_id(+)
					 AND c.reg_id_fk = ivrb.RegID(+)
					 AND c.batch_number_fk = ivrb.BatchNumber(+)
				ORDER BY igl.sort_order;
		END IF;

	END DISPLAYRACKGRIDCONTAINERS;

	PROCEDURE SEARCHRACKS(p_BatchField1 inv_container_batches.batch_field_1%TYPE,
					  p_BatchField2 inv_container_batches.batch_field_2%TYPE,
					  p_BatchField3 inv_container_batches.batch_field_3%TYPE,
					  p_OpenPositions NUMBER,
					  p_ContainerSize inv_containers.qty_max%TYPE,
					  p_RestrictSize VARCHAR2,
					  p_ContainerSizeUOM inv_containers.unit_of_meas_id_fk%TYPE,
					  O_RS OUT CURSOR_TYPE) AS
		my_sql        VARCHAR2(3000);
		bAndPredicate BOOLEAN;
		l_Predicate   VARCHAR2(10);
		l_SizeCompare VARCHAR2(10);
	BEGIN
		bAndPredicate := FALSE;
		my_sql        := 'select
			 Racks.open_position_count(inv_locations.location_id) as OpenPositions
			 , Racks.NUMBEROFGRIDPOSITIONS(inv_locations.location_id, NULL)-Racks.open_position_count(inv_locations.location_id) as FilledPositions
			 , GUIUTILS.GETRACKLOCATIONPATH(inv_locations.location_id) as LocationPath
			 , GUIUTILS.GETFULLRACKLOCATIONPATH(inv_locations.location_id) as LocationPathFull
			 , location_id
			 , location_name
	   , (select gl.location_id || ''::'' || gl.name
		  from inv_vw_grid_location gl
		 where gl.parent_id = inv_locations.location_id
		   and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */
				 location_id_fk
				  from inv_containers c
				 where c.location_id_fk = gl.location_id)
		   and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */
				 location_id_fk
				  from inv_plates p
				 where p.location_id_fk = gl.location_id)
		   and not exists (select /*+ index(lr INV_LOCATION_PK) */
				 parent_id
				  from inv_locations l
				 where l.parent_id = gl.location_id
				   and collapse_child_nodes = 1)
		   and rownum = 1) as FirstOpenPosition
			 from inv_locations ';
		IF p_ContainerSize IS NOT NULL THEN
			IF NOT bAndPredicate THEN
      	my_sql := my_sql || ' where ';
			END IF;
			IF p_RestrictSize = 'on' THEN
				l_SizeCompare := '=';
			ELSE
				l_SizeCompare := '>=';
			END IF;
			my_sql        := my_sql ||
						  ' location_id in (
					select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
						where igll.parent_id = il.location_id
						and il.collapse_child_nodes = 1
						and igll.location_id in (select distinct location_id_fk from inv_containers where qty_max ' || '=' || ' ' ||
						  p_ContainerSize || ')
				)';
			bAndPredicate := TRUE;
		END IF;
		IF p_BatchField1 IS NOT NULL OR p_BatchField2 IS NOT NULL OR
		   p_BatchField3 IS NOT NULL THEN
			IF NOT bAndPredicate THEN
      	my_sql := my_sql || ' where ';
			END IF;
			IF bAndPredicate = FALSE THEN
				l_Predicate := '';
			ELSE
				l_Predicate := 'and';
			END IF;
			my_sql:= my_sql || l_Predicate ||
						  ' location_id in (
					select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
						where igll.parent_id = il.location_id
						and il.collapse_child_nodes = 1
						and igll.location_id in (select distinct ic.location_id_fk from inv_containers ic, inv_container_batches icb, inv_vw_reg_batches ivrb
							where icb.batch_id = ic.batch_id_fk							
							and icb.batch_field_1 like ''%' ||
						  p_BatchField1 || '%''
							or icb.batch_field_2 like ''%' ||
						  p_BatchField2 || '%''
						  or icb.batch_field_3 like ''%' ||
						  p_BatchField3 || '%'')
				)';
			bAndPredicate := TRUE;
		END IF;
		IF p_ContainerSizeUOM IS NOT NULL THEN
			IF NOT bAndPredicate THEN
      	my_sql := my_sql || ' where ';
			END IF;
			IF bAndPredicate = FALSE THEN
				l_Predicate := '';
			ELSE
				l_Predicate := 'and';
			END IF;
			my_sql        := my_sql || l_Predicate ||
						  ' location_id in (
					select distinct il.location_id from inv_vw_grid_location_lite igll, inv_locations il
						where igll.parent_id = il.location_id
						and il.collapse_child_nodes = 1
						and igll.location_id in (select distinct location_id_fk from inv_containers where unit_of_meas_id_fk ' || '=' || ' ' ||
						  p_ContainerSizeUOM || ')
				)';
			bAndPredicate := TRUE;
		END IF;
		IF p_OpenPositions IS NOT NULL THEN
			IF NOT bAndPredicate THEN
      	my_sql := my_sql || ' where ';
			END IF;
			IF bAndPredicate = FALSE THEN
				l_Predicate := '';
			ELSE
				l_Predicate := 'and';
			END IF;
			my_sql        := my_sql || l_Predicate ||
						  ' Racks.open_position_count(inv_locations.location_id) >= ' ||
						  p_OpenPositions;
			bAndPredicate := TRUE;
		END IF;
		IF NOT bAndPredicate THEN
      		my_sql := my_sql || ' where Racks.isRack(inv_locations.location_id) > 0';
		ELSE
			my_sql := my_sql || ' and Racks.isRack(inv_locations.location_id) > 0';
		END IF;

		my_sql := my_sql || ' order by location_name';

		OPEN O_RS FOR my_sql;

	END SEARCHRACKS;

	--' returns the number of containers in the grid, optionally from a given position within the grid
  FUNCTION NUMBEROFCONTAINERSINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		l_count VARCHAR2(10);
	BEGIN

		IF p_startingPosition IS NULL THEN
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_containers
  		WHERE location_id_fk IN
  			 (SELECT location_id
  			  FROM inv_vw_grid_location
  			  WHERE parent_id = p_LocationID);
		ELSE
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_containers
  		WHERE location_id_fk IN
  			 (SELECT location_id
  			  FROM inv_vw_grid_location
  			  WHERE parent_id = p_LocationID)
					AND location_id_fk >= p_startingPosition;
    END IF;

		RETURN l_count;

	END NUMBEROFCONTAINERSINGRID;

	--' returns the number of plates in the grid, optionally from a given position within the grid
	FUNCTION NUMBEROFPLATESINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		l_count VARCHAR2(10);
	BEGIN

		IF p_startingPosition IS NULL THEN
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_plates
  		WHERE location_id_fk IN
  			 (SELECT location_id
  			  FROM inv_vw_grid_location
  			  WHERE parent_id = p_LocationID);
		ELSE
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_plates
  		WHERE location_id_fk IN
  			 (SELECT location_id
  			  FROM inv_vw_grid_location
  			  WHERE parent_id = p_LocationID)
         	AND location_id_fk >= p_startingPosition;
    END IF;
		RETURN l_count;

	END NUMBEROFPLATESINGRID;

	--' returns the number of racks in the grid, optionally from a given position within the grid
	FUNCTION NUMBEROFRACKSINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		l_count VARCHAR2(10);
	BEGIN

		IF p_startingPosition IS NULL THEN
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_locations
  		WHERE parent_id IN (SELECT location_id
  						FROM inv_vw_grid_location
  						WHERE parent_id = p_LocationID);
		ELSE
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_locations
  		WHERE parent_id IN (SELECT location_id
  						FROM inv_vw_grid_location
  						WHERE parent_id = p_LocationID)
	         	AND location_id >= p_startingPosition;
    END IF;

		RETURN l_count;

	END NUMBEROFRACKSINGRID;

	--' given a rack location determines the number of open positions in the rack
  --' given a rack position determines the number of open positions in the rack from that position on
	FUNCTION open_position_count(p_LocationID inv_locations.location_id%TYPE)
		RETURN NUMBER IS
		l_count            NUMBER(8);
		l_numGridPositions NUMBER(8);
		l_numContainers    NUMBER(8);
		l_numPlates        NUMBER(8);
		l_numRacks         NUMBER(8);
    l_rackLocationId	inv_locations.location_id%TYPE;
    l_startingPosition inv_locations.location_id%TYPE;
	BEGIN
  	IF isRack(p_LocationID) = 1 THEN
    	l_rackLocationId := p_LocationID;
      l_startingPosition := NULL;
		ELSIF isRackLocation(p_LocationID) = 1 THEN
    	SELECT parent_id INTO l_rackLocationId FROM inv_locations WHERE location_id = p_LocationID;
      l_startingPosition := p_LocationID;
		ELSE
    	RETURN 0;
		END IF;

		l_numGridPositions := NUMBEROFGRIDPOSITIONS(l_rackLocationId, l_startingPosition);
    l_numContainers := NUMBEROFCONTAINERSINGRID(l_rackLocationId, l_startingPosition);
		l_numPlates := NUMBEROFPLATESINGRID(l_rackLocationId, l_startingPosition);
		l_numRacks := NUMBEROFRACKSINGRID(l_rackLocationId, l_startingPosition);
		l_count := l_numGridPositions - l_numContainers - l_numPlates - l_numRacks;
		IF l_count = '' OR l_count IS NULL THEN
			l_count := 0;
		END IF;

		RETURN l_count;

	END open_position_count;

	--' returns the number of positions in the grid, optionally from a given position within the grid
  FUNCTION NUMBEROFGRIDPOSITIONS(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN NUMBER IS
		l_count VARCHAR2(10);
	BEGIN

		IF p_startingPosition IS NULL THEN
  		SELECT row_count * col_count
  		INTO l_count
  		FROM inv_grid_storage igs, inv_grid_format igf
  		WHERE igs.grid_format_id_fk = igf.grid_format_id
  			 AND igs.location_id_fk = p_LocationiD;
		ELSE
			SELECT COUNT(*)
      INTO l_count
      FROM inv_grid_storage gs, inv_grid_element ge
			WHERE grid_storage_id = grid_storage_id_fk
					AND gs.location_id_fk = p_LocationID
    			AND ge.location_id_fk >= p_startingPosition;
    END IF;
		RETURN l_count;

	END NUMBEROFGRIDPOSITIONS;

	FUNCTION GETDEFAULTGRIDLOCATION(p_LocationID inv_locations.location_id%TYPE)
		RETURN VARCHAR2 IS
		l_value VARCHAR2(10);
	BEGIN

		SELECT MIN(location_id || '::' || NAME)
		INTO l_value
		FROM inv_vw_grid_location
		WHERE parent_id = p_LocationID
			 AND location_id NOT IN
			 (SELECT DISTINCT location_id_fk FROM inv_containers)
			 AND location_id NOT IN
			 (SELECT DISTINCT location_id_fk FROM inv_plates)
			 AND location_id NOT IN
			 (SELECT DISTINCT parent_id
				 FROM inv_locations
				 WHERE collapse_child_nodes = 1);

		RETURN l_value;

	END GETDEFAULTGRIDLOCATION;

	PROCEDURE REPORTINVALIDGRIDS(p_LocationID inv_locations.location_id%TYPE,
						    O_RS OUT CURSOR_TYPE) AS
	BEGIN

		OPEN O_RS FOR

			SELECT c.container_id AS container_id,
				  c.location_id_fk AS location_id_fk,
				  vl.NAME AS NAME
			FROM inv_containers c, inv_vw_grid_location vl
			WHERE c.location_id_fk = vl.location_id
				 AND c.location_id_fk IN
				 (SELECT location_id_fk
					 FROM inv_containers
					 WHERE location_id_fk IN
						  (SELECT location_id
						   FROM inv_vw_grid_location
						   WHERE parent_id = p_LocationID) HAVING
					  COUNT(*) > 1
					 GROUP BY location_id_fk);

	END REPORTINVALIDGRIDS;

	/*
     returns the location_id of the first empty position in a rack given the rack location_id
     */
	FUNCTION first_open_position(p_locationID inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
		RETURN inv_locations.location_id%TYPE IS
		l_firstOpen inv_locations.location_id%TYPE;
		l_containerId inv_containers.container_id%TYPE := 0;
	BEGIN
	IF p_containerId is NOT NULL THEN 
				l_containerId := p_containerId;
				END IF;				
		SELECT gl.location_id
		INTO l_firstOpen
		FROM inv_vw_grid_location gl
		WHERE gl.parent_id = p_locationID
			 AND NOT EXISTS (SELECT /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */
			   location_id_fk
			  FROM inv_containers c
			  WHERE c.location_id_fk = gl.location_id)
			 AND NOT EXISTS (SELECT /*+ index(p PLATE_LOCATION_ID_FK_IDX) */
			   location_id_fk
			  FROM inv_plates p
			  WHERE p.location_id_fk = gl.location_id)
			 AND NOT EXISTS (SELECT /*+ index(lr INV_LOCATION_PK) */
			   parent_id
			  FROM inv_locations l
			  WHERE gl.location_id = l.parent_id)
			 AND rownum = 1;

		RETURN l_firstOpen;
	END first_open_position;

	/*
	  determines whether location is a rack position
	*/

	FUNCTION isRackLocation(p_locationId inv_locations.location_id%TYPE)
		RETURN NUMBER IS
		l_count INT := 0;
    l_collapseChildNodes inv_locations.collapse_child_nodes%TYPE;
	BEGIN
		--' if this is a rack it isn't a rack location
  	SELECT decode(collapse_child_nodes,NULL,0,collapse_child_nodes) INTO l_collapseChildNodes FROM inv_locations WHERE location_id = p_locationId;
    IF l_collapseChildNodes = 0 THEN
  		SELECT COUNT(*)
  		INTO l_count
  		FROM inv_locations
  		WHERE collapse_child_nodes = 1
  			 AND
  			 location_id = (SELECT parent_id
  						 FROM inv_locations
  						 WHERE location_id = p_locationId);
		END IF;
		IF l_collapseChildNodes = 0 AND l_count > 0 THEN
			RETURN 1;
		ELSE
			RETURN 0;
		END IF;

	END isRackLocation;


	/*
	  determines whether the location is a rack as opposed to a rack position
	*/
	FUNCTION isRack(p_locationId inv_locations.location_id%TYPE)
  RETURN NUMBER IS
		l_count INT := 0;
  BEGIN
		SELECT COUNT(*)
		INTO l_count
		FROM inv_locations
		WHERE collapse_child_nodes = 1
			 AND
			 location_id = p_locationId;
		IF l_count > 0 THEN
			RETURN 1;
		ELSE
			RETURN 0;
		END IF;
  END isRack;

	/*
	  determines whether the rack position is filled
	*/
	FUNCTION isRackLocationFilled(p_locationId inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
	RETURN BOOLEAN IS
	l_count INT := 0;
	l_containerCount INT;
	l_plateCount INT;
	l_rackCount INT;
	l_containerId inv_containers.container_id%TYPE :=-1;
	l_plateId inv_plates.plate_id%TYPE :=-1;
	l_currLocationId inv_locations.location_id%TYPE:=-1;
	BEGIN
	
	IF p_containerId IS NOT NULL THEN l_containerId := p_containerId; END IF;
	IF p_plateId is NOT NULL THEN l_plateId := p_plateId; END IF;
	IF p_currLocationId IS NOT NULL THEN l_currLocationId := p_currLocationId; END IF;

	SELECT COUNT(*) INTO l_containerCount FROM inv_containers WHERE location_id_fk = p_locationId AND container_id <> l_containerId;
	SELECT COUNT(*) INTO l_plateCount FROM inv_plates WHERE location_id_fk = p_locationId AND plate_id <> l_plateId;
	SELECT COUNT(*) INTO l_rackCount FROM inv_locations WHERE parent_id = p_locationId AND location_id <> l_currLocationId;

	l_count := l_containerCount + l_plateCount + l_rackCount;

	IF l_count > 0 THEN
	   RETURN TRUE;
	ELSE
		RETURN FALSE;
	END IF;

	END isRackLocationFilled;

	/*
	  given a rack position id, finds the next open position in that rack
	*/
	FUNCTION getNextOpenPosition(p_locationId inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
	RETURN inv_locations.location_id%TYPE IS
		  l_parentId inv_locations.parent_id%TYPE;
		  l_nextOpen inv_locations.location_id%TYPE;
      l_count INT := 0;
	BEGIN
		--TODO: check that the input location id is a rack position and return an exception if it's not

    --' check the given rack position
    IF NOT isRackLocationFilled(p_locationId, p_containerId, p_plateId, p_currLocationId) THEN
    	l_nextOpen := p_locationId;
    ELSE
  		SELECT parent_id INTO l_parentId FROM inv_locations WHERE location_id = p_locationId;

  		--' check that this isn't the last open position
      SELECT COUNT(*) INTO l_count
      FROM inv_vw_grid_location gl
  		WHERE gl.parent_id = l_parentId
  			 AND gl.location_id > p_locationId
  			 AND NOT EXISTS (SELECT /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */
  			   location_id_fk
  			  FROM inv_containers c
  			  WHERE c.location_id_fk = gl.location_id)
  			 AND NOT EXISTS (SELECT /*+ index(p PLATE_LOCATION_ID_FK_IDX) */
  			   location_id_fk
  			  FROM inv_plates p
  			  WHERE p.location_id_fk = gl.location_id)
  			 AND NOT EXISTS (SELECT /*+ index(lr INV_LOCATION_PK) */
  			   parent_id
  			  FROM inv_locations l
  			  WHERE gl.location_id = l.parent_id)
  			 AND rownum = 1;

  		IF l_count > 0 THEN
    		SELECT gl.location_id
    		INTO l_nextOpen
    		FROM inv_vw_grid_location gl
    		WHERE gl.parent_id = l_parentId
    			 AND gl.location_id > p_locationId
    			 AND NOT EXISTS (SELECT /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */
    			   location_id_fk
    			  FROM inv_containers c
    			  WHERE c.location_id_fk = gl.location_id)
    			 AND NOT EXISTS (SELECT /*+ index(p PLATE_LOCATION_ID_FK_IDX) */
    			   location_id_fk
    			  FROM inv_plates p
    			  WHERE p.location_id_fk = gl.location_id)
    			 AND NOT EXISTS (SELECT /*+ index(lr INV_LOCATION_PK) */
    			   parent_id
    			  FROM inv_locations l
    			  WHERE gl.location_id = l.parent_id)
    			 AND rownum = 1;
  		ELSE
      	l_nextOpen := -1;
      END IF;
		END IF;

		RETURN l_nextOpen;
	END getNextOpenPosition;


	/*
  given a comma delimited list of racks or rack positions, return the next open position
  */
	FUNCTION multiGetNextOpenPosition(p_locationIds VARCHAR2)
  RETURN inv_locations.location_id%TYPE IS
	l_locationIds_t Stringutils.t_char;
  l_count INT;
  l_rackPos inv_locations.location_id%TYPE;
  l_nextOpen inv_locations.location_id%TYPE;
  l_temp INT;
  BEGIN
  		l_locationIds_t := STRINGUTILS.split(p_locationIds,',');

      FOR i IN l_locationIds_t.FIRST..l_locationIds_t.LAST
      LOOP
      	--' check to see if this is a rack
        l_temp := length(l_locationIds_t(i));
        IF (isRack(l_locationIds_t(i)) = 1) THEN
					l_nextOpen := first_open_position(l_locationIds_t(i), NULL, NULL, NULL);
				ELSE
          l_nextOpen := getNextOpenPosition(l_locationIds_t(i), NULL, NULL, NULL);
        END IF;
        IF l_nextOpen <> -1 THEN
        	RETURN l_nextOpen;
        END IF;
      END LOOP;

  END multiGetNextOpenPosition;


	/*
  given a comma delimited list of racks or rack positions, return the number of open positions
  */
	FUNCTION multiOpenPositionCount(p_locationIds VARCHAR2)
  RETURN NUMBER IS
	l_locationIds_t Stringutils.t_char;
  l_count NUMBER := 0;
  BEGIN
  		l_locationIds_t := STRINGUTILS.split(p_locationIds,',');

      FOR i IN l_locationIds_t.FIRST..l_locationIds_t.LAST
      LOOP
				l_count := l_count + open_position_count(l_locationIds_t(i));
      END LOOP;

      RETURN l_count;

  END multiOpenPositionCount;

END RACKS;
/
show errors;