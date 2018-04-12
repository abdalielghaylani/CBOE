create or replace function
"UPDATEPLATEFORMAT"
	(pPlateFormatId IN inv_Plate_format.plate_format_id%type,
	 pPlateFormatName IN inv_plate_format.plate_format_name%type,
	 pPhysPlateIdFk In inv_plate_format.phys_plate_id_fk%type)

	 return inv_plate_format.plate_format_id%type
	
is

OldPhysPlateId inv_plate_format.phys_plate_id_fk%type;
GridFormatId inv_grid_format.grid_format_id%type;
cursor grid_positions is
	select * from inv_grid_position where grid_format_id_fk = GridFormatId;

begin

	-- get old physplate id
	select phys_plate_id_fk into OldPhysPlateID from inv_plate_format where plate_format_id = pPlateFormatId;
	-- insert new values into inv_plate_format
	update inv_plate_format 
	set 
		plate_format_name = pPlateFormatName,
		phys_plate_id_fk = pPhysPlateIDFK
	where
		plate_format_id = pPlateFormatId;

	if not OldPhysPlateId = pPhysPlateIDFK then
		-- delete old wells
		delete from inv_wells where plate_format_id_fk = pPlateFormatId;

		-- create as many new wells as necessary

		-- get grid format id
		select grid_format_id_fk into GridFormatId 
		from inv_physical_plate
		where phys_plate_id = pPhysPlateIdFK;
		
		-- insert into inv_wells
		for grid_positions_cur in grid_positions loop
			insert into inv_wells
			(plate_format_id_fk, grid_position_id_fk, well_format_id_fk)
			values 
			(pPlateFormatId, grid_positions_cur.grid_position_id, 4);
		end loop;

	end if;



return pPlateFormatId;

end UpdatePlateFormat;
/
