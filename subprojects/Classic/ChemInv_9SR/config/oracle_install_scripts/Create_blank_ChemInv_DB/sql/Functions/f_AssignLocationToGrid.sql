create or replace function
"ASSIGNLOCATIONTOGRID"
	(p_location_id in inv_locations.location_id%type,
	 p_grid_element_id in inv_grid_element.grid_element_id%type)
	return inv_grid_element.grid_element_id%type
is
begin
	update inv_grid_element
	set location_id_fk = p_location_id
	where grid_element_id = p_grid_element_id;

	return p_grid_element_id;
end AssignLocationToGrid;
/
show errors;