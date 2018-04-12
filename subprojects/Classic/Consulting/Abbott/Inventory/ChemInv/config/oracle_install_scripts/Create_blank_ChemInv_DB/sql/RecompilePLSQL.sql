DECLARE
	cursor cur_invalid_objects is
		select object_name, object_type from user_objects where status='INVALID';
	rec_columns cur_invalid_objects%ROWTYPE;
	err_status NUMERIC;
BEGIN
	dbms_output.enable(10000);
	open cur_invalid_objects;
	loop

		fetch cur_invalid_objects into rec_columns;
		EXIT WHEN cur_invalid_objects%NOTFOUND;

		--dbms_output.put_line ('Recompiling ' || rec_columns.object_type || '  ' || rec_columns.object_name);
		dbms_ddl.alter_compile(rec_columns.object_type,NULL,rec_columns.object_name);

	end loop;
	close cur_invalid_objects;

	EXCEPTION
	When others then
		begin
			err_status := SQLCODE;
			--dbms_output.put_line(' Recompilation failed : ' || SQLERRM(err_status));
			if ( cur_invalid_objects%ISOPEN) then
				CLOSE cur_invalid_objects;
			end if;
	
		exception when others then
		null;
	end;
end;
/