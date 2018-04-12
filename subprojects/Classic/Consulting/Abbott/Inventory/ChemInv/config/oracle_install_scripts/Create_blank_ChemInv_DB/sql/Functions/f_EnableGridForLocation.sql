create or replace function "ENABLEGRIDFORLOCATION"
	(p_location_id in inv_locations.location_id%type,
	 p_grid_format_id in inv_grid_format.grid_format_id%type)

	 return inv_grid_storage.grid_storage_id%type

is

new_grid_storage_id inv_grid_storage.grid_storage_id%type;
new_location_id inv_locations.location_id%type;
parent_location_name inv_locations.location_name%type;
l_NameDelimeter inv_grid_format.name_delimeter%type;
cursor grid_positions is
	select * from inv_grid_position where grid_format_id_fk = p_grid_format_id;

begin
	-- get parent name
	select location_name into parent_location_name
	from inv_locations where location_id = p_location_id;

  -- get naming delimeter
  select name_delimeter into l_NameDelimeter
	from inv_grid_format where grid_format_id = p_grid_format_id;
  if Length(l_NameDelimeter) = 0 then
     l_NameDelimeter := '/';
  end if;

	-- insert into grid storage
	insert into inv_grid_storage
	(grid_format_id_fk, location_id_fk)
	values
	(p_grid_format_id, p_location_id)
	returning grid_storage_id into new_grid_storage_id;

	-- insert new locations and grid elements
	for grid_positions_cur in grid_positions loop
		-- insert into locations
		insert into inv_locations
		(parent_id, location_name, location_type_id_fk)
		values
		(p_location_id, parent_location_name || l_NameDelimeter || grid_positions_cur.name, 0)
		returning location_id into new_location_id;
		-- insert into grid elements
		insert into inv_grid_element
		(grid_position_id_fk, grid_storage_id_fk, location_id_fk)
		values
		(grid_positions_cur.grid_position_id, new_grid_storage_id, new_location_id);
	end loop;

	return new_grid_storage_id;
end EnableGridForLocation;



/
show errors;
