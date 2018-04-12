CREATE OR REPLACE PACKAGE RACKS IS
	TYPE  CURSOR_TYPE IS REF CURSOR;

  -- Public function and procedure declarations
  PROCEDURE DISPLAYRACKGRID(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
  );

  PROCEDURE DISPLAYRACKGRIDCONTAINERS(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
  );

  PROCEDURE SEARCHRACKS (
    p_BatchField1 inv_container_batches.batch_field_1%TYPE,
    p_BatchField2 inv_container_batches.batch_field_2%TYPE,
    p_BatchField3 inv_container_batches.batch_field_3%TYPE,
    p_OpenPositions NUMBER,
    p_ContainerSize inv_containers.qty_max%TYPE,
    p_RestrictSize VARCHAR2,
    p_ContainerSizeUOM inv_containers.unit_of_meas_id_fk%TYPE,
    O_RS OUT CURSOR_TYPE
  );

  FUNCTION NUMBEROFCONTAINERSINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2;

  FUNCTION NUMBEROFPLATESINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2;

  FUNCTION NUMBEROFRACKSINGRID(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2;

  FUNCTION NUMBEROFOPENGRIDS(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER;

  FUNCTION NUMBEROFOPENGRIDS2(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER;

  FUNCTION NUMBEROFGRIDPOSITIONS (
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN NUMBER;

  FUNCTION GETDEFAULTGRIDLOCATION(
    p_LocationID inv_locations.location_id%TYPE
  ) RETURN VARCHAR2;

  PROCEDURE REPORTINVALIDGRIDS(
    p_LocationID inv_locations.location_id%TYPE,
    O_RS OUT CURSOR_TYPE
  );

END RACKS;



/

show errors;