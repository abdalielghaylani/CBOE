create or replace function 

"CREATEPLATEFORMAT"
	(pPlateFormatName IN inv_plate_format.plate_format_name%type,
	 pPhysPlateIdFk In inv_plate_format.phys_plate_id_fk%type)

	 return inv_plate_format.plate_format_id%type
	
is

NewPlateFormatId inv_plate_format.plate_format_id%type;
GridFormatId inv_grid_format.grid_format_id%type;
cursor grid_positions is
	select * from inv_grid_position where grid_format_id_fk = GridFormatId;

begin

	-- insert new values into inv_plate_format
	insert into inv_plate_format
	(plate_format_name, phys_plate_id_fk)
	values
	(pPlateFormatName, pPhysPlateIdFk)
	returning plate_format_id into NewPlateFormatId;

	-- create as many new wells as necessary

	-- get grid format id
	select grid_format_id_fk into GridFormatId 
	from inv_physical_plate
	where phys_plate_id = pPhysPlateIdFK;
	
	-- insert into inv_wells
	-- 1 is the value for a compound well
	for grid_positions_cur in grid_positions loop
		insert into inv_wells
		(plate_format_id_fk, grid_position_id_fk, well_format_id_fk)
		values 
		(NewPlateFormatID, grid_positions_cur.grid_position_id, 1);
	end loop;

return NewPlateFormatId;

end CreatePlateFormat;
/

show errors;