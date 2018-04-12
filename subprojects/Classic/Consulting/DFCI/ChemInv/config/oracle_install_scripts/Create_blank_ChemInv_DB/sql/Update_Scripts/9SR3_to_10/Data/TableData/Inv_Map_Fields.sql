DECLARE
	x number(9,0) := 0;
BEGIN
	select count(*) into x from inv_map_fields where display_name = 'Solvent Dilution Volume';
	if (x = 0) then
		insert into inv_map_fields( map_field_id, display_name, table_name, column_name )
		values( 32, 'Solvent Dilution Volume', null, null );
	end if;
END;
/
commit;