create or replace function
"DELETEPLATE"
	(p_plate_id in inv_plates.plate_id%type)
	return inv_plates.plate_id%type
is
begin
	-- delete from wells
	delete from inv_wells where plate_id_fk = p_plate_id;
	-- delete from plates
	delete from inv_plates where plate_id = p_plate_id;

	return p_plate_id;
end DeletePlate;
/
show errors;