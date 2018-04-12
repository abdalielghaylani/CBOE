CREATE OR REPLACE FUNCTION SETPLATEFTCYCLE(
	p_plateIds IN VARCHAR2,
	p_oldLocationId INV_LOCATIONS.LOCATION_ID%TYPE,
	p_newLocationId INV_LOCATIONS.LOCATION_ID%TYPE)
	RETURN INTEGER IS

	l_oldIsFrozen INTEGER;
	l_newIsFrozen INTEGER;
     l_ftIncrement INT;
     l_plateIds_t stringutils.t_char;
     
BEGIN

	-- determine if freeze/thaw should be incremented
	l_oldIsFrozen := Isfrozenlocation(p_oldLocationId);
	l_newIsFrozen := Isfrozenlocation(p_newLocationId);

	IF l_oldIsFrozen = 1 AND l_newIsFrozen = 0 THEN
	   	 l_ftIncrement := 1;
	ELSE
		l_ftIncrement := 0;
	END IF;

	-- update plates
	l_plateIds_t := stringutils.split(p_plateIds, ',');
     FORALL i IN l_plateIds_t.FIRST..l_plateIds_t.LAST
          UPDATE INV_PLATES SET ft_cycles = decode(ft_cycles,null,decode(l_ftIncrement,0,NULL,l_ftIncrement),ft_cycles + l_ftIncrement) WHERE plate_id = l_plateIds_t(i);

	RETURN l_ftIncrement;

END SETPLATEFTCYCLE;
/
show errors;

