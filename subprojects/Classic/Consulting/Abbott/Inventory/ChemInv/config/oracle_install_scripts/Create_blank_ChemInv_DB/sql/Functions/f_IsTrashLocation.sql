CREATE OR REPLACE
function
"ISTRASHLOCATION"
	(pLocationID in inv_locations.location_id%type)
	return integer
is
rec_count integer;
begin
	select count(*) into rec_count from inv_locations where location_id = pLocationID and location_type_id_fk = constants.cTrashCanLocType;
	if rec_count = 0 then
		return 0;
	else
		return 1;
	end if;
end IsTrashLocation;
/
