create or replace function
"GETNUMBEROFCOMPOUNDWELLS"
	(p_plate_format_id in inv_plate_format.plate_format_id%type)
	return integer
is
num_compound_wells integer;
begin

	-- 1 is the predefined constant for compound wells
	select count(well_id) into num_compound_wells
	from inv_vw_well_format
	where plate_format_id_fk = p_plate_format_id
	and well_format_id_fk = 1;

	return num_compound_wells;
end GetNumberOfCompoundWells;
/
show errors;