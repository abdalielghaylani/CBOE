CREATE OR REPLACE PACKAGE BODY ManageRLS
AS

  PROCEDURE UpdateRoleLocations(
  	p_roleID inv_role_locations.role_id_fk%TYPE,
    p_locationIDs VARCHAR2,
    p_propagate NUMBER)
  IS
  	l_locationIDs_t stringutils.t_char;
  BEGIN
  	--' split comma delimited list of location_ids
    l_locationIDs_t := stringUtils.split(p_locationIDs);

    --' delete existing excluded locations for this user
    DELETE inv_role_locations WHERE role_id_fk = p_roleID;

		--' only insert if location ids were passed in
		IF l_locationIDs_t.FIRST IS NOT NULL THEN
      --' add a row to inv_user_locations for each location_id in the list
     	IF p_propagate = 1 THEN
       	--' propagate to child locations
        FORALL i IN l_locationIDs_t.FIRST..l_locationIDs_t.LAST
			INSERT INTO inv_role_locations SELECT p_roleID, location_id FROM inv_locations WHERE location_id NOT IN (SELECT location_id_fk FROM inv_role_locations WHERE role_id_fk = p_roleID) CONNECT BY PRIOR location_id = parent_id START WITH location_id =  l_locationIDs_t(i);  		  	
      ELSE
       	--' don't propagate
        FORALL i IN l_locationIDs_t.FIRST..l_locationIDs_t.LAST
  		  	INSERT INTO inv_role_locations VALUES (p_roleID, l_locationIDs_t(i));
      END IF; 
	END IF;
  END;

	PROCEDURE GetRoleLocations(
		p_roleID IN inv_role_locations.role_id_fk%TYPE,
		O_RS OUT CURSOR_TYPE)
  IS

  BEGIN
		OPEN O_RS FOR
			SELECT * FROM inv_role_locations WHERE role_id_fk = p_roleID;
  END;


END ManageRLS;

/

show errors;