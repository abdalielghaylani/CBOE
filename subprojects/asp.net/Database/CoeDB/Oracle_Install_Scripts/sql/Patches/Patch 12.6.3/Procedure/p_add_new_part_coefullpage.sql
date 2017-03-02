prompt 
prompt Starting "p_add_new_part_coefullpage.sql"...
prompt 

create or replace procedure add_new_part_coefullpage(i_user_id varchar2) as
   PRAGMA AUTONOMOUS_TRANSACTION;
l_p_name varchar2(10);
str varchar2(5000);
    l_Part number;
begin 
    --check license for partitions 
    SELECT DECODE(Value, 'TRUE', 1, 0) into l_Part
     FROM V$Option WHERE parameter = 'Partitioning';
     
    if l_Part = 1 then
     --check value
     l_p_name := get_current_part_name(i_user_id);
     if l_p_name = 'NOT EXIST' then
-- find next partition name
         l_p_name :=  'd'||coedb.get_next_part_name;
        -- add new partition
        str:= 'ALTER TABLE COEDB.COEFULLPAGE ADD PARTITION '||l_p_name||q'[ VALUES( ']'||upper(i_user_id)||q'[')]';
        execute immediate(str );
     end if;
	end if;
--    exception when others then dbms_output.put_line(str); 
end;
/
