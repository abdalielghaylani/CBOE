prompt 
prompt Starting "error_exit.sql"...
prompt

BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');

	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/

prompt logged session to: sql/log_create_coedb_ora.txt
spool off

exit