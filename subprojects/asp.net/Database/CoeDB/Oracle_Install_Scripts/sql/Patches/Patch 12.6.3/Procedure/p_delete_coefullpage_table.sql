prompt 
prompt Starting "p_delete_coefullpage_table.sql"...
prompt 

create or replace procedure  delete_coefullpage_table(i_user_id varchar2) as
	PRAGMA AUTONOMOUS_TRANSACTION;
    l_p_name varchar2(10);
    l_date date;
    cnt number :=5;
    l_Part number;
begin 
    --check license for partitions 
    SELECT DECODE(Value, 'TRUE', 1, 0) into l_Part
     FROM V$Option WHERE parameter = 'Partitioning';
     
    if l_Part = 1 then
        l_p_name := get_current_part_name(i_user_id);
		if l_p_name != 'NOT EXIST' then
			for i in 1..cnt loop
			  l_date:=sysdate;
				begin
		--        dbms_output.put_line('ALTER TABLE COEFULLPAGE TRUNCATE PARTITION '||l_p_name||' UPDATE INDEXES');
					execute immediate 'ALTER TABLE COEFULLPAGE TRUNCATE PARTITION '||l_p_name||' UPDATE INDEXES';
					exit;
				exception when others then
				loop    
		--         dbms_output.put_line(to_char(l_date+(1/17280),'yyyymmddhh24miss')||','||to_char(sysdate,'yyyymmddhh24miss'));
				exit when (l_date+(1/17280))<=sysdate;
				end loop; 
				if i = cnt then raise; end if;
				end;
			end loop;
		end if;
    else
       delete from COEFULLPAGE where  UPPER(CLIENT_ID) = UPPER(i_user_id);
       commit;
    end if;
end;
/
