CREATE OR REPLACE PACKAGE BODY RLS
AS
  FUNCTION SetPredicate (
  	p_schema VARCHAR2,
    p_name VARCHAR2) RETURN VARCHAR2
  IS
    l_username  VARCHAR2(50);
    l_predicate VARCHAR2(2000);
				l_roles VARCHAR2(200);
				l_roles_t stringutils.t_char;
  BEGIN
    -- you can also get the user-defined context variables here
    l_username := sys_context('USERENV', 'SESSION_USER');
				
		CASE p_name
    	WHEN 'INV_LOCATIONS' THEN
      	l_predicate := 'LOCATION_ID ';
      ELSE
      	l_predicate := 'LOCATION_ID_FK ';
    END CASE;
   
				l_roles := sys_context('CTX_Cheminv','roleIDs');
				IF l_roles IS NOT NULL AND length(l_roles) > 0 THEN
				
    				l_roles_t := stringUtils.split(l_roles, ',');
    				
        l_predicate := l_predicate || 'NOT IN (SELECT location_id_fk FROM inv_role_locations WHERE role_id_fk in ( ';
    				FOR i  IN l_roles_t.FIRST..l_roles_t.LAST
    				LOOP
    							l_predicate := l_predicate || 		l_roles_t(i) || ',';
    				END LOOP;
								l_predicate := TRIM(',' FROM l_predicate) || '))';
				ELSE 
									l_predicate := '';
									
				END IF;
				
    RETURN   l_predicate;
  END SetPredicate;
END RLS;

/
show errors;