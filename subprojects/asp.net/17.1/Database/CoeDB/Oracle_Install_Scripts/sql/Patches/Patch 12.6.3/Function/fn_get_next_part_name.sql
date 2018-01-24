prompt 
prompt Starting "fn_get_next_part_name.sql"...
prompt 

create or replace function get_next_part_name
  return varchar2 is
  str varchar2(32000);

  begin
  select max(num)+1  into str from (
  select  to_number(substr(partition_name,2)) num
    from user_tab_partitions
   where table_name='COEFULLPAGE');
  
    return str;
  end;
  /
