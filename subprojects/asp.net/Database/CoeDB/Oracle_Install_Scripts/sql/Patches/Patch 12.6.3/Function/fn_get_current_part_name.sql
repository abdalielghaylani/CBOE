prompt 
prompt Starting "fn_get_current_part_name.sql"...
prompt 

create or replace function get_current_part_name(i_user_id varchar2)
  return varchar2 is
   PRAGMA AUTONOMOUS_TRANSACTION;
   l_p_name varchar2(10);
   cnt number;
begin
    insert into for_determ_part_name
     select partition_name, TO_LOB(high_value) as high_value
        from user_tab_partitions t where t.table_name='COEFULLPAGE';
    
    select count(1) into cnt
        from for_determ_part_name where (high_value) like q'[']'||UPPER(i_user_id)||q'[']';
    if cnt > 0 then 
        select partition_name into l_p_name
            from for_determ_part_name where (high_value) like q'[']'||UPPER(i_user_id)||q'[']';
    else
        l_p_name:='NOT EXIST';
    end if;
    rollback;
    return l_p_name;
end;
/
