create or replace function
"ISFROZENLOCATION"
	(p_location_id in inv_locations.location_id%type)
	return integer
is 
b_grid_location integer;
curr_location_id integer;
l_location_type_id integer;
begin
	-- go up until we hit a non-grid location
	b_grid_location := 0;
	curr_location_id := p_location_id;
	loop
		b_grid_location := IsGridLocation(curr_location_id);
		if b_grid_location = 0 then
			exit;
		else
			select parent_id into curr_location_id 
			from inv_vw_grid_location_lite 
			where location_id = curr_location_id;
		end if;
	end loop;

	-- see if it's a frozen location
	-- 10 - freezer, 25 -- ultra-freezer
	select location_type_id_fk into l_location_type_id 
	from inv_locations 
	where location_id = curr_location_id;
	
	if l_location_type_id = 10 or l_location_type_id = 25 then
		return 1;
	else
		return 0;
	end if;
end IsFrozenLocation;
/

