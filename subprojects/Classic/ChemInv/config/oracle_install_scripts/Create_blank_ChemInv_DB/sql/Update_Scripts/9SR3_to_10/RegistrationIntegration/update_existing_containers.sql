create global temporary table temp_container_compounds
(
  reg_id_fk number(9,0),
  batch_number_fk number(9,0),
  compound_id_fk number(9,0)
)
on commit delete rows;

declare
l_CompoundID inv_compounds.compound_id%TYPE;
l_RegIDFK inv_containers.reg_id_fk%TYPE;
l_BatchNumberFK inv_containers.batch_number_fk%TYPE;

cursor TEMP_RS is 
  select reg_id_fk, batch_number_fk from temp_container_compounds
  for update;

begin
  insert into temp_container_compounds(reg_id_fk, batch_number_fk, compound_id_fk)
  select reg_id_fk, batch_number_fk, null as compound_id_fk
  from inv_containers
  where reg_id_fk is not null and batch_number_fk is not null
  group by reg_id_fk, batch_number_fk;
  
  open TEMP_RS;
  loop
    fetch TEMP_RS into l_RegIDFK, l_BatchNumberFK;
      exit when TEMP_RS%NOTFOUND or TEMP_RS%NOTFOUND is null;
      l_CompoundID := CREATEREGCOMPOUND( l_RegIDFK, l_BatchNumberFK );
      update temp_container_compounds
      set compound_id_fk = l_CompoundID
      where current of TEMP_RS;    
  end loop;
  close TEMP_RS;
  
  update inv_containers ic
  set ic.compound_id_fk = (
    select compound_id_fk
    from temp_container_compounds tcc
    where ic.reg_id_fk = tcc.reg_id_fk
    and ic.batch_number_fk = tcc.batch_number_fk
  ), reg_id_fk = null, batch_number_fk = null
  where ic.reg_id_fk is not null
  and ic.batch_number_fk is not null;
end;
/

drop table temp_container_compounds;
