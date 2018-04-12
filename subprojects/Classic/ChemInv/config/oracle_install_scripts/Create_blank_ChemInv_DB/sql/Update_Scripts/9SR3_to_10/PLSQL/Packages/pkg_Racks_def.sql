CREATE OR REPLACE PACKAGE RACKS IS
	TYPE CURSOR_TYPE IS REF CURSOR;

	-- Public function and procedure declarations
	PROCEDURE DISPLAYRACKGRID(p_LocationID inv_locations.location_id%TYPE,
						 p_regServer VARCHAR2,
						 O_RS OUT CURSOR_TYPE);

	PROCEDURE DISPLAYRACKGRIDCONTAINERS(p_LocationID inv_locations.location_id%TYPE,
								 p_regServer VARCHAR2,
								 O_RS OUT CURSOR_TYPE);

	PROCEDURE SEARCHRACKS(p_BatchField1 inv_container_batches.batch_field_1%TYPE,
					  p_BatchField2 inv_container_batches.batch_field_2%TYPE,
					  p_BatchField3 inv_container_batches.batch_field_3%TYPE,
					  p_OpenPositions NUMBER,
					  p_ContainerSize inv_containers.qty_max%TYPE,
					  p_RestrictSize VARCHAR2,
					  p_ContainerSizeUOM inv_containers.unit_of_meas_id_fk%TYPE,
					  O_RS OUT CURSOR_TYPE);

  FUNCTION NUMBEROFCONTAINERSINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION NUMBEROFPLATESINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION NUMBEROFRACKSINGRID(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION open_position_count(p_LocationID inv_locations.location_id%TYPE)
		RETURN NUMBER;

  FUNCTION NUMBEROFGRIDPOSITIONS(p_LocationID inv_locations.location_id%TYPE, p_startingPosition inv_locations.location_id%TYPE)
		RETURN NUMBER;

	FUNCTION GETDEFAULTGRIDLOCATION(p_LocationID inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

--	FUNCTION first_open_position(p_locationID inv_locations.location_id%TYPE)
--		RETURN inv_locations.location_id%TYPE;

FUNCTION first_open_position(p_locationID inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
	RETURN inv_locations.location_id%TYPE;
	
	PROCEDURE REPORTINVALIDGRIDS(p_LocationID inv_locations.location_id%TYPE,
						    O_RS OUT CURSOR_TYPE);

	FUNCTION isRackLocation(p_locationId inv_locations.location_id%TYPE)
		RETURN NUMBER;

	FUNCTION isRack(p_locationId inv_locations.location_id%TYPE)
  RETURN NUMBER;

	--FUNCTION isRackLocationFilled(p_locationId inv_locations.location_id%TYPE)
--	RETURN BOOLEAN;
FUNCTION isRackLocationFilled(p_locationId inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
	RETURN BOOLEAN;
	
	FUNCTION multiGetNextOpenPosition(p_locationIds VARCHAR2)
  RETURN inv_locations.location_id%TYPE;

--	FUNCTION getNextOpenPosition(p_locationId inv_locations.location_id%TYPE)
--	RETURN inv_locations.location_id%TYPE;
FUNCTION getNextOpenPosition(p_locationId inv_locations.location_id%TYPE, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
	RETURN inv_locations.location_id%TYPE;
	

	FUNCTION multiOpenPositionCount(p_locationIds VARCHAR2)
  RETURN NUMBER;


END RACKS;
/

show errors;