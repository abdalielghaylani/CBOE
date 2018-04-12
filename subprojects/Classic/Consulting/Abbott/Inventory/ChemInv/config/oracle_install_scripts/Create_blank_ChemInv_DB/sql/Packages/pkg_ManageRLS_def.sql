CREATE OR REPLACE PACKAGE ManageRLS
AS

	TYPE CURSOR_TYPE IS REF CURSOR;

  PROCEDURE UpdateRoleLocations(
  	p_roleID inv_role_locations.role_id_fk%TYPE,
    p_locationIDs VARCHAR2,
    p_propagate NUMBER);

	PROCEDURE GetRoleLocations(
		p_roleID IN inv_role_locations.role_id_fk%TYPE,
		O_RS OUT CURSOR_TYPE);

END ManageRLS;

/

show errors;