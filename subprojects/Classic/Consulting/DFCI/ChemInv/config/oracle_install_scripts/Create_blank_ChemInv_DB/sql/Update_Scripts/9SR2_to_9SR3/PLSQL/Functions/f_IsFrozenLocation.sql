create or replace function 
"ISFROZENLOCATION"
	(p_location_id in inv_locations.location_id%type)
	return integer
is
l_numFreezers INT;
BEGIN
	--' find the number of freezers in this location's hierarchy
	--' 10 - freezer, 25 -- ultra-freezer
	SELECT COUNT(*) INTO l_numFreezers 
     	FROM inv_locations 
          WHERE 
          	location_type_id_fk = 10 OR location_type_id_fk = 25 
         	START WITH location_id = p_location_id
          CONNECT BY PRIOR parent_id = location_id;	
     
	if l_numFreezers > 0 then
		return 1;
	else
		return 0;
	end if;
end IsFrozenLocation;
/
show errors;

