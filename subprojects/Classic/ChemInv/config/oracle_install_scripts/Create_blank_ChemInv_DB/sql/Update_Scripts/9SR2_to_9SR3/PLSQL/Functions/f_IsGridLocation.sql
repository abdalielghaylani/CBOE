create or replace function
"ISGRIDLOCATION"
	(p_location_id in inv_locations.location_id%type)
	return integer
is
rec_count integer;
begin
	select count(*) into rec_count from inv_vw_grid_location_lite where location_id = p_location_id;
	if rec_count = 0 then
		return 0;
	else
		return 1;
	end if;
end IsGridLocation;
/
show errors;