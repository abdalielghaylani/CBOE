create or replace function
"DELETEPLATEFORMAT"
	(pPlateFormatId In inv_plate_format.plate_format_id%type)
	return inv_plate_format.plate_format_id%type

is

begin

	-- cascade delete
	delete from inv_wells where plate_format_id_fk = pPlateFormatId;
	delete from inv_plate_format where plate_format_id = pPlateFormatId;

return pPlateFormatId;

end DeletePlateFormat;
/
show errors;